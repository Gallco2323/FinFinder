using FinFinder.Data;
using FinFinder.Data.Models;
using FinFinder.Web.ViewModels.Comment;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace FinFinder.Web.Controllers
{
    public class CommentController : Controller
    {
        private readonly FinFinderDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public CommentController(FinFinderDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpPost]
        public async Task<IActionResult> Add(AddCommentViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Details", "FishCatch", new { id = model.FishCatchId });
            }

            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var comment = new Comment
            {
                Id = Guid.NewGuid(),
                Content = model.Content,
                FishCatchId = model.FishCatchId,
                UserId = userId,
                DateCreated = DateTime.UtcNow
            };

            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();

            return RedirectToAction("Details", "FishCatch", new { id = model.FishCatchId });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(Guid id)
        {
            var comment = await _context.Comments
                .FirstOrDefaultAsync(c => c.Id == id);

            if (comment == null || comment.UserId != Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)))
            {
                return Unauthorized();
            }

            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();

            return RedirectToAction("Details", "FishCatch", new { id = comment.FishCatchId });
        }
    }
}
