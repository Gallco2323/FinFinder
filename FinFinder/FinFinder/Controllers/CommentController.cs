using FinFinder.Data;
using FinFinder.Data.Models;
using FinFinder.Data.Repository.Interfaces;
using FinFinder.Services.Data.Interfaces;
using FinFinder.Web.ViewModels.Comment;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace FinFinder.Web.Controllers
{
    public class CommentController : Controller
    {
        
        
        private readonly ICommentService _commentService;

        public CommentController(  ICommentService commentService)
        {
            
           
            _commentService = commentService;
            
        }

        [HttpPost]
        public async Task<IActionResult> Add(AddCommentViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Details", "FishCatch", new { id = model.FishCatchId });
            }

            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

          await _commentService.AddCommentAsync(userId, model);

            return RedirectToAction("Details", "FishCatch", new { id = model.FishCatchId });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(Guid id)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var comment = await _commentService.GetCommentByIdAsync(id); // Fetch the comment

            if (comment == null || comment.UserId != userId)
            {
                return Unauthorized();
            }

            var success = await _commentService.DeleteCommentAsync(id, userId);

            if (!success)
            {
                return BadRequest("Unable to delete the comment.");
            }

            // Redirect using the associated FishCatchId
            return RedirectToAction("Details", "FishCatch", new { id = comment.FishCatchId });


        }
    }
}
