using MA.BlazorRest.Src.Core;
using MA.BlazorRest.Src.RequestContents;
using Microsoft.AspNetCore.Components.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Text.Json;
using JsonContent = MA.BlazorRest.Src.RequestContents.JsonContent;

namespace MA.BlazorRest.Src.Helpers
{
    internal static class HttpMessageHelper
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="files"></param>
        /// <returns></returns>
        public static MultipartFormDataContent CreateMultiPartFormData(Dictionary<string, IBrowserFile>? files)
        {
            var requestContent = new MultipartFormDataContent();
            requestContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data");

            if (files is null || !files.Any()) return requestContent;

            foreach (var item in files)
            {
                var stream = item.Value.OpenReadStream(maxAllowedSize: 512000 * 1000);
                requestContent.Add(content: new StreamContent(stream, (int)item.Value.Size), name: item.Key, fileName: item.Value.Name);
            }
            return requestContent;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <param name="form"></param>
        /// <param name="contentType"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static MultipartFormDataContent AddModelToFormData(object? model, MultipartFormDataContent? form, string? contentType)
        {
            if (model is null) throw new ArgumentNullException(nameof(model));
            if (form is null) throw new ArgumentNullException(nameof(form));
            if (contentType is null) throw new ArgumentNullException(nameof(contentType));


            foreach (PropertyInfo prop in model.GetType().GetProperties())
            {
                if (prop.GetValue(model, null) as string is not null and string data)
                {
                    var content = new StringContent(data, Encoding.UTF8, contentType);
                    form.Add(content, prop.Name);
                }
            }
            return form;
        }

        /// <summary>
        /// Create HttpRequest Message
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static HttpRequestMessage CreateHttpRequestMessage(IBlazorRestMessage message, HttpClient httpClient, JsonSerializerOptions serializerOptions)
        {
            var httpRequestMessage = new HttpRequestMessageBuilder()
               .AddUri(UriHelper.GetFinalUri(message, httpClient))
               .AddHeaders(message.Headers)
               .AddMethod(message.Method)
               .AddQuery(message.QueryParmeters);

            if (message.Content is JsonContent or FileContent and null)
                throw new ArgumentException("Message Content Cannot be null !");

            switch (message.Content)
            {
                case JsonContent messageContent:
                    httpRequestMessage.AddJsonContent(messageContent, message.Encoding,
                        message.SerilizerOption ?? serializerOptions);
                    break;

                case FileWithModelContent fileWithModelContent:
                    if (fileWithModelContent.Model != null)
                        _ = httpRequestMessage.AddFilesWithModel(fileWithModelContent.Files,
                            model: fileWithModelContent.Model);
                    break;

                case FileContent fileContent when !fileContent.Files.Any():
                    throw new Exception("No files were added to the request");

                case FileContent fileContent:
                    httpRequestMessage.AddFiles(fileContent.Files);
                    break;
            }

            return httpRequestMessage.Build();
        }
    }
}
