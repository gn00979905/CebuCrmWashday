using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WASHDAY_202508.Data;

namespace WASHDAY_202508.Pages
{
    [Authorize]
    public class EditModel : PageModel
    {
        private readonly WASHDAY_202508.Data.ApplicationDbContext _context;

        public EditModel(WASHDAY_202508.Data.ApplicationDbContext context)
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

            var dailyledger =  await _context.DailyLedgers.FirstOrDefaultAsync(m => m.Id == id);
            if (dailyledger == null)
            {
                return NotFound();
            }
            DailyLedger = dailyledger;
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(DailyLedger).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DailyLedgerExists(DailyLedger.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./DailyLedgers/Ledgers");
        }

        private bool DailyLedgerExists(int id)
        {
            return _context.DailyLedgers.Any(e => e.Id == id);
        }
    }
}
