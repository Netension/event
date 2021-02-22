using Netension.Event.Defaults;
using System;
using System.Collections.Generic;
using System.Text;

namespace RabbitMQ.Client.Events
{
    public static class HeadersExtensions
    {
        public static string GetMessageType(this IDictionary<string, object> headers)
        {
            object result;
            if (headers == null || !headers.TryGetValue(EventDefaults.MessageType, out result)) throw new InvalidOperationException($"{EventDefaults.MessageType} header does not present");
            if (result == null) throw new InvalidOperationException($"{EventDefaults.MessageType} header does not present");

            return Encoding.UTF8.GetString((byte[])result);
        }
    }
}
