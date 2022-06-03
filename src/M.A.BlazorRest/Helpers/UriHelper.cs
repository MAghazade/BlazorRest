using MA.BlazorRest.Src.Core;
using System;

namespace MA.BlazorRest.Src.Helpers
{
    internal class UriHelper
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="client"></param>
        /// <returns></returns>
        public static Uri GetFinalUri(IBlazorRestMessage message, System.Net.Http.HttpClient client)
        {
            var uri = message?.AbsoluteUrl is null && client.BaseAddress is not null
                 ? new Uri(client.BaseAddress, message?.RelativeUrl) : message?.AbsoluteUrl;

            if (uri is null)
            {
                throw new NullReferenceException(nameof(uri));
            }
         
            return uri;
        }
    }
}
