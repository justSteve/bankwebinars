using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Security;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using CUWebinars.Business.Core;
using CUWebinars.Business.Core.Helpers;
using CUWebinars.Business.Models;
using CUWebinars.Business.Notification;
using CUWebinars.Business.Services;
using CUWebinars.Web.Core;
using CUWebinars.Web.Models;
using CUWebinars.Web.Models.Importers;
using CUWebinars.Web.Services;
using GemBox.Document;
using HtmlAgilityPack;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Queue;
using NameParser;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Ninject.Extensions.Logging;
using PhoneNumbers;
using Trace = System.Diagnostics.Trace;

namespace CUWebinars.Web.Helpers
{
    public class AppHelper : IAppHelper
    {
        private readonly IStateService _stateService;
        private readonly ILogger _logger;
        private readonly HttpRequestBase _request;
        //public readonly TtsConfigHelper _globalConfig;

        private readonly GlobalConfig _globalConfig = GlobalConfig.GlobalConfigSingleton;



        public AppHelper(HttpRequestBase request, IStateService stateService, ILogger logger)
        {
            _request = request;
            _stateService = stateService;
            _logger = logger;
            //_globalConfig = new TtsConfigHelper();
        }

        public static IEnumerable<int> StringToIntList(string str)
        {
            if (String.IsNullOrEmpty(str))
                yield break;

            foreach (var s in str.Split(','))
            {
                int num;
                if (int.TryParse(s, out num))
                    yield return num;
            }
        }

        /// <summary>
        /// Retrieves the city state zip via URL parameter and stored in the ASP.NET Session
        /// </summary>
        /// <returns>City | State based on input zipcode</returns>
        public string GetCityStateFromZip(int zipCode)
        {
            string cityStateFromZip = string.Empty;

            using (var loggerConnection =
                    new SqlConnection(ConfigurationManager.ConnectionStrings["LoggerConnection"].ConnectionString))
            {
                loggerConnection.Open();

                var body =
                    "select City, (select stateAbbreviation from States where StateCode = " +
                    "               (SELECT StateCode FROM [dbo].zipcodes WHERE zipcode = " + zipCode + "))," +
                    "               (SELECT timezone FROM [dbo].[TimeZoneByZip] WHERE zip = '" + zipCode +
                    "') from [dbo].zipcodes where zipcode = " + zipCode;

                using (var cmdGetBody = new SqlCommand(body, loggerConnection))
                {
                    cmdGetBody.CommandType = CommandType.Text;
                    // execute the command
                    SqlDataReader msgReader = cmdGetBody.ExecuteReader();

                    while (msgReader.Read())
                    {
                        var city = "";
                        if (!msgReader.IsDBNull(0))
                        {
                            city = msgReader.GetString(0);
                        }
                        var state = "";
                        if (!msgReader.IsDBNull(1))
                        {
                            state = msgReader.GetString(1);
                        }
                        var timezone = "";
                        if (!msgReader.IsDBNull(2))
                        {
                            timezone = msgReader.GetString(2);
                        }
                        else
                        {
                            timezone = "-6";
                        }
                        cityStateFromZip = string.Concat(city, ",", state, ",", timezone);
                    }

                    return cityStateFromZip;
                }
            }
        }

        SelectList IAppHelper.GetListOfAffiliates(int selectedValue)
        {
            return GetListOfAffiliates(selectedValue);
        }


        public static USTimeZone ComputeTimeZone(string offset)
        {
            var timeZone = USTimeZone.Central;
            switch (offset)
            {
                case "-4":
                    timeZone = USTimeZone.Caribbean;
                    break;
                case "-5":
                    timeZone = USTimeZone.Eastern;
                    break;
                case "-6":
                    timeZone = USTimeZone.Central;
                    break;
                case "-7":
                    timeZone = USTimeZone.Mountain;
                    break;
                case "-8":
                    timeZone = USTimeZone.Pacific;
                    break;
                case "-9":
                    timeZone = USTimeZone.Alaska;
                    break;
                case "-10":
                    timeZone = USTimeZone.Hawaii;
                    break;
            }
            return timeZone;
        }

        public static IEnumerable<char> CharsToTitleCase(string s)
        {
            s = s.ToLower();
            bool newWord = true;
            foreach (char c in s)
            {
                if (newWord)
                {
                    yield return Char.ToUpper(c);
                    newWord = false;
                }
                else yield return Char.ToLower(c);
                if (c == ' ') newWord = true;
            }
        }

        /// <summary>
        /// Retrieves the City State info found via FDI Certification code
        /// </summary>
        /// <returns>Affiliate or null</returns>
        public static string GetCityStateFromCert(int Cert)
        {

            var _connSproc = new SqlConnection(ConfigurationManager.ConnectionStrings["Institutions"].ConnectionString);

            _connSproc.Open();

            SqlCommand cmdGetBody = new SqlCommand(
                "select City, (select stateAbbreviation from States where StateName " +
                "= (select STNAME from Institutions where Cert = "
                + Cert + ")), ZIP, ADDRESS, NAME  from Institutions where Cert = " + Cert, _connSproc);


            cmdGetBody.CommandType = CommandType.Text;

            // execute the command
            SqlDataReader msgReader = cmdGetBody.ExecuteReader();

            var value = "";

            while (msgReader.Read())
            {
                value = msgReader.FieldCount > 0 ? msgReader[0].ToString() + "|" + msgReader[1].ToString() + "|" + msgReader[2].ToString() + "|" + msgReader[3].ToString() + "|" + msgReader[4].ToString() : null;
            }
            return value;
        }
        public SessionStartInfo GetSessionStartInfo()
        {

            HttpRequest request = HttpContext.Current.Request;

            var info = new SessionStartInfo
            {
                RemoteAddress = _request.ServerVariables["REMOTE_ADDR"],
                RemoteHost = _request.ServerVariables["REMOTE_HOST"],
                RemoteUser = _request.ServerVariables["REMOTE_USER"],
                UserAgent = _request.ServerVariables["HTTP_USER_AGENT"],
                UserCookie = _request.ServerVariables["HTTP_COOKIE"],
                FirstPage = _stateService.GetValue<string>("FirstPage"),
                Elmah = _stateService.GetValue<string>("Elmah"),
                SessionRoot = _stateService.GetValue<string>("SessionRoot"),
                SessionID = _stateService.GetValue<string>(WebUiConstants.SessionId),
                AffiliateSessionSource = _stateService.GetValue<string>("AffiliateSessionSource")
            };

            return info;
        }

        public static SelectList GetListOfAffiliates(int selectedValue)
        {
            IList<Affiliate> affiliates = null;//_aff
            IDictionary<int, string> affiliatesDictionary = new Dictionary<int, string> { };

            foreach (var affiliate in affiliates)
            {
                affiliatesDictionary.Add(affiliate.idUserAff, affiliate.ttsDomain);
            }

            return new SelectList(affiliatesDictionary, "Key", "Value", (int)selectedValue);
        }


        public AuditInfoModel GetUserAuditInfo2()
        {
            var model = new AuditInfoModel();
            model.FirstPage = SecurityElement.Escape(_stateService.GetValue<string>("FirstPage"));
            model.AffiliateSessionSource = SecurityElement.Escape(_stateService.GetValue<string>("AffiliateSessionSource"));
            model.RemoteAddress = SecurityElement.Escape(_request.ServerVariables["REMOTE_ADDR"]);
            model.RemoteHost = SecurityElement.Escape(_request.ServerVariables["REMOTE_HOST"]);
            model.RemoteUser = SecurityElement.Escape(_request.ServerVariables["REMOTE_USER"]);
            model.UserAgent = SecurityElement.Escape(_request.ServerVariables["HTTP_USER_AGENT"]);
            model.Cookie = SecurityElement.Escape(_request.ServerVariables["HTTP_COOKIE"]);
            model.Elmah = SecurityElement.Escape(_stateService.GetValue<string>("Elmah"));
            model.SessionRoot = SecurityElement.Escape(_stateService.GetValue<string>("SessonRoot"));
            model.SessionID = SecurityElement.Escape(_stateService.GetValue<string>("SessionID"));
            model.SessionStart = SecurityElement.Escape(JsonConvert.SerializeObject(GetSessionStartInfo()));

            return model;
        }

