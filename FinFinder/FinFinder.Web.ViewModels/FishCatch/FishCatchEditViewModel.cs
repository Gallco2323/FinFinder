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

        public required string Species { get; set; }

        [Required]
        [MinLength(DescriptionMinLength)]
        [MaxLength(DescriptionMaxLength)]
        public required string Description { get; set; }

        [Required]
        [Range(MinWeight, MaxWeight)]

        public double Weight { get; set; }

        [Required]
        [Range(MinLength, MaxLength)]
        public double Length { get; set; }

        [Required]
        [MinLength(LocationMinLength)]
        [MaxLength(LocationMaxLength)]

        public required string Location { get; set; }


        public IFormFile? Photo { get; set; }


        public string? PhotoUrl { get; set; }
        [Required]
        public Guid FishingTechniqueId { get; set; }

        // List for selecting techniques
        public List<FishingTechnique> FishingTechniques { get; set; } = new List<FishingTechnique>();
    }
}
