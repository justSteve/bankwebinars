using BrockAllen.MembershipReboot.WebHost;
using CUWebinars.Business.Notification;
using CUWebinars.Business.Notification.Email;

namespace CUWebinars.Business.Core
{
    public class TtsConfig
    {
        public static TtsConfiguration Create(string baseUrl)
        {
            var config = new TtsConfiguration();

            var notificationDelivery = new SmtpMessageDelivery();
            var notificationFormatter = new NotificationFormatter(new EnvironmentInformation{ BaseUrl = baseUrl });

            config.AddEventHandler(new NotificationOrderHandler(notificationFormatter, notificationDelivery));

            return config;
        }
    }
}