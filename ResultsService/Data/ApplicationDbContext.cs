using Microsoft.EntityFrameworkCore;
using ResultsService.Models;

namespace ResultsService.Data
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
