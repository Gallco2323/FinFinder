using FinFinder.Data.Models;
using FinFinder.Web.ViewModels.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FinFinder.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Administrator")]
    public class UserController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole<Guid>> _roleManager;

        public UserController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole<Guid>> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }
        public async Task<IActionResult> Index()
        {
            var users = _userManager.Users.ToList();
            var model = new List<UserViewModel>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);

                model.Add(new UserViewModel
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    Email = user.Email,
                    Roles = roles
                });
            }

            return View(model);
        }

        // GET: Admin/User/Edit/{id}
        public async Task<IActionResult> Edit(Guid id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null)
            {
                return NotFound();
            }

            var roles = await _userManager.GetRolesAsync(user);

            var model = new EditUserViewModel
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                Roles = roles.ToList(),
                AllRoles = _roleManager.Roles.Select(r => r.Name).ToList()
            };

            return View(model);
        }

        // POST: Admin/User/Edit/{id}
        [HttpPost]
        public async Task<IActionResult> Edit(EditUserViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.FindByIdAsync(model.Id.ToString());
            if (user == null)
            {
                return NotFound();
            }

            user.UserName = model.UserName;
            user.Email = model.Email;

            var userRoles = await _userManager.GetRolesAsync(user);

            // Remove old roles
            var rolesToRemove = userRoles.Except(model.Roles).ToList();
            await _userManager.RemoveFromRolesAsync(user, rolesToRemove);

            // Add new roles
            var rolesToAdd = model.Roles.Except(userRoles).ToList();
            await _userManager.AddToRolesAsync(user, rolesToAdd);

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return View(model);
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: Admin/User/Delete/{id}
        [HttpPost]
        public async Task<IActionResult> Delete(Guid id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null)
            {
                return NotFound();
            }

            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return RedirectToAction(nameof(Index));
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
