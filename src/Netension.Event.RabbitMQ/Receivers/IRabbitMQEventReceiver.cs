using Netension.Event.Abstraction;
using RabbitMQ.Client.Events;

namespace Netension.Event.RabbitMQ.Receivers
{
    public interface IRabbitMQEventReceiver : IEventReceiver<BasicDeliverEventArgs>
    {
    }
}
