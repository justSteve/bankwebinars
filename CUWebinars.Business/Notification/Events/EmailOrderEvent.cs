using CUWebinars.Business.Models;
using CUWebinars.NotificationSystem.Event;
using System.Collections.Generic;

namespace CUWebinars.Business.Notification.Events
{
    public class EmailOrderEvent<T> : TtsBusEvent<T>, IAllowMultiple
        where T : Order
    {
        public IEnumerable<string> Recipients { get; set; }
    }
}
