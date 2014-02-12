using BrockAllen.MembershipReboot;
using CUWebinars.Business.Models;
using CUWebinars.Business.Notification.Events;
using CUWebinars.Web.Notification;

namespace CUWebinars.Business.Notification
{
    public class NotificationOrderHandler<T> : NotifyEventHandler<T>, 
        NotificationSystem.Event.IEventHandler<OrderSubmittedEvent<T>>
        where T : UserAccount
    {

        #region Constructors

        public NotificationOrderHandler(INotificationFormatter<T> notificationFormatter)
            : base(notificationFormatter)
        {
        }

        public NotificationOrderHandler(INotificationFormatter<T> notificationFormatter,
            INotificationDelivery notificationDelivery)
            : base(notificationFormatter, notificationDelivery)
        {
        }

        #endregion


        public void Handle(OrderSubmittedEvent<T> orderSubmittedEvent)
        {
            Process(orderSubmittedEvent, orderSubmittedEvent.Order);
        }
    }

    #region Non-generic Derived Class

    public class NotificationOrderHandler : NotificationOrderHandler<UserAccount>
    {
        public NotificationOrderHandler(INotificationFormatter<UserAccount> notificationFormatter)
            : base(notificationFormatter)
        {

        }

        public NotificationOrderHandler(INotificationFormatter<UserAccount> notificationFormatter,
            INotificationDelivery notificationDelivery)
            : base(notificationFormatter, notificationDelivery)
        {
        }
    }

    #endregion

}
