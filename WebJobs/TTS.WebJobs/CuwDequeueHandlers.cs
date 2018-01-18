using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using CUWebinars.Business.Notification.Email;
using HtmlAgilityPack;
using log4net;
using Mandrill;
using Mandrill.Model;
using Microsoft.Azure.WebJobs;
using Newtonsoft.Json;

namespace TTS.WebJobs
{

    public class CuwDequeueHandlers
    {
        public static ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        public static string tenant = ConfigurationManager.AppSettings["Tenant"];
        public static string tenantURL = ConfigurationManager.AppSettings["TenantURL"];

        public static void DequeueOrderConfirmMultiMessages(
            [QueueTrigger("%NotificationsQueue%")] NotificationMessage notificationMessage, // ** QueueTrigger is NotificationsQueue key/value in App.Config
            [Blob("tts-orderconfmultimsg/{PersistedName}")] out string messageContent, // magically persists whatever is placed in the messageContent variable to blob @ that location
            int dequeueCount,
            TextWriter log)
        {

            ILog _logger = LogManager.GetLogger(typeof(CuwDequeueHandlers));

            messageContent = notificationMessage.Body; // output param which magically persists messageContent text to blob location specified above

            var orderId = ParseOrderId(notificationMessage.Body);
            try
            {
                if (!string.IsNullOrWhiteSpace(notificationMessage.PersistedName))
                {
                    Console.WriteLine("Processing {0} email: {1}", orderId, notificationMessage.To);
                }
                
                var destinationEmailAddress = notificationMessage.To;
                var timeStamp = BuildUtcNowAsCts();
                var mailMessage = new MandrillMessage();

                if (string.IsNullOrWhiteSpace(notificationMessage.From)) // needs to be from a "verified" sending domain in Mandrill
                {
                    notificationMessage.From = ConfigurationManager.AppSettings["FallbackFromAddress"];
                    
                }

                //    try
                //    {

                //  Set this AppSetting in App.Config to something other than 'live' when testing i.e. notlive
                if (ConfigurationManager.AppSettings["EmailSendingMode"] != "live")
                {
                    Console.WriteLine("Running in TestMode {0} email: {1}", orderId, notificationMessage.To);
                    _logger.Info("Running in TestMode "+orderId+"email: "+ notificationMessage.To);
                    destinationEmailAddress = ConfigurationManager.AppSettings["TestEmailAddress"]; // would be cool if this supported multiple addresses
                }

                mailMessage.To.Add(new MandrillMailAddress(destinationEmailAddress));

                mailMessage.FromEmail = notificationMessage.From;
                mailMessage.ReplyTo = "ReplyTo-" + notificationMessage.From;

                var _addresses = "";
                if (ConfigurationManager.AppSettings["EmailSendingMode"] == "live"// only add additional emails when it's in live mode
                    && !ReferenceEquals(null, notificationMessage.Addresses)
                    && notificationMessage.Addresses.Any())
                {
                    foreach (string address in notificationMessage.Addresses)
                    {
                        mailMessage.To.Add(
                            new MandrillMailAddress()
                            {
                                Email = address,
                                Type = MandrillMailAddressType.Cc
                                // Type seems to pass through correctly but ends up sending as a stand-alone "To" message
                                //  , seems related to the "Expose the list of recipients when sending to multiple addresses" 
                                //  (unchecked when I was testing) Account | Setting in Mandrill
                            });
                        _addresses += address + ", ";
                    }
                }

                //_logger.Info(string.Format("MandrillNoti tags {0}", notificationMessage.Tags.ToString()));

                mailMessage.Subject = notificationMessage.Subject;
                if (mailMessage.Subject.Contains("ERROR!"))
                {
                    mailMessage.Text = notificationMessage.Body;
                }
                else
                {
                    mailMessage.Html = notificationMessage.Body;
                }
                var api = new MandrillApi("h36RBLZIZGmKilVQj1xY-g"); // move to config (and encrypt?)
                var result = api.Messages.SendAsync(mailMessage);

                // not sure if we need to do this, they will show up in the Mandrill control panel as well
                // process results, r.Status should be Sent, ...
                //if (notificationMessage.Body.Contains("Checklist"))
                //{
                //    foreach (MandrillSendMessageResponse r in result)
                //    {
                //        System.Threading.Thread.Sleep(5000);
                //        System.Diagnostics.Debug.WriteLine(r.Status + " " + r.RejectReason);
                //        messageContent += "<br />To: " + r.Email + ", Mandrill Status: " + r.Status + " " +
                //                          r.RejectReason;
                //        var sendResult = SendResultToProductionEndpoint(notificationMessage, r);

                //        log.WriteLine("Connection Checklist Mandrill sender hit Order {0} on: {1} with: {2}", sendResult, _addresses.TrimEnd(' ').TrimEnd(','), JsonConvert.SerializeObject(result).ToString());
                //    }
                //}
            }
            catch (Exception exception)
            {
                _logger.Error("DequeueOrderConfirmMultiMessages", exception);
                var mailMessage = new MandrillMessage();
                mailMessage.To = new List<MandrillMailAddress>{new MandrillMailAddress {
                    Email = "2afda898.ttstrain.com@amer.teams.ms"
                    , Name = "ErrorHandler"
                    , Type = MandrillMailAddressType.To}};
                mailMessage.FromEmail = "errors@ttsregistrations.com";
                mailMessage.Subject = "Exception Occurred from Noti Webjob" ;
                // now log to Azure log in the blob container "azure-webjobs-hosts/output-logs"
                var errorDetails = new StringBuilder("Operation Failed");
                errorDetails.AppendFormat(
                    "Message subject: {0} {1}{2}",
                    notificationMessage.Subject, Environment.NewLine, notificationMessage.Body
                    );
                errorDetails.AppendFormat(
                    "A top-level exception occurred in DequeueOrderConfirmMultiMessages {0}",
                    Environment.NewLine
                    );

                errorDetails.AppendFormat("Exception message {0}{1}", exception.Message, Environment.NewLine);
                errorDetails.AppendFormat("StackTrace {0}{1}", exception.StackTrace, Environment.NewLine);
                var errorResult = new MandrillSendMessageResponse
                {
                    RejectReason = errorDetails.ToString(),
                    Email = notificationMessage.To,
                    Status = MandrillSendMessageResponseStatus.Invalid,
                    Id = "error at sender"

                };
                var sendResult = SendResultToProductionEndpoint(notificationMessage, errorResult);
                mailMessage.Html = errorDetails.ToString() + "<br" + notificationMessage.Body;

                var api = new MandrillApi("h36RBLZIZGmKilVQj1xY-g");
                // move to config (and encrypt?)
                
                var result = api.Messages.SendAsync(mailMessage);

                log.WriteLine(errorDetails.ToString());
                log.Flush();
            }
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
        private static string SendResultToProductionEndpoint(NotificationMessage notificationMessage, MandrillSendMessageResponse result)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                if (result != null)
                {
                    var orderId = ParseOrderId(notificationMessage.Body);
                    //matches fields in production's NotiResultsModel class
                    var postForm = "Email=" + result.Email;
                    postForm += "&Status=" + result.Status;
                    postForm += "&RejectReason=" + result.RejectReason;
                    postForm += "&Id=" + result.Id;
                    postForm += "&idOrder=" + orderId;
                    _logger.Info("Incoming Notification Handler Starts orderId: " + orderId + " addresses: " + notificationMessage.Addresses + " To: " + notificationMessage.To + " CC: " + notificationMessage.CC);


                    //WebRequest req = WebRequest.Create("http://552890de.ngrok.io/order/notiResults");
                    WebRequest req = WebRequest.Create(tenantURL + "/order/notiResults");

                    byte[] send = Encoding.Default.GetBytes(postForm);
                    req.Method = "POST";
                    req.ContentType = "application/x-www-form-urlencoded";
                    req.ContentLength = send.Length;

                    Stream sout = req.GetRequestStream();
                    sout.Write(send, 0, send.Length);
                    sout.Flush();
                    sout.Close();

                    WebResponse res = req.GetResponse();
                    StreamReader sr = new StreamReader(res.GetResponseStream());
                    string returnvalue1 = sr.ReadToEnd();
                    var importResult = returnvalue1;
                    _logger.Info(importResult);

                    return orderId;
                }
            }
            catch (Exception e)
            {
                _logger.FatalFormat("NotiSendResultsToProductionEndpoint");
                Console.WriteLine("SendResultToProductionEndpoint" + e.Message);
                return "error returned: " + e.Message;
            }

