using System;
using System.Configuration;
using System.Net.Configuration;
using System.Net.Mail;

namespace CUWebinars.Business.Notification.Email
{
    public class SmtpMessageDelivery : INotificationDelivery
    {
        public void Notify(INotificationMessage notificationMessage)
        {
            var mailMessage = new MailMessage();

            if (String.IsNullOrWhiteSpace(notificationMessage.From))
            {
                var smtp = ConfigurationManager.GetSection("system.net/mailSettings/smtp") as SmtpSection;
                notificationMessage.From = smtp.From;
                notificationMessage.To = "david@dave.com";
            }

            using (var smtp = new SmtpClient())
            {
                smtp.Timeout = 5000;
                try
                {
                    mailMessage.From = new MailAddress(notificationMessage.From);
                    mailMessage.To.Add(new MailAddress(notificationMessage.To));
                    mailMessage.Subject = notificationMessage.Subject;
                    mailMessage.Body = notificationMessage.Body;
                    mailMessage.IsBodyHtml = true;
                    smtp.Send(mailMessage);
                }
                catch (SmtpException e)
                {
                    //Tracing.Error("[SmtpMessageDelivery.Send] SmtpException: " + e.Message);
                }
                catch (Exception e)
                {
                    //Tracing.Error("[SmtpMessageDelivery.Send] Exception: " + e.Message);
                }
            }

        }
    }
}
