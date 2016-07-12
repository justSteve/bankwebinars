using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;
using System;
using Ninject.Extensions.Logging;

namespace CUWebinars.Business.Notification.Email
{
    public class AzureWeeklyInvoiceWebJobSmtpMessageDelivery : INotificationDelivery
    {
        private CloudQueueClient _queueClient;
        private readonly string _storageAccountName;
        private readonly string _storageAccessKey;
        private readonly ILogger _logger;

        public AzureWeeklyInvoiceWebJobSmtpMessageDelivery(string storageAccountName, string storageAccessKey, ILogger logger)
        {
            _storageAccountName = storageAccountName;
            _storageAccessKey = storageAccessKey;
            _logger = logger;
        }


        public void Notify(INotificationMessage notificationMessage)
        {
            //_logger.Info("Sending notification with PersistedName {0}", notificationMessage.PersistedName);

            var storageCredentials = new StorageCredentials(_storageAccountName, _storageAccessKey);
            var cloudStorageAccount = new CloudStorageAccount(storageCredentials, false);

            _queueClient = cloudStorageAccount.CreateCloudQueueClient();

            CloudQueue cloudQueue = _queueClient.GetQueueReference("invoicesender"); // make this config driven?
            cloudQueue.CreateIfNotExists();

            EnsureMessage(notificationMessage);

            var cloudQueueMessage = new CloudQueueMessage(JsonConvert.SerializeObject(notificationMessage));
            cloudQueue.EncodeMessage = true;
            //TEMP-ALS cloudQueue.AddMessage(cloudQueueMessage);

            _logger.Info(string.Format("Queued WeeklyInvoice message to: {0}, message is: {1} ", notificationMessage.To, notificationMessage.Body));
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
