using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CognithoServer.Models
{
    public class DistributionObj
    {
        public ObjectId _id { get; set; }
        public string Name { get; set; }
        public double Average { get; set; }
        public double StandardDeviation { get; set; }
        public int __v { get; set; }
    }
}