using CUWebinars.Business.Models;
using CUWebinars.NotificationSystem.Event;
using CUWebinars.Business.Notification.ViewModel;

namespace CUWebinars.Business.Notification.Events
{
    public class SendPerDayPromoEvent<T> : TtsBusEvent<T>, IAllowMultiple
        where T : WebinarPromoViewModel
    {
    }
}
