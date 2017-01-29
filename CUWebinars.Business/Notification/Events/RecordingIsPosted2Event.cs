using CUWebinars.Business.Notification.Email;
using CUWebinars.Business.Notification.ViewModel;
using CUWebinars.NotificationSystem.Event;

namespace CUWebinars.Business.Notification.Events
{
    public class RecordingIsPosted2Event<T> : TtsBusEvent<T>, IAllowMultiple, INotificationResendableEvent
        where T : RecordingIsPosted2Message
    {
        public string RelativePath { get; set; }
        public bool ResendEvent { get; set; }
    }
}
