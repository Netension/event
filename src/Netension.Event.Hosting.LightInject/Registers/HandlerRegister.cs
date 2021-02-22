using LightInject;
using Microsoft.Extensions.Hosting;
using Netension.Event.Abstraction;
using System;
using System.Linq;
using System.Reflection;

namespace Netension.Event.Hosting.LightInject.Registers
{
    public static class TypeExtensions
    {
        public static bool IsImplementGenericInterface(this Type type, Type @interface)
        {
            return type.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition().Equals(@interface));
        }
    }

    public class HandlerRegister
    {
        private readonly IHostBuilder _hostBuilder;

        public HandlerRegister(IHostBuilder hostBuilder)
        {
            _hostBuilder = hostBuilder;
        }

        public void RegistrateHandlerFromAssemblyOf<TType>()
        {
            RegistrateHandlersFromAssembly(Assembly.GetAssembly(typeof(TType)));
        }

        public void RegistrateHandlersFromAssembly(Assembly assembly)
        {
            _hostBuilder.ConfigureContainer<IServiceContainer>((context, container) =>
            {
                container.RegisterAssembly(assembly, () => new PerScopeLifetime(), (serviceType, implementingType) => implementingType.IsImplementGenericInterface(typeof(IEventHandler<>)));
            });
        }

        public void RegistrateHandler<TEvent, THandler>()
            where TEvent : IEvent
            where THandler : IEventHandler<TEvent>
        {
            _hostBuilder.ConfigureContainer<IServiceContainer>((context, container) =>
            {
                container.RegisterScoped<IEventHandler<TEvent>, THandler>();
            });
        }
    }
}
