using System.Linq;
using System.ServiceModel.Syndication;
using System.Web.Mvc;
using System.Xml;
using CUWebinars.Web.Data.Repositories.Interfaces;
using CUWebinars.Web.Services;
using Ninject.Extensions.Logging;

namespace CUWebinars.Web.Controllers
{
    public class HomeController : Controller
    {
        private IMailService _mail;
        private readonly IWebinarRepository _repos;
        public ILogger Logger { get; set; }

        public HomeController(IMailService mail, IWebinarRepository repos, ILogger logger)
        {
            _mail = mail;
            _repos = repos;
            Logger = logger;
        }

        public ActionResult RssFeedOfAddedEvents()
        {
            //http://office.microsoft.com/en-us/office365-sharepoint-online-small-business-help/basic-tasks-in-sharepoint-online-for-office-365-for-professionals-and-small-businesses-HA101988906.aspx#_Toc272147708

            string strFeed = "https://totaltrainingsolutions-public.sharepoint.com/_layouts/15/listfeed.aspx?List={09364DB1-2255-406E-9CB6-45FE64C0D341}";

            using (XmlReader reader = XmlReader.Create(strFeed))
            {
                SyndicationFeed rssData = SyndicationFeed.Load(reader);

                return PartialView(rssData);
            }
        }
        public ActionResult DetailedConnectionInstructions()
        {
            ViewBag.PageStyleType = "two-columns-right-sidebar";
            return View();
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

        public ActionResult ContactUs()
        {
            return View();
        }

        public ActionResult Index()
        {
            ViewBag.PageStyleType = "index-flex-dark";
            var lWebinars = _repos.GetUpcoming().OrderByDescending(w => w.Date).Take(15).ToList();
            return View(lWebinars);
        }

        public ActionResult About()
        {
            ViewBag.PageStyleType = "two-columns-right-sidebar";
            return View();
        }

        public ActionResult Contact()
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

    }
}
