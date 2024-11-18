using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinFinder.Web.ViewModels.FishCatch
{
    public class FishCatchFavoriteViewModel
    {
        public Guid FishCatchId { get; set; }
        public required string Species { get; set; }
        public required string Location { get; set; }
        public DateTime DateCaught { get; set; }
        public string? PhotoURL { get; set; }
        public required string PublisherName { get; set; }
    }
}
