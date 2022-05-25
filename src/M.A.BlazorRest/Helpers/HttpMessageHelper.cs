using Microsoft.AspNetCore.Components.Forms;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;

namespace MA.BlazorRest.Src.Helpers
{
    internal static class HttpMessageHelper
    {
        public static MultipartFormDataContent CreateMultiPartFormData(Dictionary<string, IBrowserFile> files)
        {
            var requestContent = new MultipartFormDataContent();
            requestContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data");
            foreach (var item in files)
            {
                var stream = item.Value.OpenReadStream(maxAllowedSize: 512000 * 1000);
                requestContent.Add(content: new StreamContent(stream, (int)item.Value.Size), name: item.Key, fileName: item.Value.Name);
            }
            return requestContent;
        }

        public static MultipartFormDataContent AddModelToFormData(object model, MultipartFormDataContent form, string contentType)
        {
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
    }
}
