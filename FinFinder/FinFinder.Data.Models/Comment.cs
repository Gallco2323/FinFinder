using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinFinder.Data.Models
{
    using static Common.EntityValidationConstants.Comment;
    public class Comment
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        [MinLength(ContentMinLength)]
        [MaxLength(ContentMaxLength)]
        public string Content { get; set; } = null!;

        [Required]
        public DateTime DateCreated { get; set; }

        // Foreign Keys
        [Required]
        public Guid UserId { get; set; }
        [Required]
        public Guid FishCatchId { get; set; }

        // Navigation Properties
        [ForeignKey(nameof(UserId))]
        public ApplicationUser User { get; set; } = null!;

        [ForeignKey(nameof(FishCatchId))]
        public FishCatch FishCatch { get; set; } = null!;
    }

}
