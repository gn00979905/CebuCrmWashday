using CebuCrmApi.Data;
using CebuCrmApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CebuCrmApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProjectsController : ControllerBase
    {
        private readonly CrmDbContext _context;

        public ProjectsController(CrmDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetProjects()
        {
            var projects = await _context.Projects
                .Include(p => p.Units)
                .OrderBy(p => p.Name)
                .Select(p => new
                {
                    p.Id,
                    p.Name,
                    p.Developer,
                    p.Type,
                    p.Category,
                    p.Location,
                    Units = p.Units
                        .OrderBy(u => u.UnitCode)
                        .Select(u => new
                        {
                            u.Id,
                            u.ProjectId,
                            u.UnitCode,
                            u.Price,
                            u.SizeSqm,
                            u.Bedrooms,
                            u.Status,
                            u.PaymentPlan,
                            u.Discount,
                            u.FloorPlanUrl
                        })
                })
                .ToListAsync();

            return Ok(projects);
        }

        [HttpPost]
        public async Task<ActionResult<Project>> AddProject([FromBody] Project project)
        {
            _context.Projects.Add(project);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetProjects), new { id = project.Id }, project);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProject(int id, [FromBody] Project project)
        {
            if (id != project.Id)
            {
                return BadRequest("ID mismatch");
            }

            _context.Entry(project).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.Projects.AnyAsync(p => p.Id == id))
                {
                    return NotFound();
                }

                throw;
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProject(int id)
        {
            var project = await _context.Projects
                .Include(p => p.Units)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (project == null)
            {
                return NotFound();
            }

            var hasDeals = await _context.Deals.AnyAsync(d => d.Unit!.ProjectId == id);
            if (hasDeals)
            {
                return BadRequest("This project already has deals. Remove those deals first.");
            }

            _context.Projects.Remove(project);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
