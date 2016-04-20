using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CognithoServer.Logic.Statistics.Comparison.Interfaces
{
    public interface ICompParam
    {
        string GetName();
        double GetZScore();
    }
}