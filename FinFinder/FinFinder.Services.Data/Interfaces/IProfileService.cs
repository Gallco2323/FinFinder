using FinFinder.Web.ViewModels.FishCatch;
using FinFinder.Web.ViewModels.Profile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinFinder.Services.Data.Interfaces
{
    public interface IProfileService
    {
        Task<ProfileEditViewModel?> GetProfileForEditAsync(Guid userId);
        Task<bool> UpdateProfileAsync(Guid userId, ProfileEditViewModel model);
        Task<UserProfileViewModel?> GetProfileDetailsAsync(Guid userId);
        Task<List<FishCatchHiddenViewModel>> GetHiddenPostsAsync(Guid userId);
        Task<bool> UnhidePostAsync(Guid userId, Guid fishCatchId);
    }
}
