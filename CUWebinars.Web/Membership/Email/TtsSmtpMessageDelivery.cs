using BrockAllen.MembershipReboot;
using CUWebinars.Business.Constants;
using CUWebinars.Web.Services;
using System;
using System.Configuration;
using System.Net.Configuration;
using System.Net.Mail;

namespace CUWebinars.Web.Membership.Email
{
    public class TtsSmtpMessageDelivery : IMessageDelivery
    {
        private readonly IStateService _stateService;
        public TtsSmtpMessageDelivery(IStateService stateService)
        {
            _stateService = stateService;
        }

        public void Send(Message msg)
        {
            if (_stateService.HasValue(DomainConstants.UserCreatedViaNewOrder) || _stateService.HasValue(DomainConstants.UserCreatedDuringCartCheckout))
            {
                return;
            }

            var mailMessage = new MailMessage();
            DateTime timeStamp = DateTime.Now;
            var tmpMsg = string.Empty;

            if (string.IsNullOrWhiteSpace(msg.From))
            {
                var smtp = ConfigurationManager.GetSection("system.net/mailSettings/smtp") as SmtpSection;
                msg.From = smtp.From;
            }

            using (var smtp = new SmtpClient())
            {
                smtp.Timeout = 5000;

                mailMessage.From = new MailAddress(ConfigurationManager.AppSettings["TenantEmail"]);

                string destinationEmailAddress = msg.To;

                if (ConfigurationManager.AppSettings["EmailSendingMode"] != "live")
                {
                    destinationEmailAddress = ConfigurationManager.AppSettings["TestEmailAddress"];
                }
                if (System.Diagnostics.Debugger.IsAttached)
                {
                    destinationEmailAddress = ConfigurationManager.AppSettings["TestEmailAddress"];
                }
#if DEBUG
                destinationEmailAddress = ConfigurationManager.AppSettings["TestEmailAddress"];
#endif

                mailMessage.To.Add(new MailAddress(destinationEmailAddress));

                mailMessage.Subject = msg.Subject;
                mailMessage.Body = msg.Body;
                mailMessage.IsBodyHtml = true;

                try
                {
                    smtp.Send(mailMessage);
                }
                catch (ArgumentNullException)
                {
                    Tracing.Error("Error MailerArgumentNullException: message is null ");
                }
                catch (InvalidOperationException ex)
                {
                    tmpMsg = "ERROR MailerInvalidOperationException: " + msg.To;
                    tmpMsg += " Subject: " + msg.Subject;
                    tmpMsg += " ErrorMsg: " + ex.Message;
                    tmpMsg += " Timestamp was: " + timeStamp;
                    Tracing.Error(tmpMsg);

                }
                catch (SmtpFailedRecipientsException)
                {
                    tmpMsg = "ERROR MailerSmtpFailedRecipientsException: " + msg.To;
                    tmpMsg += " Subject: " + msg.Subject;
                    tmpMsg += " Timestamp was: " + timeStamp;
                    Tracing.Error(tmpMsg);
                }
                catch (SmtpException ex)
                {
                    System.Threading.Thread.Sleep(2000);
                    tmpMsg = "ERROR MailerSmtpException: " + msg.To;
                    tmpMsg += " Subject: " + msg.Subject;
                    tmpMsg += " ErrorMsg: " + ex.Message;
                    tmpMsg += " Timestamp was: " + timeStamp;
                    Tracing.Error(tmpMsg);
                }
                catch (Exception exception)
                {
                    System.Threading.Thread.Sleep(2000);
                    tmpMsg = "ERROR MailerSmtpException: " + msg.To;
                    tmpMsg += " Subject: " + msg.Subject;
                    tmpMsg += " ErrorMsg: " + exception.Message;
                    tmpMsg += " Timestamp was: " + timeStamp;
                    Tracing.Error(tmpMsg);
                }

                tmpMsg = "MailerSent: " + msg.To;
                tmpMsg += " Subject: " + msg.Subject;
                tmpMsg += " Timestamp was: " + timeStamp;

                Tracing.Information(tmpMsg);
            }
        }
    }
}
