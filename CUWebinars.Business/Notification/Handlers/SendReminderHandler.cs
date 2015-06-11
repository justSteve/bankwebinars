using System;
using CUWebinars.Business.Constants;
using CUWebinars.Business.Models;
using CUWebinars.Business.Notification.Email;
using CUWebinars.Business.Notification.Events;
using CUWebinars.Business.Notification.Formatters;
using CUWebinars.NotificationSystem.Event;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Ninject.Extensions.Logging;
using Ninject.Extensions.Logging.Log4net.Infrastructure;

namespace CUWebinars.Business.Notification.Handlers
{
    public class SendReminderHandler<T> : IEventHandler<SendReminderEvent<T>>
        where T : Order
    {
        private readonly IFormatter _generalFormatter;
        private readonly INotificationDelivery _notificationDelivery;
        private readonly ILogger _logger;

        public SendReminderHandler(IFormatter generalFormatter, ILogger logger)
            : this(generalFormatter, new SmtpMessageDelivery(new Log4NetLogger(typeof(SmtpMessageDelivery))), logger)
        {

        }
        public SendReminderHandler(IFormatter generalFormatter, INotificationDelivery notificationDelivery, ILogger logger)
        {
            _generalFormatter = generalFormatter;
            _notificationDelivery = notificationDelivery;
            _logger = logger;
        }


        public virtual void Process(SendReminderEvent<T> sendReminderEvent)
        {
            try
            {
                var notificationMessage = _generalFormatter.Format(sendReminderEvent.EventObject,
                    "SendReminder");
                notificationMessage.PersistedName = string.Format("SendReminder_{0}_{1}{2}",
                    sendReminderEvent.EventObject.idOrder
                    , DomainConstants.BuildUtcNowAsCts.ToString(DomainConstants.DateTimeLongFormat)
                    , ".htm");

                //  adds the name of the message to the Json object stored in NotificationStorage.
                JObject notificationStorage;

                if (string.IsNullOrWhiteSpace(sendReminderEvent.Details))
                {
                    notificationStorage = new JObject();
                }
                else
                {
                    notificationStorage = JObject.Parse(sendReminderEvent.Details);
                }

                JProperty sendConnectionInfoMsg = new JProperty(
                    string.Concat("SendReminderEventMsg-", DomainConstants.BuildUtcNowAsCts.Ticks),
                    notificationMessage.PersistedName
                    );
                notificationStorage.Add(sendConnectionInfoMsg);

                sendReminderEvent.EventObject.NotificationStorage =
                    notificationStorage.ToString(Formatting.None);

                notificationMessage.To = sendReminderEvent.EventObject.BillingEmail;

                _notificationDelivery.Notify(notificationMessage);
            }
            catch (NullReferenceException nullReferenceException)
            {
                if (ReferenceEquals(null, sendReminderEvent.EventObject))
                {
                    _logger.Error(string.Format("sendReminderEvent ExceptionMessage: {0}", nullReferenceException.Message), nullReferenceException);
                }
                else
                {
                    _logger.Error(
                        string.Format("Event processing failed for sendReminderEvent - OrderId {0}. ExceptionMessage: {1}",
                            sendReminderEvent.EventObject.idOrder,
                            nullReferenceException.Message)
                        , nullReferenceException);
                }
            }
            catch (Exception exception)
            {
                _logger.Error(string.Format("Event processing (outer) failed for sendReminderEvent - OrderId {0}. ExceptionMessage: {1}"
                    , sendReminderEvent.EventObject.idOrder,
                    exception.Message), exception);
            }
        }

        public void Handle(SendReminderEvent<T> @event)
        {
            Process(@event);
        }
    }

    public class SendReminderHandler : SendReminderHandler<Order>
    {
        public SendReminderHandler(IFormatter generalFormatter, ILogger logger)
            : base(generalFormatter, logger)
        {
        }

        public SendReminderHandler(IFormatter generalFormatter, INotificationDelivery notificationDelivery, ILogger logger)
            : base(generalFormatter, notificationDelivery, logger)
        {
        }

    }
}
