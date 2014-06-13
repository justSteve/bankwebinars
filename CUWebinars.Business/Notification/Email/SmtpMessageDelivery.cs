using System;
using System.Configuration;
using System.Net.Configuration;
using System.Net.Mail;
using CUWebinars.Business.Core.Tracing;

namespace CUWebinars.Business.Notification.Email
{
    public class SmtpMessageDelivery : INotificationDelivery
    {
        public void Notify(INotificationMessage notificationMessage)
        {
            var timeStamp = DateTime.Now;
            var mailMessage = new MailMessage();
            var tmpMsg = string.Empty;

            if (String.IsNullOrWhiteSpace(notificationMessage.From))
            {
                var smtp = ConfigurationManager.GetSection("system.net/mailSettings/smtp") as SmtpSection;
                notificationMessage.From = smtp.From;
            }

            using (var smtp = new SmtpClient())
            {
                smtp.Timeout = 5000;

                mailMessage.To.Add(new MailAddress("steve@ttstrain.com"));
                //mailMessage.To.Add(new MailAddress(notificationMessage.To));
                //mailMessage.To.Add(new MailAddress("58c8ac9@emailtests.com"));
                //mailMessage.ReplyToList.Add(new MailAddress(notificationMessage.ReplyTo));
                //if (notificationMessage.Addresses != null)
                //{
                //    foreach (var address in notificationMessage.Addresses)
                //    {
                //        mailMessage.To.Add(new MailAddress(address));
                //    }
                //}
                //else
                //{
                //    mailMessage.To.Add(new MailAddress(notificationMessage.To));
                //}

                try
                {
                    mailMessage.From = new MailAddress(notificationMessage.From);
                    
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
                    Tracer.Error(tmpMsg);
                }
                catch (Exception exception)
                {
                    System.Threading.Thread.Sleep(2000);
                    tmpMsg = "ERROR MailerSmtpException: " + notificationMessage.To;
                    tmpMsg += " Subject: " + notificationMessage.Subject;
                    tmpMsg += " ErrorMsg: " + exception.Message;
                    tmpMsg += " Timestamp was: " + timeStamp;
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
