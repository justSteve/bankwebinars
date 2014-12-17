using BrockAllen.MembershipReboot;
using CUWebinars.Business.Constants;
using CUWebinars.Web.Core;
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
        private readonly GlobalConfig _globalConfig = GlobalConfig.GlobalConfigSingleton;
        public TtsSmtpMessageDelivery(IStateService stateService)
        {
            _stateService = stateService;
        }

        public void Send(Message msg)
        {
            if (_stateService.HasValue(DomainConstants.UserCreatedViaNewOrder) || _stateService.HasValue(DomainConstants.UserCreatedDuringCartCheckout))
            {
                _stateService.ClearValue(DomainConstants.UserCreatedDuringCartCheckout);
                return;
            } else if (_stateService.HasValue(DomainConstants.UserCreatedViaMigrator))
            {
                _stateService.ClearValue(DomainConstants.UserCreatedDuringCartCheckout);
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

                mailMessage.From = new MailAddress(_globalConfig.TenantEmail);

                string destinationEmailAddress = msg.To;

                //  Set this AppSetting in Web.Config to true when testing i.e. not live
                if (_globalConfig.EmailSendingMode != "live")
                {
                    destinationEmailAddress = _globalConfig.TestEmailAddress;
                    mailMessage.To.Add(new MailAddress(_globalConfig.TestEmailAddress2));
                }
                //if (System.Diagnostics.Debugger.IsAttached)
                //{
                //    destinationEmailAddress = _globalConfig.TestEmailAddress;
                //}

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
