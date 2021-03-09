using Microsoft.Extensions.Logging;
using Moq;
using Netension.Event.Abstraction;
using Netension.Event.RabbitMQ.Unwrappers;
using Netension.Event.Test.Extensions;
using Netension.Extensions.Correlation;
using Netension.Extensions.Correlation.Defaults;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Netension.Event.Test.Unwrappers
{
    public class RabbitMQCorrelationUnwrapper_Test
    {
        private readonly ILogger<RabbitMQCorrelationUnwrapper> _logger;
        private Mock<ICorrelationMutator> _correlationMutatorMock;
        private Mock<IRabbitMQEventUnwrapper> _nextMock;

        public RabbitMQCorrelationUnwrapper_Test(ITestOutputHelper outputHelper)
        {
            _logger = new LoggerFactory()
                        .AddXUnit(outputHelper)
                        .CreateLogger<RabbitMQCorrelationUnwrapper>();
        }

        private RabbitMQCorrelationUnwrapper CreateSUT()
        {
            _correlationMutatorMock = new Mock<ICorrelationMutator>();
            _nextMock = new Mock<IRabbitMQEventUnwrapper>();

            return new RabbitMQCorrelationUnwrapper(_correlationMutatorMock.Object, _nextMock.Object, _logger);
        }

        [Fact(DisplayName = "RabbitMQCorrelationUnwrapper - UnwrapAsync - Call next")]
        public async Task RabbitMQCorrelationUnwrapper_UnwrapAsync_CallNext()
        {
            // Arrange
            var sut = CreateSUT();
            var basicPropertiesMock = new Mock<IBasicProperties>();
            var eventArgs = new BasicDeliverEventArgs(null, 0, false, null, null, basicPropertiesMock.Object, null);

            basicPropertiesMock.Setup(bp => bp.Headers)
                .Returns(new Dictionary<string, object> {
                    { CorrelationDefaults.CorrelationId, Encoding.UTF8.GetBytes(Guid.NewGuid().ToString()) }
                });

            _nextMock.Setup(n => n.UnwrapAsync(It.IsAny<BasicDeliverEventArgs>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Event());

            // Act
            await sut.UnwrapAsync(eventArgs, CancellationToken.None);

            // Assert
            _nextMock.Verify(n => n.UnwrapAsync(It.Is<BasicDeliverEventArgs>(ea => ea.Equals(eventArgs)), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact(DisplayName = "RabbitMQCorrelationUnwrapper - UnwrapAsync - Unwrap CorrelationId")]
        public async Task RabbitMQCorrelationUnwrapper_UnwrapAsync_UnwraptCorrelationId()
        {
            // Arrange
            var sut = CreateSUT();
            var correlationId = Guid.NewGuid();
            var basicPropertiesMock = new Mock<IBasicProperties>();

            basicPropertiesMock.Setup(bp => bp.Headers)
                .Returns(new Dictionary<string, object> {
                    { CorrelationDefaults.CorrelationId, Encoding.UTF8.GetBytes(correlationId.ToString()) }
                });

            _nextMock.Setup(n => n.UnwrapAsync(It.IsAny<BasicDeliverEventArgs>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Event());

            // Act
            await sut.UnwrapAsync(new BasicDeliverEventArgs(null, 0, false, null, null, basicPropertiesMock.Object, null), CancellationToken.None);

            // Assert
            _correlationMutatorMock.VerifySet(cm => cm.CorrelationId = correlationId, Times.Once);
        }

        [Fact(DisplayName = "RabbitMQCorrelationUnwrapper - UnwrapAsync - CorrelationId not present")]
        public async Task RabbitMQCorrelationUnwrapper_UnwrapAsync_CorrelationIdNotPresent()
        {
            // Arrange
            var sut = CreateSUT();
            var basicPropertiesMock = new Mock<IBasicProperties>();

            basicPropertiesMock.Setup(bp => bp.Headers)
                .Returns(new Dictionary<string, object>());

            _nextMock.Setup(n => n.UnwrapAsync(It.IsAny<BasicDeliverEventArgs>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Event());

            // Act
            // Assert
            await Assert.ThrowsAsync<InvalidOperationException>(async () => await sut.UnwrapAsync(new BasicDeliverEventArgs(null, 0, false, null, null, basicPropertiesMock.Object, null), CancellationToken.None));
        }

        [Fact(DisplayName = "RabbitMQCorrelationUnwrapper - UnwrapAsync - Unwrap CausationId")]
        public async Task RabbitMQCorrelationUnwrapper_UnwrapAsync_UnwrapCausationId()
        {
            // Arrange
            var sut = CreateSUT();
            var causationId = Guid.NewGuid();
            var basicPropertiesMock = new Mock<IBasicProperties>();

            basicPropertiesMock.Setup(bp => bp.Headers)
                .Returns(new Dictionary<string, object>
                {
                    { CorrelationDefaults.CorrelationId, Encoding.UTF8.GetBytes(Guid.NewGuid().ToString()) },
                    { CorrelationDefaults.CausationId, Encoding.UTF8.GetBytes(causationId.ToString()) }
                });

            _nextMock.Setup(n => n.UnwrapAsync(It.IsAny<BasicDeliverEventArgs>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Event());

            // Act
            await sut.UnwrapAsync(new BasicDeliverEventArgs(null, 0, false, null, null, basicPropertiesMock.Object, null), CancellationToken.None);

            // Assert
            _correlationMutatorMock.VerifySet(cm => cm.CausationId = causationId, Times.Once);
        }

        [Fact(DisplayName = "RabbitMQCorrelationUnwrapper - UnwrapAsync - CausationId not present")]
        public async Task RabbitMQCorrelationUnwrapper_UnwrapAsync_CausationIdNotPresent()
        {
            // Arrange
            var sut = CreateSUT();
            var basicPropertiesMock = new Mock<IBasicProperties>();

            basicPropertiesMock.Setup(bp => bp.Headers)
                .Returns(new Dictionary<string, object>
                {
                    { CorrelationDefaults.CorrelationId, Encoding.UTF8.GetBytes(Guid.NewGuid().ToString()) }
                });

            _nextMock.Setup(n => n.UnwrapAsync(It.IsAny<BasicDeliverEventArgs>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Event());

            // Act
            await sut.UnwrapAsync(new BasicDeliverEventArgs(null, 0, false, null, null, basicPropertiesMock.Object, null), CancellationToken.None);

            // Assert
            _correlationMutatorMock.VerifySet(cm => cm.CausationId = null, Times.Once);
        }

        [Fact(DisplayName = "RabbitMQCorrelationUnwrapper - UnwrapAsync - CausationId is null")]
        public async Task RabbitMQCorrelationUnwrapper_UnwrapAsync_CausationIdIsNull()
        {
            // Arrange
            var sut = CreateSUT();
            var basicPropertiesMock = new Mock<IBasicProperties>();

            basicPropertiesMock.Setup(bp => bp.Headers)
                .Returns(new Dictionary<string, object>
                {
                    { CorrelationDefaults.CorrelationId, Encoding.UTF8.GetBytes(Guid.NewGuid().ToString()) },
                    { CorrelationDefaults.CausationId, null }
                });

            _nextMock.Setup(n => n.UnwrapAsync(It.IsAny<BasicDeliverEventArgs>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Event());

            // Act
            await sut.UnwrapAsync(new BasicDeliverEventArgs(null, 0, false, null, null, basicPropertiesMock.Object, null), CancellationToken.None);

            // Assert
            _correlationMutatorMock.VerifySet(cm => cm.CausationId = null, Times.Once);
        }

        [Fact(DisplayName = "RabbitMQCorrelationUnwrapper - UnwrapAsync - CausationId is empty")]
        public async Task RabbitMQCorrelationUnwrapper_UnwrapAsync_CausationIdIsEmpty()
        {
            // Arrange
            var sut = CreateSUT();
            var basicPropertiesMock = new Mock<IBasicProperties>();

            basicPropertiesMock.Setup(bp => bp.Headers)
                .Returns(new Dictionary<string, object>
                {
                    { CorrelationDefaults.CorrelationId, Encoding.UTF8.GetBytes(Guid.NewGuid().ToString()) },
                    { CorrelationDefaults.CausationId, new byte[0] }
                });

            _nextMock.Setup(n => n.UnwrapAsync(It.IsAny<BasicDeliverEventArgs>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Event());

            // Act
            await sut.UnwrapAsync(new BasicDeliverEventArgs(null, 0, false, null, null, basicPropertiesMock.Object, null), CancellationToken.None);

            // Assert
            _correlationMutatorMock.VerifySet(cm => cm.CausationId = null, Times.Once);
        }

        [Fact(DisplayName = "RabbitMQCorrelationUnwrapper - UnwrapAsync - Unwrap MessageId")]
        public async Task RabbitMQCorrelationUnwrapper_UnwrapAsync_UnwrapMessageId()
        {
            // Arrange
            var sut = CreateSUT();
            var basicPropertiesMock = new Mock<IBasicProperties>();
            var @event = new Event();

            basicPropertiesMock.Setup(bp => bp.Headers)
                .Returns(new Dictionary<string, object>
                {
                    { CorrelationDefaults.CorrelationId, Encoding.UTF8.GetBytes(Guid.NewGuid().ToString()) }
                });

            _nextMock.Setup(n => n.UnwrapAsync(It.IsAny<BasicDeliverEventArgs>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(@event);

            // Act
            await sut.UnwrapAsync(new BasicDeliverEventArgs(null, 0, false, null, null, basicPropertiesMock.Object, null), CancellationToken.None);

            // Assert
            _correlationMutatorMock.VerifySet(cm => cm.MessageId = @event.EventId, Times.Once);
        }
    }
}
