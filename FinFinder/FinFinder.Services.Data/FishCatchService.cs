using FinFinder.Data.Models;
using FinFinder.Data.Repository.Interfaces;
using FinFinder.Services.Data.Interfaces;
using FinFinder.Web.ViewModels.Comment;
using FinFinder.Web.ViewModels.FishCatch;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace FinFinder.Services.Data
{
    public class FishCatchService : IFishCatchService
    {
        private readonly IRepository<FishCatch, Guid> _fishCatchRepository;
        private readonly IRepository<FishingTechnique, Guid> _fishingTechniqueRepository;
        private readonly IRepository<Photo, Guid> _photoRepository;
        private readonly ICompositeKeyRepository<Favorite, Guid,Guid> _favoriteRepository;
        private readonly IRepository<Comment, Guid> _commentRepository;
        private readonly IRepository<Like, Guid> _likeRepository;

        public FishCatchService(
            IRepository<FishCatch, Guid> fishCatchRepository,
            IRepository<FishingTechnique, Guid> fishingTechniqueRepository,
            IRepository<Photo, Guid> photoRepository,
            ICompositeKeyRepository<Favorite, Guid, Guid> favoriteRepository,
        IRepository<Comment, Guid> commentRepository,
            IRepository<Like, Guid> likeRepository
            )
        {
            _fishCatchRepository = fishCatchRepository;
            _fishingTechniqueRepository = fishingTechniqueRepository;
            _photoRepository = photoRepository;
            _favoriteRepository = favoriteRepository;
            _commentRepository = commentRepository;
            _likeRepository = likeRepository;
        }

        public async Task<bool> AddToFavoritesAsync(Guid fishCatchId, Guid userId)
        {
            var existingFavorite = await _favoriteRepository
           .GetAllAttached()
           .Where(f => f.FishCatchId == fishCatchId && f.UserId == userId)
           .FirstOrDefaultAsync();

            if (existingFavorite != null)
                return false;

            var favorite = new Favorite
            {
                UserId = userId,
                FishCatchId = fishCatchId
            };

            await _favoriteRepository.AddAsync(favorite);
            return true;
        }
        public async Task<IEnumerable<ManageFishCatchViewModel>> GetAllFishCatchesAsync()
        {
            var fishCatches = await _fishCatchRepository.GetAllAttached()
                .Include(fc => fc.User) // Include publisher details
                .Include(fc => fc.Photos) // Include photos
                .ToListAsync();

            return fishCatches.Select(fc => new ManageFishCatchViewModel
            {
                Id = fc.Id,
                Species = fc.Species,
                LocationName = fc.LocationName,
                PublisherName = fc.User.UserName,
                DateCaught = fc.DateCaught,
                PhotoURLs = fc.Photos.Select(p => p.Url).ToList(),
                IsDeleted = fc.IsDeleted
            });
        }


        public async Task<bool> CreateFishCatchAsync(FishCatchCreateViewModel model, Guid userId)
        {
            var fishCatch = new FishCatch
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Species = model.Species,
                Description = model.Description,
                Weight = model.Weight,
                Length = model.Length,
                Latitude = model.Latitude,
                Longitude = model.Longitude,
                LocationName = model.LocationName,
                FishingTechniqueId = model.FishingTechniqueId,
                DateCaught = DateTime.UtcNow
            };

            await _fishCatchRepository.AddAsync(fishCatch);

            if (model.PhotoFiles != null && model.PhotoFiles.Any())
            {
                foreach (var file in model.PhotoFiles)
                {
                    var fileName = $"{Guid.NewGuid()}_{file.FileName}";
                    var filePath = Path.Combine("wwwroot/images", fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    var photo = new Photo
                    {
                        Url = $"/images/{fileName}",
                        FishCatchId = fishCatch.Id
                    };
                    await _photoRepository.AddAsync(photo);
                }
            }

            return true;
        }
    

        public async Task<FishCatchFilterViewModel> GetFilteredFishCatchesAsync(string selectedFilter, string searchTerm)
        {
            var fishCatches = await _fishCatchRepository
            .GetAllAttached()
            .Where(fc => fc.IsDeleted == false)
            .Include(fc => fc.User)
            .Include(fc => fc.Photos)
            .Include(fc => fc.Likes)
            .ToListAsync();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                fishCatches = fishCatches.Where(fc =>
                    fc.Species.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                    fc.LocationName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                    fc.User.UserName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            switch (selectedFilter)
            {
                case "MostLiked":
                    fishCatches = fishCatches.OrderByDescending(fc => fc.Likes.Count).ToList();
                    break;
                case "LeastLiked":
                    fishCatches = fishCatches.OrderBy(fc => fc.Likes.Count).ToList();
                    break;
                case "Alphabetical":
                    fishCatches = fishCatches.OrderBy(fc => fc.Species).ToList();
                    break;
                default: // DatePosted
                    fishCatches = fishCatches.OrderByDescending(fc => fc.DateCaught).ToList();
                    break;
            }

            var model = new FishCatchFilterViewModel
            {
                SelectedFilter = selectedFilter,
                Filters = new List<SelectListItem>
            {
                new SelectListItem { Value = "DatePosted", Text = "Date Posted" },
                new SelectListItem { Value = "MostLiked", Text = "Most Liked" },
                new SelectListItem { Value = "LeastLiked", Text = "Least Liked" },
                new SelectListItem { Value = "Alphabetical", Text = "Alphabetical" }
            },
                FishCatches = fishCatches.Select(fc => new FishCatchIndexViewModel
                {
                    Id = fc.Id,
                    Species = fc.Species,
                    LocationName = fc.LocationName,
                    DateCaught = fc.DateCaught,
                    PhotoURLs = fc.Photos.Select(p => p.Url).ToList(),
                    PublisherName = fc.User.UserName,
                    LikesCount = fc.Likes.Count
                }).ToList(),
                SearchTerm = searchTerm
            };

            return model;
        }

        public async Task<FishCatchDetailsViewModel> GetFishCatchDetailsAsync(Guid id, Guid? userId)
        {
            var fishCatch = await _fishCatchRepository
            .GetAllAttached()
            .Where(fc => fc.Id == id && fc.IsDeleted == false)
            .Include(fc => fc.User)
            .Include(fc => fc.Photos)
            .Include(fc => fc.Comments).ThenInclude(c => c.User)
            .Include(fc => fc.Likes)
            .FirstOrDefaultAsync();

            if (fishCatch == null) return null;

            return new FishCatchDetailsViewModel
            {
                Id = fishCatch.Id,
                Species = fishCatch.Species,
                Description = fishCatch.Description,
                Weight = fishCatch.Weight,
                Length = fishCatch.Length,
                LocationName = fishCatch.LocationName,
                Latitude = fishCatch.Latitude,
                Longitude = fishCatch.Longitude,
                DateCaught = fishCatch.DateCaught,
                Photos = fishCatch.Photos.Select(p => p.Url).ToList(),
                PublisherName = fishCatch.User.UserName,
                PublisherProfilePictureURL = fishCatch.User.ProfilePictureURL,
                PublisherId = fishCatch.User.Id,
                LikesCount = fishCatch.Likes.Count,
                IsLikedByCurrentUser = userId.HasValue && fishCatch.Likes.Any(l => l.UserId == userId.Value),
                Comments = fishCatch.Comments.Select(c => new CommentViewModel
                {
                    Id = c.Id,
                    UserId = c.UserId,
                    Content = c.Content,
                    UserName = c.User.UserName,
                    DateCreated = c.DateCreated
                }).ToList()
            };
        }

        public async Task<bool> PermanentDeleteFishCatchAsync(Guid fishCatchId, Guid userId)
        {
            var fishCatch = await _fishCatchRepository.GetAllAttached()
       .Include(fc => fc.Photos)
       .Include(fc => fc.Comments)
       .Include(fc => fc.Likes)
       .FirstOrDefaultAsync(fc => fc.Id == fishCatchId && fc.UserId == userId);

            if (fishCatch == null)
                return false;

            var photoIdsToDelete = fishCatch.Photos.Select(p => p.Id).ToList(); // Collect photo IDs
            foreach (var photoId in photoIdsToDelete)
            {
                var photo = await _photoRepository.GetByIdAsync(photoId); // Fetch the photo entity by ID
                if (photo != null)
                {
                    var filePath = Path.Combine("wwwroot", photo.Url.TrimStart('/'));
                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath);
                    }

                    _photoRepository.Delete(photo.Id); // Delete the photo by ID
                }
            }

            var favoritesToDelete = fishCatch.Favorites.ToList();
            foreach (var favorite in favoritesToDelete)
            {
                await _favoriteRepository.DeleteByCompositeKeyAsync(favorite.UserId, favorite.FishCatchId);
            }

            var commentsToDelete = fishCatch.Comments.ToList(); // Create a copy of the collection
            foreach (var comment in commentsToDelete)
            {
                await _commentRepository.DeleteAsync(comment.Id);
            }

            // Remove associated likes
            var likesToDelete = fishCatch.Likes.ToList(); // Create a copy of the collection
            foreach (var like in likesToDelete)
            {
                await _likeRepository.DeleteAsync(like.Id);
            }

            await _fishCatchRepository.DeleteAsync(fishCatch.Id);
            return true;
        }

        public async Task<FishCatchCreateViewModel> PrepareCreateViewModelAsync()
        {
            var fishingTechniques = await _fishingTechniqueRepository.GetAllAsync();
            return new FishCatchCreateViewModel
            {
                FishingTechniques = fishingTechniques.ToList()
            };
        }

        public async Task<FishCatchEditViewModel> PrepareEditViewModelAsync(Guid id, Guid userId)
        {
            var fishCatch = await _fishCatchRepository
           .GetAllAttached()
           .Where(fc => fc.Id == id && fc.UserId == userId && fc.IsDeleted == false)
           .Include(fc => fc.Photos)
           .FirstOrDefaultAsync();

            if (fishCatch == null)
                return null;

            var fishingTechniques = await _fishingTechniqueRepository.GetAllAsync();

            return new FishCatchEditViewModel
            {
                Id = fishCatch.Id,
                Species = fishCatch.Species,
                Description = fishCatch.Description,
                Weight = fishCatch.Weight,
                Length = fishCatch.Length,
                Latitude = fishCatch.Latitude,
                Longitude = fishCatch.Longitude,
                LocationName = fishCatch.LocationName,
                FishingTechniqueId = fishCatch.FishingTechniqueId,
                ExistingPhotos = fishCatch.Photos.ToList(),
                FishingTechniques = fishingTechniques.ToList()
            };
        }

        public async Task<bool> SoftDeleteFishCatchAsync(Guid fishCatchId, Guid userId)
        {
            var fishCatch = await _fishCatchRepository
            .GetAllAttached()
            .Where(fc => fc.Id == fishCatchId && fc.UserId == userId && fc.IsDeleted == false)
            .FirstOrDefaultAsync();

            if (fishCatch == null)
                return false;

            fishCatch.IsDeleted = true;
            await _fishCatchRepository.UpdateAsync(fishCatch);
            return true;
        }

        public async Task<bool> UpdateFishCatchAsync(FishCatchEditViewModel model, Guid userId)
        {
            var fishCatch = await _fishCatchRepository
           .GetAllAttached()
           .Where(fc => fc.Id == model.Id && fc.UserId == userId && fc.IsDeleted == false)
           .Include(fc => fc.Photos)
           .FirstOrDefaultAsync();

            if (fishCatch == null)
                return false;

            // Update FishCatch details
            fishCatch.Species = model.Species;
            fishCatch.Description = model.Description;
            fishCatch.Weight = model.Weight;
            fishCatch.Length = model.Length;
            fishCatch.Latitude = model.Latitude;
            fishCatch.Longitude = model.Longitude;
            fishCatch.LocationName = model.LocationName;
            fishCatch.FishingTechniqueId = model.FishingTechniqueId;

            // Remove selected photos
            if (model.PhotosToRemove != null && model.PhotosToRemove.Any())
            {
                var photosToDelete = fishCatch.Photos
                    .Where(p => model.PhotosToRemove.Contains(p.Id))
                    .ToList();

                foreach (var photo in photosToDelete)
                {
                    var filePath = Path.Combine("wwwroot", photo.Url.TrimStart('/'));
                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath);
                    }

                    _photoRepository.Delete(photo.Id);
                }
            }

            // Handle new photo uploads
            if (model.NewPhotoFiles != null && model.NewPhotoFiles.Any())
            {
                foreach (var file in model.NewPhotoFiles)
                {
                    var fileName = $"{Guid.NewGuid()}_{file.FileName}";
                    var filePath = Path.Combine("wwwroot/images", fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    var photo = new Photo
                    {
                        Url = $"/images/{fileName}",
                        FishCatchId = fishCatch.Id
                    };
                    await _photoRepository.AddAsync(photo);
                }
            }

            await _fishCatchRepository.UpdateAsync(fishCatch);
            return true;
        }
    }
}
