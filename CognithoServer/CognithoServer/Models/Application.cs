using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CognithoServer.Models
{
    public class Application
    {
        public ObjectId _id { get; set; }
        public string AppId { get; set; }
        public string AppKey { get; set; }
        public string LicenseKey { get; set; }
        public BsonDateTime Created { get; set; }
    }
}