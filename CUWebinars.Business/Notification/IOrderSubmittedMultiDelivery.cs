namespace CUWebinars.Business.Notification
{
    public interface IOrderSubmittedMultiDelivery
    {
        void Notify(INotificationMessage notificationMessage);
    }
}
