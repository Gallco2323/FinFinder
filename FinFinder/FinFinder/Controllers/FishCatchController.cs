using FinFinder.Data;
using FinFinder.Data.Models;
using FinFinder.Web.ViewModels.FishCatch;
using Microsoft.AspNetCore.Authorization;
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

        public FishCatchController(FinFinderDbContext context)
        {
            _context = context;
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var fishCatches = await _context.FishCatches
                .AsNoTracking()
                .Include(f => f.User) // Include the publisher details
                .ToListAsync();

            var model = fishCatches.Select(f => new FishCatchIndexViewModel
            {
                Id = f.Id,
                Species = f.Species,
                Location = f.Location,
                DateCaught = f.DateCaught,
                PhotoURL = f.PhotoURL ?? "/images/default-fish.jpg", // Default image if PhotoURL is null
                PublisherName = f.User.UserName,
                PublisherId = f.UserId.ToString()
            }).ToList();

            return View(model);
        }
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var model = new FishCatchCreateViewModel();
            model.FishingTechniques =  await this._context.FishingTechniques.ToListAsync();
            
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(FishCatchCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.FishingTechniques = this._context.FishingTechniques.ToList();
                return View(model);
            }
            string photoUrl = null;
            if (model.Photo != null)
            {
                // Define the path to save the uploaded file
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");
                var uniqueFileName = $"{Guid.NewGuid()}_{model.Photo.FileName}";
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                // Save the file to the specified path
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await model.Photo.CopyToAsync(fileStream);
                }

                // Set the photoUrl to save in the database
                photoUrl = $"/images/{uniqueFileName}";
            }

            // Create a new FishCatch entity with the uploaded photo URL
            var fishCatch = new FishCatch
            {
                Id = Guid.NewGuid(),
                UserId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)), // Convert UserId to Guid
                Species = model.Species,
                Description = model.Description,
                Weight = model.Weight,
                Length = model.Length,
               
                Location = model.Location,
                FishingTechniqueId = model.FishingTechniqueId,
                PhotoURL = photoUrl,
                DateCaught = DateTime.Now// This will store the current date and time
            };

            _context.Add(fishCatch);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));



        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null) return NotFound();

            var fishCatch = await _context.FishCatches.FindAsync(id);
            if (fishCatch == null || fishCatch.UserId != Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)))
                return Unauthorized();

            var model = new FishCatchEditViewModel()
            {
                Id = fishCatch.Id,
                Species = fishCatch.Species,
                Description = fishCatch.Description,
                Weight = fishCatch.Weight,
                Length = fishCatch.Length,
                Location = fishCatch.Location,
                FishingTechniqueId = fishCatch.FishingTechniqueId,
                PhotoUrl = fishCatch.PhotoURL,
                FishingTechniques = this._context.FishingTechniques.ToList()
            };

            
            
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Guid id, FishCatchEditViewModel model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            ModelState.Remove("Photo");
            if (!ModelState.IsValid)
            {
                model.FishingTechniques =await  _context.FishingTechniques.ToListAsync();
                return View(model);
            }
                var fishCatch = await _context.FishCatches.FindAsync(id);
                if (fishCatch == null || fishCatch.UserId != Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)))
                {
                    return Unauthorized();
                }

                fishCatch.Species = model.Species;
                fishCatch.Description = model.Description;
                fishCatch.Weight = model.Weight;
                fishCatch.Length = model.Length;
                fishCatch.Location = model.Location;
                fishCatch.FishingTechniqueId = model.FishingTechniqueId;

                // Check if a new photo was uploaded
                if (model.Photo != null)
                {
                    // Save the new photo
                    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");
                    var uniqueFileName = $"{Guid.NewGuid()}_{model.Photo.FileName}";
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await model.Photo.CopyToAsync(fileStream);
                    }

                    // Set the new photo URL
                    fishCatch.PhotoURL = $"/images/{uniqueFileName}";
                }

                _context.Update(fishCatch);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }



            [HttpGet]
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null) return NotFound();

            var fishCatch = await _context.FishCatches
                                           .Include(f => f.FishingTechnique)
                                           .Include(f => f.User) // Join with ApplicationUser
                                           .FirstOrDefaultAsync(m => m.Id == id);

            if (fishCatch == null) return NotFound();

            var model = new FishCatchDetailsViewModel
            {
                Id = fishCatch.Id,
                Species = fishCatch.Species,
                Description = fishCatch.Description,
                Weight = fishCatch.Weight,
                Length = fishCatch.Length,
               
                Location = fishCatch.Location,
                FishingTechniqueName = fishCatch.FishingTechnique.Name,
                PhotoURL = fishCatch.PhotoURL,
                DateCaught = fishCatch.DateCaught,

                // Publisher details
                PublisherName = fishCatch.User.UserName,
                
                PublisherProfilePictureURL = fishCatch.User.ProfilePictureURL
            };
            return View(model);
        }

        [Authorize]
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

            return RedirectToAction("Details", new { id = id });
        }

    }
}
