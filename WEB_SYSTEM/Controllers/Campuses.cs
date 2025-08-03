using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WEB_SYSTEM.Data;
using static WEB_SYSTEM.Models.InventoryModel;

namespace WEB_SYSTEM.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Campuses : ControllerBase
    {
        private readonly AppDbContext _context;

        public Campuses(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/campuses
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Campus>>> GetCampuses()
        {
            var campuses = await _context.Campuses.ToListAsync();

            if (campuses == null || campuses.Count == 0)
            {
                return NotFound();
            }

            return Ok(campuses);
        }
        [HttpPost("add")]
        public async Task<IActionResult> AddCampus([FromQuery] string campusName)
        {
            if (string.IsNullOrWhiteSpace(campusName) || campusName.Length <= 1)
            {
                return BadRequest("Invalid campus name.");
            }

            campusName = campusName.Trim();

            if (int.TryParse(campusName, out _))
            {
                return BadRequest("Campus name cannot be numeric.");
            }

            bool exists = await _context.Campuses
                .AnyAsync(c => (c.CampusName ?? "").ToLower() == campusName.ToLower());

            if (exists)
            {
                return BadRequest("Campus name already exists.");
            }

            var campus = new Campus { CampusName = campusName };

            _context.Campuses.Add(campus);
            await _context.SaveChangesAsync();

            return Ok("Successfully added campus");
        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdateCampus([FromQuery] int id, [FromQuery] string newCampusName)
        {
            if (string.IsNullOrWhiteSpace(newCampusName) || newCampusName.Length <= 1)
            {
                return BadRequest("Invalid campus name.");
            }

            newCampusName = newCampusName.Trim();

            if (int.TryParse(newCampusName, out _))
            {
                return BadRequest("Campus name cannot be numeric.");
            }

            var existingCampus = await _context.Campuses.FindAsync(id);
            if (existingCampus == null)
            {
                return NotFound($"Campus with ID {id} not found.");
            }

            // Check if another campus already has the same name
            bool duplicate = await _context.Campuses
                .AnyAsync(c => c.CampusName.ToLower() == newCampusName.ToLower() && c.ID != id);

            if (duplicate)
            {
                return BadRequest("Another campus with the same name already exists.");
            }

            existingCampus.CampusName = newCampusName;
            await _context.SaveChangesAsync();

            return Ok("Campus updated successfully.");
        }
        // DELETE: api/campuses/delete?id=1
        [HttpDelete("delete")]
        public async Task<IActionResult> DeleteCampus([FromQuery] int id)
        {
            var campus = await _context.Campuses.FindAsync(id);

            if (campus == null)
            {
                return NotFound($"Campus with ID {id} not found.");
            }

            _context.Campuses.Remove(campus);
            await _context.SaveChangesAsync();

            return Ok("Campus deleted successfully.");
        }

    }
}
