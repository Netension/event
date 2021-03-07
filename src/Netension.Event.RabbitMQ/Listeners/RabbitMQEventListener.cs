using Microsoft.Extensions.Logging;
using Netension.Event.Abstraction;
using Netension.Event.RabbitMQ.Extensions;
using Netension.Event.RabbitMQ.Initializers;
using Netension.Event.RabbitMQ.Options;
using Netension.Event.RabbitMQ.Receivers;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

namespace Netension.Event.RabbitMQ.Listeners
{
    public class RabbitMQEventListener : IEventListener
    {
        private readonly IModel _channel;
        private readonly RabbitMQListenerOptions _options;
        private readonly IRabbitMQEventReceiver _receiver;
        private readonly IRabbitMQInitializer _initializer;
        private readonly ILogger<RabbitMQEventListener> _logger;

        public RabbitMQEventListener(IModel channel, RabbitMQListenerOptions options, IRabbitMQEventReceiver receiver, IRabbitMQInitializer initializer, ILogger<RabbitMQEventListener> logger)
        {
            _channel = channel;
            _options = options;
            _receiver = receiver;
            _initializer = initializer;
            _logger = logger;
        }

        [ExcludeFromCodeCoverage]
        public Task ListenAsync(CancellationToken cancellationToken)
        {
            _initializer.InitializeAsync(_channel, _options, cancellationToken);

            var consumer = new AsyncEventingBasicConsumer(_channel);

            consumer.Received += async (sender, eventArgs) =>
            {
                _logger.LogDebug("Receiving {id} on {channel} RabbitMQ channel", eventArgs.DeliveryTag, _channel.ChannelNumber);
                try
                {
                    await _receiver.ReceiveAsync(eventArgs, cancellationToken);
                    _channel.BasicAck(eventArgs.DeliveryTag, false);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Exception during receive {id} event.", eventArgs.BasicProperties.MessageId);
                    _channel.BasicNack(eventArgs.DeliveryTag, false, true);
                }
            };

            var consumerTag = _options.Queue.ConsumerPrefix.NewConsumerTag();
            _channel.BasicConsume(_options.Queue.Name, _options.Queue.AutoAck, consumerTag, _options.Queue.NoLocal, _options.Queue.Exclusive, _options.Queue.Arguments, consumer);
            _logger.LogDebug("{tag} consumer has been created.", consumerTag);

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _channel.Close();
            _logger.LogDebug("{id} channel has been closed.", _channel.ChannelNumber);
            return Task.CompletedTask;
        }
    }
}
