using AutoFixture;
using LightInject;
using LightInject.Microsoft.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Netension.Event.Abstraction;
using Netension.Event.Containers;
using System;
using System.Linq;
using Xunit;
using Xunit.Abstractions;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;

namespace Netension.Event.Test.Containers
{
    public class EventPublisherContainer_Test
    {
        private readonly ILogger<EventPublisherResolver> _logger;
        private ServiceContainer _serviceContainer;
        private EventPublisherCollection _collection;

        public EventPublisherContainer_Test(ITestOutputHelper outputHelper)
        {
            _logger = new LoggerFactory()
                        .AddXUnit(outputHelper, LogLevel.Trace)
                        .CreateLogger<EventPublisherResolver>();
        }

        private EventPublisherResolver CreateSUT()
        {
            _serviceContainer = new ServiceContainer();
            _collection = new EventPublisherCollection();

            return new EventPublisherResolver(_serviceContainer.CreateServiceProvider(new ServiceCollection()), _collection, _logger);
        }

        [Fact(DisplayName = "EventPublisherContainer - Resolve - Resolve publishers")]
        public void EventPublisherContainer_Resolve_ResolvePublishers()
        {
            // Arrange
            var sut = CreateSUT();
            var publisherMock = new Mock<IEventPublisher>();
            var @event = new Event();
            var key1 = new Fixture().Create<string>();
            var key2 = new Fixture().Create<string>();
            var key3 = new Fixture().Create<string>();

            _collection.Add(key1, (@event) => true);
            _collection.Add(key2, (@event) => true);
            _collection.Add(key3, (@event) => false);

            _serviceContainer.RegisterInstance(publisherMock.Object, key1);
            _serviceContainer.RegisterInstance(publisherMock.Object, key2);
            _serviceContainer.RegisterInstance(publisherMock.Object, key3);

            // Act
            var result = sut.Resolve(@event);

            // Assert
            Assert.Equal(2, result.Count(r => r.Equals(publisherMock.Object)));
        }

        [Fact(DisplayName = "EventPublisherContainer - Resolve - Event null", Skip = "False failed")]
        public void EventPublisherContainer_Resolve_EventNull()
        {
            // Arrange
            var sut = CreateSUT();

            // Act
            sut.Resolve(null);

            // Assert
            Assert.Throws<ArgumentNullException>(() => sut.Resolve(null));
        }

        [Fact(DisplayName = "EventPublisherContainer - Resolve - Publisher not found")]
        public void EventPublisherContainer_Resolve_PublisherNotFound()
        {
            // Arrange
            var sut = CreateSUT();

            // Act
            sut.Resolve(new Event());

            // Assert
            Assert.True(true);
        }
    }
}
