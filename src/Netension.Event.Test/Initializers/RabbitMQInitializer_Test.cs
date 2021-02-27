using Microsoft.Extensions.Logging;
using Moq;
using Netension.Event.RabbitMQ.Initializers;
using Netension.Event.RabbitMQ.Options;
using RabbitMQ.Client;
using System.Collections.Generic;
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

        [Fact(DisplayName = "RabbitMQInitializer - InitializeAsync - Create channel")]
        public async Task RabbitMQInitializer_InitializeAsync_CreateChannel()
        {
            // Arrange
            var sut = CreateSUT();
            var _channelMock = new Mock<IModel>();
            var options = new RabbitMQListenerOptions
            {
                Queue = new QueueOptions { 
                    Name = "test_queue"
                },
                Bindings = new List<BindingOptions>
                {
                    new BindingOptions{ Exchange = "TestExchange" }
                }
            };

            // Act
            await sut.InitializeAsync(_channelMock.Object, options, CancellationToken.None);

            // Assert
            _channelMock.Verify(c => c.QueueDeclare(It.Is<string>(q => q.Equals(options.Queue.Name)), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>(), new Dictionary<string, object>()));
        }
    }
}
