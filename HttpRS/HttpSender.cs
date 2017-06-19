using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using log4net;
using Newtonsoft.Json;

namespace HttpRS
{
    /// <summary>
    /// 用來發送Http Request的物件
    /// </summary>
    public class HttpSender
    {
        private ILog _log = LogManager.GetLogger(typeof(HttpSender));

        private HttpWebRequest _request;
        private HttpWebResponse _response;
        private Uri _uri;
        private HttpHeaderList _responseHeader;

        //is decompression

        private Encoding _requestStreamEncoding;
        private Encoding _responseStreamEncoding;

        public HttpSender()
        {
            _requestStreamEncoding = Encoding.Default;
            _responseStreamEncoding = Encoding.Default;
        }

        public HttpSender(string url)
        {
            _log.InfoFormat("[ Constructor 1 ], url=>{0}", url);

            try
            {
                _uri = new Uri(url);
            }
            catch (ArgumentException ae)
            {
                throw;
            }
            catch (UriFormatException ufe)
            {
                throw;
            }
            _requestStreamEncoding = Encoding.Default;
            _responseStreamEncoding = Encoding.Default;
        }

        public HttpSender(string url, Encoding reqEncoding)
        {
            _log.InfoFormat("[ Constructor 2 ], url=>{0}", url);
            try
            {
                _uri = new Uri(url);
            }
            catch (ArgumentException ae)
            {
                throw;
            }
            catch (UriFormatException ufe)
            {
                throw;
            }
            _requestStreamEncoding = reqEncoding;
            _responseStreamEncoding = Encoding.Default;
        }

        public HttpSender(string url, Encoding requestEncoding, Encoding responseEncoding)
        {
            _log.InfoFormat("[ Constructor 3 ], url=>{0}", url);

            try
            {
                _uri = new Uri(url);
            }
            catch (ArgumentException ae)
            {
                throw;
            }
            catch (UriFormatException ufe)
            {
                throw;
            }
            
            _requestStreamEncoding = requestEncoding;
            _responseStreamEncoding = responseEncoding;
        }

        public void SetUrl(string url)
        {
            try
            {
                _uri = new Uri(url);
            }
            catch (ArgumentException ae)
            {
                throw;
            }
            catch (UriFormatException ufe)
            {
                throw;
            }
        }

        public void SetRequestEncoding(Encoding encoding)
        {
            _requestStreamEncoding = encoding;
        }

        public void SetResponseEncoding(Encoding encoding)
        {
            _responseStreamEncoding = encoding;
        }
        /// <summary>
        /// 取得HttpWebResponse的Header
        /// </summary>
        /// <returns></returns>
        public HttpHeaderList GetResponseHeaders()
        {
            return _responseHeader;
        }

        public String SendRequest(String method, String body, HttpHeaderList header)
        {
            // change method string to enum
            ResponseResult result = SendOutRequest(ParseHttpMethod(method), body, header);
            return result.ResponseBody;
        }

        public ResponseResult SendRequest(HttpRequestMethod method, String body, HttpHeaderList header)
        {
            return SendOutRequest(method, body, header);            
        }

