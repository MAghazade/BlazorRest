using System;
using System.Net;

namespace MA.BlazorRest.Src.Models
{
    public class ErrorInterceptorModel
    {
        public string? Content { get; set; }
        public HttpStatusCode? StatusCode { get; set; }
        public Exception? Exception { get; set; }
    }
}
