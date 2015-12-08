using CUWebinars.Business.Notification.Formatters;
using CUWebinars.Business.Notification.ViewModel;
using Ninject.Extensions.Logging;
using System;
using System.Configuration;
using System.Net.Configuration;
using System.Net.Mail;
using CUWebinars.Business.Constants;
using System.Collections.Generic;
using System.Linq;

namespace CUWebinars.Business.Notification.Email
{
    public class AdhocNotificationDelivery : IAdhocNotificationDelivery
    {
        private readonly ILogger _logger;
        private readonly IFormatter _generalFormatter;

        public AdhocNotificationDelivery(ILogger logger, IFormatter generalFormatter)
        {
            _logger = logger;
            _generalFormatter = generalFormatter;
        }

        public void Notify(IAdhocNotificationMessage adhocNotificationMessage)
        {
            var model = new AdhocNotificationSubmittedViewModel()
            {
                Body = adhocNotificationMessage.Body,
                Subject = adhocNotificationMessage.Subject
            };

            var notificationMessage = _generalFormatter.FormatV2(model, "~/Notification/Templates/AdhocNotification.cshtml");
            notificationMessage.Addresses = adhocNotificationMessage.Recipients.ToList();

            SendMessage(notificationMessage);

        }

        private void SendMessage(INotificationMessage notificationMessage)
        {
            _logger.Info("Sending Ad Hoc notification message!");
            _logger.Info(string.Format("Message Subject is:{0}, PersistedName is:{1}",
                notificationMessage.Subject, notificationMessage.PersistedName ?? "null"));

            var timeStamp = DomainConstants.BuildUtcNowAsCts;
            var mailMessage = new MailMessage();
            var tmpMsg = string.Empty;

            if (string.IsNullOrWhiteSpace(notificationMessage.From))
            {
                var smtp = ConfigurationManager.GetSection("system.net/mailSettings/smtp") as SmtpSection;
                notificationMessage.From = smtp.From;
            }

            using (var smtp = new SmtpClient())
            //using (var smtp = new SmtpClient(ConfigurationManager.AppSettings["smtp.host"], int.Parse(ConfigurationManager.AppSettings["smtp.port"])))
            {
                smtp.Timeout = 5000;
                //smtp.Credentials = new NetworkCredential(ConfigurationManager.AppSettings["smtp.userName"], ConfigurationManager.AppSettings["smtp.password"]);
                //smtp.EnableSsl = true;
                //smtp.UseDefaultCredentials = false;
                //smtp.DeliveryMethod = SmtpDeliveryMethod.Network;

                //  Set this AppSetting in App.Config to something other than 'live' when testing i.e. notlive
                if (ConfigurationManager.AppSettings["EmailSendingMode"] == "live")
                {
                    // mailMessage.Bcc.Add(new MailAddress("participants@" + ConfigurationManager.AppSettings["TenantDomain"] + ".com", "Webinar Participants"));
                    foreach (string address in notificationMessage.Addresses)
                    {
                        mailMessage.Bcc.Add(new MailAddress(address));
                    }
                }
                else
                {
                    string testEmail1 = ConfigurationManager.AppSettings["TestEmailAddress"];
                    if (!string.IsNullOrWhiteSpace(testEmail1))
                        mailMessage.Bcc.Add(new MailAddress(testEmail1));

                    string testEmail2 = ConfigurationManager.AppSettings["TestEmailAddress2"];
                    if (!string.IsNullOrWhiteSpace(testEmail2))
                        mailMessage.Bcc.Add(new MailAddress(testEmail2));

                    mailMessage.Bcc.Add(new MailAddress("steve@ttstrain.com"));
                }



                try
                {
                    _logger.Info(string.Format("Sending msg to {0}", notificationMessage.To));

                    mailMessage.From = new MailAddress(notificationMessage.From);
                    mailMessage.Subject = notificationMessage.Subject;
                    mailMessage.Body = notificationMessage.Body;
                    mailMessage.IsBodyHtml = true;
                    smtp.Send(mailMessage);

                    _logger.Info("Message sent successfully!");
                }
                catch (ArgumentNullException)
                {
                    _logger.Error("WriteLine MailerArgumentNullException: message is null ");
                }
                catch (InvalidOperationException ex)
                {
                    tmpMsg = "WriteLine MailerInvalidOperationException: " + string.Join(";", notificationMessage.Addresses);
                    tmpMsg += " Subject: " + notificationMessage.Subject;
                    tmpMsg += " WriteLineMsg: " + ex.Message;
                    tmpMsg += " Timestamp was: " + timeStamp;
                    _logger.Error(tmpMsg);

                }
                catch (SmtpFailedRecipientsException)
                {
                    tmpMsg = "WriteLine MailerSmtpFailedRecipientsException: " + string.Join(";", notificationMessage.Addresses); ;
                    tmpMsg += " Subject: " + notificationMessage.Subject;
                    tmpMsg += " Timestamp was: " + timeStamp;
                    _logger.Error(tmpMsg);

                }
                catch (SmtpException ex)
                {
                    System.Threading.Thread.Sleep(2000);
                    tmpMsg = "WriteLine MailerSmtpException: " + string.Join(";", notificationMessage.Addresses); ;
                    tmpMsg += " Subject: " + notificationMessage.Subject;
                    tmpMsg += " WriteLineMsg: " + ex.Message;
                    tmpMsg += " Timestamp was: " + timeStamp;
                    _logger.Error(tmpMsg);
                }
                catch (Exception exception)
                {
                    System.Threading.Thread.Sleep(2000);
                    tmpMsg = "WriteLine Exception: " + string.Join(";", notificationMessage.Addresses); ;
                    tmpMsg += " Subject: " + notificationMessage.Subject;
                    tmpMsg += " WriteLineMsg: " + exception.Message;
                    tmpMsg += " Timestamp was: " + timeStamp;
                    _logger.Error(tmpMsg);
                }

                tmpMsg = "MailerSent: " + string.Join(";", notificationMessage.Addresses); ;
                tmpMsg += " Subject: " + notificationMessage.Subject;
                tmpMsg += " Timestamp was: " + timeStamp;

                _logger.Info(tmpMsg);
            }
        }
    }
}