using System.ComponentModel.DataAnnotations;
using System.Security;

namespace Netension.Event.RabbitMQ.Options
{
    public class RabbitMQOptions
    {
        [Required]
        public string Host { get; set; }
        public int Port { get; set; } = 5672;
        [Required]
        public string UserName { get; set; }
        [Required]
        public SecureString Password { get; set; }
    }
}
