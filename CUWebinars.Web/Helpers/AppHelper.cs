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
using Glimpse.Core.Tab;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Trace = System.Diagnostics.Trace;

namespace CUWebinars.Web.Helpers
{
    public class AppHelper : IAppHelper
    {
        private readonly HttpRequestBase _request;
        private readonly IAffiliateManagementService _affiliateManagementService;

        public AppHelper(HttpRequestBase request)
        {
            _request = request;
            //_affiliateManagementService = affiliateManagementService;
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
                        string tz = "-6";
                        if (msgReader[2] == null)
                        {
                            tz = msgReader[2].ToString();
                        }
                        cityStateFromZip =
                            msgReader.FieldCount > 0
                                ? msgReader[0].ToString() + "," + msgReader[1].ToString() + "," +
                                  tz
                                : null;
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
            //TODO: time to transition the legacy xml object to a native class.
            HttpRequest request = HttpContext.Current.Request;
            var info = new SessionStartInfo
            {
                RemoteAddress = _request.ServerVariables["REMOTE_ADDR"],
                RemoteHost = _request.ServerVariables["REMOTE_HOST"],
                RemoteUser = _request.ServerVariables["REMOTE_USER"],
                UserAgent = _request.ServerVariables["HTTP_USER_AGENT"],
                UserCookie = _request.ServerVariables["HTTP_COOKIE"]
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


        private const string AUDIT_XML_TEMPLATE =
            "<AuditInfo>" +
            "<RemoteAddress>#REMOTE_ADDR#</RemoteAddress>" +
            "<RemoteHost>#REMOTE_HOST#</RemoteHost>" +
            "<RemoteUser>#REMOTE_USER#</RemoteUser>" +
            "<UserAgent>#HTTP_USER_AGENT#</UserAgent>" +
            "<Cookie>#HTTP_COOKIE#</Cookie>" +
            "</AuditInfo>";

        public string GetUserAuditInfo()
        {
            string remoteAddres = SecurityElement.Escape(_request.ServerVariables["REMOTE_ADDR"]);
            string remoteHost = SecurityElement.Escape(_request.ServerVariables["REMOTE_HOST"]);
            string remoteUser = SecurityElement.Escape(_request.ServerVariables["REMOTE_USER"]);
            string userAgent = SecurityElement.Escape(_request.ServerVariables["HTTP_USER_AGENT"]);
            string userCookie = SecurityElement.Escape(_request.ServerVariables["HTTP_COOKIE"]);

            var auditXML = new StringBuilder(AUDIT_XML_TEMPLATE);
            auditXML.Replace("#REMOTE_ADDR#", remoteAddres);
            auditXML.Replace("#REMOTE_HOST#", remoteHost);
            auditXML.Replace("#REMOTE_USER#", remoteUser);
            auditXML.Replace("#HTTP_USER_AGENT#", userAgent);
            auditXML.Replace("#HTTP_COOKIE#", userCookie);
            return auditXML.ToString();
        }

        public string GetUserAuditInfoAlt()
        {
            IDictionary<string,string> auditInfoDictionary = new Dictionary<string, string>();

            auditInfoDictionary.Add("RemoteAddress", SecurityElement.Escape(_request.ServerVariables["REMOTE_ADDR"]));
            auditInfoDictionary.Add("RemoteHost", SecurityElement.Escape(_request.ServerVariables["REMOTE_HOST"]));
            auditInfoDictionary.Add("RemoteUser", SecurityElement.Escape(_request.ServerVariables["REMOTE_USER"]));
            auditInfoDictionary.Add("UserAgent", SecurityElement.Escape(_request.ServerVariables["HTTP_USER_AGENT"]));
            auditInfoDictionary.Add("Cookie", SecurityElement.Escape(_request.ServerVariables["HTTP_COOKIE"]));

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