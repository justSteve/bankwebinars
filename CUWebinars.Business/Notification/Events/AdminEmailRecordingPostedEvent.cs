using CUWebinars.Business.Models;

namespace CUWebinars.Business.Notification.Events
{
    public class AdminEmailRecordingPostedEvent<T> : AdminEmailEvent<T>
        where T: Order
    {
    }
}
