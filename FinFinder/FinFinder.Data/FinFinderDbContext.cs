using Azure;
using FinFinder.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace FinFinder.Data
{
    public class FinFinderDbContext : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>
    {
        public FinFinderDbContext(DbContextOptions<FinFinderDbContext> options)
            : base(options)
        {
        }
        public DbSet<FishCatch> FishCatches { get; set; }
       
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Favorite> Favorites { get; set; }
        public DbSet<Like> Likes { get; set; }
        public DbSet<Photo> Photos { get; set; }
        public DbSet<FishingTechnique> FishingTechniques { get; set; }
     

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Additional configuration for entities if needed
            builder.Entity<FishingTechnique>().HasData(
        new FishingTechnique { Id = Guid.NewGuid(), Name = "Fly Fishing" },
        new FishingTechnique { Id = Guid.NewGuid(), Name = "Spin Fishing" },
        new FishingTechnique { Id = Guid.NewGuid(), Name = "Bait Fishing" },
        new FishingTechnique { Id = Guid.NewGuid(), Name = "Trolling" }
    );
        }
    }
}
