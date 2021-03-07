using Netension.Event.Abstraction;
using System;
using System.Collections.Generic;

namespace Netension.Event.Containers
{
    public class EventPublisherCollection : Dictionary<string, Func<IEvent, bool>>
    {

    }
}
