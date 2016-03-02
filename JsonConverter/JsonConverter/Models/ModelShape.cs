using JsonConverter.Models;
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
    class ModelShape
    {
        public ObjectId _id { get; set; }

        public string Instruction { get; set; }

        public List<ModelStroke> Strokes;

        public DateTime Created { get; set; }


        public string toString()
        {

            string result = string.Format("{0},{1},{2},{3},{4}", Created.ToShortDateString(), Created.ToLongTimeString(), Created.Millisecond.ToString(), _id.ToString(),Instruction.ToString());
            return result;
        }
    }
}
