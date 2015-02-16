
namespace CUWebinars.Business.Notification.Events
{
    public interface INotificationResendableEvent
    {
        bool ResendEvent { get; set; }
    }
}
