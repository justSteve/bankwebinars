using System.Text.RegularExpressions;
using CUWebinars.Business.Constants;
using CUWebinars.Business.Notification.Email;
using CUWebinars.Business.Notification.Events;
using CUWebinars.Business.Notification.Formatters;
using CUWebinars.Business.Notification.ViewModel;
using CUWebinars.NotificationSystem.Event;
using Ninject.Extensions.Logging;
using System;
using Ninject.Extensions.Logging.Log4net.Infrastructure;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CUWebinars.Business.Notification.Handlers
{
    public class SendPerDayPromoHandler<T> : IEventHandler<SendPerDayPromoEvent<T>>
        where T : WebinarPromoViewModel
    {
        private readonly IFormatter _generalFormatter;
        private readonly INotificationDelivery _notificationDelivery;
        private readonly ILogger _logger;

        public SendPerDayPromoHandler(IFormatter generalFormatter
            , ILogger logger
            )
            : this(generalFormatter, new SmtpMessageDelivery(new Log4NetLogger(typeof(SmtpMessageDelivery))), logger)
        {

        }

        public SendPerDayPromoHandler(IFormatter generalFormatter
            , INotificationDelivery notificationDelivery
            , ILogger logger)
        {
            _generalFormatter = generalFormatter;
            _notificationDelivery = notificationDelivery;
            _logger = logger;

        }

        public virtual void Process(SendPerDayPromoEvent<T> sendPerDayPromoEvent)
        {
            try
            {
                TtsConfigHelper ttsConfigHelper = new TtsConfigHelper();

                var notificationMessage = new NotificationMessage();

                notificationMessage.Body = sendPerDayPromoEvent.EventObject.EventBody;


                // parse potential multiple emails, concept from http://stackoverflow.com/questions/14689044/regex-split-on-comma-space-or-semi-colon-delimitted-string
                char[] delimiters = new[] { ',', ';', ' ' };  // List of your delimiters
                List<string> addressess = sendPerDayPromoEvent.EventObject.Affiliate.NotiPromos.Split(delimiters, StringSplitOptions.RemoveEmptyEntries).ToList();
                var sbListOfRecpt = new StringBuilder();

                var sendToEmails = "\"all.of.us@ttstrain.com\", \"" + sendPerDayPromoEvent.EventObject.Affiliate.NotiPromos.Replace(",", "\",\"") + "\"";

                foreach (var address in addressess)
                {
                    sbListOfRecpt.Append("'" + address + "',");
                }

                //List<string> addressess = sendPerDayPromoEvent.EventObject.Affiliate.ContactEmail.Split(delimiters, StringSplitOptions.RemoveEmptyEntries).ToList();
                //addressess[0] = "steve@ttstrain.com";
                //if (addressess.Count > 0)
                //{
                //    notificationMessage.To = addressess[0];

                //    if (addressess.Count > 1)
                //    {
                //        notificationMessage.Addresses = addressess.Skip(1).ToList(); // put the rest in the CC
                //    }
                //}

                notificationMessage.To = sbListOfRecpt.ToString();

                
                //notificationMessage.To = "steve@ttstrain.com";
                notificationMessage.From = ttsConfigHelper.GetPromoEmailFromAddress(); // approach for From Address TBD!

                if (sendPerDayPromoEvent.EventObject.Webinar != null)
                    if (sendPerDayPromoEvent.EventObject.Subject != null)
                        notificationMessage.Subject = sendPerDayPromoEvent.EventObject.Subject;

                _notificationDelivery.Notify(notificationMessage);
            }
            catch (NullReferenceException nullReferenceException)
            {
                if (ReferenceEquals(null, sendPerDayPromoEvent.EventObject))
                {
                    _logger.Error(string.Format("ExceptionMessage SendPerDayPromoEvent: {0}", nullReferenceException.Message), nullReferenceException);
                }
                else
                {
                    _logger.Error(
                        string.Format("Event processing failed for SendPerDayPromoEvent - OrderId {0}. ExceptionMessage: {1}",
                            sendPerDayPromoEvent.EventObject.Webinar.idWebinar,
                            nullReferenceException.Message)
                        , nullReferenceException);
                }
            }
            catch (Exception exception)
            {
                _logger.Error(string.Format("SendPerDayPromoEvent (outer) ExceptionMessage: {0}", exception.Message), exception);
            }
        }

        public void Handle(SendPerDayPromoEvent<T> @event)
        {
            Process(@event);
        }
    }

    public class SendPerDayPromoHandler : SendPerDayPromoHandler<WebinarPromoViewModel>
    {
        public SendPerDayPromoHandler(IFormatter generalFormatter, ILogger logger)
            : base(generalFormatter, logger)
        {

        }

        public SendPerDayPromoHandler(IFormatter generalFormatter, INotificationDelivery notificationDelivery, ILogger logger)
            : base(generalFormatter, notificationDelivery, logger)
        {
        }

    }
}