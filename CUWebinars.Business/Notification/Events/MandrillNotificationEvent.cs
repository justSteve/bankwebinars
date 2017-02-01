using CUWebinars.Business.Notification.Email;
using CUWebinars.Business.Notification.ViewModel;
using CUWebinars.NotificationSystem.Event;

namespace CUWebinars.Business.Notification.Events
{
    public class MandrillNotificationEvent<T> : TtsBusEvent<T>, IAllowMultiple, INotificationResendableEvent
        where T : MandrillNotificationMessage
    {
        public string RelativePath { get; set; }
        public bool ResendEvent { get; set; }
    }
}
