using CUWebinars.Business.Notification;
using CUWebinars.Business.Notification.Email;
using CUWebinars.Business.Notification.Formatters;
using CUWebinars.Business.Notification.Handlers;
using Ninject.Extensions.Logging.Log4net.Infrastructure;

namespace CUWebinars.Business.Core
{
    public class TtsConfig
    {
        public static TtsConfiguration Create(string baseUrl, bool useAzureWebjobs)
        {
            var config = new TtsConfiguration();

            INotificationDelivery notificationDelivery;

            // toggle whether to use Azure Webjobs or local code (for local debugiing/development purposes)
            if (useAzureWebjobs)
                notificationDelivery = new AzureCuwWebJobSmtpMessageDelivery();
            else
                notificationDelivery = new SmtpMessageDelivery();

            var genericFormatter = new Formatter(new EnvironmentInformation {BaseUrl = baseUrl});
            var notificationPersister = new FileBasedNotificationPersister();

            var notificationOrderHandlerLogger = new Log4NetLogger(typeof (OrderSubmittedHandler));
            var notificationOrderAdditionalLocationHandlerLogger =
                new Log4NetLogger(typeof (OrderSubmittedAdditionalLocationHandler));
            var sendShippedOrderHandlerLogger = new Log4NetLogger(typeof (SendShippedOrderHandler));
            var sendConnectionInfoHandlerLogger = new Log4NetLogger(typeof (SendConnectionInfoHandler));
            var sendReminderHandlerLogger = new Log4NetLogger(typeof (SendReminderHandler));
            var sendRecordingPostedHandlerLogger = new Log4NetLogger(typeof (SendRecordingPostedHandler));

            config.AddEventHandler(new OrderSubmittedHandler(genericFormatter, notificationDelivery,
                notificationOrderHandlerLogger, notificationPersister, new EnvironmentInformation {BaseUrl = baseUrl}));
            config.AddEventHandler(new OrderSubmittedAdditionalLocationHandler(genericFormatter, notificationDelivery,
                notificationOrderHandlerLogger, notificationPersister, new EnvironmentInformation {BaseUrl = baseUrl}));
            config.AddEventHandler(new SendShippedOrderHandler(genericFormatter, notificationDelivery,
                sendShippedOrderHandlerLogger));
            config.AddEventHandler(new SendConnectionInfoHandler(genericFormatter, notificationDelivery,
                sendConnectionInfoHandlerLogger));
            config.AddEventHandler(new SendReminderHandler(genericFormatter, notificationDelivery,
                sendReminderHandlerLogger));
            config.AddEventHandler(new SendRecordingPostedHandler(genericFormatter, notificationDelivery,
                sendRecordingPostedHandlerLogger));

            return config;
        }
    }
}
