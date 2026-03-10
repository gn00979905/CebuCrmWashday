using ClosedXML.Excel; // 引用 ClosedXML
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using WASHDAY_202508.Data;

namespace WASHDAY_202508.Pages.DailyLedgers
{
    [Authorize]
    public class DailyLedgerIndexModel : PageModel
    {
        private readonly WASHDAY_202508.Data.ApplicationDbContext _context;

        public DailyLedgerIndexModel(WASHDAY_202508.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<DailyLedger> DailyLedger { get; set; } = default!;
        // ===== 新增的屬性，用來存放總計值 =====
        // --- 屬性 ---
        public decimal TotalWalkIn { get; private set; }
        public decimal TotalSanMarino { get; private set; }

        public decimal TotalSunvida { get; private set; }
        public decimal TotalCebuRooms { get; private set; }
        public decimal TotalSugbuMercado { get; private set; }
        public decimal TotalOthers { get; private set; }
        public decimal GrandTotalSales { get; private set; }
        public decimal TotalExpenses { get; private set; }
        public decimal GrandNetSales { get; private set; }
        public List<int> AvailableYears { get; private set; } = new();
        public string PageTitle { get; private set; } = "Daily Ledger";

        // --- 綁定來自 URL 的篩選參數 ---
        [BindProperty(SupportsGet = true)]
        public int? CurrentYear { get; set; }
        [BindProperty(SupportsGet = true)]
        public int? CurrentMonth { get; set; }
        [BindProperty(SupportsGet = true)]
        public string? SearchTerm { get; set; }
        public async Task OnGetAsync(int? all, int? year, int? month)
        {
            try
            {
                //CurrentYear = year;
                //CurrentMonth = month;
                CurrentYear = year == null ? DateTime.UtcNow.Year : year;
                CurrentMonth = month == null ? DateTime.UtcNow.Month : month;
                if (all != null)
                {
                    CurrentYear = year;
                    CurrentMonth = month;
                }
                var ledgers = await GetFilteredLedgers();
                DailyLedger = ledgers.OrderByDescending(d => d.EntryDate).ToList();

                // 更新總計與標題
                UpdateTotalsAndTitle();
            }
            catch (Exception ex)
            {

                throw;
            }
        }
        // --- 新增：處理匯出 Excel 的方法 ---
        public async Task<IActionResult> OnGetExportAsync()
        {
            var ledgers = await GetFilteredLedgers();

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Ledger");
                var currentRow = 1;

                // 設定表頭
                worksheet.Cell(currentRow, 1).Value = "Date";
                worksheet.Cell(currentRow, 2).Value = "Sales/Walk-in";
                worksheet.Cell(currentRow, 3).Value = "Sunvida";
                worksheet.Cell(currentRow, 4).Value = "Cebu Rooms";
                worksheet.Cell(currentRow, 5).Value = "SugbuMercado";
                worksheet.Cell(currentRow, 6).Value = "Others";
                worksheet.Cell(currentRow, 7).Value = "Total Sales";
                worksheet.Cell(currentRow, 8).Value = "Expenses";
                worksheet.Cell(currentRow, 9).Value = "Description";
                worksheet.Cell(currentRow, 19).Value = "Net Sales";

                // 填入資料
                foreach (var item in ledgers.OrderBy(d => d.EntryDate))
                {
                    currentRow++;
                    worksheet.Cell(currentRow, 1).Value = item.EntryDate.ToString("yyyy-MM-dd");
                    worksheet.Cell(currentRow, 2).Value = item.SalesWalkIn;
                    worksheet.Cell(currentRow, 3).Value = item.SanMarino;
                    worksheet.Cell(currentRow, 4).Value = item.Sunvida;
                    worksheet.Cell(currentRow, 5).Value = item.CebuRooms;
                    worksheet.Cell(currentRow, 6).Value = item.SugbuMercado;
                    worksheet.Cell(currentRow, 7).Value = item.Others;
                    worksheet.Cell(currentRow, 8).Value = item.TotalSales;
                    worksheet.Cell(currentRow, 9).Value = item.Expenses;
                    worksheet.Cell(currentRow, 10).Value = item.Description;
                    worksheet.Cell(currentRow, 11).Value = item.NetSales;
                }

                // 將 Workbook 存入記憶體串流
                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    var fileName = $"Laundromat_Ledger_{DateTime.Now:yyyyMMdd}.xlsx";

                    // 回傳檔案給瀏覽器下載
                    return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
                }
            }
        }
        // --- 私有輔助方法 ---

