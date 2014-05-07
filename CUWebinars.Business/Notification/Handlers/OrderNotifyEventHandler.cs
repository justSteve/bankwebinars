using System;
using CUWebinars.Business.Models;
using CUWebinars.Business.Notification.Email;
using CUWebinars.Business.Notification.Formatters;
using Ninject.Extensions.Logging;

namespace CUWebinars.Business.Notification.Handlers
{
    public class OrderNotifyEventHandler<TOrder> 
        where TOrder : Order
    {
        private readonly IOrderNotificationFormatter<TOrder> _orderNotificationFormatter;
        private readonly INotificationDelivery _notificationDelivery;
        private readonly INotificationPersister _notificationPersister;
        private readonly ILogger _logger;

        public OrderNotifyEventHandler(IOrderNotificationFormatter<TOrder> orderNotificationFormatter, ILogger logger)
            : this(orderNotificationFormatter, new SmtpMessageDelivery(), new FileBasedNotificationPersister(), logger)
        {
            
        }
        public OrderNotifyEventHandler(IOrderNotificationFormatter<TOrder> orderNotificationFormatter, 
            INotificationDelivery notificationDelivery, 
            INotificationPersister notificationPersister,
            ILogger logger)
        {
            _orderNotificationFormatter = orderNotificationFormatter;
            _notificationDelivery = notificationDelivery;
            _notificationPersister = notificationPersister;
            _logger = logger;
        }

        public virtual void Process<TBody>(Events.OrderSubmittedEvent<TOrder> evt, TBody objectOfMessage)
        {
            try
            {
                var notificationMessage = _orderNotificationFormatter.Format(evt, objectOfMessage,
                    _notificationPersister);
                notificationMessage.To = evt.Order.BillingEmail;
                _notificationDelivery.Notify(notificationMessage);
            }
            catch (NullReferenceException nullReferenceException)
            {
                _logger.Error(string.Format("Event processing failed. Check that BillingEmail has a value for OrderId {0}", evt.Order.idOrder)
                    , nullReferenceException);                
            }
            catch (Exception exception)
            {
                _logger.Error(string.Format("Event processing failed for OrderId {0}", evt.Order.idOrder), exception);                
            }
        }
    }
}
