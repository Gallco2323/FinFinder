using System.ComponentModel.DataAnnotations;
using FinFinder.Data.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
namespace FinFinder.Web.ViewModels.FishCatch
{
    using static FinFinder.Common.EntityValidationConstants.FishCatch;
    public class FishCatchCreateViewModel
    {
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
        public string LocationName { get; set; } // Human-readable name

        [Required]
        public double Latitude { get; set; } // Geolocation Latitude
        [Required]
        public double Longitude { get; set; } // Geolocation Longitude

        [Required]
        public Guid FishingTechniqueId { get; set; }

        // List for selecting techniques
        public List<FishingTechnique> FishingTechniques { get; set; } = new List<FishingTechnique>();

        // Updated for photo upload
        public List<IFormFile> PhotoFiles { get; set; } = new List<IFormFile>();
    }

}
