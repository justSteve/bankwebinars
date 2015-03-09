using System;
using System.IO;
using CUWebinars.Business.Constants;
using CUWebinars.Business.Notification.Email;
using CUWebinars.Business.Notification.Events;
using CUWebinars.Business.Notification.Formatters;
using CUWebinars.Business.Notification.ViewModel;
using CUWebinars.NotificationSystem.Event;
using Ninject.Extensions.Logging;
using Ninject.Extensions.Logging.Log4net.Infrastructure;

namespace CUWebinars.Business.Notification.Handlers
{
    public class OrderSubmittedAdditionalLocationHandler<T> : IEventHandler<OrderSubmittedAdditionalLocationEvent<T>>
        where T : OrderSubmittedAdditionalLocationViewModel
    {
        private readonly INotificationPersister _notificationPersister;
        private readonly EnvironmentInformation _environmentInformation;
        private readonly IFormatter _generalFormatter;
        private readonly INotificationDelivery _notificationDelivery;
        private readonly ILogger _logger;

        public OrderSubmittedAdditionalLocationHandler(IFormatter generalFormatter, ILogger logger, INotificationPersister notificationPersister, EnvironmentInformation environmentInformation)
            : this(generalFormatter, new SmtpMessageDelivery(new Log4NetLogger(typeof(SmtpMessageDelivery))), logger, notificationPersister, environmentInformation)
        {

        }

        public OrderSubmittedAdditionalLocationHandler(IFormatter generalFormatter, INotificationDelivery notificationDelivery, ILogger logger, INotificationPersister notificationPersister, EnvironmentInformation environmentInformation)
        {
            _notificationPersister = notificationPersister;
            _environmentInformation = environmentInformation;
            _generalFormatter = generalFormatter;
            _notificationDelivery = notificationDelivery;
            _logger = logger;
        }

        public virtual void Process(OrderSubmittedAdditionalLocationEvent<T> orderSubmittedEvent)
        {
            try
            {
                var notificationMessage = _generalFormatter.Format(orderSubmittedEvent.EventObject, "OrderSubmittedAdditionalLocation");
                notificationMessage.PersistedName = string.Format("OrderSubmittedAdditionalLocation_{0}_{1}{2}"
                    , orderSubmittedEvent.EventObject.Order.idOrder
                    , DateTime.Now.ToString(DomainConstants.DateTimeLongFormat)
                    , ".htm");

                //  adds the name of the message to the Json object stored in NotificationStorage.
                string details = orderSubmittedEvent.Details;
                if (details != null)
                {

                    orderSubmittedEvent.EventObject.Order.NotificationStorage =
                        details.Insert(details.Length - 1,
                            string.Concat(",", @"""OrderSubmittedAdditionalLocationEventMsg-", DateTime.Now.Ticks, '"',
                                @":", '"', notificationMessage.PersistedName, '"'));
                }
                if (orderSubmittedEvent.EventObject.Order.idAffiliate == 62)
                {
                    notificationMessage.To = "steve@ttstrain.com";
                }
                else
                {
                    notificationMessage.To = orderSubmittedEvent.EventObject.Order.BillingEmail;
                }
                _notificationDelivery.Notify(notificationMessage);

            }
            catch (NullReferenceException nullReferenceException)
            {
                if (ReferenceEquals(null, orderSubmittedEvent.EventObject))
                {
                    _logger.Error(string.Format("ExceptionMessage orderSubmittedAdditionalLocationEvent: {0}", nullReferenceException.Message), nullReferenceException);
                }
                else
                {
                    _logger.Error(
                        string.Format("Event processing failed for orderSubmittedAdditionalLocationEvent - OrderId {0}. ExceptionMessage: {1}",
                            orderSubmittedEvent.EventObject.Order.idOrder,
                            nullReferenceException.Message)
                        , nullReferenceException);
                }
            }
            catch (Exception exception)
            {
                _logger.Error(string.Format("orderSubmittedAdditionalLocationEvent (outer) ExceptionMessage: {0}", exception.Message), exception);
            }
        }

        
        public void Handle(OrderSubmittedAdditionalLocationEvent<T> @event)
        {

            Process(@event);
        }
    }


    public class OrderSubmittedAdditionalLocationHandler : OrderSubmittedAdditionalLocationHandler<OrderSubmittedAdditionalLocationViewModel>
    {
        public OrderSubmittedAdditionalLocationHandler(IFormatter generalFormatter, ILogger logger, INotificationPersister notificationPersister, EnvironmentInformation environmentInformation)
            : base(generalFormatter, logger, notificationPersister, environmentInformation)
        {

        }

        public OrderSubmittedAdditionalLocationHandler(IFormatter generalFormatter, INotificationDelivery notificationDelivery, ILogger logger, INotificationPersister notificationPersister, EnvironmentInformation environmentInformation)
            : base(generalFormatter, notificationDelivery, logger, notificationPersister, environmentInformation)
        {
        }

    }
}