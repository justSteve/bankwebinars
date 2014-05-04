using CUWebinars.Business.Models;
using CUWebinars.Business.Notification.Email;
using CUWebinars.Business.Notification.Events;
using CUWebinars.Business.Notification.Formatters;
using CUWebinars.NotificationSystem.Event;

namespace CUWebinars.Business.Notification.Handlers
{
    public class SendConnectionInfoHandler<T> : IEventHandler<SendConnectionInfoEvent<T>> 
        where T : Order
    {
        private readonly IFormatter _generalFormatter;
        private readonly INotificationDelivery _notificationDelivery;

        public SendConnectionInfoHandler(IFormatter generalFormatter)
            : this(generalFormatter, new SmtpMessageDelivery())
        {

        }
        public SendConnectionInfoHandler(IFormatter generalFormatter, INotificationDelivery notificationDelivery)
        {
            _generalFormatter = generalFormatter;
            _notificationDelivery = notificationDelivery;
        }

        public virtual void Process(SendConnectionInfoEvent<T> objectOfMessage)
        {
            var notificationMessage = _generalFormatter.Format(objectOfMessage.EventObject, "SendConnectionInfo");
            notificationMessage.To = objectOfMessage.EventObject.BillingEmail;
            _notificationDelivery.Notify(notificationMessage);
        }


        public void Handle(SendConnectionInfoEvent<T> sendShippedOrderSubmittedEvent)
        {
            Process(sendShippedOrderSubmittedEvent);
        }
    }

    public class SendConnectionInfoHandler : SendConnectionInfoHandler<Order>
    {
        public SendConnectionInfoHandler(IFormatter generalFormatter)
            : base(generalFormatter)
        {
            
        }

        public SendConnectionInfoHandler(IFormatter generalFormatter, INotificationDelivery notificationDelivery)
            : base(generalFormatter, notificationDelivery)
        {
        }

    }
}
