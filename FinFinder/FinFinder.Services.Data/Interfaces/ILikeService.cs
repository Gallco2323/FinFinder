using FinFinder.Web.ViewModels.FishCatch;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinFinder.Services.Data.Interfaces
{
    public interface ILikeService
    {
        Task<bool> AddLikeAsync(Guid fishCatchId, Guid userId);

        Task<bool> RemoveLikeAsync(Guid fishCatchId, Guid userId);

          Task<IEnumerable<FishCatchIndexViewModel>> GetLikedPostsAsync(Guid userId);
    }
}
