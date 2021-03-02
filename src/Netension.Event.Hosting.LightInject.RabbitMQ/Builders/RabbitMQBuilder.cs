using LightInject;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Netension.Event.Abstraction;
using Netension.Event.Containers;
using Netension.Event.Hosting.LightInject.RabbitMQ.Defaults;
using Netension.Event.Hosting.LightInject.RabbitMQ.Startups;
using Netension.Event.RabbitMQ.Initializers;
using Netension.Event.RabbitMQ.Listeners;
using Netension.Event.RabbitMQ.Options;
using Netension.Event.RabbitMQ.Receivers;
using Netension.Event.RabbitMQ.Senders;
using Netension.Event.RabbitMQ.Unwrappers;
using Netension.Event.RabbitMQ.Wrappers;
using RabbitMQ.Client;
using System;

namespace Netension.Event.Hosting.LightInject.RabbitMQ.Builders
{
    public class RabbitMQBuilder
    {
        public IHostBuilder HostBuilder { get; }
        public string RabbitMQKey { get; }
        public IEventPublisherKeyRegister Register { get; }

        public RabbitMQBuilder(IHostBuilder hostBuilder, string rabbitMQKey, IEventPublisherKeyRegister register)
        {
            HostBuilder = hostBuilder;
            RabbitMQKey = rabbitMQKey;
            Register = register;
        }

        public void AddPublisher(string key, Action<RabbitMQPublisherOptions, IConfiguration> configure, Func<IEvent, bool> predicate)
        {
            HostBuilder.ConfigureServices((context, service) =>
            {
                service.AddOptions<RabbitMQPublisherOptions>(key)
                    .Configure(configure)
                    .ValidateDataAnnotations();
            });

            HostBuilder.ConfigureContainer<IServiceContainer>((context, container) =>
            {
                Register.Registrate(key, predicate);

                container.RegisterTransient<IRabbitMQEventWrapper, RabbitMQEventWrapper>(key);
                container.RegisterScoped<IEventPublisher>(factory => new RabbitMQEventPublisher(factory.GetInstance<IModel>($"{RabbitMQKey}-{RabbitMQDefaults.Connections.PublisherSuffix}"), factory.GetInstance<IRabbitMQEventWrapper>(), factory.GetInstance<IOptionsSnapshot<RabbitMQPublisherOptions>>().Get(key), factory.GetInstance<ILogger<RabbitMQEventPublisher>>()), key);
            });
        }

        public void AddListener(Action<RabbitMQListenerOptions, IConfiguration> configure)
        {
            AddListener(RabbitMQDefaults.Key, configure);
        }

        public void AddListener(string key, Action<RabbitMQListenerOptions, IConfiguration> configure)
        {
            HostBuilder.ConfigureServices((context, services) =>
            {
                services.AddOptions<RabbitMQListenerOptions>(key)
                    .Configure(configure)
                    .ValidateDataAnnotations();

                services.AddHostedService<ListenerStartup>();
            });

            HostBuilder.ConfigureContainer<IServiceContainer>((context, container) =>
            {
                container.RegisterTransient<IRabbitMQEventUnwrapper, RabbitMQEventUnwrapper>(key);
                container.RegisterTransient<IRabbitMQEventReceiver, RabbitMQEventReceiver>(key);

                container.RegisterSingleton<IEventListener>(factory => new RabbitMQEventListener(factory.GetInstance<IModel>($"{RabbitMQKey}-{RabbitMQDefaults.Connections.ListenerSuffix}"), factory.GetInstance<IOptionsSnapshot<RabbitMQListenerOptions>>().Get(key), factory.GetInstance<IRabbitMQEventReceiver>(key), factory.GetInstance<IRabbitMQInitializer>(), factory.GetInstance<ILogger<RabbitMQEventListener>>()), key);
            });
        }
    }
}
