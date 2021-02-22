using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Netension.Event.Hosting.LightInject.RabbitMQ.Options;
using Netension.Event.RabbitMQ.Receivers;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Netension.Event.Hosting.LightInject.RabbitMQ.Listeners
{
    public class RabbitMQListener : BackgroundService
    {
        private readonly IConnection _connection;
        private readonly IOptions<RabbitMQListenerOptions> _options;
        private readonly IRabbitMQEventReceiver _eventReceiver;
        private readonly ILogger<RabbitMQListener> _logger;

        public RabbitMQListener(IConnection connection, IOptions<RabbitMQListenerOptions> options, IRabbitMQEventReceiver eventReceiver, ILogger<RabbitMQListener> logger)
        {
            _connection = connection;
            _options = options;
            _eventReceiver = eventReceiver;
            _logger = logger;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var options = _options.Value;
            var channel = _connection.CreateModel();
            var consumer = new AsyncEventingBasicConsumer(channel);

            consumer.Received += async (sender, eventArgs) =>
            {
                _logger.LogDebug("Receiving {id} on {channel} RabbitMQ channel", eventArgs.DeliveryTag, channel.ChannelNumber);
                try
                {
                    await _eventReceiver.ReceiveAsync(eventArgs, stoppingToken);
                    channel.BasicAck(eventArgs.DeliveryTag, false);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Exception during receive {id} event.", eventArgs.BasicProperties.MessageId);
                    channel.BasicNack(eventArgs.DeliveryTag, false, true);
                }
            };

            channel.BasicConsume(options.Queue, false, options.Tag, false, false, new Dictionary<string, object>(), consumer);
            _logger.LogDebug("Listening on {queue} queue", options.Queue);

            return Task.CompletedTask;
        }
    }
}
