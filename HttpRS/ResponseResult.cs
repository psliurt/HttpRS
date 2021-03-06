﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace HttpRS
{
    /// <summary>
    /// The Http Response result.
    /// This object will be made after HttpSender or JsonRequestSender Send out the Http Request.
    /// </summary>
    public class ResponseResult
    {
        
        /// <summary>
        /// error flag
        /// </summary>
        public bool IsResultError { get; set; }
        /// <summary>
        /// The error message which generate during sending http request
        /// </summary>
        public string ErrorMsg { get; set; }
        /// <summary>
        /// The http response body
        /// </summary>
        public string ResponseBody { get; set; }
        /// <summary>
        /// The http response status code
        /// </summary>
        public int StatusCode { get; set; }   
        /// <summary>
        /// note
        /// </summary>
        public string OtherNote { get; set; }
        /// <summary>
        /// The Response headers
        /// </summary>
        public HttpHeaderList Headers { get; set; }

        public CookieContainer RequestCookieBag { get; set; }

        public CookieCollection ResponseCookieBag { get; set; }

        public HttpWebRequest RequestInstance { get; set; }

        public HttpWebResponse ResponseInstance { get; set; }

        private string _statusMsg;

        public string StatusMsg 
        { 
            get 
            {
                return _statusMsg;
            }
            set 
            {
                SetStatusCode(value);
                _statusMsg = value;
            }
        }

        /// <summary>
        /// Reference 
        /// https://msdn.microsoft.com/en-us/library/system.net.httpstatuscode(v=vs.110).aspx
        /// </summary>
        /// <param name="statusMsg"></param>
        private void SetStatusCode(string statusMsg)
        {
            switch (statusMsg)
            { 
                case "Accepted":
                    StatusCode = 202;
                    break;
                case "Ambiguous":
                    StatusCode = 300;
                    break;
                case "BadGateway":
                    StatusCode = 502;
                    break;
                case "BadRequest":
                    StatusCode = 400;
                    break;
                case "Conflict":
                    StatusCode = 409;
                    break;
                case "Continue":
                    StatusCode = 100;
                    break;
                case "Created":
                    StatusCode = 201;
                    break;
                case "ExpectationFailed":
                    StatusCode = 417;
                    break;
                case "Forbidden":
                    StatusCode = 403;
                    break;
                case "Found":
                    StatusCode = 302;
                    break;
                case "GatewayTimeout":
                    StatusCode = 504;
                    break;
                case "Gone":
                    StatusCode = 410;
                    break;
                case "HttpVersionNotSupported":
                    StatusCode = 505;
                    break;
                case "InternalServerError":
                    StatusCode = 500;
                    break;
                case "LengthRequired":
                    StatusCode = 411;
                    break;
                case "MethodNotAllowed":
                    StatusCode = 405;
                    break;
                case "Moved":
                    StatusCode = 301;
                    break;
                case "MovedPermanently":
                    StatusCode = 301;
                    break;
                case "MultipleChoices":
                    StatusCode = 300;
                    break;
                case "NoContent":
                    StatusCode = 204;
                    break;
                case "NonAuthoritativeInformation":
                    StatusCode = 203;
                    break;
                case "NotAcceptable":
                    StatusCode = 406;
                    break;
                case "NotFound":
                    StatusCode = 404;
                    break;
                case "NotImplemented":
                    StatusCode = 501;
                    break;
                case "OK":
                    StatusCode = 200;
                    break;
                case "PartialContent":
                    StatusCode = 206;
                    break;
                case "PaymentRequired":
                    StatusCode = 402;
                    break;
                case "PreconditionFailed":
                    StatusCode = 412;
                    break;
                case "ProxyAuthenticationRequired":
                    StatusCode = 407;
                    break;
                case "Redirect":
                    StatusCode = 302;
                    break;
                case "RedirectKeepVerb":
                    StatusCode = 307;
                    break;
                case "RedirectMethod":
                    StatusCode = 303;
                    break;
                case "RequestedRangenotSatisfiable":
                    StatusCode = 416;
                    break;
                case "RequestEntityTooLarge":
                    StatusCode = 413;
                    break;
                case "RequestTimeout":
                    StatusCode = 408;
                    break;
                case "RequestUriTooLong":
                    StatusCode = 414;
                    break;
                case "ResetContent":
                    StatusCode = 205;
                    break;
                case "SeeOther":
                    StatusCode = 303;
                    break;
                case "ServiceUnavailable":
                    StatusCode = 503;
                    break;
                case "SwitchingProtocols":
                    StatusCode = 101;
                    break;
                case "TemporaryRedirect":
                    StatusCode = 307;
                    break;
                case "Unauthorized":
                    StatusCode = 401;
                    break;
                case "UnsupportedMediaType":
                    StatusCode = 415;
                    break;
                case "Unused":
                    StatusCode = 306;
                    break;
                case "UpgradeRequired":
                    StatusCode = 426;
                    break;
                case "UseProxy":
                    StatusCode = 305;
                    break;
                default:
                    StatusCode = 200;
                    break;
            }
        }
    }
}
