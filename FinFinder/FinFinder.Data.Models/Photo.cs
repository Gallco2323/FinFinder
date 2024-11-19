using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinFinder.Data.Models
{
    public class Photo
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public string? Url { get; set; } 

        
        [Required]
        public Guid FishCatchId { get; set; }

        [ForeignKey(nameof(FishCatchId))]
        public FishCatch FishCatch { get; set; } = null!;
    }
}
