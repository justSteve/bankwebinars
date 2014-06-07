using BrockAllen.MembershipReboot;
using System;
using System.Configuration;
using System.Net.Configuration;
using System.Net.Mail;
using CUWebinars.Business.Constants;
using CUWebinars.Web.Services;
using Ninject.Extensions.Logging;

namespace CUWebinars.Web.Membership.Email
{
    public class TtsSmtpMessageDelivery : IMessageDelivery
    {
        private readonly IStateService _stateService;
        //private readonly ILogger _logger;
        public TtsSmtpMessageDelivery(IStateService stateService)
        //public TtsSmtpMessageDelivery(IStateService stateService, ILogger logger)
        {
            _stateService = stateService;
            //_logger = logger;
        }

        public void Send(Message msg)
        {
            if (_stateService.HasValue(DomainConstants.UserCreatedViaNewOrder))
            {
                return;
            }

            var mailMessage = new MailMessage();
            DateTime timeStamp = DateTime.Now;
            //Logger.Instance.LogMessage("SenderTimer: " + timeStamp);
            var tmpMsg = "";

            if (string.IsNullOrWhiteSpace(msg.From))
            {
                var smtp = ConfigurationManager.GetSection("system.net/mailSettings/smtp") as SmtpSection;
                msg.From = smtp.From;
            }

            using (var smtp = new SmtpClient())
            {
                smtp.Timeout = 5000;

                mailMessage.From = new MailAddress(msg.From);


                string destinationEmailAddress = msg.To;

                if (ConfigurationManager.AppSettings["EmailSendingMode"] == "testing")
                {
                    destinationEmailAddress = ConfigurationManager.AppSettings["TestEmailAddress"];
                }

                mailMessage.To.Add(new MailAddress(destinationEmailAddress));
                mailMessage.Subject = msg.Subject;
                mailMessage.Body = msg.Body;
                mailMessage.IsBodyHtml = true;

                try
                {
                    smtp.Send(mailMessage);
                }
                //catch (SmtpException e)
                //{
                //    _logger.Error("[SmtpMessageDelivery.Send] SmtpException: " + e.Message);
                //}
                catch (ArgumentNullException)
                {
                    Tracing.Error("Error MailerArgumentNullException: message is null ");
                }
                catch (InvalidOperationException ex)
                {
                    tmpMsg = "ERROR MailerInvalidOperationException: " + msg.To.ToString();
                    tmpMsg += " Subject: " + msg.Subject.ToString();
                    tmpMsg += " ErrorMsg: " + ex.Message;
                    tmpMsg += " Timestamp was: " + timeStamp;
                    Tracing.Error(tmpMsg);

                }
                catch (SmtpFailedRecipientsException)
                {
                    tmpMsg = "ERROR MailerSmtpFailedRecipientsException: " + msg.To.ToString();
                    tmpMsg += " Subject: " + msg.Subject.ToString();
                    tmpMsg += " Timestamp was: " + timeStamp;
                    Tracing.Error(tmpMsg);
                }
                catch (SmtpException ex)
                {
                    System.Threading.Thread.Sleep(2000);
                    tmpMsg = "ERROR MailerSmtpException: " + msg.To.ToString();
                    tmpMsg += " Subject: " + msg.Subject.ToString();
                    tmpMsg += " ErrorMsg: " + ex.Message;
                    tmpMsg += " Timestamp was: " + timeStamp;
                    Tracing.Error(tmpMsg);
                }

                tmpMsg = "MailerSent: " + msg.To.ToString();
                tmpMsg += " Subject: " + msg.Subject.ToString();
                tmpMsg += " Timestamp was: " + timeStamp;

                Tracing.Information(tmpMsg);
            }

        }
    }
}
