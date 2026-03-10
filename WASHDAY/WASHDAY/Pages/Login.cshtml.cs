// /Pages/Login.cshtml.cs
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace WASHDAY_202508.Pages
{
    public class LoginModel : PageModel
    {
        private readonly IConfiguration _configuration;
        public LoginModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        [BindProperty]
        public InputModel Input { get; set; }
        public class InputModel
        {
            [Required]
            public string Username { get; set; }

            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }
        }
        public void OnGet()
        {
        }
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // 從 appsettings.json 讀取正確的帳號密碼
            var validUsername = _configuration["SiteCredentials:Username"];
            var validPassword = _configuration["SiteCredentials:Password"];

            // 驗證使用者輸入
            if (Input.Username == validUsername && Input.Password == validPassword)
            {
                // 驗證成功，建立使用者的身分證明 (Claims)
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, Input.Username),
                    // 可以加入更多資訊，例如角色
                    // new Claim(ClaimTypes.Role, "Administrator"),
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var authProperties = new AuthenticationProperties();

                // 執行登入，這會產生加密的 cookie 並傳送給瀏覽器
                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties);

                // 登入成功後，跳轉到首頁 (儀表板)
                return LocalRedirect("/");
            }

            // 驗證失敗，顯示錯誤訊息
            ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            return Page();
        }
    }
}
