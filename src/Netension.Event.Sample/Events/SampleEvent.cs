using Microsoft.Extensions.Logging;
using Netension.Event.Abstraction;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Netension.Event.Sample.Events
{
    public class SampleEvent : Event
    {
        public string Message { get; }

        public SampleEvent(string message)
        {
            Message = message;
        }
    }

    public class SampleEventHandler : IEventHandler<SampleEvent>
    {
        private readonly ILogger<SampleEventHandler> _logger;

        public SampleEventHandler(ILogger<SampleEventHandler> logger)
        {
            _logger = logger;
        }

        public Task HandleAsync(SampleEvent @event, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Received {id} event with message: {message}", @event.EventId, @event.Message);
            return Task.CompletedTask;
        }
    }
}
