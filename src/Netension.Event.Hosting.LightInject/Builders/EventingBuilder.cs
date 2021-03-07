using LightInject;
using Microsoft.Extensions.Hosting;
using Netension.Event.Abstraction;
using Netension.Event.Containers;
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

        public void RegistrateEventPublishers(Action<LightInject.Registers.EventPublisherRegister> registrate)
        {
            var eventPublisherCollection = new EventPublisherCollection();

            HostBuilder.ConfigureContainer<IServiceContainer>((context, container) =>
            {
                container.RegisterScoped<IEventPublisher, EventPublisher>();
                container.RegisterTransient<IEventPublisherResolver, EventPublisherResolver>();
                container.RegisterInstance(eventPublisherCollection);
            });

            registrate(new LightInject.Registers.EventPublisherRegister(HostBuilder, new Containers.EventPublisherRegister(eventPublisherCollection)));
        }

        public void RegistrateEventListeners(Action<EventListenerRegister> registrate)
        {
            registrate(new EventListenerRegister(HostBuilder));
        }
    }
}
