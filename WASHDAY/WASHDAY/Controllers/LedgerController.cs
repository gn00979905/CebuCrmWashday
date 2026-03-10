using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WASHDAY_202508.Data;
using WASHDAY_202508.DTOs;

namespace WASHDAY_202508.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [IgnoreAntiforgeryToken] // <--- 2. 加上這個屬性標籤！
    public class LedgerController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public LedgerController(ApplicationDbContext context)
        {
            _context = context;
        }
        // GET: /api/ledger
        [HttpGet]
        public async Task<ActionResult<IEnumerable<LedgerApiDto>>> GetLedgerEntries([FromQuery] int? year, [FromQuery] int? month)
        {
            var query = _context.DailyLedgers.Include(l => l.ExpenseItems).AsQueryable();

            // **新增：** 如果提供了年份和月份，就篩選資料
            if (year.HasValue && month.HasValue)
            {
                query = query.Where(d => d.EntryDate.Year == year.Value && d.EntryDate.Month == month.Value);
            }

            // 從資料庫讀取 DailyLedger，但轉換成 LedgerApiDto 再回傳
            return await query
                .OrderBy(d => d.EntryDate)
                .Select(d => new LedgerApiDto
                {
                    Id = d.Id,
                    EntryDate = d.EntryDate.ToString("yyyy-MM-dd"), // 在這裡格式化日期！
                    SalesWalkIn = d.SalesWalkIn,
                    SanMarino = d.SanMarino,
                    Sunvida = d.Sunvida,
                    CebuRooms = d.CebuRooms,
                    SugbuMercado = d.SugbuMercado,
                    Others = d.Others,
                    Expenses = d.Expenses,
                    // **新增：** 使用 String.Join 把所有細項的 Category 串起來
                    // 如果想要包含金額，可以寫成: $"{e.Category} ({e.Amount})"
                    ExpenseDetails = string.Join(", ", d.ExpenseItems.Select(e => $"{e.Category} ({e.Amount})")),
                    Description = d.Description==null? "": d.Description
                })
                .ToListAsync();
        }

        // POST: /api/ledger
        [HttpPost]
        public async Task<ActionResult<LedgerApiDto>> CreateLedgerEntry(LedgerApiDto dto)
        {
            // 3. 接收 DTO，並轉換回 DailyLedger 實體來儲存
            var dailyLedger = new DailyLedger
            {
                EntryDate = DateTime.Parse(dto.EntryDate), // 將字串解析回 DateTime
                SalesWalkIn = dto.SalesWalkIn,
                SanMarino = dto.SanMarino,
                Sunvida = dto.Sunvida,
                CebuRooms = dto.CebuRooms,
                SugbuMercado = dto.SugbuMercado,
                Others = dto.Others,
                //Expenses = dto.Expenses,
                Description = dto.Description
            };

            _context.DailyLedgers.Add(dailyLedger);
            await _context.SaveChangesAsync();

            dto.Id = dailyLedger.Id; // 將新產生的 ID 寫回 DTO
            return CreatedAtAction(nameof(GetLedgerEntries), new { id = dailyLedger.Id }, dto);
        }

        // PUT: /api/ledger/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateLedgerEntry(int id, LedgerApiDto dto)
        {
            if (id != dto.Id)
            {
                return BadRequest();
            }

            var dailyLedger = await _context.DailyLedgers.FindAsync(id);
            if (dailyLedger == null)
            {
                return NotFound();
            }

            // 4. 更新實體
            dailyLedger.EntryDate = DateTime.Parse(dto.EntryDate);
            dailyLedger.SalesWalkIn = dto.SalesWalkIn;
            dailyLedger.SanMarino = dto.SanMarino;
            dailyLedger.Sunvida = dto.Sunvida;
            dailyLedger.CebuRooms = dto.CebuRooms;
            dailyLedger.SugbuMercado = dto.SugbuMercado;
            dailyLedger.Others = dto.Others;
            //dailyLedger.Expenses = dto.Expenses;
            dailyLedger.Description = dto.Description;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: /api/ledger/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLedgerEntry(int id)
        {
            var dailyLedger = await _context.DailyLedgers.FindAsync(id);
            if (dailyLedger == null)
            {
                return NotFound();
            }

            _context.DailyLedgers.Remove(dailyLedger);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
