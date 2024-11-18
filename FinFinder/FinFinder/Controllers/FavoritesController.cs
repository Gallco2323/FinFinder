using FinFinder.Data;
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

        public FavoritesController(FinFinderDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var favorites = await _context.Favorites
                .Where(f => f.UserId == userId)
                .Select(f => new FishCatchFavoriteViewModel
                {
                    FishCatchId = f.FishCatchId,
                    Species = f.FishCatch.Species,
                    Location = f.FishCatch.Location,
                    DateCaught = f.FishCatch.DateCaught,
                    PhotoURL = f.FishCatch.PhotoURL ?? "/images/default-fish.jpg",
                    PublisherName = f.FishCatch.User.UserName
                })
                .ToListAsync();

            return View(favorites);
        }

        [HttpPost]
        [Authorize]
       
        public async Task<IActionResult> Remove(Guid id)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            // Find the favorite entry for the current user and specified FishCatch
            var favorite = await _context.Favorites
                .FirstOrDefaultAsync(f => f.UserId == userId && f.FishCatchId == id);

            if (favorite != null)
            {
                _context.Favorites.Remove(favorite);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index"); // Redirect back to the favorites list
        }

    }
}
