using CognithoServer.Logic.Statistics.Comparison.Interfaces;
using CognithoServer.Logic.Statistics.Comparison.Objects;
using CognithoServer.Logic.Statistics.Norms;
using CognithoServer.Models;
using CognithoServer.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace CognithoServer.Logic.Statistics.Comparison.Comparers
{
    public class GestureCompParamContainer : CompParamContainerAbstract, ICompParamContainer
    {
        List<Stroke> mStrokes;
        List<StrokeCompParamContainer> mListCompParamContainer;

        public double MinX = double.MaxValue;
        public double MinY = double.MaxValue;
        public double MaxX = 0;
        public double MaxY = 0;

        public double Width = 0;
        public double Height = 0;
        public double BoundingBox = 0;

        public double StartEndDistance = 0;
        public double TotalTime = 0;

        public double StrokePauseAverage = 0;
        public double StrokePauseMax = 0;

        public double StrokeDistanceAverage = 0;
        public double StrokeDistanceMax = 0;

        public double StrokeProportionsAverage = 0;
        public double StrokeProportionsMax = 0;

        public double Length = 0;
        public double NumEvents = 0;
        public double TimeInterval = 0;

        public GestureCompParamContainer(List<StrokeCompParamContainer> listCompParamContainer, List<Stroke> strokes)
        {
            mListCompParamContainer = listCompParamContainer;
            mStrokes = strokes; 

            Init();

            ExtractParams();
        }

        protected void ExtractParams()
        {
            int numStrokes = mStrokes.Count;

            MotionEventCompact eventDown = mStrokes[0].ListEvents[0];
            MotionEventCompact eventUp = mStrokes[numStrokes - 1].ListEvents[mStrokes[numStrokes - 1].ListEvents.Count - 1];

            TotalTime = eventUp.EventTime - eventDown.EventTime;
            StartEndDistance = GetDistanceBetweenPoints(eventDown, eventUp);

            MotionEventCompact tempEventDown, tempEventUp;
            double tempStrokePause;
            double tempStrokeDistance;
            double tempBoundingBoxDown, tempBoundingBoxUp;

            for (int idx = 0; idx < mStrokes.Count - 1; idx++)
            {
                tempEventUp = mStrokes[idx].ListEvents[mStrokes[idx].ListEvents.Count - 1];
                tempEventDown= mStrokes[idx + 1].ListEvents[0];

                tempStrokePause = tempEventDown.EventTime - tempEventUp.EventTime;
                StrokePauseAverage += tempStrokePause;
                StrokePauseMax = mUtilsCalc.GetMaxValue(StrokePauseMax, tempStrokePause);

                tempStrokeDistance = GetDistanceBetweenPoints(tempEventUp, tempEventDown);
                StrokeDistanceAverage += tempStrokeDistance;
                StrokeDistanceMax = mUtilsCalc.GetMaxValue(StrokeDistanceMax, tempStrokeDistance);                
            }

            StrokePauseAverage = StrokePauseAverage / (mStrokes.Count - 1);
            StrokeDistanceAverage = StrokeDistanceAverage / (mStrokes.Count - 1);

            double tempStrokeProportions;

            for (int idx = 0; idx < mListCompParamContainer.Count; idx++)
            {
                MinX = mUtilsCalc.GetMinValue(mListCompParamContainer[idx].MinX, MinX);
                MinY = mUtilsCalc.GetMinValue(mListCompParamContainer[idx].MinY, MinY);

                MaxX = mUtilsCalc.GetMaxValue(mListCompParamContainer[idx].MaxX, MaxX);
                MaxY = mUtilsCalc.GetMaxValue(mListCompParamContainer[idx].MaxY, MaxY);

                if (idx + 1 < mListCompParamContainer.Count)
                {
                    tempBoundingBoxUp = mListCompParamContainer[idx].BoundingBox;
                    tempBoundingBoxDown = mListCompParamContainer[idx + 1].BoundingBox;

                    tempStrokeProportions = 1 / mUtilsCalc.CalcPercentage(tempBoundingBoxDown, tempBoundingBoxUp);
                    StrokeProportionsAverage += tempStrokeProportions;
                    StrokeProportionsMax = mUtilsCalc.GetMaxValue(StrokeProportionsMax, tempStrokeProportions);
                }

                Length += mListCompParamContainer[idx].Length;
                TimeInterval += mListCompParamContainer[idx].TimeInterval;
                NumEvents += mListCompParamContainer[idx].NumEvents;
            }

            StrokeProportionsAverage = StrokeProportionsAverage / (mListCompParamContainer.Count - 1);

            Width = MaxX - MinX;
            Height = MaxY - MinY;
            BoundingBox = Width * Height;

            CreateComparisonParameter(ConstsNorms.Gesture.STROKES_PROPORTIONS_AVG, StrokePauseAverage);
            CreateComparisonParameter(ConstsNorms.Gesture.STROKES_PROPORTIONS_MAX, StrokePauseMax);

            CreateComparisonParameter(ConstsNorms.Gesture.STROKES_PAUSE_AVG, StrokePauseAverage);
            CreateComparisonParameter(ConstsNorms.Gesture.STROKES_PAUSE_MAX, StrokePauseMax);

            CreateComparisonParameter(ConstsNorms.Gesture.STROKES_DISTANCE_AVG, StrokeDistanceAverage);
            CreateComparisonParameter(ConstsNorms.Gesture.STROKES_DISTANCE_MAX, StrokeDistanceMax);

            CreateComparisonParameter(ConstsNorms.Gesture.TOTAL_TIME, TotalTime);
            CreateComparisonParameter(ConstsNorms.Gesture.START_END_DISTANCE, StartEndDistance);

            CreateComparisonParameter(ConstsNorms.Gesture.BOUNDING_BOX, BoundingBox);
            CreateComparisonParameter(ConstsNorms.Gesture.WIDTH, Width);
            CreateComparisonParameter(ConstsNorms.Gesture.HEIGHT, Height);

            CreateComparisonParameter(ConstsNorms.Gesture.LENGTH, Length);
            CreateComparisonParameter(ConstsNorms.Gesture.TIME_INTERVAL, TimeInterval);
            CreateComparisonParameter(ConstsNorms.Gesture.NUM_EVENTS, NumEvents);
        }

        protected double GetDistanceBetweenPoints(MotionEventCompact event1, MotionEventCompact event2)
        {
            double deltaX = event1.X - event2.X;
            double deltaY = event1.Y - event2.Y;

            double distance = mUtilsCalc.CalcPitagoras(deltaX, deltaY);
            return distance;
        }

        protected double CalcAvg(double value)
        {
            return value / mListCompParamContainer.Count;
        }
    }
}