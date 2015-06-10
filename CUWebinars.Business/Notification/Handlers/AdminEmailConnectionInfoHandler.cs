using System.Linq;
using CUWebinars.Business.Constants;
using CUWebinars.Business.Models;
using CUWebinars.Business.Notification.Events;
using CUWebinars.Business.Notification.Formatters;
using CUWebinars.NotificationSystem.Event;
using Ninject.Extensions.Logging;
using System;

namespace CUWebinars.Business.Notification.Handlers
{
    public class AdminEmailConnectionInfoHandler<T> : IEventHandler<AdminEmailConnectionInfoEvent<T>>
        where T : Order
    {
        private const string NotificationName = "SendConnectionInfo";
        private readonly IFormatter _generalFormatter;
        private readonly ILogger _logger;
        private readonly INotificationDelivery _notificationDelivery;

        public AdminEmailConnectionInfoHandler(IFormatter generalFormatter, ILogger logger, INotificationDelivery notificationDelivery)
        {
            _generalFormatter = generalFormatter;
            _logger = logger;
            _notificationDelivery = notificationDelivery;
        }


        public void Handle(AdminEmailConnectionInfoEvent<T> adminEmailConnectionInfoEvent)
        {
            try
            {
                var notificationMessage = _generalFormatter.Format(adminEmailConnectionInfoEvent.EventObject, NotificationName);

                notificationMessage.To = adminEmailConnectionInfoEvent.Recipients.First();

                notificationMessage.PersistedName = string.Format("{0}_{1}{2}", 
                    string.Concat(DomainConstants.AdminEmailedPrefix, NotificationName, "_", adminEmailConnectionInfoEvent.EventObject.idOrder),
                    DomainConstants.BuildUtcNowAsCts.ToString(DomainConstants.DateTimeLongFormat),
                    ".htm"
                    );

                _notificationDelivery.Notify(notificationMessage);

                if (!ReferenceEquals(null, adminEmailConnectionInfoEvent.Recipients) && adminEmailConnectionInfoEvent.Recipients.Count() > 1)
                {
                    foreach (var recipient in adminEmailConnectionInfoEvent.Recipients.Skip(1))
                    {
                        notificationMessage.To = recipient;
                        _notificationDelivery.Notify(notificationMessage);
                    }
                }
            }
            catch (NullReferenceException nullReferenceException)
            {
                if (ReferenceEquals(null, adminEmailConnectionInfoEvent.EventObject))
                {
                    _logger.ErrorException(string.Format("ExceptionMessage adminEmailConnectionInfoEvent: {0}", nullReferenceException.Message), nullReferenceException);
                }
                else
                {
                    _logger.ErrorException(
                        string.Format("Event processing failed for adminEmailConnectionInfoEvent - OrderId {0}. ExceptionMessage: {1}",
                            adminEmailConnectionInfoEvent.EventObject.idOrder,
                            nullReferenceException.Message)
                        , nullReferenceException);
                }
            }
            catch (Exception exception)
            {
                _logger.ErrorException(string.Format("adminEmailConnectionInfoEvent (outer) ExceptionMessage: {0}", exception.Message), exception);
            }

        }
    }

    public class AdminEmailConnectionInfoHandler : AdminEmailConnectionInfoHandler<Order>
    {
        public AdminEmailConnectionInfoHandler(IFormatter generalFormatter, ILogger logger, INotificationDelivery notificationDelivery)
            : base(generalFormatter, logger, notificationDelivery)
        {

        }
    }
}