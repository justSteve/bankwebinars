using System;
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
        public static string CitrixOrgKeyMark { get; private set; }
        public static string CitrixAuthMark { get; private set; }
        //public static string LegacyConnectionString { get; private set; }
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

            var ttsConfigHelper = new TtsConfigHelper();

            INotificationDelivery notificationDelivery;
            IOrderConfirmedNotificationDelivery orderConfirmationDelivery;
            IOrderConfirmedForAdditionalLocationDelivery orderConfirmedForAdditionalLocationDelivery;
            IAdhocNotificationDelivery adhocNotificationDelivery;
            INotificationDelivery weeklyInvoiceDelivery;
            INotificationDelivery mandrillNotificationDelivery;

            // toggle whether to use Azure Webjobs or local code (for local debugiing/development purposes)
            if (useAzureWebjobs)
            {
                notificationDelivery = new AzureCuwWebJobSmtpMessageDelivery(
                    storageAccountName,
                    storageAccessKey,
                    ttsConfigHelper.GetCuwNotificationQueueName(),
                    new Log4NetLogger(typeof(AzureCuwWebJobSmtpMessageDelivery))
                    );

                // new Mandrill approach, always through Azure
                weeklyInvoiceDelivery = new AzureWeeklyInvoiceWebJobSmtpMessageDelivery(storageAccountName, storageAccessKey,
                    new Log4NetLogger(typeof(AzureWeeklyInvoiceWebJobSmtpMessageDelivery)));

                orderConfirmationDelivery =
                    new AzureOrderNotifierMessageDelivery(
                        storageAccountName,
                        storageAccessKey,
                        new Log4NetLogger(typeof(AzureOrderNotifierMessageDelivery)),
                        baseUrl
                        );

                orderConfirmedForAdditionalLocationDelivery =
                    new AzureOrderConfirmedForAdditionalLocationDelivery(storageAccountName,
                        storageAccessKey,
                        new Log4NetLogger(typeof(AzureOrderConfirmedForAdditionalLocationDelivery)),
                        baseUrl
                        );

                adhocNotificationDelivery = new AzureAdhocNotificationDelivery(
                    storageAccountName,
                    storageAccessKey,
                    new Log4NetLogger(typeof(AzureAdhocNotificationDelivery)),
                    baseUrl);

                // new Mandrill approach, always through Azure
                mandrillNotificationDelivery = new AzureCuwWebJobSmtpMessageDelivery(
                    storageAccountName,
                    storageAccessKey,
                    ttsConfigHelper.GetMandrillQueueName(),
                    new Log4NetLogger(typeof(AzureCuwWebJobSmtpMessageDelivery))
                    );
            }
            else
            {
                notificationDelivery = new SmtpMessageDelivery(new Log4NetLogger(typeof(SmtpMessageDelivery)));
                //testing out new Mandrill approach
                //notificationDelivery = new AzureCuwWebJobSmtpMessageDelivery(storageAccountName, storageAccessKey,
                //    new Log4NetLogger(typeof(AzureCuwWebJobSmtpMessageDelivery)));

                // new Mandrill approach, always through Azure
                weeklyInvoiceDelivery = new AzureWeeklyInvoiceWebJobSmtpMessageDelivery(storageAccountName, storageAccessKey,
                    new Log4NetLogger(typeof(AzureWeeklyInvoiceWebJobSmtpMessageDelivery)));

                orderConfirmationDelivery =
                    new OrderNotifierMessageDelivery(new Log4NetLogger(typeof(OrderNotifierMessageDelivery)),
                        genericFormatter
                        );

                orderConfirmedForAdditionalLocationDelivery =
                    new OrderConfirmedForAdditionalLocationDelivery(
                        new Log4NetLogger(typeof(OrderConfirmedForAdditionalLocationDelivery)), genericFormatter
                        );

                adhocNotificationDelivery = new AdhocNotificationDelivery(
                    new Log4NetLogger(typeof(AzureAdhocNotificationDelivery)),
                    genericFormatter);

                // new Mandrill approach, always through Azure
                mandrillNotificationDelivery = new AzureCuwWebJobSmtpMessageDelivery(
                    storageAccountName,
                    storageAccessKey,
                    ttsConfigHelper.GetMandrillQueueName(),
                    new Log4NetLogger(typeof(AzureCuwWebJobSmtpMessageDelivery))
                    );
            }

            //var notificationOrderHandlerLogger = new Log4NetLogger(typeof(OrderSubmittedHandler));
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
            var adhocNotificationHandlerLogger = new Log4NetLogger(typeof(AdhocNotificationHandler));
            var sendWeeklyInvoiceHandlerLogger = new Log4NetLogger(typeof(SendWeeklyInvoiceHandler));
            var sendOrderSubmittedMultiHandlerLogger = new Log4NetLogger(typeof(OrderSubmittedMultiHandler));
            var sendOrderSubmitted2HandlerLogger = new Log4NetLogger(typeof(OrderSubmitted2Handler));
            var sendRecordingIsPosted2HandlerLogger = new Log4NetLogger(typeof(RecordingIsPosted2Handler));


            config.AddEventHandler(new SendPerDayPromoHandler(genericFormatter, notificationDelivery, sendPerDayPromoHandlerLogger));
            //config.AddEventHandler(new SendPerWeekPromoHandler(genericFormatter, notificationDelivery, sendPerWeekPromoHandlerLogger));
            //config.AddEventHandler(new OrderSubmittedHandler(orderConfirmationDelivery, notificationOrderHandlerLogger));
            //config.AddEventHandler(new OrderSubmittedAdditionalLocationHandler(genericFormatter, orderConfirmedForAdditionalLocationDelivery, notificationOrderHandlerLogger));
            config.AddEventHandler(new SendShippedOrderHandler(genericFormatter, notificationDelivery, sendShippedOrderHandlerLogger));
            config.AddEventHandler(new SendConnectionInfoHandler(genericFormatter, notificationDelivery, sendConnectionInfoHandlerLogger, new EnvironmentInformation { BaseUrl = baseUrl }));
            config.AddEventHandler(new SendReminderHandler(genericFormatter, notificationDelivery, sendReminderHandlerLogger));
            config.AddEventHandler(new SendRecordingPostedHandler(genericFormatter, notificationDelivery, sendRecordingPostedHandlerLogger));
            config.AddEventHandler(new AdminEmailSendShippedOrderHandler(genericFormatter, emailOrderHandlerLogger, notificationDelivery));
            config.AddEventHandler(new AdminEmailConnectionInfoHandler(genericFormatter, emailConnectionInfoHandlerLogger, notificationDelivery));
            config.AddEventHandler(new AdminEmailRecordingPostedHandler(genericFormatter, emailRecordingPostedHandlerLogger, notificationDelivery));
            config.AddEventHandler(new AdhocNotificationHandler(adhocNotificationDelivery, adhocNotificationHandlerLogger));
            config.AddEventHandler(new SendWeeklyInvoiceHandler(genericFormatter, weeklyInvoiceDelivery, sendWeeklyInvoiceHandlerLogger));
            config.AddEventHandler(new OrderSubmittedMultiHandler(mandrillNotificationDelivery, sendOrderSubmittedMultiHandlerLogger));
            config.AddEventHandler(new OrderSubmitted2Handler(mandrillNotificationDelivery, sendOrderSubmitted2HandlerLogger));
            config.AddEventHandler(new RecordingIsPosted2Handler(mandrillNotificationDelivery, sendRecordingIsPosted2HandlerLogger));

            return config;
        }

        private static void InitializeConfig()
        {
            NameValueCollection applicationSettingsSection = ConfigurationManager.AppSettings;
            ConnectionStringSettingsCollection connectionStringSettingsCollection = ConfigurationManager.ConnectionStrings;
            CitrixAuthMark = ConfigurationManager.AppSettings["CitrixAuthMark"];
            CitrixOrgKeyMark = ConfigurationManager.AppSettings["CitrixOrgKeyMark"];

            DefaultConnectionString = connectionStringSettingsCollection["DefaultConnection"].ConnectionString;
            //LegacyConnectionString = connectionStringSettingsCollection["LegacyConnection"].ConnectionString;
            //var StorageAccessKey = applicationSettingsSection.GetKey();
            foreach (var key in applicationSettingsSection.AllKeys)
            {

                Console.WriteLine("Key: {0} Value: {1}", key, applicationSettingsSection[key]);
            }
        }

        public static DateTime UtcNowAsCts
        {
            get
            {
                DateTime timeUtc = DateTime.UtcNow;
                return TimeZoneInfo.ConvertTimeFromUtc(timeUtc, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"));
            }
        }

    }
}
