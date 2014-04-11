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

        public OrderNotifyEventHandler(IOrderNotificationFormatter<TOrder> orderNotificationFormatter)
            : this(orderNotificationFormatter, new SmtpMessageDelivery())
        {
            
        }
        public OrderNotifyEventHandler(IOrderNotificationFormatter<TOrder> orderNotificationFormatter, INotificationDelivery notificationDelivery)
        {
            _orderNotificationFormatter = orderNotificationFormatter;
            _notificationDelivery = notificationDelivery;
        }

        public virtual void Process<TBody>(Events.OrderSubmittedEvent<TOrder> evt, TBody objectOfMessage)
        {
            var notificationMessage = _orderNotificationFormatter.Format(evt, objectOfMessage);

            _notificationDelivery.Notify(notificationMessage);
        }
    }
}
