using System;
using System.Collections.Specialized;
using System.Web;

namespace MA.BlazorRest.Src
{
    internal class UriBuilderExt
    {
        private readonly NameValueCollection _collection;
        private readonly UriBuilder _builder;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="uri"></param>
        public UriBuilderExt(Uri uri)
        {
            _builder = new UriBuilder(uri);
            _collection = HttpUtility.ParseQueryString(string.Empty);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public UriBuilderExt AddParameter(string key, string value)
        {
            _collection.Add(key, value);
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="collection"></param>
        /// <returns></returns>
        public UriBuilderExt AddParameter(NameValueCollection collection)
        {

            foreach(string item in collection)
            {
                _collection.Add(item, collection[item]);
            }  
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        public Uri Uri
        {
            get
            {
                _builder.Query = _collection.ToString();
                return _builder.Uri;
            }
        }
    }
}
