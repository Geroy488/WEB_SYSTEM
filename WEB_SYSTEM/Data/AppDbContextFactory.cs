using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace WEB_SYSTEM.Data
{
    public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            optionsBuilder.UseMySql(
                "server=localhost;port=3306;database=websystem;user=root;password=root",
                new MySqlServerVersion(new Version(8, 0, 0))
            );
            return new AppDbContext(optionsBuilder.Options);
        }
    }
}
