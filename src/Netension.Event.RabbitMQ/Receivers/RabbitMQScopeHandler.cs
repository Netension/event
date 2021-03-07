using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client.Events;
using System.Threading;
using System.Threading.Tasks;

namespace Netension.Event.RabbitMQ.Receivers
{
    public class RabbitMQScopeHandler : IRabbitMQEventReceiver
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly IRabbitMQEventReceiver _next;
        private readonly ILogger<RabbitMQScopeHandler> _logger;

        public RabbitMQScopeHandler(IServiceScopeFactory serviceScopeFactory, IRabbitMQEventReceiver next, ILogger<RabbitMQScopeHandler> logger)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _next = next;
            _logger = logger;
        }

        public async Task ReceiveAsync(BasicDeliverEventArgs message, CancellationToken cancellationToken)
{
            using var scope = _serviceScopeFactory.CreateScope();
            _logger.LogDebug("New scope created for RabbitMQ event");
            await _next.ReceiveAsync(message, cancellationToken);
        }
    }
}
