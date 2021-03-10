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
            if (!headers.ContainsKey(CorrelationDefaults.CorrelationId)) throw new InvalidOperationException($"{CorrelationDefaults.CorrelationId} header was not present.");
            return Guid.Parse(Encoding.UTF8.GetString((byte[])headers[CorrelationDefaults.CorrelationId]).AsSpan());
        }

        public static void SetCorrelationId(this IDictionary<string, object> headers, Guid value)
        {
            if (value.Equals(Guid.Empty)) throw new InvalidOperationException($"{CorrelationDefaults.CorrelationId} is required.");
            headers.Add(CorrelationDefaults.CorrelationId, value.ToString());
        }

        public static Guid? GetCausationId(this IDictionary<string, object> headers)
        {
            if (!headers.ContainsKey(CorrelationDefaults.CausationId)) return null;

            var value = (byte[])headers[CorrelationDefaults.CausationId];
            if (value is null || value.Length == 0) return null;

            return Guid.Parse(Encoding.UTF8.GetString(value));
        }

        public static void SetCausationId(this IDictionary<string, object> headers, Guid? value)
        {
            string causationId = null;
            if (value.HasValue) causationId = value.Value.ToString();
            headers.Add(CorrelationDefaults.CausationId, causationId);
        }
    }
}
