using Microsoft.Extensions.Logging;
using Moq;
using Netension.Event.Abstraction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Netension.Event.Test.Dispatchers
{
    public class EventDispatcher_Test
    {
        private readonly ILogger<EventDispatcher> _logger;
        private Mock<IServiceProvider> _serviceProviderMock;

        public EventDispatcher_Test(ITestOutputHelper outputHelper)
        {
            _logger = new LoggerFactory()
                        .AddXUnit(outputHelper)
                        .CreateLogger<EventDispatcher>();
        }

        private EventDispatcher CreateSUT()
        {
            _serviceProviderMock = new Mock<IServiceProvider>();

            return new EventDispatcher(_serviceProviderMock.Object, _logger);
        }

        [Fact(DisplayName = "EventDispatcher - DispatchAsync - Get EventHandler")]
        public async Task EventDispatcher_DispatchAsync_GetEventHandler()
        {
            // Arrange
            var sut = CreateSUT();

            // Act
            await sut.DispatchAsync(new Event(Guid.NewGuid()), CancellationToken.None);

            // Assert
            _serviceProviderMock.Verify(sp => sp.GetService(It.Is<Type>(t => t.Equals(typeof(IEnumerable<IEventHandler<Event>>)))), Times.Once);
        }

        [Fact(DisplayName = "EventDispatcher - DispatchAsync - Call handlers")]
        public async Task EventDispatcher_DispatchAsync_CallHandlers()
        {
            // Arrange
            var sut = CreateSUT();
            var @event = new Event(Guid.NewGuid());
            var eventHandlerMock = new Mock<IEventHandler<Event>>();

            _serviceProviderMock.Setup(sp => sp.GetService(It.IsAny<Type>()))
                .Returns(new IEventHandler<Event>[] { eventHandlerMock.Object, eventHandlerMock.Object });

            // Act
            await sut.DispatchAsync(@event, CancellationToken.None);

            // Assert
            eventHandlerMock.Verify(eh => eh.HandleAsync(It.Is<Event>(e => e.Equals(@event)), It.IsAny<CancellationToken>()), Times.Exactly(2));
        }

        [Fact(DisplayName = "EventDispatcher - DispatchAsync - Event null")]
        public async Task EventDispatcher_DispatchAsync_EventNull()
        {
            // Arrange
            var sut = CreateSUT();

            // Act
            // Assert
            await Assert.ThrowsAsync<ArgumentNullException>(async () => await sut.DispatchAsync(null, CancellationToken.None));
        }
    }
}
