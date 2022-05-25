using System.Net.Http;

namespace MA.BlazorRest.Src.Interceptors
{
    public interface IResponseInterceptor
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>
        HttpResponseMessage IntercepteResponse(HttpResponseMessage response);
    }
}
