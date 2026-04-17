using CebuCrmApi.Data;
using CebuCrmApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CebuCrmApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LeadsController : ControllerBase
    {
        private readonly CrmDbContext _context;

        public LeadsController(CrmDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Client>>> GetLeads()
        {
            return await _context.Clients
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();
        }

        [HttpPost]
        public async Task<ActionResult<Client>> AddLead([FromBody] Client lead)
        {
            lead.CreatedAt = lead.CreatedAt == default ? DateTime.UtcNow : lead.CreatedAt;
            _context.Clients.Add(lead);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetLeads), new { id = lead.Id }, lead);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateLead(int id, [FromBody] Client lead)
        {
            if (id != lead.Id)
            {
                return BadRequest("ID mismatch");
            }

            _context.Entry(lead).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.Clients.AnyAsync(c => c.Id == id))
                {
                    return NotFound();
                }

                throw;
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLead(int id)
        {
            var lead = await _context.Clients.FindAsync(id);
            if (lead == null)
            {
                return NotFound();
            }

            _context.Clients.Remove(lead);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
