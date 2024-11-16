using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinFinder.Web.ViewModels.FishCatch
{
    public class FishCatchIndexViewModel
    {
        public Guid Id { get; set; }
        public required string Species { get; set; }
        public required string Location { get; set; }
        public DateTime DateCaught { get; set; }
        public required string PhotoURL { get; set; }

        // Publisher details
        public required string PublisherName { get; set; }
        public required string PublisherId { get; set; }
    }

}
