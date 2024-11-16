using FinFinder.Data.Models;
using FinFinder.Web.ViewModels.Profile;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FinFinder.Web.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public ProfileController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        // GET: Profile/Edit
        [HttpGet]
        public async Task<IActionResult> Edit()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            var model = new ProfileEditViewModel
            {
                UserName = user.UserName,
                Bio = user.Bio,
                ProfilePictureURL = user.ProfilePictureURL
            };

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

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            // Update user properties
            user.UserName = model.UserName;
            user.Bio = model.Bio;

            // Check if a new profile picture was uploaded
            if (model.ProfilePicture != null)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/profiles");
                Directory.CreateDirectory(uploadsFolder); // Ensure the directory exists
                var uniqueFileName = $"{Guid.NewGuid()}_{model.ProfilePicture.FileName}";
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await model.ProfilePicture.CopyToAsync(fileStream);
                }

                user.ProfilePictureURL = $"/images/profiles/{uniqueFileName}";
            }

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return View(model);
            }

            return RedirectToAction("Index", "Home");
        }
    }
}
