using FinFinder.Data.Models;
using FinFinder.Data.Repository.Interfaces;
using FinFinder.Services.Data.Interfaces;
using FinFinder.Web.ViewModels.FishCatch;
using FinFinder.Web.ViewModels.Profile;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinFinder.Services.Data
{
    using Mapping;
    public class ProfileService: IProfileService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IRepository<FishCatch, Guid> _fishCatchRepository;

        public ProfileService(UserManager<ApplicationUser> userManager, IRepository<FishCatch, Guid> fishCatchRepository)
        {
            _userManager = userManager;
            _fishCatchRepository = fishCatchRepository;
        }

        public async Task<List<FishCatchHiddenViewModel>> GetHiddenPostsAsync(Guid userId)
        {
            var hiddenPosts = await _fishCatchRepository.GetAllAttached()
                .Include(fc => fc.Photos)
                .Where(fc => fc.IsDeleted && fc.UserId == userId)
                .Select(fc => new FishCatchHiddenViewModel
                {
                    Id = fc.Id,
                    Species = fc.Species,
                    LocationName = fc.LocationName,
                    DateCaught = fc.DateCaught,
                    PhotoURLs = fc.Photos.Select(p => p.Url).ToList()
                })
                .ToListAsync();


            return hiddenPosts;
        }

        public async Task<UserProfileViewModel?> GetProfileDetailsAsync(Guid userId)
        {
            var user = await _userManager.Users
            .Include(u => u.FishCatches)
            .ThenInclude(fc => fc.Photos)
            .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
            {
                return null;
            }

            return new UserProfileViewModel
            {
                UserId = user.Id,
                UserName = user.UserName ?? string.Empty,
                Bio = user.Bio,
                ProfilePictureURL = user.ProfilePictureURL ?? "/images/default-profile.png",
                FishCount = user.FishCount,
                FishCatches = user.FishCatches
                    .Where(fc => !fc.IsDeleted)
                    .Select(fc => new FishCatchIndexViewModel
                    {
                        Id = fc.Id,
                        Species = fc.Species,
                        LocationName = fc.LocationName,
                        DateCaught = fc.DateCaught,
                        PhotoURLs = fc.Photos.Select(p => p.Url).ToList(),
                        PublisherName = user.UserName ?? string.Empty
                    }).ToList()
            };
        }

        public async Task<ProfileEditViewModel?> GetProfileForEditAsync(Guid userId)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == userId);
          
            if (user == null)
            {
                return null;
            }

            return new ProfileEditViewModel
            {
                UserName = user.UserName,
                Bio = user.Bio,
                ProfilePictureURL = user.ProfilePictureURL
            };
        }

        public async Task<bool> UnhidePostAsync(Guid userId, Guid fishCatchId)
        {
            var fishCatch = await _fishCatchRepository.GetAllAttached()
                .FirstOrDefaultAsync(fc => fc.Id == fishCatchId && fc.UserId == userId);

            if (fishCatch == null)
            {
                return false;
            }

            fishCatch.IsDeleted = false;
            await _fishCatchRepository.UpdateAsync(fishCatch);
            return true;
        }

        public async Task<bool> UpdateProfileAsync(Guid userId, ProfileEditViewModel model)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
            {
                return false;
            }

            user.UserName = model.UserName;
            user.Bio = model.Bio;

            // Handle new profile picture
            if (model.ProfilePicture != null)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/profiles");
                Directory.CreateDirectory(uploadsFolder); // Ensure directory exists
                var uniqueFileName = $"{Guid.NewGuid()}_{model.ProfilePicture.FileName}";
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await model.ProfilePicture.CopyToAsync(fileStream);
                }

                user.ProfilePictureURL = $"/images/profiles/{uniqueFileName}";
            }

            var result = await _userManager.UpdateAsync(user);
            return result.Succeeded;

        }
    }
}
