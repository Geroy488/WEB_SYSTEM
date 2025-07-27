using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using WEB_SYSTEM.Data;
using Microsoft.EntityFrameworkCore;

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
            var Student = await _context.Student
                .FirstOrDefaultAsync(s => s.Student_Name == Student_Name && s.Address == Address && s.Tel_No == Tel_No && s.Grade == Grade && s.Section == Section);

            if (Student == null)
            {
                return Unauthorized(new { message = "Login failed" });
            }

            return Ok(new { message = "Login successful" });
        }

    }
}
