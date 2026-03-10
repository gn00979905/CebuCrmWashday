using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WASHDAY_202508.Data;
using System.Globalization;
using Microsoft.AspNetCore.Authorization;

namespace WASHDAY_202508.Pages
{
    [Authorize]
    public class MonthlySummaryModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public MonthlySummaryModel(ApplicationDbContext context)
        {
            _context = context;
        }
        // --- 報表資料屬性 ---
        public List<string> IncomeCategories { get; set; } = new();
        public List<string> ExpenseCategories { get; set; } = new();
        public List<MonthlyReportRow> ReportData { get; set; } = new();

        // --- 年份導覽屬性 ---
        public int CurrentYear { get; private set; }
        public int PreviousYear { get; private set; }
        public int NextYear { get; private set; }
        // --- 內部的 ViewModel 類別 ---
        public class MonthlyReportRow
        {
            public int Month { get; set; }
            public string MonthName { get; set; } = string.Empty;
            // 使用 Dictionary 來動態儲存各類別的總和
            public Dictionary<string, decimal> IncomeTotals { get; set; } = new();
            public Dictionary<string, decimal> ExpenseTotals { get; set; } = new();

            // 計算屬性
            public decimal TotalIncome => IncomeTotals.Values.Sum();
            public decimal TotalExpense => ExpenseTotals.Values.Sum();
            public decimal NetTotal => TotalIncome - TotalExpense;
        }
        public async Task OnGetAsync(int? year)
        {
            // 1. 設定年份
            CurrentYear = year ?? DateTime.Now.Year;
            PreviousYear = CurrentYear - 1;
            NextYear = CurrentYear + 1;

            // 2. 獲取全年的收入和支出資料
            var allLedgersForYear = await _context.DailyLedgers
                .Where(l => l.EntryDate.Year == CurrentYear)
                .ToListAsync();

            var allExpensesForYear = await _context.ExpenseItems
                .Include(e => e.DailyLedger)
                .Where(e => e.DailyLedger.EntryDate.Year == CurrentYear)
                .ToListAsync();

            // 3. 確定所有欄位 (動態產生費用欄位)
            IncomeCategories = new List<string> { "SalesWalkIn","SanMarino", "Sunvida", "CebuRooms", "SugbuMercado", "Others" };
            ExpenseCategories = allExpensesForYear
                .Select(e => e.Category)
                .Distinct()
                .OrderBy(c => c)
                .ToList();

            // 4. 按月份迴圈，產生12筆彙總資料
            for (int month = 1; month <= 12; month++)
            {
                var row = new MonthlyReportRow
                {
                    Month = month,
                    MonthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month).ToUpper()
                };

                // 取得這個月的資料
                var ledgersThisMonth  = allLedgersForYear .Where(l => l.EntryDate  .Month == month);
                var expensesThisMonth = allExpensesForYear.Where(e => e.DailyLedger.EntryDate.Month == month);

                // 5. 計算每個收入類別的總和
                foreach (var category in IncomeCategories)
                {
                    decimal total = category switch
                    {
                        "SalesWalkIn" => ledgersThisMonth.Sum(l => l.SalesWalkIn),
                        "SanMarino" => ledgersThisMonth.Sum(l => l.SanMarino),

                        "Sunvida" => ledgersThisMonth.Sum(l => l.Sunvida),
                        "CebuRooms" => ledgersThisMonth.Sum(l => l.CebuRooms),
                        "SugbuMercado" => ledgersThisMonth.Sum(l => l.SugbuMercado),
                        "Others" => ledgersThisMonth.Sum(l => l.Others),
                        _ => 0
                    };
                    row.IncomeTotals[category] = total;
                }

                // 6. 計算每個費用類別的總和
                foreach (var category in ExpenseCategories)
                {
                    row.ExpenseTotals[category] = expensesThisMonth
                        .Where(e => e.Category == category)
                        .Sum(e => e.Amount);
                }

                ReportData.Add(row);
            }
        }
    }
}
