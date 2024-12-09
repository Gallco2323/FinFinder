using FinFinder.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;


namespace FinFinder.Data.Seeders
{
    public  class SeedData
    {
        private readonly FinFinderDbContext _context;
        private readonly ILogger<SeedData> _logger;
      

        public SeedData(FinFinderDbContext context, ILogger<SeedData> logger)
        {
            _context = context;
            _logger = logger;
            
        }
        public static async Task SeedRolesAsync(RoleManager<IdentityRole<Guid>> roleManager, UserManager<ApplicationUser> userManager)
        {
            var roles = new[] { "User", "Administrator" };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole<Guid>(role));
                }
            }

            var adminEmail = "admin@example.com";
            var adminPassword = "Admin@123";

            if (await userManager.FindByEmailAsync(adminEmail) == null)
            {
                var adminUser = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true,
                    FishCount = 0,
                };

                var result = await userManager.CreateAsync(adminUser, adminPassword);

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Administrator");
                }
            }
        }

       
    }
}

