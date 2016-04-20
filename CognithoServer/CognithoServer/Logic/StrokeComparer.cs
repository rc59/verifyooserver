using CognithoServer.Models;
using CognithoServer.Objects;
using CognithoServer.Utils;
using ShapeContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Collections;
using System.Drawing;
using CognithoServer.Logic.Statistics.Norms;
using CognithoServer.Logic.Statistics.Comparison;
using CognithoServer.Logic.Statistics.Comparison.Comparers;

namespace CognithoServer.Logic
{
    public class StrokeComparer
    {
        private string _msg;

        public int InstructionIdx;
        public StrokeResultObj StrokeResult;

        public NormMgr mNormMgr;

        public StrokeExtended StrokeExtended1, StrokeExtended2;

        private Stroke mStroke1, mStroke2;
        private List<MotionEventCompact> mNormalizedList1, mNormalizedList2;

        private double[] mVector1, mVector2;

        private double zScoreTest;

        private UtilsNormalize mUtilNormalizer;
        private UtilsCalc mUtilsCalc;

        private double mPrevCoord1X, mPrevCoord1Y, mPrevCoord2X, mPrevCoord2Y;
        private double mPrevUpTime1, mPrevUpTime2;

        public StrokeCompParamContainer CompParamsCtr1;
        public StrokeCompParamContainer CompParamsCtr2;

        public string StrParameters;

        public StrokeComparer()
        {
            StrParameters = string.Empty;

            mNormMgr = (NormMgr) new UtilsObjectMgr().GetObject(Utils.Consts.APPLICATION_OBJ_NORM_MGR);            

            StrokeResult = new StrokeResultObj();
            mUtilNormalizer = new UtilsNormalize();
            mUtilsCalc = new UtilsCalc();

            StrokeExtended1 = new StrokeExtended();
            StrokeExtended2 = new StrokeExtended();

            mPrevCoord1X = -1;
            mPrevCoord1Y = -1;
            mPrevCoord2X = -1;
            mPrevCoord2Y = -1;

            mPrevUpTime1 = -1;
            mPrevUpTime2 = -1;
        }

        public void Init(Stroke stroke1, Stroke stroke2)
        {
            try
            {
                _msg = "point1";
                zScoreTest = 0;

                if (stroke1.Length > 100 && stroke2.Length > 100)
                {
                    _msg = "point2";
                    StrokeResult.ShapeScore = -1;

                    mNormalizedList1 = new List<MotionEventCompact>();
                    mNormalizedList2 = new List<MotionEventCompact>();

                    mVector1 = null;
                    mVector2 = null;

                    mStroke1 = stroke1;
                    mStroke2 = stroke2;

                    _msg = "point3";
                    StrokeExtended1.DownTime = stroke1.ListEvents[0].EventTime;
                    StrokeExtended1.UpTime = stroke1.ListEvents[stroke1.ListEvents.Count - 1].EventTime;

                    StrokeExtended2.DownTime = stroke2.ListEvents[0].EventTime;
                    StrokeExtended2.UpTime = stroke2.ListEvents[stroke2.ListEvents.Count - 1].EventTime;

                    _msg = "point4";
                    bool isShapesIdentical = false;
                
                    isShapesIdentical = CheckIfShapesIdentical(mStroke1, mStroke2);
                
                    if (isShapesIdentical)
                    {
                        _msg = "point5";
                        StrokeResult.ShapeScore = Consts.SCORE_FOR_IDENTICAL_SHAPES;
                    }
                    else
                    {
                        _msg = "point6";
                        CompareStrokes(mStroke1, mStroke2);
                    }                                
                }
                else
                {
                    _msg = "point2";
                    if (stroke1.Length <= Consts.MIN_STROKE_LENGTH && stroke2.Length <= Consts.MIN_STROKE_LENGTH)
                    {
                        StrokeResult.ShapeScore = Consts.SCORE_FOR_IDENTICAL_SHAPES;
                    
                    }
                    else
                    {
                        StrokeResult.ShapeScore = 0;
                    }
                }
            }
            catch (Exception exc)
            {
                new UtilsErrorHandling().ReturnBadRequest(_msg, ConstsErrorCodes.ERROR_CODE_9);
            }
        }

        public void CompareStrokes(Stroke strokeVerify, Stroke strokeStored)
        {
            _msg = "point7";
            List<MotionEventCompact> eventsList = strokeStored.ListEvents;
            double length = strokeStored.Length;

            List<MotionEventCompact> eventsListVerify = strokeVerify.ListEvents;
            double lengthVerify = strokeVerify.Length;

            _msg = "point8";
            mVector1 = mUtilNormalizer.PrepareData(eventsList, length, out mNormalizedList1);
            mVector2 = mUtilNormalizer.PrepareData(eventsListVerify, lengthVerify, out mNormalizedList2);

            CalcShapeScore();

            _msg = "point9";
            CalcLengthDiffPercentage();
            _msg = "point10";
            CalcTimeIntervalDiffPercentage();
            CalcNumEventsDiffPercentage();
            CalcBoundingBoxDiff();
            _msg = "point11";
            CalcPressureDiff();
            CalcTouchSurfaceDiff();
            _msg = "point12";
            CalcRelativeDistance();
            CalcPauseBetweenStrokes();
            CalcMiniStrokes();

            _msg = "point13";
            CalcVelocityAndAcceleration();
            CalcAngles();
            _msg = "point14";
            CalcDirections();
            CalcSectionMatching();

            //CalcCVShapeFeatures();
            _msg = "point15";
            CheckPointerCounts();

            CompareNormalizedLists();
            _msg = "point16";
            //CalcZScore();

            //CalcFeatures();
            //ShapeMatch();

            //CalcStartDirection();
            //CalcStartFinishDistance();

            //if(zScoreTest > Z_SCORE_THRESHOLD)
            //{
            //    StrokeResult.ShapeScore = 0;
            //}

            _msg = "point17";
            if (!StrokeResult.PointerCountMatches)
            {
                StrokeResult.ShapeScore = 0;
            }            
        }

        private void CalcZScore()
        {
            CompParamsCtr1 = new StrokeCompParamContainer(mStroke1, InstructionIdx);
            CompParamsCtr2 = new StrokeCompParamContainer(mStroke2, InstructionIdx);

            Dictionary<string, double> dictScores = new Dictionary<string, double>();

            CompParamMgr mgr = new CompParamMgr(CompParamsCtr1, CompParamsCtr2);
            mgr.RunComparison();
            StrokeResult.ZScore = mgr.GetAvgZDiffScore();

            StrParameters = mgr.StrParameters;
        }

        private void CheckPointerCounts()
        {
            StrokeResult.PointerCountMatches = true;

            double maxPointerCounts1 = 0;
            double maxPointerCounts2 = 0;

            for (int idx = 0; idx < mNormalizedList1.Count; idx++)
            {
                if(mNormalizedList1[idx].PointerCount > maxPointerCounts1)
                {
                    maxPointerCounts1 = mNormalizedList1[idx].PointerCount;
                }

                if (mNormalizedList2[idx].PointerCount > maxPointerCounts2)
                {
                    maxPointerCounts2 = mNormalizedList2[idx].PointerCount;
                }
            }

            if(maxPointerCounts1 != maxPointerCounts2)
            {
                StrokeResult.PointerCountMatches = false;
            }
        }

