﻿using LightInject;
using Netension.Event;
using Netension.Event.Abstraction;
using Netension.Event.Hosting.Builders;
using System;

namespace Microsoft.Extensions.Hosting
{
    public static class HostBuilderExtensions
    {
        public static IHostBuilder UseEventing(this IHostBuilder builder, Action<EventingBuilder> build)
        {
            builder.ConfigureContainer<IServiceContainer>((context, container) =>
            {
                container.RegisterScoped<IEventDispatcher, EventDispatcher>();
            });

            build(new EventingBuilder(builder));

            return builder;
        }
    }
}