        private ResponseResult SendOutRequest(HttpRequestMethod method, string body, HttpHeaderList header)
        {
            _log.InfoFormat("[ Start SendOutRequest ], 開始執行發送Request的流程, method=>{0}, body=>{1}, header=>{2}", method, body, JsonConvert.SerializeObject(header));                

            ResponseResult rspResult = new ResponseResult();
            _request = (HttpWebRequest)WebRequest.Create(_uri);

            _log.InfoFormat("[ Proc SendOutRequest ], HttpWebRequest物件建立完畢, method=>{0}, body=>{1}", method, body);                

            ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12 | SecurityProtocolType.Tls;                                     

            _request.Method = method.ToString().ToUpper();
            _request = HeaderHelper.BuildRequestHeader(_request, header);

            _log.InfoFormat("[ Proc SendOutRequest ], HttpWebRequest的Header資料物件建立完畢, method=>{0}, body=>{1}", method, body);  

            //如果有要利用http body傳送的資料，才會用outputStream來送資料
            if (method.ToString().ToUpper() != "GET" &&
                !string.IsNullOrEmpty(body))
            {
                _log.InfoFormat("[ Proc SendOutRequest ], 要發送的Http Request不是 [ GET ], 所以需要處裡Request的Http Body資料。");

                Stream outStream = null;
                try
                {
                    outStream = _request.GetRequestStream();
                    byte[] outData = _requestStreamEncoding.GetBytes(body);
                    outStream.Write(outData, 0, outData.Length);
                }
                catch (WebException we)
                {
                    WriteErrorLog(_log, we);
                    if (we.Response == null)
                    {
                        rspResult.ErrorMsg = we.Status.ToString();
                        rspResult.IsResultError = true;
                        return rspResult;
                    }
                    else
                    {

                    }
                }
                catch (NotSupportedException nse)
                {
                    WriteErrorLog(_log, nse);
                    rspResult.ErrorMsg = nse.StackTrace.ToString();
                    rspResult.IsResultError = true;
                    return rspResult;
                }
                catch (ProtocolViolationException pve)
                {
                    WriteErrorLog(_log, pve);
                    rspResult.ErrorMsg = pve.StackTrace.ToString();
                    rspResult.IsResultError = true;
                    return rspResult;
                }
                catch (InvalidOperationException ivoe)
                {
                    WriteErrorLog(_log, ivoe);
                    rspResult.ErrorMsg = ivoe.StackTrace.ToString();
                    rspResult.IsResultError = true;
                    return rspResult;
                }
                catch (Exception e)
                {
                    WriteErrorLog(_log, e);
                    rspResult.ErrorMsg = e.StackTrace.ToString();
                    rspResult.IsResultError = true;
                    return rspResult;
                }
                finally
                {
                    if (outStream != null)
                    {
                        outStream.Close();
                    }
                }
            }

            try
            {
                _log.InfoFormat("[ Proc SendOutRequest ], 發送Http Request並取得Response物件。");

                _response = (HttpWebResponse)_request.GetResponse();
            }
            catch (WebException we)
            {
                WriteErrorLog(_log, we);
                _response = (HttpWebResponse)we.Response;
                _responseHeader = HeaderHelper.ParseResponseHeader(_response);

                if (_response == null || string.IsNullOrEmpty(_response.CharacterSet))
                {
                    SetResponseEncoding(Encoding.UTF8);
                }
                else
                {
                    SetResponseEncoding(Encoding.GetEncoding(_response.CharacterSet));
                } 

                Stream inputs = _response.GetResponseStream();
                string respBody = string.Empty;
                using (StreamReader sr = new StreamReader(inputs, this._responseStreamEncoding))
                {
                    respBody = sr.ReadToEnd();
                }
                string statusMsg = string.Format("({0})\r\n{1}", _response.StatusDescription, respBody);

                rspResult.Headers = _responseHeader;
                rspResult.IsResultError = true;
                rspResult.ResponseBody = respBody;
                rspResult.ErrorMsg = statusMsg;
                if (_response != null)
                {
                    rspResult.StatusMsg = _response.StatusCode.ToString();
                }
                else
                {
                    _log.InfoFormat("[ Proc SendOutRequest ], 無法設定StatusCode, 因為WebException發生的回應訊息內沒有StatusCode。");
                }
                
                return rspResult;
            }
            catch (NotSupportedException nse)
            {
                WriteErrorLog(_log, nse);
                rspResult.ErrorMsg = nse.StackTrace.ToString();
                rspResult.IsResultError = true;
                return rspResult;
            }
            catch (ProtocolViolationException pve)
            {
                WriteErrorLog(_log, pve);
                rspResult.ErrorMsg = pve.StackTrace.ToString();
                rspResult.IsResultError = true;
                return rspResult;
            }
            catch (InvalidOperationException ivoe)
            {
                WriteErrorLog(_log, ivoe);
                rspResult.ErrorMsg = ivoe.StackTrace.ToString();
                rspResult.IsResultError = true;
                return rspResult;
            }
            catch (Exception e)
            {
                WriteErrorLog(_log, e);
                rspResult.ErrorMsg = e.StackTrace.ToString();
                rspResult.IsResultError = true;
                return rspResult;
            }

            _responseHeader = HeaderHelper.ParseResponseHeader(_response);

            _log.InfoFormat("[ Proc SendOutRequest ], 解析回應的Response Header完畢。");           

            if (_response == null || string.IsNullOrEmpty(_response.CharacterSet))
            {
                SetResponseEncoding(Encoding.UTF8);
            }
            else
            {
                SetResponseEncoding(Encoding.GetEncoding(_response.CharacterSet));                
            }            

            Stream inputStream = null;
            String responseString = string.Empty;

            try
            {
                inputStream = _response.GetResponseStream();                
                using (StreamReader sr = new StreamReader(inputStream, this._responseStreamEncoding))
                {
                    responseString = sr.ReadToEnd();
                }
            }
            catch (ObjectDisposedException ode)
            {
                WriteErrorLog(_log, ode);
                rspResult.ErrorMsg = ode.StackTrace.ToString();
                rspResult.IsResultError = true;
                return rspResult;
            }
            catch (ProtocolViolationException pve)
            {
                WriteErrorLog(_log, pve);
                rspResult.ErrorMsg = pve.StackTrace.ToString();
                rspResult.IsResultError = true;
                return rspResult;
            }
            catch (OutOfMemoryException oome)
            {
                WriteErrorLog(_log, oome);
                rspResult.ErrorMsg = oome.StackTrace.ToString();
                rspResult.IsResultError = true;
                return rspResult;
            }
            catch (IOException ioe)
            {
                WriteErrorLog(_log, ioe);
                rspResult.ErrorMsg = ioe.StackTrace.ToString();
                rspResult.IsResultError = true;
                return rspResult;
            }
            catch (Exception e)
            {
                WriteErrorLog(_log, e);
                rspResult.ErrorMsg = e.StackTrace.ToString();
                rspResult.IsResultError = true;
                return rspResult;
            }
            finally
            {
                if (inputStream != null)
                {
                    inputStream.Close();
                }
                _response.Close();
            }

            rspResult.ResponseBody = responseString;
            rspResult.IsResultError = false;
            rspResult.Headers = _responseHeader;
            if (_response != null)
            {
                rspResult.StatusMsg = _response.StatusCode.ToString();
            }
            else
            {
                _log.InfoFormat("[ Proc SendOutRequest ], 無法設定StatusCode, 因為對方沒有回應StatusCode。");
            }
            
            return rspResult;
        }

