using Netension.Event.Abstraction;
using System;
using System.Text.Json;

namespace RabbitMQ.Client.Events
{
    public static class BodyExtensions
    {
        public static IEvent Decode(this ReadOnlyMemory<byte> body, Type type)
        {
            return (IEvent)JsonSerializer.Deserialize(body.Span, type);
        }
    }
}
