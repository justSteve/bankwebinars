using System.Text.RegularExpressions;
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
    public class SendPerDayPromoHandler<T> : IEventHandler<SendPerDayPromoEvent<T>>
        where T : WebinarPromoViewModel
    {
        private readonly IFormatter _generalFormatter;
        private readonly INotificationDelivery _notificationDelivery;
        private readonly ILogger _logger;

        public SendPerDayPromoHandler(IFormatter generalFormatter
            , ILogger logger
            )
            : this(generalFormatter, new SmtpMessageDelivery(new Log4NetLogger(typeof(SmtpMessageDelivery))), logger)
        {

        }

        public SendPerDayPromoHandler(IFormatter generalFormatter
            , INotificationDelivery notificationDelivery
            , ILogger logger)
        {
            _generalFormatter = generalFormatter;
            _notificationDelivery = notificationDelivery;
            _logger = logger;

        }

        public virtual void Process(SendPerDayPromoEvent<T> sendPerDayPromoEvent)
        {
            string sPattern = @"<img[^>]+/>";
            Regex rgx = new Regex(sPattern);
            Match m = rgx.Match(sendPerDayPromoEvent.EventObject.Webinar.Presenter.BiographyLong);
            //HACK: Parse out the photo from the bio and carry the result via the no longer used EventBody.
            if (m.Success)
                sendPerDayPromoEvent.EventObject.EventBody = rgx.Replace(sendPerDayPromoEvent.EventObject.Webinar.Presenter.BiographyLong, "");
            try
            {
                var persistedNamePrefix = "PerWeekPromo_" + sendPerDayPromoEvent.EventObject.Affiliate.ttsDomain + '_';
                var notificationMessage = _generalFormatter
                    .Format(sendPerDayPromoEvent.EventObject, "SendPerDayPromo");


                notificationMessage.PersistedName = string.Format("{0}_{1}{2}",
                    persistedNamePrefix,
                    DateTime.Now.ToString(DomainConstants.DateTimeLongFormat),
                    ".htm"
                    );

                //                notificationMessage.To = sendPerDayPromoEvent.EventObject.Affiliate.WebUser.email;
                notificationMessage.To = "steve@ttstrain.com";
                _notificationDelivery.Notify(notificationMessage);
            }
            catch (NullReferenceException nullReferenceException)
            {
                if (ReferenceEquals(null, sendPerDayPromoEvent.EventObject))
                {
                    _logger.Error(string.Format("ExceptionMessage SendPerDayPromoEvent: {0}", nullReferenceException.Message), nullReferenceException);
                }
                else
                {
                    _logger.Error(
                        string.Format("Event processing failed for SendPerDayPromoEvent - OrderId {0}. ExceptionMessage: {1}",
                            sendPerDayPromoEvent.EventObject.Webinar.idWebinar,
                            nullReferenceException.Message)
                        , nullReferenceException);
                }
            }
            catch (Exception exception)
            {
                _logger.Error(string.Format("SendPerDayPromoEvent (outer) ExceptionMessage: {0}", exception.Message), exception);
            }
        }

        public void Handle(SendPerDayPromoEvent<T> @event)
        {
            Process(@event);
        }
    }

    public class SendPerDayPromoHandler : SendPerDayPromoHandler<WebinarPromoViewModel>
    {
        public SendPerDayPromoHandler(IFormatter generalFormatter, ILogger logger)
            : base(generalFormatter, logger)
        {

        }

        public SendPerDayPromoHandler(IFormatter generalFormatter, INotificationDelivery notificationDelivery, ILogger logger)
            : base(generalFormatter, notificationDelivery, logger)
        {
        }

    }
}