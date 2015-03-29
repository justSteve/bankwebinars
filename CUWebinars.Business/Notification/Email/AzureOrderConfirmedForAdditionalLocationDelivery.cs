using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;
using Ninject.Extensions.Logging;

namespace CUWebinars.Business.Notification.Email
{
    public class AzureOrderConfirmedForAdditionalLocationDelivery : IOrderConfirmedForAdditionalLocationDelivery
    {
        private readonly string _storageAccountName;
        private readonly string _storageAccessKey;
        private readonly ILogger _logger;
        private readonly string _baseUrl;
        private CloudQueueClient _queueClient;

        public AzureOrderConfirmedForAdditionalLocationDelivery(string storageAccountName, string storageAccessKey, ILogger logger, string baseUrl)
        {
            _storageAccountName = storageAccountName;
            _storageAccessKey = storageAccessKey;
            _logger = logger;
            _baseUrl = baseUrl;
        }

        public void Notify(IAdditionalLocationOrderDetailsMessage additionalLocationOrderDetailsMessage)
        {
            _logger.Info("Enqueuing confirmation of Order for additional location - orderId {0}", additionalLocationOrderDetailsMessage.idOrder);

            additionalLocationOrderDetailsMessage.BaseUrl = _baseUrl;
            additionalLocationOrderDetailsMessage.Order = null; // only relevent for non-webjob versions of delivery classes. Not serializable.

            var storageCredentials = new StorageCredentials(_storageAccountName, _storageAccessKey);
            var cloudStorageAccount = new CloudStorageAccount(storageCredentials, false);

            _queueClient = cloudStorageAccount.CreateCloudQueueClient();

            CloudQueue cloudQueue = _queueClient.GetQueueReference("tts-cuw-addlocordernotifier-queue");
            cloudQueue.CreateIfNotExists();

            var cloudQueueMessage = new CloudQueueMessage(JsonConvert.SerializeObject(additionalLocationOrderDetailsMessage));
            cloudQueue.EncodeMessage = true;
            cloudQueue.AddMessage(cloudQueueMessage);

            _logger.Info("Notification successfully enqueued");
        }

    }
}
