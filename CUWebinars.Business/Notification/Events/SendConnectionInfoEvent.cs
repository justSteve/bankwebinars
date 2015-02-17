using CUWebinars.Business.Models;
using CUWebinars.NotificationSystem.Event;

namespace CUWebinars.Business.Notification.Events
{
    public class SendConnectionInfoEvent<T> : TtsBusEvent<T>, IAllowMultiple, INotificationResendableEvent
        where T : Order
    {
        public bool ResendEvent { get; set; }
    }
}
