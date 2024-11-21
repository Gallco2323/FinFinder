using FinFinder.Web.ViewModels.FishCatch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinFinder.Web.ViewModels.Home
{
    public class HomePageViewModel
    {
        public List<FishCatchIndexViewModel> FeaturedFishCatches { get; set; } = new List<FishCatchIndexViewModel>();
        public int TotalFishCatches { get; set; }
        public int TotalUsers { get; set; }
        public string MostPopularTechnique { get; set; } = string.Empty;
        public List<ActivityViewModel> RecentActivities { get; set; } = new List<ActivityViewModel>();
        public List<FishCatchIndexViewModel> UserRecentCatches { get; set; } = new List<FishCatchIndexViewModel>();
    }
}
