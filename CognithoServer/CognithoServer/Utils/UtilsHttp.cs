using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;

namespace CognithoServer.Utils
{
    public class UtilsHttp
    {
        public string ResponseString { get; set; }
        public HttpStatusCode StatusCode { get; set; }

        public void PostRequest(object obj, string url)
        {
            if(url.CompareTo("a") != 0)
            {
                var request = (HttpWebRequest)WebRequest.Create(url);

                string postData = JsonConvert.SerializeObject(obj);
                var data = Encoding.ASCII.GetBytes(postData);

                request.Method = "POST";
                request.ContentType = "application/json;charset=utf-8";
                request.ContentLength = data.Length;

                using (var stream = request.GetRequestStream())
                {
                    stream.Write(data, 0, data.Length);
                }

                var response = (HttpWebResponse)request.GetResponse();

                ResponseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
                StatusCode = response.StatusCode;
            }
        }
    }
}