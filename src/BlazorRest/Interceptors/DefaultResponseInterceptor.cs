using System.Net.Http;
using BlazorRest.Interceptors;

namespace BlazorRest.BlazorRest.Interceptors
{
    public class DefaultResponseInterceptor : IResponseInterceptor
    {
        public HttpResponseMessage InterceptResponse(HttpResponseMessage response)
            => response;
    }
}