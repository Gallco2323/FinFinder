using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinFinder.Web.ViewModels.FishCatch
{
    public class FishCatchDeleteViewModel
    {
        public Guid Id { get; set; }
        public required string Species { get; set; }
        public required double Weight { get; set; }
        public required string FishingTechniqueName { get; set; }
    }

}
