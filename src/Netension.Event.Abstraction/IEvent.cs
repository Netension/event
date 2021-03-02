using System;
using System.Text.Json.Serialization;

namespace Netension.Event.Abstraction
{
    public interface IEvent : IEquatable<IEvent>
    {
        Guid EventId { get; }
        [JsonIgnore]
        string MessageType { get; }
    }
}
