using System;
using System.Collections.Specialized;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using MA.BlazorRest.Src.RequestContents;

namespace MA.BlazorRest.Src.Core
{
    public sealed class BlazorRestMessage : IBlazorRestMessage
    {
        public BlazorRestMessage(Uri? absoluteUrl,HttpMethod method)
        {
            AbsoluteUrl = absoluteUrl ?? throw new ArgumentNullException(nameof(absoluteUrl));
            Method = method ?? throw new ArgumentNullException(nameof(method));
        }

        public BlazorRestMessage(string? relativeUrl, HttpMethod method)
        {
            RelativeUrl = relativeUrl ?? throw new ArgumentNullException(nameof(relativeUrl));
            Method = method ?? throw new ArgumentNullException(nameof(method));
        }

        /// <summary>
        ///  Use Full Url In Request
        /// </summary>
        public Uri AbsoluteUrl { get; set; } = null!;

        /// <summary>
        /// Use RelativeUrl with Base Url Registered in Program.cs File
        /// </summary>
        public string RelativeUrl { get; set; } = null!;

        /// <summary>
        /// http Headers Of Request
        /// </summary>
        public NameValueCollection Headers { get; set; } = null!;

        /// <summary>
        /// Body Content Of Request Use In Post And Put Method
        /// </summary>
        public IRestContent Content { get; set; } = null!;

        /// <summary>
        /// Serilizer Encoding type
        /// </summary>
        public Encoding Encoding { get; set; } = Encoding.UTF8;

        /// <summary>
        /// Query Parameters Use In Get Method
        /// </summary>
        public NameValueCollection QueryParmeters { get; set;} = null!;

        /// <summary>
        /// SerilizerOption
        /// </summary>
        public JsonSerializerOptions? SerilizerOption { get; set; }

        /// <summary>
        ///  Http Method Of The Request
        /// </summary>
        public HttpMethod Method { get; set; } = null!;


        /// <summary>
        /// Add Query Parmeters
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public void AddQueryParameter(string name, string value)
        {
            QueryParmeters.Add(name, value);
        }

        /// <summary>
        /// Add Http Header
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public void AddHeader(string name, string value)
        {
            Headers.Add(name, value);
        }

    }
}
