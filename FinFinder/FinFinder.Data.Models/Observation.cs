using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinFinder.Data.Models
{
    using static Common.EntityValidationConstants.Observation;
    public class Observation
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        [MinLength(BaitMinLength)]
        [MaxLength(BaitMaxLength)]
        public string Bait { get; set; } = null!;

        
        [MaxLength(NotesMaxLength)]
        [MinLength(NotesMinLength)]
        public string? Notes { get; set; }
        [Required]
        public Guid FishCatchId { get; set; }

        [ForeignKey(nameof(FishCatchId))]
        public FishCatch FishCatch { get; set; } = null!;
    }
}
