using System.Net.Http;

namespace MA.BlazorRest.Src
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
