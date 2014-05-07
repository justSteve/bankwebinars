using CUWebinars.Business.Models;
using CUWebinars.Business.Notification.Email;
using CUWebinars.Business.Notification.Formatters;

namespace CUWebinars.Business.Notification.Handlers
{
    public class OrderNotifyEventHandler<TOrder> 
        where TOrder : Order
    {
        private readonly IOrderNotificationFormatter<TOrder> _orderNotificationFormatter;
        private readonly INotificationDelivery _notificationDelivery;
        private readonly INotificationPersister _notificationPersister;

        public OrderNotifyEventHandler(IOrderNotificationFormatter<TOrder> orderNotificationFormatter)
            : this(orderNotificationFormatter, new SmtpMessageDelivery(), new FileBasedNotificationPersister())
        {
            
        }
        public OrderNotifyEventHandler(IOrderNotificationFormatter<TOrder> orderNotificationFormatter, INotificationDelivery notificationDelivery, INotificationPersister notificationPersister)
        {
            _orderNotificationFormatter = orderNotificationFormatter;
            _notificationDelivery = notificationDelivery;
            _notificationPersister = notificationPersister;
        }

        public virtual void Process<TBody>(Events.OrderSubmittedEvent<TOrder> evt, TBody objectOfMessage)
        {
            var notificationMessage = _orderNotificationFormatter.Format(evt, objectOfMessage, _notificationPersister);
            _notificationDelivery.Notify(notificationMessage);
        }
    }
}
