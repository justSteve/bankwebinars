using System;
using CUWebinars.Business.Models;
using CUWebinars.Business.Notification.Email;
using CUWebinars.Business.Notification.Events;
using CUWebinars.Business.Notification.Formatters;
using CUWebinars.NotificationSystem.Event;
using Ninject.Extensions.Logging;

namespace CUWebinars.Business.Notification.Handlers
{
    public class SendShippedOrderHandler<T> : IEventHandler<SendShippedOrderEvent<T>> 
        where T : Order
    {
        private readonly IFormatter _generalFormatter;
        private readonly INotificationDelivery _notificationDelivery;
        private readonly ILogger _logger;

        public SendShippedOrderHandler(IFormatter generalFormatter, ILogger logger)
            : this(generalFormatter, new SmtpMessageDelivery(), logger)
        {

        }
        public SendShippedOrderHandler(IFormatter generalFormatter, INotificationDelivery notificationDelivery, ILogger logger)
        {
            _generalFormatter = generalFormatter;
            _notificationDelivery = notificationDelivery;
            _logger = logger;
        }

        public virtual void Process(SendShippedOrderEvent<T> sendShippedOrderEvent)
        {
            try
            {
                var notificationMessage = _generalFormatter.Format(sendShippedOrderEvent.EventObject, "SendShippedOrder");
                notificationMessage.To = sendShippedOrderEvent.EventObject.BillingEmail;

                _notificationDelivery.Notify(notificationMessage);
            }
            catch (NullReferenceException nullReferenceException)
            {
                _logger.Error(
                    string.Format("Event processing failed. Check that BillingEmail has a value for OrderId {0}",
                        sendShippedOrderEvent.EventObject.idOrder)
                    , nullReferenceException);
            }
            catch (Exception exception)
            {
                _logger.Error(string.Format("Event processing failed for OrderId {0}", sendShippedOrderEvent.EventObject.idOrder), exception);
            }
        }

        public void Handle(SendShippedOrderEvent<T> @event)
        {
            Process(@event);
        }
    }

    public class SendShippedOrderHandler : SendShippedOrderHandler<Order>
    {
        public SendShippedOrderHandler(IFormatter generalFormatter, ILogger logger)
            : base(generalFormatter, logger)
        {
        }

        public SendShippedOrderHandler(IFormatter generalFormatter, INotificationDelivery notificationDelivery, ILogger logger)
            : base(generalFormatter, notificationDelivery, logger)
        {
        }
    }

}
