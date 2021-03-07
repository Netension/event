using System.Threading;
using System.Threading.Tasks;

namespace Netension.Event.Abstraction
{
    /// <summary>
    /// Responsible for distribute the events to the <see cref="IEventHandler{TEvent}">IEventHandler</see>.
    /// </summary>
    public interface IEventDispatcher
    {
        /// <summary>
        /// Consume the event from the channel and distribute it to the <see cref="IEventHandler{TEvent}">IEventHandler</see>.
        /// </summary>
        /// <param name="event">Incoming <see cref="IEvent"/> instance.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> of the async method.</param>
        /// <exception cref="System.ArgumentNullException">Thrown if the <paramref name="event"/> is null.</exception>
        Task DispatchAsync(IEvent @event, CancellationToken cancellationToken);
    }
}