        public string GetUserAuditInfo()
        {
            IDictionary<string, string> auditInfoDictionary = new Dictionary<string, string>();

            auditInfoDictionary.Add("FirstPage", SecurityElement.Escape(_stateService.GetValue<string>("FirstPage")));
            auditInfoDictionary.Add("RemoteAddress", SecurityElement.Escape(_request.ServerVariables["REMOTE_ADDR"]));
            auditInfoDictionary.Add("RemoteHost", SecurityElement.Escape(_request.ServerVariables["REMOTE_HOST"]));
            auditInfoDictionary.Add("RemoteUser", SecurityElement.Escape(_request.ServerVariables["REMOTE_USER"]));
            auditInfoDictionary.Add("UserAgent", SecurityElement.Escape(_request.ServerVariables["HTTP_USER_AGENT"]));
            auditInfoDictionary.Add("Cookie", SecurityElement.Escape(_request.ServerVariables["HTTP_COOKIE"]));
            auditInfoDictionary.Add("Elmah", SecurityElement.Escape(_stateService.GetValue<string>("Elmah")));
            auditInfoDictionary.Add("SessionRoot", SecurityElement.Escape(_stateService.GetValue<string>("SessonRoot")));
            auditInfoDictionary.Add("SessionID", SecurityElement.Escape(_stateService.GetValue<string>("SessionID")));
            auditInfoDictionary.Add("AffiliateSessionSource", SecurityElement.Escape(_stateService.GetValue<string>("AffiliateSessionSource")));
            auditInfoDictionary.Add("SessionStart", JsonConvert.SerializeObject(GetSessionStartInfo()));



            var jobject = JsonHelpers.CreateJsonObjectFromDictionary(auditInfoDictionary);

            //var jobject = new JObject();
            //jobject.Add("AuditInfo", auditObjectInner);

            //Trace.TraceInformation(jobject.ToString(Newtonsoft.Json.Formatting.None));

            return jobject.ToString(Newtonsoft.Json.Formatting.Indented);
        }


        public List<string> InstitutionAutoComplete(string name, string zip)
        {
            var _connSproc = new SqlConnection(ConfigurationManager.ConnectionStrings["LoggerConnection"].ConnectionString);
            _connSproc.Open();
            SqlCommand cmdGetBody = new SqlCommand(
                "select top 15 Name +'|'+ city + ', ' + STNAME from Institution where NAME like '" + name + "%' " +
                " order by name", _connSproc);

            cmdGetBody.CommandType = CommandType.Text;
            // execute the command
            SqlDataReader msgReader = cmdGetBody.ExecuteReader();

            List<string> value = new List<string>();

            while (msgReader.Read())
            {
                if (msgReader.HasRows)
                {
                    value.Add(msgReader[0].ToString());
                }
            }
            return value;
        }

        public string GetAffiliateName(int idAffiliate)
        {
            using (var _connSproc = new SqlConnection(GlobalConfig.GlobalConfigSingleton.DefaultConnectionString))
            {
                _connSproc.Open();

                using (SqlCommand cmdGetBody =
                    new SqlCommand("SELECT ttsDomain FROM dbo.Affiliate WHERE idUserAff = " + idAffiliate,
                        _connSproc))
                {
                    cmdGetBody.CommandType = CommandType.Text;
                    // execute the command
                    using (SqlDataReader msgReader = cmdGetBody.ExecuteReader())
                    {
                        var affiliateName = string.Empty;

                        while (msgReader.Read())
                        {
                            if (msgReader.HasRows)
                            {
                                affiliateName = msgReader.GetString(0);
                            }
                        }
                        return affiliateName;
                    }
                }
            }
        }

        public IList<string> ServerSideEmailCheck(IList<string> emails)
        {
            foreach (var email in emails)
            {
                try
                {
                    var addr = new System.Net.Mail.MailAddress(email);
                    //return addr.Address == email;
                }
                catch
                {
                    emails.Remove(email);

                }
            }
            return emails;
        }

        public IList<AdditionalLocation> CheckAdditionalLocationsForValidEmail(IList<AdditionalLocation> additionalLocations)
        {
            //
            IList<AdditionalLocation> areValid = new List<AdditionalLocation>();
            if (additionalLocations != null)
                foreach (var check in additionalLocations)
                {
                    try
                    {
                        var addr = new System.Net.Mail.MailAddress(check.Email);
                        areValid.Add(check);
                    }
                    catch (Exception ex)
                    {

                        var placeholder = "";
                    }
                }
            return areValid;
        }

        public bool CheckIsEmailValid(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return true;
            }
            catch
            {
                return false;
            }

        }
        //private long ToUnixTimespan(DateTime date)
        //{
        //    TimeZoneInfo tzInfo = TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time");
        //    var convertedTimeToUtc = TimeZoneInfo.ConvertTimeToUtc(date, tzInfo);

        //    TimeSpan tspan = convertedTimeToUtc.Subtract(new DateTime(1970, 1, 1, 0, 0, 0));
        //    return (long)Math.Truncate(tspan.TotalSeconds);
        //}

        public long ToUnixTimespan(DateTime myEventStart, TimeZoneInfo findSystemTimeZoneById)
        {
            TimeZoneInfo tzInfo = TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time");
            var convertedTimeToUtc = TimeZoneInfo.ConvertTimeToUtc(myEventStart, tzInfo);

            TimeSpan tspan = convertedTimeToUtc.Subtract(new DateTime(1970, 1, 1, 0, 0, 0));
            return (long)Math.Truncate(tspan.TotalSeconds);

        }

        public bool BuildWebinarOrdersInvoiceRow(Order order, StringBuilder discountNotes, out OrderRow row, out string _price,
            ref int rowNumber, out string _percent, ref int totalNumberDiscounts)
        {
            row = order.OrderRows.FirstOrDefault(r => r.RowStatus == OrderRowStatus.Active);

            _price = row.RowPrice.ToString("C").Replace(".00", "");
            rowNumber++;

            _percent = (row.PercentPaid * 100).ToString().Replace(".00", "").Replace(".0", "") +
                       "%";

            //var totalToShow = order.Total.ToString("c");
            if (row.Discount != null)
            {
                var discount = row.Discount;
                _price = "See Note #" + totalNumberDiscounts;

                if (discount.DiscountType == DiscountType.Subscription &&
                    discount.DateValidFrom != discount.DateValidTo)
                {
                    // change request that we skip any Unlimited subs.
                    return true;
                }
                else
                {
                    discountNotes.Append(totalNumberDiscounts + " " + discount.DiscountCode +
                                         ": (" +
                                         discount.DiscountType.ToString()
                                             .Replace("DiscountType.", "") +
                                         ") ");
                }


                totalNumberDiscounts++;
                var discountAmount = "";
                if (row.Discount.PercentOff > 0)
                {
                    discountAmount =
                        (row.RowPrice * (row.Discount.PercentOff / 100)).ToString("c");
                }
                if (row.Discount.FlatOff > 0)
                {
                    discountAmount = (row.RowPrice - row.Discount.FlatOff).ToString("C");
                }
                //discountNotes.Append(discountAmount);
                discountNotes.Append(Environment.NewLine);
            }
            return false;
        }

