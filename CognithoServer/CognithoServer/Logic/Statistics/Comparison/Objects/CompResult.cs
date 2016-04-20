using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CognithoServer.Logic.Statistics.Comparison.Objects
{
    public class CompResult
    {
        public double ShapeScore { get; set; }
        public double ZScore { get; set; }

        public CompResult(double shapeScore, double zScore)
        {
            ShapeScore = shapeScore;
            ZScore = zScore;
        }
    }
}