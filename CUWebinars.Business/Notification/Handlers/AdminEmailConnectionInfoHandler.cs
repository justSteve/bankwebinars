using System.Linq;
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
                var notificationMessage = _generalFormatter.Format(adminEmailConnectionInfoEvent.EventObject, "SendRecordingPosted");

                notificationMessage.To = adminEmailConnectionInfoEvent.Recipients.First();

                _notificationDelivery.Notify(notificationMessage);

                if (adminEmailConnectionInfoEvent.Recipients.Count() > 1)
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