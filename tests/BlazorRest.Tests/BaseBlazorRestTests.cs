using BlazorRest.Interceptors;
using Microsoft.Extensions.Options;
using Moq.Protected;
using Xunit;
using Moq;
using Shouldly;
using System.Net;
using BlazorRest.BlazorRest.Interceptors;
using BlazorRest.Models;

namespace BlazorRest.Tests
{
    public class BaseBlazorRestTests
    {
        private readonly Mock<IJwtService> _jwtMock = new();
        private readonly Mock<IRequestInterceptor> _requestInterceptorMock = new();
        private readonly Mock<IResponseInterceptor> _responseInterceptorMock = new();
        private readonly Mock<IErrorInterceptor> _errorInterceptorMock = new();

        private HttpClient CreateMockHttpClient(HttpResponseMessage? response, out Mock<HttpMessageHandler> handlerMock)
        {
            handlerMock = new Mock<HttpMessageHandler>();
            handlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(response ?? new HttpResponseMessage(HttpStatusCode.OK));

            return new HttpClient(handlerMock.Object);
        }

        private BlazorRestClient CreateRestClient(
            HttpClient httpClient,
            IJwtService? jwt = null,
            IRequestInterceptor? requestInterceptor = null,
            IResponseInterceptor? responseInterceptor = null,
            IErrorInterceptor? errorInterceptor = null)
        {
            return new BlazorRestClient(
                httpClient,
                Options.Create(new BlazorRestOptions()),
                requestInterceptor ?? new DefaultRequestInterceptor(),
                responseInterceptor ?? new DefaultResponseInterceptor(),
                errorInterceptor ?? new DefaultErrorInterceptor(),
                jwt
            );
        }

        [Fact]
        public async Task JWT_Addition_Works_Correctly()
        {
            _jwtMock.Setup(j => j.GetTokenAsync()).ReturnsAsync("token123");

            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("data")
            };
            var httpClient = CreateMockHttpClient(response, out var handlerMock);

            var rest = CreateRestClient(httpClient, jwt: _jwtMock.Object);

            await rest.GetAsync<string>("https://example.com");

            handlerMock.Protected().Verify(
                "SendAsync",
                Times.Once(),
                ItExpr.Is<HttpRequestMessage>(req => req.Headers.Authorization != null && req.Headers.Authorization.Parameter == "token123"),
                ItExpr.IsAny<CancellationToken>()
            );
        }

        [Fact]
        public async Task JWT_Not_Added_WhenNull()
        {
            _jwtMock.Setup(j => j.GetTokenAsync()).ReturnsAsync((string?)null);

            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("data")
            };
            var httpClient = CreateMockHttpClient(response, out var handlerMock);

            var rest = CreateRestClient(httpClient, jwt: _jwtMock.Object);

            await rest.GetAsync<string>("https://example.com");

