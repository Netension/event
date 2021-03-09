using LightInject;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Netension.Event.Abstraction;
using Netension.Event.Hosting.LightInject.RabbitMQ.Builders;
using Netension.Event.Hosting.LightInject.RabbitMQ.Defaults;
using Netension.Event.RabbitMQ.Initializers;
using Netension.Event.RabbitMQ.Listeners;
using Netension.Event.RabbitMQ.Options;
using Netension.Event.RabbitMQ.Receivers;
using Netension.Event.RabbitMQ.Unwrappers;
using RabbitMQ.Client;
using System;

namespace Netension.Event.Hosting.LightInject.Registers
{
    public static class EventListenerRegisterExtensions
    {
        public static void RegistrateRabbitMQListener(this EventListenerRegister register, string rabbitKey, string key, Action<RabbitMQListenerOptions, IConfiguration> configure, Action<RabbitMQListenerBuilder> build)
        {
            register.Builder.ConfigureServices((context, services) =>
            {
                services.AddOptions<RabbitMQListenerOptions>(key)
                    .Configure(configure)
                    .ValidateDataAnnotations();
            });

            register.Builder.ConfigureContainer<IServiceContainer>((context, container) =>
            {
                container.RegisterScoped<IRabbitMQEventUnwrapper, RabbitMQEventUnwrapper>(key);
                container.RegisterScoped<IRabbitMQEventReceiver, RabbitMQEventReceiver>(key);
                container.Decorate(typeof(IRabbitMQEventReceiver), typeof(RabbitMQScopeHandler), registration => registration.ServiceName.Equals(key));
                container.RegisterScoped<IEventListener>((factory) => new RabbitMQEventListener(factory.GetInstance<IModel>($"{rabbitKey}-{RabbitMQDefaults.Connections.ListenerSuffix}"), factory.GetInstance<IOptionsSnapshot<RabbitMQListenerOptions>>().Get(key), factory.GetInstance<IRabbitMQEventReceiver>(key), factory.GetInstance<IRabbitMQInitializer>(), factory.GetInstance<ILogger<RabbitMQEventListener>>()), key);
            });

            build(new RabbitMQListenerBuilder(key, register.Builder));
        }
    }
}
