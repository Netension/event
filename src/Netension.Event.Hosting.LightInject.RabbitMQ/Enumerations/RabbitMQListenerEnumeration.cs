using Microsoft.Extensions.Configuration;
using Netension.Core;
using Netension.Event.Hosting.LightInject.RabbitMQ.Builders;
using Netension.Event.Hosting.LightInject.RabbitMQ.Defaults;
using Netension.Event.RabbitMQ.Options;
using System;

namespace Netension.Event.Hosting.LightInject.RabbitMQ.Enumerations
{
    public class RabbitMQListenerEnumeration : Enumeration
    {
        public string RabbitKey { get; }
        public Action<RabbitMQListenerOptions, IConfiguration> Configure { get; }
        public Action<RabbitMQListenerBuilder> Build { get; }

        public RabbitMQListenerEnumeration(int id, string name, string configurationSection, Action<RabbitMQListenerBuilder> build)
            : this(id, name, RabbitMQDefaults.Key, (options, configuration) => configuration.GetSection(configurationSection).Bind(options), build)
        {

        }

        public RabbitMQListenerEnumeration(int id, string name, Action<RabbitMQListenerOptions, IConfiguration> configure, Action<RabbitMQListenerBuilder> build)
            : this(id, name, RabbitMQDefaults.Key, configure, build)
        {

        }

        public RabbitMQListenerEnumeration(int id, string name, string rabbitKey, Action<RabbitMQListenerOptions, IConfiguration> configure, Action<RabbitMQListenerBuilder> build) 
            : base(id, name)
        {
            RabbitKey = rabbitKey;
            Configure = configure;
            Build = build;
        }
    }
}
