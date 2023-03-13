using MA.BlazorRest.Src.Contracts;
using MA.BlazorRest.Src.Interceptors;
using MA.BlazorRest.Src.Responses;
using Microsoft.Extensions.Options;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using MA.BlazorRest.M.A.BlazorRest.Responses;
using MA.BlazorRest.Src.Helpers;

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
            return SendVoidAsync(CreateMessage(message), cancellationToken);
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

            return SendAsync<TResponse>(CreateMessage(message), new(), cancellationToken);
        }

        public Task<BaseResponse<TResponse>> SendAsync<TResponse>(IBlazorRestMessage message, ResponseOptions responseOptions,
            CancellationToken cancellationToken = default)
        {
            if (message is null) throw new ArgumentNullException(nameof(message));
            if (responseOptions is null) throw new ArgumentNullException(nameof(responseOptions));

            return SendAsync<TResponse>(CreateMessage(message), responseOptions, cancellationToken);
        }


        /// <summary>
        /// Sends Get Request With Default response options
        /// </summary>
        /// <typeparam name="TResponse"></typeparam>
        /// <param name="url"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public Task<BaseResponse<TResponse>> GetAsync<TResponse>(string url, CancellationToken cancellationToken = default)
        {
            if (url is null) throw new ArgumentNullException(nameof(url));

            return SendAsync<TResponse>(CreateMessage(CrreateDefaultMessage(url)), new(), cancellationToken);
        }

        /// <summary>
        /// Sends Get Request With  response options
        /// </summary>
        /// <typeparam name="TResponse"></typeparam>
        /// <param name="url"></param>
        /// <param name="responseOptions"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public Task<BaseResponse<TResponse>> GetAsync<TResponse>(string url, ResponseOptions responseOptions, CancellationToken cancellationToken = default)
        {
            if (url is null) throw new ArgumentNullException(nameof(url));

            return SendAsync<TResponse>(CreateMessage(CrreateDefaultMessage(url)), responseOptions, cancellationToken);
        }


        private IBlazorRestMessage CrreateDefaultMessage(string url)
        {
            if (UriHelper.IsAbsoluteUrl(url))
                return new BlazorRestMessage(new Uri(url), HttpMethod.Get);

            return new BlazorRestMessage(url, HttpMethod.Get);
        }
    }
}
