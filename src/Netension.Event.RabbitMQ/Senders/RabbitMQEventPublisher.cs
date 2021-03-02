using Microsoft.Extensions.Logging;
using Netension.Event.Abstraction;
using Netension.Event.RabbitMQ.Options;
using Netension.Event.RabbitMQ.Wrappers;
using RabbitMQ.Client;
using System.Threading;
using System.Threading.Tasks;

namespace Netension.Event.RabbitMQ.Senders
{
    public class RabbitMQEventPublisher : IEventPublisher
    {
        private readonly IModel _channel;
        private readonly IRabbitMQEventWrapper _wrapper;
        private readonly RabbitMQPublisherOptions _options;
        private readonly ILogger<RabbitMQEventPublisher> _logger;

        public RabbitMQEventPublisher(IModel channel, IRabbitMQEventWrapper wrapper, RabbitMQPublisherOptions options, ILogger<RabbitMQEventPublisher> logger)
        {
            _channel = channel;
            _wrapper = wrapper;
            _options = options;
            _logger = logger;
        }

        public async Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken)
            where TEvent : IEvent
        {
            await PublishAsync(@event, _options.RoutingKey, cancellationToken);
        }

        public async Task PublishAsync<TEvent>(TEvent @event, string topic, CancellationToken cancellationToken)
            where TEvent : IEvent
        {
            _logger.LogDebug("Send {id} event", @event.EventId);
            var message = await _wrapper.WrapAsync(@event, cancellationToken);

            var properties = _channel.CreateBasicProperties();
            properties.Headers = message.Headers;

            var options = _options;
            _channel.BasicPublish(options.Exchange, topic, options.Mandatory, properties, message.Body);
        }
    }
}
