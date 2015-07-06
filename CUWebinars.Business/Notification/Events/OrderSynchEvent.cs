using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CUWebinars.Business.Notification.Email;
using CUWebinars.NotificationSystem.Event;

namespace CUWebinars.Business.Notification.Events
{
    public class OrderSynchEvent<T> : TtsBusEvent<T>, IAllowMultiple
        where T : OrderSynchMessage
    {
        public string RelativePath { get; set; }
        public bool ResendEvent { get; set; }
    }
}