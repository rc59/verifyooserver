using CognithoServer.Logic.Statistics.Comparison.Interfaces;
using CognithoServer.Logic.Statistics.Comparison.Objects;
using CognithoServer.Logic.Statistics.Norms;
using CognithoServer.Models;
using CognithoServer.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CognithoServer.Logic.Statistics.Comparison.Comparers
{
    public class StrokeCompParamContainer : CompParamContainerAbstract, ICompParamContainer
    {        
        protected Stroke mStroke;

        protected UtilsCV mUtilsCV;

        public double MinPressure = double.MaxValue;
        public double MaxPressure = 0;
        public double AvgPressure = 0;

        public double MinSurface = double.MaxValue;
        public double MaxSurface = 0;
        public double AvgSurface = 0;

        public double MaxVelocity = 0;
        public double MaxVelocityX = 0;
        public double MaxVelocityY = 0;

        public double AvgVelocity = 0;
        public double AvgVelocityX = 0;
        public double AvgVelocityY = 0;

        public double TotalVelocityX = 0;
        public double TotalVelocityY = 0;

        public double MaxAcceleration = 0;
        public double MaxAccelerationX = 0;
        public double MaxAccelerationY = 0;

        public double AvgAcceleration = 0;
        public double AvgAccelerationX = 0;
        public double AvgAccelerationY = 0;

        public double AvgTremorX = 0;
        public double AvgTremorY = 0;
        public double AvgTremorZ = 0;

        public double MaxAngleChange = 0;
        public double AvgAngleChange = 0;
        public double NumLowAngleChanges = 0;
        public double NumMedAngleChanges = 0;
        public double NumHighAngleChanges = 0;

        public double MinX = double.MaxValue;
        public double MinY = double.MaxValue;
        public double MaxX = 0;
        public double MaxY = 0;

        public double Width = 0;
        public double Height = 0;
        public double BoundingBox = 0;

        public double Length = 0;
        public double TimeInterval = 0;
        public double NumEvents = 0;

        double HoughLines = 0;
        double HoughCircles = 0;
        double ContourTree = 0;

        public StrokeCompParamContainer(Stroke stroke, int instructionIdx)
        {
            mStroke = stroke;
            mInstructionIdx = instructionIdx;

            Init();
            ExtractParams();
        }

        private void ExtractParams()
        {
            if(mStroke.ListEvents.Count > 0)
            {
                double tempVelocity;
                double tempAcceleration;
                double tempAccelerationX;
                double tempAccelerationY;
                double tempAngle;
                double tempAngleChange;

                double[] angles = new double[mStroke.ListEvents.Count];
                double[] anglesChanges = new double[mStroke.ListEvents.Count];

                for (int idxEvent = 0; idxEvent < mStroke.ListEvents.Count; idxEvent++)
                {
                    MinPressure = mUtilsCalc.GetMinValue(mStroke.ListEvents[idxEvent].Pressure, MinPressure);
                    MaxPressure = mUtilsCalc.GetMaxValue(mStroke.ListEvents[idxEvent].Pressure, MaxPressure);
                    AvgPressure += mStroke.ListEvents[idxEvent].Pressure;

                    MinSurface = mUtilsCalc.GetMinValue(mStroke.ListEvents[idxEvent].TouchSurface, MinSurface);
                    MaxSurface = mUtilsCalc.GetMaxValue(mStroke.ListEvents[idxEvent].TouchSurface, MaxSurface);
                    AvgSurface += mStroke.ListEvents[idxEvent].TouchSurface;

                    MaxVelocityX = mUtilsCalc.GetMaxAbsValue(mStroke.ListEvents[idxEvent].VelocityX, MaxVelocityX);
                    MaxVelocityY = mUtilsCalc.GetMaxAbsValue(mStroke.ListEvents[idxEvent].VelocityY, MaxVelocityY);

                    TotalVelocityX += mStroke.ListEvents[idxEvent].VelocityX;
                    TotalVelocityY += mStroke.ListEvents[idxEvent].VelocityY;

                    tempVelocity = mUtilsCalc.CalcPitagoras(mStroke.ListEvents[idxEvent].VelocityX, mStroke.ListEvents[idxEvent].VelocityY);
                    MaxVelocity = mUtilsCalc.GetMaxAbsValue(tempVelocity, MaxVelocity);

                    MaxX = mUtilsCalc.GetMaxValue(mStroke.ListEvents[idxEvent].X, MaxX);
                    MaxY = mUtilsCalc.GetMaxValue(mStroke.ListEvents[idxEvent].Y, MaxY);

                    MinX = mUtilsCalc.GetMinValue(mStroke.ListEvents[idxEvent].X, MinX);
                    MinY = mUtilsCalc.GetMinValue(mStroke.ListEvents[idxEvent].Y, MinY);

                    AvgVelocity += tempVelocity;
                    AvgVelocityX += Math.Abs(mStroke.ListEvents[idxEvent].VelocityX);
                    AvgVelocityY += Math.Abs(mStroke.ListEvents[idxEvent].VelocityY);

                    if (idxEvent > 0)
                    {
                        tempAccelerationX = mStroke.ListEvents[idxEvent].VelocityX - mStroke.ListEvents[idxEvent - 1].VelocityX;
                        tempAccelerationX = tempAccelerationX / (mStroke.ListEvents[idxEvent].EventTime - mStroke.ListEvents[idxEvent - 1].EventTime);
                        tempAccelerationX = tempAccelerationX * 1000;

                        MaxAccelerationX = mUtilsCalc.GetMaxAbsValue(tempAccelerationX, MaxAccelerationX);

                        tempAccelerationY = mStroke.ListEvents[idxEvent].VelocityY - mStroke.ListEvents[idxEvent - 1].VelocityY;
                        tempAccelerationY = tempAccelerationY / (mStroke.ListEvents[idxEvent].EventTime - mStroke.ListEvents[idxEvent - 1].EventTime);
                        tempAccelerationY = tempAccelerationY * 1000;

                        MaxAccelerationY = mUtilsCalc.GetMaxAbsValue(tempAccelerationY, MaxAccelerationY);

                        tempAcceleration = mUtilsCalc.CalcPitagoras(mStroke.ListEvents[idxEvent].VelocityX, mStroke.ListEvents[idxEvent].VelocityY) -
                                           mUtilsCalc.CalcPitagoras(mStroke.ListEvents[idxEvent - 1].VelocityX, mStroke.ListEvents[idxEvent - 1].VelocityY);
                        tempAcceleration = tempAcceleration / (mStroke.ListEvents[idxEvent].EventTime - mStroke.ListEvents[idxEvent - 1].EventTime);
                        tempAcceleration = tempAcceleration * 1000;

                        MaxAcceleration = mUtilsCalc.GetMaxValue(tempAcceleration, MaxAcceleration);

                        AvgAccelerationX += Math.Abs(tempAccelerationX);
                        AvgAccelerationY += Math.Abs(tempAccelerationY);
                        AvgAcceleration += Math.Abs(tempAcceleration);

                        AvgTremorX += mStroke.ListEvents[idxEvent].AngleX - mStroke.ListEvents[idxEvent - 1].AngleX;
                        AvgTremorY += mStroke.ListEvents[idxEvent].AngleY - mStroke.ListEvents[idxEvent - 1].AngleY;
                        AvgTremorZ += mStroke.ListEvents[idxEvent].AngleZ - mStroke.ListEvents[idxEvent - 1].AngleZ;

                        tempAngle = GetAngle(mStroke.ListEvents[idxEvent - 1], mStroke.ListEvents[idxEvent]);
                        angles[idxEvent] = tempAngle;

                        if (idxEvent < mStroke.ListEvents.Count - 1)
                        {
                            tempAngleChange = GetAngleChange(mStroke.ListEvents[idxEvent - 1], mStroke.ListEvents[idxEvent], mStroke.ListEvents[idxEvent + 1]);

                            ClassifyAngleChange(tempAngleChange);
                            
                            anglesChanges[idxEvent] = tempAngleChange;

                            MaxAngleChange = mUtilsCalc.GetMaxValue(MaxAngleChange, tempAngleChange);
                            AvgAngleChange += tempAngleChange;
                        }
                    }
                }

                Width = MaxX - MinX;
                Height = MaxY - MinY;
                BoundingBox = Width * Height;

                AvgPressure = CalcAverage(AvgPressure);            
                AvgSurface = CalcAverage(AvgSurface);

                AvgVelocity = CalcAverage(AvgVelocity);
                AvgVelocityX = CalcAverage(AvgVelocityX);
                AvgVelocityY = CalcAverage(AvgVelocityY);

                AvgAcceleration = AvgAcceleration / (mStroke.ListEvents.Count - 1);
                AvgAccelerationX = AvgAccelerationX / (mStroke.ListEvents.Count - 1);
                AvgAccelerationY = AvgAccelerationY / (mStroke.ListEvents.Count - 1);

                AvgTremorX = AvgTremorX / (mStroke.ListEvents.Count - 1);
                AvgTremorY = AvgTremorY / (mStroke.ListEvents.Count - 1);
                AvgTremorZ = AvgTremorZ / (mStroke.ListEvents.Count - 1);

                AvgAngleChange = AvgAngleChange / (mStroke.ListEvents.Count - 2);

                //mUtilsCV = new UtilsCV(mStroke, MaxX, MaxY);
                //mUtilsCV.CalcCVShapeFeatures();

                //HoughLines = mUtilsCV.HoughLines;
                //HoughCircles = mUtilsCV.HoughCircles;
                //ContourTree = mUtilsCV.ContourTree;

                Length = mStroke.Length;
                NumEvents = mStroke.ListEvents.Count;
                TimeInterval = GetTimeInterval();
            }

            CreateComparisonParameter(ConstsNorms.Stroke.MIN_PRESSURE, MinPressure);
            CreateComparisonParameter(ConstsNorms.Stroke.MAX_PRESSURE, MaxPressure);
            CreateComparisonParameter(ConstsNorms.Stroke.AVG_PRESSURE, AvgPressure);

            CreateComparisonParameter(ConstsNorms.Stroke.MIN_SURFACE, MinSurface);
            CreateComparisonParameter(ConstsNorms.Stroke.MAX_SURFACE, MaxSurface);
            CreateComparisonParameter(ConstsNorms.Stroke.AVG_SURFACE, AvgSurface);

            CreateComparisonParameter(ConstsNorms.Stroke.LENGTH, Length);
            CreateComparisonParameter(ConstsNorms.Stroke.NUM_EVENTS, NumEvents);
            CreateComparisonParameter(ConstsNorms.Stroke.TIME_INTERVAL, TimeInterval);

            CreateComparisonParameter(ConstsNorms.Stroke.MAX_VELOCITY, MaxVelocity);
            CreateComparisonParameter(ConstsNorms.Stroke.MAX_VELOCITY_X, MaxVelocityX);
            CreateComparisonParameter(ConstsNorms.Stroke.MAX_VELOCITY_Y, MaxVelocityY);

            CreateComparisonParameter(ConstsNorms.Stroke.AVG_VELOCITY, AvgVelocity);
            CreateComparisonParameter(ConstsNorms.Stroke.AVG_VELOCITY_X, AvgVelocityX);
            CreateComparisonParameter(ConstsNorms.Stroke.AVG_VELOCITY_Y, AvgVelocityY);

            CreateComparisonParameter(ConstsNorms.Stroke.MAX_ACCELERATION, MaxVelocity);
            CreateComparisonParameter(ConstsNorms.Stroke.MAX_ACCELERATION_X, MaxVelocityX);
            CreateComparisonParameter(ConstsNorms.Stroke.MAX_ACCELERATION_Y, MaxVelocityY);

            CreateComparisonParameter(ConstsNorms.Stroke.AVG_ACCELERATION, AvgVelocity);
            CreateComparisonParameter(ConstsNorms.Stroke.AVG_ACCELERATION_X, AvgVelocityX);
            CreateComparisonParameter(ConstsNorms.Stroke.AVG_ACCELERATION_Y, AvgVelocityY);

            CreateComparisonParameter(ConstsNorms.Stroke.BOUNDING_BOX, BoundingBox);
            CreateComparisonParameter(ConstsNorms.Stroke.WIDTH, Width);
            CreateComparisonParameter(ConstsNorms.Stroke.HEIGHT, Height);

            CreateComparisonParameter(ConstsNorms.Stroke.HOUGH_LINES, HoughLines);
            CreateComparisonParameter(ConstsNorms.Stroke.HOUGH_CIRCLES, HoughCircles);
            CreateComparisonParameter(ConstsNorms.Stroke.CONTOUR_TREE, ContourTree);
        }

        private bool IsBetween(double value, double low, double high)
        {
            if(value > low && value < high)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void ClassifyAngleChange(double angleChange)
        {
            if (IsBetween(angleChange, Consts.ANGLE_CHANGE_THRESHOLD_LOW, Consts.ANGLE_CHANGE_THRESHOLD_MED))
            {
                NumLowAngleChanges++;
            }
            else
            {
                if (IsBetween(angleChange, Consts.ANGLE_CHANGE_THRESHOLD_MED, Consts.ANGLE_CHANGE_THRESHOLD_HIGH))
                {
                    NumMedAngleChanges++;
                }
                else
                {
                    if (angleChange > Consts.ANGLE_CHANGE_THRESHOLD_HIGH)
                    {
                        NumHighAngleChanges++;
                    }
                }
            }
        }

        private double GetAngleChange(MotionEventCompact point1, MotionEventCompact point2, MotionEventCompact point3)
        {
            double angleChange = 0;

            double angle1 = GetAngle(point1, point2);
            double angle2 = GetAngle(point2, point3);

            angleChange = Math.Abs(angle1 - angle2);

            return angleChange;
        }

        private double GetAngle(MotionEventCompact point1, MotionEventCompact point2)
        {
            double angle = 0;

            double currX = point1.X;
            double currY = point1.Y;
            double nextX = point2.X;
            double nextY = point2.Y;

            double diffX = currX - nextX;
            double diffY = currY - nextY;

            try
            {
                angle = Math.Round(Math.Atan2(diffY, diffX) * 180 / 3.14, 2);
            }
            catch (Exception exc)
            {
                angle = -1;
            }

            return angle;
        }

        protected double CalcAverage(double value)
        {
            return (value / mStroke.ListEvents.Count);
        }

        protected double GetTimeInterval()
        {
            MotionEventCompact downEvent = mStroke.ListEvents[0];
            MotionEventCompact upEvent = mStroke.ListEvents[mStroke.ListEvents.Count - 1];

            double timeInterval = upEvent.EventTime - downEvent.EventTime;

            return timeInterval;
        }
    }
}