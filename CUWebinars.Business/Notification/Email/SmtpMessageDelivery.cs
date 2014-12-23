using System;
using System.Collections.Specialized;
using System.Configuration;
using System.Net.Configuration;
using System.Net.Mail;
using CUWebinars.Business.Core.Tracing;

namespace CUWebinars.Business.Notification.Email
{
    public class SmtpMessageDelivery : INotificationDelivery
    {
        private readonly NameValueCollection _appSettings = ConfigurationManager.AppSettings;
        public void Notify(INotificationMessage notificationMessage)
        {
            var timeStamp = DateTime.Now;
            var mailMessage = new MailMessage();
            var tmpMsg = string.Empty;

            if (string.IsNullOrWhiteSpace(notificationMessage.From))
            {
                var smtp = ConfigurationManager.GetSection("system.net/mailSettings/smtp") as SmtpSection;
                notificationMessage.From = smtp.From;
            }

            using (var smtp = new SmtpClient())
            {
                smtp.Timeout = 5000;
                
                string destinationEmailAddress = notificationMessage.To;

                //  Set this AppSetting in Web.Config to something other than live when testing e.g. notlive
                if (_appSettings["EmailSendingMode"] != "live")
                {
                    destinationEmailAddress = _appSettings["TestEmailAddress"];
                    mailMessage.To.Add(new MailAddress(_appSettings["TestEmailAddress2"]));
                }

                mailMessage.To.Add(destinationEmailAddress);
                
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
                    Tracer.Error("Error MailerArgumentNullException: message is null ");
                }
                catch (InvalidOperationException ex)
                {
                    tmpMsg = "ERROR MailerInvalidOperationException: " + notificationMessage.To;
                    tmpMsg += " Subject: " + notificationMessage.Subject;
                    tmpMsg += " ErrorMsg: " + ex.Message;
                    tmpMsg += " Timestamp was: " + timeStamp;
                    Tracer.Error(tmpMsg);

                }
                catch (SmtpFailedRecipientsException)
                {
                    tmpMsg = "ERROR MailerSmtpFailedRecipientsException: " + notificationMessage.To;
                    tmpMsg += " Subject: " + notificationMessage.Subject;
                    tmpMsg += " Timestamp was: " + timeStamp;
                    Tracer.Error(tmpMsg);
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
                }

                tmpMsg = "MailerSent: " + notificationMessage.To;
                tmpMsg += " Subject: " + notificationMessage.Subject;
                tmpMsg += " Timestamp was: " + timeStamp;

                Tracer.Information(tmpMsg);

            }

        }
    }
}
