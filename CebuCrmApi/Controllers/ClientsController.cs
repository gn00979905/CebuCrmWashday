using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CebuCrmApi.Models;
using CebuCrmApi.Data;

namespace CebuCrmApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClientsController : ControllerBase
    {
        private readonly CrmDbContext _context;
        public ClientsController(CrmDbContext context)
        {
            _context = context;
        }

        // --- 1. 取得所有客戶 (GET) ---
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Client>>> GetClients()
        {
            return await _context.Clients.ToListAsync();
        }

        // --- 2. 新增客戶 (POST) ---
        [HttpPost]
        public async Task<ActionResult<Client>> AddClient([FromBody] Client client)
        {
            _context.Clients.Add(client);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetClients), new { id = client.Id }, client);
        }

        // --- 3. 🌟 新增：更新客戶資料與狀態 (PUT) ---
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateClient(int id, [FromBody] Client client)
        {
            // 防呆檢查：網址的 ID 必須跟傳進來的 JSON ID 一致
            if (id != client.Id)
            {
                return BadRequest("ID mismatch");
            }

            // 標記這筆資料已被修改
            _context.Entry(client).State = EntityState.Modified;

            try
            {
                // 儲存進資料庫
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                // 如果同時有人在刪除這筆資料，檢查它還在不在
                if (!ClientExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent(); // 204 NoContent 是 PUT 成功的標準回傳值
        }

        // --- 4. 刪除客戶 (DELETE) ---
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteClient(int id)
        {
            var client = await _context.Clients.FindAsync(id);
            if (client == null) return NotFound();
            _context.Clients.Remove(client);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // --- 輔助函式：檢查客戶是否存在 ---
        private bool ClientExists(int id)
        {
            return _context.Clients.Any(e => e.Id == id);
        }
    }
}