using Netension.Event.Abstraction;
using RabbitMQ.Client.Events;

namespace Netension.Event.RabbitMQ.Unwrappers
{
    public interface IRabbitMQEventUnwrapper : IEventUnwrapper<BasicDeliverEventArgs>
    {
    }
}
