using CUWebinars.NotificationSystem.Event;

namespace CUWebinars.Business.Notification.Events
{
    public class TtsBusEvent<T> : IEvent
    {
        public T EventObject { get; set; }
    }
}