        public void SetPreviousCoords(double prevCoord1X, double prevCoord1Y, double prevCoord2X, double prevCoord2Y)
        {
            mPrevCoord1X = prevCoord1X;
            mPrevCoord1Y = prevCoord1Y;
            mPrevCoord2X = prevCoord2X;
            mPrevCoord2Y = prevCoord2Y;
        }

        public void SetPreviousDownTime(double downTime1, double downTime2)
        {
            mPrevUpTime1 = downTime1;
            mPrevUpTime2 = downTime2;
        }

        protected double MeanSqr(double value1, double value2)
        {
            return Math.Sqrt(Math.Pow(value1 - value2, 2));
        }

        protected void CompareNormalizedLists()
        {
            double pressure = 0;
            double surface = 0;
            double velocity = 0;

            double maxPressure = GetMaxValue(StrokeExtended1.MaxPressure, StrokeExtended2.MaxPressure);
            double minPressure = GetMinValue(StrokeExtended1.MinPressure, StrokeExtended2.MinPressure);
            double rangePressure = maxPressure - minPressure;

            double maxSurface = GetMaxValue(StrokeExtended1.MaxSurface, StrokeExtended2.MaxSurface);
            double minSurface = GetMinValue(StrokeExtended1.MinSurface, StrokeExtended2.MinSurface);
            double rangeSurface = maxSurface - minSurface;

            double maxVelocity = GetMaxValue(StrokeExtended1.MaxVelocity, StrokeExtended2.MaxVelocity);
            double rangeVelocity = maxVelocity;

            double tempVelocity1, tempVelocity2;

            MotionEventCompact tempEvent1, tempEvent2;

            for (int idx = 0; idx < mNormalizedList1.Count; idx++)
            {
                tempEvent1 = mNormalizedList1[idx];
                tempEvent2 = mNormalizedList2[idx];

                tempVelocity1 = mUtilsCalc.CalcPitagoras(tempEvent1.VelocityX, tempEvent1.VelocityY);
                tempVelocity2 = mUtilsCalc.CalcPitagoras(tempEvent2.VelocityX, tempEvent2.VelocityY);

                pressure += MeanSqr(tempEvent1.Pressure, tempEvent2.Pressure);
                surface += MeanSqr(tempEvent1.TouchSurface, tempEvent2.TouchSurface);
                velocity += MeanSqr(tempVelocity1, tempVelocity2);
            }

            pressure = pressure / mNormalizedList1.Count;
            surface = surface / mNormalizedList1.Count;
            velocity = velocity / mNormalizedList1.Count;
        }

        protected void CalcShapeScore()
        {
            double distance = mUtilNormalizer.MinimumCosineDistance(mVector1, mVector2, 2);

            double weight = 0;
            if (distance == 0)
            {
                weight = 300;
            }
            else
            {
                weight = 1 / distance;
            }

            StrokeResult.ShapeScore = weight;
        }

        protected double[] CreateListOfEventIntervals(List<MotionEventCompact> listEvents)
        {
            double[] listEventIntervals = new double[listEvents.Count - 1];

            for (int idx = 1; idx < listEvents.Count; idx++)
            {
                listEventIntervals[idx - 1] = listEvents[idx].EventTime - listEvents[idx - 1].EventTime;
            }

            double[] sortedList = listEventIntervals.OrderBy(d => d).ToArray<double>();

            return sortedList;
        }

        protected Hashtable CreateHashOfEventIntervals(double[] listEventIntervals)
        {
            Hashtable hashEventIntervals = new Hashtable();

            for (int idx = 0; idx < listEventIntervals.Length; idx++)
            {
                if(!hashEventIntervals.ContainsKey(listEventIntervals[idx]))
                {
                    hashEventIntervals.Add(listEventIntervals[idx], true);
                }
            }

            return hashEventIntervals;
        }

        protected void CalcMiniStrokes()
        {
            double[] listEventIntervals1 = CreateListOfEventIntervals(mStroke1.ListEvents);
            double[] listEventIntervals2 = CreateListOfEventIntervals(mStroke2.ListEvents);

            Hashtable hashEventIntervals1 = CreateHashOfEventIntervals(listEventIntervals1);
            Hashtable hashEventIntervals2 = CreateHashOfEventIntervals(listEventIntervals2);
        }

        protected void CalcLengthDiffPercentage()
        {
            StrokeResult.LengthDiffPercentage = mUtilsCalc.CalcPercentage(mStroke1.Length, mStroke2.Length);
        }

        protected void CalcTimeIntervalDiffPercentage()
        {
            double stroke1DownTime = mStroke1.ListEvents[0].EventTime;
            double stroke1UpTime = mStroke1.ListEvents[mStroke1.ListEvents.Count - 1].EventTime;

            double stroke1TimeInterval = stroke1UpTime - stroke1DownTime;

            double stroke2DownTime = mStroke2.ListEvents[0].EventTime;
            double stroke2UpTime = mStroke2.ListEvents[mStroke2.ListEvents.Count - 1].EventTime;

            double stroke2TimeInterval = stroke2UpTime - stroke2DownTime;

            StrokeResult.TimeIntervalDiffPercentage = mUtilsCalc.CalcPercentage(stroke1TimeInterval, stroke2TimeInterval);
        }

        protected void CalcNumEventsDiffPercentage()
        {
            StrokeResult.NumEventsDiffPercentage = mUtilsCalc.CalcPercentage(mStroke1.ListEvents.Count, mStroke1.ListEvents.Count);
        }

        protected void CalcBoundingBoxDiff()
        {
            double height1, width1, boundingBox1, height2, width2, boundingBox2;

            CalcBoundingBoxForStroke(mStroke1, out height1, out width1, out boundingBox1, StrokeExtended1);
            CalcBoundingBoxForStroke(mStroke2, out height2, out width2, out boundingBox2, StrokeExtended2);

            StrokeResult.HeightDiffPercentage = mUtilsCalc.CalcPercentage(height1, height2);
            StrokeResult.WidthDiffPercentage = mUtilsCalc.CalcPercentage(width1, width2);
            StrokeResult.BoundingBoxDiffPercentage = mUtilsCalc.CalcPercentage(boundingBox1, boundingBox2);

            StrokeResult.HeightDiffAbsolute = Math.Abs(height1 - height2);
            StrokeResult.WidthDiffAbsolute = Math.Abs(width1 - width2);
            StrokeResult.BoundingBoxDiffAbsolute = Math.Abs(boundingBox1 - boundingBox2);
        }
                
