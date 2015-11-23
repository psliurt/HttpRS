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
        /// 
        /// </summary>
        private Dictionary<string, string> _headers = new Dictionary<string, string>();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="headerKey"></param>
        /// <param name="headerValue"></param>
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
        /// 
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, string> GetHeaderCollection()
        {
            return _headers;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public List<String> GetHeaderKeys()
        {
            return _headers.Keys.ToList<String>();
        }

        /// <summary>
        /// 
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
