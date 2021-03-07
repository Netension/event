using Netension.Event.Abstraction;
using System;

namespace Netension.Event.Containers
{
    public class EventPublisherRegister : IEventPublisherRegister
    {
        private readonly EventPublisherCollection _registrations;

        public EventPublisherRegister(EventPublisherCollection registrations)
        {
            _registrations = registrations;
        }

        public void Registrate(string key, Func<IEvent, bool> predicate)
        {
            if (key is null) throw new ArgumentNullException(nameof(key));
            if (predicate is null) throw new ArgumentNullException(nameof(predicate));

            _registrations.Add(key, predicate);
        }
    }
}