        public HttpRequestMethod ParseHttpMethod(string methodStr)
        {
            string upperCaseMethodStr = methodStr.Trim().ToUpper();
            switch (upperCaseMethodStr)
            { 
                case "OPTIONS":
                    return HttpRequestMethod.Options;
                case "GET":
                    return HttpRequestMethod.Get;
                case "HEAD":
                    return HttpRequestMethod.Head;
                case "POST":
                    return HttpRequestMethod.Post;
                case "PUT":
                    return HttpRequestMethod.Put;
                case "DELETE":
                    return HttpRequestMethod.Delete;
                case "TRACE":
                    return HttpRequestMethod.Trace;
                case "CONNECT":
                    return HttpRequestMethod.Connect;
                default:
                    return HttpRequestMethod.Post;
            }
        }

        private void WriteErrorLog(ILog log, Exception e)
        {
            log.InfoFormat("[ Proc HttpSender SendOutRequest Error ]");
            log.Error(e);
            log.Error(e.Message);
            log.Error(e.StackTrace);
            if (e.InnerException != null)
            {
                log.Error(e.InnerException);
                log.Error(e.InnerException.Message);
                log.Error(e.InnerException.StackTrace);
            }
        }

    }

    public enum HttpRequestMethod
    { 
        Options,
        Get,
        Head,
        Post,
        Put,
        Delete,
        Trace,
        Connect
    }
}
