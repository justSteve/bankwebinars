using CUWebinars.Business.Models;
using CUWebinars.Business.Services;
using CUWebinars.Web.Helpers;
using CUWebinars.Web.ViewModel;
using Ninject.Extensions.Logging;
using System.Linq;
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
                Webinars = EventInvokerHelpers.GetUpcomingWebinarsAsSelectListItems(_orderManagementService)
            };

            return PartialView("_adHocNotification", model);
        }

        [HttpPost]
        public ActionResult SendAdhocEvent(int webinarId)
        {
            var regTypes = EventInvokerHelpers.GetRegTypesForWebinarAsSelectListItems(webinarId, _orderManagementService);

            return Json(regTypes);
        }

        public PartialViewResult SendReminder()
        {
            var model = new AdhocNotificationViewModel
            {
                Webinars = EventInvokerHelpers.GetUpcomingWebinarsAsSelectListItems(_orderManagementService)
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
                Webinars = EventInvokerHelpers.GetUpcomingWebinarsAsSelectListItems(_orderManagementService)
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
                Webinars = EventInvokerHelpers.GetRecordedWebinarsAsSelectListItems(_orderManagementService)
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
            var ordersShipped = EventInvokerHelpers.GetShippedWebinarsAsSelectListItems(_orderManagementService);

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
        
    }
}