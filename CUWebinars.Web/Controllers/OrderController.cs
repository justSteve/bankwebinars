using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;
using CUWebinars.Business.Constants;
using CUWebinars.Business.Models;
using CUWebinars.Business.Services;
using CUWebinars.Web.Core;
using CUWebinars.Web.Core.Orchestrators;
using CUWebinars.Web.Helpers;
using CUWebinars.Web.Models;
using Newtonsoft.Json.Linq;
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
        private IOrderManagementService _orderManagementService;
        private IWebinarManagementService _webinarManagementService;
        private readonly IAppHelper _appHelper;

        private bool _disposed;

        public OrderController(ILogger logger, IOrderControllerOrchestrator orderControllerOrchestrator, IOrderManagementService orderManagementService, IWebinarManagementService webinarManagementService, IAppHelper appHelper)
        {
            _logger = logger;
            _orderControllerOrchestrator = orderControllerOrchestrator;
            _orderManagementService = orderManagementService;
            _webinarManagementService = webinarManagementService;
            _appHelper = appHelper;
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult jf_Registrations(JotFormModel incomingFormCollection)
        {
            int idOfLastOrder = default(int);
            string verificationKey = string.Empty;
            string confirmChangeEmailUrl = string.Empty;
            var incomingOrderModel = new IncomingOrderModel();

            //Debug.Assert(incomingOrderModel.Email != null, "incomingOrderModel.Email != null");
            _logger.Info("Order creation on email: " + incomingOrderModel.Email);

            var json_serializer = new JavaScriptSerializer();
            var rawInfo = (IDictionary<string, object>)json_serializer.DeserializeObject(incomingFormCollection.rawRequest);

            JotFormOrderModel incomingJotFormOrder = JsonConvert.DeserializeObject<JotFormOrderModel>(incomingFormCollection.rawRequest);


            if (!ModelState.IsValid)
            {
                _logger.Fatal("Invalid JotFormModel State: " + incomingOrderModel.Email);

                var myError = ProcessModelStateErrors();
                return Json(new { Result = WebUiConstants.Fail, Errors = myError });
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
                incomingOrderModel.BillingAddress.Phone = "(" + incomingJotFormOrder.q6_phoneNumber6.area + ")" + incomingJotFormOrder.q6_phoneNumber6.phone;
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

                incomingOrderModel.idWebinar = Convert.ToInt32(incomingJotFormOrder.q11_webinarid);



                var orderManagementQueryResult = _orderControllerOrchestrator.GetPreparatoryData(incomingOrderModel,
                    incomingOrderModel.Email);
                var webinar = orderManagementQueryResult.Webinar;
                var idRegtype = 0;
                //"Live Session Only - $155"
                var label = incomingJotFormOrder.q10_registrationType.ToLower().Split('-')[0].Trim();
                switch (label)
                {
                    case "live session only":
                        idRegtype = webinar.Duration == 1 ? 97 : 84;
                        break;

                    case "ondemand recording only":
                        idRegtype = webinar.Duration == 1 ? 98 : 85;
                        break;
                    case "cd":
                        // "cd-rom and hardcopy handouts": the split on - catches (incorrectly) cd-rom
                        idRegtype = webinar.Duration == 1 ? 99 : 86;
                        break;
                    case "live plus ondemand weblinks":
                        idRegtype = webinar.Duration == 1 ? 100 : 87;
                        break;

                    case "premier package":
                        idRegtype = webinar.Duration == 1 ? 101 : 88;
                        break;
                    default:
                        _logger.Error("invalid regType. Label passed: " + label);
                        break;
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

        [HttpGet]
        public JsonResult GetWebinarDetails(int idWebinar)
        {

            var globals = GlobalConfig.GlobalConfigSingleton;
            var options = _orderManagementService.GetOptionsByWebinarId(idWebinar, false);
            Object listOfOptions = new object();

            var result = JsonConvert.SerializeObject(options);

            var webinarDetails = _webinarManagementService.GetWebinar(idWebinar);

            var topics = _webinarManagementService.GetTopicsPerWebinar(webinarDetails.idWebinar);

            var showTopics = "";
            foreach (var topic in topics)
            {
                showTopics = showTopics + topic.topicDesc + ", ";
            }
            showTopics = showTopics.TrimEnd(' ');
            showTopics = showTopics.TrimEnd(',');
            var presenterPhoto = globals.ImgRepository + webinarDetails.Presenter.PhotoFull;
            return Json(new { Webinar = new { webinarDetails.Title, webinarDetails.Date, webinarDetails.Duration, webinarDetails.Description, webinarDetails.DescriptionLong, webinarDetails.Presenter.WebUser.FullName, webinarDetails.Presenter.BiographyLong, presenterPhoto, showTopics, webinarDetails.ceu, webinarDetails.LearnCaption, webinarDetails.LearnBody, webinarDetails.Status }, RegTypes = result, }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// For us to use a query string, this has to be a GET request.
        /// </summary>
        /// <param name="incomingOrderModel"></param>
        /// <returns></returns>
        [HttpGet]
        public JsonResult CreateOrder(IncomingOrderModel incomingOrderModel)
        {
            if (!ModelState.IsValid)
            {
                var myError = ProcessModelStateErrors();
                return Json(new { Result = WebUiConstants.Fail, Error = myError });
            }
            //http://v3.bankwebinars.com/order/createorder?idAffiliate=12014&BillingAddress.AddressType=Billing&BillingAddress.Name=CDRom%20UpcomingHasConnInfo&BillingAddress.Phone=%28256%29%20837-6110&BillingAddress.StreetAddress=220%20Wynn%20Drive&BillingAddress.StreetAddress2=%20&BillingAddress.City=Huntsville&BillingAddress.Zip=35893&BillingAddress.State=AL&BillingAddress.Country=USA&ShippingAddress.AddressType=Shipping&ShippingAddress.Name=CDRom%20UpcomingHasConnInfo&ShippingAddress.Phone=%28256%29%20837-6110&ShippingAddress.StreetAddress=220%20Wynn%20Drive&ShippingAddress.StreetAddress2=%20&ShippingAddress.City=Huntsville&ShippingAddress.Zip=35553&ShippingAddress.State=AL&ShippingAddress.Country=USA&Email=CDRomHasConnectionInfo@ttstrain1.com&Title=VP/Chief%20Compliance%20Officer&Institution=Grone%20Federal%20Credit%20Union&FirstName=CDRom&LastName=UpcomingHasConnInfo&idRegType=99&idWebinar=3558

            string queryString = GetQueryString(incomingOrderModel);
            WebClient client = new WebClient();
            string url = "http://importer.bankwebinars.com/Registrations/ImportOrders3/?" + queryString;
            string result = client.DownloadString(url);
            
            return Json(new { Result = result });

        }

        //http://stackoverflow.com/questions/6848296/how-do-i-serialize-an-object-into-query-string-format
        public string GetQueryString(object obj)
        {
            var properties = from p in obj.GetType().GetProperties()
                             where p.GetValue(obj, null) != null
                             select p.Name + "=" + HttpUtility.UrlEncode(p.GetValue(obj, null).ToString());

            return String.Join("&", properties.ToArray());
        }

        /// <summary>
        /// For us to use a query string, this has to be a GET request.
        /// </summary>
        /// <param name="incomingOrderModel"></param>
        /// <returns></returns>
        [HttpGet]
        public JsonResult CreateOrder2(IncomingOrderModel incomingOrderModel)
        {
            int idOfLastOrder = default(int);
            string verificationKey = string.Empty;
            string confirmChangeEmailUrl = string.Empty;

            if (!ModelState.IsValid)
            {
                var myError = ProcessModelStateErrors();
                return Json(new { Result = WebUiConstants.Fail, Error = myError });
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

        private string ProcessModelStateErrors()
        {
            //Please use this general pattern when logging ModelState errors.
            var myErr = "ProcessModelStateErrors found errors. Session Info: " + Environment.NewLine;
            myErr += _appHelper.GetUserAuditInfo();

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
            return myErr;
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