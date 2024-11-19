using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinFinder.Data.Models
{
    using static Common.EntityValidationConstants.ApplicationUser;
    public class ApplicationUser : IdentityUser<Guid> // Specify Guid as the primary key type
    {
        public string? ProfilePictureURL { get; set; }
        [MinLength(BioMinLength)]
        [MaxLength(BioMaxLength)]
        public string? Bio { get; set; }
        [Required]
        public int FishCount { get; set; }

        // Navigation Properties
        public ICollection<FishCatch> FishCatches { get; set; } = new List<FishCatch>();
        public ICollection<Favorite> Favorites { get; set; } = new List<Favorite>();
        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
        public ICollection<Like> Likes { get; set; } = new List<Like>();
    }
}
