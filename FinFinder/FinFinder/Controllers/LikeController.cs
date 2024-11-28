using FinFinder.Data;
using FinFinder.Data.Models;
using FinFinder.Services.Data.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace FinFinder.Web.Controllers
{
    [Authorize]
    public class LikeController : Controller
    {
      
        private readonly ILikeService _likeService;

        public LikeController( ILikeService likeService)
        {
          
            _likeService = likeService;
        }

        [HttpPost]
        public async Task<IActionResult> Add(Guid fishCatchId)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var success = await _likeService.AddLikeAsync(fishCatchId, userId);

            if (!success)
            {
                TempData["Error"] = "You have already liked this post.";
            }

            return RedirectToAction("Details", "FishCatch", new { id = fishCatchId });
        }

        [HttpPost]
        public async Task<IActionResult> Remove(Guid fishCatchId)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var success = await _likeService.RemoveLikeAsync(fishCatchId, userId);

            if (!success)
            {
                TempData["Error"] = "You have not liked this post.";
            }

            return RedirectToAction("Details", "FishCatch", new { id = fishCatchId });
        }
    }

}
