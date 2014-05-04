using System.Collections.Generic;
using CUWebinars.Business.Models;
using CUWebinars.NotificationSystem.Event;

namespace CUWebinars.Business.Notification.Events
{
    public class SendConnectionInfoEvent<T> : TtsBusEvent<T>, IAllowMultiple
        where T : Order
    {
        
    }
}
