using CebuCrmApi.Data;
using CebuCrmApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CebuCrmApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UnitsController : ControllerBase
    {
        private readonly CrmDbContext _context;

        public UnitsController(CrmDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetUnits([FromQuery] int? projectId)
        {
            var query = _context.Units
                .Include(u => u.Project)
                .AsQueryable();

            if (projectId.HasValue)
            {
                query = query.Where(u => u.ProjectId == projectId.Value);
            }

            var units = await query
                .OrderBy(u => u.Project!.Name)
                .ThenBy(u => u.UnitCode)
                .Select(u => new
                {
                    u.Id,
                    u.ProjectId,
                    ProjectName = u.Project!.Name,
                    u.UnitCode,
                    u.Price,
                    u.SizeSqm,
                    u.Bedrooms,
                    u.Status,
                    u.FloorPlanUrl
                })
                .ToListAsync();

            return Ok(units);
        }

        [HttpPost]
        public async Task<ActionResult<Unit>> AddUnit([FromBody] Unit unit)
        {
            _context.Units.Add(unit);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetUnits), new { id = unit.Id }, unit);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUnit(int id, [FromBody] Unit unit)
        {
            if (id != unit.Id)
            {
                return BadRequest("ID mismatch");
            }

            _context.Entry(unit).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.Units.AnyAsync(u => u.Id == id))
                {
                    return NotFound();
                }

                throw;
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUnit(int id)
        {
            var unit = await _context.Units.FindAsync(id);
            if (unit == null)
            {
                return NotFound();
            }

            var hasDeals = await _context.Deals.AnyAsync(d => d.UnitId == id);
            if (hasDeals)
            {
                return BadRequest("This unit is already linked to deals. Remove those deals first.");
            }

            _context.Units.Remove(unit);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
