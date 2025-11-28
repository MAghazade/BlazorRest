using System;
using System.Net;

namespace BlazorRest.Models
{
    public class ErrorInterceptorModel
    {
        public string? Content { get; set; }
        public HttpStatusCode? StatusCode { get; set; }
        public Exception? Exception { get; set; }
    }
}
