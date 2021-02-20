using System;

namespace Netension.Event.Abstraction
{
    public interface IEvent : IEquatable<IEvent>
    {
        Guid EventId { get; }
        string MessageType { get; }
    }
}
