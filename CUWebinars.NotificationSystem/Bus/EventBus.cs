using System;
using System.Collections.Generic;
using System.Linq;
using CUWebinars.NotificationSystem.Event;

namespace CUWebinars.NotificationSystem.Bus
{
    public class EventBus : List<IEventHandler>, IEventBus
    {
        readonly Dictionary<Type, IEnumerable<IEventHandler>> _handlerCache = new Dictionary<Type, IEnumerable<IEventHandler>>();
        readonly GenericMethodActionBuilder<IEventHandler, IEvent> _actions = new GenericMethodActionBuilder<IEventHandler, IEvent>(typeof(IEventHandler<>), "Handle");

        public void RaiseEvent(IEvent evt)
        {
            var action = GetAction(evt);
            var matchingHandlers = GetHandlers(evt);

            foreach (var handler in matchingHandlers)
            {
                action(handler, evt);
            }
        }

        Action<IEventHandler, IEvent> GetAction(IEvent evt)
        {
            return _actions.GetAction(evt);
        }

        private IEnumerable<IEventHandler> GetHandlers(IEvent evt)
        {
            var eventType = evt.GetType();

            if (_handlerCache.ContainsKey(eventType)) return _handlerCache[eventType];

            var eventHandlerType = typeof(IEventHandler<>).MakeGenericType(eventType);

            //  Method group conversion used in the where clause. Same as (l => eventHandlerType.IsInstanceOfType(l))
            var query = this.Where(eventHandlerType.IsInstanceOfType);

            var handlers = query.ToArray();
                
            _handlerCache.Add(eventType, handlers);

            return _handlerCache[eventType];
        }
    }

}
