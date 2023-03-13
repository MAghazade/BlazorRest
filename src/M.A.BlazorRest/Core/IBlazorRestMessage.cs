using MA.BlazorRest.Src.RequestContents;
using System;
using System.Collections.Specialized;
using System.Net.Http;
using System.Text;
using System.Text.Json;

namespace MA.BlazorRest.Src.Core
{
    public interface IBlazorRestMessage
    {
        /// <summary>
        /// Request content 
        /// </summary>
        IRestContent Content { get; set; }

     
        /// <summary>
        /// http request headers
        /// </summary>
        NameValueCollection Headers { get; set; }

        /// <summary>
        /// http request query parameters
        /// </summary>
        NameValueCollection QueryParmeters { get; set; }

        
        /// <summary>
        /// full url with protocol
        /// </summary>
        Uri AbsoluteUrl { get; set; }

        /// <summary>
        /// relative url uses  base url  for domain name
        /// </summary>
        string RelativeUrl { get; set; }

        /// <summary>
        /// add http header method 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        void AddHeader(string name, string value);

        /// <summary>
        /// add http query parameter method 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        void AddQueryParameter(string name, string value);

        /// <summary>
        /// http method of request
        /// </summary>
        HttpMethod Method { get; set; }
    }
}