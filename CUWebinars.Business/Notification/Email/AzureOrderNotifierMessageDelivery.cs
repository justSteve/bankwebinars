using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;
using Ninject.Extensions.Logging;

namespace CUWebinars.Business.Notification.Email
{
    public class AzureOrderNotifierMessageDelivery : IOrderConfirmedNotificationDelivery
    {
        private readonly string _storageAccountName;
        private readonly string _storageAccessKey;
        private readonly ILogger _logger;
        private readonly string _baseUrl;
        private CloudQueueClient _queueClient;

        public AzureOrderNotifierMessageDelivery(string storageAccountName, string storageAccessKey, ILogger logger, string baseUrl)
        {
            _storageAccountName = storageAccountName;
            _storageAccessKey = storageAccessKey;
            _logger = logger;
            _baseUrl = baseUrl;
        }

        public void Notify(IConfirmOrderMessage confirmOrderMessage)
        {
            _logger.Info("Enqueuing confirmation of Order {0}", confirmOrderMessage.idOrder);

            confirmOrderMessage.Order = null; // only relevent for non-webjob versions of delivery classes. Not serializable.
            confirmOrderMessage.BaseUrl = _baseUrl;

            var storageCredentials = new StorageCredentials(_storageAccountName, _storageAccessKey);
            var cloudStorageAccount = new CloudStorageAccount(storageCredentials, false);

            _queueClient = cloudStorageAccount.CreateCloudQueueClient();

            CloudQueue cloudQueue = _queueClient.GetQueueReference("tts-cuw-ordernotifier-queue");
            cloudQueue.CreateIfNotExists();
            //EnsureMessage(confirmOrderMessage);

            var cloudQueueMessage = new CloudQueueMessage(JsonConvert.SerializeObject(confirmOrderMessage));
            cloudQueue.EncodeMessage = true;
            cloudQueue.AddMessage(cloudQueueMessage);

            _logger.Info("Notification successfully enqueued");            
        }
    }
}
