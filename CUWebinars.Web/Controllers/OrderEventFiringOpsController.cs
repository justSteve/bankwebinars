using CUWebinars.Business.Services;
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
        
        [HttpPost]
        public JsonResult FireSendOrderShippedEvent(int orderId)
        {
            try
            {
                _orderManagementService.FireSendOrderShippedNotificationEvent(orderId);

                return Json(new {Result = "Success"});
            }
            catch (Exception exception)
            {
                _logger.ErrorException(exception.Message, exception);
                return Json(new { Result = "Fail" });
            }
        }
    }
}