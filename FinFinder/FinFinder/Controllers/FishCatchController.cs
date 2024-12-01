using FinFinder.Data;
using FinFinder.Data.Models;
using FinFinder.Data.Repository.Interfaces;
using FinFinder.Services.Data.Interfaces;
using FinFinder.Web.ViewModels.Comment;
using FinFinder.Web.ViewModels.FishCatch;
using FinFinder.Web.ViewModels.Profile;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using System.Security.Claims;

namespace FinFinder.Web.Controllers
{
    using static Common.EntityValidationConstants.FishCatch;
    public class FishCatchController : Controller
    {
        private readonly IFishCatchService _fishCatchService;
      
       

        public FishCatchController(IFishCatchService fishCatchService, UserManager<ApplicationUser> userManager, IRepository<FishCatch, Guid> fishCatchRepository)
        {
            _fishCatchService = fishCatchService;
            
            
        }

        // INDEX
        public async Task<IActionResult> Index(string selectedFilter = "DatePosted", string searchTerm = "")
        {
            var model = await _fishCatchService.GetFilteredFishCatchesAsync(selectedFilter, searchTerm);
            return View(model);
        }

        // CREATE GET
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var model = await _fishCatchService.PrepareCreateViewModelAsync();

            return View(model);
        }

        // CREATE POST
        [HttpPost]
        public async Task<IActionResult> Create(FishCatchCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var createModel = await _fishCatchService.PrepareCreateViewModelAsync();
                model.FishingTechniques = createModel.FishingTechniques; // Await the method and assign its result directly
                return View(model);
            }

            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var success = await _fishCatchService.CreateFishCatchAsync(model, userId);

            if (!success)
            {
                ModelState.AddModelError("", "An error occurred while creating the fish catch.");
                return View(model);
            }

            return RedirectToAction(nameof(Index));
        }

        // EDIT GET
        [HttpGet]
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
                return NotFound();

            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var model = await _fishCatchService.PrepareEditViewModelAsync(id.Value, userId);

            if (model == null)
                return Unauthorized();

            return View(model);
        }

        // EDIT POST
        [HttpPost]
        [HttpPost]
        public async Task<IActionResult> Edit(Guid id, FishCatchEditViewModel model)
        {
            if (id != model.Id)
                return NotFound();

            if (!ModelState.IsValid)
            {
                var editModel = await _fishCatchService.PrepareEditViewModelAsync(id, Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)));
                model.FishingTechniques = editModel.FishingTechniques;
                model.ExistingPhotos = editModel.ExistingPhotos;
                return View(model);
            }

            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var success = await _fishCatchService.UpdateFishCatchAsync(model, userId);

            if (!success)
            {
                ModelState.AddModelError("", "An error occurred while updating the fish catch.");
                return View(model);
            }

            return RedirectToAction(nameof(Index));
        }



        // DETAILS
        [HttpGet]
        public async Task<IActionResult> Details(Guid id)
        {
            var userId = User.Identity?.IsAuthenticated == true
           ? Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier))
           : (Guid?)null;

            var model = await _fishCatchService.GetFishCatchDetailsAsync(id, userId);

            if (model == null)
                return NotFound();

            return View(model);
        }

        // ADD TO FAVORITES
        [HttpPost]
        public async Task<IActionResult> AddToFavorites(Guid id)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var success = await _fishCatchService.AddToFavoritesAsync(id, userId);

            if (!success)
            {
                return BadRequest("An error occurred while adding the fish catch to favorites.");
            }

            return RedirectToAction("Details", new { id });
        }
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> SoftDelete(Guid id)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var success = await _fishCatchService.SoftDeleteFishCatchAsync(id, userId);

            if (!success)
            {
                return Unauthorized();
            }

            return RedirectToAction(nameof(Index));
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> PermanentDelete(Guid id)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var success = await _fishCatchService.PermanentDeleteFishCatchAsync(id, userId);

            if (!success)
            {
                return Unauthorized();
            }

            return RedirectToAction(nameof(Index));
        }



    }
}
