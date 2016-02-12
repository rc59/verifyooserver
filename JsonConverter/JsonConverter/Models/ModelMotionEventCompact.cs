﻿using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace JsonConverter.Models
{
    [DataContract]
    class ModelMotionEventCompact
    {
        
        public ObjectId _id { get; set; }
        
        public double X;
        
        public double Y;

        public double VelocityX;

        public double VelocityY;
        
        public double Velocity;

        public double Pressure;
        
        public double EventTime;
        
        public double TouchSurface;
        
        public double AngleZ;
        
        public double AngleX;
        
        public double AngleY;

        public string toString(ModelMotionEventCompact prevEvent)
        {
            double accelerationX = 0;
            double accelerationY = 0;
            double acceleration = 0;

            if (prevEvent != null && VelocityX == 0 && VelocityY == 0)
            {
                double timeInterval = EventTime - prevEvent.EventTime;
                timeInterval = timeInterval / 1000;

                if(timeInterval > 0)
                {
                    VelocityX = X - prevEvent.X;
                    VelocityX = VelocityX / timeInterval;

                    VelocityY = Y - prevEvent.Y;
                    VelocityY = VelocityY / timeInterval;

                    Velocity = CalcPitagoras(VelocityX, VelocityY);

                    accelerationX = VelocityX - prevEvent.VelocityX;
                    accelerationX = accelerationX / timeInterval;

                    accelerationY = VelocityY - prevEvent.VelocityY;
                    accelerationY = accelerationY / timeInterval;

                    acceleration = CalcPitagoras(accelerationX, accelerationY);
                }
            }

            string result = string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13}", EventTime.ToString(), X.ToString(), Y.ToString(), Pressure.ToString(), TouchSurface.ToString(), AngleZ.ToString(), AngleX.ToString(), AngleY.ToString(), VelocityX.ToString(), VelocityY.ToString(), Velocity.ToString(), accelerationX.ToString(), accelerationY.ToString(), acceleration.ToString());
            return result;
        }

        double CalcPitagoras(double value1, double value2)
        {
            return Math.Sqrt(value1 * value1 + value2 * value2);
        }
    }
}
