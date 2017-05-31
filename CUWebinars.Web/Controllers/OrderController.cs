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
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using CUWebinars.Business.Constants;
using CUWebinars.Business.Core;
using CUWebinars.Business.Core.Helpers;
using CUWebinars.Business.Models;
using CUWebinars.Web.Models.Importers;
using DDay.iCal;
using Newtonsoft.Json.Linq;

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

        [HttpPost]
        //[ValidateAntiForgeryToken(Order = 0)]
        public ActionResult NotiResults(NotiResultsModel model)
        {
            var order = _orderManagementService.GetOrderById(model.idOrder);


            var recordNotiResults = "";
            if (order == null)
            {
                _logger.Fatal("NotiResults found null order!" + model.Id);
                return null;
            }
            var row =

                    order.OrderRows.SingleOrDefault(r => r.RowStatus == OrderRowStatus.Active);
            Debug.Assert(row != null, "row != null at notiResults");
            var webinar = _orderManagementService.GetWebinarById(row.idWebinar);
            model.ResultId = order.idOrder + "_" + DateTime.Now.ToShortDateString().Replace(" ", "_");

            JProperty msg = new JProperty(JsonPropertyKeys.NotiResults + "_" + model.idOrder + "_" + DateTime.Now.ToShortDateString()
                , JToken.Parse(JsonConvert.SerializeObject(model)));

            JObject result = (JObject)JToken.FromObject(model);

            JObject wComments = JObject.Parse(webinar.Comments);
            JArray resultsArray = new JArray();
            foreach (var pair in wComments)
            {
                Console.WriteLine(pair.Key);
                if (pair.Key.StartsWith("Noti"))
                {
                    resultsArray = JArray.Parse(pair.Value.ToString());
                }
            }
            if (resultsArray == null)
            {
                resultsArray = new JArray();
                resultsArray.Add(result);

            }
            else
            {
                resultsArray.Add(result);
            }
            webinar.Comments = JsonHelpers.ReplaceJsonWithStoredField(webinar.Comments, new JProperty("NotiResults", resultsArray), JsonPropertyKeys.NotiResults);

            order.UserComments = JsonHelpers.MergeJsonWithStoredField(order.UserComments, msg);

            _orderManagementService.SaveChanges();

            //webinar.Comments = JsonHelpers.ReplaceJsonWithStoredField();
            _webinarManagementService.SaveChanges();
            return null;
        }
        [HttpPost]
        //[ValidateAntiForgeryToken(Order = 0)]
        public ActionResult AuditConnectionChecklistSender(int idWebinar)
        {
            var webinar = _orderManagementService.GetWebinarById(idWebinar);
            var orders = _orderManagementService.GetOrdersForLiveNotifications(webinar.idWebinar);
            var notiResultsToJson = JObject.Parse(webinar.Comments);
            Debug.Assert(notiResultsToJson != null, "notiResultsToJson != null");
            //var listOfNotiResults = notiResultsToJson.Properties().Select(p => p.Name).ToList();

            //var recordNotiResults = "";
            //if (webinar == null)
            //{
            //    _logger.Fatal("Found no such webinar!" + idWebinar);
            //    return null;
            //}
            //var row =

            //        webinar.OrderRows.SingleOrDefault(r => r.RowStatus == OrderRowStatus.Active);
            //Debug.Assert(row != null, "row != null at notiResults");


            return null;
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
                            string.Format("MigrateOrder|CreateUser failed: {0}", email), exception);
                        Elmah.ErrorSignal.FromCurrentContext().Raise(exception);
                        throw;
                    }
                }

                idOfLastOrder = _orderControllerOrchestrator.MigrateOrder(migratedOrder, email,
                    migratorQueryResult, verificationKey, confirmChangeEmailUrl);

                _logger.Info(string.Format("MigrateOrder|CreateNewOrder: {0}", idOfLastOrder));

                //Order resultOrder = _orderManagementService.GetOrderById(idOfLastOrder);
                //var discount =
                //    resultOrder.OrderRows.SingleOrDefault(o => o.RowStatus == OrderRowStatus.Active).Discount;
                //_logger.Info("Discount: RedeemDiscountStarts: {0}, validFrom: {1}, validTo: {2}, CreditedUsed: {3}, CreditsRemain: {4}", discount.DiscountCode, discount.DateValidFrom, discount.DateValidTo, discount.CreditsUsed, discount.CreditsRemain);
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

        public void MigrateCompliancePerspectivesOrders()
        {
            IList<Order> orders =
                _orderManagementService.GetOrdersByWebinar(842).Where(o => o.OrderStatus == OrderStatus.Billed || o.OrderStatus == OrderStatus.Submitted || o.OrderStatus == OrderStatus.OutstandingBalance || o.OrderStatus == OrderStatus.Paid).ToList();
            _logger.Info("CPMigrator found " + orders.Count + " orders to process.");
            var nextCPId = _webinarManagementService.GetNextCompliancePerspectives();

            foreach (var o in orders)
            {
                _logger.Info("CPMigrator begins: " + o.BillingEmail);
                var row = o.OrderRows.Single(r => r.RowStatus == OrderRowStatus.Active);
                var addLocString = "";
                if (row.AdditionalLocation != null)
                {
                    foreach (var addLoc in row.AdditionalLocation)
                    {
                        addLocString += addLoc.Email + ",";
                    }
                }
                try
                {
                    var thisOrder = new MigrateOrderModel
                    {
                        idWebinar = nextCPId.Value,
                        idAffiliate = o.idAffiliate,
                        Email = o.BillingEmail,
                        Title = o.WebUser.Title ?? "",
                        Status = o.OrderStatus,
                        AdditionalLocationsString = addLocString.TrimEnd(','),
                        idOrderLegacy = 0,
                        idRegType = row.idRegType,
                        OrderDate = DateTime.Now,
                        Total = o.Total,
                        BillingAddress = new Address
                        {
                            StreetAddress = o.BillingAddress,
                            StreetAddress2 = o.BillingAddress2,
                            City = o.BillingCity,
                            State = o.BillingState,
                            AddressType = "0",
                            Phone = o.BillingPhone,
                            Name = o.WebUser.FullName,
                            Zip = o.BillingZip
                        },
                        ShippingAddress = new Address
                        {
                            StreetAddress = o.ShippingAddress,
                            StreetAddress2 = o.ShippingAddress2,
                            City = o.ShippingCity,
                            State = o.ShippingState,
                            AddressType = "1",
                            Phone = o.ShippingPhone,
                            Name = o.WebUser.FullName,
                            Zip = o.ShippingZip
                        },

                        Institution = o.Institution,
                        DiscountCode = null,
                        AdminComments = "",
                        FirstName = o.FirstName,
                        LastName = o.LastName,
                        Origin = o.Origin,
                        LegacyRegType = 0,
                        SendNotification = false
                    };
                    thisOrder.DiscountCode = "CP_" + o.idOrder;
                    var makeCPOrder = MigrateOrderCompPerspectivesPost(thisOrder);
                    _logger.Info(makeCPOrder.ToString());


                }
                catch (Exception ex)
                {
                    _logger.FatalException("migrating CP failed on: " + o.idOrder + "  with: ", ex);
                }
            }
        }
        /// <summary>
        /// Permits migration of legacy system.
        /// </summary>
        /// <param name="migrateOrderModel"></param>
        /// <returns></returns>

        public JsonResult MigrateOrderCompPerspectivesPost(MigrateOrderModel migratedOrder)
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
                            string.Format("MigrateOrder|CreateUser failed: {0}", email), exception);
                        Elmah.ErrorSignal.FromCurrentContext().Raise(exception);
                        throw;
                    }
                }

                idOfLastOrder = _orderControllerOrchestrator.MigrateOrder(migratedOrder, email,
                    migratorQueryResult, verificationKey, confirmChangeEmailUrl);

                _logger.Info(string.Format("MigrateOrder|CreateNewOrder: {0}", idOfLastOrder));

                //Order resultOrder = _orderManagementService.GetOrderById(idOfLastOrder);
                //var discount =
                //    resultOrder.OrderRows.SingleOrDefault(o => o.RowStatus == OrderRowStatus.Active).Discount;
                //_logger.Info("Discount: RedeemDiscountStarts: {0}, validFrom: {1}, validTo: {2}, CreditedUsed: {3}, CreditsRemain: {4}", discount.DiscountCode, discount.DateValidFrom, discount.DateValidTo, discount.CreditsUsed, discount.CreditsRemain);
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
            var myErr = "ProcessModelStateErrors found errors. ";

            foreach (ModelState modelState in ViewData.ModelState.Values)
            {
                foreach (ModelError error in modelState.Errors)
                {
                    myErr += error.ErrorMessage + Environment.NewLine;
                }
            }
            myErr += " Session Info: " + Environment.NewLine;
            myErr += _appHelper.GetUserAuditInfo();

            //a better implementation:
            //http://stackoverflow.com/questions/2845852/asp-net-mvc-how-to-convert-modelstate-errors-to-json
            var errorList = ModelState.Where(kvp => kvp.Value.Errors.Count > 0)
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value.Errors.Select(e => e.ErrorMessage)
                    .ToArray()
            );

            _logger.Error(errorList.ToString());
            return myErr.ToString();
        }


        /// <summary>
        /// Imports orders based on form submissions from Affiliate Import Sheets.
        /// </summary>
        /// <param name="ImportOrderModel"></param>
        /// <returns></returns>
        /// 
        [AllowAnonymous]
        [HttpPost]
        [ValidateInput(false)]
        public JsonResult Importorder4Acs(ImportOrderForAcsModel _importedOrder)
        {
            _logger.Info("importorder4ACS Incoming Values: " + JsonConvert.SerializeObject(_importedOrder, Formatting.None, new JsonSerializerSettings { MaxDepth = 1, ReferenceLoopHandling = ReferenceLoopHandling.Ignore }));
            //_logger.Info("importorder4ACS AdditionalLocation: " + JsonConvert.SerializeObject(_importedOrder.AdditionalLocationsString, Formatting.None, new JsonSerializerSettings { MaxDepth = 1, ReferenceLoopHandling = ReferenceLoopHandling.Ignore }));
            if (ModelState.IsValid)
            {
                var idRegType = _webinarManagementService.GetRegTypeByACS(_importedOrder.DeliveryType, Convert.ToInt32(_importedOrder.BankWebID));
                string addLocString = null;

                if (_importedOrder.AdditionalLocationsString != null)
                {
                    addLocString = _importedOrder.AdditionalLocationsString.TrimEnd(',');
                }

                ImportOrderModel importedOrder = new ImportOrderModel
                {
                    idWebinar = Convert.ToInt32(_importedOrder.BankWebID),
                    Source = "ACSGmailImportViaWebJob?Version=3",
                    idAffiliate = 62,
                    RegistrationType = _importedOrder.DeliveryType,
                    OrderDate = Convert.ToDateTime(_importedOrder.DateSubmittedToACS),
                    BillingAddress = new Address
                    {
                        AddressType = "Billing",
                        City = _importedOrder.City,
                        Country = "US",
                        Name = _importedOrder.FirstName + " " + _importedOrder.LastName,
                        Phone = _importedOrder.Phone,
                        State = _importedOrder.State_Province_Region,
                        StreetAddress = _importedOrder.StreetorP_O_Box,
                        StreetAddress2 = "",
                        Zip = _importedOrder.Zip_PostalCode
                    },
                    ShippingAddress = new Address
                    {
                        AddressType = "Shipping",
                        City = _importedOrder.City,
                        Country = "US",
                        Name = _importedOrder.FirstName + " " + _importedOrder.LastName,
                        Phone = _importedOrder.Phone,
                        State = _importedOrder.State_Province_Region,
                        StreetAddress = _importedOrder.StreetorP_O_Box,
                        StreetAddress2 = "",
                        Zip = _importedOrder.Zip_PostalCode
                    },
                    Institution = _importedOrder.Company,
                    Title = _importedOrder.Title,
                    FirstName = _importedOrder.FirstName,
                    LastName = _importedOrder.LastName,

                    AdditionalLocationsString = addLocString,
                    Email = _importedOrder.Email


                };
                int idOfLastOrder = default(int);
                string verificationKey = string.Empty;
                string confirmChangeEmailUrl = string.Empty;

                if (!ModelState.IsValid)
                {
                    var myError = ProcessModelStateErrors();
                    return Json(new { Result = WebUiConstants.Fail, Error = myError });
                }
                _logger.Info("ACS EmailParser Input: " + JsonConvert.SerializeObject(importedOrder, Formatting.None, new JsonSerializerSettings { MaxDepth = 1, ReferenceLoopHandling = ReferenceLoopHandling.Ignore }));

                if (idRegType == 0)
                {
                    _logger.Fatal(string.Format("idRegType comes up 0. RegType = {0}; email = {1}; orderDate = {2};",
                        importedOrder.RegistrationType, importedOrder.Email, importedOrder.OrderDate));
                    return
                        Json(
                            new
                            {
                                Result = WebUiConstants.Fail,
                                Error = "Invalid Registration Type: " + importedOrder.RegistrationType
                            });
                }
                importedOrder.RegistrationType = idRegType.ToString();


                try
                {
                    //_logger.Info("Begin import: " + JsonConvert.SerializeObject(importedOrder, Formatting.None, new JsonSerializerSettings { MaxDepth = 1, ReferenceLoopHandling = ReferenceLoopHandling.Ignore }));

                    var importQueryResult = _orderControllerOrchestrator.GetPreparatoryDataForImporter(importedOrder, importedOrder.Email.Trim());

                    bool userAlreadyExists = true;

                    // If WebUser is null, it is a new user that will be created upon importation of this order.
                    if (ReferenceEquals(null, importQueryResult.WebUser))
                    {
                        try
                        {
                            userAlreadyExists = false;

                            importQueryResult.WebUser =
                                _orderControllerOrchestrator.ImportUser(importedOrder, importedOrder.Email.Trim());

                            verificationKey = _orderControllerOrchestrator.GetVerificationKeyForNewUserAccount();

                            confirmChangeEmailUrl =
                                _orderControllerOrchestrator.GetConfirmChangeEmailLinkForNewUserAccount();

                            _orderControllerOrchestrator.FinalizeImportedRegistation(verificationKey);

                            _logger.Info(string.Format("ImportOrder|CreateUser Succeeded: {0}",
                                importedOrder.Email.Trim()));
                        }
                        catch (Exception exception)
                        {
                            _logger.ErrorException(
                                string.Format("ImportOrder|CreateUser failed: {0}", exception.Message), exception);
                            Elmah.ErrorSignal.FromCurrentContext().Raise(exception);
                            throw;
                        }
                    }
                    else
                    {
                        // send address info
                        var billingAdd = importQueryResult.WebUser.Addresses.Where(a => a.AddressType == "Billing").SingleOrDefault();
                        var shippingAdd = importQueryResult.WebUser.Addresses.Where(a => a.AddressType == "Shipping").SingleOrDefault();
                        if (!ReferenceEquals(null, billingAdd))
                        {
                            billingAdd.City = _importedOrder.City;
                            billingAdd.StreetAddress = _importedOrder.StreetorP_O_Box;
                            billingAdd.Phone = _importedOrder.Phone;
                            billingAdd.State = _importedOrder.State_Province_Region;
                            billingAdd.Zip = _importedOrder.Zip_PostalCode;
                            billingAdd.Name = _importedOrder.FirstName + " " + _importedOrder.LastName;
                        }
                        if (!ReferenceEquals(null, shippingAdd))
                        {
                            shippingAdd.City = _importedOrder.City;
                            shippingAdd.StreetAddress = _importedOrder.StreetorP_O_Box;
                            shippingAdd.Phone = _importedOrder.Phone;
                            shippingAdd.State = _importedOrder.State_Province_Region;
                            shippingAdd.Zip = _importedOrder.Zip_PostalCode;
                            shippingAdd.Name = _importedOrder.FirstName + " " + _importedOrder.LastName;
                        }
                        //_orderControllerOrchestrator.U
                    }

                    idOfLastOrder = _orderControllerOrchestrator.ImportOrder(importedOrder, importedOrder.Email.Trim(),
                        importQueryResult, verificationKey, confirmChangeEmailUrl, userAlreadyExists);

                    var newOrder = _orderManagementService.GetOrderById(idOfLastOrder);

                    try
                    {
                        _logger.Info("ACS Imported: " +
                                     JsonConvert.SerializeObject(newOrder, new JsonSerializerSettings()
                                     {
                                         ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                                     }));

                    }
                    catch (Exception ex)
                    {
                        _logger.FatalException("Attempt to seriealize order failed: " + newOrder.idOrder, ex);
                    }

                    newOrder.OrderDate = importedOrder.OrderDate;
                    newOrder.OrderStatus = OrderStatus.AwaitingVerification;

                    var cftTitleString = _importedOrder.AffiliateComments;

                    if (cftTitleString ==
                        newOrder.OrderRows.Single(r => r.RowStatus == OrderRowStatus.Active).Webinar.Title)
                    {
                        cftTitleString = "";
                    }
                    else
                    {
                        cftTitleString = "The title of this event at cftnow.org is: " + cftTitleString;
                    }

                    newOrder.UserComments = "{\"ACSImporter\": \"This registration originated at cftnow.org. " + cftTitleString + "\"}";

                    newOrder.AuditInfo = "{\"ACSImporter:\"" + JsonConvert.SerializeObject(_importedOrder) + "}";

                    _orderManagementService.SaveChanges();

                    _logger.Info(string.Format("ImportOrder from {0} produced: {1}", importedOrder.Source, idOfLastOrder));
                    return Json(new { Result = idOfLastOrder.ToString() }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception exception)
                {
                    var errString = string.Format("ImportOrder from {3} failed on {0} - {1} with msg: {2}",
                        importedOrder.Email, importedOrder.idWebinar, exception.Message,
                        importedOrder.Source);
                    Elmah.ErrorSignal.FromCurrentContext().Raise(exception);
                    _logger.ErrorException(errString, exception);

                }

                return Json(new { Result = "0" }, JsonRequestBehavior.AllowGet);
                //return Json(new { Result = WebUiConstants.Fail });
            }
            else
            {
                return null;
            }
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
                    _logger.Info("ACS EmailParser Posted: " + JsonConvert.SerializeObject(importedOrder, Formatting.None, new JsonSerializerSettings { MaxDepth = 1, ReferenceLoopHandling = ReferenceLoopHandling.Ignore }));
                    idRegType = Convert.ToInt32(importedOrder.RegistrationType);
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
            importedOrder.OrderDate = DateTime.Now;
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



        [AllowAnonymous]
        [AcceptVerbs(HttpVerbs.Get), ValidateInput(false)]
        public ActionResult AddBillingEmail(int idOrder)
        {

            var order = _orderManagementService.GetOrderById(idOrder);

            if (order != null)
            {
                return View(order);
                //                return Json(new { Result = "Success" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { Result = "0" }, JsonRequestBehavior.AllowGet);
            }

        }

        [AllowAnonymous]
        [AcceptVerbs(HttpVerbs.Get), ValidateInput(false)]
        public ActionResult ChangeCC(int idOrder)
        {

            var order = _orderManagementService.GetOrderById(idOrder);

            if (order != null)
            {
                return View(order);
                //                return Json(new { Result = "Success" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { Result = "0" }, JsonRequestBehavior.AllowGet);
            }

        }

        [AllowAnonymous]
        [AcceptVerbs(HttpVerbs.Post), ValidateInput(false)]
        public ActionResult AddBillingEmail(string emailToAdd, int idOrder)
        {

            var order = _orderManagementService.GetOrderById(idOrder);

            if (order != null)
            {
                _logger.Info("AddBilling Email of:" + order.BillingEmail + " to: " + order.idOrder);
                //
                return Json(new { Result = "Success. Future billing notifications will include <b>" + emailToAdd + ".</b>" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { Result = "0" }, JsonRequestBehavior.AllowGet);
            }

        }
        [AllowAnonymous]
        [AcceptVerbs(HttpVerbs.Post), ValidateInput(false)]
        public ActionResult ChangeCC(string emailToAdd, int idOrder)
        {

            var order = _orderManagementService.GetOrderById(idOrder);

            if (order != null)
            {
                _logger.Info("ChangeCC Email of:" + order.BillingEmail + " to: " + order.idOrder);
                //
                IList<string> addresses = _orderManagementService.OrderHasCc(order).Split(',');
                IList<string> ccEmailAddresses = new List<string>();

                if (addresses != null)
                {
                    //  assume user has separated email addresses with either a semi-colon or a comma, as is convention.
                    ccEmailAddresses.AddRange(addresses);
                    ccEmailAddresses.AddRange(emailToAdd.Split(','));
                }

                var newJson = new JProperty(JsonPropertyKeys.CarbonCopy, ccEmailAddresses);
                order.UserComments = JsonHelpers.MergeJsonWithStoredField(order.UserComments, newJson);

                _orderManagementService.SaveChanges();
                return Json(new { Result = "Success. Future notifications will include <b>" + emailToAdd + ".</b>" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { Result = "0" }, JsonRequestBehavior.AllowGet);
            }

        }


        [AllowAnonymous]
        [AcceptVerbs(HttpVerbs.Post), ValidateInput(false)]
        public ActionResult FindCoWorkerLink(string email, string _orderId)
        {
            if (ModelState.IsValid)
            {
                var orderId = Convert.ToInt32(_orderId.Replace("BW-", "").Replace("CU-", ""));
                var orderById = _orderManagementService.GetOrderById(orderId);
                var ordersByEmail = _orderManagementService.GetOrdersByEmail(email, 19);

                foreach (var order in ordersByEmail)
                {
                    if (order.idOrder == orderById.idOrder)
                    {
                        var row = order.OrderRows.SingleOrDefault(r => r.RowStatus == OrderRowStatus.Active);
                        if (row.Webinar.Status == WebinarStatus.Recorded)
                            return Json(new { odCode = _globalConfig.TenantURL + "/o/" + row.idOrder + "-" + row.OnDemandCode }, JsonRequestBehavior.AllowGet);
                        //else
                        return Json(new { joinCode = _globalConfig.TenantURL + "/j/" + row.TtsJoinUrl }, JsonRequestBehavior.AllowGet);
                    }
                }


                return Json(new { LinkNotFound = "Link Not Found" }, JsonRequestBehavior.AllowGet);
            }
            return null;
        }


        [AllowAnonymous]
        [AcceptVerbs(HttpVerbs.Post), ValidateInput(false)]
        public void UpdateTimeZone()
        {
            var incoming = HttpContext.Request.Form[0].TrimStart('[').TrimEnd(']');

            try
            {
                StringBuilder sb = new StringBuilder();
                _logger.Info("UpdateTimeZone Starts");
                if (incoming != null)
                {
                    var msgHtml = JsonConvert.DeserializeObject<MandrillIncomingMsg.mandrill_events>(incoming.ToString());
                    _logger.Info("UpdateTimeZone msgHtml" + msgHtml);
                    var parsedMessage = ParseMandrillMsg.ParseIncomingMessages("<html><body>" + msgHtml.msg.html + "</body></html>", DateTime.Now.ToString());
                    _logger.Info("UpdateTimeZone Parsed: " + JsonConvert.SerializeObject(parsedMessage));

                }

            }
            catch (Exception ex)
            {
                _logger.Warn("UpdateTimeZone: " + incoming + " exception: " + ex);

            }
            //return null;
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