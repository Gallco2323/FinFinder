using FinFinder.Data.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinFinder.Web.ViewModels.FishCatch
{
    using static FinFinder.Common.EntityValidationConstants.FishCatch;
    public class FishCatchEditViewModel
    {
        [Required]
        public Guid Id { get; set; }

        [Required]
        [MinLength(SpeciesMinLength)]
        [MaxLength(SpeciesMaxLength)]
        public string Species { get; set; }

        [Required]
        [MinLength(DescriptionMinLength)]
        [MaxLength(DescriptionMaxLength)]
        public string Description { get; set; }

        [Required]
        [Range(MinWeight, MaxWeight)]
        public double Weight { get; set; }

        [Required]
        [Range(MinLength, MaxLength)]
        public double Length { get; set; }

        // Location
        [Required]
        [MinLength(LocationMinLength)]
        [MaxLength(LocationMaxLength)]
        public string LocationName { get; set; } // Editable location name
        [Required]
        public double Latitude { get; set; }
        [Required]
        public double Longitude { get; set; }

        [Required]
        public Guid FishingTechniqueId { get; set; }

        // List for selecting techniques
        public List<FishingTechnique> FishingTechniques { get; set; } = new List<FishingTechnique>();

        // Updated for multiple photos
        public List<IFormFile> NewPhotoFiles { get; set; } = new List<IFormFile>();
        public List<Photo> ExistingPhotos { get; set; } = new List<Photo>();

        public List<Guid> PhotosToRemove { get; set; } = new List<Guid>();
    }

}
