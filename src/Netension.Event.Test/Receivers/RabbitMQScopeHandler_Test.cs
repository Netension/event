using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Netension.Event.RabbitMQ.Receivers;
using RabbitMQ.Client.Events;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Netension.Event.Test.Receivers
{
    public class RabbitMQScopeHandler_Test
    {
        private readonly ILogger<RabbitMQScopeHandler> _logger;
        private Mock<IServiceScopeFactory> _serviceScopeFactoryMock;
        private Mock<IRabbitMQEventReceiver> _rabbitMQEventReceiverMock;

        public RabbitMQScopeHandler_Test(ITestOutputHelper outputHelper)
        {
            _logger = new LoggerFactory()
                        .AddXUnit(outputHelper)
                        .CreateLogger<RabbitMQScopeHandler>();
        }

        private RabbitMQScopeHandler CreateSUT()
        {
            _serviceScopeFactoryMock = new Mock<IServiceScopeFactory>();
            _rabbitMQEventReceiverMock = new Mock<IRabbitMQEventReceiver>();

            return new RabbitMQScopeHandler(_serviceScopeFactoryMock.Object, _rabbitMQEventReceiverMock.Object, _logger);
        }

        [Fact(DisplayName = "RabbitMQScopeHandler - HandleAsync - Create scope")]
        public async Task RabbitMQScopeHandler_HandleAsync_CreateScope()
        {
            // Arrange
            var sut = CreateSUT();

            // Act
            await sut.ReceiveAsync(null, CancellationToken.None);

            // Assert
            _serviceScopeFactoryMock.Verify(ssf => ssf.CreateScope(), Times.Once);
        }

        [Fact(DisplayName = "RabbitMQScopeHandler - HandleAsync - Receive next")]
        public async Task RabbitMQScopeHandler_HandleAsync_ReceiveNext()
        {
            // Arrange
            var sut = CreateSUT();
            var deliveryEventArgs = new BasicDeliverEventArgs();

            // Act
            await sut.ReceiveAsync(deliveryEventArgs, CancellationToken.None);

            // Assert
            _rabbitMQEventReceiverMock.Verify(rer => rer.ReceiveAsync(It.Is<BasicDeliverEventArgs>(bdea =>bdea.Equals(deliveryEventArgs)), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
