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
            var fishCatch = await _fishCatchService.GetFishCatchByIdAsync(id);

            if (fishCatch == null)
            {
                return NotFound("Fish catch not found.");
            }

            // Pass the creator's UserId to the existing service method
            var success = await _fishCatchService.PermanentDeleteFishCatchAsync(id, fishCatch.UserId);

            if (!success)
            {
                ModelState.AddModelError("", "An error occurred while deleting the fish catch.");
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
