using FinFinder.Services.Data.Interfaces;
using FinFinder.Web.ViewModels.FishCatch;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FinFinder.Web.Areas.Admin.Controllers
{
    [Area("Admin")]

    public class AdminFishCatchController : Controller
    {
        private readonly IFishCatchService _fishCatchService;

        public AdminFishCatchController(IFishCatchService fishCatchService)
        {
            _fishCatchService = fishCatchService;
        }

        public async Task<IActionResult> Index()
        {
            // Get all fish catches for management
            var fishCatches = await _fishCatchService.GetAllFishCatchesAsync();

            var model = fishCatches.Select(fc => new ManageFishCatchViewModel
            {
                Id = fc.Id,
                Species = fc.Species,
                LocationName = fc.LocationName,
                PublisherName = fc.PublisherName,
                DateCaught = fc.DateCaught,
                PhotoURLs = fc.PhotoURLs,
                IsDeleted = fc.IsDeleted
            }).ToList();

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(Guid id)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var success = await _fishCatchService.PermanentDeleteFishCatchAsync(id, userId);

            if (!success)
            {
                TempData["Error"] = "Failed to delete the post. It may not exist or you lack permission.";
            }
            else
            {
                TempData["Success"] = "Post deleted successfully.";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
