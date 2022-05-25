using Microsoft.AspNetCore.Components.Forms;
using System;
using System.Collections.Generic;
using System.Net.Http;

namespace MA.BlazorRest.Src.RequestContents
{
    public class FileWithModelContent : FileContent
    {
        public object? Model { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="File"></param>
        /// <param name="fileParameterName"></param>
        /// <param name="model"></param>
        public FileWithModelContent(
            IBrowserFile file,
            string fileParameterName,
            object model) : base(file, fileParameterName)
        {
           
            Model = model ?? throw new ArgumentNullException(nameof(model));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="files"></param>
        /// <param name="model"></param>
        public FileWithModelContent(
          Dictionary<string, IBrowserFile> files,
           object model) : base(files)
        {
            Model = model ?? throw new ArgumentNullException(nameof(model));
        }

    }
}
