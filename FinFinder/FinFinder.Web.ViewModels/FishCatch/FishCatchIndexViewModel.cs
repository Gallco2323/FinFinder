using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinFinder.Web.ViewModels.FishCatch
{
    public class FishCatchIndexViewModel
    {
        public Guid Id { get; set; }

        [Required]
        public string Species { get; set; }

        // Updated Location
        [Required]
        public string LocationName { get; set; } // Display-friendly name
        public double Latitude { get; set; }
        public double Longitude { get; set; }

        [Required]
        public string PublisherName { get; set; }
        [Required]
        public string PublisherId { get; set; }

        public DateTime DateCaught { get; set; }
        public int LikesCount { get; set; }

        // Support for multiple photos
        public List<string> PhotoURLs { get; set; } = new List<string>();
    }


}
