using WASHDAY_202508.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;


namespace WASHDAY_202508.Pages
{
    [Authorize]
    public class ReportsModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public ReportsModel(ApplicationDbContext context)
        {
            _context = context;
        }
        // --- 篩選與導覽屬性 ---
        public int CurrentYear { get; private set; }
        public int CurrentMonth { get; private set; }
        public DateTime PreviousMonthDate { get; private set; }
        public DateTime NextMonthDate { get; private set; }

        // --- 圖表資料屬性 ---
        public List<string> ExpenseChartLabels { get; set; } = new();
        public List<decimal> ExpenseChartData { get; set; } = new();
        public List<string> ExpenseChartColors { get; set; } = new();
        // --- ↓↓↓ 新增的年度圖表屬性 ↓↓↓ ---

        // 1. 用於「營收 vs. 支出趨勢圖」
        public List<string> AnnualTrendLabels { get; set; } = new List<string> { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" };
        public List<decimal> AnnualSalesData { get; set; } = new();
        public List<decimal> AnnualExpenseDataForSales { get; set; } = new();

        // 2. 用於「年度費用分析圖」
        public List<string> AnnualExpenseLabels { get; set; } = new();
        public List<decimal> AnnualExpenseData { get; set; } = new();
        public List<string> AnnualExpenseColors { get; set; } = new();
        // --- ↑↑↑ 新增屬性結束 ↑↑↑ ---

        public async Task OnGetAsync(int? year, int? month)
        {
            var today = DateTime.Now;
            CurrentYear = year ?? today.Year;
            CurrentMonth = month ?? today.Month;

            var currentDate = new DateTime(CurrentYear, CurrentMonth, 1);
            PreviousMonthDate = currentDate.AddMonths(-1);
            NextMonthDate = currentDate.AddMonths(1);

            // -- 從資料庫查詢並彙總支出資料 --
            var temp = await _context.ExpenseItems
                .Include(e => e.DailyLedger).ToListAsync();
            var expenseBreakdown = temp.Where(e => e.DailyLedger.EntryDate.Year == CurrentYear &&
                            e.DailyLedger.EntryDate.Month == CurrentMonth)
                .GroupBy(e => e.Category)
                .Select(g => new
                {
                    Category = g.Key,
                    TotalAmount = g.Sum(e => e.Amount)
                })
                .OrderByDescending(x => x.TotalAmount).ToList();
                

            if (expenseBreakdown.Any())
            {
                ExpenseChartLabels = expenseBreakdown.Select(x => x.Category).ToList();
                ExpenseChartData = expenseBreakdown.Select(x => x.TotalAmount).ToList();
                ExpenseChartColors = GenerateColors(expenseBreakdown.Count);
                //// --- 準備圖表資料 ---
                //var random = new Random();
                //foreach (var item in expenseBreakdown)
                //{
                //    ExpenseChartLabels.Add(item.Category);
                //    ExpenseChartData.Add(item.TotalAmount);
                //    // 產生隨機顏色
                //    var color = String.Format("#{0:X6}", random.Next(0x1000000));
                //    ExpenseChartColors.Add(color);
                //}
            }
            // --- ↓↓↓ 1. 新增：「營收 vs. 支出趨勢圖」的資料查詢 ↓↓↓ ---
            var yearlyLedgers = await _context.DailyLedgers
                .Include(l => l.ExpenseItems) // 包含費用細項，確保 Expenses 總額計算正確
                .Where(l => l.EntryDate.Year == CurrentYear)
                .ToListAsync();

            var monthlySales = yearlyLedgers
                .GroupBy(l => l.EntryDate.Month)
                .Select(g => new { Month = g.Key, Sales = g.Sum(l => l.TotalSales) })
                .ToDictionary(x => x.Month, x => x.Sales);

            var monthlyExpenses = yearlyLedgers
                .GroupBy(l => l.EntryDate.Month)
                .Select(g => new { Month = g.Key, Expenses = g.Sum(l => l.Expenses) })
                .ToDictionary(x => x.Month, x => x.Expenses);

            var salesData = new decimal[12];
            var expenseData = new decimal[12];

            for (int i = 1; i <= 12; i++)
            {
                salesData[i - 1] = monthlySales.TryGetValue(i, out var s) ? s : 0;
                expenseData[i - 1] = monthlyExpenses.TryGetValue(i, out var e) ? e : 0;
            }
            AnnualSalesData = salesData.ToList();
            AnnualExpenseDataForSales = expenseData.ToList();

            //--- ↓↓↓ 2. 新增：「年度費用分析圖」的資料查詢 ↓↓↓ ---
            var AnnualTemp  = await _context.ExpenseItems
                              .Include(e => e.DailyLedger).ToListAsync();
            var annualExpenseBreakdown = AnnualTemp
                .Where(e=>e.DailyLedger.EntryDate.Year == CurrentYear)
                .GroupBy(e=>e.Category)
                .Select(g => new 
                {
                     Category = g.Key,
                    TotalAmount = g.Sum(e=>e.Amount)
                })
                .OrderByDescending(x=>x.TotalAmount)
                .ToList();

            if(annualExpenseBreakdown.Any())
            {
                AnnualExpenseLabels = annualExpenseBreakdown.Select(x=>x.Category).ToList();
                AnnualExpenseData = annualExpenseBreakdown.Select(x=>x.TotalAmount).ToList();
                AnnualExpenseColors = GenerateColors(annualExpenseBreakdown.Count);// 重複使用我們已有的顏色產生器
            }
        
        }
        private List<string> GenerateColors(int count)
        {
            //var colors = new List<string>();
            //var random = new Random();
            //for (int i = 0; i < count; i++)
            //{
            //    var color = String.Format("#{0:X6}", random.Next(0x1000000));
            //    colors.Add(color);
            //}
            //return colors;
            var colors = new List<string>();
            var random = new Random();
            for (int i = 0; i < count; i++)
            {
                colors.Add($"rgba({random.Next(50, 200)}, {random.Next(50, 200)}, {random.Next(50, 200)}, 0.7)");
            }
            return colors;
        }
    }
}
