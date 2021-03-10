using Netension.Event.Hosting.LightInject.RabbitMQ.Enumerations;

namespace Netension.Event.Sample.Enumerations
{
    public static class EventingEnumerations
    {
        public static RabbitMQEnumeration RabbitMQ => new RabbitMQEnumeration(0, "RabbitMQ");

        public static class Publishers
        {
            public static RabbitMQPublisherEnumeration Publisher => new RabbitMQPublisherEnumeration(0, "default-publisher", "RabbitMQ:Publish", (builder) => builder.UseCorrelation());
        }

        public static class Listeners
        {
            public static RabbitMQListenerEnumeration Listener => new RabbitMQListenerEnumeration(0, "default-listener", "RabbitMQ:Listen", (builder) => builder.UseCorrelation());
        }
    }
}
