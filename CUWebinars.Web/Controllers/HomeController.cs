using System;
using CUWebinars.Business.Repository;
using CUWebinars.Web.Infrastructure.Attributes;
using Ninject.Extensions.Logging;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Web.Mvc;
using System.Xml;

namespace CUWebinars.Web.Controllers
{
    //[MonitorAffiliate]
    public class HomeController : Controller
    {

        private readonly IWebinarRepository _webinarRepository;
        private readonly ILogger _logger;
        private bool _disposed;

        public HomeController(IWebinarRepository webinarRepository, ILogger logger)
        {
            _webinarRepository = webinarRepository;
            _logger = logger;
        }


        public ActionResult RssFeedOfAddedEvents()
        {
            //http://office.microsoft.com/en-us/office365-sharepoint-online-small-business-help/basic-tasks-in-sharepoint-online-for-office-365-for-professionals-and-small-businesses-HA101988906.aspx#_Toc272147708

            string strFeed =
                "https://totaltrainingsolutions-public.sharepoint.com/_layouts/15/listfeed.aspx?List={09364DB1-2255-406E-9CB6-45FE64C0D341}";

            using (XmlReader reader = XmlReader.Create(strFeed))
            {
                SyndicationFeed rssData = SyndicationFeed.Load(reader);

                return PartialView(rssData);
            }
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
            return RedirectToAction("MyWebinars", "Account");
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

        [HttpPost]
        public ActionResult ConfirmOrder(FormCollection formCollection)
        {
            return View();
        }

    public ActionResult ContactUs()
        {
            ViewBag.PageStyleType = "two-columns-right-sidebar";
            return View();
        }

        public ActionResult Index()
        {
            ViewBag.PageStyleType = "index-flex-dark";
            var lWebinars = _webinarRepository.GetUpcoming().OrderByDescending(w => w.Date).Take(15).ToList();
            return View(lWebinars);
        }

        public ActionResult About()
        {
            ViewBag.PageStyleType = "two-columns-right-sidebar";
            return View();
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
