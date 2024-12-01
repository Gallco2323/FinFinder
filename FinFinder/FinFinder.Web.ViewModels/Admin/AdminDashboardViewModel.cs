using FinFinder.Web.ViewModels.Home;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinFinder.Web.ViewModels.Admin
{
    public class AdminDashboardViewModel
    {
        public int TotalUsers { get; set; }
        public int TotalFishCatches { get; set; }
        public int TotalComments { get; set; }

        public List<ActivityViewModel> RecentActivities { get; set; } = new List<ActivityViewModel>();
    }

   
}
