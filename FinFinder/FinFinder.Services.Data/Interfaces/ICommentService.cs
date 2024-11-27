using FinFinder.Data.Models;
using FinFinder.Web.ViewModels.Comment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinFinder.Services.Data.Interfaces
{
    public interface ICommentService
    {
        
        Task AddCommentAsync(Guid userId, AddCommentViewModel model);

        Task<bool> DeleteCommentAsync(Guid commentId, Guid userId);

        
        Task<IEnumerable<Comment>> GetCommentsForFishCatchAsync(Guid fishCatchId);

        
        Task<Comment?> GetCommentByIdAsync(Guid commentId);

        
        Task<bool> CanUserDeleteCommentAsync(Guid commentId, Guid userId);
    }
}
