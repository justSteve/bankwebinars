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

namespace CUWebinars.Business.Notification.Handlers
{
    public class SendWeeklyInvoiceHandler<T> : IEventHandler<SendWeeklyInvoiceEvent<T>>
        where T : SendWeeklyInvoiceViewModel
    {
        private readonly IFormatter _generalFormatter;
        private readonly INotificationDelivery _weeklyInvoiceDelivery;
        private readonly ILogger _logger;

        public SendWeeklyInvoiceHandler(IFormatter generalFormatter
            , ILogger logger
            )
            : this(generalFormatter, new SmtpMessageDelivery(new Log4NetLogger(typeof(SmtpMessageDelivery))), logger)
        {

        }

        public SendWeeklyInvoiceHandler(IFormatter generalFormatter
            , INotificationDelivery weeklyInvoiceDelivery
            , ILogger logger)
        {
            _generalFormatter = generalFormatter;
            _weeklyInvoiceDelivery = weeklyInvoiceDelivery;
            _logger = logger;

        }

        public virtual void Process(SendWeeklyInvoiceEvent<T> sendWeeklyInvoiceEvent)
        {
            try
            {
                TtsConfigHelper ttsConfigHelper = new TtsConfigHelper();

                //var persistedNamePrefix = "WeeklyInvoice_" + sendWeeklyInvoiceEvent.EventObject.Affiliate.ttsDomain + '_';

                var notificationMessage = new NotificationMessage();

                notificationMessage.Body = sendWeeklyInvoiceEvent.EventObject.EmailBody;

                //notificationMessage.PersistedName = string.Format("{0}_{1}{2}",
                //    persistedNamePrefix,
                //    DomainConstants.BuildUtcNowAsCts.ToString(DomainConstants.DateTimeLongFormat),
                //    ".htm"
                //    );
                notificationMessage.PersistedName = "NOT USED FOR WEEKLY INVOICE PROCESSING";

                // parse potential multiple emails, concept from http://stackoverflow.com/questions/14689044/regex-split-on-comma-space-or-semi-colon-delimitted-string
                char[] delimiters = new[] { ',', ';', ' ' };  // List of your delimiters
                List<string> addressess = sendWeeklyInvoiceEvent.EventObject.Affiliate.ContactEmail.Split(delimiters, StringSplitOptions.RemoveEmptyEntries).ToList();

                if (addressess.Count > 0)
                {
                    notificationMessage.To = addressess[0];

                    if (addressess.Count > 1)
                    {
                        notificationMessage.Addresses = addressess.Skip(1).ToList(); // put the rest in the CC
                    }
                }

                notificationMessage.From = ttsConfigHelper.GetWeeklyInvoiceEmailFromAddress(); // approach for From Address TBD!
                notificationMessage.Subject = sendWeeklyInvoiceEvent.EventObject.Subject; // or ttsConfigHelper.GetWeeklyInvoiceEmailSubject(); // approach for Subject TBD!

                _weeklyInvoiceDelivery.Notify(notificationMessage);
            }
            catch (NullReferenceException nullReferenceException)
            {
                if (ReferenceEquals(null, sendWeeklyInvoiceEvent.EventObject))
                {
                    _logger.Error(string.Format("ExceptionMessage SendWeeklyInvoiceEvent: {0}", nullReferenceException.Message), nullReferenceException);
                }
                else
                {
                    //_logger.Error(
                    //    string.Format("Event processing failed for SendWeeklyInvoiceEvent - OrderId {0}. ExceptionMessage: {1}",
                    //        sendWeeklyInvoiceEvent.EventObject.Webinar.idWebinar,
                    //        nullReferenceException.Message)
                    //    , nullReferenceException);
                }
            }
            catch (Exception exception)
            {
                _logger.Error(string.Format("SendWeeklyInvoiceEvent (outer) ExceptionMessage: {0}", exception.Message), exception);
            }
        }

        public void Handle(SendWeeklyInvoiceEvent<T> @event)
        {
            Process(@event);
        }
    }

    public class SendWeeklyInvoiceHandler : SendWeeklyInvoiceHandler<SendWeeklyInvoiceViewModel>
    {
        public SendWeeklyInvoiceHandler(IFormatter generalFormatter, ILogger logger)
            : base(generalFormatter, logger)
        {

        }

        public SendWeeklyInvoiceHandler(IFormatter generalFormatter, INotificationDelivery notificationDelivery, ILogger logger)
            : base(generalFormatter, notificationDelivery, logger)
        {
        }

    }
}