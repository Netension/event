using Netension.Event.Abstraction;
using System.Collections.Generic;

namespace Netension.Event.Containers
{
    public interface IEventPublisherResolver
    {
        IEnumerable<IEventPublisher> Resolve(IEvent @event);
    }
}
