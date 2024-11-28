using FinFinder.Data.Models;
using FinFinder.Data.Repository.Interfaces;
using FinFinder.Services.Data.Interfaces;
using FinFinder.Web.ViewModels.FishCatch;
using FinFinder.Web.ViewModels.Home;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinFinder.Services.Data
{
    public class HomeService : IHomeService
    {
        private readonly IRepository<FishCatch, Guid> _fishCatchRepository;
        private readonly IRepository<FishingTechnique, Guid> _fishingTechniqueRepository;
        private readonly UserManager<ApplicationUser> _userManager;

        public HomeService(
            IRepository<FishCatch, Guid> fishCatchRepository,
            IRepository<FishingTechnique, Guid> fishingTechniqueRepository,
            UserManager<ApplicationUser> userManager)
        {
            _fishCatchRepository = fishCatchRepository;
            _fishingTechniqueRepository = fishingTechniqueRepository;
            _userManager = userManager;
        }
        public async Task<List<FishCatchIndexViewModel>> GetFeaturedFishCatchesAsync()
        {
            var featuredCatches = await _fishCatchRepository
                .GetAllAttached()
                .Where(fc => !fc.IsDeleted)
            .OrderByDescending(fc => fc.Likes.Count) // Most liked
            .Take(5)
            .Include(fc => fc.Photos)
            .Include(fc => fc.User)
            .Select(fc => new FishCatchIndexViewModel
            {
                Id = fc.Id,
                Species = fc.Species,
                LocationName = fc.LocationName,
                DateCaught = fc.DateCaught,
                PhotoURLs = fc.Photos.Select(ph => ph.Url).ToList(),
                PublisherName = fc.User.UserName
            })
            .ToListAsync();

            return featuredCatches;
        }

        public async Task<string> GetMostPopularTechniqueAsync()
        {
            return await _fishingTechniqueRepository
                .GetAllAttached()
                .Where(ft => ft.FishCatches.Any())
           .OrderByDescending(ft => ft.FishCatches.Count)
           .Select(ft => ft.Name)
           .FirstOrDefaultAsync() ?? "None";
        }

        public async Task<List<ActivityViewModel>> GetRecentActivitiesAsync()
        {
            var recentActivities = await _fishCatchRepository
            .GetAllAttached()
            .OrderByDescending(fc => fc.DateCaught)
            .Take(5)
            .Include(fc => fc.User)
            .Select(fc => new ActivityViewModel
            {
                UserName = fc.User.UserName,
                ActionDescription = $"posted a new catch: {fc.Species}",
                Timestamp = fc.DateCaught
            })
            .ToListAsync();

            return recentActivities;
        }

        public async Task<int> GetTotalFishCatchesAsync()
        {
            return await _fishCatchRepository.GetAllAttached().Where(f=>f.IsDeleted==false).CountAsync();
        }

        public async Task<int> GetTotalUsersAsync()
        {
            return await _userManager.Users.CountAsync();
        }

        public async Task<List<FishCatchIndexViewModel>> GetUserRecentCatchesAsync(Guid userId)
        {
            var userRecentCatches = await _fishCatchRepository
            .GetAllAttached()
            .Where(fc => !fc.IsDeleted && fc.UserId == userId)
            .OrderByDescending(fc => fc.DateCaught)
            .Take(3)
            .Include(fc => fc.Photos)
            .Select(fc => new FishCatchIndexViewModel
            {
                Id = fc.Id,
                Species = fc.Species,
                LocationName = fc.LocationName,
                DateCaught = fc.DateCaught,
                PhotoURLs = fc.Photos.Select(ph => ph.Url).ToList()
            })
            .ToListAsync();

            return userRecentCatches;
        }
    }
}
