using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WEB_SYSTEM.Data;
using static WEB_SYSTEM.Models.InventoryModel;

namespace WEB_SYSTEM.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Admins : ControllerBase
    {
        private readonly AppDbContext _context;

        public Admins(AppDbContext context)
        {
            _context = context;
        }
        private string GenerateJwtToken(Admin admin)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("12345678901234567890123456789012"));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
        new Claim("sid", admin.StudentId.ToString())
            };

            var token = new JwtSecurityToken(
                issuer: "yourdomain.com",
                audience: "yourdomain.com",
                claims: claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        [Authorize]
        [HttpGet("secured")]
        public IActionResult Secured()  
        {
            return Ok("You accessed a protected route.");
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromQuery] int StudentId)
        {
            var admin = await _context.Admins
                .FirstOrDefaultAsync(s => s.StudentId == StudentId);

            if (admin == null)
            {
                return Unauthorized(new { message = "Login failed !" });
            }

            var token = GenerateJwtToken(admin);

            return Ok(new
            {
                message = "Student Login successful :)",
                student = new
                {
                    admin.StudentId,
                    admin.StudentName,
                    admin.Grade,
                    admin.Section,
                    admin.Birthday
                },
                token = token
            });
        }


        // GET: api/students
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Admins>>> GetAdmins()
        {
            var admin = await _context.Admins.ToListAsync();

            if (admin == null || admin.Count == 0)
            {
                return NotFound();
            }

            return Ok(admin);
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddAdmins([FromQuery] string studentName, string Grade, string Section, string Birthday)
        {
            if (string.IsNullOrWhiteSpace(studentName) || studentName.Length <= 1)
            {
                return BadRequest("Invalid student name.");
            }

            studentName = studentName.Trim();

            if (int.TryParse(studentName, out _))
            {
                return BadRequest("Student name cannot be numeric.");
            }

            bool exists = await _context.Admins
                .AnyAsync(c => (c.StudentName ?? "").ToLower() == studentName.ToLower());

            if (exists)
            {
                return BadRequest("Student name already exists.");
            }
            
            var admin = new Admin
            {
                StudentName = studentName,
                Grade = Grade,
                Section = Section,
                Birthday = DateOnly.Parse(Birthday) // Fix: convert string to DateOnly
            };

            _context.Admins.Add(admin);
            await _context.SaveChangesAsync();

            return Ok("Successfully added student");
        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdateAdmin([FromQuery] int studentId, [FromQuery] Admin updatedAdmin)
        {
            if (string.IsNullOrWhiteSpace(updatedAdmin.StudentName) || updatedAdmin.StudentName.Length <= 1)
            {
                return BadRequest("Invalid student name.");
            }

            updatedAdmin.StudentName = updatedAdmin.StudentName.Trim();

            if (int.TryParse(updatedAdmin.StudentName, out _))
            {
                return BadRequest("Student name cannot be numeric.");
            }

            var existingAdmin = await _context.Admins.FindAsync(studentId);
            if (existingAdmin == null)
            {
                return NotFound($"Student with ID {studentId} not found.");
            }

            bool duplicate = await _context.Admins
                .AnyAsync(c => c.StudentName.ToLower() == updatedAdmin.StudentName.ToLower() && c.StudentId != studentId);

            if (duplicate)
            {
                return BadRequest("Another student with the same name already exists.");
            }

            // Update all fields
            existingAdmin.StudentName = updatedAdmin.StudentName;
            existingAdmin.Grade = updatedAdmin.Grade?.Trim();
            existingAdmin.Section = updatedAdmin.Section?.Trim();
            existingAdmin.Birthday = updatedAdmin.Birthday;
           

            await _context.SaveChangesAsync();

            return Ok("Student updated successfully.");
        }

        // DELETE: api/admins/delete?id=1
        [HttpDelete("delete")]
        public async Task<IActionResult> DeleteAdmins([FromQuery] int studentId)
        {
            var admin = await _context.Admins.FindAsync(studentId);

            if (admin == null)
            {
                return NotFound($"Student with ID {studentId} not found.");
            }

            _context.Admins.Remove(admin);
            await _context.SaveChangesAsync();

            return Ok("Student deleted successfully.");
        }
    }
}
