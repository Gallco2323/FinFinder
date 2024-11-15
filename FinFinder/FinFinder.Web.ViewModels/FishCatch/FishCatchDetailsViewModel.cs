using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinFinder.Web.ViewModels.FishCatch
{
    public class FishCatchDetailsViewModel
    {
        public Guid Id { get; set; }

        public required string Species { get; set; }
        public required string Description { get; set; }
        public double Weight { get; set; }
        public double Length { get; set; }
        
        public required string Location { get; set; }
        public required string FishingTechniqueName { get; set; }
        public string? PhotoURL { get; set; }
        public DateTime DateCaught { get; set; }

        public required string PublisherName { get; set; }
        
        public string? PublisherProfilePictureURL { get; set; }
    }

}
