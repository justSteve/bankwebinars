using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using HtmlAgilityPack;
using Newtonsoft.Json;
using Ninject.Extensions.Logging;

namespace CUWebinars.Web.Core
{
    public class ParseMandrillMsg
    {
        private readonly ILogger _logger;


        public
            ParseMandrillMsg(ILogger logger)
        {
            _logger = logger;
        }

        public static Dictionary<string, string> ParseAcs(string _doc, string orderDate)
        {
            HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();

            doc.LoadHtml(_doc);

            if (doc.ParseErrors != null && doc.ParseErrors.Count() > 0)
            {
                // Handle any parse errors as required

                //return true;
            }
            else
            {
                if (doc.DocumentNode != null)
                {
                    Dictionary<string, string> ACSOrderDictionary = new Dictionary<string, string>
                    {
                        {"BillingContact", ""},
                        {"CreditCard", ""},
                        {"EmailAddress", ""},
                        {"EmailAddressforCreditCardReceipt", ""},
                        {"Ext", ""},
                        {"State", ""},
                        {"AffiliateID", "62"},
                        {"FirstName", ""},
                        {"LastName", ""},
                        {"Company", ""},
                        {"Title", ""},
                        {"Email", ""},
                        {"Phone", ""},
                        {"Country", ""},
                        {"StreetorP.O.Box", ""},
                        {"City", ""},
                        {"State/Province/Region", ""},
                        {"Zip/PostalCode", ""},
                        {"DeliveryType", ""},
                        {"WebinarTitle", ""},
                        {"CourseNumber", ""},
                        {"WebinarDate", ""},
                        {"WebinarTime(EasternTime)", ""},
                        {"CourseDeliveryType", ""},
                        {"CoursePrice", ""},
                        {"BankWebID", ""},
                        {"PaymentMethod", ""},
                        {"CompanyBillingInformation", ""},
                        {"ZeroValue", ""},
                        {"DateSubmittedToACS", orderDate},
                        {"AdditionalLocationsString", ""}

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

                            var flag = "";
                            var sb = new StringBuilder("Importer Values" + Environment.NewLine);

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

                                if (ACSOrderDictionary.ContainsKey(name))
                                {
                                    ACSOrderDictionary[name] = value;
                                }
                                else
                                {
                                    if (name.EndsWith("LocationEmail"))
                                    {
                                        ACSOrderDictionary["AdditionalLocationsString"] += value + ",";
                                    }
                                    //_logger.Warn("Keyname not known: " + name + " value:" + value);
                                }
                                values[i] = value;
                                names[i] = name;
                            }
                            if (flag != "")
                            {
                                var aHolder = "";
                                //_logger.Warn("WebJob.EmailParser importer error: " + flag + ACSOrderDictionary["email"]);
                            }
                            else
                            {
                                foreach (var line in ACSOrderDictionary)
                                {
                                    sb.Append(line.Key + " : " + line.Value + Environment.NewLine);
                                }
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
                            //_logger.Warn("ERROR Parsing Importer Loop: " + ex);
                        }
                        return ACSOrderDictionary;
                    }
                }
                else
                {
                    return null;
                }
            }
            return null;

        }
    }
}