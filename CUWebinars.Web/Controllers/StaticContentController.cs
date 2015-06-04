using System.Web.Mvc;

namespace CUWebinars.Web.Controllers
{
    public class StaticContentController : Controller
    {
        public ViewResult ServerError()
        {
            return View();
        }

        public ViewResult PageNotFound()
        {
            // Note: the HandleErrorInfo can be accessed via ControllerContext.Controller.ViewData.Model

            ControllerContext.HttpContext.Response.StatusCode = 404;

            return View();
        }

        public ViewResult SessionNotAvailable()
        {
            return View();
        }

        public ViewResult IncorrectPassword()
        {
            return View();
        }

    }
}