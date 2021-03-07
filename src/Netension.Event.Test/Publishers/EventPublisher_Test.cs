using AutoFixture;
using Microsoft.Extensions.Logging;
using Moq;
using Netension.Event.Abstraction;
using Netension.Event.Containers;
using Netension.Event.Publishers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Netension.Event.Test.Publishers
{
    public class EventPublisher_Test
    {
        private Mock<IEventPublisherResolver> _eventPublisherResolverMock;
        private readonly ILogger<EventPublisher> _logger;

        public EventPublisher_Test(ITestOutputHelper outputHelper)
        {
            _logger = new LoggerFactory()
                        .AddXUnit(outputHelper)
                        .CreateLogger<EventPublisher>();
        }

        private EventPublisher CreateSUT()
        {
            _eventPublisherResolverMock = new Mock<IEventPublisherResolver>();

            return new EventPublisher(_eventPublisherResolverMock.Object, _logger);
        }

        [Fact(DisplayName = "EventPublisher - PublishAsync - Event is null")]
        public async Task EventPublisher_PublishAsync_EventIsNull()
        {
            // Arrange
            var sut = CreateSUT();

            // Act
            // Assert
            await Assert.ThrowsAsync<ArgumentNullException>(async () => await sut.PublishAsync<Event>(null, CancellationToken.None));
            await Assert.ThrowsAsync<ArgumentNullException>(async () => await sut.PublishAsync<Event>(null, new Fixture().Create<string>(), CancellationToken.None));
        }

        [Fact(DisplayName = "EventPublisher - PublishAsync - Resolve publishers")]
        public async Task EventPublisher_PublishAsync_ResolvePublishers()
        {
            // Arrange
            var sut = CreateSUT();
            var @event = new Event();

            _eventPublisherResolverMock.Setup(epr => epr.Resolve(It.IsAny<Event>()))
                .Returns(new List<IEventPublisher> { new Mock<IEventPublisher>().Object });

            // Act
            await sut.PublishAsync(@event, CancellationToken.None);

            // Assert
            _eventPublisherResolverMock.Verify(epkr => epkr.Resolve(It.Is<IEvent>(e => e.Equals(@event))), Times.Once);
        }

        [Fact(DisplayName = "EventPublisher - PublishAsync - Publish event")]
        public async Task EventPublisher_PublishAsync_PublishEvent()
        {
            // Arrange
            var sut = CreateSUT();
            var @event = new Event();
            var eventPublisherMock = new Mock<IEventPublisher>();

            _eventPublisherResolverMock.Setup(epr => epr.Resolve(It.IsAny<Event>()))
                .Returns(new List<IEventPublisher> { eventPublisherMock.Object, eventPublisherMock.Object });

            // Act
            await sut.PublishAsync(@event, CancellationToken.None);

            // Assert
            eventPublisherMock.Verify(ep => ep.PublishAsync(It.Is<IEvent>(e => e.Equals(@event)), It.IsAny<CancellationToken>()), Times.Exactly(2));
        }

        [Fact(DisplayName = "EventPublisher - PublishAsync - Publisher not found")]
        public async Task EventPublisher_PublishAsync_PublisherNotFound()
        {
            // Arrange
            var sut = CreateSUT();

            _eventPublisherResolverMock.Setup(epr => epr.Resolve(It.IsAny<Event>()))
                .Returns(Enumerable.Empty<IEventPublisher>());

            // Act
            // Assert
            await Assert.ThrowsAsync<InvalidOperationException>(async () => await sut.PublishAsync(new Event(), CancellationToken.None));
        }

        [Fact(DisplayName = "EventPublisher - PublishAsync To Topic - Resolve publishers")]
        public async Task EventPublisher_PublishAsyncToTopic_ResolvePublishers()
        {
            // Arrange
            var sut = CreateSUT();
            var @event = new Event();

            _eventPublisherResolverMock.Setup(epr => epr.Resolve(It.IsAny<Event>()))
                .Returns(new List<IEventPublisher> { new Mock<IEventPublisher>().Object });

            // Act
            await sut.PublishAsync(@event, new Fixture().Create<string>(), CancellationToken.None);

            // Assert
            _eventPublisherResolverMock.Verify(epkr => epkr.Resolve(It.Is<IEvent>(e => e.Equals(@event))), Times.Once);
        }

        [Fact(DisplayName = "EventPublisher - PublishAsync To Topic - Publish event")]
        public async Task EventPublisher_PublishAsyncToTopic_PublishEvent()
        {
            // Arrange
            var sut = CreateSUT();
            var @event = new Event();
            var eventPublisherMock = new Mock<IEventPublisher>();
            var topic = new Fixture().Create<string>();

            _eventPublisherResolverMock.Setup(epr => epr.Resolve(It.IsAny<Event>()))
                .Returns(new List<IEventPublisher> { eventPublisherMock.Object, eventPublisherMock.Object });

            // Act
            await sut.PublishAsync(@event, topic, CancellationToken.None);

            // Assert
            eventPublisherMock.Verify(ep => ep.PublishAsync(It.Is<IEvent>(e => e.Equals(@event)), It.Is<string>(t => t.Equals(topic)), It.IsAny<CancellationToken>()), Times.Exactly(2));
        }

        [Fact(DisplayName = "EventPublisher - PublishAsync To Topic - Publisher not found")]
        public async Task EventPublisher_PublishAsyncToTopic_PublisherNotFound()
        {
            // Arrange
            var sut = CreateSUT();

            _eventPublisherResolverMock.Setup(epr => epr.Resolve(It.IsAny<Event>()))
                .Returns(Enumerable.Empty<IEventPublisher>());

            // Act
            // Assert
            await Assert.ThrowsAsync<InvalidOperationException>(async () => await sut.PublishAsync(new Event(), new Fixture().Create<string>(), CancellationToken.None));
        }
    }
}
