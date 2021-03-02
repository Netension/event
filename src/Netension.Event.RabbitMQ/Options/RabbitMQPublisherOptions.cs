using System.ComponentModel.DataAnnotations;

namespace Netension.Event.RabbitMQ.Options
{
    public class RabbitMQPublisherOptions
    {
        [Required]
        public string Exchange { get; set; }
        public string RoutingKey { get; set; } = string.Empty;
        public bool Mandatory { get; set; }
    }
}
