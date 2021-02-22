namespace Netension.Event.Hosting.LightInject.RabbitMQ.Defaults
{
    public static class RabbitMQDefaults
    {
        public const string Key = "rabbitmq";
        public static readonly string Receiver = $"{Key}-Default";

        public static class Prefixes
        {
            public static readonly string Listener = $"-Listener";
        }
    }
}
