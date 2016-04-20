using CognithoServer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace CognithoServer.Utils
{
    public class UtilsLicense
    {
        public void CheckLicense(string licenseKey)
        {
            if (licenseKey.CompareTo("123") != 0)
            {
                var resp = new HttpResponseMessage(HttpStatusCode.BadRequest)
                {
                    Content = new StringContent("License is invalid")
                };
                throw new HttpResponseException(resp);
            }
        }

        public void CheckAppKey()
        {
            string[] appId = HttpContext.Current.Request.Headers.GetValues("AppId");
            string[] appKey = HttpContext.Current.Request.Headers.GetValues("AppKey");

            if (appId != null && appId.Length >= 1 && appKey != null && appKey.Length >= 1)
            {
                string strAppId = appId[0];
                string strAppKey = appKey[0];
                Application app = new UtilsDB().GetAppById(strAppId);
                if (app.AppKey.CompareTo(strAppKey) == 0)
                {
                    return;
                }
            }

            var resp = new HttpResponseMessage(HttpStatusCode.BadRequest)
            {
                Content = new StringContent("Application key invalid")
            };
            throw new HttpResponseException(resp);
        }
    }
}