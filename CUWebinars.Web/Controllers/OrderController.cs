using System.Collections.Generic;
using System.Web.Script.Serialization;
using CUWebinars.Business.Constants;
using CUWebinars.Business.Models;
using CUWebinars.Web.Core.Orchestrators;
using CUWebinars.Web.Helpers;
using CUWebinars.Web.Models;
using Ninject.Extensions.Logging;
using System;
using System.Web.Mvc;
using Newtonsoft.Json;

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

        [HttpPost]
        [AllowAnonymous]
        public ActionResult jf_Registrations(JotFormModel incomingFormCollection)
        {
            int idOfLastOrder = default(int);
            string verificationKey = string.Empty;
            string confirmChangeEmailUrl = string.Empty;
            var incomingOrderModel = new IncomingOrderModel();

            var json_serializer = new JavaScriptSerializer();
            var rawInfo = (IDictionary<string, object>)json_serializer.DeserializeObject(incomingFormCollection.rawRequest);

            JotFormOrderModel incomingJotFormOrder = JsonConvert.DeserializeObject<JotFormOrderModel>(incomingFormCollection.rawRequest);
            
            
            if (!ModelState.IsValid)
            {
                ProcessModelStateErrors();
                return Json(new { Result = WebUiConstants.Fail });
            }

            try
            {
                incomingOrderModel.idAffiliate = 19;
                incomingOrderModel.AffiliateComments = "form.submissionID: " + incomingFormCollection.submissionID;
                incomingOrderModel.BillingAddress = new Address();
                incomingOrderModel.BillingAddress.AddressType = "Billing";
                incomingOrderModel.BillingAddress.City = incomingJotFormOrder.q7_address.city;
                incomingOrderModel.BillingAddress.Country = incomingJotFormOrder.q7_address.country;
                incomingOrderModel.BillingAddress.Name = incomingJotFormOrder.q4_name.first + " " + incomingJotFormOrder.q4_name.last;
                incomingOrderModel.BillingAddress.Phone = "("+ incomingJotFormOrder.q6_phoneNumber6.area + ")"+incomingJotFormOrder.q6_phoneNumber6.phone;
                incomingOrderModel.BillingAddress.State = incomingJotFormOrder.q7_address.state;
                incomingOrderModel.BillingAddress.StreetAddress = incomingJotFormOrder.q7_address.addr_line1;
                incomingOrderModel.BillingAddress.StreetAddress2 = incomingJotFormOrder.q7_address.addr_line2;
                incomingOrderModel.BillingAddress.Zip = incomingJotFormOrder.q7_address.postal;
                incomingOrderModel.ShippingAddress = new Address();
                incomingOrderModel.ShippingAddress.AddressType = "Shipping";
                incomingOrderModel.ShippingAddress.City = incomingJotFormOrder.q7_address.city;
                incomingOrderModel.ShippingAddress.Country = incomingJotFormOrder.q7_address.country;
                incomingOrderModel.ShippingAddress.Name = incomingJotFormOrder.q4_name.first + " " + incomingJotFormOrder.q4_name.last;
                incomingOrderModel.ShippingAddress.Phone = "(" + incomingJotFormOrder.q6_phoneNumber6.area + ")" + incomingJotFormOrder.q6_phoneNumber6.phone;
                incomingOrderModel.ShippingAddress.State = incomingJotFormOrder.q7_address.state;
                incomingOrderModel.ShippingAddress.StreetAddress = incomingJotFormOrder.q7_address.addr_line1;
                incomingOrderModel.ShippingAddress.StreetAddress2 = incomingJotFormOrder.q7_address.addr_line2;
                incomingOrderModel.ShippingAddress.Zip = incomingJotFormOrder.q7_address.postal;
                

                incomingOrderModel.Email = incomingJotFormOrder.q5_email5;
                incomingOrderModel.FirstName = incomingJotFormOrder.q4_name.first;
                incomingOrderModel.LastName = incomingJotFormOrder.q4_name.last;
                //incomingOrderModel. = incomingJotFormOrder.q8_institution;
                incomingOrderModel.Institution = incomingJotFormOrder.q8_institution;
                incomingOrderModel.Origin = "jotFormRestRequest";
                incomingOrderModel.Title = incomingJotFormOrder.q9_title;
                
                incomingOrderModel.idWebinar= Convert.ToInt32(incomingJotFormOrder.q11_webinarid);

                
                _logger.Info("Begin import: " + incomingOrderModel.Email);

                var orderManagementQueryResult = _orderControllerOrchestrator.GetPreparatoryData(incomingOrderModel,
                    incomingOrderModel.Email);
                var webinar = orderManagementQueryResult.Webinar;
                var idRegtype = 0;
                //"Live Session Only - $155"
                if (string.Equals(incomingJotFormOrder.q10_registrationType.ToLower().Split('-')[0].Trim(), "live session only"))
                {
                    if (webinar.Duration == 1)
                        idRegtype = 97;
                    else
                        idRegtype = 84;
                }

                if (string.Equals(incomingJotFormOrder.q10_registrationType.ToLower().Split('-')[0].Trim(), "ondemand recording only"))
                {
                    if (webinar.Duration == 1)
                        idRegtype = 98;
                    else
                        idRegtype = 85;
                }

                if (string.Equals(incomingJotFormOrder.q10_registrationType.ToLower().Split('-')[0].Trim(), "cd-rom and hardcopy handouts"))
                {
                    if (webinar.Duration == 1)
                        idRegtype = 99;
                    else
                        idRegtype = 86;
                }

                if (string.Equals(incomingJotFormOrder.q10_registrationType.ToLower().Split('-')[0].Trim(), "live plus ondemand weblinks"))
                {
                    if (webinar.Duration == 1)
                        idRegtype = 100;
                    else
                        idRegtype = 87;
                }

                if (string.Equals(incomingJotFormOrder.q10_registrationType.ToLower().Split('-')[0].Trim(), "premier package"))
                {
                    if (webinar.Duration == 1)
                        idRegtype = 101;
                    else
                        idRegtype = 88;
                }

                incomingOrderModel.idRegType = idRegtype;


                if (ReferenceEquals(null, orderManagementQueryResult.WebUser))
                {
                    try
                    {
                        orderManagementQueryResult.WebUser =
                            _orderControllerOrchestrator.ProcessNewUser(incomingOrderModel, incomingOrderModel.Email);

                        verificationKey = _orderControllerOrchestrator.GetVerificationKeyForNewUserAccount();

                        confirmChangeEmailUrl =
                            _orderControllerOrchestrator.GetConfirmChangeEmailLinkForNewUserAccount();

                        _orderControllerOrchestrator.FinalizeNewRegistration(incomingOrderModel, verificationKey);
                        //  Now we clear the value, so TtsSmtpMessageDelivery can go back to business as usual.
                        //_stateService.ClearValue(DomainConstants.UserCreatedViaNewOrder);
                        _logger.Info(string.Format("CreateOrder|CreateUser Succeeded: {0}", incomingOrderModel.Email));
                    }
                    catch (Exception exception)
                    {
                        _logger.ErrorException(
                            string.Format("CreateOrder|CreateUser failed: {0}", exception.Message), exception);
                        throw;
                    }
                }

                idOfLastOrder = _orderControllerOrchestrator.CreateNewOrder(incomingOrderModel, incomingOrderModel.Email,
                    orderManagementQueryResult, verificationKey, confirmChangeEmailUrl);

                //idOfLastOrderOrderRow = importedOrder.OrderRows.Single(or => or.RowStatus == OrderRowStatus.Active).idOrderRow;
                _logger.Info(string.Format("CreateOrder|CreateNewOrder: {0}", idOfLastOrder));

                return Json(new { Result = idOfLastOrder.ToString() }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception exception)
            {
                var errString = string.Format("Order creation failed on {0} - {1} with msg: {2}",
                   incomingOrderModel.Email, incomingOrderModel.idWebinar, exception.Message);
                _logger.ErrorException(errString, exception);
            }

            //return View("Error");
            return Json(new { Result = idOfLastOrder }, JsonRequestBehavior.AllowGet);
            //return Json(new { Result = WebUiConstants.Fail });}
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
                        //  Now we clear the value, so TtsSmtpMessageDelivery can go back to business as usual.
                        //_stateService.ClearValue(DomainConstants.UserCreatedViaNewOrder);
                        _logger.Info(string.Format("CreateOrder|CreateUser Succeeded: {0}", email));
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

                //idOfLastOrderOrderRow = importedOrder.OrderRows.Single(or => or.RowStatus == OrderRowStatus.Active).idOrderRow;
                _logger.Info(string.Format("CreateOrder|CreateNewOrder: {0}", idOfLastOrder));

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