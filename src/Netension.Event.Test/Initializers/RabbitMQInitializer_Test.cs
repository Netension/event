using AutoFixture;
using Microsoft.Extensions.Logging;
using Moq;
using Netension.Event.RabbitMQ.Initializers;
using Netension.Event.RabbitMQ.Options;
using RabbitMQ.Client;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Netension.Event.Test.Initializers
{
    public class RabbitMQInitializer_Test
    {
        private readonly ILogger<RabbitMQInitializer> _logger;

        public RabbitMQInitializer_Test(ITestOutputHelper outputHelper)
        {
            _logger = new LoggerFactory()
                        .AddXUnit(outputHelper)
                        .CreateLogger<RabbitMQInitializer>();
        }

        private RabbitMQInitializer CreateSUT()
        {
            return new RabbitMQInitializer(_logger);
        }

        [Fact(DisplayName = "RabbitMQInitializer - ListenAsync - Initialize queue")]
        public async Task RabbitMQInitializer_InitializeAsync_CreateChannel()
        {
            // Arrange
            var sut = CreateSUT();
            var _channelMock = new Mock<IModel>();
            var options = new Fixture()
                            .Build<RabbitMQListenerOptions>()
                                .With(o => o.Bindings, Enumerable.Empty<BindingOptions>())
                            .Create();

            // Act
            await sut.InitializeAsync(_channelMock.Object, options, CancellationToken.None);

            // Assert
            _channelMock.Verify(c => c.QueueDeclare(It.Is<string>(q => q.Equals(options.Queue.Name)), It.Is<bool>(d => d.Equals(options.Queue.Durable)), It.Is<bool>(e => e.Equals(options.Queue.Exclusive)), It.Is<bool>(ad => ad.Equals(options.Queue.AutoDelete)), It.Is<IDictionary<string, object>>(a => a.Equals(options.Queue.Arguments))), Times.Once);
        }

        [Fact(DisplayName = "RabbitMQInitializer - ListenAsync - Bind to exchange")]
        public async Task RabbitMQInitializer_InitializeAsync_BindToExchange()
        {
            // Arrange
            var sut = CreateSUT();
            var _channelMock = new Mock<IModel>();
            var bindOptions = new Fixture().Create<BindingOptions>();
            var options = new Fixture()
                                .Build<RabbitMQListenerOptions>()
                                    .With(o => o.Bindings, new List<BindingOptions> { bindOptions, bindOptions })
                                .Create();

            // Act
            await sut.InitializeAsync(_channelMock.Object, options, CancellationToken.None);

            // Assert
            _channelMock.Verify(c => c.QueueBind(It.Is<string>(q => q.Equals(options.Queue.Name)), It.Is<string>(e => e.Equals(bindOptions.Exchange)), It.Is<string>(rk => rk.Equals(bindOptions.RoutingKey)), It.Is<IDictionary<string, object>>(a => a.Equals(bindOptions.Arguments))), Times.Exactly(2));
        }
    }
}
