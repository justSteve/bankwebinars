using CUWebinars.Business.Repository;
using CUWebinars.Web.Helpers;
using CUWebinars.Web.Infrastructure.Attributes;
using Ninject.Extensions.Logging;
using System;
using System.Linq;
using System.Web.Mvc;
using CUWebinars.Business.Core.Helpers;
using CUWebinars.Business.Services;
using CUWebinars.Web.Core;
using CUWebinars.Web.Models;
using CUWebinars.Web.Services;
using Newtonsoft.Json;

namespace CUWebinars.Web.Controllers
{
    //[MonitorAffiliate]
    public class HomeController : Controller
    {
        private GlobalConfig _globalConfig = GlobalConfig.GlobalConfigSingletonCreator.UniqueInstance;

        private readonly IWebinarRepository _webinarRepository;
        private IStateService _stateService;

        private readonly IAffiliateManagementService _affiliateManagementService;

        private readonly ILogger _logger;
        private bool _disposed;

        public HomeController(IWebinarRepository webinarRepository, ILogger logger, IAffiliateManagementService affiliateManagementService, IStateService stateService)
        {
            _webinarRepository = webinarRepository;
            _logger = logger;
            _affiliateManagementService = affiliateManagementService;
            _stateService = stateService;
        }

        public ActionResult TestStaticPage()
        {
            //            ViewBag.PageStyleType = "two-columns-right-sidebar";
            return View();
        }

        public ActionResult PrivacyStatement()
        {
            ViewBag.PageStyleType = "two-columns-right-sidebar";
            return View();
        }

        public ActionResult DetailedConnectionInstructions()
        {
            ViewBag.PageStyleType = "two-columns-right-sidebar";
            return View();
        }

        public ActionResult MyWebinars()
        {
            return RedirectToAction("MyWebinars", "Cart");
        }

        public ActionResult WhatIsAWebinar()
        {
            ViewBag.PageStyleType = "two-columns-right-sidebar";
            return View();
        }

        public ActionResult CommonQuestions()
        {
            ViewBag.PageStyleType = "two-columns-right-sidebar";
            return View();
        }

        public ActionResult DSSignUp(int id, FormCollection frm)
        {
            Business.Models.Affiliate affiliate;
            if (id > 1)
            {
                affiliate = _affiliateManagementService.FindById(id);
            }
            else
            {
                affiliate = _affiliateManagementService.FindById(19);
            }
            string msgText = "";
            int i = 0;
            foreach (var x in frm)
            {
                msgText += "" + x.ToString() + ": " + frm[i] + "<br>"; //Request.Form[x] + ;

                i++;
            }
            _logger.Info(msgText);
            //NotificationFacade.Instance.SendMessage(ConfigurationManager.AppSettings["TestEmailAddress"], "Director Series Registration", "Registrations@BankWebinars.com", msgText);

            ViewData["isConfirmed"] = "true";
            return View("~/Views/Home/DSSignUp.cshtml", affiliate);

        }
        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult DESModules(int? id)
        {
            var lWebinars = _webinarRepository.GetUpcoming().OrderByDescending(w => w.Date).Take(15).ToList();
            return View(lWebinars);

        }

        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult DSSignUp(int? id)
        {

            ViewBag.PageStyleType = "index-flex-dark";

            _stateService.SetValue(WebUiConstants.DesSession, "true");

            if (id.HasValue)
            {
                if (id == 17146)
                {

                    ViewBag.showPrice = "$1,395 for a 12-month subscription.<br>$1,895 for non-members";
                }

            }

            Business.Models.Affiliate affiliate;

            if (id.HasValue)
            {
                affiliate = _affiliateManagementService.FindById(id.Value);
            }
            else
            {

                affiliate = _affiliateManagementService.FindById(19);
            }

            return View(affiliate);
        }


        //[HttpPost]
        //public ActionResult ConfirmOrder(FormCollection formCollection)
        //{
        //    return View();
        //}

        public ActionResult ContactUs()
        {
            ViewBag.PageStyleType = "two-columns-right-sidebar";
            return View();
        }

        public ActionResult LearnMore()
        {
            ViewBag.PageStyleType = "two-columns-right-sidebar";
            return View();
        }

        public ActionResult Index()
        {
            if (_globalConfig.Tenant == "DirectorSeries")
            {
                _stateService.SetValue(WebUiConstants.DesSession, "true");


                var lWebinars = _webinarRepository.GetUpcoming().OrderByDescending(w => w.Date).Take(15).ToList();

                return View("~/Views/Home/IndexDES.cshtml", lWebinars);
            }
            else
            {
                ViewBag.PageStyleType = "index-flex-dark";
                if (Request.Url != null && Request.Url.PathAndQuery.ContainsIgnoreCase("email does not match"))
                {
                    ViewBag.Msg = ("Order's email does not match login email.");
                }

                var lWebinars = _webinarRepository.GetUpcoming().OrderByDescending(w => w.Date).Take(15).ToList();
                return View(lWebinars);
            }
        }

        public ActionResult About()
        {
            ViewBag.PageStyleType = "two-columns-right-sidebar";
            return View();
        }

        [HttpPost]
        //[ValidateAntiForgeryToken(Order = 0)]
        [HandleAjaxException(Order = 1)]
        public ActionResult SessionIsActive()
        {
            if (Request.IsAuthenticated)
            {
                return Json(new { Result = WebUiConstants.Success });
            }

            return Json(new { Result = WebUiConstants.Fail });
        }


        //[HttpPost]
        //public ActionResult Contact(ContactModel model)
        //{
        //    var msg = string.Format("Comment From: {1}{0}Email:{2}{0}Phone: {3}{0}Comment:{4}",
        //      Environment.NewLine,
        //      model.Name,
        //      model.Email,
        //      model.Phone,
        //      model.Comment);

        //    if (_mail.SendMail("noreply@yourdomain.com",
        //      "foo@yourdomain.com",
        //      "Website Contact",
        //      msg))
        //    {
        //        ViewBag.MailSent = true;
        //    }

        //    return View();
        //}



        protected override void Dispose(bool disposing)
        {
            if (!_disposed && disposing)
            {
                var logger = _logger as IDisposable;
                if (logger != null)
                    logger.Dispose();

                _webinarRepository.Dispose();

                base.Dispose(true);
            }
            _disposed = true;
        }
    }
}
