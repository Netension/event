using Microsoft.Extensions.Hosting;

namespace Netension.Event.Hosting.LightInject.Registers
{
    public class EventPublisherRegister
    {
        public IHostBuilder Builder { get; }
        public Containers.EventPublisherRegister Register { get; }

        public EventPublisherRegister(IHostBuilder builder, Containers.EventPublisherRegister register)
        {
            Builder = builder;
            Register = register;
        }
    }
}
