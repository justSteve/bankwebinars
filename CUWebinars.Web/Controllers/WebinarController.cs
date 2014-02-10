using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using CUWebinars.Business.Repository;
using CUWebinars.Business.Services;
using CUWebinars.Web.Data.Repositories.Interfaces;
using CUWebinars.Web.Core.Browsers.Webinars;
using CUWebinars.Web.Services;
using CUWebinars.Web.ViewModel;
using Ninject.Extensions.Logging;
using CUWebinars.Business.Models;


namespace CUWebinars.Web.Controllers
{
    public class WebinarController : Controller
    {
        private TTSWebinarsContext db = new TTSWebinarsContext();
        private IMailService _mail;
        private readonly IWebinarRepository _webinarRepository;
        private readonly IOrderManagementService _orderManagementService;
        public ILogger Logger { get; set; }

        public WebinarController(IMailService mail, IWebinarRepository webinarRepository, ILogger logger, IOrderManagementService orderManagementService)
        {
            _mail = mail;
            _webinarRepository = webinarRepository;
            Logger = logger;
            _orderManagementService = orderManagementService;
        }
        //
        // GET: /Webinar/

        public ActionResult Index()
        {
            var webinars = _webinarRepository.GetAllActive();
            ViewBag.TopicCaption = " ";
            ViewBag.Title = "All Listed Events for CUWebinars";

            webinars = _webinarRepository.GetUpcoming().OrderBy(w => w.Date);
            ViewBag.Title = "All Upcoming Events for CUWebinars";

            return View(webinars);
        }

        public ActionResult ListByTopic(int ID)
        {
            // var webinars = WebinarFacade.Instance.SelectAllActiveWebinarsByTopic(ID);

            Session["TopicID"] = ID;
            ViewBag.SearchTerm = "TopicID=" + Session["TopicID"].ToString();
            ViewBag.TopicCaption = "";

            switch (Session["TopicID"].ToString())
            {
                case "16":
                    ViewBag.TopicCaption = "IRA ";
                    ViewBag.Title = "CUWebinars related to IRAs ";
                    break;
                case "15":
                    ViewBag.TopicCaption = "Compliance ";
                    ViewBag.Title = "CUWebinars related to Compliance ";
                    break;
                case "17":
                    ViewBag.TopicCaption = "Customer Service ";
                    ViewBag.Title = "CUWebinars related to Customer Service ";
                    break;
                case "18":
                    ViewBag.TopicCaption = "Security ";
                    ViewBag.Title = "CUWebinars related to Security ";
                    break;
                case "19":
                    ViewBag.TopicCaption = "Operations ";
                    ViewBag.Title = "CUWebinars related to Operations ";
                    break;
                case "20":
                    ViewBag.TopicCaption = "Auditing ";
                    ViewBag.Title = "CUWebinars related to Auditing ";
                    break;
                case "21":
                    ViewBag.TopicCaption = "Sales ";
                    ViewBag.Title = "CUWebinars related to Sales ";
                    break;
                case "22":
                    ViewBag.TopicCaption = "Lending ";
                    ViewBag.Title = "CUWebinars related to Lending ";
                    break;
                case "23":
                    ViewBag.TopicCaption = "Human Resources ";
                    ViewBag.Title = "CUWebinars related to Human Resources ";
                    break;
                case "25":
                    ViewBag.TopicCaption = "Computer Skills ";
                    ViewBag.Title = "CUWebinars related to Computer Skills ";
                    break;
                case "26":
                    ViewBag.TopicCaption = "Risk Management ";
                    ViewBag.Title = "CUWebinars related to Risk Management ";
                    break;
                case "27":
                    ViewBag.TopicCaption = "Teller ";
                    ViewBag.Title = "CUWebinars related to Teller ";
                    break;
            }

            var webinars = _webinarRepository.GetByTopic(ID);
            //var dtos = new WebinarDTOAssembler().Entities2DTOs(webinars);
            //return View(dtos);
            return View(webinars);
        }

