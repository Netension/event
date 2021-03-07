using System;
using System.Collections.Generic;

namespace Netension.Event.Abstraction
{
    /// <summary>
    /// Base type of the events. Equatable by EventId property.
    /// </summary>
    public interface IEvent : IEqualityComparer<IEvent>
    {
        /// <summary>
        /// Unique id of the event.
        /// </summary>
        /// <remarks>
        /// Events are equal by EventId.
        /// </remarks>
        Guid? EventId { get; }
    }
}
