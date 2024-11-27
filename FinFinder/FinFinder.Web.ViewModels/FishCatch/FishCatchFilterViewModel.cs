using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinFinder.Web.ViewModels.FishCatch
{
    public class FishCatchFilterViewModel
    {
        public string SelectedFilter { get; set; } = "DatePosted"; // Default filter option
        public List<SelectListItem> Filters { get; set; } = new List<SelectListItem>
    {
        new SelectListItem { Value = "DatePosted", Text = "Date Posted", Selected = true },
        new SelectListItem { Value = "MostLiked", Text = "Most Liked" },
        new SelectListItem { Value = "LeastLiked", Text = "Least Liked" },
        new SelectListItem { Value = "Alphabetical", Text = "Alphabetical" }
    };

        public string? SearchTerm { get; set; }
        public List<FishCatchIndexViewModel> FishCatches { get; set; } = new List<FishCatchIndexViewModel>();
    }

}
