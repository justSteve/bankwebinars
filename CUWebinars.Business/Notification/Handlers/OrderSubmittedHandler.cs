using System;
using System.Diagnostics;
using System.IO;
using CUWebinars.Business.Constants;
using CUWebinars.Business.Core.Tracing;
using CUWebinars.Business.Models;
using CUWebinars.Business.Notification.Email;
using CUWebinars.Business.Notification.Events;
using CUWebinars.Business.Notification.Formatters;
using CUWebinars.Business.Notification.ViewModel;
using CUWebinars.NotificationSystem.Event;
using Ninject.Extensions.Logging;

namespace CUWebinars.Business.Notification.Handlers
{
    public class OrderSubmittedHandler<T> : IEventHandler<OrderSubmittedEvent<T>>
        where T : OrderSubmittedViewModel
    {
        private readonly INotificationPersister _notificationPersister;
        private readonly EnvironmentInformation _environmentInformation;
        private readonly IFormatter _generalFormatter;
        private readonly INotificationDelivery _notificationDelivery;
        private readonly ILogger _logger;

        public OrderSubmittedHandler(IFormatter generalFormatter, ILogger logger, INotificationPersister notificationPersister, EnvironmentInformation environmentInformation)
            : this(generalFormatter, new SmtpMessageDelivery(), logger, notificationPersister, environmentInformation)
        {

        }

        public OrderSubmittedHandler(IFormatter generalFormatter, INotificationDelivery notificationDelivery, ILogger logger, INotificationPersister notificationPersister, EnvironmentInformation environmentInformation)
        {
            _notificationPersister = notificationPersister;
            _environmentInformation = environmentInformation;
            _generalFormatter = generalFormatter;
            _notificationDelivery = notificationDelivery;
            _logger = logger;
        }

        public virtual void Process(OrderSubmittedEvent<T> orderSubmittedEvent)
        {
            try
            {
                var notificationMessage = _generalFormatter.Format(orderSubmittedEvent.EventObject, "OrderSubmitted");
                notificationMessage.PersistedName = string.Format("OrderSubmitted-{0}{1}", DateTime.Now.ToString(DomainConstants.DateTimeLongFormat), ".htm");

                var fullFilePathToPersistedNotification = Path.Combine(_environmentInformation.BaseUrl,
                    orderSubmittedEvent.RelativeFilePath);
                //_notificationPersister.PersistNotification(notificationMessage.Body, fullFilePathToPersistedNotification);

                notificationMessage.To = orderSubmittedEvent.EventObject.Order.BillingEmail;
                _notificationDelivery.Notify(notificationMessage);

            }
            catch (NullReferenceException nullReferenceException)
            {
                if (ReferenceEquals(null, orderSubmittedEvent.EventObject))
                {
                    _logger.Error(string.Format("ExceptionMessage orderSubmittedEvent: {0}", nullReferenceException.Message), nullReferenceException);
                }
                else
                {
                    _logger.Error(
                        string.Format("Event processing failed for orderSubmittedEvent - OrderId {0}. ExceptionMessage: {1}",
                            orderSubmittedEvent.EventObject.Order.idOrder,
                            nullReferenceException.Message)
                        , nullReferenceException);
                }
            }
            catch (Exception exception)
            {
                _logger.Error(string.Format("orderSubmittedEvent (outer) ExceptionMessage: {0}", exception.Message), exception);
            }
        }

        public void Handle(OrderSubmittedEvent<T> @event)
        {
            Process(@event);
        }
    }

    public class OrderSubmittedHandler : OrderSubmittedHandler<OrderSubmittedViewModel>
    {
        public OrderSubmittedHandler(IFormatter generalFormatter, ILogger logger, INotificationPersister notificationPersister, EnvironmentInformation environmentInformation)
            : base(generalFormatter, logger, notificationPersister, environmentInformation)
        {

        }

        public OrderSubmittedHandler(IFormatter generalFormatter, INotificationDelivery notificationDelivery, ILogger logger, INotificationPersister notificationPersister, EnvironmentInformation environmentInformation)
            : base(generalFormatter, notificationDelivery, logger, notificationPersister, environmentInformation)
        {
        }

    }
}