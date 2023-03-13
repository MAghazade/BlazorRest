using MA.BlazorRest.Src.Helpers;
using MA.BlazorRest.Src.RequestContents;
using Microsoft.AspNetCore.Components.Forms;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace MA.BlazorRest.Src
{
    internal class HttpRequestMessageBuilder
    {
        private readonly HttpRequestMessage _httpRequestMessage = new HttpRequestMessage();
        /// <summary>
        /// 
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        public HttpRequestMessageBuilder AddUri(Uri uri)
        {
            _httpRequestMessage.RequestUri = uri;
            return this;
        }

      /// <summary>
      /// 
      /// </summary>
      /// <param name="httpMethod"></param>
      /// <returns></returns>
      /// <exception cref="ArgumentNullException"></exception>
        public HttpRequestMessageBuilder AddMethod(HttpMethod? httpMethod)
        {
            if (httpMethod is null) throw new ArgumentNullException(nameof(httpMethod));
            
            _httpRequestMessage.Method = httpMethod;
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <param name="encoding"></param>
        /// <param name="serialierOptions"></param>
        /// <returns></returns>
        public HttpRequestMessageBuilder AddJsonContent(JsonContent model, Encoding encoding, JsonSerializerOptions serialierOptions)
        {
            _httpRequestMessage.Content = new StringContent(JsonSerializer.Serialize(model.Model, serialierOptions), encoding, "application/json");
            return this;
        }


        public HttpRequestMessageBuilder AddHeaders(NameValueCollection? headers)
        {
            if (headers is not null)
            {
                foreach (KeyValuePair<string, string> header in headers)
                {
                    _httpRequestMessage.Headers.TryAddWithoutValidation(header.Key, header.Value);
                }
            }

            return this;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        /// <exception cref="NullReferenceException"></exception>
        public HttpRequestMessageBuilder AddQuery(NameValueCollection? parameters = default)
        {
            if (parameters is null)
            {
                return this;
            }

            if (_httpRequestMessage.RequestUri is null)
            {
                throw new NullReferenceException("Request Uri Can not Be Null Here !");
            }

            _httpRequestMessage.RequestUri = new UriBuilderExt(_httpRequestMessage.RequestUri)
                            .AddParameter(parameters).Uri;
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public HttpRequestMessageBuilder AddJwtHeader(string token)
        {
            _httpRequestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="file"></param>
        /// <param name="fileNameInForm"></param>
        /// <returns></returns>
        public HttpRequestMessageBuilder AddFile(IBrowserFile file, string fileNameInForm)
        {
            var requestContent = HttpMessageHelper.CreateMultiPartFormData(new()
            {
                { fileNameInForm, file }
            });

            _httpRequestMessage.Content = requestContent;
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="files"></param>
        /// <returns></returns>
        public HttpRequestMessageBuilder AddFiles(Dictionary<string, IBrowserFile> files)
        {
            var requestContent = HttpMessageHelper.CreateMultiPartFormData(files);
            _httpRequestMessage.Content = requestContent;
            return this;
        }

       /// <summary>
       /// 
       /// </summary>
       /// <param name="files"></param>
       /// <param name="model"></param>
       /// <returns></returns>
        public HttpRequestMessageBuilder AddFilesWithModel(Dictionary<string, IBrowserFile>? files, object model)
        {
            var requestContent = HttpMessageHelper.CreateMultiPartFormData(files);

            requestContent = HttpMessageHelper
                .AddModelToFormData(model, requestContent, "application/json");

            _httpRequestMessage.Content = requestContent;
            return this;
        }

       public HttpRequestMessageBuilder AddHttpContent(HttpContent? content)
       {
            if (content is null) return this;

           _httpRequestMessage.Content = content;
           return this;
       }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public HttpRequestMessage Build()
        {
            return _httpRequestMessage;
        }
    }
}
