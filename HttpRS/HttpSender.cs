using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;


namespace HttpRS
{
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

        public HttpHeaderList GetResponseHeaders()
        {
            return _responseHeader;
        }

        public String SendRequest(String method, String body, HttpHeaderList header)
        {
            _request = (HttpWebRequest)WebRequest.Create(_uri);
            _request.Method = method;
            _request = HeaderHelper.BuildRequestHeader(_request, header);

            if (!string.IsNullOrEmpty(method) &&
                method.Trim().ToUpper() != "GET" &&
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
                        return we.Status.ToString();
                    }
                    else
                    { 
                        
                    }
                }
                catch (NotSupportedException nse)
                {

                }
                catch (ProtocolViolationException pve)
                {

                }
                catch (InvalidOperationException ivoe)
                {

                }
                catch (Exception e)
                {
                    //log or do some process
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

                //using (StreamReader sr = new StreamReader(inputs, _responseStreamEncoding))
                using (StreamReader sr = new StreamReader(inputs, Encoding.GetEncoding(_response.CharacterSet)))
                {
                    respBody = sr.ReadToEnd();
                }

                

                string statusMsg = string.Format("({0})\r\n{1}", _response.StatusDescription, respBody);
                return statusMsg;
            }
            catch (NotSupportedException nse)
            {

            }
            catch (ProtocolViolationException pve)
            { 
            
            }
            catch (InvalidOperationException ivoe)
            {

            }
            catch (Exception e)
            { 
            
            }
            
            _responseHeader = HeaderHelper.ParseResponseHeader(_response);
            //string contentType = _responseHeader.GetHeaderValue("content-type");
            //ParseContentType(contentType);

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

            }
            catch (ProtocolViolationException pve)
            {
            }
            catch (OutOfMemoryException oome)
            {

            }
            catch (IOException ioe)
            { 
            
            }
            catch (Exception e)
            {
                //log some error message or do some process;
            }
            finally
            {
                if (inputStream != null)
                {
                    inputStream.Close();
                }
                _response.Close();
            }
            return responseString;
        }


        public ResponseResult SendRequest(HttpRequestMethod method, String body, HttpHeaderList header)
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

                }
                catch (ProtocolViolationException pve)
                {

                }
                catch (InvalidOperationException ivoe)
                {

                }
                catch (Exception e)
                {
                    //log or do some process
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

            }
            catch (ProtocolViolationException pve)
            {

            }
            catch (InvalidOperationException ivoe)
            {

            }
            catch (Exception e)
            {

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


                if (_response.ContentType.Contains("text/html"))
                {
                    //charset\s{0,}=\s{0,}"\s{0,}[\d\w]{0,}\D{0,}[\d\w]{0,}\D{0,}[\d\w]{0,}\z{0,}"
                    ParseHtmlCharSet(responseString);
                }

            }
            catch (ObjectDisposedException ode)
            {

            }
            catch (ProtocolViolationException pve)
            {
            }
            catch (OutOfMemoryException oome)
            {

            }
            catch (IOException ioe)
            {

            }
            catch (Exception e)
            {
                //log some error message or do some process;
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

        private Encoding ParseHtmlCharSet(string html)
        {
            Regex reg = new Regex(@"charset\s{0,}=\s{0,}""\s{0,}[\d\w]{0,}\D{0,}[\d\w]{0,}\D{0,}[\d\w]{0,}\z{0,}""");
            Match m = reg.Match(html);
            string s = m.Value;
            Regex rx = new Regex(@"charset\s{0,}=\s{0,}[\w]{1,}\D{0,1}[\d\w]{0,}");
            Match mm = rx.Match(html);
            string ss = mm.Value;
            return Encoding.UTF8;
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
