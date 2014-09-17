using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;

namespace CUWebinars.Business.Notification.Email
{
    public class AzureCuwWebJobSmtpMessageDelivery : INotificationDelivery
    {
        private static CloudQueueClient _queueClient;
        const string name = "cuwebinarsnotifications";
        const string key = "R6+DUPUtVBKXbnoRTPfdzYXF3KeCcVZdKtpgSig2LJYne52rc6MGU+dgTadzAHbEubBjOhAoB3l8IHMdC8Prgg==";


        public void Notify(INotificationMessage notificationMessage)
        {
            var storageCredentials = new StorageCredentials(name, key);
            var cloudStorageAccount = new CloudStorageAccount(storageCredentials, false);

            _queueClient = cloudStorageAccount.CreateCloudQueueClient();

            CloudQueue cloudQueue = _queueClient.GetQueueReference("tts-cuw-notifications-queue");

            if (ReferenceEquals(null, notificationMessage.From))
                notificationMessage.From = string.Empty;

            var cloudQueueMessage = new CloudQueueMessage(JsonConvert.SerializeObject(notificationMessage));
            cloudQueue.EncodeMessage = true;
            cloudQueue.AddMessage(cloudQueueMessage);
        }
    }
}
