using System.Collections.Generic;
using CUWebinars.NotificationSystem.Event;

namespace CUWebinars.NotificationSystem.Bus
{
    public class AggregateEventBus : List<IEventBus>, IEventBus
    {
        public void RaiseEvent(IEvent evt)
        {
            foreach (var eb in this)
            {
                eb.RaiseEvent(evt);
            }
        }
    }
}
