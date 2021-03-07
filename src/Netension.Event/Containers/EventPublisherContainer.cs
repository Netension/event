using Microsoft.Extensions.Logging;
using Netension.Event.Abstraction;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Netension.Event.Containers
{
    public class EventPublisherContainer : IEventPublisherRegister, IEventPublisherResolver
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<EventPublisherContainer> _logger;

        private readonly IDictionary<string, Func<IEvent, bool>> _registrations = new Dictionary<string, Func<IEvent, bool>>();

        public EventPublisherContainer(IServiceProvider serviceProvider, ILogger<EventPublisherContainer> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public void Registrate(string key, Func<IEvent, bool> predicate)
        {
            if (key is null) throw new ArgumentNullException(nameof(key));
            if (predicate is null) throw new ArgumentNullException(nameof(predicate));

            _registrations.Add(key, predicate);
        }

        public IEnumerable<IEventPublisher> Resolve(IEvent @event)
        {
            _logger.LogDebug("Resolve {type} for {id}", typeof(IEventPublisher), @event.EventId);

            var factory = (Func<string, IEventPublisher>)_serviceProvider.GetService(typeof(Func<string, IEventPublisher>));
            foreach (var key in _registrations.Where(r => r.Value(@event)))
            {
                _logger.LogDebug("Resolve {key} publisher for {id} event", key.Key, @event.EventId);
                yield return factory(key.Key);
            }
        }
    }
}
