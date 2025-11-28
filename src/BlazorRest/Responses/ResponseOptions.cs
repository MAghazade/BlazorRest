using System.Text;
using System.Text.Json;


namespace BlazorRest.Responses
{
    public class ResponseOptions
    {

        public JsonSerializerOptions SerializerOptions { get; set; } = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public Encoding Encoding { get; set; } = Encoding.UTF8;
    }
}
