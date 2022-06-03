using Microsoft.AspNetCore.Components.Forms;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MA.BlazorRest.Src.RequestContents
{
    public class FileContent : RestContent
    {
        public Dictionary<string, IBrowserFile> Files { get; private set; } = new();


        /// <summary>
        /// 
        /// </summary>
        /// <param name="file"></param>
        /// <param name="fileParameterName"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public FileContent(IBrowserFile? file, string? fileParameterName)
        {
            if (file is not null && fileParameterName is not null)
            {
                Files.Add(fileParameterName, file);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="files"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="Exception"></exception>
        public FileContent(Dictionary<string, IBrowserFile>? files)
        {
            if (files is not null && files.Any())
            {
                Files = files;
            }
        }
    }
}

