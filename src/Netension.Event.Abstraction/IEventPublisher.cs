using System.Threading;
using System.Threading.Tasks;

namespace Netension.Event.Abstraction
{
    public interface IEventPublisher
    {
        Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken)
            where TEvent : IEvent;
        Task PublishAsync<TEvent>(TEvent @event, string topic, CancellationToken cancellationToken)
            where TEvent : IEvent;
    }
}
