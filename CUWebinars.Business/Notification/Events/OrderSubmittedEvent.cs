
using CUWebinars.Business.Models;
using CUWebinars.NotificationSystem.Event;

namespace CUWebinars.Business.Notification.Events
{
    public class OrderSubmittedEvent<T> : IEvent, IAllowMultiple
    {
        public Order Order { get; set; } 
    }
}