using FinFinder.Data;
using FinFinder.Data.Models;
using FinFinder.Models;
using FinFinder.Web.ViewModels.FishCatch;
using FinFinder.Web.ViewModels.Home;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Security.Claims;

namespace FinFinder.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly FinFinderDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        public HomeController(ILogger<HomeController> logger, FinFinderDbContext finFinderDbContext, UserManager<ApplicationUser> userManager)
        {
            _logger = logger;
            _context = finFinderDbContext;
            _userManager = userManager;

        }

        public async Task<IActionResult> Index()
        {
            // Get Featured Fish Catches (e.g., most liked or recent)
            var featuredCatches = await _context.FishCatches
                .Where(fc => !fc.IsDeleted)
                .Include(fc => fc.Photos)
                .OrderByDescending(fc => fc.Likes.Count) // Most liked
                .Take(5)
                .Select(fc => new FishCatchIndexViewModel
                {
                    Id = fc.Id,
                    Species = fc.Species,
                    LocationName = fc.LocationName,
                    DateCaught = fc.DateCaught,
                    PhotoURLs = fc.Photos.Select(ph => ph.Url).ToList(),
                    PublisherName = fc.User.UserName
                })
                .ToListAsync();

            // Get Community Stats
            var totalFishCatches = await _context.FishCatches.CountAsync(fc => !fc.IsDeleted);
            var totalUsers = await _userManager.Users.CountAsync();
            var mostPopularTechnique = await _context.FishingTechniques
                .OrderByDescending(ft => ft.FishCatches.Count)
                .Select(ft => ft.Name)
                .FirstOrDefaultAsync() ?? "None";

            // Get Recent Activity
            var recentActivities = await _context.FishCatches
                .Where(fc => !fc.IsDeleted)
                .OrderByDescending(fc => fc.DateCaught)
                .Take(5)
                .Select(fc => new ActivityViewModel
                {
                    UserName = fc.User.UserName,
                    ActionDescription = $"posted a new catch: {fc.Species}",
                    Timestamp = fc.DateCaught
                })
                .ToListAsync();

            // Get User Recent Catches
            List<FishCatchIndexViewModel> userRecentCatches = new List<FishCatchIndexViewModel>();
            if (User.Identity?.IsAuthenticated == true)
            {
                var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                userRecentCatches = await _context.FishCatches
                    .Where(fc => !fc.IsDeleted && fc.UserId == userId)
                    .Include(fc => fc.Photos)
                    .OrderByDescending(fc => fc.DateCaught)
                    .Take(3)
                    .Select(fc => new FishCatchIndexViewModel
                    {
                        Id = fc.Id,
                        Species = fc.Species,
                        LocationName = fc.LocationName,
                        DateCaught = fc.DateCaught,
                        PhotoURLs = fc.Photos.Select(ph=>ph.Url).ToList() 
                    })
                    .ToListAsync();
            }

            // Build ViewModel
            var model = new HomePageViewModel
            {
                FeaturedFishCatches = featuredCatches,
                TotalFishCatches = totalFishCatches,
                TotalUsers = totalUsers,
                MostPopularTechnique = mostPopularTechnique,
                RecentActivities = recentActivities,
                UserRecentCatches = userRecentCatches
            };

            return View(model);
        }


        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
