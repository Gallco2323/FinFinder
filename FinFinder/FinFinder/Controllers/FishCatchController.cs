using FinFinder.Data;
using FinFinder.Data.Models;
using FinFinder.Web.ViewModels.FishCatch;
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
        public IActionResult Index()
        {
            return View();
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
        // GET: FishCatch/Details/5
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

    }
}
