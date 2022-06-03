using MA.BlazorRest.Src.Models;
using System.Threading.Tasks;

namespace MA.BlazorRest.Src.Interceptors
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
