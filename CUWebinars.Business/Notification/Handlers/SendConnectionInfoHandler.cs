using System.Collections.Generic;
using CUWebinars.Business.Constants;
using CUWebinars.Business.Core.Helpers;
using CUWebinars.Business.Models;
using CUWebinars.Business.Notification.Email;
using CUWebinars.Business.Notification.Events;
using CUWebinars.Business.Notification.Formatters;
using CUWebinars.NotificationSystem.Event;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Ninject.Extensions.Logging;
using Ninject.Extensions.Logging.Log4net.Infrastructure;
using System;
using System.Linq;

namespace CUWebinars.Business.Notification.Handlers
{
    public class SendConnectionInfoHandler<T> : IEventHandler<SendConnectionInfoEvent<T>>
        where T : Order
    {
        private readonly IFormatter _generalFormatter;
        private readonly INotificationDelivery _notificationDelivery;
        private readonly ILogger _logger;

        public SendConnectionInfoHandler(IFormatter generalFormatter,
            ILogger logger,
            EnvironmentInformation environmentInformation)
            : this(generalFormatter, new SmtpMessageDelivery(new Log4NetLogger(typeof(SmtpMessageDelivery))), logger, environmentInformation)
        {

        }
        public SendConnectionInfoHandler(IFormatter generalFormatter,
            INotificationDelivery notificationDelivery,
            ILogger logger,
            EnvironmentInformation environmentInformation)
        {
            _generalFormatter = generalFormatter;
            _notificationDelivery = notificationDelivery;
            _logger = logger;

        }

        public virtual void Process(SendConnectionInfoEvent<T> sendConnectionInfoEvent)
        {
            int orderId = sendConnectionInfoEvent.EventObject.idOrder;

            _logger.Info("SendConnectionInfoEventmailer is processing " + sendConnectionInfoEvent.EventObject.OrderRows
                .Single(or => or.RowStatus == OrderRowStatus.Active).idOrder);

            INotificationMessage notificationMessage = _generalFormatter.Format(sendConnectionInfoEvent.EventObject, "SendConnectionInfo");

            var isAdditionalLocation =
                sendConnectionInfoEvent.EventObject.OrderRows
                    .Single(or => or.RowStatus == OrderRowStatus.Active)
                    .AdditionalLocation;

            //  adds the name of the message to the Json object stored in NotificationStorage.
            try
            {

                var persistedNamePrefix = sendConnectionInfoEvent.ResendEvent
                    ? "ConnectionInfo_ReSend_" + orderId
                    : "ConnectionInfo_" + orderId;

                notificationMessage.PersistedName = string.Format("{0}_{1}{2}",
                    persistedNamePrefix,
                    DomainConstants.BuildUtcNowAsCts.ToString(DomainConstants.DateTimeLongFormat), ".htm"
                    );

                JObject notificationStorage;

                if (string.IsNullOrWhiteSpace(sendConnectionInfoEvent.Details))
                {
                    notificationStorage = new JObject();
                }
                else
                {
                    notificationStorage = JObject.Parse(sendConnectionInfoEvent.Details);
                }

                JProperty sendConnectionInfoMsg = new JProperty(
                    string.Concat("SendConnectionInfoMsg-", DomainConstants.BuildUtcNowAsCts.Ticks),
                    notificationMessage.PersistedName
                    );
                notificationStorage.Add(sendConnectionInfoMsg);

                sendConnectionInfoEvent.EventObject.NotificationStorage =
                    notificationStorage.ToString(Formatting.None);

                if (isAdditionalLocation.Any())
                {
                    string tmpNameStorage = sendConnectionInfoEvent.EventObject.FirstName;

                    //send a notification to each of any additional locations records
                    int count = 0;
                    foreach (var additionalLocation in isAdditionalLocation)
                    {
                        //override the order's Name property so that additional locations addressees 
                        // get correct name. order isn't saved so after execution, the property reverts.
                        sendConnectionInfoEvent.EventObject.FirstName = additionalLocation.FullName;

                        INotificationMessage additionalLocationNotificationMessage =
                            _generalFormatter.Format(
                            sendConnectionInfoEvent.EventObject,
                            "SendConnectionInfo"
                            );

                        _logger.Info("Sending Connection Info: " + additionalLocation.Email);

                        persistedNamePrefix = sendConnectionInfoEvent.ResendEvent
                                                ? "AddLoc_ConnectionInfo_ReSend_" + ++count + "_" + orderId
                                                : "AddLoc_ConnectionInfo_" + ++count + "_" + orderId;

                        additionalLocationNotificationMessage.PersistedName = string.Format("{0}_{1}{2}",
                            persistedNamePrefix,
                            DomainConstants.BuildUtcNowAsCts.ToString(DomainConstants.DateTimeLongFormat), ".htm"
                            );

                        JProperty sendConnectionInfoAddLocMsg = new JProperty(
                                string.Concat(string.Format("AddLocSendConnectionInfoMsg-{0}-", count), DomainConstants.BuildUtcNowAsCts.Ticks),
                                notificationMessage.PersistedName
                                );

                        notificationStorage.Add(sendConnectionInfoAddLocMsg);

                        additionalLocationNotificationMessage.To = additionalLocation.Email;

                        _notificationDelivery.Notify(additionalLocationNotificationMessage);
                    }

                    sendConnectionInfoEvent.EventObject.FirstName = tmpNameStorage; // assign name back.
                }

                sendConnectionInfoEvent.EventObject.NotificationStorage = notificationStorage.ToString(Formatting.None);

                IList<string> ccEmailAddresses = null;
                //see Account/ShareNotifications
                if (!string.IsNullOrWhiteSpace(sendConnectionInfoEvent.EventObject.UserComments))
                {
                    JToken addresses;

                    addresses = JObject.Parse(sendConnectionInfoEvent.EventObject.UserComments).GetValue(JsonPropertyKeys.CarbonCopy);

                    if (!ReferenceEquals(null, addresses))
                    {
                        ccEmailAddresses = EventHandlerHelpers.GetCcEmailAddresses(addresses.ToString());
                    }
                }

                notificationMessage.To = sendConnectionInfoEvent.EventObject.BillingEmail;
                notificationMessage.Addresses = ccEmailAddresses;
                _notificationDelivery.Notify(notificationMessage);
            }
            catch (NullReferenceException nullReferenceException)
            {
                if (ReferenceEquals(null, sendConnectionInfoEvent.EventObject))
                {
                    _logger.Error(
                        string.Format("sendConnectionInfoEvent ExceptionMessage: {0}", nullReferenceException.Message),
                        nullReferenceException);
                }
                else
                {
                    _logger.Error(
                        string.Format(
                            "Event processing failed for sendConnectionInfoEvent - OrderId {0}. ExceptionMessage: {1}",
                            orderId,
                            nullReferenceException.Message)
                        , nullReferenceException);
                }
            }
            catch (Exception exception)
            {
                _logger.Error(
                    string.Format(
                        "Event processing (outer) failed for sendConnectionInfoEvent - OrderId {0}. ExceptionMessage: {1}"
                        , orderId,
                        exception.Message), exception);
            }
        }

        public void Handle(SendConnectionInfoEvent<T> @event)
        {
            Process(@event);
        }
    }

    public class SendConnectionInfoHandler : SendConnectionInfoHandler<Order>
    {
        public SendConnectionInfoHandler(IFormatter generalFormatter, ILogger logger, EnvironmentInformation environmentInformation)
            : base(generalFormatter, logger, environmentInformation)
        {

        }

        public SendConnectionInfoHandler(IFormatter generalFormatter, INotificationDelivery notificationDelivery, ILogger logger, EnvironmentInformation environmentInformation)
            : base(generalFormatter, notificationDelivery, logger, environmentInformation)
        {
        }

    }
}
