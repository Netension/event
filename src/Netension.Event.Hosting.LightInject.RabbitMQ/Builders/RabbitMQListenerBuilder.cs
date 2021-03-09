using LightInject;
using Microsoft.Extensions.Hosting;
using Netension.Event.RabbitMQ.Unwrappers;

namespace Netension.Event.Hosting.LightInject.RabbitMQ.Builders
{
    public class RabbitMQListenerBuilder
    {
        public string Key { get; }
        public IHostBuilder HostBuilder { get; }

        public RabbitMQListenerBuilder(string key, IHostBuilder hostBuilder)
        {
            Key = key;
            HostBuilder = hostBuilder;
        }

        public RabbitMQListenerBuilder UseCorrelation()
        {
            HostBuilder.ConfigureContainer<IServiceContainer>((context, container) =>
            {
                container.Decorate(typeof(IRabbitMQEventUnwrapper), typeof(RabbitMQCorrelationUnwrapper), registration => registration.ServiceName.Equals(Key));
            });

            return this;
        }
    }
}
