using MA.BlazorRest.Src.Contracts;
using MA.BlazorRest.Src.Interceptors;
using MA.BlazorRest.Src.Responses;
using Microsoft.Extensions.Options;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace MA.BlazorRest.Src.Core
{
    internal class BlazorRest : BaseBlazorRest, IBlazorRest
    {
        public BlazorRest(
                HttpClient httpClient,
                IOptions<BlazorRestOptions> options,
                IJwtService? jwt = default,
                IRequestInterceptor? requestInterceptor = default,
                IResponseInterceptor? responseInterceptor = default,
                IErrorInterceptor? errorInterceptor = default)
                : base(httpClient, options, jwt, requestInterceptor, responseInterceptor, errorInterceptor) { }
        /// <summary>
        /// send http request with basic response type
        /// </summary>
        /// <param name="message"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public Task<BaseResponse> SendAsync(IBlazorRestMessage message, CancellationToken cancellationToken = default)
        {
            if (message is null) throw new ArgumentNullException(nameof(message));
            return SendBaseAsync(CreateMessage(message), cancellationToken);
        }


        /// <summary>
        ///  send http request with specific response type
        /// </summary>
        /// <typeparam name="TResponse"></typeparam>
        /// <param name="message"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public Task<BaseResponse<TResponse>> SendAsync<TResponse>(IBlazorRestMessage message, CancellationToken cancellationToken = default)
        {
            if (message is null) throw new ArgumentNullException(nameof(message));
            return base.SendAsync<TResponse>(CreateMessage(message), message.SerilizerOption ?? JsonSerializerOptions, cancellationToken);
        }
    }
}
