using CUWebinars.Business.Services;
using CUWebinars.Web.Core;
using CUWebinars.Web.Core.Orchestrators;
using CUWebinars.Web.Helpers;
using CUWebinars.Web.Models;
using Newtonsoft.Json;
using Ninject.Extensions.Logging;
using System;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

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

        [HttpGet]
        public JsonResult GetWebinarDetails(int idWebinar)
        {

            var globals = GlobalConfig.GlobalConfigSingleton;
            var options = _orderManagementService.GetOptionsByWebinarId(idWebinar, false);

            var result = JsonConvert.SerializeObject(options);

            var webinarDetails = _webinarManagementService.GetWebinar(idWebinar);

            var topics = _webinarManagementService.GetTopicsPerWebinar(webinarDetails.idWebinar);

            var showTopics = string.Empty;
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
                return Json(new { Result = WebUiConstants.Fail, Error = myError }, JsonRequestBehavior.AllowGet);
            }
            //http://v3.bankwebinars.com/order/createorder?idAffiliate=12014&BillingAddress.AddressType=Billing&BillingAddress.Name=CDRom%20UpcomingHasConnInfo&BillingAddress.Phone=%28256%29%20837-6110&BillingAddress.StreetAddress=220%20Wynn%20Drive&BillingAddress.StreetAddress2=%20&BillingAddress.City=Huntsville&BillingAddress.Zip=35893&BillingAddress.State=AL&BillingAddress.Country=USA&ShippingAddress.AddressType=Shipping&ShippingAddress.Name=CDRom%20UpcomingHasConnInfo&ShippingAddress.Phone=%28256%29%20837-6110&ShippingAddress.StreetAddress=220%20Wynn%20Drive&ShippingAddress.StreetAddress2=%20&ShippingAddress.City=Huntsville&ShippingAddress.Zip=35553&ShippingAddress.State=AL&ShippingAddress.Country=USA&Email=CDRomHasConnectionInfo@ttstrain1.com&Title=VP/Chief%20Compliance%20Officer&Institution=Grone%20Federal%20Credit%20Union&FirstName=CDRom&LastName=UpcomingHasConnInfo&idRegType=99&idWebinar=3558

            string queryString = GetQueryString(incomingOrderModel);
            WebClient client = new WebClient();
            string url = "http://importer.bankwebinars.com/Registrations/ImportOrders3/?" + queryString;
            string result = client.DownloadString(url);

            return Json(new { Result = result }, JsonRequestBehavior.AllowGet);

        }

        //http://stackoverflow.com/questions/6848296/how-do-i-serialize-an-object-into-query-string-format
        public string GetQueryString(object obj)
        {
            var properties = from p in obj.GetType().GetProperties()
                             where p.GetValue(obj, null) != null
                             select p.Name + "=" + HttpUtility.UrlEncode(p.GetValue(obj, null).ToString());

            return string.Join("&", properties.ToArray());
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
                        Elmah.ErrorSignal.FromCurrentContext().Raise(exception);
                        throw;
                    }
                }

                idOfLastOrder = _orderControllerOrchestrator.CreateNewOrder(incomingOrderModel, email,
                    orderManagementQueryResult, verificationKey, confirmChangeEmailUrl);

                _logger.Info(string.Format("CreateOrder|CreateNewOrder: {0}", idOfLastOrder));

                return Json(new { Result = idOfLastOrder.ToString() }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception exception)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(exception);
                var errString = string.Format("Order creation failed on {0} - {1} with msg: {2}", incomingOrderModel.Email, incomingOrderModel.idWebinar, exception.Message);
                _logger.ErrorException(errString, exception);
            }

            return Json(new { Result = "0" }, JsonRequestBehavior.AllowGet);
            //return Json(new { Result = WebUiConstants.Fail });
        }

        /// <summary>
        /// For us to use a query string, this has to be a GET request.
        /// </summary>
        /// <param name="incomingOrderModel"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult MigrateOrder(MigrateOrderModel migratedOrder)
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
                var email = migratedOrder.Email.Trim();

                _logger.Info("Begin migrate: " + email);

                var migratorQueryResult = _orderControllerOrchestrator.GetPreparatoryDataForMigrator(migratedOrder
                    , email);

                if (ReferenceEquals(null, migratorQueryResult.WebUser))
                {
                    try
                    {
                        migratorQueryResult.WebUser =
                            _orderControllerOrchestrator.MigrateUser(migratedOrder, email);

                        verificationKey = _orderControllerOrchestrator.GetVerificationKeyForNewUserAccount();

                        confirmChangeEmailUrl =
                            _orderControllerOrchestrator.GetConfirmChangeEmailLinkForNewUserAccount();

                        _orderControllerOrchestrator.FinalizeMigratedRegistation(migratedOrder, verificationKey);

                        _logger.Info(string.Format("MigrateOrder|CreateUser Succeeded: {0}", email));
                    }
                    catch (Exception exception)
                    {
                        _logger.ErrorException(
                            string.Format("MigrateOrder|CreateUser failed: {0}", exception.Message), exception);
                        Elmah.ErrorSignal.FromCurrentContext().Raise(exception);
                        throw;
                    }
                }

                idOfLastOrder = _orderControllerOrchestrator.MigrateOrder(migratedOrder, email,
                    migratorQueryResult, verificationKey, confirmChangeEmailUrl);

                //idOfLastOrderOrderRow = importedOrder.OrderRows.Single(or => or.RowStatus == OrderRowStatus.Active).idOrderRow;
                _logger.Info(string.Format("MigrateOrder|CreteNewOrder: {0}", idOfLastOrder));

                return Json(new { Result = idOfLastOrder.ToString() }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception exception)
            {
                var errString = string.Format("Order creation failed on {0} - {1} with msg: {2}", migratedOrder.Email, migratedOrder.idWebinar, exception.Message);
                Elmah.ErrorSignal.FromCurrentContext().Raise(exception);
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