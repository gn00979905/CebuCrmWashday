using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WASHDAY_202508.Data;

namespace WASHDAY_202508.Pages
{
    [Authorize]
    public class CreateModel : PageModel
    {
        private readonly WASHDAY_202508.Data.ApplicationDbContext _context;

        public CreateModel(WASHDAY_202508.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            DailyLedger = new DailyLedger
            {
                EntryDate = DateTime.Now // 設定為今天
            };
            return Page();
        }

        [BindProperty]
        public DailyLedger DailyLedger { get; set; } = default!;

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.DailyLedgers.Add(DailyLedger);
            await _context.SaveChangesAsync();

            return RedirectToPage("./LedgerGrid");
        }
    }
}
