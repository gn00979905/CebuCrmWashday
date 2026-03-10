// /Pages/Index.cshtml.cs
using Microsoft.AspNetCore.Authorization; // 引用
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WASHDAY_202508.Data;

namespace WASHDAY_202508.Pages
{
    [Authorize] // <--- 加上這個標籤
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        // --- 新增的屬性，用來儲存當前顯示的年月和導覽連結的日期 ---
        public int CurrentYear { get; private set; }
        public int CurrentMonth { get; private set; }
        public DateTime PreviousMonthDate { get; private set; }
        public DateTime NextMonthDate { get; private set; }

        public DashboardDataViewModel DashboardData { get; set; } = new();

        // 修改 OnGetAsync 方法以接收參數
        public async Task OnGetAsync(int? year, int? month)
        {
            // 如果 URL 沒有提供年月，就使用當前的年月
            var today = DateTime.Now;
            CurrentYear = year ?? today.Year;
            CurrentMonth = month ?? today.Month;

            var currentDate = new DateTime(CurrentYear, CurrentMonth, 1);

            // 計算上個月和下個月的日期，給前端的按鈕使用
            PreviousMonthDate = currentDate.AddMonths(-1);
            NextMonthDate = currentDate.AddMonths(1);

            // 1. 取得指定月份的所有帳目紀錄
            var monthlyLedgers = await _context.DailyLedgers
                .Include(d => d.ExpenseItems)
                .Where(d => d.EntryDate.Year == CurrentYear && d.EntryDate.Month == CurrentMonth)
                .ToListAsync();

            // 2. 計算 KPI 卡片的數據
            DashboardData.MonthlySales = monthlyLedgers.Sum(d => d.TotalSales);
            DashboardData.MonthlyExpenses = monthlyLedgers.Sum(d => d.Expenses);

            // 3. 計算營收來源圓餅圖的數據
            DashboardData.WalkInSales = monthlyLedgers.Sum(d => d.SalesWalkIn);
            DashboardData.SanMarino = monthlyLedgers.Sum(d=>d.SanMarino);
            DashboardData.SunvidaSales = monthlyLedgers.Sum(d => d.Sunvida);
            DashboardData.CebuRooms = monthlyLedgers.Sum(d => d.CebuRooms);
            DashboardData.SugbuMercado = monthlyLedgers.Sum(d => d.SugbuMercado);
            DashboardData.OtherSales = monthlyLedgers.Sum(d => d.Others);

            // 4. 準備每日營收折線圖的數據
            var daysInMonth = DateTime.DaysInMonth(CurrentYear, CurrentMonth);
            var dailySales = new decimal[daysInMonth];

            var groupedSales = monthlyLedgers.GroupBy(d => d.EntryDate.Day);
            foreach (var group in groupedSales)
            {
                dailySales[group.Key - 1] = group.Sum(d => d.TotalSales);
            }

            DashboardData.LineChartLabels = Enumerable.Range(1, daysInMonth).Select(d => d.ToString()).ToList();
            DashboardData.LineChartData = dailySales.ToList();

            // **新增：** 準備年度銷售長條圖的數據
            // 5. 取得當前所選年份的所有帳目紀錄
            var yearlyLedgers = await _context.DailyLedgers
                .Include(l => l.ExpenseItems) // 包含費用細項，確保 Expenses 總額計算正確
                .Where(l => l.EntryDate.Year == CurrentYear)
                .ToListAsync();

            // 6. 按月份分組並計算總銷售額
            var monthlySales = yearlyLedgers
                .GroupBy(d => d.EntryDate.Month)
                .Select(g => new { Month = g.Key, Total = g.Sum(d => d.TotalSales) })
                .ToDictionary(x => x.Month, x => x.Total);

            var monthlyExpenses = yearlyLedgers
                .GroupBy(l => l.EntryDate.Month)
                .Select(g => new { Month = g.Key, Expenses = g.Sum(l => l.Expenses) })
                .ToDictionary(x => x.Month, x => x.Expenses);

            //DashboardData.AnnualSalesChartLabels = new List<string> { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" };
            //DashboardData.AnnualSalesChartData = annualSalesData.ToList();

            var salesData = new decimal[12];
            var expenseData = new decimal[12];

            for (int i = 1; i <= 12; i++)
            {
                salesData[i - 1] = monthlySales.TryGetValue(i, out var s) ? s : 0;
                expenseData[i - 1] = monthlyExpenses.TryGetValue(i, out var e) ? e : 0;
            }

            DashboardData.AnnualSalesData = salesData.ToList();
            DashboardData.AnnualExpenseDataForSales = expenseData.ToList();
        }

        public class DashboardDataViewModel
        {
            public decimal MonthlySales { get; set; }
            public decimal MonthlyExpenses { get; set; }
            public decimal MonthlyNetProfit => MonthlySales - MonthlyExpenses;
            public decimal WalkInSales { get; set; }
            public decimal SanMarino { get; set; }

            public decimal SunvidaSales { get; set; }
            public decimal CebuRooms { get; set; }
            public decimal SugbuMercado { get; set; }
            public decimal OtherSales { get; set; }
            public List<string> LineChartLabels { get; set; } = new();
            public List<decimal> LineChartData { get; set; } = new();
            // **新增：** 年度圖表的資料屬性
            public List<string> AnnualSalesChartLabels { get; set; } = new();
            public List<decimal> AnnualSalesChartData { get; set; } = new();
            // 1. 用於「營收 vs. 支出趨勢圖」
            public List<string> AnnualTrendLabels { get; set; } = new List<string> { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" };
            public List<decimal> AnnualSalesData { get; set; } = new();
            public List<decimal> AnnualExpenseDataForSales { get; set; } = new();
        }
    }
}
