using System;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Net.Configuration;
using System.Net.Mail;
using CUWebinars.Business.Core.Tracing;
using Ninject.Extensions.Logging;

namespace CUWebinars.Business.Notification.Email
{
    public class SmtpMessageDelivery : INotificationDelivery
    {
        private readonly ILogger _logger;
        private readonly NameValueCollection _appSettings = ConfigurationManager.AppSettings;

        public SmtpMessageDelivery(ILogger logger)
        {
            _logger = logger;
        }

        public void Notify(INotificationMessage notificationMessage)
        {
            var timeStamp = DateTime.Now;
            var mailMessage = new MailMessage();
            var tmpMsg = string.Empty;
            string destinationEmailAddress = notificationMessage.To;

            //_logger.Info("Sending CUW message!");

            if (string.IsNullOrWhiteSpace(notificationMessage.From))
            {
                var smtp = ConfigurationManager.GetSection("system.net/mailSettings/smtp") as SmtpSection;
                notificationMessage.From = smtp.From;
            }

            using (var smtp = new SmtpClient())
            {
                smtp.Timeout = 5000;
                

                //  Set this AppSetting in Web.Config to something other than live when testing e.g. notlive
                if (_appSettings["EmailSendingMode"] != "live")
                {
                    destinationEmailAddress = _appSettings["TestEmailAddress"];
                    mailMessage.To.Add(new MailAddress(_appSettings["TestEmailAddress2"]));
                }

                mailMessage.To.Add(destinationEmailAddress);

                if (!ReferenceEquals(null, notificationMessage.Addresses) && notificationMessage.Addresses.Any())
                {
                    foreach (var address in notificationMessage.Addresses)
                    {
                        mailMessage.CC.Add(address);
                    }
                }
                
                try
                {
                    mailMessage.From = new MailAddress(_appSettings["TenantEmail"]);
                    
                    mailMessage.Subject = notificationMessage.Subject;
                    mailMessage.Body = notificationMessage.Body;
                    mailMessage.IsBodyHtml = true;
                    smtp.Send(mailMessage);
                }
                catch (ArgumentNullException)
                {
                    Tracer.Warning("MailerArgumentNullException: message is null ");
                    _logger.Fatal("MailerArgumentNullException: message is null ");
                }
                catch (InvalidOperationException ex)
                {
                    tmpMsg = "ERROR MailerInvalidOperationException: " + notificationMessage.To;
                    tmpMsg += " Subject: " + notificationMessage.Subject;
                    tmpMsg += " ErrorMsg: " + ex.Message;
                    tmpMsg += " Timestamp was: " + timeStamp;
                    Tracer.Warning(tmpMsg);
                    _logger.Fatal(tmpMsg);
                }
                catch (SmtpFailedRecipientsException)
                {
                    tmpMsg = "ERROR MailerSmtpFailedRecipientsException: " + notificationMessage.To;
                    tmpMsg += " Subject: " + notificationMessage.Subject;
                    tmpMsg += " Timestamp was: " + timeStamp;
                    Tracer.Warning(tmpMsg);
                    _logger.Fatal(tmpMsg);
                }
                catch (SmtpException ex)
                {
                    System.Threading.Thread.Sleep(2000);
                    tmpMsg = "ERROR MailerSmtpException: " + notificationMessage.To;
                    tmpMsg += " Subject: " + notificationMessage.Subject;
                    tmpMsg += " ErrorMsg: " + ex.Message;
                    tmpMsg += " Timestamp was: " + timeStamp;
                    tmpMsg += " ExceptionType was: SmtpException";
                    tmpMsg += string.Format(" InnerExceptionType was: {0}", ex.InnerException == null ? string.Empty : ex.InnerException.GetType().ToString());

                    Tracer.Error(tmpMsg);
                    _logger.Error(tmpMsg);
                }
                catch (Exception exception)
                {
                    System.Threading.Thread.Sleep(2000);
                    tmpMsg = "ERROR Exception: " + notificationMessage.To;
                    tmpMsg += " Subject: " + notificationMessage.Subject;
                    tmpMsg += " ErrorMsg: " + exception.Message;
                    tmpMsg += " Timestamp was: " + timeStamp;
                    tmpMsg += " ExceptionType was: Exception";
                    tmpMsg += string.Format(" InnerExceptionType was: {0}", exception.InnerException == null ? string.Empty : exception.InnerException.GetType().ToString());

                    Tracer.Error(tmpMsg);
                    _logger.Error(tmpMsg);
                }

                tmpMsg = "MailerSent: " + notificationMessage.To;
                tmpMsg += " Subject: " + notificationMessage.Subject;
                tmpMsg += " Timestamp was: " + timeStamp;

                Tracer.Information(tmpMsg);
                _logger.Info(tmpMsg);
            }

        }
    }
}
