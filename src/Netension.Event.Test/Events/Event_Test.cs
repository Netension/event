using System;
using Xunit;

namespace Netension.Event.Test.Events
{
    public class Event_Test
    {
        [Fact(DisplayName = "Event - Equal by id")]
        public void Event_EqualById()
        {
            // Arrange
            var id = Guid.NewGuid();

            // Act
            // Assert
            Assert.True(new Event(id).Equals(new Event(id)));
        }

        [Fact(DisplayName = "Event - Not equal by id")]
        public void Event_NotEqualById()
        {
            // Arrange
            // Act
            // Assert
            Assert.False(new Event(Guid.NewGuid()).Equals(new Event(Guid.NewGuid())));
        }

        [Fact(DisplayName = "Command - Message type")]
        public void Event_MessageType()
        {
            // Arrange
            var @event = new Event(Guid.NewGuid());

            // Act
            var messageType = @event.MessageType;

            // Assert
            Assert.Equal($"{@event.GetType().FullName}, {@event.GetType().Assembly.GetName().Name}", messageType);
        }
    }
}
