using CUWebinars.Business.Models;
using CUWebinars.NotificationSystem.Event;

namespace CUWebinars.Business.Notification.Events
{
    public class SendShippedOrderEvent<T> : TtsBusEvent<T>, IAllowMultiple
        where T : Order
    {
    }
}
