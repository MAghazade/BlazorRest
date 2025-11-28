using BlazorRest.Models;
using System.Threading.Tasks;

namespace BlazorRest.Interceptors
{
   public interface IErrorInterceptor
    { 
        /// <summary>
        /// 
        /// </summary>
        /// <param name="error"></param>
        /// <returns></returns>
        Task InterceptError(ErrorInterceptorModel? error);
    }

}
