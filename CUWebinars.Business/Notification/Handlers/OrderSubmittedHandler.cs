using CUWebinars.Business.Constants;
using CUWebinars.Business.Notification.Email;
using CUWebinars.Business.Notification.Events;
using CUWebinars.NotificationSystem.Event;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Ninject.Extensions.Logging;
using System;

namespace CUWebinars.Business.Notification.Handlers
{
    public class OrderSubmittedHandler<T> : IEventHandler<OrderSubmittedEvent<T>>
        where T : ConfirmOrderMessage
    {
        private readonly IOrderConfirmedNotificationDelivery _notificationDelivery;
        private readonly ILogger _logger;

        public OrderSubmittedHandler(IOrderConfirmedNotificationDelivery notificationDelivery
            , ILogger logger)
        {
            _notificationDelivery = notificationDelivery;
            _logger = logger;

        }

        public virtual void Process(OrderSubmittedEvent<T> orderSubmittedEvent)
        {
            _logger.Info("Begins OrderSubmitted Notification for {0}", orderSubmittedEvent.EventObject.idOrder);

            try
            {
                if (orderSubmittedEvent.EventObject.UserCreatedInCart)
                {
                    orderSubmittedEvent.EventObject.AddPasswordUrl = orderSubmittedEvent.RelativePath;
                }

                int orderId = orderSubmittedEvent.EventObject.idOrder;

                var persistedNamePrefix = orderSubmittedEvent.ResendEvent
                    ? "OrderSubmitted-ReSend_" + orderId
                    : "OrderSubmittedSend_" + orderId;

                orderSubmittedEvent.EventObject.PersistedName = string.Format("{0}_{1}{2}",
                    persistedNamePrefix,
                    DateTime.Now.ToString(DomainConstants.DateTimeLongFormat),
                    ".htm"
                    );

                JObject notificationStorage;

                if (string.IsNullOrWhiteSpace(orderSubmittedEvent.Details))
                {
                    notificationStorage = new JObject();
                }
                else
                {
                    notificationStorage = JObject.Parse(orderSubmittedEvent.Details);
                }

                JProperty orderSubmittedEventMsg = new JProperty(
                    string.Concat("OrderSubmittedEventMsg-", DateTime.Now.Ticks),
                    orderSubmittedEvent.EventObject.PersistedName
                    );

                notificationStorage.Add(orderSubmittedEventMsg);
                
                orderSubmittedEvent.EventObject.Order.NotificationStorage =
                    orderSubmittedEvent.EventObject.Details = notificationStorage.ToString(Formatting.None);

                _logger.Info("PersistedName for Order {0} is {1}", orderId, orderSubmittedEvent.EventObject.PersistedName);

                _notificationDelivery.Notify(orderSubmittedEvent.EventObject);

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
                            orderSubmittedEvent.EventObject.idOrder,
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

    public class OrderSubmittedHandler : OrderSubmittedHandler<ConfirmOrderMessage>
    {
        public OrderSubmittedHandler(IOrderConfirmedNotificationDelivery notificationDelivery, ILogger logger)
            : base(notificationDelivery, logger)
        {
        }

    }
}