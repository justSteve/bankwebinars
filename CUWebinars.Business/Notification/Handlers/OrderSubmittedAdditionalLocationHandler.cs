using CUWebinars.Business.Constants;
using CUWebinars.Business.Notification.Email;
using CUWebinars.Business.Notification.Events;
using CUWebinars.Business.Notification.Formatters;
using CUWebinars.NotificationSystem.Event;
using Ninject.Extensions.Logging;
using System;

namespace CUWebinars.Business.Notification.Handlers
{
    public class OrderSubmittedAdditionalLocationHandler<T> : IEventHandler<OrderSubmittedAdditionalLocationEvent<T>>
        where T : AdditionalLocationOrderDetailsMessage
    {
        private readonly IFormatter _generalFormatter;
        private readonly IAzureOrderConfirmedForAdditionalLocationNotifier _notificationDelivery;
        private readonly ILogger _logger;

        public OrderSubmittedAdditionalLocationHandler(IFormatter generalFormatter, IAzureOrderConfirmedForAdditionalLocationNotifier notificationDelivery, ILogger logger)
        {
            _generalFormatter = generalFormatter;
            _notificationDelivery = notificationDelivery;
            _logger = logger;
        }

        public virtual void Process(OrderSubmittedAdditionalLocationEvent<T> orderSubmittedAdditionalLocationEvent)
        {
            try
            {
                int idOrder = orderSubmittedAdditionalLocationEvent.EventObject.idOrder;

                var persistedNamePrefix = orderSubmittedAdditionalLocationEvent.ResendEvent
                    ? "OrderSubmittedAdditionalLocation-ReSend_" + idOrder
                    : "OrderSubmittedAdditionalLocationSend_" + idOrder;

                orderSubmittedAdditionalLocationEvent.EventObject.PersistedName = string.Format("{0}_{1}{2}",
                    persistedNamePrefix,
                    DateTime.Now.ToString(DomainConstants.DateTimeLongFormat),
                    ".htm"
                    );

                //  adds the name of the message to the Json object stored in NotificationStorage.
                //string details = orderSubmittedAdditionalLocationEvent.Details;
                //if (details != null)
                //{

                //    orderSubmittedAdditionalLocationEvent.EventObject.Order.NotificationStorage =
                //        details.Insert(details.Length - 1,
                //            string.Concat(",", @"""OrderSubmittedAdditionalLocationEventMsg-", DateTime.Now.Ticks, '"',
                //                @":", '"', notificationMessage.PersistedName, '"'));
                //}
                //if (orderSubmittedAdditionalLocationEvent.EventObject.Order.idAffiliate == 62)
                //{
                //    notificationMessage.To = "steve@ttstrain.com";
                //}
                //else
                //{
                //    notificationMessage.To = orderSubmittedAdditionalLocationEvent.EventObject.Order.BillingEmail;
                //}

                _logger.Info("PersistedName for Additional Location {0} is {1}", idOrder, orderSubmittedAdditionalLocationEvent.EventObject.PersistedName);
                _notificationDelivery.Notify(orderSubmittedAdditionalLocationEvent.EventObject);
            }
            catch (NullReferenceException nullReferenceException)
            {
                if (ReferenceEquals(null, orderSubmittedAdditionalLocationEvent.EventObject))
                {
                    _logger.Error(string.Format("ExceptionMessage orderSubmittedAdditionalLocationEvent: {0}", nullReferenceException.Message), nullReferenceException);
                }
                else
                {
                    _logger.Error(
                        string.Format("Event processing failed for orderSubmittedAdditionalLocationEvent - OrderId {0}. ExceptionMessage: {1}",
                            orderSubmittedAdditionalLocationEvent.EventObject.idOrder,
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


    public class OrderSubmittedAdditionalLocationHandler : OrderSubmittedAdditionalLocationHandler<AdditionalLocationOrderDetailsMessage>
    {
 
        public OrderSubmittedAdditionalLocationHandler(IFormatter generalFormatter, IAzureOrderConfirmedForAdditionalLocationNotifier notificationDelivery, ILogger logger)
            : base(generalFormatter, notificationDelivery, logger)
        {
        }
    }
}