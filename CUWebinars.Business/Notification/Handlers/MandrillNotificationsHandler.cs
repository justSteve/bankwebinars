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
    public class MandrillNotificationsHandler<T> : IEventHandler<MandrillNotificationEvent<T>>
        where T : MandrillNotificationMessage
    {
        private readonly INotificationDelivery _RecordingIsPosted2NotificationDelivery;
        private readonly ILogger _logger;

        public MandrillNotificationsHandler(INotificationDelivery notificationDelivery
            , ILogger logger)
        {
            _RecordingIsPosted2NotificationDelivery = notificationDelivery;
            _logger = logger;

        }

        public void Handle(MandrillNotificationEvent<T> mandrillNotificationEvent)
        {
            try
            {
                TtsConfigHelper ttsConfigHelper = new TtsConfigHelper();

                var persistedNamePrefix = "2OrderConfirmation";

                var notificationMessage = new NotificationMessage();

                notificationMessage.Body = mandrillNotificationEvent.EventObject.Body;

                notificationMessage.PersistedName = string.Format("{0}_{1}{2}",
                    persistedNamePrefix,
                    DomainConstants.BuildUtcNowAsCts.ToString(DomainConstants.DateTimeLongFormat),
                    ".htm"
                );
                
                // parse potential 2ple emails, concept from http://stackoverflow.com/questions/14689044/regex-split-on-comma-space-or-semi-colon-delimitted-string
                char[] delimiters = new[] { ',', ';', ' ' };  // List of your delimiters
                List<string> addressess = mandrillNotificationEvent.EventObject.Recipients.ToList();

                if (addressess.Count > 0)
                {
                    notificationMessage.To = addressess[0];
                    if (addressess.Count > 1)
                    {
                        notificationMessage.Addresses = addressess.Skip(1).ToList(); // put the rest in the CC
                    }
                }
                _logger.Info("MandrillNotificationHandler fires: " + notificationMessage.Subject + " to: " +  notificationMessage.Addresses);
                notificationMessage.From = ttsConfigHelper.GetMandrillFromAddress(); // approach for From Address TBD!
                notificationMessage.Subject = mandrillNotificationEvent.EventObject.Subject; // or ttsConfigHelper.GetWeeklyInvoiceEmailSubject(); // needs to be done early (prior to RecordingIsPosted2.cshtml view being Rendered to String)

                _RecordingIsPosted2NotificationDelivery.Notify(notificationMessage);
            }
            catch (NullReferenceException nullReferenceException)
            {
                if (ReferenceEquals(null, mandrillNotificationEvent.EventObject))
                {
                    _logger.Error(string.Format("ExceptionMessage MandrillNotificationEvent: {0}", nullReferenceException.Message), nullReferenceException);
                }
                else
                {
                    IList<string> strings = mandrillNotificationEvent.EventObject.Recipients.ToArray();
                    _logger.FatalException(
                        string.Format("Event processing failed for MandrillNotificationEvent - recipients {0}. ExceptionMessage: {1}",
                            string.Join(", ", strings),
                            nullReferenceException.Message)
                        , nullReferenceException);
                }
            }
            catch (Exception exception)
            {
                _logger.Error(string.Format("MandrillNotificationEvent (outer) ExceptionMessage: {0}", exception.Message), exception);
            }
        }
    }
}