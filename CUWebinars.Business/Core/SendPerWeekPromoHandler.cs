using CUWebinars.Business.Constants;
using CUWebinars.Business.Notification.Email;
using CUWebinars.Business.Notification.Events;
using CUWebinars.Business.Notification.Formatters;
using CUWebinars.Business.Notification.ViewModel;
using CUWebinars.NotificationSystem.Event;
using Ninject.Extensions.Logging;
using System;
using Ninject.Extensions.Logging.Log4net.Infrastructure;

namespace CUWebinars.Business.Notification.Handlers
{
    public class SendPerWeekPromoHandler<T> : IEventHandler<SendPerWeekPromoEvent<T>>
        where T : WebinarPromoViewModel
    {
        private readonly IFormatter _generalFormatter;
        private readonly INotificationDelivery _notificationDelivery;
        private readonly ILogger _logger;

        public SendPerWeekPromoHandler(IFormatter generalFormatter
            , ILogger logger
            )
            : this(generalFormatter, new SmtpMessageDelivery(new Log4NetLogger(typeof(SmtpMessageDelivery))), logger)
        {

        }

        public SendPerWeekPromoHandler(IFormatter generalFormatter
            , INotificationDelivery notificationDelivery
            , ILogger logger)
        {
            _generalFormatter = generalFormatter;
            _notificationDelivery = notificationDelivery;
            _logger = logger;

        }

        public virtual void Process(SendPerWeekPromoEvent<T> sendPerWeekPromoEvent)
        {
            try
            {
                var persistedNamePrefix = "PerWeekPromo" + '_' + sendPerWeekPromoEvent.EventObject.Affiliate.ttsDomain + '_';
                
                var notificationMessage = _generalFormatter
                    .Format(sendPerWeekPromoEvent.EventObject, "SendPerWeekPromo");


                notificationMessage.PersistedName = string.Format("{0}-{1}{2}",
                    persistedNamePrefix,
                    DateTime.Now.ToString(DomainConstants.DateTimeLongFormat),
                    ".htm"
                    );

                //notificationMessage.To = sendPerWeekPromoEvent.EventObject.Affiliate.WebUser.email;
                notificationMessage.To = "steve@ttstrain.com";
                _notificationDelivery.Notify(notificationMessage);
            }
            catch (NullReferenceException nullReferenceException)
            {
                if (ReferenceEquals(null, sendPerWeekPromoEvent.EventObject))
                {
                    _logger.Error(string.Format("ExceptionMessage SendPerWeekPromoEvent: {0}", nullReferenceException.Message), nullReferenceException);
                }
                else
                {
                    _logger.Error(
                        string.Format("Event processing failed for SendPerWeekPromoEvent - OrderId {0}. ExceptionMessage: {1}",
                            sendPerWeekPromoEvent.EventObject.Webinar.idWebinar,
                            nullReferenceException.Message)
                        , nullReferenceException);
                }
            }
            catch (Exception exception)
            {
                _logger.Error(string.Format("SendPerWeekPromoEvent (outer) ExceptionMessage: {0}", exception.Message), exception);
            }
        }

        public void Handle(SendPerWeekPromoEvent<T> @event)
        {
            Process(@event);
        }
    }

    public class SendPerWeekPromoHandler : SendPerWeekPromoHandler<WebinarPromoViewModel>
    {
        public SendPerWeekPromoHandler(IFormatter generalFormatter, ILogger logger)
            : base(generalFormatter, logger)
        {

        }

        public SendPerWeekPromoHandler(IFormatter generalFormatter, INotificationDelivery notificationDelivery, ILogger logger)
            : base(generalFormatter, notificationDelivery, logger)
        {
        }

    }
}