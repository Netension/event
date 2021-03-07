using Microsoft.Extensions.Logging;
using Netension.Event.Abstraction;
using Netension.Event.Containers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Netension.Event.Publishers
{
    public class EventPublisher : IEventPublisher
    {
        private readonly IEventPublisherResolver _resolver;
        private readonly ILogger<EventPublisher> _logger;

        public EventPublisher(IEventPublisherResolver resolver, ILogger<EventPublisher> logger)
        {
            _resolver = resolver;
            _logger = logger;
        }

        public async Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken)
            where TEvent : IEvent
        {
            if (@event is null) throw new ArgumentNullException(nameof(@event));

            await PublishInternalAsync(@event, cancellationToken);
        }

        public async Task PublishAsync<TEvent>(TEvent @event, string topic, CancellationToken cancellationToken)
            where TEvent : IEvent
        {
            if (@event is null) throw new ArgumentNullException(nameof(@event));

            await PublishInternalAsync(@event, topic, cancellationToken);
        }

        private async Task PublishInternalAsync<TEvent>(TEvent @event, CancellationToken cancellationToken)
            where TEvent : IEvent
        {
            var publishers = _resolver.Resolve(@event);

            if (!publishers.Any())
            {
                _logger.LogError("Publisher not found for {id} event", @event.EventId);
                throw new InvalidOperationException($"Publisher not found for {@event.EventId} event");
            }

            var tasks = new List<Task>();
            foreach (var publisher in publishers)
            {
                tasks.Add(publisher.PublishAsync(@event, cancellationToken));
            }
            await Task.WhenAll(tasks);
        }

        private async Task PublishInternalAsync<TEvent>(TEvent @event, string topic, CancellationToken cancellationToken)
            where TEvent : IEvent
        {
            var publishers = _resolver.Resolve(@event);

            if (!publishers.Any())
            {
                _logger.LogError("Publisher not found for {id} event", @event.EventId);
                throw new InvalidOperationException($"Publisher not found for {@event.EventId} event");
            }

            var tasks = new List<Task>();
            foreach (var publisher in publishers)
            {
                tasks.Add(publisher.PublishAsync(@event, topic, cancellationToken));
            }
            await Task.WhenAll(tasks);
        }
    }
}
