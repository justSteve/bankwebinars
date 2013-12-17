using System.Web.Mvc;
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
        private readonly IWebinarRepository _repos;
        public ILogger Logger { get; set; }

        public NotificationsController(IMailService mail, IWebinarRepository repos, ILogger logger)
        {
            _mail = mail;
            _repos = repos;
            Logger = logger;
        }
        public ActionResult Index()
        {

            var connectionInfo = new ConnectionInfoViewModel();

            //connectionInfo.Orders.Add();

            //new MailController(_mail, _repos, Logger).ConnectionInfoEmail(connectionInfo).Deliver();
            return View();
        }

    }
}
