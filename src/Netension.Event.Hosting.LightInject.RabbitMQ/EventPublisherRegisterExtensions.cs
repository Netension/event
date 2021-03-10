using LightInject;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Netension.Event.Abstraction;
using Netension.Event.Hosting.LightInject.RabbitMQ.Builders;
using Netension.Event.Hosting.LightInject.RabbitMQ.Defaults;
using Netension.Event.Hosting.LightInject.RabbitMQ.Enumerations;
using Netension.Event.RabbitMQ.Options;
using Netension.Event.RabbitMQ.Senders;
using Netension.Event.RabbitMQ.Wrappers;
using RabbitMQ.Client;
using System;

namespace Netension.Event.Hosting.LightInject.Registers
{
    public static class EventPublisherRegisterExtensions
    {
        public static void RegistrateRabbitMQPublisher(this EventPublisherRegister register, RabbitMQPublisherEnumeration enumeration)
        {
            register.RegistrateRabbitMQPublisher(enumeration.RabbitKey, enumeration.Name, enumeration.Predicate, enumeration.Configure, enumeration.Build);
        }

        public static void RegistrateRabbitMQPublisher(this EventPublisherRegister register, string rabbitKey, string key, Func<IEvent, bool> predicate, Action<RabbitMQPublisherOptions, IConfiguration> configure, Action<RabbitMQPublisherBuilder> build)
        {
            register.Builder.ConfigureServices((context, services) =>
            {
                services.AddOptions<RabbitMQPublisherOptions>(key)
                    .Configure(configure)
                    .ValidateDataAnnotations();
            });

            register.Builder.ConfigureContainer<IServiceContainer>((context, container) =>
            {
                container.RegisterScoped<IRabbitMQEventWrapper, RabbitMQEventWrapper>(key);
                container.RegisterScoped<IEventPublisher>((factory) => new RabbitMQEventPublisher(factory.GetInstance<IModel>($"{rabbitKey}-{RabbitMQDefaults.Connections.PublisherSuffix}"), factory.GetInstance<IRabbitMQEventWrapper>(key), factory.GetInstance<IOptionsSnapshot<RabbitMQPublisherOptions>>().Get(key), factory.GetInstance<ILogger<RabbitMQEventPublisher>>()), key);
            });

            register.Register.Registrate(key, predicate);

            build(new RabbitMQPublisherBuilder(key, register.Builder));
        }
    }
}
