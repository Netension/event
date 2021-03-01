using Microsoft.Extensions.Logging;
using Netension.Event.Abstraction;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Netension.Event.RabbitMQ.Unwrappers
{
    public class RabbitMQEventUnwrapper : IRabbitMQEventUnwrapper
    {
        private readonly ILogger<RabbitMQEventUnwrapper> _logger;

        public RabbitMQEventUnwrapper(ILogger<RabbitMQEventUnwrapper> logger)
        {
            _logger = logger;
        }

        public Task<IEvent> UnwrapAsync(BasicDeliverEventArgs envelop, CancellationToken cancellationToken)
        {
            var messageType = envelop.BasicProperties.Headers.GetMessageType();

            _logger.LogDebug("Unwrap {messageType} event", messageType);

            return Task.FromResult(envelop.Body.Decode(Type.GetType(messageType)));
        }
    }
}
