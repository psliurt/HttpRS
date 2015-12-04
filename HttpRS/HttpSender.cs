using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
namespace HttpRS
{
    /// <summary>
    /// 
    /// </summary>
    public class HttpSender
    {
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
            _uri = new Uri(url);
            _requestStreamEncoding = Encoding.Default;
            _responseStreamEncoding = Encoding.Default;
        }

        public HttpSender(string url, Encoding reqEncoding)
        {
            _uri = new Uri(url);
            _requestStreamEncoding = reqEncoding;
            _responseStreamEncoding = Encoding.Default;
        }

        public HttpSender(string url, Encoding requestEncoding, Encoding responseEncoding)
        {
            _uri = new Uri(url);
            _requestStreamEncoding = requestEncoding;
            _responseStreamEncoding = responseEncoding;
        }

        public void SetUrl(string url)
        {
            _uri = new Uri(url);
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
            ResponseResult rspResult = new ResponseResult();
            _request = (HttpWebRequest)WebRequest.Create(_uri);
            _request.Method = method.ToString().ToUpper();
            _request = HeaderHelper.BuildRequestHeader(_request, header);

            //如果有要利用http body傳送的資料，才會用outputStream來送資料
            if (method.ToString().ToUpper() != "GET" &&
                !string.IsNullOrEmpty(body))
            {
                Stream outStream = null;
                try
                {
                    outStream = _request.GetRequestStream();
                    byte[] outData = _requestStreamEncoding.GetBytes(body);
                    outStream.Write(outData, 0, outData.Length);
                }
                catch (WebException we)
                {
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
                    rspResult.ErrorMsg = nse.StackTrace.ToString();
                    rspResult.IsResultError = true;
                    return rspResult;
                }
                catch (ProtocolViolationException pve)
                {
                    rspResult.ErrorMsg = pve.StackTrace.ToString();
                    rspResult.IsResultError = true;
                    return rspResult;
                }
                catch (InvalidOperationException ivoe)
                {
                    rspResult.ErrorMsg = ivoe.StackTrace.ToString();
                    rspResult.IsResultError = true;
                    return rspResult;
                }
                catch (Exception e)
                {
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
                _response = (HttpWebResponse)_request.GetResponse();
            }
            catch (WebException we)
            {
                _response = (HttpWebResponse)we.Response;
                _responseHeader = HeaderHelper.ParseResponseHeader(_response);

                SetResponseEncoding(Encoding.GetEncoding(_response.CharacterSet));

                Stream inputs = _response.GetResponseStream();
                string respBody = string.Empty;
                using (StreamReader sr = new StreamReader(inputs, Encoding.GetEncoding(_response.CharacterSet)))
                {
                    respBody = sr.ReadToEnd();
                }
                string statusMsg = string.Format("({0})\r\n{1}", _response.StatusDescription, respBody);

                rspResult.Headers = _responseHeader;
                rspResult.IsResultError = true;
                rspResult.ResponseBody = respBody;
                rspResult.ErrorMsg = statusMsg;
                rspResult.StatusMsg = _response.StatusCode.ToString();
                return rspResult;
            }
            catch (NotSupportedException nse)
            {
                rspResult.ErrorMsg = nse.StackTrace.ToString();
                rspResult.IsResultError = true;
                return rspResult;
            }
            catch (ProtocolViolationException pve)
            {
                rspResult.ErrorMsg = pve.StackTrace.ToString();
                rspResult.IsResultError = true;
                return rspResult;
            }
            catch (InvalidOperationException ivoe)
            {
                rspResult.ErrorMsg = ivoe.StackTrace.ToString();
                rspResult.IsResultError = true;
                return rspResult;
            }
            catch (Exception e)
            {
                rspResult.ErrorMsg = e.StackTrace.ToString();
                rspResult.IsResultError = true;
                return rspResult;
            }

            _responseHeader = HeaderHelper.ParseResponseHeader(_response);
            SetResponseEncoding(Encoding.GetEncoding(_response.CharacterSet));

            Stream inputStream = null;
            String responseString = string.Empty;

            try
            {
                inputStream = _response.GetResponseStream();
                using (StreamReader sr = new StreamReader(inputStream, Encoding.GetEncoding(_response.CharacterSet)))
                {
                    responseString = sr.ReadToEnd();
                }
            }
            catch (ObjectDisposedException ode)
            {
                rspResult.ErrorMsg = ode.StackTrace.ToString();
                rspResult.IsResultError = true;
                return rspResult;
            }
            catch (ProtocolViolationException pve)
            {
                rspResult.ErrorMsg = pve.StackTrace.ToString();
                rspResult.IsResultError = true;
                return rspResult;
            }
            catch (OutOfMemoryException oome)
            {
                rspResult.ErrorMsg = oome.StackTrace.ToString();
                rspResult.IsResultError = true;
                return rspResult;
            }
            catch (IOException ioe)
            {
                rspResult.ErrorMsg = ioe.StackTrace.ToString();
                rspResult.IsResultError = true;
                return rspResult;
            }
            catch (Exception e)
            {
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
            rspResult.StatusMsg = _response.StatusCode.ToString();
            return rspResult;
        }

        private HttpRequestMethod ParseHttpMethod(string methodStr)
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
