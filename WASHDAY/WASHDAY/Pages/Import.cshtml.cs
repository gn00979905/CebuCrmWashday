using ClosedXML.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using WASHDAY_202508.Data;

namespace WASHDAY_202508.Pages
{
    [Authorize] // 這個頁面也需要登入才能使用
    public class ImportModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        public ImportModel(ApplicationDbContext context)
        {
            _context = context;
        }
        [BindProperty]
        [Required(ErrorMessage = "Please select a file to upload.")]
        [Display(Name ="Excel File")]
        public IFormFile UploadedFile { get; set; }
        [BindProperty]
        [Required]
        [Display(Name = "Year")]
        public int SelectedYear { get; set; }
        [BindProperty]
        [Required]
        [Display(Name = "Month")]
        public int SelectedMonth { get; set; }
        public List<SelectListItem> Years { get; set; }
        public List<SelectListItem> Months { get; set; }

        // 用來顯示成功或失敗的訊息
        public string? SuccessMessage { get; set; }
        public string? ErrorMessage { get; set; }
        public void OnGet()
        {
            // 初始化年月下拉選單的選項
            var currentYear = DateTime.Now.Year;
            Years = Enumerable.Range(currentYear - 5, 11)
                              .Select(y => new SelectListItem { Value = y.ToString(), Text = y.ToString() })
                              .ToList();

            Months = Enumerable.Range(1, 12)
                               .Select(m => new SelectListItem { Value = m.ToString(), Text = new DateTime(2000, m, 1).ToString("MMMM") })
                               .ToList();

            SelectedYear = currentYear;
            SelectedMonth = DateTime.Now.Month;
        }
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                OnGet(); // 如果模型驗證失敗，需要重新填充下拉選單
                return Page();
            }

            var ledgersToCreate = new List<DailyLedger>();

            try
            {
                // 將上傳的檔案讀入記憶體
                await using var stream = new MemoryStream();
                await UploadedFile.CopyToAsync(stream);

                using var workbook = new XLWorkbook(stream);
                var worksheet = workbook.Worksheet(1); // 讀取第一個工作表
                var rows = worksheet.RangeUsed().RowsUsed().Skip(1); // 讀取所有使用中的列，並跳過第一行(表頭)

                foreach (var row in rows)
                {
                    // 嘗試讀取第一欄的日期 (數字)
                    if (!row.Cell(1).TryGetValue<int>(out var day))
                    {
                        // 如果第一欄不是數字，可能是特殊行，例如 "RENTAL"
                        if (row.Cell(1).GetString().Trim().ToUpper() == "RENTAL")
                        {
                            if (row.Cell(6).TryGetValue<decimal>(out var rentalExpense))
                            {
                                // 為 RENTAL 建立一筆特殊的支出紀錄，日期設為該月最後一天
                                var lastDayOfMonth = DateTime.DaysInMonth(SelectedYear, SelectedMonth);
                                ledgersToCreate.Add(new DailyLedger
                                {
                                    EntryDate = new DateTime(SelectedYear, SelectedMonth, lastDayOfMonth),
                                    Description = "Monthly Rental",
                                    //Expenses = rentalExpense
                                });
                            }
                        }
                        continue; // 跳過無法解析的行 (例如空行或 TOTAL 行)
                    }

                    // 建立日期
                    var entryDate = new DateTime(SelectedYear, SelectedMonth, day);

                    // 讀取各欄位的值
                    row.Cell(2).TryGetValue<decimal>(out var walkInSales);
                    row.Cell(3).TryGetValue<decimal>(out var sunvidaSales);
                    row.Cell(4).TryGetValue<decimal>(out var sugbuMercado);
                    row.Cell(5).TryGetValue<decimal>(out var othersSales);
                    row.Cell(7).TryGetValue<decimal>(out var expenses);

                    var newLedger = new DailyLedger
                    {
                        EntryDate = entryDate,
                        SalesWalkIn = walkInSales,
                        Sunvida = sunvidaSales,
                        SugbuMercado = sugbuMercado,
                        Others = othersSales,
                        //Expenses = expenses,
                    };
                    ledgersToCreate.Add(newLedger);
                }

                // --- 寫入資料庫 ---
                // 1. 刪除該月份的所有現有紀錄
                var existingRecords = _context.DailyLedgers
                    .Where(d => d.EntryDate.Year == SelectedYear && d.EntryDate.Month == SelectedMonth);
                _context.DailyLedgers.RemoveRange(existingRecords);

                // 2. 新增從 Excel 讀取到的新紀錄
                await _context.DailyLedgers.AddRangeAsync(ledgersToCreate);

                // 3. 儲存變更
                await _context.SaveChangesAsync();

                SuccessMessage = $"Successfully imported {ledgersToCreate.Count} records for {new DateTime(SelectedYear, SelectedMonth, 1):MMMM yyyy}.";
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Import failed: {ex.Message}. Please ensure the Excel file format is correct.";
            }

            OnGet(); // 重新填充下拉選單
            return Page();
        }
    }
}
