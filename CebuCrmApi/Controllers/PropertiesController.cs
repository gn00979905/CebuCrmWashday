using CebuCrmApi.Data;
using CebuCrmApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CebuCrmApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PropertiesController : ControllerBase
    {
        private readonly CrmDbContext _context;
        // 透過依賴注入 (DI) 取得資料庫實體
        public PropertiesController(CrmDbContext context)
        {
            _context = context;
        }

        // GET: api/properties
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Property>>> GetProperties()
        {
            // 真實從資料庫撈取所有房產 (非同步)
            return await _context.Properties.ToListAsync();
        }

        // POST: api/properties
        [HttpPost]
        public async Task<ActionResult<Property>> AddProperty([FromBody] Property newProperty)
        {
            // 將新房產存入資料庫
            _context.Properties.Add(newProperty);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetProperties), new { id = newProperty.Id }, newProperty);
        }
        // PUT: api/properties/5
        // 更新物件資訊 (如：狀態改為 Sold)
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProperty(int id, [FromBody] Property property)
        {
            if (id != property.Id) return BadRequest();

            _context.Entry(property).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Properties.Any(e => e.Id == id)) return NotFound();
                else throw;
            }

            return NoContent();
        }

        // DELETE: api/properties/5
        // 刪除物件
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProperty(int id)
        {
            var property = await _context.Properties.FindAsync(id);
            if (property == null) return NotFound();

            _context.Properties.Remove(property);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
