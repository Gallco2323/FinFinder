using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinFinder.Data.Models
{
    public class Like
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        // Foreign Key for User
        [Required]
        public Guid UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public ApplicationUser User { get; set; } = null!;

        // Foreign Key for FishCatch
        [Required]
        public Guid FishCatchId { get; set; }

        [ForeignKey(nameof(FishCatchId))]
        public FishCatch FishCatch { get; set; } = null!;

        // Optional timestamp for when the like was created
        public DateTime LikedOn { get; set; } = DateTime.UtcNow;
    }
}
