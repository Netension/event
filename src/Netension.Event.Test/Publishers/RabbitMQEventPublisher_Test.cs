//using AutoFixture;
//using Microsoft.Extensions.Logging;
//using Microsoft.Extensions.Options;
//using Moq;
//using Netension.Event.Abstraction;
//using Netension.Event.Defaults;
//using Netension.Event.Extensions;
//using Netension.Event.RabbitMQ.Messages;
//using Netension.Event.RabbitMQ.Options;
//using Netension.Event.RabbitMQ.Senders;
//using Netension.Event.RabbitMQ.Wrappers;
//using Netension.Event.Test.Extensions;
//using RabbitMQ.Client;
//using System;
//using System.Collections.Generic;
//using System.Text.Json;
//using System.Threading;
//using System.Threading.Tasks;
//using Xunit;
//using Xunit.Abstractions;

//namespace Netension.Event.Test.Publishers
//{
//    public class RabbitMQEventPublisher_Test
//    {
//        private readonly ILogger<RabbitMQEventPublisher> _logger;
//        private Mock<IRabbitMQEventWrapper> _eventWrapperMock;
//        private Mock<IModel> _channelMock;
//        private Mock<IOptions<RabbitMQPublisherOptions>> _optionsMock;

//        public RabbitMQEventPublisher_Test(ITestOutputHelper outputHelper)
//        {
//            _logger = new LoggerFactory()
//                        .AddXUnit(outputHelper)
//                        .CreateLogger<RabbitMQEventPublisher>();
//        }

//        private RabbitMQEventPublisher CreateSUT()
//        {
//            _eventWrapperMock = new Mock<IRabbitMQEventWrapper>();
//            _channelMock = new Mock<IModel>();
//            _optionsMock = new Mock<IOptions<RabbitMQPublisherOptions>>();

//            return new RabbitMQEventPublisher(_channelMock.Object, _eventWrapperMock.Object, _optionsMock.Object, _logger);
//        }

//        [Fact(DisplayName = "RabbitMQEventSender - SendAsync - Wrap event")]
//        public async Task RabbitMQEventSender_SendAsync_WrapEvent()
//        {
//            // Arrange
//            var sut = CreateSUT();
//            var @event = new Event(Guid.NewGuid());

//            _eventWrapperMock.Setup(ew => ew.WrapAsync(It.IsAny<IEvent>(), It.IsAny<CancellationToken>()))
//                .ReturnsAsync(new RabbitMQMessage());

//            _optionsMock.SetupGet(o => o.Value)
//                .Returns(new RabbitMQPublisherOptions());

//            _channelMock.Setup(c => c.CreateBasicProperties())
//                .Returns(new Mock<IBasicProperties>().Object);

//            // Act
//            await sut.PublishAsync(@event, CancellationToken.None);

//            // Assert
//            _eventWrapperMock.Verify(ew => ew.WrapAsync(It.Is<Event>(e => e.Equals(@event)), It.IsAny<CancellationToken>()), Times.Once);
//        }

//        [Fact(DisplayName = "RabbitMQEventSender - SendAsync - Set headers")]
//        public async Task RabbitMQEventSender_SendAsync_SetHeaders()
//        {
//            // Arrange
//            var sut = CreateSUT();
//            var @event = new Event(Guid.NewGuid());
//            var propertiesMock = new Mock<IBasicProperties>();
//            var message = new RabbitMQMessage()
//            {
//                Headers = new Dictionary<string, object> { { EventDefaults.MessageType, @event.GetMessageType() } }
//            };

//            _eventWrapperMock.Setup(ew => ew.WrapAsync(It.IsAny<IEvent>(), It.IsAny<CancellationToken>()))
//                .ReturnsAsync(message);

//            _channelMock.Setup(c => c.CreateBasicProperties())
//                .Returns(propertiesMock.Object);

//            _optionsMock.SetupGet(o => o.Value)
//                .Returns(new Fixture().Create<RabbitMQPublisherOptions>());

//            // Act
//            await sut.PublishAsync(@event, null, CancellationToken.None);

//            // Assert
//            propertiesMock.VerifySet(p => p.Headers = message.Headers, Times.Once);
//        }

//        [Fact(DisplayName = "RabbitMQEventSender - SendAsync - PublishEvent")]
//        public async Task RabbitMQEventSender_SendAsync_PublishEvent()
//        {
//            // Arrange
//            var sut = CreateSUT();
//            var routingKey = new Fixture().Create<string>();
//            var @event = new Event(Guid.NewGuid());
//            var propertiesMock = new Mock<IBasicProperties>();
//            var message = new RabbitMQMessage()
//            {
//                Headers = new Dictionary<string, object>(),
//                Body = @event.Encode(new JsonSerializerOptions())
//            };
//            var options = new Fixture().Create<RabbitMQPublisherOptions>();

//            _eventWrapperMock.Setup(ew => ew.WrapAsync(It.IsAny<IEvent>(), It.IsAny<CancellationToken>()))
//                .ReturnsAsync(message);

//            _channelMock.Setup(c => c.CreateBasicProperties())
//                .Returns(propertiesMock.Object);

//            _optionsMock.SetupGet(o => o.Value)
//                .Returns(options);

//            // Act
//            await sut.PublishAsync(@event, routingKey, CancellationToken.None);

//            // Assert
//            _channelMock.Verify(c => c.BasicPublish(It.Is<string>(e => e.Equals(options.Exchange)), It.Is<string>(rk => rk.Equals(routingKey)), It.Is<bool>(m => m.Equals(options.Mandatory)), It.IsAny<IBasicProperties>(), It.Is<ReadOnlyMemory<byte>>(b => b.Equals(message.Body))), Times.Once);
//        }

//        [Fact(DisplayName = "RabbitMQEventSender - SendAsync - PublishEvent without routing key")]
//        public async Task RabbitMQEventSender_SendAsync_PublishEventWithoutRoutingKey()
//        {
//            // Arrange
//            var sut = CreateSUT();
//            var @event = new Event(Guid.NewGuid());
//            var propertiesMock = new Mock<IBasicProperties>();
//            var message = new RabbitMQMessage()
//            {
//                Headers = new Dictionary<string, object>(),
//                Body = @event.Encode(new JsonSerializerOptions())
//            };
//            var options = new Fixture().Create<RabbitMQPublisherOptions>();

//            _eventWrapperMock.Setup(ew => ew.WrapAsync(It.IsAny<IEvent>(), It.IsAny<CancellationToken>()))
//                .ReturnsAsync(message);

//            _channelMock.Setup(c => c.CreateBasicProperties())
//                .Returns(propertiesMock.Object);

//            _optionsMock.SetupGet(o => o.Value)
//                .Returns(options);

//            // Act
//            await sut.PublishAsync(@event, CancellationToken.None);

//            // Assert
//            _channelMock.Verify(c => c.BasicPublish(It.Is<string>(e => e.Equals(options.Exchange)), It.Is<string>(rk => rk.Equals(options.RoutingKey)), It.Is<bool>(m => m.Equals(options.Mandatory)), It.IsAny<IBasicProperties>(), It.Is<ReadOnlyMemory<byte>>(b => b.Equals(message.Body))), Times.Once);
//        }
//    }
//}
