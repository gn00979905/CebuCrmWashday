using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WASHDAY_202508.Data;
using WASHDAY_202508.DTOs;

namespace WASHDAY_202508.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [IgnoreAntiforgeryToken]// 允許前端 fetch 存取
    public class CustomersController : Controller
    {
        private readonly ApplicationDbContext _context;
        public CustomersController(ApplicationDbContext context)
        {
            _context = context;
        }
        // GET: api/customers
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CustomerDto>>> GetCustomers()
        {
            return await _context.Customers
                .OrderBy(c => c.Name) // 預設按名字排序
                .Select(c => new CustomerDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    PhoneNumber = c.PhoneNumber,
                    Email = c.Email,
                    Address = c.Address,
                    Notes = c.Notes
                })
                .ToListAsync();
        }

        // POST: api/customers
        [HttpPost]
        public async Task<ActionResult<CustomerDto>> CreateCustomer(CustomerDto dto)
        {
            var customer = new Customer
            {
                Name = dto.Name,
                PhoneNumber = dto.PhoneNumber,
                Email = dto.Email,
                Address = dto.Address,
                Notes = dto.Notes
            };

            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();

            dto.Id = customer.Id;
            return CreatedAtAction(nameof(GetCustomers), new { id = customer.Id }, dto);
        }

        // PUT: api/customers/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCustomer(int id, CustomerDto dto)
        {
            if (id != dto.Id) return BadRequest();

            var customer = await _context.Customers.FindAsync(id);
            if (customer == null) return NotFound();

            customer.Name = dto.Name;
            customer.PhoneNumber = dto.PhoneNumber;
            customer.Email = dto.Email;
            customer.Address = dto.Address;
            customer.Notes = dto.Notes;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: api/customers/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCustomer(int id)
        {
            var customer = await _context.Customers.FindAsync(id);
            if (customer == null) return NotFound();

            _context.Customers.Remove(customer);
            await _context.SaveChangesAsync();
            return NoContent();
        }

    }
}
