using CUWebinars.Business.Models;
using CUWebinars.Business.Notification.Email;
using CUWebinars.Business.Notification.Events;
using CUWebinars.Business.Notification.Formatters;
using CUWebinars.NotificationSystem.Event;

namespace CUWebinars.Business.Notification.Handlers
{
    public class SendRecordingPostedHandler<T> : IEventHandler<SendRecordingPostedEvent<T>>
        where T : Order
    {
        private readonly IFormatter _generalFormatter;
        private readonly INotificationDelivery _notificationDelivery;

        public SendRecordingPostedHandler(IFormatter generalFormatter)
            : this(generalFormatter, new SmtpMessageDelivery())
        {

        }

        public SendRecordingPostedHandler(IFormatter generalFormatter, INotificationDelivery notificationDelivery)
        {
            _generalFormatter = generalFormatter;
            _notificationDelivery = notificationDelivery;
        }


        public void Handle(SendRecordingPostedEvent<T> sendShippedOrderSubmittedEvent)
        {
            Process(sendShippedOrderSubmittedEvent.EventObject);
        }

        public virtual void Process<TObject>(TObject objectOfMessage)
        {
            var notificationMessage = _generalFormatter.Format(objectOfMessage, "SendRecordingPosted");

            _notificationDelivery.Notify(notificationMessage);
        }

    }

    public class SendRecordingPostedHandler : SendRecordingPostedHandler<Order>
    {
        public SendRecordingPostedHandler(IFormatter generalFormatter)
            : base(generalFormatter)
        {
            
        }

        public SendRecordingPostedHandler(IFormatter generalFormatter, INotificationDelivery notificationDelivery)
            : base(generalFormatter, notificationDelivery)
        {
        }
        
    }
}
