using FinFinder.Data.Models;
using FinFinder.Data.Repository.Interfaces;
using FinFinder.Services.Data.Interfaces;
using FinFinder.Web.ViewModels.FishCatch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FinFinder.Services.Mapping;

namespace FinFinder.Services.Data
{
    using Mapping;
    using Microsoft.EntityFrameworkCore;

    public class FavoriteService : IFavoriteService
    {
        private IRepository<Favorite, object> _favoriteRepository;
        private IRepository<FishCatch, Guid> _fishCatchRepository;

        public FavoriteService(IRepository<Favorite, object> favoriteRepository, IRepository<FishCatch, Guid> fishCatchRepository)
        {
            _favoriteRepository = favoriteRepository;
            _fishCatchRepository = fishCatchRepository;
        }
        public async Task<IEnumerable<FishCatchFavoriteViewModel>> GetUserFavoritesAsync(Guid userId)
        {
            IEnumerable<FishCatchFavoriteViewModel> favorites = await _favoriteRepository
                .GetAllAttached()
         .Where(f => f.FishCatch.IsDeleted == false) // Exclude soft-deleted FishCatches
         .Where(f => f.UserId == userId) // Favorites for the current user
         .Include(f => f.FishCatch) // Include FishCatch details
         .ThenInclude(fc => fc.Photos) // Include associated photos
         .Include(f => f.FishCatch.User) // Include publisher details
         .Select(f => new FishCatchFavoriteViewModel
         {
             FishCatchId = f.FishCatchId,
             Species = f.FishCatch.Species,
             LocationName = f.FishCatch.LocationName, // Use only the display-friendly location name
             DateCaught = f.FishCatch.DateCaught,
             PhotoURLs = f.FishCatch.Photos.Select(p => p.Url).ToList(),
             PublisherName = f.FishCatch.User.UserName
         })
         .ToArrayAsync();

            return favorites;
        }

        public async Task<bool> RemoveFavoriteAsync(Guid userId, Guid fishCatchId)
        {
            var favorite = await _favoriteRepository.
                GetAllAttached()
                .Where(f => f.UserId == userId && f.FishCatchId == fishCatchId)
                .FirstOrDefaultAsync();
                

            if (favorite == null)
            {
                return false;
            }

            await _favoriteRepository.DeleteAsync(favorite.FishCatchId);
            return true;
        }
    }
}
