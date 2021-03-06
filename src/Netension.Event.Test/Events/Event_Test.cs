using Netension.Event.Extensions;
using System;
using Xunit;

namespace Netension.Event.Test.Events
{
    public class Event_Test
    {
        [Fact(DisplayName = "Event - Set id")]
        public void Event_SetId()
        {
            // Arrange
            var id = Guid.NewGuid();

            // Act
            var sut = new Event(id);

            // Assert
            Assert.Equal(id, sut.EventId);
        }

        [Fact(DisplayName = "Event - Generate id")]
        public void Event_GeneratedId()
        {
            // Arrange
            // Act
            var sut = new Event();

            // Assert
            Assert.NotNull(sut.EventId);
        }

        [Fact(DisplayName = "Event - Equals - Same id")]
        public void Event_Equals_SameId()
        {
            // Arrange
            var id = Guid.NewGuid();

            // Act
            // Assert
            Assert.True(new Event(id).Equals(new Event(id)));
        }

        [Fact(DisplayName = "Event - Equals - Different id")]
        public void Event_Equals_DifferentId()
        {
            // Arrange
            // Act
            // Assert
            Assert.False(new Event(Guid.NewGuid()).Equals(new Event(Guid.NewGuid())));
        }

        [Fact(DisplayName = "Event - Equals - Null")]
        public void Event_Equals_Null()
        {
            // Arrange
            // Act
            // Assert
            Assert.False(new Event(Guid.NewGuid()).Equals(null));
        }

        [Fact(DisplayName = "Event - EqualityCompare - Null to Event")]
        public void Event_EqualComparer_NullToEvent()
        {
            Assert.False(new Event().Equals(null, new Event()));
        }

        [Fact(DisplayName = "Event - EqualCompare - Event to Null")]
        public void Event_EqualComparer_EventToNull()
        {
            // Arrange
            // Act
            // Assert
            Assert.False(new Event().Equals(new Event(), null));
        }

        [Fact(DisplayName = "Event - EqualCompare - Null to Null")]
        public void Event_EqualComparer_NullToNull()
        {
            // Arrange
            // Act
            // Assert
            Assert.True(new Event().Equals(null, null));
        }

        [Fact(DisplayName = "Event - EqualCompare - Same id")]
        public void Event_EqualComparer_SameId()
        {
            // Arrange
            var id = Guid.NewGuid();

            // Act
            // Assert
            Assert.True(new Event().Equals(new Event(id), new Event(id)));
        }

        [Fact(DisplayName = "Event - EqualCompare - Different id")]
        public void Event_EqualComparer_DifferentId()
        {
            // Arrange
            // Act
            // Assert
            Assert.False(new Event().Equals(new Event(), new Event()));
        }

        [Fact(DisplayName = "Command - Message type")]
        public void Event_MessageType()
        {
            // Arrange
            var @event = new Event(Guid.NewGuid());

            // Act
            var messageType = @event.GetMessageType();

            // Assert
            Assert.Equal($"{@event.GetType().FullName}, {@event.GetType().Assembly.GetName().Name}", messageType);
        }
    }
}
