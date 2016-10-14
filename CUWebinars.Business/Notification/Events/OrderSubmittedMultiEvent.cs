using CUWebinars.Business.Notification.Email;
using CUWebinars.NotificationSystem.Event;

namespace CUWebinars.Business.Notification.Events
{
    public class OrderSubmittedMultiEvent<T> : TtsBusEvent<T>, IAllowMultiple
        where T : OrderSubmittedMultiMessage
    {
    }
}
