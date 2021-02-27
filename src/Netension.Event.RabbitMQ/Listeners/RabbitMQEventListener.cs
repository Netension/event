using Microsoft.Extensions.Logging;
using Netension.Event.Abstraction;
using Netension.Event.RabbitMQ.Options;
using Netension.Event.RabbitMQ.Receivers;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Netension.Event.RabbitMQ.Listeners
{
    public class RabbitMQEventListener : IEventListener
    {
        private readonly IConnection _connection;
        private readonly RabbitMQListenerOptions _options;
        private readonly IRabbitMQEventReceiver _receiver;
        private readonly ILogger<RabbitMQEventListener> _logger;

        private IModel _channel;

        public RabbitMQEventListener(IConnection connection, RabbitMQListenerOptions options, IRabbitMQEventReceiver receiver, ILogger<RabbitMQEventListener> logger)
        {
            _connection = connection;
            _options = options;
            _receiver = receiver;
            _logger = logger;
        }

        public Task ListenAsync(CancellationToken cancellationToken)
        {
            _channel = _connection.CreateModel();
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

            var consumerTag = $"{_options.Queue.ConsumerPrefix}-{Guid.NewGuid()}";
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
