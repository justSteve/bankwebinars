using CUWebinars.Business.Models;
using CUWebinars.NotificationSystem.Event;

namespace CUWebinars.Business.Notification.Events
{
    public class SendConnectionInfoEvent<T> : NotificationResendableEvent<T>, IAllowMultiple
        where T : Order
    {
    }
}
