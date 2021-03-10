using Microsoft.Extensions.Configuration;
using Netension.Core;
using Netension.Event.Abstraction;
using Netension.Event.Hosting.LightInject.RabbitMQ.Builders;
using Netension.Event.Hosting.LightInject.RabbitMQ.Defaults;
using Netension.Event.RabbitMQ.Options;
using System;

namespace Netension.Event.Hosting.LightInject.RabbitMQ.Enumerations
{
    public class RabbitMQPublisherEnumeration : Enumeration
    {
        public string RabbitKey { get; }
        public Func<IEvent, bool> Predicate { get; }
        public Action<RabbitMQPublisherOptions, IConfiguration> Configure { get; }
        public Action<RabbitMQPublisherBuilder> Build { get; }

        public RabbitMQPublisherEnumeration(int id, string name, string configurationSection, Action<RabbitMQPublisherBuilder> build)
            : this(id, name, RabbitMQDefaults.Key, (options, configuration) => configuration.GetSection(configurationSection).Bind(options), (@event) => true, build)
        {

        }

        public RabbitMQPublisherEnumeration(int id, string name, string configurationSection, Func<IEvent, bool> predicate, Action<RabbitMQPublisherBuilder> build)
            : this(id, name, RabbitMQDefaults.Key, (options, configuration) => configuration.GetSection(configurationSection).Bind(options), predicate, build)
        {

        }

        public RabbitMQPublisherEnumeration(int id, string name, Action<RabbitMQPublisherOptions, IConfiguration> configure, Func<IEvent, bool> predicate, Action<RabbitMQPublisherBuilder> build)
            : this(id, name, RabbitMQDefaults.Key, configure, predicate, build)
        {

        }

        public RabbitMQPublisherEnumeration(int id, string name, string rabbitKey, Action<RabbitMQPublisherOptions, IConfiguration> configure, Func<IEvent, bool> predicate, Action<RabbitMQPublisherBuilder> build) 
            : base(id, name)
        {
            RabbitKey = rabbitKey;
            Predicate = predicate;
            Configure = configure;
            Build = build;
        }

    }
}
