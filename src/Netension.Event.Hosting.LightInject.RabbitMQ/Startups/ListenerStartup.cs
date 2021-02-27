using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Netension.Event.Abstraction;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Netension.Event.Hosting.LightInject.RabbitMQ.Startups
{
    public class ListenerStartup : BackgroundService
    {
        private readonly IEnumerable<IEventListener> _listeners;
        private readonly ILogger<ListenerStartup> _logger;

        public ListenerStartup(IEnumerable<IEventListener> listeners, ILogger<ListenerStartup> logger)
        {
            _listeners = listeners;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogDebug("Start {0} listener(s).", _listeners.Count());
            foreach (var listener in _listeners)
            {
                await listener.ListenAsync(stoppingToken);
            }
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogDebug("Stop {0} listener(s).", _listeners.Count());
            foreach (var listener in _listeners)
            {
                await listener.StopAsync(cancellationToken);
            }
        }
    }
}
