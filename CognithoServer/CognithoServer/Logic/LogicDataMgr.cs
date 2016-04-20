using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CognithoServer.Logic
{
    public class GlobalValues
    {
        public ThreasholdObj Pressure;
        public ThreasholdObj TouchSurface;
        public ThreasholdObj Velocity;
        public ThreasholdObj Acceleration;

    }

    public class InstructionSpecificValues
    {
        public int InstructionIdx { get; set; }

        public ThreasholdObj NumStrokes { get; set; }
    }

    public class ThreasholdObj
    {
        double Min { get; set; }
        double Low { get; set; }
        double High { get; set; }
        double Max { get; set; }

        public ThreasholdObj(double min, double low, double high, double max)
        {
            Min = min;
            Low = low;
            High = high;
            Max = max;
        }
    }

    public class LogicDataMgr
    {
        private GlobalValues mGlobalValues;
        private List<InstructionSpecificValues> mListInstructionSpecificValues;

        public LogicDataMgr()
        {
            mListInstructionSpecificValues = new List<InstructionSpecificValues>();

            InstructionSpecificValues tempInstructionSpecificValues = new InstructionSpecificValues();

            tempInstructionSpecificValues.InstructionIdx = 0;
            tempInstructionSpecificValues.NumStrokes = new ThreasholdObj(1,2,4,8);
            mListInstructionSpecificValues.Add(tempInstructionSpecificValues);

            tempInstructionSpecificValues.InstructionIdx = 1;
            tempInstructionSpecificValues.NumStrokes = new ThreasholdObj(2, 4, 6, 10);
            mListInstructionSpecificValues.Add(tempInstructionSpecificValues);

            mGlobalValues = new GlobalValues();

            mGlobalValues.Pressure = new ThreasholdObj(0.25, 0.3, 0.6, 0.7);
            mGlobalValues.TouchSurface = new ThreasholdObj(0.25, 0.3, 0.6, 0.7);
            mGlobalValues.Velocity = new ThreasholdObj(1000, 2000, 5000, 6000);
            mGlobalValues.Acceleration = new ThreasholdObj(50, 100, 300, 400);
        }
    }
}