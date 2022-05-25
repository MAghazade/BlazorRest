
using System;

namespace MA.BlazorRest.Src.RequestContents
{
    public class JsonContent : RestContent
    {
        public object Model { get;  private set; }

       /// <summary>
       /// 
       /// </summary>
       /// <param name="model"></param>
        public JsonContent(object model)
        {
            Model = model??throw new ArgumentNullException(nameof(model));
        }      
    }
}
