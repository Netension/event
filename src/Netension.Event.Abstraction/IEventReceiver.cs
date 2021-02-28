using System.Threading;
using System.Threading.Tasks;

namespace Netension.Event.Abstraction
{
    public interface IEventReceiver<in TMessage>
    {
        Task ReceiveAsync(TMessage message, CancellationToken cancellationToken);
    }
}
