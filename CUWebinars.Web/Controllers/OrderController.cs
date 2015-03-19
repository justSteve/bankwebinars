using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.RSSBus.Gmail;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using CUWebinars.Business.Models;
using CUWebinars.Business.Services;
using CUWebinars.Web.Core;
using CUWebinars.Web.Core.Orchestrators;
using CUWebinars.Web.Helpers;
using CUWebinars.Web.Models;
using HtmlAgilityPack;
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

        private readonly GmailConnection _conn = new GmailConnection(
                    ConfigurationManager.ConnectionStrings["RSSBUS_Affiliate"].ConnectionString);

        private readonly GmailConnection _connAdmin = new GmailConnection(
                    ConfigurationManager.ConnectionStrings["RSSBUS_Admin"].ConnectionString);

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

        public JsonResult ImportACS(ImportOrderModel importedOrder)
        {
            ParseEmails_ACS();

            return null;

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
                                    {"FirstName", ""}, {"LastName", ""}, {"Company", ""}, {"Title", ""}, {"Email", ""}, {"Phone", ""}, {"Country", ""}, {"StreetorP.O.Box", ""}, {"City", ""}, {"State/Province/Region", ""}, {"Zip/PostalCode", ""}, {"DeliveryType", ""}, {"WebinarTitle", ""}, {"CourseNumber", ""}, {"WebinarDate", ""}, {"WebinarTime(EasternTime)", ""}, {"CourseDeliveryType", ""}, {"CoursePrice", ""}, {"BankWebID", ""}, {"PaymentMethod", ""}, {"CompanyBillingInformation", ""}, {"ZeroValue", ""}, {"DateSubmittedToACS", ""}
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
                                if (value.Length == 0){value = name;}

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

                        var nvPaired = MapDicToMigratorOrderModel(ACSOrderDictionary);

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


        private string MapDicToMigratorOrderModel(Dictionary<string, string> dic)
        {

            String AffiliateID = "";
            String WebinarID =  dic["FirstName"];
            int idRegType = _webinarManagementService.GetRegTypeByACS(dic["DeliveryType"],Convert.ToInt32(dic["BankWebID"]));
            var FirstName = dic["FirstName"];
            var LastName = dic["LastName"];
            var Title = dic["Title"];
            var Institution = dic["Institution"];
            var Email = dic["Email"];
            var Phone = dic["Phone"];
            var Address = dic["Address"];
            var Address2 = "";
            var City = dic["City"];
            var State = dic["State"];
            var Zip = dic["Zip"];
            var DiscountCode = "";
            //var AdditionalLocations = dic["AdditionalLocations"];
            var shippingFirstName = dic["FirstName"];
            var shippingLastName = dic["LastName"];
            var shippingPhone = dic["Phone"];
            var shippingAddress = dic["Address"];
            var shippingCity = dic["City"];
            var shippingState = dic["State"];
            var shippingZip = dic["Zip"];
            var AffiliateComments = dic[""];
            var OrderDate = dic[""];
            
            return null;
            //var qString = BuildMigratorString();


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

        //public virtual int ParseMsgBody_(string msgBody, string orderDate)
        //{
        //    var doc = new HtmlAgilityPack.HtmlDocument();
        //    doc.LoadHtml(msgBody);

        //    var root = doc.DocumentNode;

        //    var keyRows = root.SelectNodes("//*//tr[contains(@bgcolor, 'FFFFFF')]/td");
        //    var valueRows = root.SelectNodes("//*//tr[contains(@bgcolor, 'EAF2FA')]/td");


        //    //var keyRows = root.SelectNodes("//*[table]/child::table[1]/tbody[1]/tr/td/table/tbody/tr[@bgcolor='#EAF2FA']/td");
        //    //var valueRows = root.SelectNodes("//*[table]/child::table[1]/tbody[1]/tr/td/table/tbody/tr[@bgcolor='#FFFFFF']/td");

        //    var keys = keyRows.Select(row => row.InnerText.Trim()).ToList();

        //    var values = (from row in valueRows where !row.InnerText.Trim().Equals("&nbsp;") select row.InnerText.Trim()).ToList();

        //    var kvs = keys.Zip(values, (k, v) => new KeyValuePair<string, string>(k, v));
        //    var dic = kvs.ToDictionary(b => b.Key);

        //    var sb = new StringBuilder();
        //    foreach (var keyValuePair in kvs)
        //    {
        //        sb.Append(string.Format("{0} : {1}{2}", keyValuePair.Key, keyValuePair.Value, Environment.NewLine));
        //        Console.WriteLine("Key: {0}, Value: {1}", keyValuePair.Key, keyValuePair.Value);
        //    }


        //    using (var sw = new StreamWriter("OrderMigrator.csv", true))
        //    {
        //        sw.AutoFlush = true;

        //        sw.Write("AffiliateID,WebinarID,idRegType,FirstName,LastName,Title,Institution,Email,Phone,Address,Address2,	City,State,Zip,DiscountCode,AdditionalLocations,firstname,lastname,phone,address,city,St,Zip,total," +
        //            "shipmentDate,StoreComments,idOrder,orderDate,status{0}", Environment.NewLine
        //            );
        //    }

        //    Console.ReadKey();
        //    return 0;
        //}

        public void RestoreTTSOrders()
        {

            var gda = new GmailDataAdapter
            {
                SelectCommand = new GmailCommand(
                    //"SELECT * FROM sys_tables", 
                    "SELECT * from [Gmail/Sent Mail] where id = '224227,224245,224248,224251,224307,224321,224324'",//MaxItems = 0 AND Date > '3-1-2015' AND Date < '3-6-2015'",// and SUBJECT NOT LIKE '%password%'",
                    //"SELECT * from [Gmail/Sent Mail] where MaxItems = 0 AND Date > '3-1-2015' AND Date < '3-6-2015'",// and SUBJECT NOT LIKE '%password%'",
                    // LIKE "Test" AND ([From] = test1@email.com OR [From] = test2@email.com) AND Date > '1-1-2012'
                    _connAdmin)
            };
            var AreErrors = "";
            var MyGuid = "";
            var msgThreadID = "";
            var msgDate = "";

            var reader = gda.SelectCommand.ExecuteReader();
            while (reader.Read())
            {
                int readerID = Convert.ToInt32(reader["id"]);
                var gda1 = new GmailDataAdapter()
                    {

                        SelectCommand = new GmailCommand(
                            "SELECT * from [Gmail/Sent Mail] where id = " + readerID,
                            _conn)
                    };
                var reader1 = gda1.SelectCommand.ExecuteReader();
                var sb = new StringBuilder();
                while (reader1.Read())
                {
                    {

                        using (var sw = new StreamWriter("C:\\Users\\Steve\\OneDrive\\Code\\ttsOrders.csv", true))
                        {

                            sw.AutoFlush = true;

                            while (reader.Read())
                            {
                                if (reader[2].ToString().Contains("Confirmation of Order"))
                                {
                                    sw.WriteLine(reader[0] + "," + reader[2].ToString().Replace(" for ", "\t") + "," +
                                                 reader[3] + "," + reader[4]);
                                }
                            }
                        }
                    }
                }
            }
        }

        public virtual int ParseEmails_ACS()
        {
            var gda = new GmailDataAdapter
            {
                SelectCommand = new GmailCommand(
                    //"SELECT * FROM sys_tables", 
                    "SELECT id from [Gmail/All Mail] where Date > '1-1-2015' and SEARCHCRITERIA = 'SUBJECT \"Webinar Registration\"'",
                    //AND ([From] = test1@email.com OR [From] = test2@email.com) AND Date > '1-1-2012'
                    //"SELECT id from MailMessages where SEARCHCRITERIA " +
                    //"= 'UNSEEN SUBJECT \"Webinar Registration\"' ",
                    _conn)
            };
            var AreErrors = "";
            var MyGuid = "";
            var msgThreadID = "";
            var msgDate = "";
            try
            {
                var reader = gda.SelectCommand.ExecuteReader();

                while (reader.Read())
                {
                    AreErrors = "";
                    //
                    int readerID = Convert.ToInt32(reader["id"]);

                    if (readerID < 1)
                    {
                        break;
                    }
                    var gda1 = new GmailDataAdapter()
                    {

                        SelectCommand = new GmailCommand(
                            "SELECT * from [Gmail/All Mail] where id = " + readerID,
                            _conn)
                    };
                    var reader1 = gda1.SelectCommand.ExecuteReader();
                    var sb = new StringBuilder();
                    while (reader1.Read())
                    {
                        {
                            Console.WriteLine("=================================================");
                            for (int i = 0; i < reader1.FieldCount; i++)
                            {
                                Console.WriteLine(reader1.GetName(i) + ": " + reader1.GetValue(i));
                                sb.Append(reader1.GetName(i) + ": " + reader1.GetValue(i) + Environment.NewLine);
                            }
                        }

                        ParseMsgBody("<html><body>" + reader1["MessageBody"].ToString() + "</body></html>", reader1["Date"].ToString());
                    }
                }
            }
            catch
            {
                throw;
            }
            return 0;
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