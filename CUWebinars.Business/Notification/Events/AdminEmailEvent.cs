using CUWebinars.NotificationSystem.Event;
using System.Collections.Generic;

namespace CUWebinars.Business.Notification.Events
{
    public class AdminEmailEvent<T> : TtsBusEvent<T>, IAllowMultiple
    {
        public IEnumerable<string> Recipients { get; set; }
    }
}
