using Microsoft.Extensions.Logging;
using Netension.Event.Abstraction;
using Netension.Event.RabbitMQ.Unwrappers;
using RabbitMQ.Client.Events;
using System.Threading;
using System.Threading.Tasks;

namespace Netension.Event.RabbitMQ.Receivers
{
    public class RabbitMQEventReceiver : IRabbitMQEventReceiver
    {
        private readonly IRabbitMQEventUnwrapper _unwrapper;
        private readonly IEventDispatcher _eventDispatcher;
        private readonly ILogger<RabbitMQEventReceiver> _logger;

        public RabbitMQEventReceiver(IRabbitMQEventUnwrapper unwrapper, IEventDispatcher eventDispatcher, ILogger<RabbitMQEventReceiver> logger)
        {
            _unwrapper = unwrapper;
            _eventDispatcher = eventDispatcher;
            _logger = logger;
        }

        public async Task ReceiveAsync(BasicDeliverEventArgs message, CancellationToken cancellationToken)
        {
            var @event = await _unwrapper.UnwrapAsync(message, cancellationToken);
            _logger.LogDebug("Receive {id} event", @event.EventId);
            _logger.LogTrace("Event object: {@event}", @event);

            await _eventDispatcher.DispatchAsync(@event, cancellationToken);
        }
    }
}
