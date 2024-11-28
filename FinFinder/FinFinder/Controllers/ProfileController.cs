using FinFinder.Data;
using FinFinder.Data.Models;
using FinFinder.Services.Data.Interfaces;
using FinFinder.Web.ViewModels.FishCatch;
using FinFinder.Web.ViewModels.Profile;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace FinFinder.Web.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        
        private readonly IProfileService _profileService;
        public ProfileController( IProfileService _profileService)
        {
          
            this._profileService = _profileService;
        }

        // GET: Profile/Edit
        [HttpGet]
        public async Task<IActionResult> Edit()
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var model = await _profileService.GetProfileForEditAsync(userId);

            if (model == null)
            {
                return NotFound("User not found.");
            }

            return View(model);
        }

        // POST: Profile/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ProfileEditViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var success = await _profileService.UpdateProfileAsync(userId, model);

            if (!success)
            {
                ModelState.AddModelError(string.Empty, "Unable to update profile.");
                return View(model);
            }


            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public async Task<IActionResult> Details(Guid id)
        {
            var model = await _profileService.GetProfileDetailsAsync(id);

            if (model == null)
            {
                return NotFound("User not found.");
            }

            return View(model);

        }

        [HttpGet]
        public async Task<IActionResult> HiddenPosts(Guid id)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            if (userId != id)
            {
                return Unauthorized();
            }

            var model = await _profileService.GetHiddenPostsAsync(userId);
            return View(model);
        }
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Unhide(Guid id)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var success = await _profileService.UnhidePostAsync(userId, id);

            if (!success)
            {
                return Unauthorized();
            }

            return RedirectToAction(nameof(HiddenPosts), new { id = userId });
        }


    }
}
