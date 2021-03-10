using Microsoft.Extensions.Configuration;
using Netension.Core;
using Netension.Event.Hosting.LightInject.RabbitMQ.Defaults;
using Netension.Event.RabbitMQ.Options;
using System;

namespace Netension.Event.Hosting.LightInject.RabbitMQ.Enumerations
{
    public class RabbitMQEnumeration : Enumeration
    {
        public Action<RabbitMQOptions, IConfiguration> Configure { get; }

        public RabbitMQEnumeration(int id, string configurationSection)
            : this(id, (options, configuration) => configuration.GetSection(configurationSection).Bind(options))
        {

        }

        public RabbitMQEnumeration(int id, Action<RabbitMQOptions, IConfiguration> configure)
            : this(id, RabbitMQDefaults.Key, configure)
        {

        }

        public RabbitMQEnumeration(int id, string name, Action<RabbitMQOptions, IConfiguration> configure) 
            : base(id, name)
        {
            Configure = configure;
        }
    }
}
