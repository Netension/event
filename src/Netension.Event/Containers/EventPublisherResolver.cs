using Microsoft.Extensions.Logging;
using Netension.Event.Abstraction;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Netension.Event.Containers
{
    public class EventPublisherResolver : IEventPublisherResolver
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<EventPublisherResolver> _logger;

        private readonly IDictionary<string, Func<IEvent, bool>> _registrations;

        public EventPublisherResolver(IServiceProvider serviceProvider, EventPublisherCollection registrations, ILogger<EventPublisherResolver> logger)
        {
            _serviceProvider = serviceProvider;
            _registrations = registrations;
            _logger = logger;
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
