using BrockAllen.MembershipReboot;
using CUWebinars.Business.Constants;
using CUWebinars.Web.Core;
using CUWebinars.Web.Services;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;
using Ninject.Extensions.Logging;

namespace CUWebinars.Web.Membership.Email
{
    public class AzureWebJobSmtpMessageDelivery : IMessageDelivery
    {
        private readonly GlobalConfig _globalConfig = GlobalConfig.GlobalConfigSingleton;
        private readonly IStateService _stateService;
        private readonly ILogger _logger;
        private static CloudQueueClient _queueClient;


        public AzureWebJobSmtpMessageDelivery(IStateService stateService, ILogger logger)
        {
            _stateService = stateService;
            _logger = logger;
        }

        public void Send(Message msg)
        {
            if (_stateService.HasValue(DomainConstants.UserCreatedViaNewOrder) ||
                _stateService.HasValue(DomainConstants.UserCreatedDuringCartCheckout) || 
                _stateService.HasValue(DomainConstants.UserCreatedViaMigrator) ||
                _stateService.HasValue(DomainConstants.CartCreatedUserPasswordCreate) ||
                msg.Subject.Contains("Email Account Verified"))
            {
                _stateService.ClearValue(DomainConstants.UserCreatedDuringCartCheckout);
                return;
            }

            _logger.Info("Sending MR notification to {0}. Subject: {1} ", msg.To, msg.Subject);

            var storageCredentials = new StorageCredentials(_globalConfig.StorageAccountName, _globalConfig.StorageAccessKey);
            var cloudStorageAccount = new CloudStorageAccount(storageCredentials, false);
            
            _queueClient = cloudStorageAccount.CreateCloudQueueClient();
            
            CloudQueue cloudQueue = _queueClient.GetQueueReference("tts-mr-notifications-queue");

            if (ReferenceEquals(null, msg.From))
                msg.From = string.Empty;

            var cloudQueueMessage = new CloudQueueMessage(JsonConvert.SerializeObject(msg));
            cloudQueue.EncodeMessage = true;
            cloudQueue.AddMessage(cloudQueueMessage);

            _logger.Info("Notification successfully enqueued AzureWebJobSmtpMessageDelivery" + msg.To);
        }
    }
}