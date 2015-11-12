using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
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

        //private void ParseContentType(string contentTypeValue)
        //{
        //    string[] contentTypes = contentTypeValue.Split(',');
        //    foreach (string ct in contentTypes)
        //    {
        //        if (!string.IsNullOrEmpty(ct))
        //        {
        //            string[] mediaTypeNencoding = ct.Trim().Split(';');
        //            string mediaType = mediaTypeNencoding[0].Trim();
        //            if (mediaTypeNencoding.Length > 1 &&
        //                !string.IsNullOrEmpty(mediaTypeNencoding[1]))
        //            {
        //                string encoding = mediaTypeNencoding[1].Trim();

        //                if (encoding.Trim().ToLower().Contains("utf-8"))
        //                {
        //                    SetResponseEncoding(Encoding.UTF8);
        //                }
        //                if (encoding.Trim().ToLower().Contains("big5"))
        //                {
        //                    SetResponseEncoding(Encoding.Default);
        //                }
        //            }
        //            else
        //            {
        //                SetResponseEncoding(Encoding.Default);
        //            }
                    
        //        }
        //    }
        //}

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
                    //_request.ContentLength = outData.Length;// Is this can rewrite by program?
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
                //string ct = _responseHeader.GetHeaderValue("content-type");
                //ParseContentType(ct);

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
