using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WASHDAY_202508.Pages.Orders
{
    [Authorize]
    public class OrdersModel : PageModel
    {
        public int InitialYear { get; private set; }
        public int InitialMonth { get; private set; }
        public void OnGet()
        {
            var today = DateTime.Now;
            InitialYear = today.Year;
            InitialMonth = today.Month;
        }
    }
}