        public bool BuildAdjustedOrderInvoiceRow(Order order, string adjustmentDirection, OrderRow row, Dictionary<string, JToken> dict,
            StringBuilder discountNotes, out decimal adjustedTotal, out decimal adjustedRoyalty, ref int totalNumberDiscounts)
        {
            if (order.InvoiceDetail.Contains("Royalty is decreased"))
            {
                adjustmentDirection = "Royalty is decreased";
            }

            adjustedTotal = row.RowPrice - (decimal)dict["OriginalTotal"];
            adjustedRoyalty = (decimal)dict[adjustmentDirection];

            if (row.Discount != null)
            {
                var discount = row.Discount;

                if (discount.DiscountType == DiscountType.Subscription &&
                    discount.DateValidFrom != discount.DateValidTo)
                {
                    //skip unlimited subs
                    return true;
                }
                else
                {
                    discountNotes.Append(totalNumberDiscounts + " " + discount.DiscountCode +
                                         ": (" +
                                         discount.DiscountType.ToString()
                                             .Replace("DiscountType.", "") +
                                         ") ");
                }

                //_price = "See Note #" + totalNumberDiscounts;
                discountNotes.Append(totalNumberDiscounts + " " + discount.DiscountCode + ": (" +
                                     discount.DiscountType.ToString()
                                         .Replace("DiscountType.", "") +
                                     ") ");

                totalNumberDiscounts++;
                var discountAmount = "";
                if (row.Discount.PercentOff > 0)
                {
                    discountAmount = (row.UnitPrice * (row.Discount.PercentOff / 100)).ToString("c");
                }
                if (row.Discount.FlatOff > 0)
                {
                    discountAmount = (row.UnitPrice - row.Discount.FlatOff).ToString("C");
                }
                discountNotes.Append(Environment.NewLine);
            }
            return false;
        }

        public void Document_FieldMerging(DocumentModel document, Affiliate affiliate)
        {
            document.MailMerge.FieldMerging += (sender, e) =>
            {
                if (affiliate.BillingModel == "aff")
                {
                    if (e.Inline != null && e.FieldName == "TotalRevenueBilledLabel")
                        ((Run)e.Inline).Text = "";
                    if (e.Inline != null && e.FieldName == "TotalRevenuePaidLabel")
                        ((Run)e.Inline).Text = "";
                    if (e.Inline != null && e.FieldName == "TotalNetDueLabel")
                        ((Run)e.Inline).Text = "";
                    if (e.Inline != null && e.FieldName == "TotalAdjustedRevenueBilledLabel")
                        ((Run)e.Inline).Text = "";
                    if (e.Inline != null && e.FieldName == "TotalAdjustedRevenuePaidLabel")
                        ((Run)e.Inline).Text = "";
                    if (e.Inline != null && e.FieldName == "TotalAdjustedNetDueLabel")
                        ((Run)e.Inline).Text = "";
                    if (e.Inline != null && e.FieldName == "TotalOnBilled")
                        ((Run)e.Inline).Text = "";
                    if (e.Inline != null && e.FieldName == "TotalOnPaid")
                        ((Run)e.Inline).Text = "";
                    if (e.Inline != null && e.FieldName == "TotalNetDue")
                        ((Run)e.Inline).Text = "";
                    if (e.Inline != null && e.FieldName == "GrandTotalOnBilled")
                        ((Run)e.Inline).Text = "";
                    if (e.Inline != null && e.FieldName == "GrandTotalOnPaid")
                        ((Run)e.Inline).Text = "";
                    if (e.Inline != null && e.FieldName == "GrandTotalNetDue")
                        ((Run)e.Inline).Text = "";
                    if (e.Inline != null && e.FieldName == "GrandTotalRevenueBilledLabel")
                        ((Run)e.Inline).Text = "";
                    if (e.Inline != null && e.FieldName == "GrandTotalRevenuePaidLabel")
                        ((Run)e.Inline).Text = "";
                    if (e.Inline != null && e.FieldName == "GrandTotalNetDueLabel")
                        ((Run)e.Inline).Text = "";
                }

                if (e.IsValueFound)
                {
                    switch (e.FieldName)
                    {
                        case "Date":
                            ((Run)e.Inline).Text = ((DateTime)e.Value).ToString("dddd, MMMM d, yyyy");
                            break;

                        case "Percent":
                        case "Royalty":
                            break;
                    }
                }
            };

        }

        public bool BuildPostEventOrdersInvoiceRows(Order order, StringBuilder discountNotes, out OrderRow row,
            out string _price, ref int rowNumber, out string _percent, ref int totalNumberDiscounts)
        {
            row = order.OrderRows.FirstOrDefault(r => r.RowStatus == OrderRowStatus.Active);
            _price = row.RowPrice.ToString("C").Replace(".00", "");
            rowNumber++;
            _percent = (row.PercentPaid * 100).ToString().Replace(".00", "").Replace(".0", "") +
                       "%";
            if (row.Discount != null)
            {
                var discount = row.Discount;

                if (discount.DiscountType == DiscountType.Subscription &&
                    discount.DateValidFrom != discount.DateValidTo)
                {
                    //skip unlimited subs
                    return true;
                }
                else
                {
                    discountNotes.Append(totalNumberDiscounts + " " + discount.DiscountCode +
                                         ": (" +
                                         discount.DiscountType.ToString()
                                             .Replace("DiscountType.", "") +
                                         ") ");
                }

                _price = "See Note #" + totalNumberDiscounts;
                discountNotes.Append(totalNumberDiscounts + " " + discount.DiscountCode + ": (" +
                                     discount.DiscountType.ToString()
                                         .Replace("DiscountType.", "") +
                                     ") ");

                totalNumberDiscounts++;
                var discountAmount = "";
                if (row.Discount.PercentOff > 0)
                {
                    discountAmount = (row.UnitPrice * (row.Discount.PercentOff / 100)).ToString("c");
                }
                if (row.Discount.FlatOff > 0)
                {
                    discountAmount = (row.UnitPrice - row.Discount.FlatOff).ToString("C");
                }

                discountNotes.Append(Environment.NewLine);
            }
            return false;
        }


        public static string DisplayUserTimeZone(Order order)
        {
            OrderRow row = order.OrderRows.SingleOrDefault(r => r.RowStatus == OrderRowStatus.Active);
            if (row == null) throw new NullReferenceException();

            var hoursAdjust = row.Webinar.Date;

            switch (order.WebUser.timeZone)
            {
                case USTimeZone.Pacific:

                    return hoursAdjust.AddHours(-2).ToShortTimeString() + " " + order.WebUser.timeZone.ToString();
                    break;
                case USTimeZone.Mountain:

                    return hoursAdjust.AddHours(-1).ToShortTimeString() + " " + order.WebUser.timeZone.ToString();
                    break;
                case USTimeZone.Central:

                    return hoursAdjust.ToShortTimeString() + " " + order.WebUser.timeZone.ToString();
                    break;
                case USTimeZone.Eastern:

                    return hoursAdjust.AddHours(1).ToShortTimeString() + " " + order.WebUser.timeZone.ToString();
                    break;
                case USTimeZone.Alaska:

                    return hoursAdjust.AddHours(-3).ToShortTimeString() + " " + order.WebUser.timeZone.ToString();
                    break;
                case USTimeZone.Hawaii:

                    return hoursAdjust.AddHours(-5).ToShortTimeString() + " " + order.WebUser.timeZone.ToString();
                    break;

                case USTimeZone.Caribbean:

                    return hoursAdjust.AddHours(2).ToShortTimeString() + " " + order.WebUser.timeZone.ToString();
                    break;
            }
            return "";
        }

        public static string DisplayUserDate(Order order)
        {
            OrderRow row = order.OrderRows.SingleOrDefault(r => r.RowStatus == OrderRowStatus.Active);
            if (row == null) throw new NullReferenceException();
            return DateTimeHelper.FormatDate(row.Webinar.Date) + " " + DateTimeHelper.FormatTimeWithDuration(row.Webinar.Date, order.WebUser.timeZone, true, row.Webinar.Duration);

        }

        public string CleanHtmlCodesAndLogo(string body
            , string tenantLogo
            , string addloccost
            , string openingMessage = null)
        {
            body = body.Replace("&gt;", ">");
            body = body.Replace("&lt;", "<");
            body = body.Replace("[logo]", "<img src=" + tenantLogo + " />");
            if (!string.IsNullOrEmpty(addloccost))
                body = body.Replace("[addloccost]", addloccost);
            if (!string.IsNullOrEmpty(openingMessage))
            {
                body = body.Replace("[OpeningMessage]", openingMessage);
            }
            else
            {
                body = body.Replace("[OpeningMessage]", "");
            }
            return body;
        }


