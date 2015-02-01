using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
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
            ControllerContext.HttpContext.Response.StatusCode = 404;

            return View();
        }

        public ViewResult SessionNotAvailable()
        {
            return View();
        }

    }
}