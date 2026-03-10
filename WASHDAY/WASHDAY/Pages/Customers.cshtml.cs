using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WASHDAY_202508.Pages
{
    [Authorize]
    public class CustomersModel : PageModel
    {
        public void OnGet()
        {
        }
    }
}