        public NotificationMessageFields BuildNotiFields(Order order, string hasCCAddress)
        {

            var fields = new NotificationMessageFields();
            OrderRow row = order.OrderRows.SingleOrDefault(r => r.RowStatus == OrderRowStatus.Active);

            var fileBuilder = "Materials Coming Soon";
            if (row.Webinar.WebinarFiles.Count > 0)
                fileBuilder = "";
            foreach (var i in row.Webinar.WebinarFiles)
            {
                fileBuilder += "<a href='http://ttsmedia.ttstrain.com/" + i.fileLocation + "'>" + i.fileDesc + "</a><br>";
            }
            string TenantURL = _globalConfig.TenantURL;
            fields.SupportEmail = _globalConfig.TenantEmail;
            string Tenant = _globalConfig.Tenant;

            fields.AffFooter = "This webinar brought to you by TTS & " + _globalConfig.Tenant;
            if (order.idAffiliate != 2988 && order.idAffiliate != 19 && order.idAffiliate != 41382)
                fields.AffFooter = "This webinar brought to you by " + order.Affiliate.DisplayTitle + " & " + _globalConfig.Tenant;

            if (_globalConfig.Tenant == "CUWebinars" && order.idAffiliate != 2988 && order.idAffiliate != 62 && order.idAffiliate != 19 && order.idAffiliate != 41382)
                fields.AffFooter = "This webinar brought to you by " + order.Affiliate.DisplayTitle + " & " + _globalConfig.Tenant;


            fields.OndemandLink = " <a href='" + TenantURL + "/o/" + order.idOrder + "-" + row.OnDemandCode + "'>" + TenantURL + "/o/" + order.idOrder + "-" + row.OnDemandCode + "</a>";
            fields.UpdateOrderPage = " <a href='" + TenantURL + "/resume/" + order.idOrder + "'>Update Order</a> page.";
            fields.ConfirmAccountLink = " <a href='" + TenantURL + "/acc/apwd/" + order.idOrder + "'>" + TenantURL + "/acc/apwd/" + order.idOrder + "</a>";
            fields.DetailedConnectionInfoLink = " <a href='" + TenantURL + "/Home/DetailedConnectionInstructions'>" + TenantURL + "/Home/DetailedConnectionInstructions</a>";
            fields.CertificateLink = " <a href='" + TenantURL + "Account/MyCertificate?orderID=" + order.idOrder + "'>" + TenantURL + "/Account/MyCertificate?orderID=" + order.idOrder + "</a>";
            fields.LinkToMyWebinars = " <a href='" + TenantURL + "/MyWebinars?idOrder=" + order.idOrder + "'>" + TenantURL + "/MyWebinars?idOrder=" + order.idOrder + "</a>";
            fields.ChangeTimeZoneLink = " <a href='" + TenantURL + "/Account/EditUser/" + order.idUser + "'>" + " click to change timezone." + "</a>";
            fields.TenantSignature = "The " + Tenant + " Staff";

            fields.AttendType = row.RegistrationType.OptionLabelShort;
            fields.ServiceNote = "<b>Service Update!</b></br> Please update your whitelist to include whatever@ttstregistrations.com. <br>To ensure your receipt of future notifications please send a blank message to whatever@ttsregistrations.com";
            fields.ServiceNote = "";
            fields.OrderID = row.idOrder;
            fields.Institution = order.Institution;
            fields.BillingEmail = order.BillingEmail;
            fields.TenantName = _globalConfig.Tenant;
            fields.TenantURL = _globalConfig.TenantURL;
            fields.WebinarTitle = row.Webinar.Title;
            fields.Duration = row.Webinar.Duration.ToString().Replace(".00", "");

            fields.Total = order.Total.ToString("C0");
            fields.FirstName = order.FirstName;
            fields.LastName = order.LastName;
            fields.ShowTimeZone = order.WebUser.timeZone.ToString();

            var hoursAdjust = row.Webinar.Date;

            switch (order.WebUser.timeZone)
            {
                case USTimeZone.Pacific:

                    fields.ShowStartTime = hoursAdjust.AddHours(-2).ToShortTimeString();
                    break;
                case USTimeZone.Mountain:

                    fields.ShowStartTime = hoursAdjust.AddHours(-1).ToShortTimeString();
                    break;
                case USTimeZone.Central:

                    fields.ShowStartTime = hoursAdjust.ToShortTimeString();
                    break;
                case USTimeZone.Eastern:

                    fields.ShowStartTime = hoursAdjust.AddHours(1).ToShortTimeString();
                    break;
                case USTimeZone.Alaska:

                    fields.ShowStartTime = hoursAdjust.AddHours(-3).ToShortTimeString();
                    break;
                case USTimeZone.Hawaii:

                    fields.ShowStartTime = hoursAdjust.AddHours(-5).ToShortTimeString();
                    break;

                case USTimeZone.Caribbean:

                    fields.ShowStartTime = hoursAdjust.AddHours(2).ToShortTimeString();
                    break;


            }
            fields.DisplayDate = DateTimeHelper.FormatDate(row.Webinar.Date) + " " + DateTimeHelper.FormatTimeWithDuration(row.Webinar.Date, order.WebUser.timeZone, true, row.Webinar.Duration);

            fields.Phone = row.Webinar.AccessPhone;
            fields.AccessCode = row.Webinar.AccessCodeAttendee;
            fields.PresenterMaterials = fileBuilder;
            fields.ClickToJoinAddLocLink = "<a href='" + row.Webinar.CitrixRegisterUrl + "'>" +
                                     row.Webinar.CitrixRegisterUrl + "</a>";
            fields.ClickToJoinLink = "<a href='" + TenantURL + "/j/" + row.TtsJoinUrl + "'>" +
                                     TenantURL + "/j/" + row.TtsJoinUrl + "</a>";
            fields.AddReminder = " <a href='" + TenantURL + "/Webinar/ICalOrder?icsOrder=" + order.idOrder + "'>" +
                                 "Add to Outlook Calendar</a>";
            //fields.CCCaption =
            //    " Connection info is not currently shared. <a href='" + TenantURL + "/Resume/" + order.idOrder + "'>" +
            //                                 " (change?) </a> ";
            fields.CCCaption = "";

            if (hasCCAddress != null && hasCCAddress != "")
            {
                var _listOfCCs = hasCCAddress.Split(',');
                var listOfCCs = "";
                foreach (var cc in _listOfCCs)
                {
                    listOfCCs += cc;
                }
                fields.CCCaption =
                    " Connection info is shared with " + listOfCCs.TrimEnd(',') + ". ";
            }
            fields.PaymentStatus = order.OrderStatus.ToString();

            if (order.OrderStatus == OrderStatus.Paid ||
                order.idAffiliate != 62
                || order.idAffiliate != 375
                || order.idAffiliate != 376
                || order.idAffiliate != 379
                || order.idAffiliate != 380
                || order.idAffiliate != 383
                || order.idAffiliate != 384
                || order.idAffiliate != 385
                || order.idAffiliate != 386
                || order.idAffiliate != 387
                || order.idAffiliate != 394
                || order.idAffiliate != 395
                || order.idAffiliate != 396
                || order.idAffiliate != 963
                || order.idAffiliate != 2986
                || order.idAffiliate != 11464
                || order.idAffiliate != 12014
                || order.idAffiliate != 16132
                || order.idAffiliate != 22805
                || order.idAffiliate != 31267
                )
            {
                fields.PaymentCaption = "";
            }
            else
            {
                if (row.Webinar.Status == WebinarStatus.Recorded)
                {
                    fields.PaymentCaption =
                        "We will be sending an invoice to " + order.BillingEmail +
                        ". If you wish to pay by credit card <a href='" + TenantURL + "/Resume/" + order.idOrder +
                        "'> click here.</a>" +
                        " Is someone else in your organization responsible for payments? <a href='" +
                        TenantURL + "/Order/AddBillingEmail?idOrder=" + order.idOrder + "'>" +
                        "Enter their email here</a> and we will send the required information directly.";
                }
                else
                {
                    fields.PaymentCaption =
                        "We'll be sending an invoice to " + order.BillingEmail + " a few days following the webinar. " +
                        "However, if you wish to pay immediately <a href='" + TenantURL + "/Resume/" + order.idOrder +
                        "'> click here.</a>" +
                        " Is someone else in your organization responsible for payments? <a href='" +
                        TenantURL + "/Order/AddBillingEmail?idOrder=" + order.idOrder + "'>" +
                        "Enter their email here</a> and we will send the required information directly.";
                }
            }

            if (row.RegistrationType.SKU.ToLower().Contains("wsp"))
            {
                // uncomment if we decide to without the code until after payment

                //if (order.OrderStatus == OrderStatus.Paid)
                //{
                //    fields.PaymentCaption =
                //        "Your subscription code is [" + row.Discount.DiscountCode + "] and is activated - ready to use! From now until the credits have been exhausted any order placed by " +
                //        order.BillingEmail + " will have your WSP credits automatically applied. In addition, you can distribute the code shown below to others within your organization. " +
                //        " During checkout that code can be entered manually and will work just the same as if used by the primary email address. If you would like additional addresses to have the same 'auto-apply' rights as " +
                //        order.BillingEmail + " just get in touch with us and we will be happy to add them.";
                //}
                //else
                //{
                fields.PaymentCaption =
                    "Your subscription code is [" + row.Discount.DiscountCode + "] and is activated - ready to use! From now until the credits have been exhausted any order placed by " +
                    order.BillingEmail + " will have your WSP credits automatically applied. And feel free to share the code " + row.Discount.DiscountCode + " above with others in your organization. " +
                    " It can be applied during checkout - look for the button labeled <i>Add Discount?</i>. Clicking that will prompt for your code [<b>" + row.Discount.DiscountCode + "</b>]." +
                    " If you would like additional addresses to have the same <i>auto-apply</i>  rights as " +
                    order.BillingEmail + " just get in touch with us and we will be happy to add them.";
                //}
            }
            if (row.RegistrationType.ShowRecordingNotifications.ToLower() == "no")
            {
                fields.RegDesc =
                    "Your registration includes access to the recording and handouts for five (5) business days. You can upgrade your order to gain 6 months OnDemand access - or get the Premier Package which includes a CD-ROM. <a href='" +
                    TenantURL + "/Resume/" + order.idOrder +
                    "'>" + " We'll be happy to adjust your registration.</a> ";
            }
            else if (row.RegistrationType.ShowShippedNotifications.ToLower() == "no" && 
                !row.Webinar.Title.Contains("Compliance Perspectives") )
            {
                fields.RegDesc =
                    "Your registration includes access to the recording and handouts for six (6) months. <a href='" +
                    TenantURL + "/Resume/" + order.idOrder +
                    "'>" + " You can still upgrade your order to include the CD-ROM.</a>";
            }
            if (row.RegistrationType.ShowShippedNotifications.ToLower() == "yes")
            {
                fields.RegDesc = " Your CD-ROM will ship in 5-7 business days. Your registration also includes access to the recording and handouts for six (6) months.";
            }

            if (row.AdditionalLocation != null)
            {
                var locs = "";
                foreach (var loc in row.AdditionalLocation)
                {
                    locs += loc.Email + ", ";
                }
                locs = locs.TrimEnd(',');

                if (row.AdditionalLocation.Count == 1)
                {
                    fields.ExistingAddLocs = locs.TrimEnd(',') +
                                             " is currently included as an Additional Location. ";
                }
                if (row.AdditionalLocation.Count > 1)
                {
                    fields.ExistingAddLocs = locs.TrimEnd(',').Replace(",", ", ") +
                                             " are the included Additional Locations. ";
                }

                if (locs == "")
                {
                    fields.ExistingAddLocs =
                        " Need to support remote branches? Additional locations cost [addloccost] per seat. ";
                }

            }


            return fields;

        }

