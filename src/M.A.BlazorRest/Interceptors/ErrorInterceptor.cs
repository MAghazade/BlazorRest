using MA.BlazorRest.Src.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;

namespace MA.BlazorRest.Src.Interceptors
{
    internal class ErrorInterceptor : IErrorInterceptor
    {
        public Task IntercepteError(ErrorInterceptorModel? error)
        {
            if (error is null)
            {
                return Task.CompletedTask;
            }

            if (error.StatusCode == HttpStatusCode.Unauthorized)
            {
                throw new Exception("Unauthorized Error!,See Inner Exception For More Details", new Exception(error.Content));
            }

            if (error.StatusCode == HttpStatusCode.InternalServerError)
            {
                throw new Exception("Internal Server Error!,See Inner Exception For More Details", new Exception(error.Content));
            }

            if (error.StatusCode == HttpStatusCode.BadRequest)
            {
                throw new Exception("Bad Request,See Inner Exception For More Details", new Exception(error.Content));
            }

            return Task.CompletedTask;
        }
    }
}
