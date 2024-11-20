using FinFinder.Data;
using FinFinder.Data.Models;
using FinFinder.Web.ViewModels.FishCatch;
using FinFinder.Web.ViewModels.Profile;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using System.Security.Claims;

namespace FinFinder.Web.Controllers
{
    using static Common.EntityValidationConstants.FishCatch;
    public class FishCatchController : Controller
    {
        private readonly FinFinderDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public FishCatchController(FinFinderDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // INDEX
        public async Task<IActionResult> Index()
        {
            var fishCatches = await _context.FishCatches
                .Where(fc => !fc.IsDeleted)
                .AsNoTracking()
                .Include(f => f.User)
                .Include(f => f.Photos)
                .ToListAsync();

            var model = fishCatches.Select(f => new FishCatchIndexViewModel
            {
                Id = f.Id,
                Species = f.Species,
                LocationName = f.LocationName,
                Latitude = f.Latitude,
                Longitude = f.Longitude,
                DateCaught = f.DateCaught,
                PhotoURLs = f.Photos.Select(p => p.Url).ToList(),
                PublisherName = f.User.UserName,
                PublisherId = f.UserId.ToString()
            }).ToList();

            return View(model);
        }

        // CREATE GET
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var model = new FishCatchCreateViewModel
            {
                FishingTechniques = await _context.FishingTechniques.ToListAsync()
            };

            return View(model);
        }

        // CREATE POST
        [HttpPost]
        public async Task<IActionResult> Create(FishCatchCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.FishingTechniques = await _context.FishingTechniques.ToListAsync();
                return View(model);
            }

            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
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

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            user.FishCount++;

            _context.FishCatches.Add(fishCatch);
            await _context.SaveChangesAsync();

            // Handle photo uploads
            if (model.PhotoFiles != null && model.PhotoFiles.Any())
            {
                var photos = new List<Photo>();
                foreach (var file in model.PhotoFiles)
                {
                    var fileName = $"{Guid.NewGuid()}_{file.FileName}";
                    var filePath = Path.Combine("wwwroot/images", fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    photos.Add(new Photo
                    {
                        Url = $"/images/{fileName}",
                        FishCatchId = fishCatch.Id
                    });
                }

                _context.Photos.AddRange(photos);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        // EDIT GET
        [HttpGet]
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null) return NotFound();

            var fishCatch = await _context.FishCatches
                .Include(fc => fc.Photos)
                .FirstOrDefaultAsync(fc => fc.Id == id);

            if (fishCatch == null || fishCatch.UserId != Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)))
                return Unauthorized();

            var model = new FishCatchEditViewModel
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
                FishingTechniques = await _context.FishingTechniques.ToListAsync()
            };

            return View(model);
        }

        // EDIT POST
        [HttpPost]
        [HttpPost]
        public async Task<IActionResult> Edit(Guid id, FishCatchEditViewModel model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                model.FishingTechniques = await _context.FishingTechniques.ToListAsync();
                model.ExistingPhotos = await _context.Photos
                    .Where(p => p.FishCatchId == id)
                    .ToListAsync();
                return View(model);
            }

            var fishCatch = await _context.FishCatches
                .Include(fc => fc.Photos)
                .FirstOrDefaultAsync(fc => fc.Id == id);

            if (fishCatch == null || fishCatch.UserId != Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)))
            {
                return Unauthorized();
            }

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

                    _context.Photos.Remove(photo);
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
                    _context.Photos.Add(photo);
                }
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }



        // DETAILS
        [HttpGet]
        public async Task<IActionResult> Details(Guid id)
        {
            var fishCatch = await _context.FishCatches
                .Include(fc => fc.Photos)
                .Include(fc => fc.User)
                .FirstOrDefaultAsync(fc => fc.Id == id);

            if (fishCatch == null || fishCatch.IsDeleted)
                return NotFound();

            var model = new FishCatchDetailsViewModel
            {
                Id = fishCatch.Id,
                Species = fishCatch.Species,
                Description = fishCatch.Description,
                Weight = fishCatch.Weight,
                Length = fishCatch.Length,
                Latitude = fishCatch.Latitude,
                Longitude = fishCatch.Longitude,
                LocationName = fishCatch.LocationName,
                DateCaught = fishCatch.DateCaught,
                FishingTechniqueName = _context.FishingTechniques.FirstOrDefault(ft=>ft.Id == fishCatch.FishingTechniqueId).Name,
                Photos = fishCatch.Photos.Select(p => p.Url).ToList(),
                PublisherName = fishCatch.User.UserName,
                PublisherProfilePictureURL = fishCatch.User.ProfilePictureURL,
                PublisherId = fishCatch.User.Id
            };

            return View(model);
        }

        // ADD TO FAVORITES
        [HttpPost]
        public async Task<IActionResult> AddToFavorites(Guid id)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var existingFavorite = await _context.Favorites
                .FirstOrDefaultAsync(f => f.UserId == userId && f.FishCatchId == id);

            if (existingFavorite == null)
            {
                var favorite = new Favorite
                {
                    UserId = userId,
                    FishCatchId = id
                };
                _context.Favorites.Add(favorite);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Details", new { id });
        }
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> SoftDelete(Guid id)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var fishCatch = await _context.FishCatches
                .FirstOrDefaultAsync(fc => fc.Id == id && fc.UserId == userId);

            if (fishCatch == null)
            {
                return Unauthorized();
            }

            fishCatch.IsDeleted = true; // Perform soft delete
            _context.Update(fishCatch);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> PermanentDelete(Guid id)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var fishCatch = await _context.FishCatches
                .Include(fc => fc.Photos)
                .FirstOrDefaultAsync(fc => fc.Id == id && fc.UserId == userId);

            if (fishCatch == null)
            {
                return Unauthorized();
            }

            // Delete associated photos
            if (fishCatch.Photos.Any())
            {
                var photoPaths = fishCatch.Photos.Select(p => Path.Combine("wwwroot", p.Url.TrimStart('/')));
                foreach (var path in photoPaths)
                {
                    if (System.IO.File.Exists(path))
                    {
                        System.IO.File.Delete(path);
                    }
                }
                _context.Photos.RemoveRange(fishCatch.Photos);
            }

            _context.FishCatches.Remove(fishCatch); // Permanent delete
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
