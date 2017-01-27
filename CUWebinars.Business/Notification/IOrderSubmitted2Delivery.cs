namespace CUWebinars.Business.Notification
{
    public interface IOrderSubmitted2Delivery
    {
        void Notify(INotificationMessage notificationMessage);
    }
}
