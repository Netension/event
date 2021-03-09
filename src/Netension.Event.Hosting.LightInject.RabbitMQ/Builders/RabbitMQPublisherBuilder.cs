using LightInject;
using Microsoft.Extensions.Hosting;
using Netension.Event.RabbitMQ.Wrappers;

namespace Netension.Event.Hosting.LightInject.RabbitMQ.Builders
{
    public class RabbitMQPublisherBuilder
    {
        public string Key { get; }
        public IHostBuilder Builder { get; }

        public RabbitMQPublisherBuilder(string key, IHostBuilder builder)
        {
            Key = key;
            Builder = builder;
        }

        public RabbitMQPublisherBuilder UseCorrelation()
        {
            Builder.ConfigureContainer<IServiceContainer>((context, container) =>
            {
                container.Decorate(typeof(IRabbitMQEventWrapper), typeof(RabbitMQCorrelationWrapper), registration => registration.ServiceName.Equals(Key));
            });

            return this;
        }
    }
}