            return "null";
        }

        private static string ParseOrderId(string notificationMessageBody)
        {
            HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();

            doc.LoadHtml(notificationMessageBody);

            var loggerNotes = "";
            if (doc.ParseErrors != null && doc.ParseErrors.Count() > 0)
            {
                loggerNotes = "ParseNotiActionResult ParseErrors: ";
                // Handle any parse errors as required
                foreach (var error in doc.ParseErrors)
                {
                    loggerNotes += error.Reason;
                }
                return "ErrorInParseOrderId: " + loggerNotes;
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
                            var findingId = bodyNode.SelectNodes("//td")
                                .SkipWhile(g => !g.InnerText.Contains("Order ID:"))
                                .Select(s => s).Skip(2).FirstOrDefault();
                            //_logger.Info("NotiHandler found Orderid: " + findingId.InnerText);
                            return findingId.InnerText;
                        }
                        catch (Exception ex)
                        {
                            loggerNotes += "ParseAcs FatalExecption: " + ex.Message;
                            _logger.Fatal("IncomingParseACS", ex);

                            return "ParseNotiActionResult fatal " + ex.Message;
                        }
                    }
                }
                else
                {
                    loggerNotes += "ParseAcs Returned Null! ";
                    //_logger.Warn("Incoming | AcsModel is null");
                    return loggerNotes;
                }
            }
            return null;

        }

        private static DateTime BuildUtcNowAsCts()
        {
            DateTime timeUtc = DateTime.UtcNow;
            return TimeZoneInfo.ConvertTimeFromUtc(timeUtc, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"));

        }
    }
}
