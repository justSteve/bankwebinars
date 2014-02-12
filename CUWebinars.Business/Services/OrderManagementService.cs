using System.Collections.Generic;
using System.Linq;
using BrockAllen.MembershipReboot;
using CUWebinars.Business.Core;
using CUWebinars.Business.Models;
using CUWebinars.Business.Notification;
using CUWebinars.Business.Notification.Events;
using CUWebinars.Business.Repository;
using CUWebinars.NotificationSystem.Event;
using IEvent = CUWebinars.NotificationSystem.Event.IEvent;
using IEventSource = CUWebinars.NotificationSystem.Event.IEventSource;

namespace CUWebinars.Business.Services
{
    public class OrderManagementService : IOrderManagementService, IEventSource
    {
        private readonly IOptionRepository _optionRepository;
        private readonly IRefDataRepository _refDataRepository;
        private readonly TtsConfiguration _ttsConfig;
        List<IEvent> events = new List<IEvent>();

        public OrderManagementService(IOptionRepository optionRepository, IRefDataRepository refDataRepository, TtsConfiguration ttsConfig)
        {
            _optionRepository = optionRepository;
            _refDataRepository = refDataRepository;
            _ttsConfig = ttsConfig;
        }

        public string BuildConnectionInfo(OrderRow orderRow)
        {
            // add code here to build string

            var option = _optionRepository.FindOption((int)orderRow.RegistrationType);

            return string.Empty;
        }

        public IList<Option> GetOptionsByWebinarId(int id)
        {
            return _refDataRepository.FindOptionsByWebinarId(id);
        }

        public void CreateOrderEvent(Order order, UserAccount userAccount)
        {
            AddEvent(new OrderSubmittedEvent<UserAccount>{ Account = userAccount, Order = order });
        }


        public void DispatchDummyOrder()
        {
            foreach (var orderSubmittedEvent in GetEvents())
            {
                _ttsConfig.NotificationEventBus.RaiseEvent(orderSubmittedEvent);
            }
        }

        public IEnumerable<IEvent> GetEvents()
        {
            return events;
        }

        protected void AddEvent<TE>(TE orderEvent) where TE : IEvent
        {
            if (orderEvent is IAllowMultiple || events.All(x => x.GetType() != orderEvent.GetType()))
            {
                events.Add(orderEvent);
            }
        }

        public void Clear()
        {
            events.Clear();
        }
    }
}
