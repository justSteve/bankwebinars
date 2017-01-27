using CUWebinars.Business.Notification.Email;
using CUWebinars.NotificationSystem.Event;

namespace CUWebinars.Business.Notification.Events
{
    public class OrderSubmitted2Event<T> : TtsBusEvent<T>, IAllowMultiple
        where T : OrderSubmitted2Message
    {
    }
}
