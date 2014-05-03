using CUWebinars.Business.Models;

namespace CUWebinars.Business.Notification.Events
{
    public class SendConnectionInfoEvent<T> : TtsBusEvent<T>
        where T : Order
    {
    }
}