        protected void CalcPressureDiff()
        {
            double maxPressure1 = double.MinValue;
            double minPressure1 = double.MaxValue;
            double avgPressure1 = 0;

            double maxPressure2 = double.MinValue;
            double minPressure2 = double.MaxValue;
            double avgPressure2 = 0;

            GetPressureValues(mStroke1, out maxPressure1, out minPressure1, out avgPressure1);
            GetPressureValues(mStroke2, out maxPressure2, out minPressure2, out avgPressure2);

            StrokeExtended1.MaxPressure = maxPressure1;
            StrokeExtended2.MaxPressure = maxPressure2;

            StrokeExtended1.MinPressure = minPressure1;
            StrokeExtended2.MinPressure = minPressure2;

            StrokeExtended1.PressureRange = maxPressure1 - minPressure1;
            StrokeExtended2.PressureRange = maxPressure2 - minPressure2;

            StrokeResult.MaxPressureDiff = Math.Abs(maxPressure1 - maxPressure2);
            StrokeResult.MinPressureDiff = Math.Abs(minPressure1 - minPressure2);
            StrokeResult.AvgPressureDiff = Math.Abs(avgPressure1 - avgPressure2);
        }

        protected void CalcTouchSurfaceDiff()
        {
        
            double maxSurface1 = double.MinValue;
            double minSurface1 = double.MaxValue;
            double avgSurface1 = 0;

            double maxSurface2 = double.MinValue;
            double minSurface2 = double.MaxValue;
            double avgSurface2 = 0;

            GetSurfaceValues(mStroke1, out maxSurface1, out minSurface1, out avgSurface1);
            GetSurfaceValues(mStroke2, out maxSurface2, out minSurface2, out avgSurface2);

            StrokeExtended1.MaxSurface = maxSurface1;
            StrokeExtended1.MinSurface = minSurface1;

            StrokeExtended2.MaxSurface = maxSurface2;
            StrokeExtended2.MinSurface = minSurface2;

            StrokeResult.MaxSurfaceDiff = Math.Abs(maxSurface1 - maxSurface2);
            StrokeResult.MinSurfaceDiff = Math.Abs(minSurface1 - minSurface2);
            StrokeResult.AvgSurfaceDiff = Math.Abs(avgSurface1 - avgSurface2);
        }        

        protected void CalcRelativeDistance()
        {
            if (mPrevCoord1X != -1)
            {
                double distance1X, distance1Y, distance2X, distance2Y;
                double distance1, distance2;

                distance1X = mStroke1.ListEvents[0].X - mPrevCoord1X;
                distance1Y = mStroke1.ListEvents[0].Y - mPrevCoord1Y;

                distance1 = mUtilsCalc.CalcPitagoras(distance1X, distance1Y);

                distance2X = mStroke2.ListEvents[0].X - mPrevCoord2X;
                distance2Y = mStroke2.ListEvents[0].Y - mPrevCoord2Y;

                distance2 = mUtilsCalc.CalcPitagoras(distance2X, distance2Y);

                StrokeResult.RelativeDistanceXDiffAbsolute = distance1X - distance2X;
                StrokeResult.RelativeDistanceYDiffAbsolute = distance1Y - distance2Y;
                StrokeResult.RelativeDistanceDiffAbsolute = distance1 - distance2;

                StrokeResult.RelativeDistanceXDiffPercentage = mUtilsCalc.CalcPercentage(distance1X, distance2X);
                StrokeResult.RelativeDistanceYDiffPercentage = mUtilsCalc.CalcPercentage(distance1Y, distance2Y);
                StrokeResult.RelativeDistanceDiffPercentage = mUtilsCalc.CalcPercentage(distance1, distance2);
            }
        }

        protected void CalcPauseBetweenStrokes()
        {
            if (mPrevUpTime1 != -1)
            {
                double pauseBetweenStroke1 = mStroke1.ListEvents[0].EventTime - mPrevUpTime1;
                double pauseBetweenStroke2 = mStroke2.ListEvents[0].EventTime - mPrevUpTime2;

                StrokeResult.PauseBetweenStrokesDiffAbsolute = pauseBetweenStroke1 - pauseBetweenStroke2;
                StrokeResult.PauseBetweenStrokesDiffPercentage = mUtilsCalc.CalcPercentage(pauseBetweenStroke1, pauseBetweenStroke2);
            }
        }

