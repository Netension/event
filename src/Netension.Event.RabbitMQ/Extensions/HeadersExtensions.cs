using Netension.Event.Defaults;
using System.Collections.Generic;

namespace RabbitMQ.Client.Events
{
    public static class HeadersExtensions
    {
        public static string GetMessageType(this IDictionary<string, object> headers)
        {
            object result;
            headers.TryGetValue(EventDefaults.MessageType, out result);

            return (string)result;
        }
    }
}
