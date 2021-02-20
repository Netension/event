using System.Threading;
using System.Threading.Tasks;

namespace Netension.Event.Abstraction
{
    public interface IEventWrapper<TEnvelop>
    {
        Task<TEnvelop> WrapAsync(IEvent @event, CancellationToken cancellationToken);
    }
}
