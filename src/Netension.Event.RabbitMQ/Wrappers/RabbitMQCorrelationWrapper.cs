using Microsoft.Extensions.Logging;
using Netension.Event.Abstraction;
using Netension.Event.RabbitMQ.Messages;
using Netension.Extensions.Correlation;
using Netension.Extensions.Correlation.Defaults;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Netension.Event.RabbitMQ.Wrappers
{
    public class RabbitMQCorrelationWrapper : IRabbitMQEventWrapper
    {
        private readonly ICorrelationAccessor _correlationAccessor;
        private readonly IRabbitMQEventWrapper _next;
        private readonly ILogger<RabbitMQCorrelationWrapper> _logger;

        public RabbitMQCorrelationWrapper(ICorrelationAccessor correlationAccessor, IRabbitMQEventWrapper next, ILogger<RabbitMQCorrelationWrapper> logger)
        {
            _correlationAccessor = correlationAccessor;
            _next = next;
            _logger = logger;
        }

        public async Task<RabbitMQMessage> WrapAsync<TEvent>(TEvent @event, CancellationToken cancellationToken) where TEvent : IEvent
        {
            var message = await _next.WrapAsync(@event, cancellationToken);

            _logger.LogDebug("Set {header} header to {id}", CorrelationDefaults.CorrelationId, _correlationAccessor.CorrelationId);
            message.Headers.SetCorrelationId(_correlationAccessor.CorrelationId);

            _logger.LogDebug("Set {header} header to {id}", CorrelationDefaults.CausationId, _correlationAccessor.MessageId);
            message.Headers.SetCausationId(_correlationAccessor.MessageId);

            return message;
        }
    }
}
