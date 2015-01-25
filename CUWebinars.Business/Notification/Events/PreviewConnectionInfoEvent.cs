using CUWebinars.Business.Notification.ViewModel;
using CUWebinars.NotificationSystem.Event;

namespace CUWebinars.Business.Notification.Events
{
    public class PreviewConnectionInfoEvent<T> : TtsBusEvent<T>, IAllowMultiple
        where T : OrderSubmittedViewModel
    {
    }
}
