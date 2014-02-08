using CUWebinars.Business.Models;
using CUWebinars.Business.Repository;
using CUWebinars.Web.Services;
using Ninject.Extensions.Logging;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace CUWebinars.Web.Areas.Admin.Controllers
{
    public class WebinarsController : Controller
    {
        private TTSWebinarsContext db = new TTSWebinarsContext();
        private IMailService _mail;
        private readonly IWebinarRepository _webinarRepository;
        public ILogger Logger { get; set; }

        //
        // GET: /Admin/Webinars/
        public WebinarsController(IMailService mail, IWebinarRepository webinarRepository, ILogger logger)
        {
            _mail = mail;
            _webinarRepository = webinarRepository;
            Logger = logger;
        }


        ////[Authorize(Roles = AppRoles.Admin)]
        //[AcceptVerbs(HttpVerbs.Post)]
        //public ActionResult SendInfo(int ID, FormCollection formValues)
        //{
        //    IList<Order> orders = webinarRepository.GetOrdersByWebinarForConnectionInfo(ID);

        //    new MailController(_mail, webinarRepository, Logger).ConnectionInfoEmail(orders).Deliver();
        //    return RedirectToAction("HostProperties");
        //}


        public ActionResult Index()
        {
            var webinars = db.Webinars.Include(w => w.Presenter);
            return View(webinars.ToList());
        }

        //
        // GET: /Admin/Webinars/Details/5

        public ActionResult Details(int id)
        {
            Webinar webinar = db.Webinars.Find(id);
            if (webinar == null)
            {
                return HttpNotFound();
            }
            return View(webinar);
        }

        //
        // GET: /Admin/Webinars/Create

        public ActionResult Create()
        {
            ViewBag.PresenterId = new SelectList(db.Presenters, "Id", "Biography");
            return View();
        }

        //
        // POST: /Admin/Webinars/Create

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
        // GET: /Admin/Webinars/Edit/5

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
        // POST: /Admin/Webinars/Edit/5

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
        // GET: /Admin/Webinars/Delete/5

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
        // POST: /Admin/Webinars/Delete/5

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