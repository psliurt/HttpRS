using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
namespace HttpRS
{
    public class HttpHeaderList
    {
        private Dictionary<string, string> _headers = new Dictionary<string, string>();

        public bool AddHeader(string headerKey, string headerValue)
        {
            if (!_headers.ContainsKey(headerKey))
            {
                _headers.Add(headerKey, headerValue);
                return true;
            }
            return false;            
        }

        public Dictionary<string, string> GetHeaderCollection()
        {
            return _headers;
        }

        public List<String> GetHeaderKeys()
        {
            return _headers.Keys.ToList<String>();
        }

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
