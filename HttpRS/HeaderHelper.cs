using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using log4net;
using Newtonsoft.Json;

namespace HttpRS
{
    /// <summary>
    /// 
    /// </summary>
    public class HeaderHelper
    {
        private static ILog _log = LogManager.GetLogger(typeof(HeaderHelper));

        /// <summary>
        /// 建立請求的頭檔
        /// </summary>
        /// <param name="request"></param>
        /// <param name="headers"></param>
        /// <returns></returns>
        public static HttpWebRequest BuildRequestHeader(HttpWebRequest request, HttpHeaderList headers)
        {
            _log.InfoFormat("[ Start BuildRequestHeader ], 開始建立Request所需要的Header, 把HttpHeaderList內的標頭資料轉換成HttpWebRequest的Header, 自定義的HttpHeaderList=>{0}", JsonConvert.SerializeObject(headers));

            try
            {
                foreach (String h in headers.GetHeaderKeys())
                {
                    _log.InfoFormat("[ Proc BuildRequestHeader ], 開始建立Request所需要的Header, header=>{0}", h);

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
                _log.InfoFormat("[ Proc BuildRequestHeader Error ]");
                _log.Error(e);
                _log.Error(e.Message);
                _log.Error(e.StackTrace);
                if (e.InnerException != null)
                {
                    _log.Error(e.InnerException);
                    _log.Error(e.InnerException.Message);
                    _log.Error(e.InnerException.StackTrace);
                }
            }            

            _log.InfoFormat("[ End BuildRequestHeader ], 開始建立Request所需要的Header,把HttpHeaderList內的標頭資料轉換成HttpWebRequest的Header, 自定義的HttpHeaderList=>{0}", JsonConvert.SerializeObject(headers));

            return request;
        }

        /// <summary>
        /// 解析Http Response的header
        /// </summary>
        /// <param name="response">HttpWebResponse 物件</param>
        /// <returns></returns>
        public static HttpHeaderList ParseResponseHeader(HttpWebResponse response)
        {
            _log.InfoFormat("[ Start ParseResponseHeader ], 開始解析Response物件的標頭");

            HttpHeaderList headers = new HttpHeaderList();
            try
            {
                if (response != null && response.Headers != null)
                {
                    foreach (string headerKey in response.Headers.AllKeys)
                    {
                        _log.InfoFormat("[ Proc ParseResponseHeader ], 讀取Response物件內的Header, headerKey=>{0}, header value=>{1}", headerKey, response.Headers[headerKey]);

                        headers.AddHeader(headerKey, response.Headers[headerKey]);
                    }
                }
            }
            catch (Exception e)
            {
                _log.InfoFormat("[ Proc ParseResponseHeader Error ]");
                _log.Error(e);
                _log.Error(e.Message);
                _log.Error(e.StackTrace);
                if (e.InnerException != null)
                {
                    _log.Error(e.InnerException);
                    _log.Error(e.InnerException.Message);
                    _log.Error(e.InnerException.StackTrace);
                }
            }            

            _log.InfoFormat("[ End ParseResponseHeader ], 解析Response物件的標頭完畢, 標頭檔物件(headers)=>{0}", JsonConvert.SerializeObject(headers, Formatting.Indented));
            return headers;
        }
    }
}
