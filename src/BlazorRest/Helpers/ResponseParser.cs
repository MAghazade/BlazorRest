using System;
using System.IO;
using System.Text.Json;
using System.Xml.Serialization;

namespace BlazorRest
{
    internal static class ResponseParser
    {
        private static readonly JsonSerializerOptions DefaultJsonSerializerOptions = new()
        {
            PropertyNameCaseInsensitive = true,
        };


        /// <summary>
        /// Parses the raw content string into the desired TResponse type based on content type and TResponse type.
        /// Supports JSON, XML, string, and value types (int, bool, DateTime, etc.).
        /// </summary>
        public static TResponse? Parse<TResponse>(string content, string? contentType = null,
            JsonSerializerOptions? jsonOptions = null)
        {
            if (string.IsNullOrWhiteSpace(content))
                return default;

            contentType = contentType?.ToLowerInvariant();

            try
            {
                if (contentType != null && contentType.Contains("json", StringComparison.InvariantCultureIgnoreCase))
                {
                    return JsonSerializer.Deserialize<TResponse>(content, jsonOptions ?? DefaultJsonSerializerOptions);
                }

                if (contentType != null && contentType.Contains("xml", StringComparison.InvariantCultureIgnoreCase))
                {
                    using var reader = new StringReader(content);
                    var serializer = new XmlSerializer(typeof(TResponse));
                    return (TResponse?)serializer.Deserialize(reader);
                }

                if (typeof(TResponse) == typeof(string))
                {
                    return (TResponse)(object)content;
                }

                if (typeof(TResponse).IsValueType)
                {
                    return (TResponse)Convert.ChangeType(content, typeof(TResponse));
                }


                return default;
            }
            catch
            {
                return default;
            }
        }
    }
}