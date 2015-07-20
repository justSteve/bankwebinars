
namespace CUWebinars.Business.Notification
{
    public interface IAdhocNotificationDelivery
    {
        void Notify(IAdhocNotificationMessage adhocNotificationMessage);
    }
}
