using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls.WebParts;
using CUWebinars.Business.Models;
using CUWebinars.Business.Services;
using CUWebinars.Web.Helpers;
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

        public PartialViewResult SendAdhocEvent()
        {
            var model = new AdhocNotificationViewModel
            {
                Webinars = GetUpcomingWebinarsAsSelectListItems()
            };

            return PartialView("_adHocNotification", model);
        }

        [HttpPost]
        public ActionResult SendAdhocEvent(int webinarId)
        {
            var regTypes = GetRegTypesForWebinarAsSelectListItems(webinarId);

            return Json(regTypes);
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

            return Json(new { Result = WebUiConstants.Success });
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

            return Json(new { Result = WebUiConstants.Success });
        }

        public PartialViewResult SendRecordingPosted()
        {
            var model = new AdhocNotificationViewModel
            {
                Webinars = GetRecordedWebinarsAsSelectListItems()
            };

            return PartialView("_SendRecordingPosted", model);
        }
        
        [HttpPost]
        public JsonResult SendRecordingPosted(int webinarId)
        {
            var orders = _orderManagementService.GetOrdersForRecordedNotifications(webinarId);

            if (orders.Any())
            {
                _orderManagementService.FireSendRecordingIsPostedEvent(orders);

                return Json(new {Result = WebUiConstants.Success});
            }

            return Json(new { Result = WebUiConstants.NoOrdersForWebinar });
        }

       
        public PartialViewResult SendShippedOrder()
        {
            var ordersShipped = GetShippedWebinarsAsSelectListItems();

            var model = new AdhocNotificationViewModel
            {
                OrdersList = ordersShipped
            };

            return PartialView("_SendShippedOrder", model);
        }

        [HttpPost]
        public JsonResult SendShippedOrder(int orderId)
        {
            var order = _orderManagementService.GetOrderById(orderId);
            
            _orderManagementService.FireSendOrderShippedNotificationEvent(new Order[] { order });

            return Json(new {Result = WebUiConstants.Success});
        }


        private IEnumerable<SelectListItem> GetRegTypesForWebinarAsSelectListItems(int idWebinar)
        {
            return _orderManagementService.FindRegTypesByWebinarId(idWebinar).Select(r => new SelectListItem { Text = r.OptionLabel, Value = r.idRegType.ToString() });
        }


        private IEnumerable<SelectListItem> GetUpcomingWebinarsAsSelectListItems()
        {
            return _orderManagementService.GetUpcomingWebinars()
                .OrderBy(w => w.Date)
                .Select(w => new SelectListItem { Text = w.Title, Value = w.idWebinar.ToString() });
        }

        private IEnumerable<SelectListItem> GetRecordedWebinarsAsSelectListItems()
        {
            return _orderManagementService.GetRecordedWebinars().Select(w => new SelectListItem { Text = w.Title, Value = w.idWebinar.ToString() });
        }

        private IEnumerable<SelectListItem> GetShippedWebinarsAsSelectListItems()
        {
            return _orderManagementService.GetOrdersForShippedNotification().Select(w => new SelectListItem { Text = w.idOrder.ToString(), Value = w.idOrder.ToString() });
        }

    }
}