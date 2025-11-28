using System.Net.Http;

namespace BlazorRest.Interceptors
{
    public interface IResponseInterceptor
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>
        HttpResponseMessage InterceptResponse(HttpResponseMessage response);
    }
}
