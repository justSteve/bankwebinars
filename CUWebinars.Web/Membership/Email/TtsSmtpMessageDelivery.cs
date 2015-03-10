using BrockAllen.MembershipReboot;
using CUWebinars.Business.Constants;
using CUWebinars.Web.Core;
using CUWebinars.Web.Services;
using System;
using System.Configuration;
using System.Net.Configuration;
using System.Net.Mail;
using Ninject.Extensions.Logging;

namespace CUWebinars.Web.Membership.Email
{
    public class TtsSmtpMessageDelivery : IMessageDelivery
    {
        private readonly IStateService _stateService;
        private readonly ILogger _logger;
        private readonly GlobalConfig _globalConfig = GlobalConfig.GlobalConfigSingleton;
        public  TtsSmtpMessageDelivery(IStateService stateService, ILogger logger)
        {
            _stateService = stateService;
            _logger = logger;
        }

        public void Send(Message msg)
        {
            if (_stateService.HasValue(DomainConstants.UserCreatedViaNewOrder) || 
                _stateService.HasValue(DomainConstants.UserCreatedDuringCartCheckout) || 
                _stateService.HasValue(DomainConstants.UserCreatedViaMigrator) ||
                _stateService.HasValue(DomainConstants.CartCreatedUserPasswordCreate) ||
                msg.Subject.Contains("Email Account Verified") ||
                msg.Subject.Equals(string.Empty))
            {
                _stateService.ClearValue(DomainConstants.UserCreatedDuringCartCheckout);
                return;
            }

            var mailMessage = new MailMessage();
            DateTime timeStamp = DateTime.Now;
            var tmpMsg = string.Empty;
            string destinationEmailAddress = msg.To;

            if (string.IsNullOrWhiteSpace(msg.From))
            {
                var smtp = ConfigurationManager.GetSection("system.net/mailSettings/smtp") as SmtpSection;
                msg.From = smtp.From;
            }

            _logger.Info(string.Format("Sending AccountMaintenance message to: {0} about {1} ", destinationEmailAddress, msg.Subject));


            using (var smtp = new SmtpClient())
            {
                smtp.Timeout = 5000;

                mailMessage.From = new MailAddress(_globalConfig.TenantEmail);

                //  Set this AppSetting in Web.Config to true when testing i.e. notlive
                if (_globalConfig.EmailSendingMode != "live")
                {
                    destinationEmailAddress = _globalConfig.TestEmailAddress;
                    mailMessage.To.Add(new MailAddress(_globalConfig.TestEmailAddress2));
                }

                mailMessage.To.Add(new MailAddress(destinationEmailAddress));

                mailMessage.Subject = msg.Subject;
                mailMessage.Body = msg.Body;
                mailMessage.IsBodyHtml = true;

                try
                {
                    smtp.Send(mailMessage);
                    _logger.Info(string.Format("Sending AccountMaintenance message to: {0} about {1} ", destinationEmailAddress, msg.Subject));
                }
                catch (ArgumentNullException)
                {
                    Tracing.Error("Error MailerArgumentNullException: message is null ");
                    _logger.Error("WriteLine MailerArgumentNullException: message is null ");
                }
                catch (InvalidOperationException ex)
                {
                    tmpMsg = "ERROR MailerInvalidOperationException: " + msg.To;
                    tmpMsg += " Subject: " + msg.Subject;
                    tmpMsg += " ErrorMsg: " + ex.Message;
                    tmpMsg += " Timestamp was: " + timeStamp;
                    Tracing.Error(tmpMsg);
                    _logger.Error(tmpMsg);
                }
                catch (SmtpFailedRecipientsException)
                {
                    tmpMsg = "ERROR MailerSmtpFailedRecipientsException: " + msg.To;
                    tmpMsg += " Subject: " + msg.Subject;
                    tmpMsg += " Timestamp was: " + timeStamp;
                    Tracing.Error(tmpMsg);
                    _logger.Error(tmpMsg);
                }
                catch (SmtpException ex)
                {
                    System.Threading.Thread.Sleep(2000);
                    tmpMsg = "ERROR MailerSmtpException: " + msg.To;
                    tmpMsg += " Subject: " + msg.Subject;
                    tmpMsg += " ErrorMsg: " + ex.Message;
                    tmpMsg += " Timestamp was: " + timeStamp;
                    tmpMsg += " ExceptionType was: SmtpException";
                    tmpMsg += string.Format(" InnerExceptionType was: {0}", ex.InnerException == null ? string.Empty : ex.InnerException.GetType().ToString());
                    Tracing.Error(tmpMsg);
                    _logger.Error(tmpMsg);
                }
                catch (Exception exception)
                {
                    System.Threading.Thread.Sleep(2000);
                    tmpMsg = "ERROR Exception: " + msg.To;
                    tmpMsg += " Subject: " + msg.Subject;
                    tmpMsg += " ErrorMsg: " + exception.Message;
                    tmpMsg += " Timestamp was: " + timeStamp;
                    tmpMsg += " ExceptionType was: Exception";
                    tmpMsg += string.Format(" InnerExceptionType was: {0}", exception.InnerException == null ? string.Empty : exception.InnerException.GetType().ToString());

                    Tracing.Error(tmpMsg);
                    _logger.Error(tmpMsg);
                }

                tmpMsg = "MailerSent: " + msg.To;
                tmpMsg += " Subject: " + msg.Subject;
                tmpMsg += " Timestamp was: " + timeStamp;

                Tracing.Information(tmpMsg);
                
                _logger.Info(tmpMsg);
            }
        }
    }
}