        public void ScheduleConnInfoSenderAudit(Webinar webinar)
        {
            var _storageCredentials = new StorageCredentials(_globalConfig.StorageAccountName,
                _globalConfig.StorageAccessKey);
            var _cloudStorageAccount = new CloudStorageAccount(_storageCredentials, false);

            var _queueClient = _cloudStorageAccount.CreateCloudQueueClient();

            CloudQueue cloudQueue = _queueClient.GetQueueReference("conn-info-sender-audit");  // passed in during construction, usually from web.config
            cloudQueue.CreateIfNotExists();



            System.Threading.Thread.Sleep(TimeSpan.FromMinutes(1));


            var cloudQueueMessage = new CloudQueueMessage(JsonConvert.SerializeObject(webinar.Comments));
            cloudQueue.EncodeMessage = true;
            cloudQueue.AddMessage(cloudQueueMessage);

        }

        public string ReplaceMergeCodes(string copy, Affiliate aff)
        {
            copy = copy.Replace("{aff_EmailBanner}", aff.EmailBanner);
            copy = copy.Replace("{aff_EmailFooter}", aff.EmailFooter);
            copy = copy.Replace("{aff_ttsdomain}", aff.ttsDomain);
            copy = copy.Replace("{aff_idUserAff}", aff.idUserAff.ToString());
            //copy = copy.Replace("{aff_timeZone}", aff.timeZone);
            copy = copy.Replace("{aff_ContactPerson}", aff.ContactPerson);
            copy = copy.Replace("{aff_NotiPromos}", aff.NotiPromos);
            copy = copy.Replace("{aff_ContactPhone}", aff.ContactPhone);
            //copy = copy.Replace("{aff_ContactPhone}", aff.ContactPhone);

            return copy;

        }

        public string ReplaceTimeString(Webinar webinar, USTimeZone timeZone)
        {
            return "<i>" + DateTimeHelper.FormatTime(webinar.Date, timeZone, false) +
                  " - " +
                  DateTimeHelper.FormatTime(
                      webinar.Date.AddHours((double)webinar.Duration), timeZone, true) + "<br /></i>";
        }

        public object FuzzyMatch(string parsedOrderEventTitle, IEnumerable<string> @select)
        {
            throw new NotImplementedException();
        }

