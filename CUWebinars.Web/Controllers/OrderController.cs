using CUWebinars.Business.Services;
using CUWebinars.Web.Core;
using CUWebinars.Web.Core.Orchestrators;
using CUWebinars.Web.Helpers;
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

namespace CUWebinars.Web.Controllers
{
    public class OrderController : Controller
    {

        private readonly ILogger _logger;
        private readonly IOrderControllerOrchestrator _orderControllerOrchestrator;
        private IOrderManagementService _orderManagementService;
        private IWebinarManagementService _webinarManagementService;
        private readonly IAppHelper _appHelper;

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

                _logger.Info("Begin import: " + email);

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

                _logger.Info(string.Format("MigrateOrder|CreteNewOrder: {0}", idOfLastOrder));

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
                _logger.Info("ACS Importer hears: " + importedOrder.Email + " idWebinar: " + importedOrder.idWebinar);
                idRegType = _webinarManagementService.GetRegTypeByACS(importedOrder.RegistrationType,
                    importedOrder.idWebinar);
                //Elmah.ErrorSignal.FromCurrentContext().Raise(exception);
            }
            else
            {
                idRegType = _webinarManagementService.GetRegTypeByLableAndWebinar(importedOrder.RegistrationType,
                     importedOrder.idWebinar);
            }

