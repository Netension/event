using Microsoft.Extensions.Hosting;

namespace Netension.Event.Hosting.LightInject.Registers
{
    public class EventListenerRegister
    {
        public IHostBuilder Builder { get; }

        public EventListenerRegister(IHostBuilder builder)
        {
            Builder = builder;
        }
    }
}
