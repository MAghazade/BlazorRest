using MA.BlazorRest.Src.Contracts;
using MA.BlazorRest.Src.Interceptors;
using System;
using System.Text.Json;

namespace MA.BlazorRest.Src
{
    public class BlazorRestOptions
    {
        
        public Uri? BaseUri { get; set; }
        public Type? JwtTokenService { get; set; }
        public Type? ErrorInterceptor { get; set; }
        public Type? ResponseInterceptor { get; set; }
        public Type? RequestInterceptor { get; set; }
        public JsonSerializerOptions jsonSerializerOptions { get; set; } = new();

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public BlazorRestOptions UseJwtService<T>( ) where T : IJwtTokenService
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
            RequestInterceptor = typeof(T);
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
