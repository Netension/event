using System.ComponentModel.DataAnnotations;

namespace Netension.Event.RabbitMQ.Options
{
    public class RabbitMQReceiverOptions
    {
        [Required]
        public string Exchange { get; set; }
        [Required]
        public string Queue { get; set; }
    }
}
