using CUWebinars.Business.Models;
using CUWebinars.Business.Notification.Email;
using CUWebinars.Business.Notification.Events;
using CUWebinars.Business.Notification.Formatters;
using CUWebinars.NotificationSystem.Event;

namespace CUWebinars.Business.Notification.Handlers
{
    public class SendReminderHandler<T> : IEventHandler<SendReminderEvent<T>>
        where T : Order
    {
        private readonly IFormatter _generalFormatter;
        private readonly INotificationDelivery _notificationDelivery;

        public SendReminderHandler(IFormatter generalFormatter)
            : this(generalFormatter, new SmtpMessageDelivery())
        {

        }
        public SendReminderHandler(IFormatter generalFormatter, INotificationDelivery notificationDelivery)
        {
            _generalFormatter = generalFormatter;
            _notificationDelivery = notificationDelivery;
        }

        public void Handle(SendReminderEvent<T> sendShippedOrderSubmittedEvent)
        {
            Process(sendShippedOrderSubmittedEvent);
        }

        public virtual void Process(SendReminderEvent<T> sendShippedOrderSubmittedEvent)
        {
            var notificationMessage = _generalFormatter.Format(sendShippedOrderSubmittedEvent.EventObject, "SendReminder");
            notificationMessage.To = sendShippedOrderSubmittedEvent.EventObject.BillingEmail;

            _notificationDelivery.Notify(notificationMessage);
        }
    }

    public class SendReminderHandler : SendReminderHandler<Order>
    {
        public SendReminderHandler(IFormatter generalFormatter)
            : base(generalFormatter)
        {
        }

        public SendReminderHandler(IFormatter generalFormatter, INotificationDelivery notificationDelivery)
            : base(generalFormatter, notificationDelivery)
        {
        }
        
    }
}
