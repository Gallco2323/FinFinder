using Azure;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinFinder.Data.Models
{
    using static FinFinder.Common.EntityValidationConstants.FishCatch;
    public class FishCatch
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();


        [Required]
        [MinLength(SpeciesMinLength)]
        [MaxLength(SpeciesMaxLength)]
        public string Species { get; set; } = null!;

        [Required]
        [MinLength(LocationMinLength)]
        [MaxLength(LocationMaxLength)]
        public string Location { get; set; } = null!;


        [Required]
        [MinLength(DescriptionMinLength)]
        [MaxLength(DescriptionMaxLength)]
        public string Description { get; set; } = null!; 

        public string? PhotoURL { get; set; }

        [Required] 
        public DateTime DateCaught { get; set; } 

        [Required]
        [Range(MinWeight, MaxWeight)]
          public double Weight { get; set; }

        [Required]
        [Range(MinLength, MaxLength)]

        public double Length { get; set; }




        // Foreign Keys
        [Required]
        public Guid UserId { get; set; } 
        [Required]
        public Guid ObservationId { get; set; }
        [Required]
        public Guid FishingTechniqueId { get; set; }


        // Navigation Properties
        [ForeignKey(nameof(UserId))]
        public ApplicationUser User { get; set; } = null!;

        [ForeignKey(nameof(ObservationId))]
        public Observation? Observation { get; set; } 

        [ForeignKey((nameof(FishingTechniqueId)))]
        public FishingTechnique FishingTechnique { get; set; } = null!;

        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
        public ICollection<Favorite> Favorites { get; set; } = new List<Favorite>();
        
        
    }
}
