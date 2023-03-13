

using System.Net.Http;

namespace MA.BlazorRest.Src.RequestContents
{
    public interface IRestContent
    {
         HttpContent GetHttpContent();
    }
}
