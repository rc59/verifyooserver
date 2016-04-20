using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CognithoServer.Logic.Statistics.Norms
{
    public class ConstsNorms
    {
        public class Stroke
        {
            public const string MAX_PRESSURE = "MAX_PRESSURE";
            public const string MIN_PRESSURE = "MIN_PRESSURE";
            public const string AVG_PRESSURE = "AVG_PRESSURE";

            public const string MAX_SURFACE = "MAX_SURFACE";
            public const string MIN_SURFACE = "MIN_SURFACE";
            public const string AVG_SURFACE = "AVG_SURFACE";

            public const string MAX_VELOCITY_X = "MAX_VELOCITY_X";
            public const string MAX_VELOCITY_Y = "MAX_VELOCITY_Y";
            public const string MAX_VELOCITY = "MAX_VELOCITY";

            public const string AVG_VELOCITY_X = "AVG_VELOCITY_X";
            public const string AVG_VELOCITY_Y = "AVG_VELOCITY_Y";
            public const string AVG_VELOCITY = "AVG_VELOCITY";

            public const string MAX_ACCELERATION_X = "MAX_ACCELERATION_X";
            public const string MAX_ACCELERATION_Y = "MAX_ACCELERATION_Y";
            public const string MAX_ACCELERATION = "MAX_ACCELERATION";

            public const string AVG_ACCELERATION_X = "AVG_ACCELERATION_X";
            public const string AVG_ACCELERATION_Y = "AVG_ACCELERATION_Y";
            public const string AVG_ACCELERATION = "AVG_ACCELERATION";

            public const string AVG_TREMOR_X = "AVG_TREMOR_X";
            public const string AVG_TREMOR_Y = "AVG_TREMOR_Y";
            public const string AVG_TREMOR_Z = "AVG_TREMOR_Z";
            
            public const string MIN_ANGLE_CHANGE = "MIN_ANGLE_CHANGE";
            public const string MAX_ANGLE_CHANGE = "MAX_ANGLE_CHANGE";
            public const string AVG_ANGLE_CHANGE = "AVG_ANGLE_CHANGE";

            /***********************************************************/
            public const string LENGTH = "LENGTH";
            public const string NUM_EVENTS = "NUM_EVENTS";
            public const string TIME_INTERVAL = "TIME_INTERVAL";

            public const string BOUNDING_BOX = "BOUNDING_BOX";
            public const string WIDTH = "WIDTH";
            public const string HEIGHT = "HEIGHT";

            public const string HOUGH_LINES = "HOUGH_LINES";
            public const string HOUGH_CIRCLES = "HOUGH_CIRCLES";
            public const string CONTOUR_TREE = "CONTOUR_TREE";
            /***********************************************************/
        }

        public class Gesture
        {
            public const string TOTAL_TIME = "TOTAL_TIME";
            public const string START_END_DISTANCE = "START_END_DISTANCE";

            public const string BOUNDING_BOX = "BOUNDING_BOX";
            public const string WIDTH = "WIDTH";
            public const string HEIGHT = "HEIGHT";

            public const string LENGTH = "LENGTH";
            public const string TIME_INTERVAL = "TIME_INTERVAL";
            public const string NUM_EVENTS = "NUM_EVENTS";

            public const string STROKES_PAUSE_AVG = "STROKES_PAUSE_AVG";
            public const string STROKES_PAUSE_MAX = "STROKES_PAUSE_MAX";

            public const string STROKES_DISTANCE_AVG = "STROKES_DISTANCE_AVG";
            public const string STROKES_DISTANCE_MAX = "STROKES_DISTANCE_MAX";

            public const string STROKES_PROPORTIONS_AVG = "STROKES_PROPORTIONS_AVG";
            public const string STROKES_PROPORTIONS_MAX = "STROKES_PROPORTIONS_MAX";
        }
    }
}