using CUWebinars.NotificationSystem.Bus;

namespace CUWebinars.Business.Core
{
    public class TtsConfiguration
    {
        private readonly EventBus _notificationEventBus = new EventBus();

        public IEventBus NotificationEventBus
        {
            get { return _notificationEventBus; }
        }

        public void AddEventHandler(params NotificationSystem.Event.IEventHandler[] handlers)
        {
            _notificationEventBus.AddRange(handlers);
        }
    }
}