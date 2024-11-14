using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FinFinder.Data
{
    public class FinFinderDbContext : IdentityDbContext
    {
        public FinFinderDbContext(DbContextOptions<FinFinderDbContext> options)
            : base(options)
        {
        }
    }
}
