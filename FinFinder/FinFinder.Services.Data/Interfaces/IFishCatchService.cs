using FinFinder.Data.Models;
using FinFinder.Web.ViewModels.FishCatch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinFinder.Services.Data.Interfaces
{
    public interface IFishCatchService
    {
        Task<FishCatchFilterViewModel> GetFilteredFishCatchesAsync(string selectedFilter, string searchTerm);
        Task<FishCatchDetailsViewModel> GetFishCatchDetailsAsync(Guid id, Guid? userId);
        Task<FishCatchCreateViewModel> PrepareCreateViewModelAsync();
        Task<bool> CreateFishCatchAsync(FishCatchCreateViewModel model, Guid userId);
        Task<FishCatchEditViewModel> PrepareEditViewModelAsync(Guid id, Guid userId);
        Task<bool> UpdateFishCatchAsync(FishCatchEditViewModel model, Guid userId);
        Task<bool> AddToFavoritesAsync(Guid fishCatchId, Guid userId);
        Task<bool> SoftDeleteFishCatchAsync(Guid fishCatchId, Guid userId);
        Task<bool> PermanentDeleteFishCatchAsync(Guid fishCatchId, Guid userId);
        Task<IEnumerable<ManageFishCatchViewModel>> GetAllFishCatchesAsync();
        Task<FishCatch> GetFishCatchByIdAsync(Guid id);

    }
}
