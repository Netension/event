using Netension.Event.Abstraction;
using System;

namespace Netension.Event
{
    public class Event : IEvent
    {
        public Guid EventId { get; }
        public string MessageType => $"{GetType().FullName}, {GetType().Assembly.GetName().Name}";

        public Event(Guid eventId)
        {
            EventId = eventId;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as IEvent);
        }

        public bool Equals(IEvent other)
        {
            return other != null && EventId.Equals(other.EventId);
        }

        public override int GetHashCode()
        {
            return -2107324841 + EventId.GetHashCode();
        }
    }
}
