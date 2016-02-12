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

        public bool Match { get; set; }

        public double TotalGames { get; set; }

        public string GcmToken { get; set; }

        public List<ModelStroke> Strokes;
        
        public bool IsSource { get; set; }
        
        public double ScreenHeight { get; set; }
        
        public double ScreenWidth { get; set; }
        
        public string Name { get; set; }
        
        public string ModelName { get; set; }
        
        public string DeviceId { get; set; }
        
        public string OS { get; set; }
        
        public string Instruction { get; set; }
        
        public DateTime Created { get; set; }
        
        public double __v { get; set; }

        public string toString()
        {
            string fixedInstruction = Instruction;
            if (fixedInstruction == "צייר צורה")
            {
                fixedInstruction = "Draw a shape";
            }

            if (fixedInstruction == "צייר שני לבבות")
            {
                fixedInstruction = "Draw two hearts";
            }

            if (fixedInstruction == "צייר לב")
            {
                fixedInstruction = "Draw a heart";
            }

            string result = string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12}", Created.ToShortDateString(), Created.ToLongTimeString(), Created.Millisecond.ToString(), _id, Name, ModelName, DeviceId, OS, fixedInstruction, Match.ToString(), IsSource.ToString(), ScreenHeight.ToString(), ScreenWidth.ToString());
            return result;
        }
    }
}
