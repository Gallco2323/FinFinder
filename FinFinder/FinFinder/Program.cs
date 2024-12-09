using FinFinder.Data;
using FinFinder.Data.Models;
using FinFinder.Data.Repository.Interfaces;
using FinFinder.Models;
using FinFinder.Services.Mapping;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FinFinder
{
    using FinFinder.Data.Repository;
    using FinFinder.Services.Data;
    using FinFinder.Services.Data.Interfaces;
    using FinFinder.Web.Infrastructure.Extensions;
    using FinFinder.Data.Seeders;
    public class Program
    {
        
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            string connectionString = builder.Configuration.GetConnectionString("SqlServer");

            // Add services to the container.

            builder.Services.AddDbContext<FinFinderDbContext>(options =>
            {
                options.UseSqlServer(connectionString);
            });

            builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
            {
                options.User.RequireUniqueEmail = true;
                options.SignIn.RequireConfirmedAccount = false;
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredLength = 6;
            })
            .AddRoles<IdentityRole<Guid>>()
            .AddEntityFrameworkStores<FinFinderDbContext>();


          

            //builder.Services.AddScoped<IRepository<FishCatch, Guid>, BaseRepository<FishCatch, Guid>>();
            //builder.Services.AddScoped<IRepository<FishingTechnique, Guid>, BaseRepository<FishingTechnique, Guid>>();
            //builder.Services.AddScoped<IRepository<Like, Guid>, BaseRepository<Like, Guid>>();
            //builder.Services.AddScoped<IRepository<Photo, Guid>, BaseRepository<Photo, Guid>>();
            //builder.Services.AddScoped<IRepository<Favorite, object>, BaseRepository<Favorite, object>>();
            //builder.Services.AddScoped<IRepository<Comment, Guid>, BaseRepository<Comment, Guid>>();
            builder.Services.RegisterRepositories(typeof(ApplicationUser).Assembly);
            


            builder.Services.AddScoped<ICommentService, CommentService>();
            builder.Services.AddScoped<IFavoriteService, FavoriteService>();
            builder.Services.AddScoped<ILikeService, LikeService>();
            builder.Services.AddScoped<IProfileService, ProfileService>();
            builder.Services.AddScoped<IHomeService, HomeService>();
            builder.Services.AddScoped<IFishCatchService, FishCatchService>();




            //builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
            //    .AddEntityFrameworkStores<FinFinderDbContext>();
            builder.Services.AddControllersWithViews();
            builder.Services.AddRazorPages();

            var app = builder.Build();

            AutoMapperConfig.RegisterMappings(typeof(ErrorViewModel).Assembly);
            // Role seeding logic
            using (var scope = app.Services.CreateScope())
            {
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                Task.Run(() => SeedData.SeedRolesAsync(roleManager, userManager)).GetAwaiter().GetResult();
            }


            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error/500"); // Redirect to custom 500 page
                app.UseStatusCodePagesWithReExecute("/Error/{0}"); // Redirect to custom error pages like 404
            }
            else
            {
                app.UseDeveloperExceptionPage(); // Default developer page for debugging
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();
            
            app.MapControllerRoute(
                name: "admin",
                pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");
            app.MapRazorPages();

            app.Run();
        }
    }
}
