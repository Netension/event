using System;

namespace Netension.Event.Abstraction
{
    public interface IEvent
    {
        Guid EventId { get; }
    }
}
