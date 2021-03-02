﻿using LightInject;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Netension.Event.Abstraction;
using Netension.Event.Containers;
using Netension.Event.Hosting.Builders;
using Netension.Event.Hosting.LightInject.RabbitMQ.Builders;
using Netension.Event.Hosting.LightInject.RabbitMQ.Defaults;
using Netension.Event.Publishers;
using Netension.Event.RabbitMQ.Initializers;
using Netension.Event.RabbitMQ.Options;
using Netension.Extensions.Security;
using RabbitMQ.Client;
using System;

namespace Netension.Event.Hosting.RabbitMQ
{
    public static class EventingBuilderExtensions
    {
        public static void UseRabbitMQ(this EventingBuilder builder, Action<RabbitMQOptions, IConfiguration> configure, Action<RabbitMQBuilder> build)
        {
            builder.UseRabbitMQ(RabbitMQDefaults.Key, configure, build);
        }

        public static void UseRabbitMQ(this EventingBuilder builder, string key, Action<RabbitMQOptions, IConfiguration> configure, Action<RabbitMQBuilder> build)
        {
            var keyContainer = new EventPublisherKeyContainer();

            builder.HostBuilder.ConfigureServices((context, services) =>
            {
                services.AddOptions<RabbitMQOptions>(key)
                    .Configure(configure)
                    .ValidateDataAnnotations();
            });

            builder.HostBuilder.ConfigureContainer<IServiceContainer>((context, container) =>
            {
                container.RegisterInstance<IEventPublisherKeyResolver>(keyContainer);
                container.RegisterInstance<IEventPublisherKeyRegister>(keyContainer);

                container.RegisterSingleton(factory => CreateChannel(factory.GetInstance<IOptionsSnapshot<RabbitMQOptions>>().Get(key)), $"{key}-{RabbitMQDefaults.Connections.ListenerSuffix}");
                container.RegisterSingleton(factory => CreateChannel(factory.GetInstance<IOptionsSnapshot<RabbitMQOptions>>().Get(key)), $"{key}-{RabbitMQDefaults.Connections.PublisherSuffix}");
               
                container.RegisterTransient<IRabbitMQInitializer, RabbitMQInitializer>();
                container.RegisterTransient<IEventPublisher, EventPublisher>();
            });

            build(new RabbitMQBuilder(builder.HostBuilder, key, keyContainer));
        }

        private static IModel CreateChannel(RabbitMQOptions options)
        {
            return new ConnectionFactory
            {
                HostName = options.Host,
                Port = options.Port,
                UserName = options.UserName,
                Password = options.Password.Decrypt(),
                DispatchConsumersAsync = true
            }.CreateConnection().CreateModel();
        }
    }
}