        public ActionResult AllActive(string eventsToShow)
        {
            var webinars = _webinarRepository.GetAllActive();
            ViewBag.TopicCaption = " ";
            ViewBag.Title = "All Listed Events for CUWebinars";
            if (eventsToShow == "upcoming")
            {
                webinars = _webinarRepository.GetUpcoming().OrderBy(w => w.Date);
                ViewBag.Title = "All Upcoming Events for CUWebinars";
            }
            if (eventsToShow == "recorded")
            {
                webinars = _webinarRepository.GetRecorded().OrderBy(w => w.Date);
                ViewBag.Title = "All Recorded Events for CUWebinars";
            }

            return View(webinars);

        }

        public ActionResult SearchWebinars()
        {
            if (Request["searchFor"] != null)
                Session["searchTerm"] = Request["searchFor"].ToString();

            return View("~/Views/Webinar/SearchWebinars.cshtml");
        }

        public ActionResult SearchByTopic(
            [Core.DataTables.WebinarsBrowserRequestModelBinder] WebinarsBrowserRequestModel webinarsBrowserRequest)
        {

            var webinars = _webinarRepository.GetByTopic(Convert.ToInt32(webinarsBrowserRequest.Search.Replace("TopicID=", "")));
            var wList = new List<WebinarsBrowserSearchResultEntryDTO>();
            foreach (var webinar in webinars)
            {
                var item = new WebinarsBrowserSearchResultEntryDTO();
                item.Webinar = webinar;
                item.RegistrationsCount = 0;
                wList.Add(item);
            }

            WebinarsBrowserSearchResultDTO searchResult = new WebinarsBrowserSearchResultDTO();
            searchResult.Entries = wList;
            searchResult.FoundCount = 2;
            searchResult.TotalCount = webinars.Count();

            var data = new WebinarsSearchDTOAssembler(webinarsBrowserRequest.EchoId).Entity2DTO(searchResult);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Details(int id)
        {
            ViewBag.PageStyleType = "holy-grail-three-columns";

            var model = new WebinarDetailsViewModel()
            {
                Webinar = db.Webinars.Find(id)
                ,
                Options = _orderManagementService.GetOptionsByWebinarId(id)
                ,
                ConnectionInfo = ""
            };

            if (model.Webinar == null)
            {
                return HttpNotFound();
            }
            return View(model);
        }

        //
        // GET: /Webinar/Create

        public ActionResult Create()
        {
            ViewBag.PresenterId = new SelectList(db.Presenters, "Id", "Biography");
            return View();
        }

        //
        // POST: /Webinar/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Webinar webinar)
        {
            if (ModelState.IsValid)
            {
                db.Webinars.Add(webinar);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.PresenterId = new SelectList(db.Presenters, "Id", "Biography", webinar.idPresenter);
            return View(webinar);
        }

        //
        // GET: /Webinar/Edit/5

        public ActionResult Edit(int id = 0)
        {
            Webinar webinar = db.Webinars.Find(id);
            if (webinar == null)
            {
                return HttpNotFound();
            }
            ViewBag.PresenterId = new SelectList(db.Presenters, "Id", "Biography", webinar.idPresenter);
            return View(webinar);
        }

        //
        // POST: /Webinar/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Webinar webinar)
        {
            if (ModelState.IsValid)
            {
                db.Entry(webinar).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.PresenterId = new SelectList(db.Presenters, "Id", "Biography", webinar.idPresenter);
            return View(webinar);
        }

        //
        // GET: /Webinar/Delete/5

        public ActionResult Delete(int id = 0)
        {
            Webinar webinar = db.Webinars.Find(id);
            if (webinar == null)
            {
                return HttpNotFound();
            }
            return View(webinar);
        }

        //
        // POST: /Webinar/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Webinar webinar = db.Webinars.Find(id);
            db.Webinars.Remove(webinar);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
