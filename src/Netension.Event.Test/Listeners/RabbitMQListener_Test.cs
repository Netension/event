using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Netension.Event.Hosting.LightInject.RabbitMQ.Listeners;
using Netension.Event.Hosting.LightInject.RabbitMQ.Options;
using Netension.Event.RabbitMQ.Receivers;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Netension.Event.Test.Listeners
{
    public class RabbitMQListener_Test
    {
        private readonly ILogger<RabbitMQListener> _logger;
        private Mock<IConnection> _connectionMock;
        private Mock<IOptions<RabbitMQListenerOptions>> _listenerOptionsMock;
        private Mock<IRabbitMQEventReceiver> _eventReceiverMock;

        public RabbitMQListener_Test(ITestOutputHelper outputHelper)
        {
            _logger = new LoggerFactory()
                        .AddXUnit(outputHelper)
                        .CreateLogger<RabbitMQListener>();
        }

        private RabbitMQListener CreateSUT()
        {
            _connectionMock = new Mock<IConnection>();
            _listenerOptionsMock = new Mock<IOptions<RabbitMQListenerOptions>>();
            _eventReceiverMock = new Mock<IRabbitMQEventReceiver>();

            return new RabbitMQListener(_connectionMock.Object, _listenerOptionsMock.Object, _eventReceiverMock.Object, _logger);
        }

        [Fact(DisplayName = "RabbitMQListener - StartAsync - Create channel")]
        public async Task RabbitMQListener_StartAsync_CreateChannel()
        {
            // Arrange
            var sut = CreateSUT();

            _connectionMock.Setup(c => c.CreateModel())
                .Returns(new Mock<IModel>().Object);

            _listenerOptionsMock.Setup(lo => lo.Value)
                .Returns(new RabbitMQListenerOptions());

            // Act
            await sut.StartAsync(CancellationToken.None);

            // Assert
            _connectionMock.Verify(c => c.CreateModel(), Times.Once);
        }

        [Fact(DisplayName = "RabbitMQListener - StartAsync - Consume")]
        public async Task RabbitMQListener_StartAsync_Consume()
        {
            // Arrange
            var sut = CreateSUT();
            var channelMock = new Mock<IModel>();
            var options = new RabbitMQListenerOptions
            {
                Exchange = "TestExchange",
                Queue = "TestQueue",
                Tag = "TestTag"
            };

            _connectionMock.Setup(c => c.CreateModel())
                .Returns(channelMock.Object);

            _listenerOptionsMock.Setup(lo => lo.Value)
                .Returns(options);

            // Act
            await sut.StartAsync(CancellationToken.None);

            // Assert
            channelMock.Verify(c => c.BasicConsume(It.Is<string>(q => q.Equals(options.Queue)), It.Is<bool>(p => !p), It.Is<string>(t => t.Equals(options.Tag)), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<IDictionary<string, object>>(), It.IsAny<IBasicConsumer>()), Times.Once);
        }
    }
}
