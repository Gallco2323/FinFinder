using FinFinder.Web.ViewModels.FishCatch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinFinder.Services.Data.Interfaces
{
    public interface IFavoriteService
    {

        Task<IEnumerable<FishCatchFavoriteViewModel>> GetUserFavoritesAsync(Guid userId);

        Task<bool> RemoveFavoriteAsync(Guid userId, Guid fishCatchId);
    }
}
