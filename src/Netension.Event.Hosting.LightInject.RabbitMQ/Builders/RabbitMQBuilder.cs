using LightInject;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Netension.Event.Abstraction;
using Netension.Event.Hosting.LightInject.RabbitMQ.Defaults;
using Netension.Event.Hosting.LightInject.RabbitMQ.Startups;
using Netension.Event.RabbitMQ.Initializers;
using Netension.Event.RabbitMQ.Listeners;
using Netension.Event.RabbitMQ.Options;
using Netension.Event.RabbitMQ.Receivers;
using Netension.Event.RabbitMQ.Unwrappers;
using RabbitMQ.Client;
using System;

namespace Netension.Event.Hosting.LightInject.RabbitMQ.Builders
{
    public class RabbitMQBuilder
    {
        public IHostBuilder HostBuilder { get; }
        public string RabbitMQKey { get; }

        public RabbitMQBuilder(IHostBuilder hostBuilder, string rabbitMQKey)
        {
            HostBuilder = hostBuilder;
            RabbitMQKey = rabbitMQKey;
        }

        public void AddListener(Action<RabbitMQListenerOptions, IConfiguration> configure)
        {
            AddListener(RabbitMQDefaults.Listener, configure);
        }

        public void AddListener(string key, Action<RabbitMQListenerOptions, IConfiguration> configure)
        {
            HostBuilder.ConfigureServices((context, services) =>
            {
                services.AddOptions<RabbitMQListenerOptions>(key)
                    .Configure(configure)
                    .ValidateDataAnnotations();
            });

            HostBuilder.ConfigureServices((context, services) =>
            {
                services.AddHostedService<ListenerStartup>();
            });

            HostBuilder.ConfigureContainer<IServiceContainer>((context, container) =>
            {
                container.RegisterTransient<IRabbitMQEventUnwrapper, RabbitMQEventUnwrapper>(key);
                container.RegisterTransient<IRabbitMQEventReceiver, RabbitMQEventReceiver>(key);

                container.RegisterSingleton(factory => factory.GetInstance<IConnectionFactory>().CreateConnection());

                container.RegisterSingleton<IEventListener>(factory => new RabbitMQEventListener(factory.GetInstance<IConnection>($"{RabbitMQKey}-{RabbitMQDefaults.Connections.ListenerSuffix}"), factory.GetInstance<IOptionsSnapshot<RabbitMQListenerOptions>>().Get(key), factory.GetInstance<IRabbitMQEventReceiver>(key), factory.GetInstance<IRabbitMQInitializer>(), factory.GetInstance<ILogger<RabbitMQEventListener>>()), key);
            });
        }
    }
}
