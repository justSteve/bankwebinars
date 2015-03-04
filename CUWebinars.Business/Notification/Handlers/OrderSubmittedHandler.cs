using CUWebinars.Business.Constants;
using CUWebinars.Business.Notification.Email;
using CUWebinars.Business.Notification.Events;
using CUWebinars.Business.Notification.Formatters;
using CUWebinars.Business.Notification.ViewModel;
using CUWebinars.NotificationSystem.Event;
using Newtonsoft.Json;
using Ninject.Extensions.Logging;
using System;

namespace CUWebinars.Business.Notification.Handlers
{
    public class OrderSubmittedHandler<T> : IEventHandler<OrderSubmittedEvent<T>>
        where T : OrderSubmittedViewModel
    {
        private readonly IFormatter _generalFormatter;
        private readonly INotificationDelivery _notificationDelivery;
        private readonly ILogger _logger;

        public OrderSubmittedHandler(IFormatter generalFormatter
            , ILogger logger
            )
            : this(generalFormatter, new SmtpMessageDelivery()
                , logger)
        {

        }

        public OrderSubmittedHandler(IFormatter generalFormatter
            , INotificationDelivery notificationDelivery
            , ILogger logger)
        {
            _generalFormatter = generalFormatter;
            _notificationDelivery = notificationDelivery;
            _logger = logger;

        }

        public virtual void Process(OrderSubmittedEvent<T> orderSubmittedEvent)
        {
            _logger.Info("Begins OrderSubmitted Notification");
            try
            {
                if (orderSubmittedEvent.EventObject.UserCreatedInCart)
                {
                    orderSubmittedEvent.EventObject.AddPasswordUrl = orderSubmittedEvent.RelativePath;
                }

                var notificationMessage = _generalFormatter.Format(orderSubmittedEvent.EventObject, "OrderSubmitted");

                var persistedNamePrefix = orderSubmittedEvent.ResendEvent
                    ? "OrderSubmitted-ReSend_" + orderSubmittedEvent.EventObject.Order.idOrder
                    : "OrderSubmittedSend_" + orderSubmittedEvent.EventObject.Order.idOrder;

                notificationMessage.PersistedName = string.Format("{0}_{1}{2}",
                    persistedNamePrefix,
                    DateTime.Now.ToString(DomainConstants.DateTimeLongFormat),
                    ".htm"
                    );

                _logger.Info("PersistedName for Order {0} is {1}", orderSubmittedEvent.EventObject.Order.idOrder, notificationMessage.PersistedName);

                //  adds the name of the message to the Json object stored in NotificationStorage.
                string details = orderSubmittedEvent.Details;

                if (details != null)
                {
                    orderSubmittedEvent.EventObject.Order.NotificationStorage =
                        details.Insert(details.Length - 1,
                            string.Concat(",", @"""OrderSubmittedEventMsg-", DateTime.Now.Ticks, '"', @":", '"',
                                notificationMessage.PersistedName, '"'));
                }

                if (orderSubmittedEvent.EventObject.Order.idAffiliate == 62)
                {
                    notificationMessage.To = "steve@ttstrain.com";
                }
                else
                {
                    notificationMessage.To = orderSubmittedEvent.EventObject.Order.BillingEmail;
                }

                _logger.Info("Sending Notifn for Order {0}", orderSubmittedEvent.EventObject.Order.idOrder);
                
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
            _logger.Info("Ends OrderSubmitted Notification");
        }

        public void Handle(OrderSubmittedEvent<T> @event)
        {
            Process(@event);
        }
    }

    public class OrderSubmittedHandler : OrderSubmittedHandler<OrderSubmittedViewModel>
    {
        public OrderSubmittedHandler(IFormatter generalFormatter, ILogger logger)
            : base(generalFormatter, logger)
        {

        }

        public OrderSubmittedHandler(IFormatter generalFormatter, INotificationDelivery notificationDelivery, ILogger logger)
            : base(generalFormatter, notificationDelivery, logger)
        {
        }

    }
}