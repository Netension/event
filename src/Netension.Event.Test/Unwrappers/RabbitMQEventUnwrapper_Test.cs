using Microsoft.Extensions.Logging;
using Moq;
using Netension.Event.Defaults;
using Netension.Event.RabbitMQ.Unwrappers;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Netension.Event.Test.Unwrappers
{
    public static class RabbitMQExtensions
    {
        public static ReadOnlyMemory<byte> Encode(this Event @event)
        {
            return new ReadOnlyMemory<byte>(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(@event)));
        }
    }

    public class RabbitMQEventUnwrapper_Test
    {
        private readonly ILogger<RabbitMQEventUnwrapper> _logger;

        public RabbitMQEventUnwrapper_Test(ITestOutputHelper outputHelper)
        {
            _logger = new LoggerFactory()
                        .AddXUnit(outputHelper)
                        .CreateLogger<RabbitMQEventUnwrapper>();
        }

        private RabbitMQEventUnwrapper CreateSUT()
        {
            return new RabbitMQEventUnwrapper(_logger);
        }

        [Fact(DisplayName = "RabbitMQEventUnwrapper - UnwrapAsync - Determine event type")]
        public async Task RabbitMQEventUnwrapper_UnwrapAsync_DetermineEventType()
        {
            // Arrange
            var sut = CreateSUT();
            var @event = new Event(Guid.NewGuid());
            var basicPropertiesMock = new Mock<IBasicProperties>();

            basicPropertiesMock.SetupGet(bp => bp.Headers)
                .Returns(new Dictionary<string, object> { { EventDefaults.MessageType, @event.MessageType } });

            // Act
            var result = await sut.UnwrapAsync(new BasicDeliverEventArgs(null, 0, false, null, null, basicPropertiesMock.Object,  @event.Encode()), CancellationToken.None);

            // Assert
            Assert.Equal(@event, result);
        }
    }
}
