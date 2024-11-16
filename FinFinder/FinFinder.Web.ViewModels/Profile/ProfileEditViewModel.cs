using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinFinder.Web.ViewModels.Profile
{
    using static Common.EntityValidationConstants.ApplicationUser;
    public class ProfileEditViewModel
    {
        [Required]
        [MinLength(UserNameMinLength)]
        [MaxLength(UserNameMaxLength)]
       
        public required string UserName { get; set; }
        [MinLength(BioMinLength)]
        [MaxLength(BioMaxLength)]

        public string? Bio { get; set; }

        // Profile picture file upload
        public IFormFile? ProfilePicture { get; set; }

        // To display the current profile picture if available
        public string? ProfilePictureURL { get; set; }
    }
}
