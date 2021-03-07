using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Netension.Event.RabbitMQ.Options
{
    public class RabbitMQListenerOptions
    {
        /// <summary>
        /// Options for listener queue.
        /// </summary>
        [Required]
        public QueueOptions Queue { get; set; }

        /// <summary>
        /// Options for bindings.
        /// </summary>
        [Required]
        public IEnumerable<BindingOptions> Bindings { get; set; }
    }

    public class QueueOptions
    {
        /// <summary>
        /// Name of the queue.
        /// </summary>
        [Required]
        public string Name { get; set; }

        /// <summary>
        /// The queue will survive a broker restart. (Default: true)
        /// </summary>
        public bool Durable { get; set; } = true;

        /// <summary>
        /// The queue used by only one connection and the queue will be deleted when that connection closes. (Default: false)
        /// </summary>
        public bool Exclusive { get; set; } = false;

        /// <summary>
        /// The queue has had at least one consumer. It is deleted when last consumer unsubscribes. (Default: false)
        /// </summary>
        public bool AutoDelete { get; set; } = false;

        /// <summary>
        /// Prefix of the consumer tag.
        /// </summary>
        public string ConsumerPrefix { get; set; } = string.Empty;

        public bool AutoAck { get; set; }

        /// <summary>
        /// If the no-local field is set the server will not send messages to the connection that published them. Default: false.
        /// </summary>
        public bool NoLocal { get; set; } = false;

        /// <summary>
        /// Used by plugins and broker-specific features such as message TTL, queue length limit, etc.
        /// </summary>
        public IDictionary<string, object> Arguments { get; set; } = new Dictionary<string, object>();
    }

    public class BindingOptions
    {
        /// <summary>
        /// Name of the exhange.
        /// </summary>
        [Required]
        public string Exchange { get; set; }

        /// <summary>
        /// Routing key of the binding.
        /// </summary>
        public string RoutingKey { get; set; } = string.Empty;

        public IDictionary<string, object> Arguments { get; set; } = new Dictionary<string, object>();
    }
}
