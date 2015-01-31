using CUWebinars.Business.Models;
using CUWebinars.Business.Notification.Events;
using CUWebinars.Business.Notification.Formatters;
using CUWebinars.NotificationSystem.Event;
using Ninject.Extensions.Logging;
using System;
using System.Linq;

namespace CUWebinars.Business.Notification.Handlers
{
    public class EmailSendShippedOrderHandler<T> : IEventHandler<EmailSendShippedOrderEvent<T>>
        where T : Order
    {
        private readonly IFormatter _generalFormatter;
        private readonly ILogger _logger;
        private readonly INotificationDelivery _notificationDelivery;

        public EmailSendShippedOrderHandler(IFormatter generalFormatter, ILogger logger, INotificationDelivery notificationDelivery)
        {
            _generalFormatter = generalFormatter;
            _logger = logger;
            _notificationDelivery = notificationDelivery;
        }

        public void Handle(EmailSendShippedOrderEvent<T> emailOrderEvent)
        {
            try
            {
                var notificationMessage = _generalFormatter.Format(emailOrderEvent.EventObject, "SendShippedOrder");

                notificationMessage.To = emailOrderEvent.Recipients.First();
                
                _notificationDelivery.Notify(notificationMessage);

                if (emailOrderEvent.Recipients.Count() > 1)
                {
                    foreach (var recipient in emailOrderEvent.Recipients.Skip(1))
                    {
                        notificationMessage.To = recipient;
                        _notificationDelivery.Notify(notificationMessage);
                    }
                }
            }
            catch (NullReferenceException nullReferenceException)
            {
                if (ReferenceEquals(null, emailOrderEvent.EventObject))
                {
                    _logger.ErrorException(string.Format("ExceptionMessage emailOrderEvent: {0}", nullReferenceException.Message), nullReferenceException);
                }
                else
                {
                    _logger.ErrorException(
                        string.Format("Event processing failed for emailOrderEvent - OrderId {0}. ExceptionMessage: {1}",
                            emailOrderEvent.EventObject.idOrder,
                            nullReferenceException.Message)
                        , nullReferenceException);
                }
            }
            catch (Exception exception)
            {
                _logger.ErrorException(string.Format("emailOrderEvent (outer) ExceptionMessage: {0}", exception.Message), exception);
            }
        }
    }

    public class EmailSendShippedOrderHandler : EmailSendShippedOrderHandler<Order>
    {
        public EmailSendShippedOrderHandler(IFormatter generalFormatter, ILogger logger, INotificationDelivery notificationDelivery)
            : base(generalFormatter, logger, notificationDelivery)
        {

        }
    }
}
