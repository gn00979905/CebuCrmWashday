using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;


namespace WASHDAY_202508.Pages
{
    [Authorize]
    public class LedgerGridModel : PageModel
    {  // **新增：** 用來儲存初始顯示的年份和月份
        public int InitialYear { get; private set; }
        public int InitialMonth { get; private set; }
        public void OnGet()
        {  // 設定頁面載入時，預設顯示的年月為當前的年月
            var today = DateTime.Now;
            InitialYear = today.Year;
            InitialMonth = today.Month;
        }
    }
}
