using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
namespace HttpRS
{
    /// <summary>
    /// A warpper class, which is made for sending Http json request.
    /// </summary>
    public class JsonRequestSender
    {
        private HttpHeaderList _headers { get; set; }
        private HttpSender _sender { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="url"></param>
        public JsonRequestSender(string url)
        {
            _headers = new HttpHeaderList();
            _headers.AddHeader("Content-Type", "application/json; charset=utf-8");
            _sender = new HttpSender(url, Encoding.UTF8);
        }

        public void AddHeader(string header, string val)
        {
            //content-type預設會加，所以這邊若有使用者再次加入，就會擋掉。
            if (header.ToLower().Trim() == "content-type")
            { return; }

            _headers.AddHeader(header, val);
        }


        public ResponseResult SendJson<T>(HttpRequestMethod method, T obj) where T: class
        {
            string reqBody = JsonConvert.SerializeObject(obj);
            return _sender.SendRequest(method, reqBody, _headers);
        }
    }
}
