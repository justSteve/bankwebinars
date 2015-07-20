using CUWebinars.Business.Constants;
using CUWebinars.Business.Notification.Email;
using CUWebinars.Business.Notification.Events;
using CUWebinars.NotificationSystem.Event;
using Ninject.Extensions.Logging;
using System;

namespace CUWebinars.Business.Notification.Handlers
{
    public class AdhocNotificationHandler<T> : IEventHandler<AdhocNotificationEvent<T>>
        where T : AdhocNotificationMessage
    {
        private readonly IAdhocNotificationDelivery _notificationDelivery;
        private readonly ILogger _logger;

        public AdhocNotificationHandler(IAdhocNotificationDelivery notificationDelivery
            , ILogger logger)
        {
            _notificationDelivery = notificationDelivery;
            _logger = logger;

        }

        public void Handle(AdhocNotificationEvent<T> adhocNotificationEvent)
        {
            try
            {
                adhocNotificationEvent.EventObject.PersistedName = string.Format("{0}_{1}{2}",
                    "AdhocNotification_",
                    DomainConstants.BuildUtcNowAsCts.ToString(DomainConstants.DateTimeLongFormat),
                    ".htm"
                    );

                _notificationDelivery.Notify(adhocNotificationEvent.EventObject);
            }
            catch (Exception exception)
            {
                _logger.Error(string.Format("adhocNotificationEvent ExceptionMessage: {0}", exception.Message), exception);
            }
        }
    }

    public class AdhocNotificationHandler : AdhocNotificationHandler<AdhocNotificationMessage>
    {
        public AdhocNotificationHandler(IAdhocNotificationDelivery adhocNotificationDelivery, ILogger logger)
            : base(adhocNotificationDelivery, logger)
        {
        }

    }
}
