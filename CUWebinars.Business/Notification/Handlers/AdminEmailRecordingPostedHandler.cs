using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CUWebinars.Business.Models;
using CUWebinars.Business.Notification.Events;
using CUWebinars.Business.Notification.Formatters;
using CUWebinars.NotificationSystem.Event;
using Ninject.Extensions.Logging;

namespace CUWebinars.Business.Notification.Handlers
{
    public class AdminEmailRecordingPostedHandler<T> : IEventHandler<AdminEmailRecordingPostedEvent<T>>
        where T: Order
    {
        private readonly IFormatter _generalFormatter;
        private readonly ILogger _logger;
        private readonly INotificationDelivery _notificationDelivery;

        public AdminEmailRecordingPostedHandler(IFormatter generalFormatter, ILogger logger, INotificationDelivery notificationDelivery)
        {
            _generalFormatter = generalFormatter;
            _logger = logger;
            _notificationDelivery = notificationDelivery;
        }

        public void Handle(AdminEmailRecordingPostedEvent<T> adminEmailRecordingPostedEvent)
        {
            throw new NotImplementedException();
        }
    }
    public class AdminEmailRecordingPostedHandler : AdminEmailRecordingPostedHandler<Order>
    {
        public AdminEmailRecordingPostedHandler(IFormatter generalFormatter, ILogger logger, INotificationDelivery notificationDelivery)
            : base(generalFormatter, logger, notificationDelivery)
        {

        }
    }
}