        public ParseOrderModel ParseConfSem(string _doc, string toString)
        {
            //_doc = _doc.Replace("\t", "");
            var startBlock = _doc.IndexOf("Mailing Address");
            var endBlock = _doc.IndexOf("Bill To:");

            var splitBlock = _doc.Substring(startBlock, endBlock - startBlock).Split('\n');

            startBlock = _doc.IndexOf("AMOUNT") + 6;
            endBlock = _doc.IndexOf("TOTAL");
            var regDataBlock = _doc.Substring(startBlock, endBlock - startBlock)
                .Replace(". Sponsored by: BankWebinars.com: Registration -", "|")
                .Replace(": Live Teleconference, ", "|").Trim().Split('|');

            startBlock = _doc.IndexOf("DATE:") + 5;
            endBlock = _doc.IndexOf("Mailing Address");
            var orderDateBlock = _doc.Substring(startBlock, endBlock - startBlock)
                .Trim();
            var model = new ParseOrderModel();

            model.OrderDate = Convert.ToDateTime(orderDateBlock);

            try
            {
                //var sb = new StringBuilder();

                var count = 0;
                var lineCount = 0;
                var phoneNum = "";
                foreach (var line in splitBlock)
                {
                    lineCount++;
                    if (line.Contains("@"))
                    {
                        count++;
                        model.Email = line;
                        break;
                    }
                    if (phoneNum == "")
                    {
                        phoneNum = ParsePhone(line);
                    }
                }
                if (count == 0)
                {
                    model.LoggerNotes = "ERROR: no email address detected.";
                    return model;
                }

                if (phoneNum == "")
                {
                    phoneNum = "555-555-5555";
                }

                if (count > 1)
                {
                    model.LoggerNotes = "Multiple emails detected.";
                }

                var name = new HumanName(splitBlock[1]);

                model.FirstName = name.First;
                model.LastName = name.Last;

                model.Title = splitBlock[2];
                model.Institution = splitBlock[3];

                model.BillingAddress = new Address();
                model.ShippingAddress = new Address();


                model.BillingAddress = new Address();
                model.ShippingAddress = new Address();
                model.BillingAddress.AddressType = "Billing";
                model.BillingAddress.Phone = phoneNum;
                model.BillingAddress.Name = name.FullName;
                model.BillingAddress.StreetAddress = splitBlock[4];
                //check if city/state/zip found in [5] or [6]
                var has2AddressLines = splitBlock[5].Split(',');
                if (has2AddressLines.Length > 1)
                {
                    model.BillingAddress.City = splitBlock[5].Split(',')[0];
                    model.BillingAddress.State = splitBlock[5].Split(',')[1].Split(' ')[1];
                    model.BillingAddress.Zip = splitBlock[5].Split(',')[1].Split(' ')[2];
                }
                else
                {
                    if (splitBlock[4].Contains(","))
                    {
                        model.BillingAddress.City = splitBlock[4].Split(',')[0];
                        model.BillingAddress.State = splitBlock[4].Split(',')[1].Split(' ')[1];
                        model.BillingAddress.Zip = splitBlock[4].Split(',')[1].Split(' ')[2];
                    }
                    else
                    {
                        model.BillingAddress.City = splitBlock[6].Split(',')[0];
                        model.BillingAddress.State = splitBlock[6].Split(',')[1].Split(' ')[1];
                        model.BillingAddress.Zip = splitBlock[6].Split(',')[1].Split(' ')[2];
                    }
                }

                model.ShippingAddress.AddressType = "Shipping";
                model.ShippingAddress.Name = name.FullName;
                model.ShippingAddress.Phone = phoneNum;
                model.ShippingAddress.StreetAddress = splitBlock[4];
                if (has2AddressLines.Length > 1)
                {
                    model.ShippingAddress.City = splitBlock[5].Split(',')[0];
                    model.ShippingAddress.State = splitBlock[5].Split(',')[1].Split(' ')[1];
                    model.ShippingAddress.Zip = splitBlock[5].Split(',')[1].Split(' ')[2];
                }
                else
                {
                    if (splitBlock[4].Contains(","))
                    {
                        model.ShippingAddress.City = splitBlock[4].Split(',')[0];
                        model.ShippingAddress.State = splitBlock[4].Split(',')[1].Split(' ')[1];
                        model.ShippingAddress.Zip = splitBlock[4].Split(',')[1].Split(' ')[2];
                    }
                    else
                    {

                        model.ShippingAddress.City = splitBlock[6].Split(',')[0];
                        model.ShippingAddress.State = splitBlock[6].Split(',')[1].Split(' ')[1];
                        model.ShippingAddress.Zip = splitBlock[6].Split(',')[1].Split(' ')[2];
                    }

                }
                model.EventTitle = regDataBlock[0].Trim();
                if (model.EventTitle.Contains(":"))
                {
                    var findPO = regDataBlock[0].Trim().Split(':')[0].Trim();
                    if (findPO.Split(' ').Length == 1)
                    {

                        var getsTail = regDataBlock[0].Trim().Split(':').Skip(1);
                        model.EventTitle = "";
                        foreach (var line in getsTail)
                        {
                            model.EventTitle += line + ":";
                        }
                        model.EventTitle = model.EventTitle.TrimEnd(':').Trim();
                    }
                }
                model.EventDate = regDataBlock[1].Split(';')[1].Trim();
                model.EventTime = regDataBlock[1].Split(';')[0].Trim();
                model.RegTypeAsString = regDataBlock[2].Trim();

                return model;

            }

            catch (Exception e)
            {
                model.LoggerNotes = "ERROR: ParseConfSem tossed: " + e.Message;
                _logger.FatalException("ParseConfSem", e);
                throw;
            }
            return null;
        }


        //public ParseOrderModel ParseConfSem(string _doc, string toString)
        //{ //html version of parser is depricated
        //    var model = new ParseOrderModel();
        //    try
        //    {
        //        model.OrderDate = DateTime.Now;

        //        HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();

        //        doc.LoadHtml(_doc);

        //        if (doc.ParseErrors != null && doc.ParseErrors.Count() > 0)
        //        {
        //            // Handle any parse errors as required
        //            model.LoggerNotes = "ParseConfSem Errors: ";
        //            foreach (var error in doc.ParseErrors)
        //            {
        //                model.LoggerNotes += error.Reason;
        //            }
        //            _logger.Warn("IncomingParseConfSem Errors: " + model.LoggerNotes);
        //        }
        //        if (doc.DocumentNode != null)
        //        {
        //            try
        //            {
        //                var splitBlock = doc.DocumentNode.SelectNodes("//table")[3].InnerHtml.Replace("<br>", "\n").Split('\n');
        //                var sb = new StringBuilder();

        //                var count = 0;
        //                var lineCount = 0;
        //                var phoneNum = "";
        //                foreach (var line in splitBlock)
        //                {
        //                    lineCount++;
        //                    if (line.Contains("@"))
        //                    {
        //                        count++;
        //                        model.Email = line;
        //                        break;
        //                    }
        //                    else
        //                    {
        //                        if (lineCount > 4)
        //                        {
        //                            var phoneUtil = PhoneNumberUtil.IsViablePhoneNumber(line);
        //                            if (phoneUtil)
        //                            {
        //                                phoneNum = line;
        //                            }
        //                            else
        //                            {
        //                                sb.Append(line + " ");
        //                            }
        //                        }
        //                    }

        //                }
        //                if (count == 0)
        //                {
        //                    model.LoggerNotes = "ERROR: no email address detected.";
        //                    return model;
        //                }

        //                if (count > 1)
        //                {
        //                    model.LoggerNotes = "Multiple emails detected.";
        //                }

        //                var name = new HumanName(splitBlock[1]);

        //                model.FirstName = name.First;
        //                model.LastName = name.Last;

        //                model.Title = splitBlock[2];
        //                model.Institution = splitBlock[3];

        //                model.BillingAddress = new Address();
        //                model.ShippingAddress = new Address();

        //                var myAddress = GetMapzenAddress(sb.ToString());

        //                model.BillingAddress.AddressType = "Billing";
        //                model.BillingAddress.Name = name.FullName;
        //                model.BillingAddress.StreetAddress = myAddress.road.FirstOrDefault();
        //                model.BillingAddress.City = myAddress.city.FirstOrDefault();
        //                model.BillingAddress.State = myAddress.state.FirstOrDefault();
        //                model.BillingAddress.Zip = myAddress.postcode.FirstOrDefault();
        //                model.BillingAddress.Country = myAddress.country.FirstOrDefault();
        //                model.BillingAddress.Phone = phoneNum;


        //                model.ShippingAddress.AddressType = "Shipping";
        //                model.ShippingAddress.Name = name.FullName;
        //                model.ShippingAddress.StreetAddress = myAddress.road.FirstOrDefault();
        //                model.ShippingAddress.City = myAddress.city.FirstOrDefault();
        //                model.ShippingAddress.State = myAddress.state.FirstOrDefault();
        //                model.ShippingAddress.Zip = myAddress.postcode.FirstOrDefault();
        //                model.ShippingAddress.Country = myAddress.country.FirstOrDefault();
        //                model.ShippingAddress.Phone = phoneNum;

        //                var regData = doc.DocumentNode.SelectNodes("//table")[4].InnerText;

        //                var regDataBlockStart = regData.IndexOf("AMOUNT", StringComparison.Ordinal) + "Amount".Length;
        //                var regDataBlockEnd = regData.IndexOf("$", StringComparison.Ordinal);

        //                var regDataBlock = regData.Substring(regDataBlockStart, regDataBlockEnd - regDataBlockStart)
        //                    .Replace(". Sponsored by: BankWebinars.com: Registration -", "|")
        //                    .Replace(": Live Teleconference, ", "|")
        //                    .Replace("\n1\n", "").Trim();
        //                if (doc.DocumentNode.SelectNodes("//table")[3].InnerText.ToLower().Contains("paid"))
        //                {
        //                    model.Status = OrderStatus.Paid;
        //                    model.EventTitle = regDataBlock.Split('|')[0].Trim();
        //                    model.EventDate = regDataBlock.Split('|')[1].Split(';')[1].Trim();
        //                    model.EventTime = regDataBlock.Split('|')[1].Split(';')[0].Trim();
        //                    model.RegTypeAsString = regDataBlock.Split('|')[2].Trim();

        //                }
        //                else
        //                {
        //                    var po = regDataBlock.Split(':')[0];

