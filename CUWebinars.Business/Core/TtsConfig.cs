using CUWebinars.Business.Notification;
using CUWebinars.Business.Notification.Email;
using CUWebinars.Business.Notification.Formatters;
using CUWebinars.Business.Notification.Handlers;

namespace CUWebinars.Business.Core
{
    public class TtsConfig
    {
        public static TtsConfiguration Create(string baseUrl)
        {
            var config = new TtsConfiguration();

            var notificationDelivery = new SmtpMessageDelivery();
            var orderNotificationFormatter = new OrderNotificationFormatter(new EnvironmentInformation{ BaseUrl = baseUrl });
            var genericFormatter = new Formatter(new EnvironmentInformation {BaseUrl = baseUrl});

            config.AddEventHandler(new NotificationOrderHandler(orderNotificationFormatter, notificationDelivery));
            config.AddEventHandler(new SendShippedOrderHandler(genericFormatter, notificationDelivery));

            return config;
        }
    }
}