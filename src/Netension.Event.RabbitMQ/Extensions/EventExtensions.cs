using System.Text;
using System.Text.Json;

namespace Netension.Event.Abstraction
{
    public static class EventExtensions
    {
        public static byte[] Encode(this IEvent @event, JsonSerializerOptions options)
        {
            return Encoding.UTF8.GetBytes(JsonSerializer.Serialize(@event, options));
        }
    }
}
