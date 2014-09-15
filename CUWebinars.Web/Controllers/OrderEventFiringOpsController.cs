using System;
using CUWebinars.Business.Models;
using CUWebinars.Business.Services;
using CUWebinars.Web.Helpers;
using CUWebinars.Web.ViewModel;
using Newtonsoft.Json.Linq;
using Ninject.Extensions.Logging;
using System.Linq;
using System.Web.Mvc;

namespace CUWebinars.Web.Controllers
{
    public class OrderEventFiringOpsController : Controller
    {
        private readonly IOrderManagementService _orderManagementService;
        private readonly IWebinarManagementService _webinarManagementService;
        private readonly ILogger _logger;
        private bool _disposed;

        public OrderEventFiringOpsController(IOrderManagementService orderManagementService, IWebinarManagementService webinarManagementService, ILogger logger)
        {
            _orderManagementService = orderManagementService;
            _webinarManagementService = webinarManagementService;
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
                Webinars = EventInvokerHelpers.GetUpcomingWebinarsAsSelectListItems(_webinarManagementService)
            };

            return PartialView("_adHocNotification", model);
        }

        [HttpPost]
        public ActionResult SendAdhocEvent(int webinarId)
        {
            var regTypes = EventInvokerHelpers.GetRegTypesForWebinarAsSelectListItems(webinarId, _webinarManagementService);

            return Json(regTypes);
        }

        public PartialViewResult SendReminder()
        {
            var model = new AdhocNotificationViewModel
            {
                Webinars = EventInvokerHelpers.GetUpcomingWebinarsAsSelectListItems(_webinarManagementService)
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
                Webinars = EventInvokerHelpers.GetUpcomingWebinarsAsSelectListItems(_webinarManagementService)
            };

            return PartialView("_SendConnectionInfo", model);
        }

        [HttpPost]
        public JsonResult SendConnectionInfo(int webinarId)
        {
            var orders = _orderManagementService.GetOrdersForLiveNotifications(webinarId);

            foreach (var order in orders)
            {
                AdditionalLocation nuller = new AdditionalLocation();
                GenerateRegistrantKey(order, nuller);

                if (order.OrderRows.Single(or => or.RowStatus == OrderRowStatus.Active).AdditionalLocation.Count > 0)
                {
                    foreach (var additionalLocation in order.OrderRows.Single(or => or.RowStatus == OrderRowStatus.Active).AdditionalLocation)
                    {
                        GenerateRegistrantKey(order, additionalLocation);
                    }
                }
            }
            _orderManagementService.FireSendConnectionInfoNotificationEvent(orders);

            return Json(new { Result = WebUiConstants.Success });
        }

        private void GenerateRegistrantKey(Order order, AdditionalLocation additionalLocation)
        {
            var row = order.OrderRows.Single(or => or.RowStatus == OrderRowStatus.Active);
            var regKeyResponse = "";
            if (additionalLocation.Email == null)
            {
                regKeyResponse = _orderManagementService.CreateRegistrantKey(order.FirstName, order.LastName
                    , order.BillingEmail, row.Webinar.idWebinar, row.Webinar.WebinarKey);
            }
            else
            {
                var contents = additionalLocation.FullName.Split(' ').ToString();
                var lastName = contents.Skip(1).ToString();

                regKeyResponse = _orderManagementService.CreateRegistrantKey(additionalLocation.FullName.Split(' ')[0], lastName
                   , additionalLocation.Email, row.Webinar.idWebinar, row.Webinar.WebinarKey);
            }

            if (ReferenceEquals(null, regKeyResponse))
                throw new NullReferenceException("The Registration Key Response from the Citrix API resulted in a null response.");

            JObject parsedJsonObject = JObject.Parse(regKeyResponse);

            if (parsedJsonObject[WebUiConstants.RegistrantKey] != null)
            {
                var registrantKey = parsedJsonObject[WebUiConstants.RegistrantKey].ToString();
                var joinUrl = parsedJsonObject[WebUiConstants.JoinUrl].ToString();
                if (additionalLocation.Email == null)
                {
                    row.RegistrantKey = registrantKey;
                    row.JoinURL = joinUrl;
                }
                else
                {
                    additionalLocation.JoinURL = joinUrl;
                    additionalLocation.RegistrantKey= registrantKey;   
                }
                var ResultOfUpdate = _orderManagementService.UpdateOrderChanges(order);
            }
        }

        public PartialViewResult SendRecordingPosted()
        {
            var model = new AdhocNotificationViewModel
            {
                Webinars = EventInvokerHelpers.GetRecordedWebinarsAsSelectListItems(_webinarManagementService)
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

                return Json(new { Result = WebUiConstants.Success });
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

            return Json(new { Result = WebUiConstants.Success });
        }

        protected override void Dispose(bool disposing)
        {
            if (!_disposed && disposing)
            {
                var logger = _logger as IDisposable;
                if (logger != null)
                    logger.Dispose();

                _orderManagementService.Dispose();

                base.Dispose(true);
            }
            _disposed = true;
        }
    }
}