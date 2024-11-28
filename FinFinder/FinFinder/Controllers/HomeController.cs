using FinFinder.Data;
using FinFinder.Data.Models;
using FinFinder.Models;
using FinFinder.Services.Data.Interfaces;
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
        private readonly IHomeService _homeService;
        private readonly ILogger<HomeController> _logger;

        public HomeController(IHomeService homeService, ILogger<HomeController> logger)
        {
            _homeService = homeService;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var userId = User.Identity?.IsAuthenticated == true
            ? Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier))
            : (Guid?)null;

            var model = new HomePageViewModel
            {
                FeaturedFishCatches = await _homeService.GetFeaturedFishCatchesAsync(),
                TotalFishCatches = await _homeService.GetTotalFishCatchesAsync(),
                TotalUsers = await _homeService.GetTotalUsersAsync(),
                MostPopularTechnique = await _homeService.GetMostPopularTechniqueAsync(),
                RecentActivities = await _homeService.GetRecentActivitiesAsync(),
                UserRecentCatches = userId.HasValue
                    ? await _homeService.GetUserRecentCatchesAsync(userId.Value)
                    : new List<FishCatchIndexViewModel>()
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
