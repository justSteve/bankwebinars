namespace CUWebinars.Business.Notification
{
    public interface INotificationPersister
    {
        void PersistNotification(string notification, string connectionDetails);
    }
}