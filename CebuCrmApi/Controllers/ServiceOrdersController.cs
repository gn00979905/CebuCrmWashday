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

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            var order = await _context.ServiceOrders.FindAsync(id);
            if (order == null) return NotFound();
            _context.ServiceOrders.Remove(order);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
