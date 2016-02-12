using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace JsonConverter.Objects
{
    [DataContract]
    class MotionEventCompact
    {
        [DataMember]
        public string _id { get; set; }
        [DataMember]
        public double X;
        [DataMember]
        public double Y;
        [DataMember]
        public double VelocityX;
        [DataMember]
        public double VelocityY;
        [DataMember]
        public double Velocity;
        [DataMember]
        public double Pressure;
        [DataMember]
        public UInt64 EventTime;
        [DataMember]
        public double TouchSurface;
        [DataMember]
        public double AngleZ;
        [DataMember]
        public double AngleX;
        [DataMember]
        public double AngleY;

        public string toString()
        {
            string result = string.Format("{0},{1},{2},{3},{4},{5},{6},{7}", EventTime.ToString(), X.ToString(), Y.ToString(), Pressure.ToString(), TouchSurface.ToString(), AngleZ.ToString(), AngleX.ToString(), AngleY.ToString());
            return result;
        }
    }
}
