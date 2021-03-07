using Netension.Event.Abstraction;

namespace Netension.Event.Extensions
{
    public static class EventExtensions
    {
        public static string GetMessageType(this IEvent @event)
        {
            return $"{@event.GetType().FullName}, {@event.GetType().Assembly.GetName().Name}";
        }
    }
}
