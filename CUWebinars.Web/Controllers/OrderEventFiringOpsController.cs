using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls.WebParts;
using CUWebinars.Business.Services;
using CUWebinars.Web.ViewModel;
using Ninject.Extensions.Logging;
using System;
using System.Web.Mvc;

namespace CUWebinars.Web.Controllers
{
    public class OrderEventFiringOpsController : Controller
    {
        private readonly IOrderManagementService _orderManagementService;
        private readonly ILogger _logger;

        public OrderEventFiringOpsController(IOrderManagementService orderManagementService, ILogger logger)
        {
            _orderManagementService = orderManagementService;
            _logger = logger;
        }

        public ViewResult Index()
        {
            

            return View();
        }

        public PartialViewResult SendReminder()
        {
            var model = new AdhocNotificationViewModel
            {
                Webinars = GetUpcomingWebinarsAsSelectListItems()
            };

            return PartialView("_SendReminder", model);
        }
        
        [HttpPost]
        public JsonResult SendReminder(int webinarId)
        {
            var orders = _orderManagementService.GetOrdersForLiveNotifications(webinarId);

            _orderManagementService.FireSendReminderNotificationEvent(orders);

            return Json(new { Result = "Success" });
        }

        public PartialViewResult SendConnectionInfo()
        {
            var model = new AdhocNotificationViewModel
            {
                Webinars = GetUpcomingWebinarsAsSelectListItems()
            };

            return PartialView("_SendConnectionInfo", model);
        }

        [HttpPost]
        public JsonResult SendConnectionInfo(int webinarId)
        {
            var orders = _orderManagementService.GetOrdersForLiveNotifications(webinarId);

            _orderManagementService.FireSendConnectionInfoNotificationEvent(orders);

            return Json(new { Result = "Success" });
        }

        [HttpPost]
        public JsonResult FireSendConnectionInfo(IList<string> webUsers)
        {

            return null;



        }
        
        [HttpPost]
        public JsonResult FireSendOrderShippedEvent(int orderId)
        {
            try
            {
               // _orderManagementService.FireSendOrderShippedNotificationEvent(orderId);

                return Json(new {Result = "Success"});
            }
            catch (Exception exception)
            {
                _logger.ErrorException(exception.Message, exception);
                return Json(new { Result = "Fail" });
            }
        }

        private IEnumerable<SelectListItem> GetUpcomingWebinarsAsSelectListItems()
        {
            return _orderManagementService.GetUpcomingWebinars().Select(w => new SelectListItem { Text = w.Title, Value = w.idWebinar.ToString() });
        }

    }
}