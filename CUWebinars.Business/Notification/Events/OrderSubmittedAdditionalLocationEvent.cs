using CUWebinars.Business.Notification.ViewModel;
using CUWebinars.NotificationSystem.Event;

namespace CUWebinars.Business.Notification.Events
{
    public class OrderSubmittedAdditionalLocationEvent<T> : NotificationResendableEvent<T>, IAllowMultiple
        where T : OrderSubmittedAdditionalLocationViewModel
    {
        public string RelativeFilePath { get; set; }
    }
}