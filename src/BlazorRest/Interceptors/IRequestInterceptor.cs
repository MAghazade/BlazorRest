using System.Net.Http;

namespace BlazorRest
{
    public interface IRequestInterceptor
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        HttpRequestMessage InterceptRequest(HttpRequestMessage request);
    }

}
