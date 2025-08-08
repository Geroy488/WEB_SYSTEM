using Microsoft.EntityFrameworkCore;
using WEB_SYSTEM.Controllers;
using static WEB_SYSTEM.Models.InventoryModel;

namespace WEB_SYSTEM.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
               : base(options)
        {

        }
        public DbSet<Campus> Campuses { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<Admin> Admins { get; set; }
    }
}
