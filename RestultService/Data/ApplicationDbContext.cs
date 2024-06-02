using Microsoft.EntityFrameworkCore;
using ResultService.Models;

namespace ResultService.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Result> Results { get; set; }
    }
}
