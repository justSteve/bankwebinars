using System.Collections.Specialized;
using System.Configuration;
using CUWebinars.Business.Notification;
using CUWebinars.Business.Notification.Email;
using CUWebinars.Business.Notification.Formatters;
using CUWebinars.Business.Notification.Handlers;
using Ninject.Extensions.Logging.Log4net.Infrastructure;

namespace CUWebinars.Business.Core
{
    public class TtsConfig
    {
        public static string DefaultConnectionString { get; private set; }
        public static string TracingLevel { get; private set; }
        public static TtsConfiguration Create(
            string baseUrl,
            bool useAzureWebjobs,
            string storageAccountName,
            string storageAccessKey,
            string tracingLevel)
        {
            InitializeConfig();

            TracingLevel = tracingLevel;

            var genericFormatter = new Formatter(new EnvironmentInformation { BaseUrl = baseUrl });
            var config = new TtsConfiguration();

            INotificationDelivery notificationDelivery;
            IOrderConfirmedNotificationDelivery orderConfirmationDelivery;
            IOrderConfirmedForAdditionalLocationDelivery orderConfirmedForAdditionalLocationDelivery;

            // toggle whether to use Azure Webjobs or local code (for local debugiing/development purposes)
            if (useAzureWebjobs)
            {
                notificationDelivery = new AzureCuwWebJobSmtpMessageDelivery(storageAccountName, storageAccessKey,
                    new Log4NetLogger(typeof (AzureCuwWebJobSmtpMessageDelivery)));
                orderConfirmationDelivery =
                    new AzureOrderNotifierMessageDelivery(
                        storageAccountName,
                        storageAccessKey,
                        new Log4NetLogger(typeof (AzureOrderNotifierMessageDelivery)),
                        baseUrl
                        );
                orderConfirmedForAdditionalLocationDelivery =
                    new AzureOrderConfirmedForAdditionalLocationDelivery(storageAccountName,
                        storageAccessKey,
                        new Log4NetLogger(typeof(AzureOrderConfirmedForAdditionalLocationDelivery)),
                        baseUrl
                        );
            }
            else
            {
                notificationDelivery = new SmtpMessageDelivery(new Log4NetLogger(typeof (SmtpMessageDelivery)));
                orderConfirmationDelivery =
                    new OrderNotifierMessageDelivery(new Log4NetLogger(typeof (OrderNotifierMessageDelivery)),
                        genericFormatter
                        );
                orderConfirmedForAdditionalLocationDelivery =
                    new OrderConfirmedForAdditionalLocationDelivery(
                        new Log4NetLogger(typeof (OrderConfirmedForAdditionalLocationDelivery)), genericFormatter
                        );
            }

            var notificationOrderHandlerLogger = new Log4NetLogger(typeof(OrderSubmittedHandler));
            //var notificationOrderAdditionalLocationHandlerLogger =
            //    new Log4NetLogger(typeof (OrderSubmittedAdditionalLocationHandler));
            var sendShippedOrderHandlerLogger = new Log4NetLogger(typeof(SendShippedOrderHandler));
            var sendConnectionInfoHandlerLogger = new Log4NetLogger(typeof(SendConnectionInfoHandler));
            var sendReminderHandlerLogger = new Log4NetLogger(typeof(SendReminderHandler));
            var sendRecordingPostedHandlerLogger = new Log4NetLogger(typeof(SendRecordingPostedHandler));
            var emailOrderHandlerLogger = new Log4NetLogger(typeof(AdminEmailSendShippedOrderHandler));
            var emailConnectionInfoHandlerLogger = new Log4NetLogger(typeof(AdminEmailConnectionInfoHandler));
            var emailRecordingPostedHandlerLogger = new Log4NetLogger(typeof(AdminEmailRecordingPostedHandler));
            var sendPerDayPromoHandlerLogger = new Log4NetLogger(typeof(SendPerDayPromoHandler));
            var sendPerWeekPromoHandlerLogger = new Log4NetLogger(typeof(SendPerWeekPromoHandler));


            config.AddEventHandler(new SendPerDayPromoHandler(genericFormatter, notificationDelivery,sendPerDayPromoHandlerLogger));
            //config.AddEventHandler(new SendPerWeekPromoHandler(genericFormatter, notificationDelivery, sendPerWeekPromoHandlerLogger));
            config.AddEventHandler(new OrderSubmittedHandler(orderConfirmationDelivery, notificationOrderHandlerLogger));
            config.AddEventHandler(new OrderSubmittedAdditionalLocationHandler(genericFormatter, orderConfirmedForAdditionalLocationDelivery, notificationOrderHandlerLogger));
            config.AddEventHandler(new SendShippedOrderHandler(genericFormatter, notificationDelivery,sendShippedOrderHandlerLogger));
            config.AddEventHandler(new SendConnectionInfoHandler(genericFormatter, notificationDelivery, sendConnectionInfoHandlerLogger, new EnvironmentInformation { BaseUrl = baseUrl }));
            config.AddEventHandler(new SendReminderHandler(genericFormatter, notificationDelivery, sendReminderHandlerLogger));
            config.AddEventHandler(new SendRecordingPostedHandler(genericFormatter, notificationDelivery, sendRecordingPostedHandlerLogger));
            config.AddEventHandler(new AdminEmailSendShippedOrderHandler(genericFormatter, emailOrderHandlerLogger, notificationDelivery));
            config.AddEventHandler(new AdminEmailConnectionInfoHandler(genericFormatter, emailConnectionInfoHandlerLogger, notificationDelivery));
            config.AddEventHandler(new AdminEmailRecordingPostedHandler(genericFormatter, emailRecordingPostedHandlerLogger, notificationDelivery));

            return config;
        }

        private static void InitializeConfig()
        {
            NameValueCollection applicationSettingsSection = ConfigurationManager.AppSettings;
            ConnectionStringSettingsCollection connectionStringSettingsCollection = ConfigurationManager.ConnectionStrings;

            DefaultConnectionString = connectionStringSettingsCollection["DefaultConnection"].ConnectionString;
        }
    }
}
