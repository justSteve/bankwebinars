using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CUWebinars.Web.Controllers.Admin
{
    namespace TTSTrain.Webinars.WebEntry.Controllers.Admin
    {
        [ElmahHandleError]
        //[Authorize(Roles = AppRoles.Admin)]
        public class AdminHomeController : Controller
        {
            //
            // GET: /Admin/
            public ActionResult Index()
            {
                return View("~/Views/Admin/Home/Index.cshtml");
            }
        }
    }

}