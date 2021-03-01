using RabbitMQ.Client;
using System.Collections.Generic;

namespace Netension.Event.RabbitMQ.Messages
{
    public class RabbitMQMessage
    {
        public byte[] Body { get; set; }
        public IDictionary<string, object> Headers { get; set; } = new Dictionary<string, object>();
    }
}
