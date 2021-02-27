using System.Threading;
using System.Threading.Tasks;

namespace Netension.Event.Abstraction
{
    public interface IEventListener
    {
        Task ListenAsync(CancellationToken cancellationToken);
        Task StopAsync(CancellationToken cancellationToken);
    }
}
