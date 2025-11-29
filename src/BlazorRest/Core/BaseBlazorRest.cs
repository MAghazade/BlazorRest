using BlazorRest.Interceptors;
using BlazorRest.Models;
using BlazorRest.Responses;
using BlazorRest.Helpers;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using System;

namespace BlazorRest
{
    internal abstract class BaseBlazorRest
    {
        private readonly HttpClient _httpClient;
        private readonly IJwtService? _jwt;
        private readonly IErrorInterceptor _errorInterceptor;
        private readonly IResponseInterceptor _responseInterceptor;
        private readonly IRequestInterceptor _requestInterceptor;

        protected BaseBlazorRest(
            HttpClient httpClient,
            IOptions<BlazorRestOptions> options,
            IRequestInterceptor requestInterceptor,
            IResponseInterceptor responseInterceptor,
            IErrorInterceptor errorInterceptor,
            IJwtService? jwt = null
        )
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            if (options is null) throw new ArgumentNullException(nameof(options));

            _jwt = jwt;
            _requestInterceptor = requestInterceptor;
            _responseInterceptor = responseInterceptor;
            _errorInterceptor = errorInterceptor;
        }

        protected async Task<BaseResponse<TResponse>> SendAsync<TResponse>(
            HttpRequestMessage httpRequestMessage,
            ResponseOptions? responseOptions = null,
            CancellationToken cancellationToken = default)
        {
            string? content = null;
            HttpResponseMessage? response = null;

            try
            {
                response = await SendAsync(httpRequestMessage, cancellationToken);

                if (response is null)
                {
                    await _errorInterceptor.InterceptError(new ErrorInterceptorModel { Content = "response is null" });
                    return new BaseResponse<TResponse> { IsSuccessful = false, ErrorMessage = "response is null" };
                }

                content = await response.Content.ReadAsStringAsync(cancellationToken);

                if (string.IsNullOrWhiteSpace(content))
                {
                    await _errorInterceptor.InterceptError(new ErrorInterceptorModel
                        { Content = "response content is null", StatusCode = response.StatusCode });
                    return new BaseResponse<TResponse>
                    {
                        IsSuccessful = false, ErrorMessage = "response content is null",
                        StatusCode = response.StatusCode
                    };
                }

                var contentType = response.Content.Headers.ContentType?.MediaType;

                var data = ResponseParser.Parse<TResponse>(
                    content,
                    contentType,
                    responseOptions?.SerializerOptions
                );

                if (!response.IsSuccessStatusCode)
                {
                    await _errorInterceptor.InterceptError(new ErrorInterceptorModel
                    {
                        Content = content,
                        StatusCode = response.StatusCode
                    });
                }

                return new BaseResponse<TResponse>
                {
                    StatusCode = response.StatusCode,
                    IsSuccessful = response.IsSuccessStatusCode,
                    Data = data,
                    Content = content
                };
            }
            catch (Exception ex)
            {
                await _errorInterceptor.InterceptError(new ErrorInterceptorModel
                {
                    Exception = ex,
                    Content = content,
                    StatusCode = response?.StatusCode
                });

                return new BaseResponse<TResponse>
                {
                    IsSuccessful = false,
                    ErrorMessage = ex.Message,
                    Content = content,
                    StatusCode = response?.StatusCode
                };
            }
        }

        protected async Task<HttpResponseMessage?> SendAsync(HttpRequestMessage httpRequestMessage,
            CancellationToken cancellationToken = default)
        {
            if (_jwt != null)
            {
                var token = await _jwt.GetTokenAsync();
                if (token != null)
                    httpRequestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            httpRequestMessage = _requestInterceptor.InterceptRequest(httpRequestMessage);

            HttpResponseMessage? response = null;

            try
            {
                response = await _httpClient.SendAsync(httpRequestMessage, cancellationToken);
            }
            catch (Exception ex)
            {
                await _errorInterceptor.InterceptError(new ErrorInterceptorModel { Exception = ex });
            }

            if (response == null) return null;

            response = _responseInterceptor.InterceptResponse(response);

            if (!response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync(cancellationToken);
                await _errorInterceptor.InterceptError(new ErrorInterceptorModel
                {
                    Content = content,
                    StatusCode = response.StatusCode
                });
            }

            return response;
        }

        protected async Task<BaseResponse> SendVoidAsync(HttpRequestMessage httpRequestMessage,
            CancellationToken cancellationToken = default)
        {
            var response = await SendAsync(httpRequestMessage, cancellationToken);
            string? content = response?.Content != null
                ? await response.Content.ReadAsStringAsync(cancellationToken)
                : null;

            return new BaseResponse
            {
                IsSuccessful = response?.IsSuccessStatusCode ?? false,
                StatusCode = response?.StatusCode,
                Content = content
            };
        }

        protected HttpRequestMessage CreateMessage(IBlazorRestMessage message)
        {
            if (message is null) throw new ArgumentNullException(nameof(message));
            return HttpMessageHelper.CreateHttpRequestMessage(message, _httpClient);
        }
    }
}