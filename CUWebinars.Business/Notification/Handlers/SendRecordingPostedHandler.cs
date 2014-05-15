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


        public void Handle(SendRecordingPostedEvent<T> sendRecordingPostedEvent)
        {
            Process(sendRecordingPostedEvent);
        }

        public virtual void Process(SendRecordingPostedEvent<T> sendRecordingPostedEvent)
        {
            try
            {
                var notificationMessage = _generalFormatter.Format(sendRecordingPostedEvent.EventObject, "SendRecordingPosted");
                notificationMessage.To = sendRecordingPostedEvent.EventObject.BillingEmail;
                _notificationDelivery.Notify(notificationMessage);
            }
            catch (NullReferenceException nullReferenceException)
            {
                _logger.Error(
                    string.Format("Event processing failed. Check that BillingEmail has a value for OrderId {0}",
                        sendRecordingPostedEvent.EventObject.idOrder), nullReferenceException);
            }
            catch (Exception exception)
            {
                _logger.Error(string.Format("Event processing failed for OrderId {0}.", sendRecordingPostedEvent.EventObject.idOrder), exception);
            }
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
