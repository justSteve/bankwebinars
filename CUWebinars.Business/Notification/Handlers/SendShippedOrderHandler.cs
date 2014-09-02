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
                notificationMessage.ReplyTo = "registrations@BankWebinars.com";
                notificationMessage.To = sendShippedOrderEvent.EventObject.BillingEmail;

                _notificationDelivery.Notify(notificationMessage);
            }
            catch (NullReferenceException nullReferenceException)
            {
                if (ReferenceEquals(null, sendShippedOrderEvent.EventObject))
                {
                    _logger.Error(string.Format("sendShippedOrderEvent ExceptionMessage: {0}", nullReferenceException.Message), nullReferenceException);
                }
                else
                {
                    _logger.Error(
                        string.Format("Event processing failed for sendShippedOrderEvent - OrderId {0}. ExceptionMessage: {1}",
                            sendShippedOrderEvent.EventObject.idOrder,
                            nullReferenceException.Message)
                        , nullReferenceException);
                }
            }
            catch (Exception exception)
            {
                _logger.Error(string.Format("Event processing failed (outer) for sendShippedOrderEvent - OrderId {0}. ExceptionMessage: {1}"
                    , sendShippedOrderEvent.EventObject.idOrder, exception.Message), exception);
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
