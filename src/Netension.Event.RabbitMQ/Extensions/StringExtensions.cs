using System;

namespace Netension.Event.RabbitMQ.Extensions
{
    public static class StringExtensions
    {
        public static string NewConsumerTag(this string prefix)
        {
            return $"{prefix}-{Guid.NewGuid()}".TrimStart('-');
        }
    }
}
