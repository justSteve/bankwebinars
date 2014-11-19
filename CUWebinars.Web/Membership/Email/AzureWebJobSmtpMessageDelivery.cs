using BrockAllen.MembershipReboot;
using CUWebinars.Business.Constants;
using CUWebinars.Web.Core;
using CUWebinars.Web.Services;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;

namespace CUWebinars.Web.Membership.Email
{
    public class AzureWebJobSmtpMessageDelivery : IMessageDelivery
    {
        private readonly GlobalConfig _globalConfig = GlobalConfig.GlobalConfigSingleton;
        private readonly IStateService _stateService;
        private static CloudQueueClient _queueClient;


        public AzureWebJobSmtpMessageDelivery(IStateService stateService)
        {
            _stateService = stateService;
        }

        public void Send(Message msg)
        {
            if (_stateService.HasValue(DomainConstants.UserCreatedViaNewOrder))
            {
                return;
            }

            var storageCredentials = new StorageCredentials(_globalConfig.StorageAccountName, _globalConfig.StorageAccessKey);
            var cloudStorageAccount = new CloudStorageAccount(storageCredentials, false);
            
            _queueClient = cloudStorageAccount.CreateCloudQueueClient();
            
            CloudQueue cloudQueue = _queueClient.GetQueueReference("tts-mr-notifications-queue");

            if (ReferenceEquals(null, msg.From))
                msg.From = string.Empty;

            var cloudQueueMessage = new CloudQueueMessage(JsonConvert.SerializeObject(msg));
            cloudQueue.EncodeMessage = true;
            cloudQueue.AddMessage(cloudQueueMessage);

        }
    }
}