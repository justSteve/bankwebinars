using System;
using System.Linq;
using CUWebinars.Business.Models;
using CUWebinars.Business.Notification.Email;
using CUWebinars.Business.Notification.Events;
using CUWebinars.Business.Notification.Formatters;
using CUWebinars.NotificationSystem.Event;
using Ninject.Extensions.Logging;

namespace CUWebinars.Business.Notification.Handlers
{
    public class SendConnectionInfoHandler<T> : IEventHandler<SendConnectionInfoEvent<T>>
        where T : Order
    {
        private readonly IFormatter _generalFormatter;
        private readonly INotificationDelivery _notificationDelivery;
        private readonly ILogger _logger;

        public SendConnectionInfoHandler(IFormatter generalFormatter, ILogger logger)
            : this(generalFormatter, new SmtpMessageDelivery(), logger)
        {

        }
        public SendConnectionInfoHandler(IFormatter generalFormatter, INotificationDelivery notificationDelivery, ILogger logger)
        {
            _generalFormatter = generalFormatter;
            _notificationDelivery = notificationDelivery;
            _logger = logger;
        }

        public virtual void Process(SendConnectionInfoEvent<T> sendConnectionInfoEvent)
        {

            try
            {
                var notificationMessage = _generalFormatter.Format(sendConnectionInfoEvent.EventObject, "SendConnectionInfo");

                var isAdditionalLocation =
                    sendConnectionInfoEvent.EventObject.OrderRows.Single(or => or.RowStatus == OrderRowStatus.Active)
                        .AdditionalLocation;
                if (isAdditionalLocation.Count != 0)
                {
                    //send a notification to each of any additional locations records
                    notificationMessage.To = sendConnectionInfoEvent.EventObject.BillingEmail;
                    foreach (var additionalLocation in isAdditionalLocation)
                    {
                        //override the order's Name property so that additional locations addressees 
                        // get correct name. order isn't saved so after execution, the property reverts.
                        sendConnectionInfoEvent.EventObject.FirstName = additionalLocation.FullName;
                        notificationMessage = _generalFormatter.Format(sendConnectionInfoEvent.EventObject, "SendConnectionInfo");
                        notificationMessage.To = additionalLocation.Email;
                        _notificationDelivery.Notify(notificationMessage);
                    }
                }

                notificationMessage.To = sendConnectionInfoEvent.EventObject.BillingEmail;
                _notificationDelivery.Notify(notificationMessage);
            }
            catch (NullReferenceException nullReferenceException)
            {
                if (ReferenceEquals(null, sendConnectionInfoEvent.EventObject))
                {
                    _logger.Error(string.Format("sendConnectionInfoEvent ExceptionMessage: {0}", nullReferenceException.Message), nullReferenceException);
                }
                else
                {
                    _logger.Error(
                        string.Format("Event processing failed for sendConnectionInfoEvent - OrderId {0}. ExceptionMessage: {1}",
                            sendConnectionInfoEvent.EventObject.idOrder,
                            nullReferenceException.Message)
                        , nullReferenceException);
                }
            }
            catch (Exception exception)
            {
                _logger.Error(string.Format("Event processing (outer) failed for sendConnectionInfoEvent - OrderId {0}. ExceptionMessage: {1}"
                    , sendConnectionInfoEvent.EventObject.idOrder,
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
        public SendConnectionInfoHandler(IFormatter generalFormatter, ILogger logger)
            : base(generalFormatter, logger)
        {

        }

        public SendConnectionInfoHandler(IFormatter generalFormatter, INotificationDelivery notificationDelivery, ILogger logger)
            : base(generalFormatter, notificationDelivery, logger)
        {
        }

    }
}
