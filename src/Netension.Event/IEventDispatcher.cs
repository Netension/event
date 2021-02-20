using Microsoft.Extensions.Logging;
using Netension.Event.Abstraction;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Netension.Event
{
    public class EventDispatcher : IEventDispatcher
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<EventDispatcher> _logger;

        public EventDispatcher(IServiceProvider serviceProvider, ILogger<EventDispatcher> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public async Task DispatchAsync(IEvent @event, CancellationToken cancellationToken)
        {
            var eventHandlerType = typeof(IEventHandler<>).MakeGenericType(@event.GetType());
            _logger.LogDebug("Looking for {type} event handlers", eventHandlerType);

            var handlers = (dynamic)_serviceProvider.GetService(typeof(IEnumerable<>).MakeGenericType(eventHandlerType));

            var handleTasks = new List<Task>();
            foreach (var handler in handlers)
            {
                handleTasks.Add(handler.HandleAsync((dynamic)@event, cancellationToken));
            }

            await Task.WhenAll(handleTasks);
        }
    }
}
