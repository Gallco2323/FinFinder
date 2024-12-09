using Microsoft.AspNetCore.Mvc;

namespace FinFinder.Web.Controllers
{
    public class ErrorController : Controller
    {
        [Route("Error/404")]
        public IActionResult NotFoundPage()
        {
            Response.StatusCode = 404; // Set response code
            return View("NotFound"); // Use the "NotFound.cshtml" view
        }

        [Route("Error/500")]
        public IActionResult InternalServerError()
        {
            Response.StatusCode = 500; // Set response code
            return View("ServerError"); // Use the "ServerError.cshtml" view
        }
    }
}