            handlerMock.Protected().Verify(
                "SendAsync",
                Times.Once(),
                ItExpr.Is<HttpRequestMessage>(req => req.Headers.Authorization == null),
                ItExpr.IsAny<CancellationToken>()
            );
        }

        [Fact]
        public async Task RequestInterceptor_Modifies_Request()
        {
            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("data")
            };
            var httpClient = CreateMockHttpClient(response, out var handlerMock);

            _requestInterceptorMock
                .Setup(r => r.InterceptRequest(It.IsAny<HttpRequestMessage>()))
                .Returns<HttpRequestMessage>(req =>
                {
                    req.Headers.Add("X-Intercepted", "yes");
                    return req;
                });

            var rest = CreateRestClient(httpClient, requestInterceptor: _requestInterceptorMock.Object);

            await rest.GetAsync<string>("https://example.com");

            handlerMock.Protected().Verify(
                "SendAsync",
                Times.Once(),
                ItExpr.Is<HttpRequestMessage>(req =>
                    req.Headers.Contains("X-Intercepted") &&
                    req.Headers.GetValues("X-Intercepted").First() == "yes"),
                ItExpr.IsAny<CancellationToken>()
            );
        }

        [Fact]
        public async Task ResponseInterceptor_Called()
        {
            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("data")
            };
            var httpClient = CreateMockHttpClient(response, out _);

            _responseInterceptorMock
                .Setup(r => r.InterceptResponse(It.IsAny<HttpResponseMessage>()))
                .Returns<HttpResponseMessage>(r => r);

            _errorInterceptorMock
                .Setup(e => e.InterceptError(It.IsAny<ErrorInterceptorModel?>()))
                .Returns(Task.CompletedTask);

            var rest = CreateRestClient(httpClient,
                responseInterceptor: _responseInterceptorMock.Object,
                errorInterceptor: _errorInterceptorMock.Object);

            var result = await rest.GetAsync<string>("https://example.com");

            result.IsSuccessful.ShouldBeTrue();
            _responseInterceptorMock.Verify(r => r.InterceptResponse(It.IsAny<HttpResponseMessage>()), Times.Once);
        }

        [Fact]
        public async Task Non2xx_Triggers_ErrorInterceptor_WithoutThrowing()
        {
            var response = new HttpResponseMessage(HttpStatusCode.BadRequest)
            {
                Content = new StringContent("bad request")
            };
            var httpClient = CreateMockHttpClient(response, out _);

            _errorInterceptorMock.Setup(e => e.InterceptError(It.IsAny<ErrorInterceptorModel?>())).Returns(Task.CompletedTask);

            var rest = CreateRestClient(httpClient, errorInterceptor: _errorInterceptorMock.Object);

            var result = await rest.GetAsync<string>("https://example.com");

            result.IsSuccessful.ShouldBeFalse();
            result.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
            result.Content.ShouldBe("bad request");
            _errorInterceptorMock.Verify(e => e.InterceptError(It.IsAny<ErrorInterceptorModel?>()), Times.AtLeastOnce());
        }

        [Fact]
        public async Task Exception_Triggers_ErrorInterceptor()
        {
            var handlerMock = new Mock<HttpMessageHandler>();
            handlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ThrowsAsync(new HttpRequestException("fail"));

            var httpClient = new HttpClient(handlerMock.Object);

            _errorInterceptorMock.Setup(e => e.InterceptError(It.IsAny<ErrorInterceptorModel?>())).Returns(Task.CompletedTask);

            var rest = CreateRestClient(httpClient, errorInterceptor: _errorInterceptorMock.Object);

            var result = await rest.GetAsync<string>("https://example.com");

            result.IsSuccessful.ShouldBeFalse();
            _errorInterceptorMock.Verify(e => e.InterceptError(It.IsAny<ErrorInterceptorModel?>()), Times.AtLeastOnce());
        }
        

        [Fact]
        public async Task Handles_Null_Response()
        {
            var httpClient = CreateMockHttpClient(null, out _);

            _errorInterceptorMock.Setup(e => e.InterceptError(It.IsAny<ErrorInterceptorModel?>())).Returns(Task.CompletedTask);

            var rest = CreateRestClient(httpClient, errorInterceptor: _errorInterceptorMock.Object);

            var result = await rest.GetAsync<string>("https://example.com");

            result.IsSuccessful.ShouldBeFalse();
            result.Content.ShouldBeNull();
        }

        [Theory]
        [InlineData(HttpStatusCode.OK)]
        [InlineData(HttpStatusCode.Created)]
        [InlineData(HttpStatusCode.NoContent)]
        public async Task Successful_2xx_Responses(HttpStatusCode status)
        {
            var response = new HttpResponseMessage(status)
            {
                Content = new StringContent("{}")
            };
            var httpClient = CreateMockHttpClient(response, out _);
            var rest = CreateRestClient(httpClient);

            var result = await rest.GetAsync<object>("https://example.com");

            result.IsSuccessful.ShouldBeTrue();
        }
    }
}
