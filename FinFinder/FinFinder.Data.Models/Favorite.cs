using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace FinFinder.Data.Models
{
    [PrimaryKey(nameof(UserId),nameof(FishCatchId))]
    public class Favorite
    {
        
        
        [Required]
        [Key]
        public Guid UserId { get; set; }
        [ForeignKey(nameof(UserId))]
        public ApplicationUser User { get; set; } = null!;

        [Required]
        [Key]
        public Guid FishCatchId { get; set; }

        [ForeignKey(nameof(FishCatchId))]
        public FishCatch FishCatch { get; set; } = null!;
    }

}
