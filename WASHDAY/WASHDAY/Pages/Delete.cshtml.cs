using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WASHDAY_202508.Data;

namespace WASHDAY_202508.Pages
{
    [Authorize]
    public class DeleteModel : PageModel
    {
        private readonly WASHDAY_202508.Data.ApplicationDbContext _context;

        public DeleteModel(WASHDAY_202508.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public DailyLedger DailyLedger { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dailyledger = await _context.DailyLedgers.Include(m => m.ExpenseItems)
                .FirstOrDefaultAsync(m => m.Id == id); // <--- **一定要加這行 (.Include)**;

            if (dailyledger == null)
            {
                return NotFound();
            }
            else
            {
                DailyLedger = dailyledger;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dailyledger = await _context.DailyLedgers.FindAsync(id);
            if (dailyledger != null)
            {
                DailyLedger = dailyledger;
                _context.DailyLedgers.Remove(DailyLedger);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./DailyLedgers/Ledgers");
        }
    }
}
