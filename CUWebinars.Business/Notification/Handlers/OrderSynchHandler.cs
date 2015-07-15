using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CUWebinars.Business.Constants;
using CUWebinars.Business.Notification.Email;
using CUWebinars.Business.Notification.Events;
using CUWebinars.NotificationSystem.Event;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Ninject.Extensions.Logging;

namespace CUWebinars.Business.Notification.Handlers
{
    public class OrderSynchHandler<T> : IEventHandler<OrderSynchEvent<T>>
        where T : OrderSynchMessage
    {
        //private readonly IOrderConfirmedNotificationDelivery _notificationDelivery;
        private readonly ILogger _logger;

        public OrderSynchHandler(ILogger logger)
        {
            _logger = logger;

        }
        //public OrderSynchHandler(IOrderConfirmedNotificationDelivery notificationDelivery
        //    , ILogger logger)
        //{
        //    _notificationDelivery = notificationDelivery;
        //    _logger = logger;

        //}

        public virtual void Process(OrderSynchEvent<T> orderSynchEvent)
        {
            _logger.Info("Begins OrderSynch Notification for {0}", orderSynchEvent.EventObject.idOrder);

            try
            {
                //if (orderSynchEvent.EventObject.UserCreatedInCart)
                //{
                //    orderSynchEvent.EventObject.AddPasswordUrl = orderSynchEvent.RelativePath;
                //}

                int orderId = orderSynchEvent.EventObject.idOrder;

                //var persistedNamePrefix = orderSynchEvent.ResendEvent
                //    ? "OrderSynch-ReSend_" + orderId
                //    : "OrderSynchSend_" + orderId;

                //orderSynchEvent.EventObject.PersistedName = string.Format("{0}_{1}{2}",
                //    persistedNamePrefix,
                //    DomainConstants.BuildUtcNowAsCts.ToString(DomainConstants.DateTimeLongFormat),
                //    ".htm"
                //    );

                //JObject notificationStorage;

                //if (string.IsNullOrWhiteSpace(orderSynchEvent.Details))
                //{
                //    notificationStorage = new JObject();
                //}
                //else
                //{
                //    notificationStorage = JObject.Parse(orderSynchEvent.Details);
                //}

                //JProperty orderSynchEventMsg = new JProperty(
                //    string.Concat("OrderSynchEventMsg-", DomainConstants.BuildUtcNowAsCts.Ticks),
                //    orderSynchEvent.EventObject.PersistedName
                //    );

                //notificationStorage.Add(orderSynchEventMsg);

                //orderSynchEvent.EventObject.Order.NotificationStorage =
                //    orderSynchEvent.EventObject.Details = notificationStorage.ToString(Formatting.None);

                //_logger.Info("PersistedName for Order {0} is {1}", orderId, orderSynchEvent.EventObject.PersistedName);

                //_notificationDelivery.Notify(orderSynchEvent.EventObject);

            }
            catch (NullReferenceException nullReferenceException)
            {
                if (ReferenceEquals(null, orderSynchEvent.EventObject))
                {
                    _logger.Error(string.Format("ExceptionMessage orderSynchEvent: {0}", nullReferenceException.Message), nullReferenceException);
                }
                else
                {
                    _logger.Error(
                        string.Format("Event processing failed for orderSynchEvent - OrderId {0}. ExceptionMessage: {1}",
                            orderSynchEvent.EventObject.idOrder,
                            nullReferenceException.Message)
                        , nullReferenceException);
                }
            }
            catch (Exception exception)
            {
                _logger.Error(string.Format("orderSynchEvent (outer) ExceptionMessage: {0}", exception.Message), exception);
            }
            _logger.Info("Ends OrderSynch Notification: {0}", orderSynchEvent.EventObject.idOrder);
        }

        public void Handle(OrderSynchEvent<T> @event)
        {
            Process(@event);
        }
    }

    public class OrderSynchHandler : OrderSynchHandler<OrderSynchMessage>
    {
        public OrderSynchHandler(ILogger logger)
            : base(logger)
        {
        }

    }

}
