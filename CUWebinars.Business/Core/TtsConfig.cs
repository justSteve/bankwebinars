using CUWebinars.Business.Notification;
using CUWebinars.Business.Notification.Email;
using CUWebinars.Business.Notification.Formatters;

namespace CUWebinars.Business.Core
{
    public class TtsConfig
    {
        public static TtsConfiguration Create(string baseUrl)
        {
            var config = new TtsConfiguration();

            var notificationDelivery = new SmtpMessageDelivery();
            var orderNotificationFormatter = new OrderNotificationFormatter(new EnvironmentInformation{ BaseUrl = baseUrl });

            config.AddEventHandler(new CUWebinars.Business.Notification.Handlers.NotificationOrderHandler(orderNotificationFormatter, notificationDelivery));

            return config;
        }
    }
}