using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Netension.Event.Abstraction;
using Netension.Event.RabbitMQ.Messages;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Netension.Event.RabbitMQ.Wrappers
{
    public class RabbitMQEventWrapper : IRabbitMQEventWrapper
    {
        private readonly IOptions<JsonSerializerOptions> _options;
        private readonly ILogger<RabbitMQEventWrapper> _logger;

        public RabbitMQEventWrapper(IOptions<JsonSerializerOptions> options, ILogger<RabbitMQEventWrapper> logger)
        {
            _options = options;
            _logger = logger;
        }

        public Task<RabbitMQMessage> WrapAsync(IEvent @event, CancellationToken cancellationToken)
        {
            if (@event == null) throw new ArgumentNullException(nameof(@event));

            _logger.LogDebug("Wrap {id} event", @event.EventId);

            var message = new RabbitMQMessage
            {
                Body = @event.Encode(_options.Value)
            };
            message.Headers.SetMessageType(@event.MessageType);

            return Task.FromResult(message);
        }
    }
}
