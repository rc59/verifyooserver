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
        
        public double TimeInterval;
        
        public double PauseBeforeStroke;
        
        public double NumEvents;
        
        public double DownTime;
        
        public double UpTime;
        
        public double PressureMax;
        
        public double PressureMin;
        
        public double PressureAvg;
        
        public double TouchSurfaceMax;
        
        public double TouchSurfaceMin;
        
        public double TouchSurfaceAvg;
        
        public double Width;
        
        public double Height;
        
        public double Area;
        
        public double RelativePosX;
        
        public double RelativePosY;
        public bool Match { get; set; }
   
        public double MatchScore { get; set; }
        public string toString()
        {
            string result = string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15},{16},{17},{18},{19}", _id.ToString(), Length.ToString(), Match.ToString(), MatchScore.ToString(), TimeInterval.ToString(), PauseBeforeStroke.ToString(), NumEvents.ToString(), DownTime.ToString(), UpTime.ToString(), PressureMax.ToString(), PressureMin.ToString(), PressureAvg.ToString(), TouchSurfaceMax.ToString(), TouchSurfaceMin.ToString(), TouchSurfaceAvg.ToString(), Width.ToString(), Height.ToString(), Area.ToString(), RelativePosX.ToString(), RelativePosY.ToString());
            return result;
        }
    }
}
