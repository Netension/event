using LightInject;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Netension.Event.Abstraction;
using Netension.Event.Hosting.LightInject.RabbitMQ.Defaults;
using Netension.Event.Hosting.LightInject.RabbitMQ.Listeners;
using Netension.Event.Hosting.LightInject.RabbitMQ.Options;
using Netension.Event.RabbitMQ.Receivers;
using Netension.Event.RabbitMQ.Unwrappers;
using System;

namespace Netension.Event.Hosting.LightInject.RabbitMQ.Builders
{
    public class RabbitMQBuilder
    {
        public IHostBuilder HostBuilder { get; }

        public RabbitMQBuilder(IHostBuilder hostBuilder)
        {
            HostBuilder = hostBuilder;
        }

        public void AddListener(Action<RabbitMQListenerOptions, IConfiguration> configure)
        {
            AddListener(RabbitMQDefaults.Receiver, configure);
        }

        public void AddListener(string key, Action<RabbitMQListenerOptions, IConfiguration> configure)
        {
            HostBuilder.ConfigureServices((context, services) =>
            {
                services.AddOptions<RabbitMQListenerOptions>()
                    .Configure(configure)
                    .ValidateDataAnnotations();
            });

            HostBuilder.ConfigureServices((context, services) =>
            {
                services.AddHostedService<RabbitMQListener>();
            });

            HostBuilder.ConfigureContainer<IServiceContainer>((context, container) =>
            {
                container.RegisterTransient<IRabbitMQEventUnwrapper, RabbitMQEventUnwrapper>(key);
                container.RegisterTransient<IRabbitMQEventReceiver, RabbitMQEventReceiver>(key);
            });
        }
    }
}
