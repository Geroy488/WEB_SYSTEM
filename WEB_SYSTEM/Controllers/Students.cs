using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using WEB_SYSTEM.Data;
using WEB_SYSTEM.Models;
using static WEB_SYSTEM.Models.InventoryModel;



namespace WEB_SYSTEM.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Students : ControllerBase
    {
        private readonly AppDbContext _context;

        public Students(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromQuery] string Student_Name, [FromQuery] string Address, [FromQuery] string Tel_No, [FromQuery] string Grade, [FromQuery] string Section)
        {
            var student = await _context.Student
                .FirstOrDefaultAsync(s => s.Student_Name == Student_Name && s.Address == Address && s.Tel_No == Tel_No && s.Grade == Grade && s.Section == Section);

            if (student == null)
            {
                return Unauthorized(new { message = "Login failed" });
            }

            return Ok(new { message = "Login successful" });
        }
        // GET: api/campuses
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Students>>> GetStudents()
        {
            var student = await _context.Student.ToListAsync();

            if (student == null || student.Count == 0)
            {
                return NotFound();
            }

            return Ok(student);
        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdateStudent([FromQuery] int id, [FromQuery] string newStudent_Name)
        {
            if (string.IsNullOrWhiteSpace(newStudent_Name) || newStudent_Name.Length <= 1)
            {
                return BadRequest("Invalid student name.");
            }

            newStudent_Name = newStudent_Name.Trim();

            if (int.TryParse(newStudent_Name, out _))
            {
                return BadRequest("Student name cannot be numeric.");
            }

            var existingStudents = await _context.Student.FindAsync(id);
            if (existingStudents == null)
            {
                return NotFound($"Student with ID {id} not found.");
            }

            // Check if another campus already has the same name
            bool duplicate = await _context.Student
                .AnyAsync(c => c.Student_Name.ToLower() == newStudent_Name.ToLower() && c.Student_Id != id);

            if (duplicate)
            {
                return BadRequest("Another student with the same name already exists.");
            }

            existingStudents.Student_Name = newStudent_Name;
            await _context.SaveChangesAsync();

            return Ok("Student updated successfully.");
        }
        // DELETE: api/campuses/delete?id=1
        [HttpDelete("delete")]
        public async Task<IActionResult> DeleteStudent([FromQuery] int Id)
        {
            var student = await _context.Student.FindAsync(Id);

            if (student == null)
            {
                return NotFound($"Student with ID {Id} not found.");
            }

            _context.Student.Remove(student);
            await _context.SaveChangesAsync();

            return Ok("Campus deleted successfully.");
        }

    }
}
