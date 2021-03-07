using AutoFixture;
using Microsoft.Extensions.Logging;
using Moq;
using Netension.Event.RabbitMQ.Initializers;
using Netension.Event.RabbitMQ.Listeners;
using Netension.Event.RabbitMQ.Options;
using Netension.Event.RabbitMQ.Receivers;
using RabbitMQ.Client;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;
using Netension.Event.RabbitMQ.Extensions;
using System.Collections.Generic;
using RabbitMQ.Client.Events;

namespace Netension.Event.Test.Listeners
{
    public class RabbitMQEventListener_Test
    {
        private readonly ILogger<RabbitMQEventListener> _logger;
        private Mock<IModel> _channelMock;
        private RabbitMQListenerOptions _options;
        private Mock<IRabbitMQEventReceiver> _rabbitMQReceiverMock;
        private Mock<IRabbitMQInitializer> _rabbitMQInitializerMock;

        public RabbitMQEventListener_Test(ITestOutputHelper outputHelper)
        {
            _logger = new LoggerFactory()
                        .AddXUnit(outputHelper)
                        .CreateLogger<RabbitMQEventListener>();
        }

        private RabbitMQEventListener CreateSUT()
        {
            _channelMock = new Mock<IModel>();
            _options = new Fixture().Create<RabbitMQListenerOptions>();
            _rabbitMQReceiverMock = new Mock<IRabbitMQEventReceiver>();
            _rabbitMQInitializerMock = new Mock<IRabbitMQInitializer>();

            return new RabbitMQEventListener(_channelMock.Object, _options, _rabbitMQReceiverMock.Object, _rabbitMQInitializerMock.Object, _logger);
        }

        [Fact(DisplayName = "RabbitMQEventListener - ListenAsync - Initialize queue")]
        public async Task RabbitMQEventListener_ListenAsync_InitializeQueue()
        {
            // Arrange
            var sut = CreateSUT();

            // Act
            await sut.ListenAsync(CancellationToken.None);

            // Assert
            _rabbitMQInitializerMock.Verify(ri => ri.InitializeAsync(It.Is<IModel>(c => c.Equals(_channelMock.Object)), It.Is<RabbitMQListenerOptions>(rmlo => rmlo.Equals(_options)), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact(DisplayName = "RabbitMQEventListener - ListenAsync - Consume")]
        public async Task RabbitMQEventListener_ListenAsync_Consume()
        {
            // Arrange
            var sut = CreateSUT();

            // Act
            await sut.ListenAsync(CancellationToken.None);

            // Assert
            _channelMock.Verify(c => c.BasicConsume(It.Is<string>(q => q.Equals(_options.Queue.Name)), It.Is<bool>(aa => aa.Equals(_options.Queue.AutoAck)), It.Is<string>(ct => ct.StartsWith(_options.Queue.ConsumerPrefix)), It.Is<bool>(nl => nl.Equals(_options.Queue.NoLocal)), It.Is<bool>(e => e.Equals(_options.Queue.Exclusive)), It.Is<IDictionary<string, object>>(a => a.Equals(_options.Queue.Arguments)), It.IsNotNull<AsyncEventingBasicConsumer>()), Times.Once);
        }

        [Fact(DisplayName = "RabbitMQEventListener - StopAsync - Close channel")]
        public async Task RabbitMQEventListener_StopAsync_CloseChannel()
        {
            // Arrange
            var sut = CreateSUT();

            // Act
            await sut.StopAsync(CancellationToken.None);

            // Assert
            _channelMock.Verify(c => c.Close(), Times.Once);
        }
    }
}
