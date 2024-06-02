using Microsoft.EntityFrameworkCore;
using VotingService.Models;

namespace VotingService.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Vote> Votes { get; set; }
    }
}
