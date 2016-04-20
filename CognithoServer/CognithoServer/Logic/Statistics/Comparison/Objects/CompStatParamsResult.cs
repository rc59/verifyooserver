using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CognithoServer.Logic.Statistics.Comparison.Objects
{
    public class CompStatParamsResult
    {
        public double ZscoreStored { get; set; }
        public double ZscoreVerify { get; set; }
        public double ZscoreDiff { get; set; }
        public string Name { get; set; }
    }
}