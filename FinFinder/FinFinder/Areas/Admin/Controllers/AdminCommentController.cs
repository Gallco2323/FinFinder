using FinFinder.Services.Data.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinFinder.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Administrator")]
    public class AdminCommentController : Controller
    {
        private readonly ICommentService _commentService;

        public AdminCommentController(ICommentService commentService)
        {
            _commentService = commentService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var comments = await _commentService.GetAllCommentsAsync();
            return View(comments);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(Guid id)
        {
            var success = await _commentService.AdminDeleteCommentAsync(id);

            if (!success)
            {
                ModelState.AddModelError("", "Failed to delete the comment.");
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
