using LightInject;
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
            build(new EventingBuilder(builder));

            return builder;
        }
    }
}
