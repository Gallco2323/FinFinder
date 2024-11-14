using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinFinder.Data.Models
{
    public class ApplicationUser : IdentityUser<Guid> // Specify Guid as the primary key type
    {
        public string ProfilePictureURL { get; set; }
        public string Bio { get; set; }

        // Navigation Properties
        public ICollection<FishCatch> FishCatches { get; set; }
        public ICollection<Favorite> Favorites { get; set; }
        public ICollection<Comment> Comments { get; set; }
        public ICollection<Observation> Observations { get; set; }
    }
}
