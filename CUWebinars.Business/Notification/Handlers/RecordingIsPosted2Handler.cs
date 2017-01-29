using System;
using System.Collections.Generic;
using System.Linq;
using CUWebinars.Business.Constants;
using CUWebinars.Business.Notification.Email;
using CUWebinars.Business.Notification.Events;
using CUWebinars.NotificationSystem.Event;
using Ninject.Extensions.Logging;

namespace CUWebinars.Business.Notification.Handlers
{
    public class RecordingIsPosted2Handler<T> : IEventHandler<RecordingIsPosted2Event<T>>
        where T : RecordingIsPosted2Message
    {
        private readonly INotificationDelivery _RecordingIsPosted2NotificationDelivery;
        private readonly ILogger _logger;

        public RecordingIsPosted2Handler(INotificationDelivery notificationDelivery
            , ILogger logger)
        {
            _RecordingIsPosted2NotificationDelivery = notificationDelivery;
            _logger = logger;

        }

        public void Handle(RecordingIsPosted2Event<T> RecordingIsPosted2Event)
        {
            try
            {
                TtsConfigHelper ttsConfigHelper = new TtsConfigHelper();

                var persistedNamePrefix = "2OrderConfirmation";

                var notificationMessage = new NotificationMessage();

                notificationMessage.Body = RecordingIsPosted2Event.EventObject.Body;

                notificationMessage.PersistedName = string.Format("{0}_{1}{2}",
                    persistedNamePrefix,
                    DomainConstants.BuildUtcNowAsCts.ToString(DomainConstants.DateTimeLongFormat),
                    ".htm"
                );
                
                // parse potential 2ple emails, concept from http://stackoverflow.com/questions/14689044/regex-split-on-comma-space-or-semi-colon-delimitted-string
                char[] delimiters = new[] { ',', ';', ' ' };  // List of your delimiters
                List<string> addressess = RecordingIsPosted2Event.EventObject.Recipients.ToList();

                if (addressess.Count > 0)
                {
                    notificationMessage.To = addressess[0];
                    if (addressess.Count > 1)
                    {
                        notificationMessage.Addresses = addressess.Skip(1).ToList(); // put the rest in the CC
                    }
                }

                notificationMessage.From = ttsConfigHelper.GetMandrillFromAddress(); // approach for From Address TBD!
                notificationMessage.Subject = RecordingIsPosted2Event.EventObject.Subject; // or ttsConfigHelper.GetWeeklyInvoiceEmailSubject(); // needs to be done early (prior to RecordingIsPosted2.cshtml view being Rendered to String)

                _RecordingIsPosted2NotificationDelivery.Notify(notificationMessage);
            }
            catch (NullReferenceException nullReferenceException)
            {
                if (ReferenceEquals(null, RecordingIsPosted2Event.EventObject))
                {
                    _logger.Error(string.Format("ExceptionMessage RecordingIsPosted2Event: {0}", nullReferenceException.Message), nullReferenceException);
                }
                else
                {
                    //_logger.Error(
                    //    string.Format("Event processing failed for RecordingIsPosted2Event - OrderId {0}. ExceptionMessage: {1}",
                    //        RecordingIsPosted2Event.EventObject.Webinar.idWebinar,
                    //        nullReferenceException.Message)
                    //    , nullReferenceException);
                }
            }
            catch (Exception exception)
            {
                _logger.Error(string.Format("RecordingIsPosted2Event (outer) ExceptionMessage: {0}", exception.Message), exception);
            }
        }
    }
}