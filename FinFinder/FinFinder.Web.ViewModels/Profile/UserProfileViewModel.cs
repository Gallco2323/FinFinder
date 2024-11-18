using FinFinder.Web.ViewModels.FishCatch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinFinder.Web.ViewModels.Profile
{
    public class UserProfileViewModel
    {
        public required string UserName { get; set; }
        public string? ProfilePictureURL { get; set; }
        public string? Bio { get; set; }
        public int FishCount { get; set; }

        // List of Fish Catches by the User
        public List<FishCatchIndexViewModel> FishCatches { get; set; } = new List<FishCatchIndexViewModel>();
    }
}
