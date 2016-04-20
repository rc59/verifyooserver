using CognithoServer.Logic.Statistics.Comparison.Interfaces;
using CognithoServer.Logic.Statistics.Norms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CognithoServer.Logic.Statistics.Comparison.Objects
{
    public class CompParam : ICompParam
    {
        public double ZScore { get; set; }
        public string Name { get; set; }

        public CompParam(INormObj normObj, double value)
        {
            Name = normObj.GetName();
            ZScore = (value - normObj.GetAverage()) / normObj.GetStandardDeviation();
        }       

        public string GetName()
        {
            return Name;
        }

        public double GetZScore()
        {
            return ZScore;
        }
    }
}