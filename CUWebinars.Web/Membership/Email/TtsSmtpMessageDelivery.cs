using BrockAllen.MembershipReboot;
using System;
using System.Configuration;
using System.Net.Configuration;
using System.Net.Mail;

namespace CUWebinars.Web.Membership.Email
{
    public class TtsSmtpMessageDelivery : IMessageDelivery
    {
        public void Send(Message msg)
        {
            var mailMessage = new MailMessage();

            if (string.IsNullOrWhiteSpace(msg.From))
            {
                var smtp = ConfigurationManager.GetSection("system.net/mailSettings/smtp") as SmtpSection;
                msg.From = smtp.From;
            }

            using (var smtp = new SmtpClient())
            {
                smtp.Timeout = 5000;

                mailMessage.From = new MailAddress(msg.From);
                //mailMessage.To.Add(new MailAddress(msg.To));
                mailMessage.To.Add(new MailAddress("steve@ttstrain.com"));
                mailMessage.Subject = msg.Subject;
                mailMessage.Body = msg.Body;
                mailMessage.IsBodyHtml = true;

                try
                {
                    smtp.Send(mailMessage);
                }
                catch (SmtpException e)
                {
                    Tracing.Error("[SmtpMessageDelivery.Send] SmtpException: " + e.Message);
                }
                catch (Exception e)
                {
                    Tracing.Error("[SmtpMessageDelivery.Send] Exception: " + e.Message);
                }
            }
        }
    }
}
