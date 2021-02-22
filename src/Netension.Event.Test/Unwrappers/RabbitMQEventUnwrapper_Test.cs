using Microsoft.Extensions.Logging;
using Moq;
using Netension.Event.Defaults;
using Netension.Event.RabbitMQ.Unwrappers;
using Netension.Event.Test.Extensions;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Netension.Event.Test.Unwrappers
{
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

        [Fact(DisplayName = "RabbitMQEventUnwrapper - UnwrapAsync - Message-Type header does not present")]
        public async Task RabbitMQEventUnwrapper_UnwrapAsync_MessageTypeHeaderDoesNotPresent()
        {
            // Arrange
            var sut = CreateSUT();
            var @event = new Event(Guid.NewGuid());
            var basicPropertiesMock = new Mock<IBasicProperties>();

            basicPropertiesMock.SetupGet(bp => bp.Headers)
                .Returns(new Dictionary<string, object>());

            // Act
            // Assert
            await Assert.ThrowsAsync<InvalidOperationException>(async () => await sut.UnwrapAsync(new BasicDeliverEventArgs(null, 0, false, null, null, basicPropertiesMock.Object, @event.Encode()), CancellationToken.None));
        }

        [Fact(DisplayName = "RabbitMQEventUnwrapper - UnwrapAsync - Header is null")]
        public async Task RabbitMQEventUnwrapper_UnwrapAsync_HeaderIsNull()
        {
            // Arrange
            var sut = CreateSUT();
            var @event = new Event(Guid.NewGuid());
            var basicPropertiesMock = new Mock<IBasicProperties>();

            // Act
            // Assert
            await Assert.ThrowsAsync<InvalidOperationException>(async () => await sut.UnwrapAsync(new BasicDeliverEventArgs(null, 0, false, null, null, basicPropertiesMock.Object, @event.Encode()), CancellationToken.None));
        }

        [Fact(DisplayName = "RabbitMQEventUnwrapper - UnwrapAsync - Message-Type header is null")]
        public async Task RabbitMQEventUnwrapper_UnwrapAsync_MessageTypeHeaderIsNull()
        {
            // Arrange
            var sut = CreateSUT();
            var @event = new Event(Guid.NewGuid());
            var basicPropertiesMock = new Mock<IBasicProperties>();

            basicPropertiesMock.SetupGet(bp => bp.Headers)
                .Returns(new Dictionary<string, object> 
                {
                    { EventDefaults.MessageType, null }
                });

            // Act
            // Assert
            await Assert.ThrowsAsync<InvalidOperationException>(async () => await sut.UnwrapAsync(new BasicDeliverEventArgs(null, 0, false, null, null, basicPropertiesMock.Object, @event.Encode()), CancellationToken.None));
        }
    }
}
