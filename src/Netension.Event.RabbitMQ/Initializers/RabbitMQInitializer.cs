using Microsoft.Extensions.Logging;
using Netension.Event.RabbitMQ.Options;
using RabbitMQ.Client;
using System.Threading;
using System.Threading.Tasks;

namespace Netension.Event.RabbitMQ.Initializers
{
    public class RabbitMQInitializer : IRabbitMQInitializer
    {
        private readonly ILogger<RabbitMQInitializer> _logger;

        public RabbitMQInitializer(ILogger<RabbitMQInitializer> logger)
        {
            _logger = logger;
        }

        public Task InitializeAsync(IModel channel, RabbitMQListenerOptions options, CancellationToken cancellationToken)
        {
            _logger.LogDebug("Create {queue} queue", options.Queue.Name);
            channel.QueueDeclare(options.Queue.Name, options.Queue.Durable, options.Queue.Exclusive, options.Queue.AutoDelete, options.Queue.Arguments);

            foreach (var binding in options.Bindings)
            {
                _logger.LogDebug("Bind {queue} queue to {exchange} exchange", options.Queue.Name, binding.Exchange);
                channel.QueueBind(options.Queue.Name, binding.Exchange, binding.RoutingKey, binding.Arguments);
            }

            return Task.CompletedTask;
        }
    }
}
