using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CognithoServer.Objects
{
    public class StrokeExtended
    {
        public CVShapeFeatures CVShapeFeatures;

        public double MaxVelocity;
        public double MaxVelocityX;
        public double MaxVelocityY;
        
        public double MinVelocityX;
        public double MinVelocityY;

        public double MaxPressure;
        public double MinPressure;
        public double PressureRange;

        public double MaxSurface;
        public double MinSurface;

        public double Height;
        public double Width;
        public double BoundingBox;

        public double MaxX;
        public double MinX;
        public double MaxY;
        public double MinY;

        public double DownTime;
        public double UpTime;
    }
}