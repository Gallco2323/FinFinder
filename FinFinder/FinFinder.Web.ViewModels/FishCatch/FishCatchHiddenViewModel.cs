using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinFinder.Web.ViewModels.FishCatch
{
    public class FishCatchHiddenViewModel
    {
        public Guid Id { get; set; }
        public string Species { get; set; }
        public string LocationName { get; set; }
        public DateTime DateCaught { get; set; }
        public List<string> PhotoURLs { get; set; } = new List<string>();
    }

}
