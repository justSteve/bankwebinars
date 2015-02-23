using CUWebinars.Business.Models;
using CUWebinars.Business.Notification.ViewModel;
using CUWebinars.NotificationSystem.Event;

namespace CUWebinars.Business.Notification.Events
{
    public class SendRecordingPostedEvent<T> : TtsBusEvent<T>, IAllowMultiple, INotificationResendableEvent
        where T : PostEventPublishModel
    {
        public string RelativePath { get; set; }
        public bool ResendEvent { get; set; }
    }
}
