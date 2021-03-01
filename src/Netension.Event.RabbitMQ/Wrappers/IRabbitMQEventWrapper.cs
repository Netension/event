using Netension.Event.Abstraction;
using Netension.Event.RabbitMQ.Messages;

namespace Netension.Event.RabbitMQ.Wrappers
{
    public interface IRabbitMQEventWrapper : IEventWrapper<RabbitMQMessage>
    {
    }
}
