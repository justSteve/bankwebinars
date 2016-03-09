using CUWebinars.Business.Notification.Formatters;
using CUWebinars.Business.Notification.ViewModel;
using Ninject.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net.Configuration;
using System.Net.Mail;
using CUWebinars.Business.Constants;

namespace CUWebinars.Business.Notification.Email
{
    public class OrderNotifierMessageDelivery : IOrderConfirmedNotificationDelivery
    {
        private readonly ILogger _logger;
        private readonly IFormatter _generalFormatter;

        public OrderNotifierMessageDelivery(ILogger logger, IFormatter generalFormatter)
        {
            _logger = logger;
            _generalFormatter = generalFormatter;
        }

        public void Notify(IConfirmOrderMessage confirmOrderMessage)
        {
            var order = confirmOrderMessage.Order;

            var orderSubmittedViewModel = new OrderSubmittedViewModel
            {
                AddPasswordUrl = confirmOrderMessage.AddPasswordUrl,
                ConfirmChangeEmailUrl = confirmOrderMessage.ConfirmChangeEmailUrl,
                Order = confirmOrderMessage.Order,
                OrderGenesis = confirmOrderMessage.OrderGenesis,
                UserCreatedInCart = confirmOrderMessage.UserCreatedInCart,
                UserCreatedOnImport = confirmOrderMessage.UserCreatedOnImport
            };

            var notificationMessage = _generalFormatter.Format(orderSubmittedViewModel, "OrderSubmitted");
            notificationMessage.PersistedName = confirmOrderMessage.PersistedName;


            //notificationMessage.To = order.idAffiliate == 62 ? "steve@ttstrain.com" : order.BillingEmail;

            notificationMessage.To = order.BillingEmail;
            notificationMessage.Bcc = order.Affiliate.ContactEmail;
            
            SendMessage(notificationMessage);
        }

        private void SendMessage(INotificationMessage notificationMessage)
        {
            _logger.Info(string.Format("Message Order Comfirmation Subject is: {0}, PersistedName is:{1}",
                notificationMessage.Subject, notificationMessage.PersistedName ?? "null"));

            var timeStamp = DomainConstants.BuildUtcNowAsCts;
            var mailMessage = new MailMessage();
            var tmpMsg = string.Empty;
            var destinationEmailAddress = notificationMessage.To;
            var destinationEmailAddressAffiliate = notificationMessage.Bcc;

            if (string.IsNullOrWhiteSpace(notificationMessage.From))
            {
                var smtp = ConfigurationManager.GetSection("system.net/mailSettings/smtp") as SmtpSection;
                notificationMessage.From = smtp.From;
            }

            using (var smtp = new SmtpClient())
            //using (var smtp = new SmtpClient(ConfigurationManager.AppSettings["smtp.host"], int.Parse(ConfigurationManager.AppSettings["smtp.port"])))
            {
                smtp.Timeout = 5000;
                
                //  Set this AppSetting in App.Config to something other than 'live' when testing i.e. notlive
                if (ConfigurationManager.AppSettings["EmailSendingMode"] != "live")
                {
                    destinationEmailAddress = ConfigurationManager.AppSettings["TestEmailAddress"];
                    destinationEmailAddressAffiliate = "steve@juststeve.com";
                }

                try
                {
                    mailMessage.From = new MailAddress(notificationMessage.From);

                    mailMessage.Subject = notificationMessage.Subject;
                    mailMessage.Body = notificationMessage.Body;
                    mailMessage.IsBodyHtml = true;
                    mailMessage.To.Add(destinationEmailAddress);
                    mailMessage.Bcc.Add(destinationEmailAddressAffiliate);
                    _logger.Info("OrderNotification Bcc:" + mailMessage.Bcc +" value should be: "+ destinationEmailAddressAffiliate);
                    smtp.Send(mailMessage);

                }
                catch (ArgumentNullException)
                {
                    _logger.Error("WriteLine MailerArgumentNullException: message is null ");
                }
                catch (InvalidOperationException ex)
                {
                    tmpMsg = "WriteLine MailerInvalidOperationException: " + destinationEmailAddress;
                    tmpMsg += " Addressee: " + notificationMessage.To;
                    tmpMsg += " Subject: " + notificationMessage.Subject;
                    tmpMsg += " WriteLineMsg: " + ex.Message;
                    tmpMsg += " Timestamp was: " + timeStamp;
                    _logger.Error(tmpMsg);

                }
                catch (SmtpFailedRecipientsException)
                {
                    tmpMsg = "WriteLine MailerSmtpFailedRecipientsException: " + destinationEmailAddress;
                    tmpMsg += " Addressee: " + notificationMessage.To;
                    tmpMsg += " Subject: " + notificationMessage.Subject;
                    tmpMsg += " Timestamp was: " + timeStamp;
                    _logger.Error(tmpMsg);

                }
                catch (SmtpException ex)
                {
                    System.Threading.Thread.Sleep(2000);
                    tmpMsg = "WriteLine MailerSmtpException: " + destinationEmailAddress;
                    tmpMsg += " Addressee: " + notificationMessage.To;
                    tmpMsg += " Subject: " + notificationMessage.Subject;
                    tmpMsg += " WriteLineMsg: " + ex.Message;
                    tmpMsg += " Timestamp was: " + timeStamp;
                    _logger.Error(tmpMsg);
                }
                catch (Exception exception)
                {
                    System.Threading.Thread.Sleep(2000);
                    tmpMsg = "WriteLine Exception: " + destinationEmailAddress;
                    tmpMsg += " Addressee: " + notificationMessage.To;
                    tmpMsg += " Subject: " + notificationMessage.Subject;
                    tmpMsg += " WriteLineMsg: " + exception.Message;
                    tmpMsg += " Timestamp was: " + timeStamp;
                    _logger.Error(tmpMsg);
                }

                tmpMsg = "MailerSent: " + destinationEmailAddress;
                tmpMsg += " Addressee: " + notificationMessage.To;
                tmpMsg += " Subject: " + notificationMessage.Subject;
                tmpMsg += " Timestamp was: " + timeStamp;

                _logger.Info(tmpMsg);
            }
        }
    }
}
