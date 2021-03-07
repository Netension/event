using AutoFixture;
using Microsoft.Extensions.Logging;
using Moq;
using Netension.Event.Abstraction;
using Netension.Event.Defaults;
using Netension.Event.Extensions;
using Netension.Event.RabbitMQ.Messages;
using Netension.Event.RabbitMQ.Options;
using Netension.Event.RabbitMQ.Senders;
using Netension.Event.RabbitMQ.Wrappers;
using Netension.Event.Test.Extensions;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Netension.Event.Test.Publishers
{
    public class RabbitMQEventPublisher_Test
    {
        private readonly ILogger<RabbitMQEventPublisher> _logger;
        private Mock<IRabbitMQEventWrapper> _eventWrapperMock;
        private Mock<IModel> _channelMock;
        private RabbitMQPublisherOptions _options;

        public RabbitMQEventPublisher_Test(ITestOutputHelper outputHelper)
        {
            _logger = new LoggerFactory()
                        .AddXUnit(outputHelper)
                        .CreateLogger<RabbitMQEventPublisher>();
        }

        private RabbitMQEventPublisher CreateSUT()
        {
            _eventWrapperMock = new Mock<IRabbitMQEventWrapper>();
            _channelMock = new Mock<IModel>();
            _options = new Fixture().Create<RabbitMQPublisherOptions>();

            return new RabbitMQEventPublisher(_channelMock.Object, _eventWrapperMock.Object, _options, _logger);
        }

        [Fact(DisplayName = "RabbitMQEventSender - PublishAsync - Wrap event")]
        public async Task RabbitMQEventSender_PublishAsync_WrapEvent()
        {
            // Arrange
            var sut = CreateSUT();
            var @event = new Event(Guid.NewGuid());

            _eventWrapperMock.Setup(ew => ew.WrapAsync(It.IsAny<IEvent>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new RabbitMQMessage());

            _channelMock.Setup(c => c.CreateBasicProperties())
                .Returns(new Mock<IBasicProperties>().Object);

            // Act
            await sut.PublishAsync(@event, CancellationToken.None);

            // Assert
            _eventWrapperMock.Verify(ew => ew.WrapAsync(It.Is<Event>(e => e.Equals(@event)), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact(DisplayName = "RabbitMQEventSender - PublishAsync - Set headers")]
        public async Task RabbitMQEventSender_PublishAsync_SetHeaders()
        {
            // Arrange
            var sut = CreateSUT();
            var @event = new Event(Guid.NewGuid());
            var propertiesMock = new Mock<IBasicProperties>();
            var message = new RabbitMQMessage()
            {
                Headers = new Dictionary<string, object> { { EventDefaults.MessageType, @event.GetMessageType() } }
            };

            _eventWrapperMock.Setup(ew => ew.WrapAsync(It.IsAny<IEvent>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(message);

            _channelMock.Setup(c => c.CreateBasicProperties())
                .Returns(propertiesMock.Object);

            // Act
            await sut.PublishAsync(@event, CancellationToken.None);

            // Assert
            propertiesMock.VerifySet(p => p.Headers = message.Headers, Times.Once);
        }

        [Fact(DisplayName = "RabbitMQEventSender - PublishAsync - PublishEvent")]
        public async Task RabbitMQEventSender_PublishAsync_PublishEvent()
        {
            // Arrange
            var sut = CreateSUT();
            var routingKey = new Fixture().Create<string>();
            var @event = new Event(Guid.NewGuid());
            var propertiesMock = new Mock<IBasicProperties>();
            var message = new RabbitMQMessage()
            {
                Headers = new Dictionary<string, object>(),
                Body = @event.Encode(new JsonSerializerOptions())
            };

            _eventWrapperMock.Setup(ew => ew.WrapAsync(It.IsAny<IEvent>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(message);

            _channelMock.Setup(c => c.CreateBasicProperties())
                .Returns(propertiesMock.Object);

            // Act
            await sut.PublishAsync(@event, routingKey, CancellationToken.None);

            // Assert
            _channelMock.Verify(c => c.BasicPublish(It.Is<string>(e => e.Equals(_options.Exchange)), It.Is<string>(rk => rk.Equals(routingKey)), It.Is<bool>(m => m.Equals(_options.Mandatory)), It.IsAny<IBasicProperties>(), It.Is<ReadOnlyMemory<byte>>(b => b.Equals(message.Body))), Times.Once);
        }

        [Fact(DisplayName = "RabbitMQEventSender - PublishAsync - PublishEvent without routing key")]
        public async Task RabbitMQEventSender_PublishAsync_PublishEventWithoutRoutingKey()
        {
            // Arrange
            var sut = CreateSUT();
            var @event = new Event(Guid.NewGuid());
            var propertiesMock = new Mock<IBasicProperties>();
            var message = new RabbitMQMessage()
            {
                Headers = new Dictionary<string, object>(),
                Body = @event.Encode(new JsonSerializerOptions())
            };

            _eventWrapperMock.Setup(ew => ew.WrapAsync(It.IsAny<IEvent>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(message);

            _channelMock.Setup(c => c.CreateBasicProperties())
                .Returns(propertiesMock.Object);

            // Act
            await sut.PublishAsync(@event, CancellationToken.None);

            // Assert
            _channelMock.Verify(c => c.BasicPublish(It.Is<string>(e => e.Equals(_options.Exchange)), It.Is<string>(rk => rk.Equals(_options.RoutingKey)), It.Is<bool>(m => m.Equals(_options.Mandatory)), It.IsAny<IBasicProperties>(), It.Is<ReadOnlyMemory<byte>>(b => b.Equals(message.Body))), Times.Once);
        }

        [Fact(DisplayName = "RabbitMQEventSender - PublishAsync - Event is null")]
        public async Task RabbitMQEventSender_PublishAsync_EventIsNull()
        {
            // Arrange
            var sut = CreateSUT();

            // Act
            // Assert
            await Assert.ThrowsAsync<ArgumentNullException>(async () => await sut.PublishAsync<Event>(null, CancellationToken.None));
        }

        [Fact(DisplayName = "RabbitMQEventSender - PublishAsync with RoutingKey - Event is null")]
        public async Task RabbitMQEventSender_PublishAsyncWithRoutingKey_EventIsNull()
        {
            // Arrange
            var sut = CreateSUT();

            // Act
            // Assert
            await Assert.ThrowsAsync<ArgumentNullException>(async () => await sut.PublishAsync<Event>(null, new Fixture().Create<string>(), CancellationToken.None));
        }
    }
}
