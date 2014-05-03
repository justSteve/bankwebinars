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
            Process(sendShippedOrderSubmittedEvent.EventObject);
        }

        public virtual void Process<TObject>(TObject objectOfMessage)
        {
            var notificationMessage = _generalFormatter.Format(objectOfMessage, "SendReminder");

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
