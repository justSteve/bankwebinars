using System.Collections.Generic;

namespace CUWebinars.NotificationSystem.Event
{
    public interface IEventSource
    {
        IEnumerable<IEvent> GetEvents();
        void Clear();
    }
}
