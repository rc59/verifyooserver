using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CognithoServer.Objects
{
    public class CVShapeFeatures
    {
        public double CirclesDiffAbsolute;
        public double CirclesDiffPercentage;

        public double LinesDiffAbsolute;
        public double LinesDiffPercentage;

        public double Score1;
        public double Score2;
        public double Score3;

        public double NormalizedScore1;
        public double NormalizedScore2;
        public double NormalizedScore3;

        public double ContourTreeDiffAbsolute;
        public double ContourTreeDiffPercentage;
    }

    public class StrokeResultObj
    {       
        public CVShapeFeatures CVShapeFeatures;

        public double ShapeScore;
        public double ZScore;

        public double AngleMatches;
        public double AngleMatchesDiff;
        public double AngleChangesMatches;
        public double VeclocityDirectionMatches;

        public double LengthDiffPercentage;
        public double TimeIntervalDiffPercentage;
        public double NumEventsDiffPercentage;

        public double HeightDiffPercentage;
        public double WidthDiffPercentage;
        public double BoundingBoxDiffPercentage;

        public double HeightDiffAbsolute;
        public double WidthDiffAbsolute;
        public double BoundingBoxDiffAbsolute;

        public double MaxPressureDiff;
        public double MinPressureDiff;
        public double AvgPressureDiff;

        public double MaxSurfaceDiff;
        public double MinSurfaceDiff;
        public double AvgSurfaceDiff;

        public double RelativeDistanceXDiffPercentage = -1;
        public double RelativeDistanceYDiffPercentage = -1;
        public double RelativeDistanceDiffPercentage = -1;
        public double RelativeDistanceXDiffAbsolute = -1;
        public double RelativeDistanceYDiffAbsolute = -1;
        public double RelativeDistanceDiffAbsolute = -1;

        public double PauseBetweenStrokesDiffPercentage = -1;
        public double PauseBetweenStrokesDiffAbsolute = -1;        

        public double MaxVelocityXDiffAbsolute;
        public double MaxVelocityYDiffAbsolute;
        public double MaxVelocityDiffAbsolute;

        public double MinVelocityXDiffAbsolute;
        public double MinVelocityYDiffAbsolute;

        public double MaxVelocityXDiffPercentage;
        public double MaxVelocityYDiffPercentage;
        public double MaxVelocityDiffPercentage;

        public double MinVelocityXDiffPercentage;
        public double MinVelocityYDiffPercentage;

        public double AvgVelocityXDiffAbsolute;
        public double AvgVelocityYDiffAbsolute;
        public double AvgVelocityDiffAbsolute;

        public double AvgVelocityXDiffPercentage;
        public double AvgVelocityYDiffPercentage;
        public double AvgVelocityDiffPercentage;

        public double MaxAccelerationXDiffAbsolute;
        public double MaxAccelerationYDiffAbsolute;
        public double MaxAccelerationDiffAbsolute;

        public double MinAccelerationXDiffAbsolute;
        public double MinAccelerationYDiffAbsolute;

        public double MaxAccelerationXDiffPercentage;
        public double MaxAccelerationYDiffPercentage;
        public double MaxAccelerationDiffPercentage;

        public double MinAccelerationXDiffPercentage;
        public double MinAccelerationYDiffPercentage;

        public double AvgAccelerationXDiffPercentage;
        public double AvgAccelerationYDiffPercentage;
        public double AvgAccelerationDiffPercentage;

        public double AvgAccelerationXDiffAbsolute;
        public double AvgAccelerationYDiffAbsolute;
        public double AvgAccelerationDiffAbsolute;

        public bool PointerCountMatches;
    }
}