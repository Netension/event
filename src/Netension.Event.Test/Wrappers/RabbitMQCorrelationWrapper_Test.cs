using Microsoft.Extensions.Logging;
using Moq;
using Netension.Event.RabbitMQ.Messages;
using Netension.Event.RabbitMQ.Wrappers;
using Netension.Extensions.Correlation;
using Netension.Extensions.Correlation.Defaults;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Netension.Event.Test.Wrappers
{
    public class RabbitMQCorrelationWrapper_Test
    {
        private readonly ILogger<RabbitMQCorrelationWrapper> _logger;
        private Mock<ICorrelationAccessor> _correlationAccessorMock;
        private Mock<IRabbitMQEventWrapper> _nextMock;

        public RabbitMQCorrelationWrapper_Test(ITestOutputHelper outputHelper)
        {
            _logger = new LoggerFactory()
                        .AddXUnit(outputHelper)
                        .CreateLogger<RabbitMQCorrelationWrapper>();
        }

        private RabbitMQCorrelationWrapper CreateSUT()
        {
            _correlationAccessorMock = new Mock<ICorrelationAccessor>();
            _nextMock = new Mock<IRabbitMQEventWrapper>();

            return new RabbitMQCorrelationWrapper(_correlationAccessorMock.Object, _nextMock.Object, _logger);
        }

        [Fact(DisplayName = "RabbitMQCorrelationWrapper - WrapAsync - Call next")]
        public async Task RabbitMQCorrelationWrapper_WrapAsync_CallNext()
        {
            // Arrange
            var sut = CreateSUT();
            var correlationId = Guid.NewGuid();
            var @event = new Event();

            _correlationAccessorMock.Setup(ca => ca.CorrelationId)
                .Returns(Guid.NewGuid());
            _nextMock.Setup(n => n.WrapAsync(It.IsAny<Event>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new RabbitMQMessage());

            // Act
            await sut.WrapAsync(@event, CancellationToken.None);

            // Assert
            _nextMock.Verify(n => n.WrapAsync(It.Is<Event>(e => e.Equals(@event)), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact(DisplayName = "RabbitMQCorrelationWrapper - WrapAsync - Set CorrelationId")]
        public async Task RabbitMQCorrelationWrapper_WrapAsync_SetCorrelationId()
        {
            // Arrange
            var sut = CreateSUT();
            var correlationId = Guid.NewGuid();

            _correlationAccessorMock.Setup(ca => ca.CorrelationId)
                .Returns(correlationId);

            _nextMock.Setup(n => n.WrapAsync(It.IsAny<Event>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new RabbitMQMessage());

            // Act
            var result = await sut.WrapAsync(new Event(), CancellationToken.None);

            // Assert
            Assert.Equal(correlationId, Guid.Parse(result.Headers[CorrelationDefaults.CorrelationId].ToString().AsSpan()));
        }

        [Fact(DisplayName = "RabbitMQCorrelationWrapper - WrapAsync - Null CorrelationId")]
        public async Task RabbitMQCorrelationWrapper_WrapAsync_NullCorrelationId()
        {
            // Arrange
            var sut = CreateSUT();

            _nextMock.Setup(n => n.WrapAsync(It.IsAny<Event>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new RabbitMQMessage());

            // Act
            // Assert
            await Assert.ThrowsAsync<InvalidOperationException>(async () => await sut.WrapAsync(new Event(), CancellationToken.None));
        }

        [Fact(DisplayName = "RabbitMQCorrelationWrapper - WrapAsync - Set CausationId")]
        public async Task RabbitMQCorrelationWrapper_WrapAsync_SetCausationId()
        {
            // Arrange
            var sut = CreateSUT();
            var messageId = Guid.NewGuid();

            _correlationAccessorMock.Setup(ca => ca.CorrelationId)
                .Returns(Guid.NewGuid());
            _correlationAccessorMock.Setup(ca => ca.MessageId)
                .Returns(messageId);

            _nextMock.Setup(n => n.WrapAsync(It.IsAny<Event>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new RabbitMQMessage());

            // Act
            var result = await sut.WrapAsync(new Event(), CancellationToken.None);

            // Assert
            Assert.Equal(messageId, Guid.Parse(result.Headers[CorrelationDefaults.CausationId].ToString().AsSpan()));
        }
    }
}
