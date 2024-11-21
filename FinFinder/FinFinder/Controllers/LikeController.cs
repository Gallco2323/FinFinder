using FinFinder.Data;
using FinFinder.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace FinFinder.Web.Controllers
{
    [Authorize]
    public class LikeController : Controller
    {
        private readonly FinFinderDbContext _context;

        public LikeController(FinFinderDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> Add(Guid fishCatchId)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var existingLike = await _context.Likes
                .FirstOrDefaultAsync(l => l.FishCatchId == fishCatchId && l.UserId == userId);

            if (existingLike == null)
            {
                var like = new Like
                {
                    Id = Guid.NewGuid(),
                    FishCatchId = fishCatchId,
                    UserId = userId
                };

                _context.Likes.Add(like);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Details", "FishCatch", new { id = fishCatchId });
        }

        [HttpPost]
        public async Task<IActionResult> Remove(Guid fishCatchId)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var like = await _context.Likes
                .FirstOrDefaultAsync(l => l.FishCatchId == fishCatchId && l.UserId == userId);

            if (like != null)
            {
                _context.Likes.Remove(like);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Details", "FishCatch", new { id = fishCatchId });
        }
    }

}
