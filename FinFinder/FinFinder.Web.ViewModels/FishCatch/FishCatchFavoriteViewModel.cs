using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinFinder.Web.ViewModels.FishCatch
{
    public class FishCatchFavoriteViewModel
    {
        public Guid FishCatchId { get; set; }

        [Required]
        public string Species { get; set; }

        // Updated Location
        [Required]
        public string LocationName { get; set; }

        public DateTime DateCaught { get; set; }

        // Updated for multiple photos
        public List<string> PhotoURLs { get; set; } = new List<string>();

        [Required]
        public string PublisherName { get; set; }
    }

}
