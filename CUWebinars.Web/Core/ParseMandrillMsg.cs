using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using CUWebinars.Business.Models;
using CUWebinars.Web.Models;
using CUWebinars.Web.Models.Importers;
using HtmlAgilityPack;
using NameParser;
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

        public static ParseOrderModel ParseConfSem(string _doc, string orderDate)
        {
            try
            {

                var model = new ParseOrderModel();
                var attendeeBlockStart = _doc.IndexOf("Ship To:", StringComparison.Ordinal) + "Ship To:".Length;
                var attendeeBlockEnd = _doc.IndexOf("Bill To:", StringComparison.Ordinal);

                var attendeeBlock = _doc.Substring(attendeeBlockStart, attendeeBlockEnd - attendeeBlockStart);

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
                    _logger.Warn("IncomingParseConfSem Errors: " + model.LoggerNotes);
                }
                if (doc.DocumentNode != null)
                {
                    try
                    {

                        var splitBlock = doc.DocumentNode.SelectNodes("//table")[3].InnerText.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None);
                        //var splitBlock = attendeeBlock.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None);
                        var name = new HumanName(splitBlock[5]);

                        model.Email = splitBlock[12];
                        model.FirstName = name.First;
                        model.LastName = name.Last;

                        model.Title = splitBlock[6];
                        model.Institution = splitBlock[7];

                        model.BillingAddress = new Address();
                        model.ShippingAddress = new Address();

                        model.BillingAddress.AddressType = "Billing";
                        model.BillingAddress.Name = name.FullName;
                        model.BillingAddress.StreetAddress = splitBlock[8];
                        model.BillingAddress.City = splitBlock[9].Split(',')[0];
                        model.BillingAddress.State = splitBlock[9].Split(',')[1].Split(' ')[1];
                        model.BillingAddress.Zip = splitBlock[9].Split(',')[1].Split(' ')[2];
                        model.BillingAddress.Country = splitBlock[10];
                        model.BillingAddress.Phone = splitBlock[11];

                        model.ShippingAddress.AddressType = "Shipping";
                        model.ShippingAddress.Name = name.FullName;
                        model.ShippingAddress.StreetAddress = splitBlock[8];
                        model.ShippingAddress.City = splitBlock[9].Split(',')[0];
                        model.ShippingAddress.State = splitBlock[9].Split(',')[1].Split(' ')[1];
                        model.ShippingAddress.Zip = splitBlock[9].Split(',')[1].Split(' ')[2];
                        model.ShippingAddress.Country = splitBlock[10];
                        model.ShippingAddress.Phone = splitBlock[11];

                        var regData = doc.DocumentNode.SelectNodes("//table")[4].InnerText;

                        var regDataBlockStart = regData.IndexOf("Amount", StringComparison.Ordinal) + "Amount".Length;
                        var regDataBlockEnd = regData.IndexOf("Shipping", StringComparison.Ordinal);

                        var regDataBlock = regData.Substring(regDataBlockStart, regDataBlockEnd - regDataBlockStart)
                            .Replace(". Sponsored by: BankWebinars.com: Registration - ", "|")
                            .Replace(": Live Teleconference, ", "|");

                        var po = regDataBlock.Split(':')[0];

                        model.EventTitle = regDataBlock.Split('|')[0].Substring(po.Length + ": ".Length);
                        model.EventDate = regDataBlock.Split('|')[1].Split(';')[1].Trim();
                        model.EventTime = regDataBlock.Split('|')[1].Split(';')[0].Trim();
                        model.RegTypeAsString = regDataBlock.Split('|')[2];

                        return model;

                    }
                    catch (Exception ex)
                    {
                        model.LoggerNotes += "ParseConfSem FatalExecption: " + ex.Message;
                        _logger.FatalException("IncomingParseConfSem", ex);

                    }
                    return model;

                }
                else
                {
                    model.LoggerNotes += "ParseConfSem Returned Null! ";
                    _logger.Warn("IncomingConfSem is null");
                    return null;
                }

            }
            catch (Exception e)
            {
                _logger.FatalException("ParseConfSem", e);
                throw;
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

        public static MigrateOrderModel ConvertToMigrator(ParseOrderModel parsedOrder)
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


            return model;
        }

        public static ParseOrderModel ParseRateWatch(string _doc)
        {
            var model = new ParseOrderModel();
            try
            {
                string[] splitBlock = "".Split(',');

                var regDataBlockStart = _doc.IndexOf("*Name:*", StringComparison.Ordinal) + "*Name:*".Length;
                var regDataBlockEnd = _doc.IndexOf("*Email*", StringComparison.Ordinal);

                var parsedName = _doc.Substring(regDataBlockStart, regDataBlockEnd - regDataBlockStart).Replace("\n", "");


                regDataBlockStart = _doc.IndexOf("*Email*", StringComparison.Ordinal) + "*Email*".Length;
                regDataBlockEnd = _doc.IndexOf("*Company Name:*", StringComparison.Ordinal);
                var parsedEmail = _doc.Substring(regDataBlockStart, regDataBlockEnd - regDataBlockStart).Replace("\n", "");


                regDataBlockStart = _doc.IndexOf("*Company Name:*", StringComparison.Ordinal) + "*Company Name:*".Length;
                regDataBlockEnd = _doc.IndexOf("*Title*", StringComparison.Ordinal);
                var parsedCompanyName = _doc.Substring(regDataBlockStart, regDataBlockEnd - regDataBlockStart).Replace("\n", "");

                regDataBlockStart = _doc.IndexOf("*Title*", StringComparison.Ordinal) + "*Title*".Length;
                regDataBlockEnd = _doc.IndexOf("*Address:*", StringComparison.Ordinal);
                var parsedTitle = _doc.Substring(regDataBlockStart, regDataBlockEnd - regDataBlockStart).Replace("\n", "");

                regDataBlockStart = _doc.IndexOf("*Address:*", StringComparison.Ordinal) + "*Address:*".Length;
                regDataBlockEnd = _doc.IndexOf("*Phone Number:*", StringComparison.Ordinal);
                var parsedAddress = _doc.Substring(regDataBlockStart, regDataBlockEnd - regDataBlockStart).TrimStart('\n').TrimEnd('\n');

                regDataBlockStart = _doc.IndexOf("*Phone Number:*", StringComparison.Ordinal) + "*Phone Number:*".Length;
                regDataBlockEnd = _doc.IndexOf("*Account Number:*", StringComparison.Ordinal);
                var parsedPhone = _doc.Substring(regDataBlockStart, regDataBlockEnd - regDataBlockStart).TrimStart('\n').TrimEnd('\n');

                regDataBlockStart = _doc.IndexOf("* Event Name: *", StringComparison.Ordinal) + "* Event Name:*".Length;
                regDataBlockEnd = _doc.IndexOf("* Date:*", StringComparison.Ordinal);
                var parsedEventName = _doc.Substring(regDataBlockStart, regDataBlockEnd - regDataBlockStart).TrimStart('\n').TrimEnd('\n');

                regDataBlockStart = _doc.IndexOf("* Date:*", StringComparison.Ordinal) + "* Date:*".Length;
                regDataBlockEnd = _doc.IndexOf("* Time:*", StringComparison.Ordinal);
                var parsedEventDate = _doc.Substring(regDataBlockStart, regDataBlockEnd - regDataBlockStart).TrimStart('\n').TrimEnd('\n');


                regDataBlockStart = _doc.IndexOf("* Time:*", StringComparison.Ordinal) + "* Time:*".Length;
                regDataBlockEnd = _doc.IndexOf("* Event Type:*", StringComparison.Ordinal);
                var parsedEventTime = _doc.Substring(regDataBlockStart, regDataBlockEnd - regDataBlockStart).TrimStart('\n').TrimEnd('\n');


                regDataBlockStart = _doc.IndexOf("* Event Type:*", StringComparison.Ordinal) + "* Event Type:*".Length;
                regDataBlockEnd = _doc.IndexOf("* Total", StringComparison.Ordinal);
                var parsedEventType = _doc.Substring(regDataBlockStart, regDataBlockEnd - regDataBlockStart).TrimStart('\n').TrimEnd('\n').Split('(')[0].TrimEnd(')');

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
                model.BillingAddress.StreetAddress = parsedAddress.Split('\n')[0];
                model.BillingAddress.City = parsedAddress.Split('\n')[1].Split(',')[0];
                model.BillingAddress.State = parsedAddress.Split('\n')[1].Split(',')[1].Split(' ')[1];
                model.BillingAddress.Zip = parsedAddress.Split('\n')[1].Split(',')[1].Split(' ')[2];
                model.BillingAddress.Country = "USA";
                //"9205681401 <(920)%20568-1401>"
                model.BillingAddress.Phone = parsedPhone.Split(' ')[0];

                model.ShippingAddress.AddressType = "Shipping";
                model.ShippingAddress.Name = name.FullName;
                model.ShippingAddress.StreetAddress = parsedAddress.Split('\n')[0];
                model.ShippingAddress.City = parsedAddress.Split('\n')[1].Split(',')[0];
                model.ShippingAddress.State = parsedAddress.Split('\n')[1].Split(',')[1].Split(' ')[1];
                model.ShippingAddress.Zip = parsedAddress.Split('\n')[1].Split(',')[1].Split(' ')[2];
                model.ShippingAddress.Country = "USA";
                //"9205681401 <(920)%20568-1401>"
                model.ShippingAddress.Phone = parsedPhone.Split(' ')[0];

                

                model.EventTitle = parsedEventName.Trim();
                model.EventDate = parsedEventDate.Trim();
                model.EventTime = parsedEventTime.Trim();
                model.RegTypeAsString = parsedEventType;

                return model;

            }
            catch (Exception ex)
            {
                model.LoggerNotes += "ParseRateWatch FatalExecption: " + ex.Message;
                _logger.FatalException("IncomingParserateWatch", ex);

            }
            return model;


        }
    }
}

