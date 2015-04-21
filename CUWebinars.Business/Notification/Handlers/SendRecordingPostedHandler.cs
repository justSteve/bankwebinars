using System;
using System.Collections.Generic;
using System.Linq;
using CUWebinars.Business.Constants;
using CUWebinars.Business.Core.Helpers;
using CUWebinars.Business.Models;
using CUWebinars.Business.Notification.Email;
using CUWebinars.Business.Notification.Events;
using CUWebinars.Business.Notification.Formatters;
using CUWebinars.Business.Notification.ViewModel;
using CUWebinars.NotificationSystem.Event;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Ninject.Extensions.Logging;
using Ninject.Extensions.Logging.Log4net.Infrastructure;

namespace CUWebinars.Business.Notification.Handlers
{
    public class SendRecordingPostedHandler<T> : IEventHandler<SendRecordingPostedEvent<T>>
        where T : PostEventPublishModel
    {
        private readonly IFormatter _generalFormatter;
        private readonly INotificationDelivery _notificationDelivery;
        private readonly ILogger _logger;

        public SendRecordingPostedHandler(IFormatter generalFormatter, ILogger logger)
            : this(generalFormatter, new SmtpMessageDelivery(new Log4NetLogger(typeof(SmtpMessageDelivery))), logger)
        {

        }

        public SendRecordingPostedHandler(IFormatter generalFormatter, INotificationDelivery notificationDelivery, ILogger logger)
        {
            _generalFormatter = generalFormatter;
            _notificationDelivery = notificationDelivery;
            _logger = logger;
        }



        public virtual void Process(SendRecordingPostedEvent<T> sendRecordingPostedEvent)
        {
            int orderId = sendRecordingPostedEvent.EventObject.Order.idOrder;

            try
            {
                var persistedNamePrefix = sendRecordingPostedEvent.ResendEvent
                                        ? "RecordingPosted_ReSend_" + orderId
                                        : "RecordingPosted_" + orderId;

                var notificationMessage = _generalFormatter.Format(sendRecordingPostedEvent.EventObject, "SendRecordingPosted");

                notificationMessage.PersistedName = string.Format("{0}_{1}{2}"
                    , persistedNamePrefix
                    , DateTime.Now.ToString(DomainConstants.DateTimeLongFormat)
                    , ".htm");

                var isAdditionalLocation = sendRecordingPostedEvent.EventObject.Order.OrderRows
                    .Single(or => or.RowStatus == OrderRowStatus.Active).AdditionalLocation;

                //  adds the name of the message to the Json object stored in NotificationStorage.
                JObject notificationStorage;

                if (string.IsNullOrWhiteSpace(sendRecordingPostedEvent.Details))
                {
                    notificationStorage = new JObject();
                }
                else
                {
                    notificationStorage = JObject.Parse(sendRecordingPostedEvent.Details);
                }

                JProperty SendShippedOrderMsgProperty = new JProperty(
                    string.Concat("SendShippedOrderMsg-", DateTime.Now.Ticks),
                    notificationMessage.PersistedName
                    );

                notificationStorage.Add(SendShippedOrderMsgProperty);

                if (isAdditionalLocation.Any())
                {
                    string tmpNameStorage = sendRecordingPostedEvent.EventObject.Order.FirstName;

                    //send a notification to each of any additional locations records
                    int count = 0;
                    foreach (var additionalLocation in isAdditionalLocation)
                    {
                        //override the order's Name property so that additional locations addressees 
                        // get correct name. order isn't saved so after execution, the property reverts.
                        sendRecordingPostedEvent.EventObject.Order.FirstName = additionalLocation.FullName;

                        INotificationMessage additionalLocationNotificationMessage =
                            _generalFormatter.Format(
                            sendRecordingPostedEvent.EventObject,
                            "SendRecordingPosted"
                            );

                        _logger.Info("Sending SendRecordingPosted Info: " + additionalLocation.Email);

                        persistedNamePrefix = sendRecordingPostedEvent.ResendEvent
                                                ? "AddLoc_RecordingPosted_ReSend_" + ++count + "_" + orderId
                                                : "AddLoc_RecordingPosted_" + ++count + "_" + orderId;

                        additionalLocationNotificationMessage.PersistedName = string.Format("{0}_{1}{2}",
                            persistedNamePrefix,
                            DateTime.Now.ToString(DomainConstants.DateTimeLongFormat), ".htm"
                            );

                        JProperty sendRecordingPostedInfoAddLocMsg = new JProperty(
                                string.Concat(string.Format("AddLocSendRecordingPostedInfoMsg-{0}-", count), DateTime.Now.Ticks),
                                notificationMessage.PersistedName
                                );

                        notificationStorage.Add(sendRecordingPostedInfoAddLocMsg);

                        additionalLocationNotificationMessage.To = additionalLocation.Email;

                        _notificationDelivery.Notify(additionalLocationNotificationMessage);                        
                    }
                    sendRecordingPostedEvent.EventObject.Order.FirstName = tmpNameStorage; // assign name back.
                    
                }

                sendRecordingPostedEvent.EventObject.Order.NotificationStorage = notificationStorage.ToString(Formatting.None);


                IList<string> ccEmailAddresses = null;


                if (!string.IsNullOrWhiteSpace(sendRecordingPostedEvent.EventObject.Order.UserComments))
                {
                    var addresses = JObject.Parse(sendRecordingPostedEvent.EventObject.Order.UserComments).GetValue(JsonPropertyKeys.CarbonCopy).ToString();

                    if (!ReferenceEquals(null, addresses))
                    {
                        ccEmailAddresses = EventHandlerHelpers.GetCcEmailAddresses(addresses);
                    }
                }
                
                notificationMessage.To = sendRecordingPostedEvent.EventObject.Order.BillingEmail;
                notificationMessage.Addresses = ccEmailAddresses;

                _notificationDelivery.Notify(notificationMessage);
            }
            catch (NullReferenceException nullReferenceException)
            {
                if (ReferenceEquals(null, sendRecordingPostedEvent.EventObject))
                {
                    _logger.Error(string.Format("sendRecordingPostedEvent ExceptionMessage: {0}", nullReferenceException.Message), nullReferenceException);
                }
                else
                {
                    _logger.FatalException(
                        string.Format("failed email: sendRecordingPostedEvent - OrderId {0}. ExceptionMessage: {1}",
                            orderId,
                            nullReferenceException.Message)
                        , nullReferenceException);
                }

            }
            catch (Exception exception)
            {
                _logger.FatalException(string.Format("failed outer sendRecordingPostedEvent - OrderId {0}. ExceptionMessage: {1}"
                    , orderId,
                    exception.Message)
                    , exception);
            }

        }

        public void Handle(SendRecordingPostedEvent<T> @event)
        {
            Process(@event);
        }
    }

    public class SendRecordingPostedHandler : SendRecordingPostedHandler<PostEventPublishModel>
    {
        public SendRecordingPostedHandler(IFormatter generalFormatter, ILogger logger)
            : base(generalFormatter, logger)
        {

        }

        public SendRecordingPostedHandler(IFormatter generalFormatter, INotificationDelivery notificationDelivery, ILogger logger)
            : base(generalFormatter, notificationDelivery, logger)
        {
        }

    }
}
