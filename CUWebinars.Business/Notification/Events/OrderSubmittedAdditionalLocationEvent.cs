using CUWebinars.Business.Notification.ViewModel;
using CUWebinars.NotificationSystem.Event;

namespace CUWebinars.Business.Notification.Events
{
    public class OrderSubmittedAdditionalLocationEvent<T> : TtsBusEvent<T>, IAllowMultiple, INotificationResendableEvent
        where T : OrderSubmittedAdditionalLocationViewModel
    {
        public string RelativeFilePath { get; set; }
        public bool ResendEvent { get; set; }
    }
}