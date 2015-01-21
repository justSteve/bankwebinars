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
    public class PreviewOrderHandler<T> : IEventHandler<PreviewOrderEvent<T>>
        where T : Order
    {
        public PreviewOrderHandler(IFormatter generalFormatter, ILogger logger)
        {

        }

        public void Handle(PreviewOrderEvent<T> @event)
        {
            throw new NotImplementedException();
        }
    }

    public class PreviewOrderHandler : PreviewOrderHandler<Order>
    {
        public PreviewOrderHandler(IFormatter generalFormatter, ILogger logger)
            : base(generalFormatter, logger)
        {

        }
    }
}
