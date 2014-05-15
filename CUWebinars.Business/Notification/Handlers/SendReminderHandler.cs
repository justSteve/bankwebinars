using System;
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
                notificationMessage.To = sendReminderEvent.EventObject.BillingEmail;

                _notificationDelivery.Notify(notificationMessage);
            }
            catch (NullReferenceException nullReferenceException)
            {
                _logger.Error(
                    string.Format("Event processing failed. Check that BillingEmail has a value for OrderId {0}",
                        sendReminderEvent.EventObject.idOrder), nullReferenceException);
            }
            catch (Exception exception)
            {
                _logger.Error(string.Format("Event processing failed for OrderId {0}", sendReminderEvent.EventObject.idOrder), exception);
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
