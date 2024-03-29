﻿using MA.BlazorRest.Src.Contracts;
using MA.BlazorRest.Src.Interceptors;
using MA.BlazorRest.Src.Models;
using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;
using MA.BlazorRest.Src.Responses;
using System.Threading.Tasks;
using System.Net.Http;
using System.Threading;
using Microsoft.Extensions.Options;
using System;
using MA.BlazorRest.Src.Helpers;
using MA.BlazorRest.M.A.BlazorRest.Responses;

namespace MA.BlazorRest.Src.Core
{

    internal abstract class BaseBlazorRest
    {
        protected readonly HttpClient HttpClient;
        private readonly IJwtService? _jwt;
        private readonly IErrorInterceptor? _errorInterceptor;
        private readonly IResponseInterceptor? _responseInterceptor;
        private readonly IRequestInterceptor? _requestInterceptor;

        protected BaseBlazorRest(
             HttpClient httpClient,
             IOptions<BlazorRestOptions> options,
             IJwtService? jwt = default,
             IRequestInterceptor? requestInterceptor = default,
             IResponseInterceptor? responseInterceptor = default,
            IErrorInterceptor? errorInterceptor = default)
        {
            if (options is null)
            {
                throw new ArgumentNullException(nameof(options));
            }
            HttpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _jwt = jwt;
            _responseInterceptor = responseInterceptor;
            _errorInterceptor = errorInterceptor;
            _requestInterceptor = requestInterceptor;
        }


        /// <summary>
        ///   send http request
        /// </summary>
        /// <typeparam name="TResponse"></typeparam>
        /// <param name="httpRequestMessage"></param>
        /// <param name="responseOptions"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        protected async Task<BaseResponse<TResponse>> SendAsync<TResponse>(HttpRequestMessage httpRequestMessage, ResponseOptions? responseOptions, CancellationToken cancellationToken = default)
        {
            var response = await SendAsync(httpRequestMessage, cancellationToken);

            if (response is null)
            {
                return new BaseResponse<TResponse>()
                {
                    IsSuccessful = false
                };
            }

            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            var data = JsonSerializer.Deserialize<TResponse>(content, responseOptions?.SerializerOptions ?? new());


            return new BaseResponse<TResponse>()
            {
                StatusCode = response.StatusCode,
                IsSuccessful = response.StatusCode == HttpStatusCode.OK,
                Data = data,
                Content = content
            };
        }


        /// <summary>
        /// send http request
        /// </summary>
        /// <param name="httpRequestMessage"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        protected async Task<HttpResponseMessage?> SendAsync(HttpRequestMessage httpRequestMessage, CancellationToken cancellationToken = default)
        {
            if (_jwt is not null)
            {
                string? jwtToken = await _jwt.GetTokenAsync();

                if (jwtToken is not null)
                {
                    httpRequestMessage.Headers.Authorization =
                        new AuthenticationHeaderValue("Bearer", jwtToken);
                }
            }

            if (_requestInterceptor is not null)
            {
                httpRequestMessage = _requestInterceptor.InterceptRequest(httpRequestMessage);
            }

            HttpResponseMessage? response = null;

            try
            {
                response = await HttpClient.SendAsync(httpRequestMessage, cancellationToken);
            }
            catch (Exception e)
            {
                if (_errorInterceptor is not null)
                {
                    await _errorInterceptor.InterceptError(new ErrorInterceptorModel
                    {
                        Exception = e
                    });
                }
                else
                {
                    throw;
                }
            }

            if (response is null) return response;


            if (_responseInterceptor is not null)
            {
                response = _responseInterceptor.InterceptResponse(response);
            }

            if (_errorInterceptor is not null)
            {
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    await _errorInterceptor.InterceptError(new ErrorInterceptorModel
                    {
                        Content = await response.Content.ReadAsStringAsync(cancellationToken),
                        StatusCode = response.StatusCode
                    });
                }
            }

            return response;
        }

        /// <summary>
        ///  send http request
        /// </summary>
        /// <param name="httpRequestMessage"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        protected async Task<BaseResponse> SendVoidAsync(HttpRequestMessage httpRequestMessage, CancellationToken cancellationToken = default)
        {
            var response = await SendAsync(httpRequestMessage, cancellationToken);

            return new BaseResponse
            {
                IsSuccessful = response?.StatusCode is HttpStatusCode.OK,
                StatusCode = response?.StatusCode,
                Content = await response?.Content?.ReadAsStringAsync(cancellationToken)!,
            };
        }


        /// <summary>
        /// Create HttpRequest Message
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        protected HttpRequestMessage CreateMessage(IBlazorRestMessage message)
        {
            if (message is null) throw new ArgumentNullException(nameof(message));

            return HttpMessageHelper.CreateHttpRequestMessage(message, HttpClient);
        }
    }
}
