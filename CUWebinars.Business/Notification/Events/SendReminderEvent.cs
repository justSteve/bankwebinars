using CUWebinars.Business.Models;

namespace CUWebinars.Business.Notification.Events
{
    public class SendReminderEvent<T> : TtsBusEvent<T>
        where T : Order
    {

    }
}
