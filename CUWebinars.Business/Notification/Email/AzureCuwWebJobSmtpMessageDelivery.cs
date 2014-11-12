using System;
using System.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;

namespace CUWebinars.Business.Notification.Email
{
    public class AzureCuwWebJobSmtpMessageDelivery : INotificationDelivery
    {
        private static CloudQueueClient _queueClient;
        private const string storageAccountName = "cuwebinarsnotifications";

        private const string accessKey =
            "R6+DUPUtVBKXbnoRTPfdzYXF3KeCcVZdKtpgSig2LJYne52rc6MGU+dgTadzAHbEubBjOhAoB3l8IHMdC8Prgg==";


        public void Notify(INotificationMessage notificationMessage)
        {
            var storageCredentials = new StorageCredentials(storageAccountName, accessKey);
            var cloudStorageAccount = new CloudStorageAccount(storageCredentials, false);

            _queueClient = cloudStorageAccount.CreateCloudQueueClient();

            CloudQueue cloudQueue = _queueClient.GetQueueReference("tts-cuw-notifications-queue");

            EnsureMessage(notificationMessage);

            var cloudQueueMessage = new CloudQueueMessage(JsonConvert.SerializeObject(notificationMessage));
            cloudQueue.EncodeMessage = true;
            cloudQueue.AddMessage(cloudQueueMessage);
        }

        private void EnsureMessage(INotificationMessage message)
        {
            if (ReferenceEquals(null, message.From))
            {
                message.From = string.Empty;
            }
            
            if (ReferenceEquals(null, message.To))
            {
                throw new Exception("Message had no recipient.");
            }

#if DEBUG
            message.To = ConfigurationManager.AppSettings["TestEmailAddress"];
#endif

            if (ReferenceEquals(null, message.ReplyTo))
            {
                message.ReplyTo = string.Empty;
            }

            if (ReferenceEquals(null, message.Subject))
            {
                message.Subject = string.Empty;
            }

            if (ReferenceEquals(null, message.Body))
            {
                throw new Exception("Message had no content.");
            }
        }
    }
}
