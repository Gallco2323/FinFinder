using FinFinder.Services.Data.Interfaces;
using FinFinder.Web.ViewModels.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinFinder.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Administrator")]
    public class DashboardController : Controller
    {
        private readonly IHomeService _homeService;

        public DashboardController(IHomeService homeService)
        {
            _homeService = homeService;
        }
        public async Task<IActionResult> Index()
        {
            var totalUsers = await _homeService.GetTotalUsersAsync();
            var totalFishCatches = await _homeService.GetTotalFishCatchesAsync();
            var totalComments = await _homeService.GetTotalCommentsAsync();
            var recentActivities = await _homeService.GetRecentActivitiesAsync();

            // Create and populate the view model
            var model = new AdminDashboardViewModel
            {
                TotalUsers = totalUsers,
                TotalFishCatches = totalFishCatches,
                TotalComments = totalComments,
                RecentActivities = recentActivities
            };
            return View(model);
        }
    }
}
