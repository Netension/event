using Netension.Event.Abstraction;
using System;

namespace Netension.Event
{
    public class Event : IEvent
    {
        public Guid EventId { get; }

        public Event(Guid eventId)
        {
            EventId = eventId;
        }
    }
}
