using CUWebinars.Business.Notification.ViewModel;
using CUWebinars.NotificationSystem.Event;

namespace CUWebinars.Business.Notification.Events
{
    public class EmailOrderEvent<T> : TtsBusEvent<T>, IAllowMultiple
        where T : OrderSubmittedViewModel
    {
    }
}
