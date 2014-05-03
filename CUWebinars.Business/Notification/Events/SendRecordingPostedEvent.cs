using CUWebinars.Business.Models;

namespace CUWebinars.Business.Notification.Events
{
    public class SendRecordingPostedEvent<T> : TtsBusEvent<T>
        where T : Order
    {
    }
}
