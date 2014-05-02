using CUWebinars.Business.Core.Helpers;
using CUWebinars.Business.Models;
using CUWebinars.Business.Notification.Email;
using CUWebinars.Business.Notification.Events;
using CUWebinars.Business.Notification.Formatters;
using CUWebinars.NotificationSystem.Event;

namespace CUWebinars.Business.Notification.Handlers
{
    public class SendShippedOrderHandler<T> : IEventHandler<SendShippedOrderEvent<T>> where T : Order
    {
        private readonly IFormatter _generalFormatter;
        private readonly INotificationDelivery _notificationDelivery;

        public SendShippedOrderHandler(IFormatter generalFormatter)
            : this(generalFormatter, new SmtpMessageDelivery())
        {

        }
        public SendShippedOrderHandler(IFormatter generalFormatter, INotificationDelivery notificationDelivery)
        {
            _generalFormatter = generalFormatter;
            _notificationDelivery = notificationDelivery;
        }

        public virtual void Process<TObject>(TObject objectOfMessage)
        {
            var notificationMessage = _generalFormatter.Format(objectOfMessage, "SendShippedOrder");

            _notificationDelivery.Notify(notificationMessage);
        }

        public void Handle(SendShippedOrderEvent<T> sendShippedOrderSubmittedEvent)
        {
            Process(sendShippedOrderSubmittedEvent.EventObject);
        }
    }

    public class SendShippedOrderHandler : SendShippedOrderHandler<Order>
    {
        public SendShippedOrderHandler(IFormatter generalFormatter)
            : base(generalFormatter)
        {
        }

        public SendShippedOrderHandler(IFormatter generalFormatter, INotificationDelivery notificationDelivery)
            : base(generalFormatter, notificationDelivery)
        {
        }
    }

}
