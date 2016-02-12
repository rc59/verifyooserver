using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace JsonConverter.Objects
{
    [DataContract]
    class Stroke
    {
        [DataMember]
        public List<MotionEventCompact> ListEvents;
        [DataMember]
        public double Length;
        [DataMember]
        public double TimeInterval;
        [DataMember]
        public double PauseBeforeStroke;
        [DataMember]
        public double NumEvents;
        [DataMember]
        public double DownTime;
        [DataMember]
        public double UpTime;

        [DataMember]
        public double PressureMax;
        [DataMember]
        public double PressureMin;
        [DataMember]
        public double PressureAvg;

        [DataMember]
        public double TouchSurfaceMax;
        [DataMember]
        public double TouchSurfaceMin;
        [DataMember]
        public double TouchSurfaceAvg;

        [DataMember]
        public double Width;
        [DataMember]
        public double Height;
        [DataMember]
        public double Area;

        [DataMember]
        public double RelativePosX;
        [DataMember]
        public double RelativePosY;
        public bool Match { get; set; }
        public double MatchScore { get; set; }

        public string toString()
        {
            string result = string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15},{16}", Length.ToString(), TimeInterval.ToString(), PauseBeforeStroke.ToString(), NumEvents.ToString(), DownTime.ToString(), UpTime.ToString(), PressureMax.ToString(), PressureMin.ToString(), PressureAvg.ToString(), TouchSurfaceMax.ToString(), TouchSurfaceMin.ToString(), TouchSurfaceAvg.ToString(), Width.ToString(), Height.ToString(), Area.ToString(), RelativePosX.ToString(), RelativePosY.ToString());
            return result;
        }
    }
}
