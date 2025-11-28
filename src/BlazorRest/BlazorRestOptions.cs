using BlazorRest.Interceptors;
using System;

namespace BlazorRest
{
    public class BlazorRestOptions
    {
        
        public Uri? BaseUri { get; set; }
        public Type? JwtTokenService { get; set; }
        public Type? ErrorInterceptor { get; set; }
        public Type? ResponseInterceptor { get; set; }
        public Type? RequestInterceptor { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public BlazorRestOptions UseJwtService<T>( ) where T : IJwtService
        {
            JwtTokenService = typeof(T);
            return this;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public BlazorRestOptions UseErrorInterceptor<T>() where T : IErrorInterceptor
        {
            ErrorInterceptor = typeof(T);
            return this;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public BlazorRestOptions UseResponseInterceptor<T>() where T : IResponseInterceptor
        {
            ResponseInterceptor = typeof(T);
            return this;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public BlazorRestOptions UseRequestInterceptor<T>() where T : IRequestInterceptor
        {
            RequestInterceptor = typeof(T);
            return this;
        }
    }
}
