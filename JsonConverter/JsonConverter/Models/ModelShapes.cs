using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using MongoDB.Bson;

namespace JsonConverter.Models
{
    [DataContract]

    class ModelShapes
    {
        public ObjectId _id { get; set; }

        public string GcmToken { get; set; }

        public bool IsSource { get; set; }

        public double ScreenHeight { get; set; }

        public double ScreenWidth { get; set; }

        public string Name { get; set; }

        public string ModelName { get; set; }

        public string DeviceId { get; set; }

        public string OS { get; set; }

        public DateTime Created { get; set; }

        public double __v { get; set; }

        public double Xdpi { get; set; }

        public double Ydpi { get; set; }

        //public string State { get; set; }

        //public string Company { get; set; }

        public List<ModelShape> ExpShapeList;

        public string toString()
        {
            string result = string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11}", Created.ToShortDateString(), Created.ToLongTimeString(), Created.Millisecond.ToString(), Name, ModelName, DeviceId, OS, IsSource.ToString(), ScreenHeight.ToString(), ScreenWidth.ToString(), Xdpi.ToString(),Ydpi.ToString());
            return result;
        }
    }
}
