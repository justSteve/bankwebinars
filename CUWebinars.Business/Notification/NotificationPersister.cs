using System.IO;

namespace CUWebinars.Business.Notification
{
    public class FileBasedNotificationPersister : INotificationPersister
    {
        public FileBasedNotificationPersister()
        {
            
        }

        public void PersistNotification(string notification, string connectionDetails)
        {
            using (var writer = new StreamWriter(connectionDetails))
            {
                writer.Write(notification);
                writer.Flush();
            }
        }
    }
}
