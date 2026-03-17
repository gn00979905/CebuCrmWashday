using Microsoft.AspNetCore.Authentication.Cookies; // 引用 Cookie 驗證
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using WASHDAY_202508.Data;
using Microsoft.AspNetCore.HttpOverrides; // 1. 在檔案最上面加入這行

namespace WASHDAY_202508
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            // 加入這段來設定資料庫連線
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
            // ===== 新增：設定 Cookie 驗證服務 =====
            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = "/Login"; // 當使用者未登入時，要跳轉到哪個頁面
                    options.AccessDeniedPath = "/AccessDenied"; // 當權限不足時的頁面 (可選)
                });
            // ===================================
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlite(connectionString));


            // Add services to the container.
            builder.Services.AddRazorPages();
            builder.Services.AddControllers(); // <--- 1. 新增這行：註冊 API 控制器服務

            var app = builder.Build();

            // ===== 關鍵修正：強制信任總機 (YARP) 傳來的網址 =====
            var forwardedHeadersOptions = new ForwardedHeadersOptions
            {
                // 我們需要 Protocol(https) 和 Host(washday-202508.onrender.com)
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto | ForwardedHeaders.XForwardedHost
            };

            // 清除預設限制，明確信任來自同一個房間 (127.0.0.1) 的總機
            forwardedHeadersOptions.KnownNetworks.Clear();
            forwardedHeadersOptions.KnownProxies.Clear();
            forwardedHeadersOptions.KnownProxies.Add(System.Net.IPAddress.Parse("127.0.0.1"));

            // 1. 最重要的一步：把設定傳進去，而且一定要放在最前面！
            app.UseForwardedHeaders(forwardedHeadersOptions);
            // ===================================================

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            // 2. HttpsRedirection 必須放在 ForwardedHeaders 之後
            app.UseHttpsRedirection();
            
            app.UseStaticFiles();

            app.UseRouting();
            // ===== 新增：設定應用程式的預設文化 =====
            var supportedCultures = new[] { new CultureInfo("en-US") };
            var localizationOptions = new RequestLocalizationOptions
            {
                DefaultRequestCulture = new RequestCulture("en-US"),
                SupportedCultures = supportedCultures,
                SupportedUICultures = supportedCultures
            };
            // 順序很重要，將它放在 UseRouting 之後
            app.UseRequestLocalization(localizationOptions);
            // ===================================

            // ===== 新增：啟用驗證和授權中介軟體 =====
            // 順序非常重要！必須在 UseRouting 和 UseAuthorization 之間
            app.UseAuthentication();
            app.UseAuthorization();
            // ===================================

            app.MapRazorPages();
            app.MapControllers(); // <--- 2. 新增這行：將 API 控制器的路由對應到網址上

            // ===== 新增：在應用程式啟動時自動執行資料庫遷移 =====
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var context = services.GetRequiredService<ApplicationDbContext>();
                    context.Database.Migrate(); // 套用所有待處理的遷移
                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "An error occurred during DB migration.");
                }
            }
            // ======================================================

            app.Run();
        }
    }
}
