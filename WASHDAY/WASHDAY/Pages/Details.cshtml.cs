using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WASHDAY_202508.Data;

namespace WASHDAY_202508.Pages
{
    [Authorize]
    public class DetailsModel : PageModel
    {
        private readonly WASHDAY_202508.Data.ApplicationDbContext _context;

        public DetailsModel(WASHDAY_202508.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public DailyLedger DailyLedger { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dailyledger = await _context.DailyLedgers
                .Include(m => m.ExpenseItems)
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
    }
}
