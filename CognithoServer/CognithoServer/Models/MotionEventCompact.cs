
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CognithoServer.Models
{
    public class MotionEventCompact
    {
        public double RawX;
        public double RawY;
        public double X;
        public double Y;
        public double VelocityX;
        public double VelocityY;
        public double Pressure;
        public double EventTime;
        public double TouchSurface;
        public double PointerCount;
        public double AngleZ;
        public double AngleX;
        public double AngleY;
    }
}