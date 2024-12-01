using Microsoft.AspNetCore.Mvc;

namespace FinFinder.Web.Areas.Admin.Controllers
{
    public class AdminFishCatchController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
