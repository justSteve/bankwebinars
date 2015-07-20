using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;
using Ninject.Extensions.Logging;

namespace CUWebinars.Business.Notification.Email
{
    public class AzureAdhocNotificationDelivery : IAdhocNotificationDelivery
    {
        private readonly string _storageAccountName;
        private readonly string _storageAccessKey;
        private readonly ILogger _logger;
        private readonly string _baseUrl;
        private CloudQueueClient _queueClient;

        public AzureAdhocNotificationDelivery(string storageAccountName, string storageAccessKey, ILogger logger, string baseUrl)
        {
            _storageAccountName = storageAccountName;
            _storageAccessKey = storageAccessKey;
            _logger = logger;
            _baseUrl = baseUrl;
        }

        public void Notify(IAdhocNotificationMessage adhocNotificationMessage)
        {
            _logger.Info("Enqueuing adhoc notification.");

            adhocNotificationMessage.BaseUrl = _baseUrl;

            var storageCredentials = new StorageCredentials(_storageAccountName, _storageAccessKey);
            var cloudStorageAccount = new CloudStorageAccount(storageCredentials, false);

            _queueClient = cloudStorageAccount.CreateCloudQueueClient();

            CloudQueue cloudQueue = _queueClient.GetQueueReference("tts-cuw-adhocnotifier-queue");
            cloudQueue.CreateIfNotExists();

            var cloudQueueMessage = new CloudQueueMessage(JsonConvert.SerializeObject(adhocNotificationMessage));
            cloudQueue.EncodeMessage = true;
            cloudQueue.AddMessage(cloudQueueMessage);

            _logger.Info("Notification successfully enqueued.");            
        }
    }
}