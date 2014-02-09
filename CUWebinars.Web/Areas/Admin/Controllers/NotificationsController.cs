using System.Web.Mvc;
using CUWebinars.Business.Repository;
using CUWebinars.Web.Data.Repositories.Interfaces;
using CUWebinars.Web.Services;
using CUWebinars.Web.ViewModel;
using Ninject.Extensions.Logging;
using CUWebinars.Business.Models;

namespace CUWebinars.Web.Areas.Admin.Controllers
{
    public class NotificationsController : Controller
    {
        //
        // GET: /Admin/Notifications/
        private TTSWebinarsContext db = new TTSWebinarsContext();
        private IMailService _mail;
        private readonly IWebinarRepository webinarRepository;
        public ILogger Logger { get; set; }

        public NotificationsController(IMailService mail, IWebinarRepository webinarRepository, ILogger logger)
        {
            _mail = mail;
            this.webinarRepository = webinarRepository;
            Logger = logger;
        }
        public ActionResult Index()
        {

            var connectionInfo = new ConnectionInfoViewModel();

            //connectionInfo.Orders.Add();

            //new MailController(_mail, webinarRepository, Logger).ConnectionInfoEmail(connectionInfo).Deliver();
            return View();
        }

    }
}