        private async Task<List<DailyLedger>> GetFilteredLedgers()
        {
            AvailableYears = await _context.DailyLedgers
                                           .Include(d => d.ExpenseItems)
                                           .Select(d => d.EntryDate.Year)
                                           .Distinct()
                                           .OrderByDescending(y => y)
                                           .ToListAsync();

            var query = _context.DailyLedgers.Include(l => l.ExpenseItems).AsQueryable();

            if (CurrentYear.HasValue)
            {
                //query = query.Where(d => d.EntryDate.Year == CurrentYear.Value);
                query = query.Where(d => d.EntryDate.Year == CurrentYear.Value);
                // 產生動態標題
                PageTitle = $"Daily Ledger for {CurrentYear.Value}";
                if (CurrentMonth.HasValue)
                {
                    query = query.Where(d => d.EntryDate.Month == CurrentMonth.Value);
                    // 產生更精確的動態標題
                    string monthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(CurrentMonth.Value);
                    PageTitle = $"Daily Ledger for {monthName} {CurrentYear.Value}";
                }
            }
            else
            {
                PageTitle = "Daily Ledger (All Records)";
            }
            DailyLedger = await query.OrderBy(d => d.EntryDate).ToListAsync();
            // 總計邏輯維持不變，會自動計算篩選後結果的總和

            if (CurrentMonth.HasValue)
            {
                query = query.Where(d => d.EntryDate.Month == CurrentMonth.Value);
            }
            if (!string.IsNullOrEmpty(SearchTerm))
            {
                // 忽略大小寫進行搜尋
                query = query.Where(d => d.Description != null && d.Description.ToLower().Contains(SearchTerm.ToLower()));
            }

            if (DailyLedger.Any())
            {
                TotalWalkIn = DailyLedger.Sum(item => item.SalesWalkIn);
                TotalWalkIn = DailyLedger.Sum(item => item.SanMarino);

                TotalSunvida = DailyLedger.Sum(item => item.Sunvida);
                TotalCebuRooms = DailyLedger.Sum(item => item.CebuRooms);
                TotalSugbuMercado = DailyLedger.Sum(item => item.SugbuMercado);
                TotalOthers = DailyLedger.Sum(item => item.Others);
                GrandTotalSales = DailyLedger.Sum(item => item.TotalSales);
                TotalExpenses = DailyLedger.SelectMany(e => e.ExpenseItems).Sum(e => e.Amount);
                GrandNetSales = DailyLedger.Sum(item => item.NetSales);
            }

            return await query.ToListAsync();
        }

        private void UpdateTotalsAndTitle()
        {
            if (CurrentYear.HasValue)
            {
                PageTitle = $"Daily Ledger for {CurrentYear.Value}";
                if (CurrentMonth.HasValue)
                {
                    string monthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(CurrentMonth.Value);
                    PageTitle = $"Daily Ledger for {monthName} {CurrentYear.Value}";
                }
            }
            else
            {
                PageTitle = "Daily Ledger (All Records)";
            }

            if (DailyLedger.Any())
            {
                TotalWalkIn = DailyLedger.Sum(item => item.SalesWalkIn);
                TotalWalkIn = DailyLedger.Sum(item => item.SanMarino);

                TotalSunvida = DailyLedger.Sum(item => item.Sunvida);
                TotalCebuRooms = DailyLedger.Sum(item => item.CebuRooms);
                TotalSugbuMercado = DailyLedger.Sum(item => item.SugbuMercado);
                TotalOthers = DailyLedger.Sum(item => item.Others);
                GrandTotalSales = DailyLedger.Sum(item => item.TotalSales);
                TotalExpenses = DailyLedger.SelectMany(e => e.ExpenseItems).Sum(e => e.Amount);
                GrandNetSales = DailyLedger.Sum(item => item.NetSales);
            }
        }
    }
}
