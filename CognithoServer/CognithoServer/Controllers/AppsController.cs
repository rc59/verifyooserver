using CognithoServer.Models;
using CognithoServer.Utils;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace CognithoServer.Controllers
{
    public class AppsController : ApiController
    {
        [HttpPost]
        public bool Post(Application application)
        {
            new UtilsLicense().CheckLicense(application.LicenseKey);
            MongoCollection<Application> applications = new UtilsDB().GetCollectionApplications();

            application.Created = DateTime.Now;
            applications.Insert(application);

            return true;
        }
    }
}