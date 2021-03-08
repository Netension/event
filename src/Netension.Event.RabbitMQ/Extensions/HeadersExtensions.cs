using Netension.Event.Defaults;
using Netension.Extensions.Correlation.Defaults;
using System.Text;

namespace System.Collections.Generic
{
    public static class HeadersExtensions
    {
        public static string GetMessageType(this IDictionary<string, object> headers)
        {
            object result;
            if (headers == null || !headers.TryGetValue(EventDefaults.MessageType, out result)) throw new InvalidOperationException($"{EventDefaults.MessageType} header does not present");
            if (result == null) throw new InvalidOperationException($"{EventDefaults.MessageType} header has not present");

            return Encoding.UTF8.GetString((byte[])result);
        }

        public static void SetMessageType(this IDictionary<string, object> headers, string value)
        {
            headers.Add(EventDefaults.MessageType, value);
        }

        public static Guid GetCorrelationId(this IDictionary<string, object> headers)
        {
            return Guid.Parse(headers[CorrelationDefaults.CorrelationId].ToString().AsSpan());
        }

        public static void SetCorrelationId(this IDictionary<string, object> headers, Guid value)
        {
            if (value.Equals(Guid.Empty)) throw new InvalidOperationException($"{CorrelationDefaults.CorrelationId} is required.");
            headers.Add(CorrelationDefaults.CorrelationId, value.ToString());
        }

        public static Guid? GetCausationId(this IDictionary<string, object> headers)
        {
            return Guid.Parse(headers[CorrelationDefaults.CausationId].ToString().AsSpan());
        }

        public static void SetCausationId(this IDictionary<string, object> headers, Guid? value)
        {
            headers.Add(CorrelationDefaults.CausationId, value.ToString());
        }
    }
}
