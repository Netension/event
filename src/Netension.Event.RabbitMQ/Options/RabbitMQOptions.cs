using Netension.Extensions.Security;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Security;
using System.Text.Json.Serialization;

namespace Netension.Event.RabbitMQ.Options
{
    [ExcludeFromCodeCoverage]
    public class RabbitMQOptions
    {
        [Required]
        public string Host { get; set; }
        public int Port { get; set; } = 5672;
        [Required]
        public string UserName { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
