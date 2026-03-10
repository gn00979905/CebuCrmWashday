using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WASHDAY_202508.Data;
using WASHDAY_202508.DTOs;

namespace WASHDAY_202508.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [IgnoreAntiforgeryToken]
    public class ExpenseItemsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ExpenseItemsController(ApplicationDbContext context)
        {
            _context = context;
        }
        // GET: /api/expenseitems?dailyLedgerId=5
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ExpenseItemDto>>> GetExpenseItems([FromQuery] int dailyLedgerId)
        {
            return await _context.ExpenseItems
                .Where(e => e.DailyLedgerId == dailyLedgerId)
                .Select(e => new ExpenseItemDto
                {
                    Id = e.Id,
                    Category = e.Category,
                    Description = e.Description,
                    Amount = e.Amount,
                    DailyLedgerId = e.DailyLedgerId
                })
                .ToListAsync();
        }
        // GET: api/ExpenseItems/ByLedger/5
        [HttpGet("ByLedger/{ledgerId}")]
        public async Task<ActionResult<IEnumerable<ExpenseItem>>> GetExpensesByLedger(int ledgerId)
        {
            return await _context.ExpenseItems
                .Where(e => e.DailyLedgerId == ledgerId)
                .ToListAsync();
        }
        // POST: /api/expenseitems
        [HttpPost]
        public async Task<ActionResult<ExpenseItemDto>> CreateExpenseItem(ExpenseItemDto dto)
        {
            var expenseItem = new ExpenseItem
            {
                Category = dto.Category,
                Description = dto.Description,
                Amount = dto.Amount,
                DailyLedgerId = dto.DailyLedgerId
            };

            _context.ExpenseItems.Add(expenseItem);
            await _context.SaveChangesAsync();

            dto.Id = expenseItem.Id;
            return CreatedAtAction(nameof(GetExpenseItems), new { id = expenseItem.Id }, dto);
        }
        // DELETE: /api/expenseitems/10
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteExpenseItem(int id)
        {
            var expenseItem = await _context.ExpenseItems.FindAsync(id);
            if (expenseItem == null)
            {
                return NotFound();
            }

            _context.ExpenseItems.Remove(expenseItem);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
