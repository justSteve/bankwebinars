using System;
using CUWebinars.Business.Models;
using CUWebinars.Business.Notification.Email;
using CUWebinars.Business.Notification.Events;
using CUWebinars.Business.Notification.Formatters;
using CUWebinars.NotificationSystem.Event;
using Ninject.Extensions.Logging;

namespace CUWebinars.Business.Notification.Handlers
{
    public class SendRecordingPostedHandler<T> : IEventHandler<SendRecordingPostedEvent<T>>
        where T : Order
    {
        private readonly IFormatter _generalFormatter;
        private readonly INotificationDelivery _notificationDelivery;
        private readonly ILogger _logger;

        public SendRecordingPostedHandler(IFormatter generalFormatter, ILogger logger)
            : this(generalFormatter, new SmtpMessageDelivery(), logger)
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
            try
            {
                var notificationMessage = _generalFormatter.Format(sendRecordingPostedEvent.EventObject, "SendRecordingPosted");
                notificationMessage.ReplyTo = "registrations@bankwebinars.com";
                notificationMessage.To = sendRecordingPostedEvent.EventObject.BillingEmail;
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
                    _logger.Error(
                        string.Format("Event processing failed for sendRecordingPostedEvent - OrderId {0}. ExceptionMessage: {1}",
                            sendRecordingPostedEvent.EventObject.idOrder,
                            nullReferenceException.Message)
                        , nullReferenceException);
                }

            }
            catch (Exception exception)
            {
                _logger.Error(string.Format("Event processing (outer) failed for sendRecordingPostedEvent - OrderId {0}. ExceptionMessage: {1}"
                    , sendRecordingPostedEvent.EventObject.idOrder,
                    exception.Message), exception);
            }

        }

        public void Handle(SendRecordingPostedEvent<T> @event)
        {
            Process(@event);
        }
    }

    public class SendRecordingPostedHandler : SendRecordingPostedHandler<Order>
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
