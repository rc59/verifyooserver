using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CognithoServer.Objects
{
    public class GestureUniqueFactor
    {
        public int InstructionIdx { get; set; }
        public double ZScore { get; set; }
        public int NumStrokes { get; set; }

        public GestureUniqueFactor(int instructionIdx, double zScore, int numStrokes)
        {
            InstructionIdx = instructionIdx;
            ZScore = zScore;
            NumStrokes = numStrokes;
        }
    }
}