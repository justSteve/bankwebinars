using System;
using CUWebinars.Business.Constants;
using CUWebinars.Business.Models;
using CUWebinars.Business.Notification.Email;
using CUWebinars.Business.Notification.Events;
using CUWebinars.Business.Notification.Formatters;
using CUWebinars.NotificationSystem.Event;
using Ninject.Extensions.Logging;

namespace CUWebinars.Business.Notification.Handlers
{
    public class SendReminderHandler<T> : IEventHandler<SendReminderEvent<T>>
        where T : Order
    {
        private readonly IFormatter _generalFormatter;
        private readonly INotificationDelivery _notificationDelivery;
        private readonly ILogger _logger;

        public SendReminderHandler(IFormatter generalFormatter, ILogger logger)
            : this(generalFormatter, new SmtpMessageDelivery(), logger)
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
                notificationMessage.PersistedName = string.Format("SendReminder-{0}{1}", DateTime.Now.ToString(DomainConstants.DateTimeLongFormat), ".htm");

                //  adds the name of the message to the Json object stored in NotificationStorage.
                string details = sendReminderEvent.Details;
                sendReminderEvent.EventObject.NotificationStorage =
                    details.Insert(details.Length - 1, string.Concat(",", @"""SendReminderEventMsg-", DateTime.Now.Ticks, '"', @":", '"', notificationMessage.PersistedName, '"'));


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
