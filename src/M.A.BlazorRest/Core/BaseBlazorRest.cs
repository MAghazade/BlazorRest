using MA.BlazorRest.Src.Contracts;
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
using MA.BlazorRest.Src.RequestContents;
using MA.BlazorRest.Src.Helpers;
using System.Linq;

namespace MA.BlazorRest.Src.Core
{

    public abstract class BaseBlazorRest
    {
        protected readonly HttpClient HttpClient;
        protected readonly JsonSerializerOptions JsonSerializerOptions;
        private readonly IJwtService? _jwt;
        private readonly IErrorInterceptor? _errorInterceptor;
        private readonly IResponseInterceptor? _responseInterceptor;
        private readonly IRequestInterceptor? _requestInterceptor;

        public BaseBlazorRest(
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

            JsonSerializerOptions = options.Value.jsonSerializerOptions;
            HttpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _jwt = jwt;
            _responseInterceptor = responseInterceptor;
            _errorInterceptor = errorInterceptor;
            _requestInterceptor = requestInterceptor;
        }


        /// <summary>
        /// send http request
        /// </summary>
        /// <typeparam name="TResponse"></typeparam>
        /// <param name="httpRequestMessage"></param>
        /// <param name="SerializerOptions"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        protected async Task<BaseResponse<TResponse>> SendAsync<TResponse>(HttpRequestMessage httpRequestMessage, JsonSerializerOptions SerializerOptions, CancellationToken cancellationToken = default)
        {
            var response = await SendAsync(httpRequestMessage, cancellationToken);
            var content = await response.Content.ReadAsStringAsync();
            var data = JsonSerializer.Deserialize<TResponse>(content, SerializerOptions ?? JsonSerializerOptions);

            return new BaseResponse<TResponse>()
            {
                StatuseCode = response.StatusCode,
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
        protected async Task<HttpResponseMessage> SendAsync(HttpRequestMessage httpRequestMessage, CancellationToken cancellationToken = default)
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
                httpRequestMessage = _requestInterceptor.IntercepteRequest(httpRequestMessage);
            }

            var response = await HttpClient.SendAsync(httpRequestMessage, cancellationToken);

            if (_responseInterceptor is not null)
            {
                response = _responseInterceptor.IntercepteResponse(response);
            }

            if (_errorInterceptor is not null)
            {
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    await _errorInterceptor.IntercepteError(new ErrorInterceptorModel
                    {
                        Content = await response.Content.ReadAsStringAsync(),
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
        protected async Task<BaseResponse> SendBaseAsync(HttpRequestMessage httpRequestMessage, CancellationToken cancellationToken = default)
        {
            var response = await SendAsync(httpRequestMessage, cancellationToken);

            return new BaseResponse
            {
                IsSuccessful = response.StatusCode is HttpStatusCode.OK,
                StatuseCode = response.StatusCode,
                Content = await response.Content.ReadAsStringAsync(),
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
            var httprequestMessage = new HttpRequestMessageBuilder()
               .AddUri(UriHelper.GetFinalUri(message, HttpClient))
               .AddHeaders(message.Headers)
               .AddMethod(message.Method)
               .AddQuery(message.QueryParmeters);

            if (message.Content is JsonContent messageContent)
            {
                if (messageContent is null) throw new Exception("Content Cannot Be Null Here!");

                httprequestMessage.AddJsonContent(messageContent, message.Encoding,
                    message.SerilizerOption ?? JsonSerializerOptions);
            }
            else if (message.Content is FileWithModelContent fileWithModelContent)
            {
                if (fileWithModelContent?.Model is null) throw new Exception("Model Cannot Be Null Here!");

                httprequestMessage.AddFilesWithModel(fileWithModelContent.Files, fileWithModelContent.Model);
            }
            else if (message.Content is FileContent fileContent)
            {
                if (fileContent is null) throw new Exception("fileContent Cannot Be Null Here!");

                if (!fileContent.Files.Any()) throw new Exception("No files were added to the request");

                httprequestMessage.AddFiles(fileContent.Files);
            }


            return httprequestMessage.Build();
        }
    }
}
