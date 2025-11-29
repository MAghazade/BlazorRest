using System.Net.Http;

namespace BlazorRest.BlazorRest.Interceptors
{
    public class DefaultRequestInterceptor : IRequestInterceptor
    {
        public HttpRequestMessage InterceptRequest(HttpRequestMessage request)
            => request;
    }
}