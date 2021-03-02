namespace Netension.Event.Hosting.LightInject.RabbitMQ.Defaults
{
    public static class RabbitMQDefaults
    {
        public const string Key = "rabbitmq";
        public static readonly string Listener = $"{Key}-Default";

        public static class Connections
        {
            public static readonly string ListenerSuffix = "listener";
            public static readonly string PublisherSuffix = "publisher";
        }
    }
}
