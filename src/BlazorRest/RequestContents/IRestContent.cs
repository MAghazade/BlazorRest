

using System.Net.Http;

namespace BlazorRest.RequestContents
{
    public interface IRestContent
    {
         HttpContent GetHttpContent();
    }
}
