using Netension.Event.Abstraction;
using System;
using System.Diagnostics.CodeAnalysis;

namespace Netension.Event
{
    /// <inheritdoc cref="IEvent"/>
    public class Event : IEvent
    {
        public Guid? EventId { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Event"/>.
        /// </summary>
        /// <param name="eventId">Unique id of the <see cref="Event"/>. If it is null it will be generated.</param>
        public Event(Guid? eventId = null)
        {
            EventId = eventId ?? Guid.NewGuid();
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as IEvent);
        }

        public bool Equals(IEvent other)
        {
            return other != null && EventId.Equals(other.EventId);
        }

        [ExcludeFromCodeCoverage]
        public override int GetHashCode()
        {
            return -2107324841 + EventId.GetHashCode();
        }

        public bool Equals(IEvent x, IEvent y)
        {
            if (x == null && y == null) return true;
            if (x == null || y == null) return false;
            return x.Equals(y);
        }

        [ExcludeFromCodeCoverage]
        public int GetHashCode(IEvent obj)
        {
            return obj.GetHashCode();
        }
    }
}
