using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CebuCrmApi.Models;
using CebuCrmApi.Data;

namespace CebuCrmApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ServiceOrdersController : ControllerBase
    {
        private readonly CrmDbContext _context;
        public ServiceOrdersController(CrmDbContext context) { _context = context; }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ServiceOrder>>> GetOrders() => await _context.ServiceOrders.ToListAsync();

        [HttpPost]
        public async Task<ActionResult<ServiceOrder>> AddOrder([FromBody] ServiceOrder order)
        {
            _context.ServiceOrders.Add(order);
            await _context.SaveChangesAsync();
            return Ok(order);
        }


        // 編輯/更新 Service
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateServiceOrder(int id, ServiceOrder serviceOrder)
        {
            if (id != serviceOrder.Id) return BadRequest("ID 不符");

            _context.Entry(serviceOrder).State = EntityState.Modified;

            try { await _context.SaveChangesAsync(); }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.ServiceOrders.Any(e => e.Id == id)) return NotFound();
                else throw;
            }
            return NoContent();
        }

        // 刪除 Service
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteServiceOrder(int id)
        {
            var serviceOrder = await _context.ServiceOrders.FindAsync(id);
            if (serviceOrder == null) return NotFound();

            _context.ServiceOrders.Remove(serviceOrder);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
