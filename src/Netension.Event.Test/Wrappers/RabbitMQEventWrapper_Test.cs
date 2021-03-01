using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Netension.Event.Abstraction;
using Netension.Event.Defaults;
using Netension.Event.RabbitMQ.Wrappers;
using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Netension.Event.Test.Wrappers
{
    public class RabbitMQEventWrapper_Test
    {
        private readonly ILogger<RabbitMQEventWrapper> _logger;
        private Mock<IOptions<JsonSerializerOptions>> _optionsMock;

        public RabbitMQEventWrapper_Test(ITestOutputHelper outputHelper)
        {
            _logger = new LoggerFactory()
                        .AddXUnit(outputHelper)
                        .CreateLogger<RabbitMQEventWrapper>();
        }

        private RabbitMQEventWrapper CreateSUT()
        {
            _optionsMock = new Mock<IOptions<JsonSerializerOptions>>();
            _optionsMock.SetupGet(o => o.Value)
                .Returns(new JsonSerializerOptions());

            return new RabbitMQEventWrapper(_optionsMock.Object, _logger);
        }

        [Fact(DisplayName = "RabbitMQEventWrapper - WrapAsync - Serialize body")]
        public async Task RabbitMQEventWrapper_WrapAsync_SerializeBody()
        {
            // Arrange
            var sut = CreateSUT();
            var @event = new Event(Guid.NewGuid());

            // Act
            var message = await sut.WrapAsync(@event, CancellationToken.None);

            // Assert
            Assert.Equal(@event.Encode(new JsonSerializerOptions()), message.Body);
        }

        [Fact(DisplayName = "RabbitMQEventWrapper - WrapAsync - Set Message-Type header")]
        public async Task RabbitMQEventWrapper_WrapAsync_SetMessageType()
        {
            // Arrange
            var sut = CreateSUT();
            var @event = new Event(Guid.NewGuid());

            // Act
            var message = await sut.WrapAsync(@event, CancellationToken.None);

            // Assert
            Assert.Equal(@event.MessageType, message.Headers[EventDefaults.MessageType]);
        }

        [Fact(DisplayName = "RabbitMQEventWrapper - WrapAsync - Null event")]
        public async Task RabbitMQEventWrapper_WrapAsync_NullEvent()
        {
            // Arrange
            var sut = CreateSUT();

            // Act
            // Assert
            await Assert.ThrowsAsync<ArgumentNullException>(async () => await sut.WrapAsync(null, CancellationToken.None));
        }
    }
}
