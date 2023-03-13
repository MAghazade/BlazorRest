
using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;

namespace MA.BlazorRest.Src.RequestContents
{
    public class JsonContent : IRestContent
    {
        public object Model { get; private set; }

        public JsonSerializerOptions? SerializerOption { get; set; } = new()
        {
            PropertyNameCaseInsensitive = true
        };


        /// <summary>
        /// encoding of serialization and deserialization
        /// </summary>
        public Encoding Encoding { get; set; } = Encoding.UTF8;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        public JsonContent(object model)
        {
            Model = model ?? throw new ArgumentNullException(nameof(model));
        }

        public HttpContent GetHttpContent()
        {
            if (SerializerOption == null)
                throw new NullReferenceException($"{nameof(SerializerOption)} Cannot be Null");

            if (Encoding == null)
                throw new NullReferenceException($"{nameof(Encoding)} Cannot be Null");

            return new StringContent(JsonSerializer.Serialize(Model, SerializerOption), Encoding, "application/json");
        }
    }
}
