using Newtonsoft.Json;
using System;
using System.Text;

namespace Netension.Event.Test.Extensions
{
    public static class RabbitMQExtensions
    {
        public static ReadOnlyMemory<byte> Encode(this Event @event)
        {
            return new ReadOnlyMemory<byte>(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(@event)));
        }
    }
}
