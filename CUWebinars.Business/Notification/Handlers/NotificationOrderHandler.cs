using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CUWebinars.Business.Models;
using CUWebinars.Business.Notification.Events;
using CUWebinars.Business.Notification.Formatters;

namespace CUWebinars.Business.Notification.Handlers
{
    public class NotificationOrderHandler<T> : OrderNotifyEventHandler<T>, NotificationSystem.Event.IEventHandler<Events.OrderSubmittedEvent<T>>
        where T: Order
    {
        public NotificationOrderHandler(IOrderNotificationFormatter<T> notificationFormatter)
            : base(notificationFormatter)
        {
        }

        public NotificationOrderHandler(IOrderNotificationFormatter<T> notificationFormatter, INotificationDelivery notificationDelivery)
            : base(notificationFormatter, notificationDelivery)
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
        public NotificationOrderHandler(IOrderNotificationFormatter<Order> notificationFormatter)
            : base(notificationFormatter)
        {

        }

        public NotificationOrderHandler(IOrderNotificationFormatter<Order> notificationFormatter,
            INotificationDelivery notificationDelivery)
            : base(notificationFormatter, notificationDelivery)
        {
        }
    }

    #endregion
}
