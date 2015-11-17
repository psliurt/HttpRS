using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HttpRS
{
    public class ResponseResult
    {
        private string _statusMsg;

        public bool IsResultError { get; set; }
        public string ErrorMsg { get; set; }
        public string ResponseBody { get; set; }
        public int StatusCode { get; set; }        
        public string OtherNote { get; set; }
        public HttpHeaderList Headers { get; set; }

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

        private void SetStatusCode(string statusMsg)
        {
            switch (statusMsg)
            { 
                case "Accepted":
                    StatusCode = 202;
                    break;
                default:
                    StatusCode = 200;
                    break;
            }
        }
    }
}
