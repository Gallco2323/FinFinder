using FinFinder.Data.Models;
using FinFinder.Data.Repository.Interfaces;
using FinFinder.Services.Data.Interfaces;
using FinFinder.Web.ViewModels.Comment;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinFinder.Services.Data
{
    public class CommentService : ICommentService
    {
        private readonly IRepository<Comment, Guid> _commentRepository;

        public CommentService(IRepository<Comment, Guid> commentRepository)
        {
           _commentRepository = commentRepository;
        }
        public async Task AddCommentAsync(Guid userId, AddCommentViewModel model)
        {
            var comment = new Comment
            {
                Id = Guid.NewGuid(),
                Content = model.Content,
                FishCatchId = model.FishCatchId,
                UserId = userId,
                DateCreated = DateTime.UtcNow
            };

            await _commentRepository.AddAsync(comment);
        }

        public async Task<bool> CanUserDeleteCommentAsync(Guid commentId, Guid userId)
        {
            var comment = await _commentRepository.GetByIdAsync(commentId);
            return comment != null && comment.UserId == userId;
        }

        public async Task<bool> DeleteCommentAsync(Guid commentId, Guid userId)
        {
            var comment = await _commentRepository.GetByIdAsync(commentId);
            if (comment == null || comment.UserId != userId)
            {
                return false; // Unauthorized or not found
            }

            return await _commentRepository.DeleteAsync(commentId);
        }
        public async Task<bool> AdminDeleteCommentAsync(Guid commentId)
        {
            var comment = await _commentRepository.GetByIdAsync(commentId);

            if (comment == null)
            {
                return false; // Not found
            }

            return await _commentRepository.DeleteAsync(commentId);
        }
        public async Task<IEnumerable<CommentViewModel>> GetAllCommentsAsync()
        {
            var comments = await _commentRepository
      .GetAllAttached() // Using GetAllAttached to allow Include
      .Include(c => c.User) // Include User for each Comment
      .ToListAsync();

            return comments.Select(c => new CommentViewModel
            {
                Id = c.Id,
                Content = c.Content,
                UserName = c.User?.UserName ?? "Unknown",
             FishCatchId = c.FishCatchId.ToString(),
                DateCreated = c.DateCreated
            });
        }

        public async Task<Comment?> GetCommentByIdAsync(Guid commentId)
        {
            return await _commentRepository.GetByIdAsync(commentId);

        }

        public async Task<IEnumerable<Comment>> GetCommentsForFishCatchAsync(Guid fishCatchId)
        {
            return await _commentRepository.FindAsync(c => c.FishCatchId == fishCatchId);
        }
    }
}
