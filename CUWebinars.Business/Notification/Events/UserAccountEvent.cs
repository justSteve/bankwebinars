using CUWebinars.NotificationSystem.Event;

namespace CUWebinars.Business.Notification.Events
{
    public class UserAccountEvent<TAccount> : IEvent
    {
        public TAccount Account { get; set; }
    }
}