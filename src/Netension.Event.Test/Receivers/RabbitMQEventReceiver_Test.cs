using Microsoft.Extensions.Logging;
using Moq;
using Netension.Event.Abstraction;
using Netension.Event.Defaults;
using Netension.Event.Extensions;
using Netension.Event.RabbitMQ.Receivers;
using Netension.Event.RabbitMQ.Unwrappers;
using Netension.Event.Test.Extensions;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Netension.Event.Test.Receivers
{
    public class RabbitMQEventReceiver_Test
    {
        private readonly ILogger<RabbitMQEventReceiver> _logger;
        private Mock<IRabbitMQEventUnwrapper> _unwrapperMock;
        private Mock<IEventDispatcher> _eventDispatherMock;

        public RabbitMQEventReceiver_Test(ITestOutputHelper outputHelper)
        {
            _logger = new LoggerFactory()
                        .AddXUnit(outputHelper)
                        .CreateLogger<RabbitMQEventReceiver>();
        }

        private RabbitMQEventReceiver CreateSUT()
        {
            _unwrapperMock = new Mock<IRabbitMQEventUnwrapper>();
            _eventDispatherMock = new Mock<IEventDispatcher>();

            return new RabbitMQEventReceiver(_unwrapperMock.Object, _eventDispatherMock.Object, _logger);
        }

        [Fact(DisplayName = "RabbitMQEventReceiver - ReceiveAsync - Unwrap message")]
        public async Task RabbitMQEventReceiver_ReceiveAsync_UnwrapMessage()
        {
            // Arrange
            var sut = CreateSUT();
            var @event = new Event(Guid.NewGuid());
            var basicPropertiesMock = new Mock<IBasicProperties>();
            var basicDeliveryEventArgs = new BasicDeliverEventArgs(null, 0, false, null, null, basicPropertiesMock.Object, @event.Encode());

            basicPropertiesMock.SetupGet(bp => bp.Headers)
                .Returns(new Dictionary<string, object> { { EventDefaults.MessageType, @event.GetMessageType() } });

            _unwrapperMock.Setup(uw => uw.UnwrapAsync(It.IsAny<BasicDeliverEventArgs>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(@event);

            // Act
            await sut.ReceiveAsync(basicDeliveryEventArgs, CancellationToken.None);

            // Assert
            _unwrapperMock.Verify(uw => uw.UnwrapAsync(It.Is<BasicDeliverEventArgs>(bdea => bdea.Equals(basicDeliveryEventArgs)), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact(DisplayName = "RabbitMQEventReceiver - ReceiveAsync - Dispatch message")]
        public async Task RabbitMQEventReceiver_ReceiveAsync_DispatchMessage()
        {
            // Arrange
            var sut = CreateSUT();
            var @event = new Event(Guid.NewGuid());
            var basicPropertiesMock = new Mock<IBasicProperties>();
            var basicDeliveryEventArgs = new BasicDeliverEventArgs(null, 0, false, null, null, basicPropertiesMock.Object, @event.Encode());

            basicPropertiesMock.SetupGet(bp => bp.Headers)
                .Returns(new Dictionary<string, object> { { EventDefaults.MessageType, @event.GetMessageType() } });

            _unwrapperMock.Setup(uw => uw.UnwrapAsync(It.IsAny<BasicDeliverEventArgs>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(@event);

            // Act
            await sut.ReceiveAsync(basicDeliveryEventArgs, CancellationToken.None);

            // Assert
            _eventDispatherMock.Verify(ed => ed.DispatchAsync(It.Is<IEvent>(e => e.Equals(@event)), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
