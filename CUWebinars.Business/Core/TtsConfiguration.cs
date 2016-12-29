using System;
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

        public Int64 ConvertToCitrixOrgKey(string webinarOrganizerKey)
        {
            Int64 oKey;
            bool res = Int64.TryParse(webinarOrganizerKey, out oKey);
            if (res)
            {
                return oKey;
            }
            return 0;
        }

        public Int64 ConvertToCitrixWebinarKey(string webinarKey)
        {
            Int64 rKey;
            bool res = Int64.TryParse(webinarKey, out rKey);
            if (res)
            {
                return rKey;
            }
            return 0;         
        }
    }
}