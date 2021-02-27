using LightInject;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Netension.Event.Hosting.Builders;
using Netension.Event.Hosting.LightInject.RabbitMQ.Builders;
using Netension.Event.Hosting.LightInject.RabbitMQ.Defaults;
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
            builder.HostBuilder.ConfigureServices((context, services) =>
            {
                services.AddOptions<RabbitMQOptions>(key)
                    .Configure(configure)
                    .ValidateDataAnnotations();
            });

            builder.HostBuilder.ConfigureContainer<IServiceContainer>((context, container) =>
            {
                container.RegisterSingleton(factory => CreateConnection(factory.GetInstance<IOptionsSnapshot<RabbitMQOptions>>().Get(key)), $"{key}-{RabbitMQDefaults.Connections.ListenerSuffix}");
            });

            build(new RabbitMQBuilder(builder.HostBuilder, key));
        }

        private static IConnection CreateConnection(RabbitMQOptions options)
        {
            return new ConnectionFactory
            {
                HostName = options.Host,
                Port = options.Port,
                UserName = options.UserName,
                Password = options.Password.Decrypt(),
                DispatchConsumersAsync = true
            }.CreateConnection();
        }
    }
}
