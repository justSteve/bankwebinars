using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Security;
using System.Text;
using System.Web;
using System.Web.Mvc;
using CUWebinars.Business.Core;
using CUWebinars.Business.Core.Helpers;
using CUWebinars.Business.Models;
using CUWebinars.Business.Notification;
using CUWebinars.Business.Services;
using CUWebinars.Web.Core;
using CUWebinars.Web.Models;
using CUWebinars.Web.Services;
using GemBox.Document;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Trace = System.Diagnostics.Trace;

namespace CUWebinars.Web.Helpers
{
    public class AppHelper : IAppHelper
    {
        private readonly IStateService _stateService;

        private readonly HttpRequestBase _request;
        //public readonly TtsConfigHelper _globalConfig;

        private readonly GlobalConfig _globalConfig = GlobalConfig.GlobalConfigSingleton;



        public AppHelper(HttpRequestBase request, IStateService stateService)
        {
            _request = request;
            _stateService = stateService;
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

            return jobject.ToString(Newtonsoft.Json.Formatting.None);
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

        public string CleanHtmlCodesAndLogo(string body, string tenantLogo, string addloccost, string specialMsg = null)
        {
            body = body.Replace("&gt;", ">");
            body = body.Replace("&lt;", "<");
            body = body.Replace("[logo]", "<img src=" + tenantLogo + " />");
            if (!string.IsNullOrEmpty(addloccost))
                body = body.Replace("[addloccost]", addloccost);
            if (!string.IsNullOrEmpty(specialMsg))
            {
                body = body.Replace("[specialmsg]", specialMsg);
            }
            else
            {
                body = body.Replace("[specialmsg]", "");
            }
            return body;
        }


        public NotificationMessageFields BuildNotiFields(Order order, string hasCCAddress)
        {

            var fields = new NotificationMessageFields();
            OrderRow row = order.OrderRows.SingleOrDefault(r => r.RowStatus == OrderRowStatus.Active);

            var fileBuilder = "Materials Coming Soon";
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
                    " Connection info is shared with " + listOfCCs.TrimEnd(',') + ".";
            }
            fields.PaymentStatus = order.OrderStatus.ToString();

            if (order.OrderStatus == OrderStatus.Paid)
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

            if (row.RegistrationType.ShowRecordingNotifications.ToLower() == "no")
            {
                fields.RegDesc =
                    "Your registration includes access to the recording and handouts for five (5) business days. You can upgrade your order to gain 6 months OnDemand access - or get the Premier Package which includes a CD-ROM. <a href='" +
                    TenantURL + "/Resume/" + order.idOrder +
                    "'>" + " We'll be happy to adjust your registration.</a> ";
            }
            else if (row.RegistrationType.ShowShippedNotifications.ToLower() == "no")
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

        public static string[] AddNonvalidToArray(string[] zipCentricFields)
        {
            if (zipCentricFields == null) throw new ArgumentNullException("zipCentricFields");

            var fixedArray = new string[3];

            zipCentricFields.CopyTo(fixedArray, 0);

            fixedArray[2] = WebUiConstants.Nonvalid;

            return fixedArray;
        }
    }
}