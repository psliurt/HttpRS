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
        /// <summary>
        /// object instance of HttpHeaderList
        /// </summary>
        private HttpHeaderList _headers { get; set; }
        /// <summary>
        /// object instance of HttpSender
        /// </summary>
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

        /// <summary>
        /// Add Http Header.
        /// </summary>
        /// <param name="header">header string</param>
        /// <param name="val">The value of the header</param>
        public void AddHeader(string header, string val)
        {
            //The Header "Content-Type" is added by default,
            //it would not be added, if you add it again.
            if (header.ToLower().Trim() == "content-type")
            { return; }

            //add the header 
            _headers.AddHeader(header, val);
        }

        /// <summary>
        /// SendJson Method will send out the http request, which http body is the json format.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="method">the Http request method.</param>
        /// <param name="obj">the obj you want to send in json format.</param>
        /// <returns></returns>
        public ResponseResult SendJson<T>(HttpRequestMethod method, T obj) where T: class
        {
            string reqBody = JsonConvert.SerializeObject(obj);
            return _sender.SendRequest(method, reqBody, _headers);
        }
    }
}
