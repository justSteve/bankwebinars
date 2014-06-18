using CUWebinars.Web.Core.Orchestrators;
using CUWebinars.Web.Helpers;
using CUWebinars.Web.Models;
using Ninject.Extensions.Logging;
using System;
using System.Web.Mvc;

namespace CUWebinars.Web.Controllers
{
    public class OrderController : Controller
    {
        private readonly ILogger _logger;
        private readonly IOrderControllerOrchestrator _orderControllerOrchestrator;
        private bool _disposed;

        public OrderController(ILogger logger, IOrderControllerOrchestrator orderControllerOrchestrator)
        {
            _logger = logger;
            _orderControllerOrchestrator = orderControllerOrchestrator;
        }

        /// <summary>
        /// For us to use a query string, this has to be a GET request.
        /// </summary>
        /// <param name="incomingOrderModel"></param>
        /// <returns></returns>
        [HttpGet]
        public JsonResult CreateOrder(IncomingOrderModel incomingOrderModel)
        {
            int idOfLastOrder = default(int);
            string verificationKey = string.Empty;
            string confirmChangeEmailUrl = string.Empty;

            if (!ModelState.IsValid)
            {
                ProcessModelStateErrors();
                return Json(new { Result = WebUiConstants.Fail });
            }

            try
            {
                var email = incomingOrderModel.Email.Trim();

                _logger.Info("Begin import: " + email);

                var orderManagementQueryResult = _orderControllerOrchestrator.GetPreparatoryData(incomingOrderModel, email);

                if (ReferenceEquals(null, orderManagementQueryResult.WebUser))
                {
                    try
                    {
                        orderManagementQueryResult.WebUser =
                            _orderControllerOrchestrator.ProcessNewUser(incomingOrderModel, email);

                        verificationKey = _orderControllerOrchestrator.GetVerificationKeyForNewUserAccount();

                        confirmChangeEmailUrl =
                            _orderControllerOrchestrator.GetConfirmChangeEmailLinkForNewUserAccount();

                        _orderControllerOrchestrator.FinalizeNewRegistration(incomingOrderModel, verificationKey);
                    }
                    catch (Exception exception)
                    {
                        _logger.ErrorException(
                            string.Format("CreateOrder|CreateUser failed: {0}", exception.Message), exception);
                        throw;
                    }
                }

                idOfLastOrder = _orderControllerOrchestrator.CreateNewOrder(incomingOrderModel, email,
                    orderManagementQueryResult, verificationKey, confirmChangeEmailUrl);

                _logger.Info("Posted idOrder=" + idOfLastOrder);

                return Json(new { Result = idOfLastOrder.ToString() }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception exception)
            {
                var errString = string.Format("Order creation failed on {0} - {1} with msg: {2}", incomingOrderModel.Email, incomingOrderModel.idWebinar, exception.Message);
                _logger.ErrorException(errString, exception);
            }

            return Json(new { Result = "0" }, JsonRequestBehavior.AllowGet);
            //return Json(new { Result = WebUiConstants.Fail });
        }

        private void ProcessModelStateErrors()
        {
//Please use this general pattern when logging ModelState errors.
            var myErr = string.Empty;
            foreach (ModelState modelState in ViewData.ModelState.Values)
            {
                foreach (ModelError error in modelState.Errors)
                {
                    myErr += error.ErrorMessage + Environment.NewLine;
                }
            }
            //a better implementation:
            //http://stackoverflow.com/questions/2845852/asp-net-mvc-how-to-convert-modelstate-errors-to-json
            //var errorList = ModelState.ToDictionary(
            //    kvp => kvp.Key,
            //    kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray()
            //);

            _logger.Error(myErr);
        }

        protected override void Dispose(bool disposing)
        {
            if (!_disposed && disposing)
            {
                var logger = _logger as IDisposable;
                if (logger != null)
                    logger.Dispose();

                base.Dispose(true);
            }
            _disposed = true;
        }
    }
}