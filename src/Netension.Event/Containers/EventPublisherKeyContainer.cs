using Netension.Event.Abstraction;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Netension.Event.Containers
{
    public class EventPublisherKeyContainer : IEventPublisherKeyRegister, IEventPublisherKeyResolver
    {
        private IDictionary<string, Func<IEvent, bool>> registrations = new Dictionary<string, Func<IEvent, bool>>();

        public EventPublisherKeyContainer()
        {

        }

        public void Registrate(string key, Func<IEvent, bool> predicate)
        {
            registrations.Add(key, predicate);
        }

        public IEnumerable<string> Resolve(IEvent @event)
        {
            return registrations.Where(r => r.Value(@event)).Select(r => r.Key);
        }
    }
}
