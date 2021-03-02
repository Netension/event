using Netension.Event.Abstraction;
using System;

namespace Netension.Event.Containers
{
    public interface IEventPublisherKeyRegister
    {
        void Registrate(string key, Func<IEvent, bool> predicate);
    }
}
