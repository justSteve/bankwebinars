using CUWebinars.Business.Services;
using CUWebinars.Web.Core;
using CUWebinars.Web.Core.Orchestrators;
using CUWebinars.Web.Helpers;
using CUWebinars.Web.Membership;
using CUWebinars.Web.Models;
using HtmlAgilityPack;
using Newtonsoft.Json;
using Ninject.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using CUWebinars.Business.Core;

namespace CUWebinars.Web.Controllers
{
    public class OrderController : Controller
    {

        private readonly ILogger _logger;
        private readonly IOrderControllerOrchestrator _orderControllerOrchestrator;
        private IOrderManagementService _orderManagementService;
        private IWebinarManagementService _webinarManagementService;
        private readonly IAppHelper _appHelper;
        private readonly GlobalConfig _globalConfig = GlobalConfig.GlobalConfigSingleton;

        public string MyGuid = Guid.NewGuid().ToString();
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
        /// Here is the original 'spreadsheet-based' importer
        /// 
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

                _logger.Info("CreateOrder2 Begin import: " + email);

                var orderManagementQueryResult = _orderControllerOrchestrator.GetPreparatoryData(incomingOrderModel, email);

                bool userAlreadyExists = true;

                if (ReferenceEquals(null, orderManagementQueryResult.WebUser))
                {
                    try
                    {
                        userAlreadyExists = false;

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
                    orderManagementQueryResult, verificationKey, confirmChangeEmailUrl, userAlreadyExists);

                var newOrder = _orderManagementService.GetOrderById(idOfLastOrder);
                if (idOfLastOrder == 0)
                {
                    newOrder = _orderManagementService.GetOrdersByUserId(orderManagementQueryResult.WebUser.idUser)
                        .Where(o => o.OrderRows.SingleOrDefault(or => or.idWebinar == incomingOrderModel.idWebinar) != null)
                        .SingleOrDefault();
                    ;
                }

                _logger.Info("ACS Importer heard: " + newOrder.BillingEmail);

                newOrder.OrderDate = TtsConfig.UtcNowAsCts;
                _orderManagementService.SaveChanges();

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
        /// Permits migration of legacy system.
        /// </summary>
        /// <param name="migrateOrderModel"></param>
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

                _logger.Info(string.Format("MigrateOrder|CreateNewOrder: {0}", idOfLastOrder));

                return Json(new { Result = idOfLastOrder.ToString() }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception exception)
            {
                var errString = string.Format("Order creation failed on {0} - {1} with msg: {2}", migratedOrder.Email, migratedOrder.idWebinar, exception.Message);
                Elmah.ErrorSignal.FromCurrentContext().Raise(exception);
                _logger.ErrorException(errString, exception);

                return Json(new { Result = errString }, JsonRequestBehavior.AllowGet);
            }

        }

        private string ProcessModelStateErrors()
        {
            //Please use this general pattern when logging ModelState errors.
            var myErr = "ProcessModelStateErrors found errors.";

            foreach (ModelState modelState in ViewData.ModelState.Values)
            {
                foreach (ModelError error in modelState.Errors)
                {
                    myErr += error.ErrorMessage + Environment.NewLine;
                }
            }
            myErr += "Session Info: " + Environment.NewLine;
            myErr += _appHelper.GetUserAuditInfo();

            //a better implementation:
            //http://stackoverflow.com/questions/2845852/asp-net-mvc-how-to-convert-modelstate-errors-to-json
            //var errorList = ModelState.ToDictionary(
            //    kvp => kvp.Key,
            //    kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray()
            //);

            _logger.Error(myErr);
            return myErr;
        }


