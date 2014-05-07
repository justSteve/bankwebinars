using CUWebinars.Business.Models;
using CUWebinars.Business.Notification.Events;
using CUWebinars.Business.Notification.Formatters;
using Ninject.Extensions.Logging;

namespace CUWebinars.Business.Notification.Handlers
{
    public class NotificationOrderHandler<T> : OrderNotifyEventHandler<T>, NotificationSystem.Event.IEventHandler<Events.OrderSubmittedEvent<T>>
        where T: Order
    {
        public NotificationOrderHandler(IOrderNotificationFormatter<T> notificationFormatter, ILogger logger)
            : base(notificationFormatter, logger)
        {
        }

        public NotificationOrderHandler(IOrderNotificationFormatter<T> notificationFormatter, 
            INotificationDelivery notificationDelivery,
            ILogger logger)
            : base(notificationFormatter, notificationDelivery, new FileBasedNotificationPersister(), logger)
        {
        }

        public void Handle(OrderSubmittedEvent<T> orderSubmittedEvent)
        {
            Process(orderSubmittedEvent, orderSubmittedEvent.Order);
        }
    }

    #region Non-generic Derived Class

    public class NotificationOrderHandler : NotificationOrderHandler<Order>
    {
        public NotificationOrderHandler(IOrderNotificationFormatter<Order> notificationFormatter, ILogger logger)
            : base(notificationFormatter, logger)
        {

        }

        public NotificationOrderHandler(IOrderNotificationFormatter<Order> notificationFormatter,
            INotificationDelivery notificationDelivery,
            ILogger logger)
            : base(notificationFormatter, notificationDelivery, logger)
        {
        }
    }

    #endregion
}
