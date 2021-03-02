using Microsoft.Extensions.Hosting;

namespace Netension.Event.Hosting.LightInject.Registers
{
    public class EventPublisherRegister
    {
        public IHostBuilder Builder { get; }

        public EventPublisherRegister(IHostBuilder builder)
        {
            Builder = builder;
        }
    }
}
