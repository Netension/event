using System.Threading;
using System.Threading.Tasks;

namespace Netension.Event.Abstraction
{
    public interface IEventDispatcher
    {
        Task DispatchAsync(IEvent @event, CancellationToken cancellationToken);
    }
}
