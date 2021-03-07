using Microsoft.Extensions.Logging;
using Netension.Event.Abstraction;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Netension.Event
{
    /// <inheritdoc cref="IEventDispatcher"/>
    public class EventDispatcher : IEventDispatcher
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<EventDispatcher> _logger;

        /// <summary>
        /// Initialize a new instance of the <see cref="EventDispatcher"/>.
        /// </summary>
        /// <param name="serviceProvider"><see cref="IServiceProvider"/> for resolve <see cref="IEventHandler{TEvent}"/>.</param>
        /// <param name="logger">Instance of the logger.</param>
        public EventDispatcher(IServiceProvider serviceProvider, ILogger<EventDispatcher> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public Task DispatchAsync(IEvent @event, CancellationToken cancellationToken)
        {
            if (@event is null) throw new ArgumentNullException(nameof(@event));

            return DispatchInternalAsync(@event, cancellationToken);
        }

        private async Task DispatchInternalAsync(IEvent @event, CancellationToken cancellationToken)
        {
            _logger.LogDebug("Dispatch {id} event", @event.EventId);
            var eventHandlerType = typeof(IEventHandler<>).MakeGenericType(@event.GetType());

            var handlers = (dynamic)_serviceProvider.GetService(typeof(IEnumerable<>).MakeGenericType(eventHandlerType));

            if (handlers == null || handlers.Length == 0) _logger.LogDebug("Handler not found for {type} event type", @event.GetType());
            if (handlers == null) return;

            var handleTasks = new List<Task>();
            foreach (var handler in handlers)
            {
                handleTasks.Add(handler.HandleAsync((dynamic)@event, cancellationToken));
            }

            await Task.WhenAll(handleTasks);
        }
    }
}
