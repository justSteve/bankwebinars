using CUWebinars.Web.Notification;

namespace CUWebinars.Business.Notification
{
    public interface INotificationDelivery
    {
        void Notify(INotificationMessage notificationMessage);
    }
}