        //                    model.Status = OrderStatus.Submitted;
        //                    model.EventTitle = regDataBlock.Split('|')[0].Substring(po.Length + ": ".Length);
        //                    model.EventDate = regDataBlock.Split('|')[1].Split(';')[1].Trim();
        //                    model.EventTime = regDataBlock.Split('|')[1].Split(';')[0].Trim();
        //                    model.RegTypeAsString = regDataBlock.Split('|')[2].Trim();
        //                }
        //                return model;

        //            }
        //            catch (Exception ex)
        //            {
        //                _logger.FatalException("IncomingParseConfSem", ex);
        //                model.LoggerNotes = "ERROR: " + ex.Message;
        //            }
        //            return model;
        //        }
        //        else
        //        {
        //            model.LoggerNotes = "ERROR: ParseConfSem Returned Null! ";
        //            _logger.Warn("IncomingConfSem is null");
        //            return null;
        //        }

        //    }
        //    catch (Exception e)
        //    {
        //        model.LoggerNotes = "ERROR: ParseConfSem tossed: " + e.Message;
        //        _logger.FatalException("ParseConfSem", e);
        //        throw;
        //    }

        //}

        private MapZenAddress GetMapzenAddress(string addString)
        {
            try
            {
                var a = 1;
                WebRequest req = WebRequest.Create("https://libpostal.mapzen.com/parse?address=" + HttpUtility.UrlEncode(addString) + "&format=keys&api_key=mapzen-iYcwH4a");

                req.Method = "GET";
                string returnvalue1 = "";

                WebResponse res = req.GetResponse();
                using (WebResponse response = req.GetResponse())
                {
                    StreamReader reader = new StreamReader(res.GetResponseStream());
                    //using (Stream stream = response.GetResponseStream())
                    //{
                    //    XmlTextReader reader = new XmlTextReader(stream);
                    //    returnvalue1 = reader.Value;
                    //}
                    returnvalue1 = reader.ReadToEnd();
                }
                //
                MapZenAddress importResult = JsonConvert.DeserializeObject<MapZenAddress>(returnvalue1);

                res.Close();

                return importResult;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            return null;
        }

        public ParseOrderModel ParseRateWatch(string __doc)
        {
            var model = new ParseOrderModel();
            try
            {
                model.OrderDate = DateTime.Now;

                HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();

                doc.LoadHtml(__doc);
                model.LoggerNotes = doc.DocumentNode.InnerText;

                var streetAddress =
                    doc.DocumentNode.SelectSingleNode("//comment()[contains(., 'Address Bar')]/following-sibling::table/tr[1]/td[4]").InnerHtml.ToString().Split('>')[1].Replace("<br", "");
                var cityStateZip = doc.DocumentNode.SelectSingleNode("//comment()[contains(., 'Address Bar')]/following-sibling::table/tr[1]/td[4]")
                    .InnerHtml.ToString().Split('>')[2].Replace("</font", "");


                var parsedName = doc.DocumentNode.SelectSingleNode("//comment()[contains(., 'Name Bar')]/following-sibling::table/tr[1]/td[4]").InnerText;
                var parsedEmail = doc.DocumentNode.SelectSingleNode("//comment()[contains(., 'Email Bar')]/following-sibling::table/tr[1]/td[4]").InnerText;
                var parsedCompanyName = doc.DocumentNode.SelectSingleNode("//comment()[contains(., 'Company Bar')]/following-sibling::table/tr[1]/td[4]").InnerText;
                var parsedTitle = doc.DocumentNode.SelectSingleNode("//comment()[contains(., 'Company Title Bar')]/following-sibling::table/tr[1]/td[4]").InnerText;
                //var parsedAddress = pageText.Substring(regDataBlockStart, regDataBlockEnd - regDataBlockStart).TrimStart('\n').TrimEnd('\n');
                var parsedPhone = doc.DocumentNode.SelectSingleNode("//comment()[contains(., 'Phone Number Bar')]/following-sibling::table/tr[1]/td[4]").InnerText;

                var parsedEventName = doc.DocumentNode.SelectSingleNode("//comment()[contains(., 'Webinar Name Bar')]/following-sibling::table/tr[1]/td[4]").InnerText;
                var parsedEventDate = doc.DocumentNode.SelectSingleNode("//comment()[contains(., 'Date Bar')]/following-sibling::table/tr[1]/td[4]").InnerText;
                var parsedEventTime = doc.DocumentNode.SelectSingleNode("//comment()[contains(., 'Time Bar')]/following-sibling::table/tr[1]/td[4]").InnerText;
                var parsedEventType = doc.DocumentNode.SelectSingleNode("//comment()[contains(., 'Event Type Bar')]/following-sibling::table/tr[1]/td[4]").InnerText;

                var name = new HumanName(parsedName);

                model.Email = parsedEmail;
                model.FirstName = name.First;
                model.LastName = name.Last;

                model.Title = parsedTitle;
                model.Institution = parsedCompanyName;

                model.BillingAddress = new Address();
                model.ShippingAddress = new Address();

                model.BillingAddress.AddressType = "Billing";
                model.BillingAddress.Name = name.FullName;
                model.BillingAddress.StreetAddress = streetAddress;
                model.BillingAddress.City = cityStateZip.Split(',')[0];
                model.BillingAddress.State = cityStateZip.Split(',')[1].Split(' ')[1];
                model.BillingAddress.Zip = cityStateZip.Split(',')[1].Split(' ')[2];
                model.BillingAddress.Country = "USA";
                model.BillingAddress.Phone = parsedPhone.Split(' ')[0];

                model.ShippingAddress.AddressType = "Shipping";
                model.ShippingAddress.Name = name.FullName;
                model.ShippingAddress.StreetAddress = streetAddress;
                model.ShippingAddress.City = cityStateZip.Split(',')[0];
                model.ShippingAddress.State = cityStateZip.Split(',')[1].Split(' ')[1];
                model.ShippingAddress.Zip = cityStateZip.Split(',')[1].Split(' ')[2];
                model.ShippingAddress.Country = "USA";
                model.ShippingAddress.Phone = parsedPhone.Split(' ')[0];

                model.EventTitle = parsedEventName.Trim();
                model.EventDate = parsedEventDate.Trim();
                model.EventTime = parsedEventTime.Trim();
                model.RegTypeAsString = parsedEventType.Split('(')[1].TrimEnd(')').Trim();

                return model;
            }
            catch (Exception ex)
            {
                model.LoggerNotes += "ParseRateWatch FatalExecption: " + ex.Message;
                _logger.FatalException("IncomingParserateWatch", ex);
            }
            return model;
        }

        public MigrateOrderModel ConvertToMigrator(ParseOrderModel parsedOrder)
        {
            MigrateOrderModel model = new MigrateOrderModel();

            model.BillingAddress = parsedOrder.BillingAddress;
            model.ShippingAddress = parsedOrder.ShippingAddress;
            model.BillingAddress.City = parsedOrder.BillingAddress.City;
            model.BillingAddress.StreetAddress = parsedOrder.BillingAddress.StreetAddress;
            model.BillingAddress.StreetAddress2 = parsedOrder.BillingAddress.StreetAddress2;
            model.BillingAddress.Country = parsedOrder.BillingAddress.Country;
            model.BillingAddress.Phone = parsedOrder.BillingAddress.Phone;
            model.BillingAddress.State = parsedOrder.BillingAddress.State;
            model.BillingAddress.Zip = parsedOrder.BillingAddress.Zip;
            model.BillingAddress.Name = parsedOrder.BillingAddress.Name;
            model.BillingAddress.AddressType = "Billing";
            model.ShippingAddress.AddressType = "Shipping";

            model.ShippingAddress.City = parsedOrder.ShippingAddress.City;
            model.ShippingAddress.StreetAddress = parsedOrder.ShippingAddress.StreetAddress;
            model.ShippingAddress.StreetAddress2 = parsedOrder.ShippingAddress.StreetAddress2;
            model.ShippingAddress.Country = parsedOrder.ShippingAddress.Country;
            model.ShippingAddress.Phone = parsedOrder.ShippingAddress.Phone;
            model.ShippingAddress.State = parsedOrder.ShippingAddress.State;
            model.ShippingAddress.Zip = parsedOrder.ShippingAddress.Zip;
            model.ShippingAddress.Name = parsedOrder.ShippingAddress.Name;

            model.Email = parsedOrder.Email;
            model.Title = parsedOrder.Title;
            model.AdditionalLocationsString = parsedOrder.AdditionalLocationsString;
            model.AdminComments = parsedOrder.AdminComments;
            model.DiscountCode = parsedOrder.DiscountCode;
            model.FirstName = parsedOrder.FirstName;
            model.LastName = parsedOrder.LastName;
            model.Institution = parsedOrder.Institution;
            model.LoggerNotes = parsedOrder.LoggerNotes;
            model.idAffiliate = parsedOrder.idAffiliate;
            model.Origin = parsedOrder.Origin;
            model.Status = parsedOrder.Status;
            model.OrderDate = parsedOrder.OrderDate;

            return model;
        }

        public ImportOrderForAcsModel ParseAcs(string _doc, string orderDate)
        {

            ImportOrderForAcsModel model = new ImportOrderForAcsModel();
            model.DateSubmittedToACS = orderDate;

            HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();

            doc.LoadHtml(_doc);

            if (doc.ParseErrors != null && doc.ParseErrors.Count() > 0)
            {
                // Handle any parse errors as required
                model.LoggerNotes = "AcsImporter ParseErrors: ";
                foreach (var error in doc.ParseErrors)
                {
                    model.LoggerNotes += error.Reason;
                }
                return model;
            }
            else
            {
                if (doc.DocumentNode != null)
                {
                    HtmlNode bodyNode = doc.DocumentNode.SelectSingleNode("//body");

                    if (bodyNode != null)
                    {
                        try
                        {
                            var _values = doc.DocumentNode.SelectNodes("//tr[@bgcolor='#FFFFFF']/td[2]");
                            var _names = doc.DocumentNode.SelectNodes("//tr[@bgcolor='#EAF2FA']/td");


                            string[] values = new string[_values.Count];
                            string[] names = new string[_values.Count];

                            var flag = "";

                            for (var i = 0; i < _values.Count - 1; i++)
                            {
                                string value = _values[i].InnerText.TrimStart().TrimEnd();
                                string name =
                                    Regex.Replace(_names[i].InnerText, @"\s", string.Empty, RegexOptions.Multiline)
                                        .TrimStart().TrimEnd();
                                //docs the error
                                if (value.Length == 0)
                                {
                                    value = name;
                                    flag = "Value is 0 length:" + name + " email: ";
                                }
                                switch (name)
                                {
                                    case "AdditionalLocationsString":
                                        model.AdditionalLocationsString = value;
                                        break;
                                    case "AffiliateID":
                                        model.AffiliateID = value;
                                        break;
                                    case "BankWebID":
                                        model.BankWebID = value;
                                        break;

                                    case "BillingContact":
                                        model.BillingContact = value;
                                        break;
                                    case "City":
                                        model.City = value;
                                        break;
                                    case "Company":
                                        model.Company = value;
                                        break;
                                    case "CompanyBillingInformation":
                                        model.CompanyBillingInformation = value;
                                        break;
                                    case "Country":
                                        model.Country = value;
                                        break;
                                    case "CourseDeliveryType":
                                        model.CourseDeliveryType = value;
                                        break;
                                    case "CourseNumber":
                                        model.CourseNumber = value;
                                        break;
                                    case "CoursePrice":
                                        model.CoursePrice = value;
                                        break;
                                    case "CreditCard":
                                        model.CreditCard = value;
                                        break;
                                    case "DateSubmittedToACS":
                                        model.DateSubmittedToACS = orderDate;
                                        break;

                                    case "DeliveryType":
                                        model.DeliveryType = value;
                                        break;
                                    case "Email":
                                        model.Email = value;
                                        break;
                                    case "EmailAddress":
                                        model.EmailAddress = value;
                                        break;
                                    case "EmailAddressforCreditCardReceipt":
                                        model.EmailAddressforCreditCardReceipt = value;
                                        break;
                                    case "Ext":
                                        model.Ext = value;
                                        break;
                                    case "FirstName":
                                        model.FirstName = value;
                                        break;
                                    case "LastName":
                                        model.LastName = value;
                                        break;
                                    case "PaymentMethod":
                                        model.PaymentMethod = value;
                                        break;
                                    case "Phone":
                                        model.Phone = value;
                                        break;
                                    case "State":
                                        model.State = value;
                                        break;
                                    case "State/Province/Region":
                                        model.State_Province_Region = value;
                                        break;
                                    case "StreetorP.O.Box":
                                        model.StreetorP_O_Box = value;
                                        break;
                                    case "Title":
                                        model.Title = value;
                                        break;
                                    case "WebinarDate":
                                        model.WebinarDate = value;
                                        break;
                                    case "WebinarTitle":
                                        model.WebinarTitle = value;
                                        break;
                                    case "ZeroValue":
                                        model.ZeroValue = value;
                                        break;
                                    case "Zip/PostalCode":
                                        model.Zip_PostalCode = value;
                                        break;
                                    default:
                                        if (name.EndsWith("LocationEmail"))
                                        {
                                            model.AdditionalLocationsString += value + ",";
                                        }
                                        else
                                        {
                                            model.LoggerNotes += ("Keyname not known: " + name + " value:" + value);
                                        }
                                        break;


                                }

                                //if (model.(name))
                                //{
                                //    ACSOrderDictionary[name] = value;
                                //}
                                //else
                                //{

                                //}
                                values[i] = value;
                                names[i] = name;
                            }

                            //_logger.Info(JsonConvert.SerializeObject(sb.ToString(), Formatting.None,
                            //    new JsonSerializerSettings
                            //    {
                            //        MaxDepth = 1,
                            //        ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                            //    }
                            //    ));
                        }
                        catch (Exception ex)
                        {
                            model.LoggerNotes += "ParseAcs FatalExecption: " + ex.Message;
                            _logger.FatalException("IncomingParseACS", ex);

                        }
                        return model;
                    }
                }
                else
                {
                    model.LoggerNotes += "ParseAcs Returned Null! ";
                    _logger.Warn("Incoming | AcsModel is null");
                    return null;
                }
            }
            return null;

        }

        public Address ParseAddress(string address)
        {

            var myAddress = GetMapzenAddress(address);
            var add = new Address();

            //add.AddressType = "Billing";
            add.StreetAddress = myAddress.road.FirstOrDefault();
            add.City = myAddress.city.FirstOrDefault();
            add.State = myAddress.state.FirstOrDefault();
            add.Zip = myAddress.postcode.FirstOrDefault();
            add.Country = myAddress.country.FirstOrDefault();

            return add;
        }

        public string ParsePhone(string phoneTest)
        {
            var phoneNum = "";
            PhoneNumberUtil phoneUtil = PhoneNumberUtil.GetInstance();
            var extractPossible = phoneUtil.IsPossibleNumber(phoneTest, "US");

            if (extractPossible)
            {
                phoneNum = phoneTest;
            }

            return phoneNum;
        }

        public string FindChangedRegTypes(Order order)
        {
            var dataOperations = new DataOperations(TtsConfig.DefaultConnectionString);
            var auditChangedRegType = dataOperations.GetAuditNotes(order, "RegTypeChanges");

            return auditChangedRegType;
        }

        public string FindChangedOrderStatus(Order order)
        {
            var dataOperations = new DataOperations(TtsConfig.DefaultConnectionString);
            var changedOrderStatus = dataOperations.GetAuditNotes(order, "OrderStatusChanges");

            return changedOrderStatus;
        }

        public static string[] AddNonvalidToArray(string[] zipCentricFields)
        {
            if (zipCentricFields == null) throw new ArgumentNullException("zipCentricFields");

            var fixedArray = new string[3];

            zipCentricFields.CopyTo(fixedArray, 0);

            fixedArray[2] = WebUiConstants.Nonvalid;

            return fixedArray;
        }

    }

    internal class MapZenAddress
    {
        public List<string> city { get; set; }
        public List<string> country { get; set; }
        public List<string> house_number { get; set; }
        public List<string> postcode { get; set; }
        public List<string> road { get; set; }
        public List<string> state { get; set; }
        public List<string> unit { get; set; }
    }
}