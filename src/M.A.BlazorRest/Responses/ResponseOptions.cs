using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;


namespace MA.BlazorRest.M.A.BlazorRest.Responses
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
