using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Web.Mvc;
using CUWebinars.Business.Models;
using CUWebinars.Web.Models;
using CUWebinars.Web.Models.Importers;
using GemBox.Document;
using Newtonsoft.Json.Linq;

namespace CUWebinars.Web.Helpers
{
    public interface IAppHelper
    {
        /// <summary>
        /// Retrieves the city state zip via URL parameter and stored in the ASP.NET Session
        /// </summary>
        /// <returns>City | State based on input zipcode</returns>
        string GetCityStateFromZip(int zipCode);

        SelectList  GetListOfAffiliates(int selectedValue);
        SessionStartInfo GetSessionStartInfo();
        string GetUserAuditInfo();
        List<string> InstitutionAutoComplete(string name, string zip);
        string GetAffiliateName(int idAffiliate);
        IList<string> ServerSideEmailCheck(IList<string> emails);
        IList<AdditionalLocation> CheckAdditionalLocationsForValidEmail(IList<AdditionalLocation> additionalLocations);
        bool CheckIsEmailValid(string email);
        long ToUnixTimespan(DateTime myEventStart, TimeZoneInfo findSystemTimeZoneById);
        
        bool BuildWebinarOrdersInvoiceRow(Order order, StringBuilder discountNotes, out OrderRow row, out string price, ref int rowNumber, out string percent, ref int totalNumberDiscounts);
        bool BuildAdjustedOrderInvoiceRow(Order order, string adjustmentDirection, OrderRow row, Dictionary<string, JToken> dict, StringBuilder discountNotes, out decimal adjustedTotal, out decimal adjustedRoyalty, ref int totalNumberDiscounts);
        void Document_FieldMerging(DocumentModel document, Affiliate affiliate);
        bool BuildPostEventOrdersInvoiceRows(Order order, StringBuilder discountNotes, out OrderRow row, out string price, ref int rowNumber, out string percent, ref int totalNumberDiscounts);
        string CleanHtmlCodesAndLogo(string body, string tenantLogo, string addloccost, string specialMsg);
        NotificationMessageFields BuildNotiFields(Order order, string hasCCAddress);
        void ScheduleConnInfoSenderAudit(Webinar idWebinar);
        string ReplaceMergeCodes(string messageBodyHtml, Affiliate affiliateId);
        string ReplaceTimeString(Webinar webinar, USTimeZone timeZone);
        object FuzzyMatch(string parsedOrderEventTitle, IEnumerable<string> @select);
        ParseOrderModel ParseConfSem(string s, string toString);
        ParseOrderModel ParseRateWatch(string _doc);
        MigrateOrderModel ConvertToMigrator(ParseOrderModel parsedOrder);
        ImportOrderForAcsModel ParseAcs(string _doc, string toString);
        Address ParseAddress(string address);
        string ParsePhone(string address);
        string FindChangedRegTypes(Order order);
    }
}