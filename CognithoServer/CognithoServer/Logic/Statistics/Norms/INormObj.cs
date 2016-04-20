using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CognithoServer.Logic.Statistics.Norms
{
    public interface INormObj
    {
        string GetName();
        double GetAverage();
        double GetStandardDeviation();
    }
}