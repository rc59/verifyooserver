using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CognithoServer.Models
{
    public class Template
    {
        public ObjectId _id { get; set; }
        public List<GestureObj> Gestures { get; set; }
        public string OS { get; set; }
        public string Name { get; set; }
        public string DeviceId { get; set; }
        public string ModelName { get; set; }
        public double Xdpi { get; set; }
        public double Ydpi { get; set; }
        public double ScreenWidth { get; set; }
        public double ScreenHeight { get; set; }
        public BsonDateTime Created { get; set; }
        public BsonDateTime Updated { get; set; }
        public int __v { get; set; }
    }
}