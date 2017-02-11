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
using CUWebinars.Business.Core.Helpers;
using CUWebinars.Business.Models;
using CUWebinars.Business.Services;
using CUWebinars.Web.Core;
using CUWebinars.Web.Services;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Trace = System.Diagnostics.Trace;

namespace CUWebinars.Web.Helpers
{
    public class AppHelper : IAppHelper
    {
        private readonly IStateService _stateService;

        private readonly HttpRequestBase _request;


        public AppHelper(HttpRequestBase request, IStateService stateService)
        {
            _request = request;
            _stateService = stateService;
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


        public string GetUserAuditInfo()
        {
            IDictionary<string, string> auditInfoDictionary = new Dictionary<string, string>();
            var sessionStart = GetSessionStartInfo();
            auditInfoDictionary.Add("RemoteAddress", SecurityElement.Escape(_request.ServerVariables["REMOTE_ADDR"]));
            auditInfoDictionary.Add("RemoteHost", SecurityElement.Escape(_request.ServerVariables["REMOTE_HOST"]));
            auditInfoDictionary.Add("RemoteUser", SecurityElement.Escape(_request.ServerVariables["REMOTE_USER"]));
            auditInfoDictionary.Add("UserAgent", SecurityElement.Escape(_request.ServerVariables["HTTP_USER_AGENT"]));
            auditInfoDictionary.Add("Cookie", SecurityElement.Escape(_request.ServerVariables["HTTP_COOKIE"]));
            auditInfoDictionary.Add("FirstPage", SecurityElement.Escape(_stateService.GetValue<string>("FirstPage")));
            auditInfoDictionary.Add("Elmah", SecurityElement.Escape(_stateService.GetValue<string>("Elmah")));
            auditInfoDictionary.Add("SessionRoot", SecurityElement.Escape(_stateService.GetValue<string>("SessonRoot")));
            auditInfoDictionary.Add("SessionID", SecurityElement.Escape(_stateService.GetValue<string>("SessionID")));
            auditInfoDictionary.Add("AffiliateSessionSource", SecurityElement.Escape(_stateService.GetValue<string>("AffiliateSessionSource")));
            auditInfoDictionary.Add("SessionStart", SecurityElement.Escape(_stateService.GetValue<string>("AffiliateSessionSource")));



            var auditObjectInner = JsonHelpers.CreateJsonObjectFromDictionary(auditInfoDictionary);

            var jobject = new JObject();
            jobject.Add("AuditInfo", auditObjectInner);

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
            //TODO: this really, really needs to be finished.
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
        
        public bool BuildWebinarOrderOrderInvoiceRow(Order order, StringBuilder discountNotes, out OrderRow row, out string _price,
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

            adjustedTotal = row.RowPrice - (decimal) dict["OriginalTotal"];
            adjustedRoyalty = (decimal) dict[adjustmentDirection];

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
                    discountAmount = (row.UnitPrice*(row.Discount.PercentOff/100)).ToString("c");
                }
                if (row.Discount.FlatOff > 0)
                {
                    discountAmount = (row.UnitPrice - row.Discount.FlatOff).ToString("C");
                }
                discountNotes.Append(Environment.NewLine);
            }
            return false;
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