        private void CalcVelocityAndAcceleration()
        {
            double maxVelocity1X, maxVelocity1Y, maxVelocity1;
            double minVelocity1X, minVelocity1Y;
            double avgVelocity1X, avgVelocity1Y, avgVelocity1;

            double maxAcceleration1X, maxAcceleration1Y, maxAcceleration1;
            double minAcceleration1X, minAcceleration1Y;
            double avgAcceleration1X, avgAcceleration1Y, avgAcceleration1;

            double maxVelocity2X, maxVelocity2Y, maxVelocity2;
            double minVelocity2X, minVelocity2Y;
            double avgVelocity2X, avgVelocity2Y, avgVelocity2;

            double maxAcceleration2X, maxAcceleration2Y, maxAcceleration2;
            double minAcceleration2X, minAcceleration2Y;
            double avgAcceleration2X, avgAcceleration2Y, avgAcceleration2;

            GetMaxVelocityAndAcceleration(mStroke1, 
                out maxVelocity1X, out maxVelocity1Y, out maxVelocity1, 
                out minVelocity1X, out minVelocity1Y, 
                out avgVelocity1X, out avgVelocity1Y, out avgVelocity1, 
                out maxAcceleration1X, out maxAcceleration1Y, out maxAcceleration1,
                out minAcceleration1X, out minAcceleration1Y,
                out avgAcceleration1X, out avgAcceleration1Y, out avgAcceleration1);

            GetMaxVelocityAndAcceleration(mStroke2, 
                out maxVelocity2X, out maxVelocity2Y, out maxVelocity2, 
                out minVelocity2X, out minVelocity2Y, 
                out avgVelocity2X, out avgVelocity2Y, out avgVelocity2, 
                out maxAcceleration2X, out maxAcceleration2Y, out maxAcceleration2,
                out minAcceleration2X, out minAcceleration2Y,
                out avgAcceleration2X, out avgAcceleration2Y, out avgAcceleration2);

            StrokeExtended1.MaxVelocity = maxVelocity1;
            StrokeExtended1.MaxVelocityX = maxVelocity1X;
            StrokeExtended1.MaxVelocityY = maxVelocity1Y;

            StrokeExtended2.MaxVelocity = maxVelocity2;
            StrokeExtended2.MaxVelocityX = maxVelocity2X;
            StrokeExtended2.MaxVelocityY = maxVelocity2Y;

            StrokeExtended1.MinVelocityX = minVelocity1X;
            StrokeExtended1.MinVelocityY = minVelocity1Y;

            StrokeExtended2.MinVelocityX = minVelocity2X;
            StrokeExtended2.MinVelocityY = minVelocity2Y;

            StrokeResult.MaxVelocityXDiffAbsolute = Math.Abs(maxVelocity1X - maxVelocity2X);
            StrokeResult.MaxVelocityYDiffAbsolute = Math.Abs(maxVelocity1Y - maxVelocity2Y);
            StrokeResult.MaxVelocityDiffAbsolute = Math.Abs(maxVelocity1 - maxVelocity2);

            StrokeResult.MinVelocityXDiffAbsolute = Math.Abs(minVelocity1X - minVelocity2X);
            StrokeResult.MinVelocityYDiffAbsolute = Math.Abs(minVelocity1Y - minVelocity2Y);

            StrokeResult.MaxVelocityXDiffPercentage = mUtilsCalc.CalcPercentage(maxVelocity1X, maxVelocity2X);
            StrokeResult.MaxVelocityYDiffPercentage = mUtilsCalc.CalcPercentage(maxVelocity1Y, maxVelocity2Y);
            StrokeResult.MaxVelocityDiffPercentage = mUtilsCalc.CalcPercentage(maxVelocity1, maxVelocity2);
            
            StrokeResult.MinVelocityXDiffPercentage = mUtilsCalc.CalcPercentage(minVelocity1X, minVelocity2X);
            StrokeResult.MinVelocityYDiffPercentage = mUtilsCalc.CalcPercentage(minVelocity1Y, minVelocity2Y);

            StrokeResult.AvgVelocityXDiffAbsolute = Math.Abs(avgVelocity1X - avgVelocity2X);
            StrokeResult.AvgVelocityYDiffAbsolute = Math.Abs(avgVelocity1Y - avgVelocity2Y);
            StrokeResult.AvgVelocityDiffAbsolute = Math.Abs(avgVelocity1 - avgVelocity2);

            StrokeResult.AvgVelocityXDiffPercentage = mUtilsCalc.CalcPercentage(avgVelocity1X, avgVelocity2X);
            StrokeResult.AvgVelocityYDiffPercentage = mUtilsCalc.CalcPercentage(avgVelocity1Y, avgVelocity2Y);
            StrokeResult.AvgVelocityDiffPercentage = mUtilsCalc.CalcPercentage(avgVelocity1, avgVelocity2);

            StrokeResult.MaxAccelerationXDiffAbsolute = Math.Abs(maxAcceleration1X - maxAcceleration2X);
            StrokeResult.MaxAccelerationYDiffAbsolute = Math.Abs(maxAcceleration1Y - maxAcceleration2Y);
            StrokeResult.MaxAccelerationDiffAbsolute = Math.Abs(maxAcceleration1 - maxAcceleration2);

            StrokeResult.MinAccelerationXDiffAbsolute = Math.Abs(minAcceleration1X - minAcceleration2X);
            StrokeResult.MinAccelerationYDiffAbsolute = Math.Abs(minAcceleration1Y - minAcceleration2Y);

            StrokeResult.MaxAccelerationXDiffPercentage = mUtilsCalc.CalcPercentage(maxAcceleration1X, maxAcceleration2X);
            StrokeResult.MaxAccelerationYDiffPercentage = mUtilsCalc.CalcPercentage(maxAcceleration1Y, maxAcceleration2Y);
            StrokeResult.MaxAccelerationDiffPercentage = mUtilsCalc.CalcPercentage(maxAcceleration1, maxAcceleration2);

            StrokeResult.MinAccelerationXDiffPercentage = mUtilsCalc.CalcPercentage(minAcceleration1X, minAcceleration2X);
            StrokeResult.MinAccelerationYDiffPercentage = mUtilsCalc.CalcPercentage(minAcceleration1Y, minAcceleration2Y);

            StrokeResult.AvgAccelerationXDiffAbsolute = Math.Abs(avgAcceleration1X - avgAcceleration2X);
            StrokeResult.AvgAccelerationYDiffAbsolute = Math.Abs(avgAcceleration1Y - avgAcceleration2Y);
            StrokeResult.AvgAccelerationDiffAbsolute = Math.Abs(avgAcceleration1 - avgAcceleration2);

            StrokeResult.AvgAccelerationXDiffPercentage = mUtilsCalc.CalcPercentage(avgAcceleration1X, avgAcceleration2X);
            StrokeResult.AvgAccelerationYDiffPercentage = mUtilsCalc.CalcPercentage(avgAcceleration1Y, avgAcceleration2Y);
            StrokeResult.AvgAccelerationDiffPercentage = mUtilsCalc.CalcPercentage(avgAcceleration1, avgAcceleration2);
        }

        private void CalcAngles()
        {
            int allowedDiff = 10;

            double[] angles1 = new double[mVector1.Length / 2];
            double[] angles2 = new double[mVector1.Length / 2];

            double[] anglesChanges1 = new double[(mVector1.Length / 2) - 1];
            double[] anglesChanges2 = new double[(mVector1.Length / 2) - 1];

            int count = 0;

            for (int idxCoord = 0; idxCoord < mVector1.Length; idxCoord += 2)
            {
                if (idxCoord + 3 >= mVector1.Length)
                {
                    break;
                }

                angles1[count] = GetAngle(mVector1, idxCoord);
                angles2[count] = GetAngle(mVector2, idxCoord);

                count++;
            }

            double countMismatches = 0;
            double countMismatchesDiff = 0;
            double countMismatchesAngleChanges = 0;

            double tempAngleDelta;

            for (int idxAngle = 0; idxAngle < angles1.Length; idxAngle++)
            {
                if(idxAngle > 0)
                {
                    anglesChanges1[idxAngle - 1] = angles1[idxAngle] - angles1[idxAngle - 1];
                    anglesChanges2[idxAngle - 1] = angles2[idxAngle] - angles2[idxAngle - 1];
                }

                if ((angles1[idxAngle] > 0 && angles2[idxAngle] < 0) || (angles1[idxAngle] < 0 && angles2[idxAngle] > 0))
                {
                    countMismatches++;
                }

                tempAngleDelta = angles1[idxAngle] - angles2[idxAngle];
                if (Math.Abs(tempAngleDelta) > allowedDiff)
                {
                    countMismatchesDiff++;
                }
            }

            for (int idxAngle = 0; idxAngle < anglesChanges1.Length; idxAngle++)
            {
                if ((anglesChanges1[idxAngle] > 0 && anglesChanges2[idxAngle] < 0) || (anglesChanges1[idxAngle] < 0 && anglesChanges2[idxAngle] > 0))
                {
                    countMismatchesAngleChanges++;
                }
            }

            double total = angles1.Length;
            double successRate = total - countMismatches;
            double successRateDiff = total - countMismatchesDiff;
            double successRateChanges = total - 1 - countMismatchesAngleChanges;

            StrokeResult.AngleMatches = successRate / total;
            StrokeResult.AngleMatchesDiff = successRateDiff / total;
            StrokeResult.AngleChangesMatches = successRateChanges / (total - 1);
        }

        private void CalcDirections()
        {
            MotionEventCompact tempEvent1, tempEvent2;

            double totalScore = mNormalizedList1.Count;
            double currentScore = totalScore;

            for (int idxEvent = 0; idxEvent < mNormalizedList1.Count; idxEvent++)
            {
                tempEvent1 = mNormalizedList1[idxEvent];
                tempEvent2 = mNormalizedList2[idxEvent];

                if((tempEvent1.VelocityX > 0 && tempEvent2.VelocityX < 0) || (tempEvent1.VelocityX < 0 && tempEvent2.VelocityX > 0))
                {
                    currentScore--;
                }
            }

            StrokeResult.VeclocityDirectionMatches = (currentScore / totalScore);
        }

