using System.Net;

namespace BlazorRest.Responses
{

    public class BaseResponse
    {
        public bool IsSuccessful { get; set; }
        public HttpStatusCode? StatusCode { get; set; }
        public string? Content { get; set; }
    }

    public class BaseResponse<T> : BaseResponse
    {
        public T? Data { get; set; }
    }

}
