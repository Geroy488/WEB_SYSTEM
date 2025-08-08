using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WEB_SYSTEM.Data;
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

        [HttpGet("login")]
        public async Task<IActionResult> Login([FromQuery] string Student_Name)
        {
            var student = await _context.Students
                .FirstOrDefaultAsync(s => s.Student_Name == Student_Name);

            if (student == null)
            {
                return Unauthorized(new { message = "Login failed" });
            }

            return Ok(new { message = "Login successful" });
        }
        // GET: api/students
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Students>>> GetStudents()
        {
            var student = await _context.Students.ToListAsync();

            if (student == null || student.Count == 0)
            {
                return NotFound();
            }

            return Ok(student);
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddStudents([FromQuery] string student_Name, string Address, string Tel_No, string Grade, string Section)
        {
            if (string.IsNullOrWhiteSpace(student_Name) || student_Name.Length <= 1)
            {
                return BadRequest("Invalid student name.");
            }

            student_Name = student_Name.Trim();
     
            if (int.TryParse(student_Name, out _))
            {
                return BadRequest("Student name cannot be numeric.");
            }

            bool exists = await _context.Students
                .AnyAsync(c => (c.Student_Name ?? "").ToLower() == student_Name.ToLower());


            if (exists)
            {
                return BadRequest("Student name already exists.");
            }

            var student = new Student
            {
                Student_Name = student_Name,
                Address = Address,
                Tel_No = Tel_No,
                Grade = Grade,
                Section = Section
            };

            _context.Students.Add(student);
            await _context.SaveChangesAsync();

            return Ok("Successfully added student");
        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdateStudent([FromQuery] int student_Id, [FromQuery] Student updatedStudent)
        {
            if (string.IsNullOrWhiteSpace(updatedStudent.Student_Name) || updatedStudent.Student_Name.Length <= 1)
            {
                return BadRequest("Invalid student name.");
            }

            updatedStudent.Student_Name = updatedStudent.Student_Name.Trim();

            if (int.TryParse(updatedStudent.Student_Name, out _))
            {
                return BadRequest("Student name cannot be numeric.");
            }

            var existingStudent = await _context.Students.FindAsync(student_Id);
            if (existingStudent == null)
            {
                return NotFound($"Student with ID {student_Id} not found.");
            }

            bool duplicate = await _context.Students
                .AnyAsync(c => c.Student_Name.ToLower() == updatedStudent.Student_Name.ToLower() && c.Student_Id != student_Id);

            if (duplicate)
            {
                return BadRequest("Another student with the same name already exists.");
            }

            // Update all fields
            existingStudent.Student_Name = updatedStudent.Student_Name;
            existingStudent.Address = updatedStudent.Address?.Trim();
            existingStudent.Tel_No = updatedStudent.Tel_No?.Trim();
            existingStudent.Grade = updatedStudent.Grade?.Trim();
            existingStudent.Section = updatedStudent.Section?.Trim();
            
            await _context.SaveChangesAsync();

            return Ok("Student updated successfully.");
        }

        // DELETE: api/campuses/delete?id=1
        [HttpDelete("delete")]
        public async Task<IActionResult> DeleteStudent([FromQuery] int student_Id)
        {
            var student = await _context.Students.FindAsync(student_Id);

            if (student == null)
            {
                return NotFound($"Student with ID {student_Id} not found.");
            }

            _context.Students.Remove(student);
            await _context.SaveChangesAsync();

            return Ok("Student deleted successfully.");
        }
    }
}

