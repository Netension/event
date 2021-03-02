using Microsoft.Extensions.Logging;
using Netension.Event.Abstraction;
using Netension.Event.Containers;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Netension.Event.Publishers
{
    public class EventPublisher : IEventPublisher
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IEventPublisherKeyResolver _resolver;
        private readonly ILogger<EventPublisher> _logger;

        public EventPublisher(IServiceProvider serviceProvider, IEventPublisherKeyResolver resolver, ILogger<EventPublisher> logger)
        {
            _serviceProvider = serviceProvider;
            _resolver = resolver;
            _logger = logger;
        }

        public async Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken)
            where TEvent : IEvent
        {
            var keys = _resolver.Resolve(@event);
            if (!keys.Any())
            {
                _logger.LogWarning("Publisher not found for {id} event", @event.EventId);
                return;
            }
            var factory = (Func<string, IEventPublisher>)_serviceProvider.GetService(typeof(Func<string, IEventPublisher>));            
            foreach (var key in keys)
            {
                _logger.LogDebug("Publish {id} to {key} publisher", @event.EventId, key);
                var publisher = factory(key);
                await publisher.PublishAsync(@event, cancellationToken);
            }
        }

        public async Task PublishAsync<TEvent>(TEvent @event, string topic, CancellationToken cancellationToken)
            where TEvent : IEvent
        {
            var keys = _resolver.Resolve(@event);
            if (!keys.Any())
            {
                _logger.LogWarning("Publisher not found for {id} event", @event.EventId);
                return;
            }
            var factory = (Func<string, IEventPublisher>)_serviceProvider.GetService(typeof(Func<string, IEventPublisher>));
            foreach (var key in keys)
            {
                _logger.LogDebug("Publish {id} to {key} publisher", @event.EventId, key);
                var publisher = factory(key);
                await publisher.PublishAsync(@event, topic, cancellationToken);
            }
        }
    }
}
