using Netension.Event.Defaults;
using System;
using System.Collections.Generic;

namespace RabbitMQ.Client.Events
{
    public static class HeadersExtensions
    {
        public static string GetMessageType(this IDictionary<string, object> headers)
        {
            object result;
            if (!headers.TryGetValue(EventDefaults.MessageType, out result)) throw new InvalidOperationException($"{EventDefaults.MessageType} header does not present");

            return (string)result;
        }
    }
}