        private void CalcSectionMatching()
        {
            double min1X = double.MaxValue;
            double min2X = double.MaxValue;
            double min1Y = double.MaxValue;
            double min2Y = double.MaxValue;

            double max1X = double.MinValue;
            double max2X = double.MinValue;
            double max1Y = double.MinValue;
            double max2Y = double.MinValue;

            for (int idxEvent = 0; idxEvent < mNormalizedList1.Count; idxEvent++)
            {
                if (mNormalizedList1[idxEvent].X < min1X)
                {
                    min1X = mNormalizedList1[idxEvent].X;
                }

                if (mNormalizedList1[idxEvent].Y < min1Y)
                {
                    min1Y = mNormalizedList1[idxEvent].Y;
                }

                if (mNormalizedList1[idxEvent].X > max1X)
                {
                    max1X = mNormalizedList1[idxEvent].X;
                }

                if (mNormalizedList1[idxEvent].Y > max1Y)
                {
                    max1Y = mNormalizedList1[idxEvent].Y;
                }

                if (mNormalizedList2[idxEvent].X < min2X)
                {
                    min2X = mNormalizedList2[idxEvent].X;
                }

                if (mNormalizedList2[idxEvent].Y < min2Y)
                {
                    min2Y = mNormalizedList2[idxEvent].Y;
                }

                if (mNormalizedList2[idxEvent].X > max2X)
                {
                    max2X = mNormalizedList2[idxEvent].X;
                }

                if (mNormalizedList2[idxEvent].Y > max2Y)
                {
                    max2Y = mNormalizedList2[idxEvent].Y;
                }
            }            

            double score = GetSectionMatchScore(5, 0,
                min1X, min1Y, min2X, min2Y,
                max1X, max1Y, max2X, max2Y);
        }

        //private VectorOfPoint EventsListToCVShape(List<MotionEventCompact> listEvents)
        //{
        //    VectorOfPoint cvShape = new VectorOfPoint();
        //    Point[] listPoints = new Point[listEvents.Count];

        //    for (int idx = 0; idx < listEvents.Count; idx++)
        //    {
        //        listPoints[idx] = new Point((int)listEvents[idx].X, (int)listEvents[idx].Y);
        //    }

        //    cvShape.Push(listPoints);

        //    return cvShape;
        //}

        //private double[] MatchShapesCV(VectorOfPoint shape1, VectorOfPoint shape2)
        //{
        //    double[] scores = new double[3];

        //    scores[0] = CvInvoke.MatchShapes(shape1, shape2, Emgu.CV.CvEnum.ContoursMatchType.I1);
        //    scores[1] = CvInvoke.MatchShapes(shape1, shape2, Emgu.CV.CvEnum.ContoursMatchType.I2);
        //    scores[2] = CvInvoke.MatchShapes(shape1, shape2, Emgu.CV.CvEnum.ContoursMatchType.I3);

        //    return scores;
        //}

        //private UMat EventsListToUMat(List<MotionEventCompact> listEvents, int rows, int cols)
        //{
        //    Mat mat = new Mat(rows, cols, DepthType.Cv8U, 3);

        //    Image<Bgr, byte> img = mat.ToImage<Bgr, byte>();
        //    Point tempPoint1, tempPoint2;

        //    for (int idx = 1; idx < listEvents.Count; idx++)
        //    {
        //        tempPoint1 = new Point((int)listEvents[idx - 1].X, (int)listEvents[idx - 1].Y);
        //        tempPoint2 = new Point((int)listEvents[idx].X, (int)listEvents[idx].Y);
        //        img.Draw(new LineSegment2D(tempPoint1, tempPoint2), new Bgr(Color.Red), 3);
        //    }

        //    UMat uimage = new UMat();
        //    CvInvoke.CvtColor(img, uimage, ColorConversion.Bgr2Gray);

        //    //use image pyr to remove noise
        //    UMat pyrDown1 = new UMat();
        //    CvInvoke.PyrDown(uimage, pyrDown1);
        //    CvInvoke.PyrUp(pyrDown1, uimage);

        //    return uimage;
        //}

        //private CircleF[] GetHoughCircles(UMat uimage)
        //{
        //    double cannyThreshold = 180.0;
        //    double circleAccumulatorThreshold = 120;
        //    CircleF[] circles = CvInvoke.HoughCircles(uimage, HoughType.Gradient, 2.0, 20.0, cannyThreshold, circleAccumulatorThreshold, 5);

        //    return circles;
        //}

        //private LineSegment2D[] GetHoughLines(UMat uimage)
        //{
        //    double cannyThreshold = 180.0;
        //    double cannyThresholdLinking = 120.0;
        //    UMat cannyEdges = new UMat();
        //    CvInvoke.Canny(uimage, cannyEdges, cannyThreshold, cannyThresholdLinking);

        //    LineSegment2D[] lines = CvInvoke.HoughLinesP(cannyEdges, 1, Math.PI / 45.0, 20, 30, 10);

        //    return lines;
        //}

        //private int[,] GetContourTree(UMat uimage)
        //{
        //    Mat canny = new Mat();
        //    VectorOfVectorOfPoint contours = new VectorOfVectorOfPoint();

        //    CvInvoke.Canny(uimage, canny, 100, 50, 3, false);
        //    int[,] hierachy = CvInvoke.FindContourTree(canny, contours, ChainApproxMethod.ChainApproxSimple);

        //    return hierachy;
        //}        

        //public void CalcCVShapeFeatures()
        //{
        //    VectorOfPoint cvShapeNormalized1 = EventsListToCVShape(mNormalizedList1);
        //    VectorOfPoint cvShapeNormalized2 = EventsListToCVShape(mNormalizedList2);

        //    VectorOfPoint cvShapeOriginal1 = EventsListToCVShape(mStroke1.ListEvents);
        //    VectorOfPoint cvShapeOriginal2 = EventsListToCVShape(mStroke2.ListEvents);
           
        //    double[] scoresOriginal = MatchShapesCV(cvShapeOriginal1, cvShapeOriginal2);
        //    double[] scoresNormalized = MatchShapesCV(cvShapeNormalized1, cvShapeNormalized2);

        //    int rows = (int) GetMaxValue(StrokeExtended1.MaxX, StrokeExtended2.MaxX);
        //    int colums = (int) GetMaxValue(StrokeExtended1.MaxY, StrokeExtended2.MaxY);

        //    UMat uimage1 = EventsListToUMat(mStroke1.ListEvents, rows, colums);
        //    UMat uimage2 = EventsListToUMat(mStroke2.ListEvents, rows, colums);

        //    int[,] contourTree1 = GetContourTree(uimage1);
        //    int[,] contourTree2 = GetContourTree(uimage2);          

        //    CircleF[] circles1 = GetHoughCircles(uimage1);
        //    CircleF[] circles2 = GetHoughCircles(uimage2);

        //    LineSegment2D[] linesp1 = GetHoughLines(uimage1);
        //    LineSegment2D[] linesp2 = GetHoughLines(uimage2);

        //    StrokeResult.CVShapeFeatures = new CVShapeFeatures();
        //    StrokeResult.CVShapeFeatures.Score1 = scoresOriginal[0];
        //    StrokeResult.CVShapeFeatures.Score2 = scoresOriginal[1];
        //    StrokeResult.CVShapeFeatures.Score3 = scoresOriginal[2];

        //    StrokeResult.CVShapeFeatures.NormalizedScore1 = scoresNormalized[0];
        //    StrokeResult.CVShapeFeatures.NormalizedScore2 = scoresNormalized[1];
        //    StrokeResult.CVShapeFeatures.NormalizedScore3 = scoresNormalized[2];

