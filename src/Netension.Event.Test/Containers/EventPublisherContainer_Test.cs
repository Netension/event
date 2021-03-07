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
        private readonly ILogger<EventPublisherContainer> _logger;
        private ServiceContainer _serviceContainer;

        public EventPublisherContainer_Test(ITestOutputHelper outputHelper)
        {
            _logger = new LoggerFactory()
                        .AddXUnit(outputHelper, LogLevel.Trace)
                        .CreateLogger<EventPublisherContainer>();
        }

        private EventPublisherContainer CreateSUT()
        {
            _serviceContainer = new ServiceContainer();

            return new EventPublisherContainer(_serviceContainer.CreateServiceProvider(new ServiceCollection()), _logger);
        }

        [Fact(DisplayName = "EventPublisherContainer - Registrate - Key is null")]
        public void EventPublisherContainer_Registrate_KeyIsNull()
        {
            // Arrange
            var sut = CreateSUT();

            // Act
            // Assert
            Assert.Throws<ArgumentNullException>(() => sut.Registrate(null, (@event) => true));
        }

        [Fact(DisplayName = "EventPublisherContainer - Registrate - Predicate is null")]
        public void EventPublisherContainer_Registrate_PredicateIsNull()
        {
            // Arrange
            var sut = CreateSUT();

            // Act
            // Assert
            Assert.Throws<ArgumentNullException>(() => sut.Registrate(new Fixture().Create<string>(), null));
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

            sut.Registrate(key1, (@event) => true);
            sut.Registrate(key2, (@event) => true);
            sut.Registrate(key3, (@event) => false);

            _serviceContainer.RegisterInstance(publisherMock.Object, key1);
            _serviceContainer.RegisterInstance(publisherMock.Object, key2);
            _serviceContainer.RegisterInstance(publisherMock.Object, key3);

            // Act
            var result = sut.Resolve(@event);

            // Assert
            Assert.Equal(2, result.Count(r => r.Equals(publisherMock.Object)));
        }

        [Fact(DisplayName = "EventPublisherContainer - Resolve - Event null")]
        public void EventPublisherContainer_Resolve_EventNull()
        {
            // Arrange
            var sut = CreateSUT();

            // Act
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
