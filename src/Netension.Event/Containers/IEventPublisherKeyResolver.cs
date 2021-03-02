using Netension.Event.Abstraction;
using System.Collections.Generic;

namespace Netension.Event.Containers
{
    public interface IEventPublisherKeyResolver
    {
        IEnumerable<string> Resolve(IEvent @event);
    }
}
