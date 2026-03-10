using CebuCrmApi.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// 1. 【修改】動態判斷 SQLite 資料庫路徑
// 如果是開發環境 (本機)，存在當前目錄；如果是正式環境 (Render)，存在持久化硬碟目錄
var dbPath = builder.Environment.IsDevelopment()
    ? "cebucrm.db"
    : "/var/data/cebucrm.db";

builder.Services.AddDbContext<CrmDbContext>(options =>
    options.UseSqlite($"Data Source={dbPath}"));

builder.Services.AddControllers();

// 註冊 YARP 服務
builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 2. 【修改】擴充 CORS 設定
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
    {
        policy.WithOrigins(
                "http://localhost:5173",
                "http://127.0.0.1:5173"
              // 之後等你的 React 部署到 Render 產生靜態網址後，記得回來把網址加在這裡！
              // 例如: "https://my-cebu-crm-frontend.onrender.com"
              )
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

// 自動建立資料庫
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<CrmDbContext>();
    db.Database.EnsureCreated();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowReactApp");
app.UseAuthorization();

// cebu-crm 本身的 API 路由
app.MapControllers();

// 啟動 YARP 反向代理
app.MapReverseProxy();

app.Run();