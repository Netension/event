using System.ComponentModel.DataAnnotations;

namespace Netension.Event.Hosting.LightInject.RabbitMQ.Options
{
    public class RabbitMQListenerOptions
    {
        [Required]
        public string Exchange { get; set; }
        [Required]
        public string Queue { get; set; }
        public string Tag { get; set; }
    }
}
