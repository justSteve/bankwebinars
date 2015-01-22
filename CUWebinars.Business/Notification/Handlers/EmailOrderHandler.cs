using CUWebinars.Business.Constants;
using CUWebinars.Business.Notification.Events;
using CUWebinars.Business.Notification.Formatters;
using CUWebinars.Business.Notification.ViewModel;
using CUWebinars.NotificationSystem.Event;
using Ninject.Extensions.Logging;
using System;

namespace CUWebinars.Business.Notification.Handlers
{
    public class EmailOrderHandler<T> : IEventHandler<EmailOrderEvent<T>>
        where T : OrderSubmittedViewModel
    {
        private readonly IFormatter _generalFormatter;
        private readonly ILogger _logger;
        private readonly INotificationDelivery _notificationDelivery;

        public EmailOrderHandler(IFormatter generalFormatter, ILogger logger, INotificationDelivery notificationDelivery)
        {
            _generalFormatter = generalFormatter;
            _logger = logger;
            _notificationDelivery = notificationDelivery;
        }

        public void Handle(EmailOrderEvent<T> emailOrderEvent)
        {
            try
            {
                var notificationMessage = _generalFormatter.Format(emailOrderEvent.EventObject, "OrderSubmitted");
                notificationMessage.PersistedName = string.Format("OrderSubmitted-{0}{1}", DateTime.Now.ToString(DomainConstants.DateTimeLongFormat), ".htm");

                notificationMessage.To = emailOrderEvent.EventObject.Order.BillingEmail;
                _notificationDelivery.Notify(notificationMessage);
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
                            emailOrderEvent.EventObject.Order.idOrder,
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

    public class EmailOrderHandler : EmailOrderHandler<OrderSubmittedViewModel>
    {
        public EmailOrderHandler(IFormatter generalFormatter, ILogger logger, INotificationDelivery notificationDelivery)
            : base(generalFormatter, logger, notificationDelivery)
        {

        }
    }
}
