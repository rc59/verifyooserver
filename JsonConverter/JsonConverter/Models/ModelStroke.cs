using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace JsonConverter.Models
{
    [DataContract]
    class ModelStroke
    {
        public ObjectId _id { get; set; }
        
        public List<ModelMotionEventCompact> ListEvents;

        public double Length;

        public string toString()
        {
            string result = string.Format("{0}", _id.ToString());
            return result;
        }
    }
}
