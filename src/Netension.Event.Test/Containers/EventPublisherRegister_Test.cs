using AutoFixture;
using Netension.Event.Abstraction;
using Netension.Event.Containers;
using System;
using Xunit;

namespace Netension.Event.Test.Containers
{
    public class EventPublisherRegister_Test
    {
        private EventPublisherCollection _collection;

        private EventPublisherRegister CreateSUT()
        {
            _collection = new EventPublisherCollection();

            return new EventPublisherRegister(_collection);
        }

        [Fact(DisplayName = "EventPublisherRegister - Registrate - Key is null")]
        public void EventPublisherRegister_Registrate_KeyIsNull()
        {
            // Arrange
            var sut = CreateSUT();

            // Act
            // Assert
            Assert.Throws<ArgumentNullException>(() => sut.Registrate(null, (@event) => true));
        }

        [Fact(DisplayName = "EventPublisherRegister - Registrate - Predicate is null")]
        public void EventPublisherRegister_Registrate_PredicateIsNull()
        {
            // Arrange
            var sut = CreateSUT();

            // Act
            // Assert
            Assert.Throws<ArgumentNullException>(() => sut.Registrate(new Fixture().Create<string>(), null));
        }

        [Fact(DisplayName = "EventPublisherRegister - Registrate - Add key")]
        public void EventPublisherRegister_Registrate_AddKey()
        {
            // Arrange
            var sut = CreateSUT();
            var key = new Fixture().Create<string>();
            Func<IEvent, bool> predicate = (@event) => true;

            // Act
            sut.Registrate(key, predicate);

            // Assert
            Assert.True(_collection.ContainsKey(key));
            Assert.Equal(predicate, _collection[key]);
        }
    }
}
