using CUWebinars.Business.Notification.Email;
using CUWebinars.NotificationSystem.Event;

namespace CUWebinars.Business.Notification.Events
{
    public class OrderSubmittedAdditionalLocationEvent<T> : TtsBusEvent<T>, IAllowMultiple, INotificationResendableEvent
        where T : AdditionalLocationOrderDetailsMessage
    {
        public string RelativeFilePath { get; set; }
        public bool ResendEvent { get; set; }
    }
}