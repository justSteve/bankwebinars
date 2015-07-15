using CUWebinars.Business.Notification.Email;

namespace CUWebinars.Business.Notification.Events
{
    public class AdhocNotificationEvent<T> : AdminEmailEvent<T>
        where T : AdhocNotificationMessage
    {
    }
}
