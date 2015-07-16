using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CUWebinars.Business.Models;
using CUWebinars.Business.Repository;
using CUWebinars.Web.ViewModel;

namespace CUWebinars.Web.Controllers
{
    public class AdhocNotificationController : Controller
    {
        private readonly IRegTypeRepository _regTypeRepository;
        private readonly IWebinarRepository _webinarRepository;
        private readonly IWebUserRepository _websUserRepository;
        private bool _disposed;

        public AdhocNotificationController(
            IRegTypeRepository regTypeRepository, 
            IWebinarRepository webinarRepository,
            IWebUserRepository websUserRepository
            )
        {
            _regTypeRepository = regTypeRepository;
            _webinarRepository = webinarRepository;
            _websUserRepository = websUserRepository;
        }

        public ViewResult Index()
        {
            var webinars = GetUpcomingWebinarsAsSelectListItems();

            var viewmodel = new AdhocNotificationViewModel
            {
                NotificationBody = string.Empty,
                Webinars = webinars
            };

            return View(viewmodel);
        }

        [HttpPost]
        public ActionResult Index(int webinarId)
        {
            var regTypes = GetRegTypesForWebinarAsSelectListItems(webinarId);
            
            return Json(regTypes);
        }

        [HttpPost]
        public ActionResult WebUsersOfWebinars(int webinarId, IList<int> regTypeIds)
        {
            var webUsers = GetWebUsersForUpcomingOrders(webinarId, regTypeIds);

            return Json(webUsers.Select(wu => wu.email));
        }

        private IEnumerable<SelectListItem> GetRegTypesForWebinarAsSelectListItems(int idWebinar)
        {
            return
                _regTypeRepository.FindRegTypesByWebinarId(idWebinar, false)
                    .Select(r => new SelectListItem {Text = r.Key.OptionLabel, Value = r.Key.idRegType.ToString()});
        }
        private IEnumerable<SelectListItem> GetUpcomingWebinarsAsSelectListItems()
        {
            return _webinarRepository.GetUpcoming().Select(w => new SelectListItem{ Text = w.Title, Value = w.idWebinar.ToString()});
        }

        private IEnumerable<WebUser> GetWebUsersForUpcomingOrders(int webinarId, IList<int> regTypeIds)
        {
            return _websUserRepository.GetWebusersForWebinarWithRegtypes(webinarId, regTypeIds);
        }

        protected override void Dispose(bool disposing)
        {
            if (!_disposed && disposing)
            {
                _regTypeRepository.Dispose();
                _webinarRepository.Dispose();
                _websUserRepository.Dispose();

                base.Dispose(true);
            }
            _disposed = true;
        }
    }
}