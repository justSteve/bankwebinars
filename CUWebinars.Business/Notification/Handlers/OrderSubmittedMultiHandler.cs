using CUWebinars.Business.Constants;
using CUWebinars.Business.Notification.Email;
using CUWebinars.Business.Notification.Events;
using CUWebinars.NotificationSystem.Event;
using Ninject.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CUWebinars.Business.Notification.Handlers
{
    public class OrderSubmittedMultiHandler<T> : IEventHandler<OrderSubmittedMultiEvent<T>>
        where T : OrderSubmittedMultiMessage
    {
        private readonly INotificationDelivery _orderSubmittedMultiNotificationDelivery;
        private readonly ILogger _logger;

        public OrderSubmittedMultiHandler(INotificationDelivery notificationDelivery
            , ILogger logger)
        {
            _orderSubmittedMultiNotificationDelivery = notificationDelivery;
            _logger = logger;

        }

        public void Handle(OrderSubmittedMultiEvent<T> orderSubmittedMultiEvent)
        {
            try
            {
                TtsConfigHelper ttsConfigHelper = new TtsConfigHelper();

                var persistedNamePrefix = "MultiOrderConfirmation";

                var notificationMessage = new NotificationMessage();

                notificationMessage.Body = orderSubmittedMultiEvent.EventObject.Body;

                notificationMessage.PersistedName = string.Format("{0}_{1}{2}",
                    persistedNamePrefix,
                    DomainConstants.BuildUtcNowAsCts.ToString(DomainConstants.DateTimeLongFormat),
                    ".htm"
                    );

                // notificationMessage.PersistedName = "NOT USED FOR WEEKLY INVOICE PROCESSING";

                // parse potential multiple emails, concept from http://stackoverflow.com/questions/14689044/regex-split-on-comma-space-or-semi-colon-delimitted-string
                char[] delimiters = new[] { ',', ';', ' ' };  // List of your delimiters
                List<string> addressess = orderSubmittedMultiEvent.EventObject.Recipients.ToList();

                if (addressess.Count > 0)
                {
                    notificationMessage.To = addressess[0];
                    if (addressess.Count > 1)
                    {
                        notificationMessage.Addresses = addressess.Skip(1).ToList(); // put the rest in the CC
                    }
                }

                notificationMessage.From = ttsConfigHelper.GetMandrillFromAddress(); // approach for From Address TBD!
                notificationMessage.Subject = orderSubmittedMultiEvent.EventObject.Subject; // or ttsConfigHelper.GetWeeklyInvoiceEmailSubject(); // needs to be done early (prior to OrderSubmittedMulti.cshtml view being Rendered to String)

                _orderSubmittedMultiNotificationDelivery.Notify(notificationMessage);
            }
            catch (NullReferenceException nullReferenceException)
            {
                if (ReferenceEquals(null, orderSubmittedMultiEvent.EventObject))
                {
                    _logger.Error(string.Format("ExceptionMessage OrderSubmittedMultiEvent: {0}", nullReferenceException.Message), nullReferenceException);
                }
                else
                {
                    //_logger.Error(
                    //    string.Format("Event processing failed for OrderSubmittedMultiEvent - OrderId {0}. ExceptionMessage: {1}",
                    //        OrderSubmittedMultiEvent.EventObject.Webinar.idWebinar,
                    //        nullReferenceException.Message)
                    //    , nullReferenceException);
                }
            }
            catch (Exception exception)
            {
                _logger.Error(string.Format("orderSubmittedMultiEvent (outer) ExceptionMessage: {0}", exception.Message), exception);
            }
        }
    }

    public class OrderSubmittedMultiHandler : OrderSubmittedMultiHandler<OrderSubmittedMultiMessage>
    {
        public OrderSubmittedMultiHandler(INotificationDelivery orderSubmittedMultiDelivery, ILogger logger)
            : base(orderSubmittedMultiDelivery, logger)
        {
        }

    }
}
