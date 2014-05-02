using CUWebinars.Business.Models;

namespace CUWebinars.Business.Notification.Events
{
    public class SendShippedOrderEvent<T> : TtsBusEvent<T>
        where T : Order
    {
    }
}
