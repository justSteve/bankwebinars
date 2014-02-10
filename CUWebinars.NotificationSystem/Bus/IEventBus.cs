using CUWebinars.NotificationSystem.Event;

namespace CUWebinars.NotificationSystem.Bus
{
    public interface IEventBus
    {
        void RaiseEvent(IEvent evt);
    }
}
