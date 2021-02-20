using System.Threading;
using System.Threading.Tasks;

namespace Netension.Event.Abstraction
{
    public interface IEventSender
    {
        Task SendAsync(IEvent @event, CancellationToken cancellationToken);
    }
}
