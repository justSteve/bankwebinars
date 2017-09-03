using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration.Configuration;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using CUWebinars.Web.Models;
using CUWebinars.Web.Models.Importers;
using HtmlAgilityPack;
using Newtonsoft.Json;
using Ninject.Extensions.Logging;

namespace CUWebinars.Web.Core
{
    public class ParseMandrillMsg
    {
        private static ILogger _logger;


        public
            ParseMandrillMsg(ILogger logger)
        {
            _logger = logger;
        }

        public static IncomingEmailModel ParseIncomingMessages(string _doc, string orderDate)
        {

            IncomingEmailModel model = new IncomingEmailModel();
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

                                }

                                values[i] = value;
                                names[i] = name;
                            }
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

        public static MigrateOrderModel ParseConfSem(string _doc, string orderDate)
        {

            var attendeeBlockStart = _doc.IndexOf("Ship to:") + "Ship to:".Length;
            var attendeeBlockEnd = _doc.IndexOf("Bill to:");

            string[] attendeeBlock = _doc.Substring(attendeeBlockStart, attendeeBlockEnd).Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None);

            var model = new MigrateOrderModel();

            var dateSubmitted = orderDate;

            HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();

            doc.LoadHtml(_doc);

            if (doc.ParseErrors != null && doc.ParseErrors.Count() > 0)
            {
                // Handle any parse errors as required
                model.LoggerNotes = "ParseConfSem Errors: ";
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
                            var a = 21;
                        }
                        catch (Exception ex)
                        {
                            model.LoggerNotes += "ParseConfSem FatalExecption: " + ex.Message;
                            _logger.FatalException("IncomingParseConfSem", ex);

                        }
                        return model;
                    }
                }
                else
                {
                    model.LoggerNotes += "ParseConfSem Returned Null! ";
                    _logger.Warn("IncomingConfSem is null");
                    return null;
                }
            }

            return null;

        }

        public static ImportOrderForAcsModel ParseAcs(string _doc, string orderDate)
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

        public static string ParseNotiActionResult(string _doc)
        {


            return null;
        }
    }
}

