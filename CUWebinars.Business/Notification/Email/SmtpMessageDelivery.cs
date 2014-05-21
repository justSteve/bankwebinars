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
            }

            using (var smtp = new SmtpClient())
            {
                smtp.Timeout = 5000;

                mailMessage.To.Add(new MailAddress("all.of.us@ttstrain.com"));
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
