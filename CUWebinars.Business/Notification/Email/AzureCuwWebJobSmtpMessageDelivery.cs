using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;
using System;

namespace CUWebinars.Business.Notification.Email
{
    public class AzureCuwWebJobSmtpMessageDelivery : INotificationDelivery
    {
        private static CloudQueueClient _queueClient;
        private readonly string _storageAccountName;
        private readonly string _storageAccessKey;

        public AzureCuwWebJobSmtpMessageDelivery(string storageAccountName, string storageAccessKey)
        {
            _storageAccountName = storageAccountName;
            _storageAccessKey = storageAccessKey;
        }


        public void Notify(INotificationMessage notificationMessage)
        {
            var storageCredentials = new StorageCredentials(_storageAccountName, _storageAccessKey);
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
