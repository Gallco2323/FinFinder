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

        public  string Species { get; set; }

        [Required]
        [MinLength(DescriptionMinLength)]
        [MaxLength(DescriptionMaxLength)]
        public  string Description { get; set; }

        public IFormFile Photo { get; set; }

       

        [Required]
        [Range(MinWeight, MaxWeight)]

        public double Weight { get; set; }

        [Required]
        [Range(MinLength, MaxLength)]
        public double Length { get; set; }

        [Required]
        [MinLength(LocationMinLength)]
        [MaxLength(LocationMaxLength)]

        public  string Location { get; set; }

        [Required]
        public Guid FishingTechniqueId { get; set; }

        // List for selecting techniques
        public List<FishingTechnique> FishingTechniques { get; set; }  = new List<FishingTechnique>();
    }
}
