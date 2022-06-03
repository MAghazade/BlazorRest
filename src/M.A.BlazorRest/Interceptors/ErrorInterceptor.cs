using MA.BlazorRest.Src.Models;
using System;
using System.Threading.Tasks;
using System.Net;

namespace MA.BlazorRest.Src.Interceptors
{
    internal class ErrorInterceptor : IErrorInterceptor
    {
        public Task InterceptError(ErrorInterceptorModel? error)
        {
            if (error is null) return Task.CompletedTask;

            if (error.Exception is not null)
            {
                throw new Exception("Network Error!,See Inner Exception For More Details", error.Exception);
            }

            switch (error.StatusCode)
            {
                case HttpStatusCode.Unauthorized: throw new Exception("Unauthorized Error!,See Inner Exception For More Details", new Exception(error.Content));
                case HttpStatusCode.InternalServerError: throw new Exception("Internal Server Error!,See Inner Exception For More Details", new Exception(error.Content));
                case HttpStatusCode.BadRequest: throw new Exception("Bad Request,See Inner Exception For More Details", new Exception(error.Content));
                default: throw new Exception(error.StatusCode.ToString(), new Exception(error.Content));
            }
        }
    }
}
