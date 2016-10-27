using CUWebinars.Business.Notification.Formatters;
using CUWebinars.Business.Notification.ViewModel;
using Ninject.Extensions.Logging;
using System;
using System.Configuration;
using System.Net.Configuration;
using System.Net.Mail;
using CUWebinars.Business.Constants;

namespace CUWebinars.Business.Notification.Email
{
    public class OrderConfirmedForAdditionalLocationDelivery : IOrderConfirmedForAdditionalLocationDelivery
    {
        private readonly ILogger _logger;
        private readonly IFormatter _generalFormatter;

        public OrderConfirmedForAdditionalLocationDelivery(ILogger logger, IFormatter generalFormatter)
        {
            _logger = logger;
            _generalFormatter = generalFormatter;
        }

        public void Notify(IAdditionalLocationOrderDetailsMessage additionalLocationOrderDetailsMessage)
        {
            var order = additionalLocationOrderDetailsMessage.Order;

            var orderSubmittedViewModel = new OrderSubmittedViewModel
            {
                AddPasswordUrl = string.Empty,
                //ConfirmChangeEmailUrl = 
                Order = order,
                UserCreatedInCart = additionalLocationOrderDetailsMessage.UserCreatedInCart,
                UserCreatedOnImport = additionalLocationOrderDetailsMessage.UserCreatedOnImport
            };

            var notificationMessage = _generalFormatter.Format(orderSubmittedViewModel,
                "OrderSubmittedAdditionalLocation");
            notificationMessage.PersistedName = additionalLocationOrderDetailsMessage.PersistedName;

            if (order.idAffiliate == 62)
            {
                notificationMessage.To = "steve@ttstrain.com";
            }
            else
            {
                notificationMessage.To = additionalLocationOrderDetailsMessage.NotifyAddress;
            }

            string details = additionalLocationOrderDetailsMessage.Details;

            if (details != null)
            {
                order.NotificationStorage =
                    details.Insert(details.Length - 1,
                        string.Concat(",", @"""OrderSubmittedAdditionalLocationEventMsg-", DomainConstants.BuildUtcNowAsCts.Ticks, '"', @":",
                            '"',
                            notificationMessage.PersistedName, '"'));
            }

            SendMessage(notificationMessage);
        }

        private void SendMessage(INotificationMessage notificationMessage)
        {
            _logger.Info("Sending Additional Loc Comfirmation message!");
            _logger.Info(string.Format("Message Subject is:{0}, PersistedName is:{1}",
                notificationMessage.Subject, notificationMessage.PersistedName ?? "null"));

            var timeStamp = DomainConstants.BuildUtcNowAsCts;
            var mailMessage = new MailMessage();
            var tmpMsg = string.Empty;
            var destinationEmailAddress = notificationMessage.To;

            if (string.IsNullOrWhiteSpace(notificationMessage.From))
            {
                var smtp = ConfigurationManager.GetSection("system.net/mailSettings/smtp") as SmtpSection;
                notificationMessage.From = smtp.From;
            }

            using (var smtp = new SmtpClient())
            {
                smtp.Timeout = 5000;

                //  Set this AppSetting in App.Config to something other than 'live' when testing i.e. notlive
                if (ConfigurationManager.AppSettings["EmailSendingMode"] != "live")
                {
                    destinationEmailAddress = ConfigurationManager.AppSettings["TestEmailAddress"];
                    mailMessage.To.Add(new MailAddress(ConfigurationManager.AppSettings["TestEmailAddress2"]));
                }

                //mailMessage.To.Add(new MailAddress("steve@ttstrain.com"));
                mailMessage.To.Add(new MailAddress(destinationEmailAddress));

                try
                {
                    mailMessage.From = new MailAddress(notificationMessage.From);

                    _logger.Info(string.Format("Sending msg to {0}", notificationMessage.To));

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
                    tmpMsg = "WriteLine MailerInvalidOperationException: " + destinationEmailAddress;
                    tmpMsg += " Subject: " + notificationMessage.Subject;
                    tmpMsg += " WriteLineMsg: " + ex.Message;
                    tmpMsg += " Timestamp was: " + timeStamp;
                    _logger.Error(tmpMsg);

                }
                catch (SmtpFailedRecipientsException)
                {
                    tmpMsg = "WriteLine MailerSmtpFailedRecipientsException: " + destinationEmailAddress;
                    tmpMsg += " Subject: " + notificationMessage.Subject;
                    tmpMsg += " Timestamp was: " + timeStamp;
                    _logger.Error(tmpMsg);

                }
                catch (SmtpException ex)
                {
                    System.Threading.Thread.Sleep(2000);
                    tmpMsg = "WriteLine MailerSmtpException: " + destinationEmailAddress;
                    tmpMsg += " Subject: " + notificationMessage.Subject;
                    tmpMsg += " WriteLineMsg: " + ex.Message;
                    tmpMsg += " Timestamp was: " + timeStamp;
                    _logger.Error(tmpMsg);
                }
                catch (Exception exception)
                {
                    System.Threading.Thread.Sleep(2000);
                    tmpMsg = "WriteLine Exception: " + destinationEmailAddress;
                    tmpMsg += " Subject: " + notificationMessage.Subject;
                    tmpMsg += " WriteLineMsg: " + exception.Message;
                    tmpMsg += " Timestamp was: " + timeStamp;
                    _logger.Error(tmpMsg);
                }

                tmpMsg = "MailerSent: " + destinationEmailAddress;
                tmpMsg += " Subject: " + notificationMessage.Subject;
                tmpMsg += " Timestamp was: " + timeStamp;

                _logger.Info(tmpMsg);
            }
        }
    }
}