using System.Text;
using System.Text.Json;

namespace Netension.Event.Abstraction
{
    public static class EventExtensions
    {
        public static byte[] Encode<TEvent>(this TEvent @event, JsonSerializerOptions options)
            where TEvent : IEvent
        {
            return Encoding.UTF8.GetBytes(JsonSerializer.Serialize(@event, options));
        }
    }
}