        //    StrokeResult.CVShapeFeatures.CirclesDiffAbsolute = Math.Abs(circles1.Length - circles2.Length);
        //    StrokeResult.CVShapeFeatures.CirclesDiffPercentage = mUtilsCalc.CalcPercentage(circles1.Length, circles2.Length);

        //    StrokeResult.CVShapeFeatures.LinesDiffAbsolute = Math.Abs(linesp1.Length - linesp2.Length);
        //    StrokeResult.CVShapeFeatures.LinesDiffPercentage = mUtilsCalc.CalcPercentage(linesp1.Length, linesp2.Length);

        //    StrokeResult.CVShapeFeatures.ContourTreeDiffAbsolute = Math.Abs(contourTree1.Length - contourTree2.Length);
        //    StrokeResult.CVShapeFeatures.ContourTreeDiffPercentage = mUtilsCalc.CalcPercentage(contourTree1.Length, contourTree2.Length);
        //}

        #region Aid Methods

        private double GetMaxValue(double value1, double value2)
        {
            if (value1 > value2)
            {
                return value1;
            }
            else
            {
                return value2;
            }
        }

        private double GetMinValue(double value1, double value2)
        {
            if (value1 < value2)
            {
                return value1;
            }
            else
            {
                return value2;
            }
        }

