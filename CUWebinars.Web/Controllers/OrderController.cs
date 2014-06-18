using CUWebinars.Business.Constants;
using CUWebinars.Web.Core.Orchestrators;
using CUWebinars.Web.Helpers;
using CUWebinars.Web.Models;
using CUWebinars.Web.Services;
using Ninject.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Web.Mvc;

namespace CUWebinars.Web.Controllers
{
    public class OrderController : Controller
    {
        private readonly IStateService _stateService;
        private readonly ILogger _logger;
        private readonly IOrderControllerOrchestrator _orderControllerOrchestrator;
        private bool _disposed;

        public OrderController(IStateService stateService, ILogger logger, IOrderControllerOrchestrator orderControllerOrchestrator)
        {
            _stateService = stateService;
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
                return Json(new { Result = WebUiConstants.Fail });
            }

            try
            {
                var email = incomingOrderModel.Email.Trim();


                _logger.Info("Begin import: " + email);

                var orderManagementQueryResult = _orderControllerOrchestrator.GetData(incomingOrderModel, email);

                if (ReferenceEquals(null, orderManagementQueryResult.WebUser))
                {
                    try
                    {

                        //  Here, we set a value which indicates to the TtsSmtpMessageDelivery object that the user was created while importing an order.
                        //  This will be checked in TtsSmtpMessageDelivery and the notification will not be sent if this value is present.
                        //  The idea being that the Order Submitted notification will contain the info nomrally in the User Registered email.
                        _stateService.SetValue(DomainConstants.UserCreatedViaNewOrder, true);

                        orderManagementQueryResult.WebUser =
                            _orderControllerOrchestrator.ProcessNewUser(incomingOrderModel, email);

                        Debug.Assert(_stateService.HasValue(DomainConstants.VerificationKey),
                            "There's no reason session should not have a value for the VerificationKey at this point ");

                        verificationKey = _stateService.GetValue<string>(DomainConstants.VerificationKey);
                        confirmChangeEmailUrl = _stateService.GetValue<string>(DomainConstants.ConfirmChangeEmailLink);
                        _stateService.ClearValue(DomainConstants.VerificationKey);
                        _stateService.ClearValue(DomainConstants.ConfirmChangeEmailLink);

                        _orderControllerOrchestrator.FinalizeNewRegistration(incomingOrderModel, verificationKey);
                        //  Now we clear the value, so TtsSmtpMessageDelivery can go back to business as usual.
                        _stateService.ClearValue(DomainConstants.UserCreatedViaNewOrder);
                    }
                    catch (Exception exception)
                    {

                        _logger.ErrorException(string.Format("CreateOrder|CreateUser failed: {0}", exception.Message),
                            exception);
                    }
                }

                idOfLastOrder = _orderControllerOrchestrator.CreateNewOrder(incomingOrderModel, email,
                    orderManagementQueryResult, verificationKey, confirmChangeEmailUrl);

                //idOfLastOrderOrderRow = importedOrder.OrderRows.Single(or => or.RowStatus == OrderRowStatus.Active).idOrderRow;
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