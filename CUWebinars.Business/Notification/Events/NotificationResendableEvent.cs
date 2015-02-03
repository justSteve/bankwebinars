
namespace CUWebinars.Business.Notification.Events
{
    public class NotificationResendableEvent<T> : TtsBusEvent<T>
    {
        public bool ResendEvent { get; set; }
    }
}