        /// <summary>
        /// Imports orders based on form submissions from Affiliate Import Sheets.
        /// </summary>
        /// <param name="ImportOrderModel"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult ImportOrder(ImportOrderModel importedOrder)
        {
            int idOfLastOrder = default(int);
            string verificationKey = string.Empty;
            string confirmChangeEmailUrl = string.Empty;

            if (!ModelState.IsValid)
            {
                var myError = ProcessModelStateErrors();
                return Json(new { Result = WebUiConstants.Fail, Error = myError });
            }
            int idRegType;
            if (importedOrder.idAffiliate == 62)
            {
                if (importedOrder.Source == "ACSGmailImportViaWebJob?Version=3")
                {
                    _logger.Info("ACS Importer hears: " + importedOrder.Email + " idWebinar: " + importedOrder.idWebinar);
                    idRegType = _webinarManagementService.GetRegTypeByACS(importedOrder.RegistrationType,
                        importedOrder.idWebinar);
                }
                else
                {
                    return Json(new { Result = 0 }, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                idRegType = _webinarManagementService.GetRegTypeByLableAndWebinar(importedOrder.RegistrationType,
                     importedOrder.idWebinar);
            }

            if (idRegType == 0)
            {
                _logger.Fatal(string.Format("idRegType comes up 0. RegType = {0}; email = {1}; orderDate = {2};", importedOrder.RegistrationType, importedOrder.Email, importedOrder.OrderDate));
                return Json(new { Result = WebUiConstants.Fail, Error = "Invalid Registration Type: " + importedOrder.RegistrationType });
            }
            importedOrder.RegistrationType = idRegType.ToString();
            try
            {
                var email = importedOrder.Email.Trim();

                _logger.Info("Begin import: " + email);

                var importQueryResult = _orderControllerOrchestrator.GetPreparatoryDataForImporter(importedOrder
                    , email);

                bool userAlreadyExists = true;

                // If WebUser is null, it is a new user that will be created upon importation of this order.
                if (ReferenceEquals(null, importQueryResult.WebUser))
                {
                    try
                    {
                        userAlreadyExists = false;

                        importQueryResult.WebUser =
                            _orderControllerOrchestrator.ImportUser(importedOrder, email);

                        verificationKey = _orderControllerOrchestrator.GetVerificationKeyForNewUserAccount();

                        confirmChangeEmailUrl =
                            _orderControllerOrchestrator.GetConfirmChangeEmailLinkForNewUserAccount();

                        _orderControllerOrchestrator.FinalizeImportedRegistation(verificationKey);

                        _logger.Info(string.Format("ImportOrder|CreateUser Succeeded: {0}", email));
                    }
                    catch (Exception exception)
                    {
                        _logger.ErrorException(
                            string.Format("ImportOrder|CreateUser failed: {0}", exception.Message), exception);
                        Elmah.ErrorSignal.FromCurrentContext().Raise(exception);
                        throw;
                    }
                }

                idOfLastOrder = _orderControllerOrchestrator.ImportOrder(importedOrder, email,
                    importQueryResult, verificationKey, confirmChangeEmailUrl, userAlreadyExists);

                if (importedOrder.idAffiliate == 62)
                {
                    var newOrder = _orderManagementService.GetOrderById(idOfLastOrder);
                    if (idOfLastOrder == 0)
                    {
                        newOrder = _orderManagementService.GetOrdersByUserId(importQueryResult.WebUser.idUser)
                            .Where(o => o.OrderRows.SingleOrDefault(or => or.idWebinar == importedOrder.idWebinar) != null)
                            .SingleOrDefault();
                        ;
                    }

                    _logger.Info("ACS Importer heard: " + newOrder.BillingEmail);


                    newOrder.OrderDate = importedOrder.OrderDate;
                    _orderManagementService.SaveChanges();
                }

                //idOfLastOrderOrderRow = importedOrder.OrderRows.Single(or => or.RowStatus == OrderRowStatus.Active).idOrderRow;
                _logger.Info(string.Format("ImportOrder from {0} produced: {1}", importedOrder.Source + "-" + importedOrder.Version, idOfLastOrder));
                return Json(new { Result = idOfLastOrder.ToString() }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception exception)
            {
                var errString = string.Format("ImportOrder from {3} failed on {0} - {1} with msg: {2}", importedOrder.Email, importedOrder.idWebinar, exception.Message, importedOrder.Source + "-" + importedOrder.Version);
                Elmah.ErrorSignal.FromCurrentContext().Raise(exception);
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

                _orderControllerOrchestrator.Dispose();
                _orderManagementService.Dispose();
                _webinarManagementService.Dispose();

                base.Dispose(true);
            }
            _disposed = true;
        }
    }
}