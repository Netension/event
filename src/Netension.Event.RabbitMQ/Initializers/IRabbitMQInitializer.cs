using Netension.Event.RabbitMQ.Options;
using RabbitMQ.Client;
using System.Threading;
using System.Threading.Tasks;

namespace Netension.Event.RabbitMQ.Initializers
{
    public interface IRabbitMQInitializer
    {
        Task InitializeAsync(IModel channel, RabbitMQListenerOptions options, CancellationToken cancellationToken);
    }
}
