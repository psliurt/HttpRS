﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using Newtonsoft.Json;

namespace HttpRS
{
    /// <summary>
    /// 
    /// </summary>
    public class HeaderHelper
    {
        

        /// <summary>
        /// 建立請求的頭檔
        /// </summary>
        /// <param name="request"></param>
        /// <param name="headers"></param>
        /// <returns></returns>
        public static HttpWebRequest BuildRequestHeader(HttpWebRequest request, HttpHeaderList headers)
        {
            try
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
                            request.Headers.Add("Authorization", headers.GetHeaderValue(h));
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

                        case "referer":
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
            }
            catch (Exception e)
            {
                
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
            try
            {
                if (response != null && response.Headers != null)
                {
                    foreach (string headerKey in response.Headers.AllKeys)
                    {
                        headers.AddHeader(headerKey, response.Headers[headerKey]);
                    }
                }
            }
            catch (Exception e)
            {
                // TODO: handle exception
            }            
            return headers;
        }
    }
}
