using FinFinder.Web.ViewModels.FishCatch;
using FinFinder.Web.ViewModels.Home;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinFinder.Services.Data.Interfaces
{
    public interface IHomeService
    {
        Task<List<FishCatchIndexViewModel>> GetFeaturedFishCatchesAsync();
        Task<int> GetTotalFishCatchesAsync();
        Task<int> GetTotalUsersAsync();
        Task<string> GetMostPopularTechniqueAsync();
        Task<List<ActivityViewModel>> GetRecentActivitiesAsync();
        Task<List<FishCatchIndexViewModel>> GetUserRecentCatchesAsync(Guid userId);
    }
}
