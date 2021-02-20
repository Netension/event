using System.Threading;
using System.Threading.Tasks;

namespace Netension.Event.Abstraction
{
    public interface IEventUnwrapper<TEnvelop>
    {
        Task<IEvent> UnwrapAsync(TEnvelop envelop, CancellationToken cancellationToken);
    }
}
