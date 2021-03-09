using Microsoft.Extensions.Logging;
using Netension.Event.Abstraction;
using Netension.Extensions.Correlation;
using Netension.Extensions.Correlation.Defaults;
using RabbitMQ.Client.Events;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Netension.Event.RabbitMQ.Unwrappers
{
    public class RabbitMQCorrelationUnwrapper : IRabbitMQEventUnwrapper
    {
        private readonly ICorrelationMutator _correlationMutator;
        private readonly IRabbitMQEventUnwrapper _next;
        private readonly ILogger<RabbitMQCorrelationUnwrapper> _logger;

        public RabbitMQCorrelationUnwrapper(ICorrelationMutator correlationMutator, IRabbitMQEventUnwrapper next, ILogger<RabbitMQCorrelationUnwrapper> logger)
        {
            _correlationMutator = correlationMutator;
            _next = next;
            _logger = logger;
        }

        public async Task<IEvent> UnwrapAsync(BasicDeliverEventArgs envelop, CancellationToken cancellationToken)
        {
            var @event = await _next.UnwrapAsync(envelop, cancellationToken);

            var correlationId = envelop.BasicProperties.Headers.GetCorrelationId();
            _logger.LogDebug("Set {property} to {value}", nameof(_correlationMutator.CorrelationId), correlationId);
            _correlationMutator.CorrelationId = correlationId;

            var causationId = envelop.BasicProperties.Headers.GetCausationId();
            _logger.LogDebug("Set {property} to {value}", nameof(_correlationMutator.CausationId), causationId);
            _correlationMutator.CausationId = causationId;

            _logger.LogDebug("Set {property} to {value}", nameof(_correlationMutator.MessageId), @event.EventId);
            _correlationMutator.MessageId = @event.EventId;

            return @event;
        }
    }
}
