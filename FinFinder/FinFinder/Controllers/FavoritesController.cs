using FinFinder.Data;
using FinFinder.Services.Data.Interfaces;
using FinFinder.Web.ViewModels.FishCatch;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace FinFinder.Web.Controllers
{
    public class FavoritesController : Controller
    {
        private readonly FinFinderDbContext _context;

        private readonly IFavoriteService _favoriteService;

        public FavoritesController(FinFinderDbContext context, IFavoriteService favoriteService)
        {
            _context = context;
            _favoriteService = favoriteService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        

          var favorites =  await _favoriteService.GetUserFavoritesAsync(userId);

            return View(favorites);
        }

        [HttpPost]
        [Authorize]

        public async Task<IActionResult> Remove(Guid id)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            // Find the favorite entry for the current user and specified FishCatch
         await  _favoriteService.RemoveFavoriteAsync(userId, id);

            return RedirectToAction("Index"); // Redirect back to the favorites list
        }

    }
}
