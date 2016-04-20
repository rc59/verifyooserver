using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CognithoServer.Logic.Statistics.Norms
{
    public class NormObj : INormObj
    {
        public string Name;
        public double Average { get; set; }
        public double StandardDeviation { get; set; }

        public NormObj(string name, double average, double standardDeviation)
        {
            Name = name;
            Average = average;
            StandardDeviation = standardDeviation;
        }

        public string GetName()
        {
            return Name;
        }

        public double GetAverage()
        {
            return Average;
        }

        public double GetStandardDeviation()
        {
            return StandardDeviation;
        }
    }
}