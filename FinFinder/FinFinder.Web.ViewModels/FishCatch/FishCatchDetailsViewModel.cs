using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinFinder.Web.ViewModels.FishCatch
{
    using FinFinder.Data.Models;
    using FinFinder.Web.ViewModels.Comment;

    public class FishCatchDetailsViewModel
    {
        public Guid Id { get; set; }

        [Required]
        public string Species { get; set; }

        [Required]
        public string Description { get; set; }

        public double Weight { get; set; }
        public double Length { get; set; }

        public bool IsFavorite { get; set; } = false; 
        // Updated Location
        [Required]
        public string LocationName { get; set; } // Display-friendly name
        public double Latitude { get; set; }
        public double Longitude { get; set; }

        [Required]
        public string FishingTechniqueName { get; set; }

        // Support for multiple photos
        public List<string> Photos { get; set; } = new List<string>();

        public List<CommentViewModel> Comments { get; set; } = new List<CommentViewModel>();

        public DateTime DateCaught { get; set; }

        // Publisher Info
        [Required]
        public string PublisherName { get; set; }
        public string? PublisherProfilePictureURL { get; set; }
        public Guid PublisherId { get; set; }

        public int LikesCount { get; set; }
        public bool IsLikedByCurrentUser { get; set; }
    }


}
