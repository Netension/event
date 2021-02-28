using System.Threading;
using System.Threading.Tasks;

namespace Netension.Event.Abstraction
{
    public interface IEventHandler<in TEvent>
        where TEvent : IEvent
    {
        Task HandleAsync(TEvent @event, CancellationToken cancellationToken);
    }
}
