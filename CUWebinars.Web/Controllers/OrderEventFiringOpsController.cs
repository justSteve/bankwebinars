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

        public PartialViewResult ResendOrderConfirmation()
        {
            var model = new ResendOrderInformationViewModel
            {
                OrderId = string.Empty
            };

            return PartialView("_resendOrderConfirmation", model);
        }

        [HttpPost]
        public ActionResult ResendOrderConfirmation(int orderId)
        {
            var order = _orderManagementService.GetOrderById(orderId);

            if (ReferenceEquals(null, order))
            {
                return Json(new { Result = WebUiConstants.Fail });
            }

            _orderManagementService.FireOrderSubmittedEvent(order);
            return Json(new { Result = WebUiConstants.Success });
        }

        public PartialViewResult ResendConnectionInfo()
        {
            var model = new ResendOrderInformationViewModel
            {
                OrderId = string.Empty
            };

            return PartialView("_resendConnectionInfo", model);
        }

        [HttpPost]
        public ActionResult ResendConnectionInfo(int orderId)
        {
            var order = _orderManagementService.GetOrderById(orderId);

            if (ReferenceEquals(null, order))
            {
                return Json(new { Result = WebUiConstants.Fail });
            }

            _orderManagementService.FireSendConnectionInfoNotificationEvent(new Order[] { order });
            return Json(new { Result = WebUiConstants.Success });
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
            
            if (orders.Any())
            {
                _orderManagementService.FireSendReminderNotificationEvent(orders);


                return Json(new { Result = WebUiConstants.Success });
            }

            return Json(new { Result = WebUiConstants.NoOrdersForWebinar });
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