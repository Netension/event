using LightInject;
using Microsoft.Extensions.Hosting;
using Netension.Event.Abstraction;
using Netension.Event.Hosting.LightInject.Registers;
using Netension.Event.Publishers;
using System;

namespace Netension.Event.Hosting.Builders
{
    public class EventingBuilder
    {
        public IHostBuilder HostBuilder { get; }

        public EventingBuilder(IHostBuilder hostBuilder)
        {
            HostBuilder = hostBuilder;
        }

        public void RegistrateEventHandlers(Action<HandlerRegister> registrate)
        {
            registrate(new HandlerRegister(HostBuilder));
        }
    }
}
