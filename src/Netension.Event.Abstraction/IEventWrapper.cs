using System.Threading;
using System.Threading.Tasks;

namespace Netension.Event.Abstraction
{
    public interface IEventWrapper<TEnvelop>
    {
        Task<TEnvelop> WrapAsync<TEvent>(TEvent @event, CancellationToken cancellationToken)
            where TEvent : IEvent;
    }
}
