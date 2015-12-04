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
    public class HeaderHelper
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="headers"></param>
        /// <returns></returns>
        public static HttpWebRequest BuildRequestHeader(HttpWebRequest request, HttpHeaderList headers)
        {
            foreach (String h in headers.GetHeaderKeys())
            {
                switch (h.Trim().ToLower())
                {
                    case "accept":
                        request.Accept = headers.GetHeaderValue(h);
                        break;
                    case "accept-encoding":
                        if (headers.GetHeaderValue(h).ToLower().Contains("gzip") &&
                            headers.GetHeaderValue(h).ToLower().Contains("deflate"))
                        {
                            request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
                        }
                        else if (headers.GetHeaderValue(h).ToLower().Contains("gzip"))
                        {
                            request.AutomaticDecompression = DecompressionMethods.GZip;
                        }
                        else if (headers.GetHeaderValue(h).ToLower().Contains("deflate"))
                        {
                            request.AutomaticDecompression = DecompressionMethods.Deflate;
                        }
                        break;
                    case "authorization":
                        request.PreAuthenticate = true;
                        break;
                    case "connection":
                        {
                            string connectionValue = headers.GetHeaderValue(h).Trim().ToLower();
                            if (connectionValue.Contains("keep-alive"))
                            { request.KeepAlive = true; }
                            if (connectionValue.Contains("upgrade"))
                            { request.Headers.Add("Connection", "Upgrade"); }
                        }                        
                        
                        break;
                    case "content-type":                        
                        request.ContentType = headers.GetHeaderValue(h);
                        
                        break;
                    case "content-length":
                        request.ContentLength = Convert.ToInt64(headers.GetHeaderValue(h));
                        break;
                    case "date":
                        request.Date = Convert.ToDateTime(headers.GetHeaderValue(h));
                        break;
                    case "expect":
                        request.Expect = headers.GetHeaderValue(h);
                        break;
                    
                    case "host":
                        request.Host = headers.GetHeaderValue(h);
                        break;
                                      
                    case "if-modified-since":
                        request.IfModifiedSince = Convert.ToDateTime(headers.GetHeaderValue(h));
                        break;
                    case "refer":
                        request.Referer = headers.GetHeaderValue(h);
                        break;
                    case "user-agent":
                        request.UserAgent = headers.GetHeaderValue(h);
                        break;  
                    default:
                        request.Headers.Add(h, headers.GetHeaderValue(h));
                        break;
                }
            }
            return request;
        }
        /// <summary>
        /// 解析Http Response的header
        /// </summary>
        /// <param name="response">HttpWebResponse 物件</param>
        /// <returns></returns>
        public static HttpHeaderList ParseResponseHeader(HttpWebResponse response)
        {
            HttpHeaderList headers = new HttpHeaderList();
            foreach (string headerKey in response.Headers.AllKeys)
            {
                headers.AddHeader(headerKey, response.Headers[headerKey]);
            }
            return headers;
        }
    }
}
