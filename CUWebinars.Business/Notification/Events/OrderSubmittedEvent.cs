using CUWebinars.Business.Models;
using CUWebinars.NotificationSystem.Event;

namespace CUWebinars.Business.Notification.Events
{
    public class OrderSubmittedEvent<T> : TtsBusEvent<T>, IAllowMultiple
        where T : Order
    {
        
    }
}