            if (idRegType == 0)
            {
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

                    _logger.Info("ACS Importer heard: " + newOrder.BillingEmail);
                    _orderManagementService.SendOrderToLegacy(newOrder);
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



        private bool ParseMsgBody(string _doc, string orderDate)
        {
            HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();

            doc.LoadHtml(_doc);

            if (doc.ParseErrors != null && doc.ParseErrors.Count() > 0)
            {
                // Handle any parse errors as required

                return true;
            }
            else
            {
                if (doc.DocumentNode != null)
                {
                    MyGuid = Guid.NewGuid().ToString();
                    var auditMsg = "MyGuid=" + MyGuid;
                    Dictionary<string, string> ACSOrderDictionary = new Dictionary<string, string>
                            {
                                    {"FirstName", ""}, {"LastName", ""}, {"Company", ""}, {"Title", ""}, {"Email", ""}, {"Phone", ""}, {"Country", ""}, {"StreetorP.O.Box", ""}, {"City", ""}, {"State/Province/Region", ""}, {"Zip/PostalCode", ""}, {"DeliveryType", ""}, {"WebinarTitle", ""}, {"CourseNumber", ""}, {"WebinarDate", ""}, {"WebinarTime(EasternTime)", ""}, {"CourseDeliveryType", ""}, {"CoursePrice", ""}, {"BankWebID", ""}, {"PaymentMethod", ""}, {"CompanyBillingInformation", ""}, {"ZeroValue", ""}, {"DateSubmittedToACS", orderDate}
                            };

                    HtmlNode bodyNode = doc.DocumentNode.SelectSingleNode("//body");

                    if (bodyNode != null)
                    {
                        try
                        {
                            var _values = doc.DocumentNode.SelectNodes("//tr[@bgcolor='#FFFFFF']/td[2]");
                            var _names = doc.DocumentNode.SelectNodes("//tr[@bgcolor='#EAF2FA']/td");


                            string[] values = new string[_values.Count];
                            string[] names = new string[_values.Count];

                            for (var i = 0; i < _values.Count - 1; i++)
                            {
                                string value = _values[i].InnerText.TrimStart().TrimEnd();
                                string name = Regex.Replace(_names[i].InnerText, @"\s", string.Empty, RegexOptions.Multiline)
                                                   .TrimStart().TrimEnd();
                                //docs the error
                                if (value.Length == 0) { value = name; }

                                if (ACSOrderDictionary.ContainsKey(name))
                                {
                                    ACSOrderDictionary[name] = value;
                                }
                                else
                                {
                                    _logger.Warn("Keyname not known:" + name);
                                }
                                values[i] = value;
                                names[i] = name;
                            }
                            _logger.Info("values =" + values + " : names = " + names);

                            var nvPaired = BuildMigratorString(ACSOrderDictionary);

                        }
                        catch (Exception ex)
                        {
                            _logger.Warn("ERROR Parsing Importer Loop: " + ex);
                            return true;
                        }

                    }
                }
            }
            return false;
        }


        private string BuildMigratorString(Dictionary<string, string> dic)
        {

            String AffiliateID = "";
            String WebinarID = dic["FirstName"];
            int idRegType = _webinarManagementService.GetRegTypeByACS(dic["DeliveryType"], Convert.ToInt32(dic["BankWebID"]));
            var FirstName = dic["FirstName"];
            var LastName = dic["LastName"];
            var Title = dic["Title"];
            var Institution = dic["Company"];
            var Email = dic["Email"];
            var Phone = dic["Phone"];
            var Address = dic["StreetorP.O.Box"];
            var Address2 = "";
            var City = dic["City"];
            var State = dic["State/Province/Region"];
            var Zip = dic["Zip/PostalCode"];
            var DiscountCode = "";
            var AdditionalLocations = "";
            var shippingFirstName = dic["FirstName"];
            var shippingLastName = dic["LastName"];
            var shippingPhone = dic["Phone"];
            var shippingAddress = dic["StreetorP.O.Box"];
            var shippingCity = dic["City"];
            var shippingState = dic["State/Province/Region"];
            var shippingZip = dic["Zip/PostalCode"];
            var AffiliateComments = "ACSImporter";
            var OrderDate = dic["DateSubmittedToACS"];

            var PostForm = "";


            PostForm = "idAffiliate=" + AffiliateID + "+BillingAddress.AddressType=Billing";
            PostForm += "&BillingAddress.Name=" + HttpUtility.UrlEncode(FirstName + " " + LastName);
            PostForm += "&BillingAddress.Phone=" + HttpUtility.UrlEncode(Phone);
            PostForm += "&BillingAddress.str=" + HttpUtility.UrlEncode(Address);
            PostForm += "&BillingAddress.str2=" + HttpUtility.UrlEncode(Address2);
            PostForm += "&BillingAddress.City=" + HttpUtility.UrlEncode(City);
            PostForm += "&BillingAddress.Zip=" + HttpUtility.UrlEncode(Zip);
            PostForm += "&BillingAddress.State=" + HttpUtility.UrlEncode(State);
            PostForm += "&BillingAddress.Country=" + "US";
            PostForm += "&ShippingAddress.AddressType=Shipping";
            PostForm += "&ShippingAddress.Name=" + HttpUtility.UrlEncode(shippingFirstName + " " + shippingLastName);
            PostForm += "&ShippingAddress.Phone=" + HttpUtility.UrlEncode(shippingPhone);
            PostForm += "&ShippingAddress.str=" + HttpUtility.UrlEncode(shippingAddress);
            PostForm += "&ShippingAddress.str2=" + "";
            PostForm += "&ShippingAddress.City=" + HttpUtility.UrlEncode(shippingCity);
            PostForm += "&ShippingAddress.State=" + HttpUtility.UrlEncode(shippingState);
            PostForm += "&ShippingAddress.Zip=" + shippingZip;
            PostForm += "&ShippingAddress.Country=US";
            PostForm += "&Email=" + HttpUtility.UrlEncode(Email);
            PostForm += "&Title=" + HttpUtility.UrlEncode(Title);
            PostForm += "&Institution=" + HttpUtility.UrlEncode(Institution);
            PostForm += "&FirstName=" + HttpUtility.UrlEncode(FirstName);
            PostForm += "&LastName=" + HttpUtility.UrlEncode(LastName);
            PostForm += "&idRegType=" + idRegType;
            PostForm += "&idWebinar=" + WebinarID;
            PostForm += "&AdditionalLocationsString=" + HttpUtility.UrlEncode(AdditionalLocations);
            PostForm += "&idOrderLegacy=0";
            PostForm += "&OrderDate=" + HttpUtility.UrlEncode(OrderDate);
            PostForm += "&ShippingDate=";
            PostForm += "&DiscountCode=" + HttpUtility.UrlEncode(DiscountCode);
            PostForm += "&Status=";
            PostForm += "&Total=";

            string submitMigrateForm = string.Empty;

            Uri url = null;
            var baseUri = new Uri(string.Concat(url.Scheme, @"://", url.Authority), UriKind.Absolute);
            submitMigrateForm = new Uri(
                baseUri,
                "Order/MigrateOrder/"
                ).ToString();

            HttpWebResponse response;
            WebRequest request = WebRequest.Create(submitMigrateForm);
            request.Method = "POST";
            byte[] byteArray = Encoding.UTF8.GetBytes(PostForm);
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = byteArray.Length;

            StreamWriter sw = new StreamWriter(request.GetRequestStream());
            sw.Write(PostForm);
            
            // Execute the query
            response = (HttpWebResponse)request.GetResponse();
            StreamReader sr = new StreamReader(response.GetResponseStream());
            //return sr.ReadToEnd();

            // Close the Stream object.
            //dataStream.Close();
            Stream dataStream = request.GetRequestStream();
            // Display the status.
            Console.WriteLine(((HttpWebResponse)response).StatusDescription);
            // Get the stream containing content returned by the server.
            dataStream = response.GetResponseStream();
            // Open the stream using a StreamReader for easy access.
            StreamReader reader = new StreamReader(dataStream);
            // Read the content.
            string responseFromServer = reader.ReadToEnd();
            sw.Close();
            return responseFromServer;

            // Display the content.
            return null;


        }


        public void SubmitACSForm(Dictionary<string, string> form)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://v3.bankwebinars.com/Order/MigrateOrder");

            // Set some reasonable limits on resources used by this request
            request.MaximumAutomaticRedirections = 4;
            request.MaximumResponseHeadersLength = 4;
            // Set credentials to use for this request.
            request.Credentials = CredentialCache.DefaultCredentials;
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            Console.WriteLine("Content length is {0}", response.ContentLength);
            Console.WriteLine("Content type is {0}", response.ContentType);

            // Get the stream associated with the response.
            Stream receiveStream = response.GetResponseStream();

            // Pipes the stream to a higher level stream reader with the required encoding format. 
            StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8);

            Console.WriteLine("Response stream received.");
            Console.WriteLine(readStream.ReadToEnd());
            response.Close();
            readStream.Close();
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