        private double GetAngle(double[] vector, int idx)
        {
            double angle = 0;

            double currX = vector[idx];
            double currY = vector[idx + 1];
            double nextX = vector[idx + 2];
            double nextY = vector[idx + 3];

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

        private void GetMaxVelocityAndAcceleration(Stroke stroke, 
            out double maxVelocityX, out double maxVelocityY, out double maxVelocity,
            out double minVelocityX, out double minVelocityY,
            out double avgVelocityX, out double avgVelocityY, out double avgVelocity, 
            out double maxAccelerationX, out double maxAccelerationY, out double maxAcceleration,
            out double minAccelerationX, out double minAccelerationY,
            out double avgAccelerationX, out double avgAccelerationY, out double avgAcceleration)
        {
            maxVelocityX = double.MinValue;
            maxVelocityY = double.MinValue;
            maxVelocity = double.MinValue;

            minVelocityX = double.MaxValue;
            minVelocityY = double.MaxValue;

            avgVelocityX = 0;
            avgVelocityY = 0;
            avgVelocity = 0;

            maxAccelerationX = double.MinValue;
            maxAccelerationY = double.MinValue;
            maxAcceleration = double.MinValue;

            minAccelerationX = double.MaxValue;
            minAccelerationY = double.MaxValue;

            avgAccelerationX = 0;
            avgAccelerationY = 0;
            avgAcceleration = 0;

            double tempVelocityX = 0;
            double tempVelocityY = 0;
            double tempVelocity = 0;

            double tempAccelerationX, tempAccelerationY, tempAcceleration;

            int numEvents = stroke.ListEvents.Count;

            double deltaX, deltaY, delta;
            double deltaTime;

            for (int idxEvent = 0; idxEvent < numEvents; idxEvent++)
            {
                if (idxEvent > 0)
                {
                    deltaX = stroke.ListEvents[idxEvent].VelocityX - tempVelocityX;
                    deltaY = stroke.ListEvents[idxEvent].VelocityY - tempVelocityY;
                    delta = mUtilsCalc.CalcPitagoras(deltaX, deltaY);

                    deltaTime = stroke.ListEvents[idxEvent].EventTime - stroke.ListEvents[idxEvent - 1].EventTime;

                    tempAccelerationX = deltaX / deltaTime;
                    tempAccelerationY = deltaY / deltaTime;
                    tempAcceleration = delta / deltaTime;

                    if (tempAccelerationX > maxAccelerationX)
                    {
                        maxAccelerationX = tempAccelerationX;                        
                    }

                    if (tempAccelerationY > maxAccelerationY)
                    {
                        maxAccelerationY = tempAccelerationY;
                    }

                    if (tempAcceleration > maxAcceleration)
                    {
                        maxAcceleration = tempAcceleration;
                    }

                    if (tempAccelerationX < minAccelerationX)
                    {
                        minAccelerationX = tempAccelerationX;
                    }

                    if (tempAccelerationY < minAccelerationY)
                    {
                        minAccelerationY = tempAccelerationY;
                    }
                    
                    avgAccelerationX += Math.Abs(tempAccelerationX);
                    avgAccelerationY += Math.Abs(tempAccelerationY);
                    avgAcceleration += Math.Abs(tempAcceleration);
                }

                tempVelocityX = stroke.ListEvents[idxEvent].VelocityX;
                tempVelocityY = stroke.ListEvents[idxEvent].VelocityY;
                tempVelocity = mUtilsCalc.CalcPitagoras(tempVelocityX, tempVelocityY);

                if(tempVelocityX > maxVelocityX)
                {
                    maxVelocityX = tempVelocityX;
                }

                if (tempVelocityY > maxVelocityY)
                {
                    maxVelocityY = tempVelocityY;
                }

                if (tempVelocity > maxVelocity)
                {
                    maxVelocity = tempVelocity;
                }

                if (tempVelocityX < minVelocityX)
                {
                    minVelocityX = tempVelocityX;
                }

                if (tempVelocityY < minVelocityY)
                {
                    minVelocityY = tempVelocityY;
                }

                avgVelocityX += Math.Abs(tempVelocityX);
                avgVelocityY += Math.Abs(tempVelocityY);
                avgVelocity += Math.Abs(tempVelocity);                
            }

            avgVelocityX = avgVelocityX / numEvents;
            avgVelocityY = avgVelocityY / numEvents;
            avgVelocity = avgVelocity / numEvents;

            avgAccelerationX = avgAccelerationX / numEvents;
            avgAccelerationY = avgAccelerationY / numEvents;
            avgAcceleration = avgAcceleration / numEvents;
        }

        private bool CheckIfShapesIdentical(Stroke strokeVerify, Stroke strokeStored)
        {
            List<MotionEventCompact> listVerify = strokeVerify.ListEvents;
            List<MotionEventCompact> listStored = strokeStored.ListEvents;

            bool isShapesIdentical = true;

            if (listStored.Count == listVerify.Count)
            {
                for (int idxEvent = 0; idxEvent < listStored.Count; idxEvent++)
                {
                    if (listStored[idxEvent].X != listVerify[idxEvent].X || listStored[idxEvent].Y != listVerify[idxEvent].Y)
                    {
                        isShapesIdentical = false;
                        break;
                    }
                }
            }
            else
            {
                isShapesIdentical = false;
            }

            return isShapesIdentical;
        }

        protected void CalcBoundingBoxForStroke(Stroke stroke, out double height, out double width, out double area, StrokeExtended strokeExtended)
        {
            double maxX = double.MinValue;
            double maxY = double.MinValue;

            double minX = double.MaxValue;
            double minY = double.MaxValue;

            for (int idxCoord = 0; idxCoord < stroke.ListEvents.Count; idxCoord++)
            {
                if (stroke.ListEvents[idxCoord].X > maxX)
                {
                    maxX = stroke.ListEvents[idxCoord].X;
                }

                if (stroke.ListEvents[idxCoord].Y > maxY)
                {
                    maxY = stroke.ListEvents[idxCoord].Y;
                }

                if (stroke.ListEvents[idxCoord].X < minX)
                {
                    minX = stroke.ListEvents[idxCoord].X;
                }

                if (stroke.ListEvents[idxCoord].Y < minY)
                {
                    minY = stroke.ListEvents[idxCoord].Y;
                }
            }

            height = maxY - minY;
            width = maxX - minX;
            area = height * width;

            strokeExtended.MaxX = maxX;
            strokeExtended.MaxY = maxY;

            strokeExtended.MinX = minX;
            strokeExtended.MinY = minY;

            strokeExtended.Height = height;
            strokeExtended.Width = width;
            strokeExtended.BoundingBox = area;
        }


        protected void GetPressureValues(Stroke stroke, out double maxPressure, out double minPressure, out double avgPressure)
        {
            maxPressure = double.MinValue;
            minPressure = double.MaxValue;
            avgPressure = 0;

            for (int idxEvent = 0; idxEvent < stroke.ListEvents.Count; idxEvent++)
            {
                if (stroke.ListEvents[idxEvent].Pressure > maxPressure)
                {
                    maxPressure = stroke.ListEvents[idxEvent].Pressure;
                }

                if (stroke.ListEvents[idxEvent].Pressure < minPressure)
                {
                    minPressure = stroke.ListEvents[idxEvent].Pressure;
                }

                avgPressure += stroke.ListEvents[idxEvent].Pressure;
            }

            avgPressure = avgPressure / stroke.ListEvents.Count;
        }

        protected void GetSurfaceValues(Stroke stroke, out double maxSurface, out double minSurface, out double avgSurface)
        {
            maxSurface = double.MinValue;
            minSurface = double.MaxValue;
            avgSurface = 0;

            for (int idxEvent = 0; idxEvent < stroke.ListEvents.Count; idxEvent++)
            {
                if (stroke.ListEvents[idxEvent].TouchSurface > maxSurface)
                {
                    maxSurface = stroke.ListEvents[idxEvent].TouchSurface;
                }

                if (stroke.ListEvents[idxEvent].TouchSurface < minSurface)
                {
                    minSurface = stroke.ListEvents[idxEvent].TouchSurface;
                }

                avgSurface += stroke.ListEvents[idxEvent].TouchSurface;
            }

            avgSurface = avgSurface / stroke.ListEvents.Count;
        }

        private double GetSectionMatchScore(int numSections, double error,
            double min1X, double min1Y, double min2X, double min2Y,
            double max1X, double max1Y, double max2X, double max2Y)
        {
            List<ScreenSection> list1X = CreateScreenSections(min1X, max1X, numSections);
            List<ScreenSection> list1Y = CreateScreenSections(min1Y, max1Y, numSections);

            List<ScreenSection> list2X = CreateScreenSections(min2X, max2X, numSections);
            List<ScreenSection> list2Y = CreateScreenSections(min2Y, max2Y, numSections);

            int[] section1X = new int[mNormalizedList1.Count];
            int[] section2X = new int[mNormalizedList1.Count];
            int[] section1Y = new int[mNormalizedList1.Count];
            int[] section2Y = new int[mNormalizedList1.Count];

            int sectionNum1X, sectionNum1Y, sectionNum2X, sectionNum2Y;
            double misses = 0;

            int diffX, diffY;

            for (int idxEvent = 0; idxEvent < mNormalizedList1.Count; idxEvent++)
            {
                sectionNum1X = GetSectionNumber(mNormalizedList1[idxEvent].X, list1X);
                sectionNum2X = GetSectionNumber(mNormalizedList2[idxEvent].X, list2X);

                sectionNum1Y = GetSectionNumber(mNormalizedList1[idxEvent].Y, list1Y);
                sectionNum2Y = GetSectionNumber(mNormalizedList2[idxEvent].Y, list2Y);

                diffX = Math.Abs(sectionNum1X - sectionNum2X);
                diffY = Math.Abs(sectionNum1Y - sectionNum2Y);

                if (diffX > error || diffY > error)
                {
                    misses++;
                }
            }

            return ((mNormalizedList1.Count - misses) / mNormalizedList1.Count);
        }

        //private double GetScoreForNumSections(double numSections, double error, double[] vector1X, double[] vector1Y, double[] vector2X, double[] vector2Y, double max1X, double max1Y, double max2X, double max2Y)
        //{
        //    List<ScreenSection> list1X = CreateScreenSections(max1X, numSections);
        //    List<ScreenSection> list1Y = CreateScreenSections(max1Y, numSections);

        //    List<ScreenSection> list2X = CreateScreenSections(max2X, numSections);
        //    List<ScreenSection> list2Y = CreateScreenSections(max2Y, numSections);

        //    int sectionNum1X, sectionNum1Y, sectionNum2X, sectionNum2Y;
        //    double misses = 0;

        //    int[] section1X = new int[vector1X.Length];
        //    int[] section2X = new int[vector1X.Length];
        //    int[] section1Y = new int[vector1X.Length];
        //    int[] section2Y = new int[vector1X.Length];

        //    int diffX, diffY;

        //    for (int idx = 0; idx < vector1X.Length; idx++)
        //    {
        //        sectionNum1X = GetSectionNumber(vector1X[idx], list1X);
        //        sectionNum2X = GetSectionNumber(vector2X[idx], list2X);

        //        sectionNum1Y = GetSectionNumber(vector1Y[idx], list1Y);
        //        sectionNum2Y = GetSectionNumber(vector2Y[idx], list2Y);

        //        section1X[idx] = sectionNum1X;
        //        section2X[idx] = sectionNum2X;

        //        section1Y[idx] = sectionNum1Y;
        //        section2Y[idx] = sectionNum2Y;

        //        diffX = Math.Abs(sectionNum1X - sectionNum2X);
        //        diffY = Math.Abs(sectionNum1Y - sectionNum2Y);

        //        if (diffX + diffY > error)
        //        {
        //            misses++;
        //        }
        //    }

        //    double score = (vector1X.Length - misses) / vector1X.Length;
        //    return score;
        //}

        //private double GetScoreForNumSectionsWithSize(double numSections, double error, double[] vector1X, double[] vector1Y, double[] vector2X, double[] vector2Y, double maxX, double maxY)
        //{
        //    List<ScreenSection> listX = CreateScreenSections(maxX, numSections);
        //    List<ScreenSection> listY = CreateScreenSections(maxY, numSections);

        //    int sectionNum1X, sectionNum1Y, sectionNum2X, sectionNum2Y;
        //    double misses = 0;

        //    int[] section1X = new int[vector1X.Length];
        //    int[] section2X = new int[vector1X.Length];
        //    int[] section1Y = new int[vector1X.Length];
        //    int[] section2Y = new int[vector1X.Length];

        //    int diffX, diffY;

        //    for (int idx = 0; idx < vector1X.Length; idx++)
        //    {
        //        sectionNum1X = GetSectionNumber(vector1X[idx], listX);
        //        sectionNum2X = GetSectionNumber(vector2X[idx], listX);

        //        sectionNum1Y = GetSectionNumber(vector1Y[idx], listY);
        //        sectionNum2Y = GetSectionNumber(vector2Y[idx], listY);

        //        section1X[idx] = sectionNum1X;
        //        section2X[idx] = sectionNum2X;

        //        section1Y[idx] = sectionNum1Y;
        //        section2Y[idx] = sectionNum2Y;

        //        diffX = Math.Abs(sectionNum1X - sectionNum2X);
        //        diffY = Math.Abs(sectionNum1Y - sectionNum2Y);

        //        if (diffX + diffY > error)
        //        {
        //            misses++;
        //        }
        //    }

        //    double score = (vector1X.Length - misses) / vector1X.Length;
        //    return score;
        //}

        private int GetSectionNumber(double value, List<ScreenSection> list)
        {
            int section = -1;

            for (int idx = 0; idx < list.Count; idx++)
            {
                if (value >= list[idx].Min && value <= list[idx].Max)
                {
                    section = idx;
                    break;
                }
            }

            return section;
        }


        private List<ScreenSection> CreateScreenSections(double minValue, double maxValue, double numSections)
        {
            List<ScreenSection> list = new List<ScreenSection>();
            ScreenSection tempSection;

            double hop = (maxValue - minValue) / numSections;
            double currentValue = minValue;

            for (int idx = 0; idx < numSections; idx++)
            {
                tempSection = new ScreenSection(currentValue, currentValue + hop);
                list.Add(tempSection);
                currentValue += hop;
            }

            return list;
        }


        private void GetMinMaxCoords(Stroke stroke, out double minX, out double maxX, out double minY, out double maxY)
        {
            minX = double.MaxValue;
            minY = double.MaxValue;

            maxX = double.MinValue;
            maxY = double.MinValue;

            for (int idx = 0; idx < stroke.ListEvents.Count; idx++)
            {
                if (stroke.ListEvents[idx].X > maxX)
                {
                    maxX = stroke.ListEvents[idx].X;
                }

                if (stroke.ListEvents[idx].X < minX)
                {
                    minX = stroke.ListEvents[idx].X;
                }

                if (stroke.ListEvents[idx].Y > maxY)
                {
                    maxY = stroke.ListEvents[idx].Y;
                }

                if (stroke.ListEvents[idx].Y < minY)
                {
                    minY = stroke.ListEvents[idx].Y;
                }
            }
        }

        #endregion

        #region Test

        public void CalcFeatures()
        {
            double gravity1X, gravity1Y, gravity2X, gravity2Y;

            double[] vector1X = null;
            double[] vector1Y = null;
            double[] vector2X = null;
            double[] vector2Y = null;

            Stroke tempStroke1 = new Stroke();
            tempStroke1.ListEvents = mNormalizedList1;

            Stroke tempStroke2 = new Stroke();
            tempStroke2.ListEvents = mNormalizedList2;

            SignatureRec(tempStroke1, StrokeExtended1, out gravity1X, out gravity1Y, out vector1X, out vector1Y);
            SignatureRec(tempStroke2, StrokeExtended2, out gravity2X, out gravity2Y, out vector2X, out vector2Y);
        }

        private int[,] Hough(double[] vectorX, double[] vectorY)
        {
            int w = vectorX.Length;
            int h = vectorY.Length;

            double pmax = Math.Sqrt(((w / 2) * (w / 2)) + ((h / 2) * (h / 2)));
            double tmax = Math.PI * 2;

            // step sizes
            double dp = pmax / (double)w;
            double dt = tmax / (double)h;

            int[,] A = new int[w * 2, h * 2]; // accumulator array

            for (int x = 0; x < w; x++)
            {
                for (int y = 0; y < h; y++)
                {
                    if (vectorX[x] > 0 && vectorY[y] > 0) // pixel is white - h-y flips incoming img
                    {
                        // book claims it's j = 1, i think it should be j = 0
                        for (int j = 0; j < h; j++)
                        {
                            double rho = ((double)(x - (w / 2)) * Math.Cos(dt * (double)j)) + ((double)(y - (h / 2)) * Math.Sin(dt * (double)j));
                            // find index k of A closest to row
                            int k = (int)((rho / pmax) * w);
                            if (k >= 0 && k < w) A[k, j]++;
                        }
                    }
                }
            }

            return A;
        }

        private void SignatureRec(Stroke stroke, StrokeExtended strokeExtended, out double gravityX, out double gravityY, out double[] vectorX, out double[] vectorY)
        {
            gravityX = -1;
            gravityY = -1;
            int numSections = 16;
            double gamma = CalcGamma(strokeExtended.Height, strokeExtended.Width);

            double minX, minY, maxX, maxY;
            GetMinMaxCoords(stroke, out minX, out maxX, out minY, out maxY);

            for (int idxEvent = 0; idxEvent < stroke.ListEvents.Count; idxEvent++)
            {
                stroke.ListEvents[idxEvent].X = stroke.ListEvents[idxEvent].X - minX;
                stroke.ListEvents[idxEvent].Y = stroke.ListEvents[idxEvent].Y - minY;
            }

            GetMinMaxCoords(stroke, out minX, out maxX, out minY, out maxY);

            int sizeX = (int)maxX;
            int sizeY = (int)maxY;

            vectorX = new double[sizeX + 1];
            vectorY = new double[sizeY + 1];

            int tempIdx;

            for (int idxEvent = 0; idxEvent < stroke.ListEvents.Count; idxEvent++)
            {
                tempIdx = (int)stroke.ListEvents[idxEvent].X;
                vectorX[tempIdx]++;

                tempIdx = (int)stroke.ListEvents[idxEvent].Y;
                vectorY[tempIdx]++;
            }

            double deltaX = maxX / numSections;
            double deltaY = maxY / numSections;

            int[] vectorNormX = new int[numSections];
            int[] vectorNormY = new int[numSections];

            for (int idx = 0; idx < numSections; idx++)
            {
                tempIdx = (int)(idx * deltaX);
                vectorNormX[idx] = (int)Math.Ceiling((vectorX[tempIdx] / deltaX));

                tempIdx = (int)(idx * deltaY);
                vectorNormY[idx] = (int)Math.Ceiling((vectorY[tempIdx] / deltaY));
            }

            GetGravityCentre(vectorX, out gravityX);
            GetGravityCentre(vectorY, out gravityY);

            int[,] A = Hough(vectorX, vectorY);
        }

        private double CalcGamma(double height, double width)
        {
            double gamma;

            if (width >= height)
            {
                gamma = width / height;
            }
            else
            {
                gamma = -1 * (height / width);
            }

            return gamma;
        }

        private void GetGravityCentre(double[] vector, out double gravityCoord)
        {
            gravityCoord = -1;

            double gravityThreashold = 0;

            for (int idx = 0; idx < vector.Length; idx++)
            {
                gravityThreashold += vector[idx];
            }

            gravityThreashold = gravityThreashold / 2;

            double sum = 0;

            for (int idx = 0; idx < vector.Length - 1; idx++)
            {
                sum += vector[idx];

                if (sum < gravityThreashold && (sum + vector[idx + 1]) >= gravityThreashold)
                {
                    gravityCoord = idx;
                    break;
                }
            }
        }
        
        #endregion

    }
}