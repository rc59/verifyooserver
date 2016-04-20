using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace CognithoServer.Utils
{
    public class UtilsErrorHandling
    {
        public void ReturnBadRequest(string msg, string errorCode)
        {
            string finalMsg = string.Format("{0}-{1}", errorCode, msg);

            var resp = new HttpResponseMessage(HttpStatusCode.BadRequest)
            {
                Content = new StringContent(finalMsg)
            };
            throw new HttpResponseException(resp);
        }
    }
}