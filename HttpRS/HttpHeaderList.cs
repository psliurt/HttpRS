using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
namespace HttpRS
{
    /// <summary>
    /// 
    /// </summary>
    public class HttpHeaderList
    {
        /// <summary>
        /// A dictionary saves the http header name and value.
        /// </summary>
        private Dictionary<string, string> _headers = new Dictionary<string, string>();

        /// <summary>
        /// 加入一個Http Header 
        /// </summary>
        /// <param name="headerKey">The header name</param>
        /// <param name="headerValue">The header value</param>
        /// <returns></returns>
        public bool AddHeader(string headerKey, string headerValue)
        {
            if (!_headers.ContainsKey(headerKey))
            {
                _headers.Add(headerKey, headerValue);
                return true;
            }
            return false;            
        }

        /// <summary>
        /// get header collection, which is the dictionary type.
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, string> GetHeaderCollection()
        {
            return _headers;
        }

        /// <summary>
        /// Get all header's name
        /// </summary>
        /// <returns></returns>
        public List<String> GetHeaderKeys()
        {
            return _headers.Keys.ToList<String>();
        }

        /// <summary>
        /// get header value by header name
        /// </summary>
        /// <param name="headerName"></param>
        /// <returns></returns>
        public String GetHeaderValue(string headerName)
        {
            foreach (string h in _headers.Keys)
            {
                if (h.Trim().ToLower() == headerName.ToLower())
                { return _headers[h]; }
            }
            return string.Empty;            
        }

    }
}
