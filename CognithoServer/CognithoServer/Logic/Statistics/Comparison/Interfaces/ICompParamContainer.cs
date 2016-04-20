using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CognithoServer.Logic.Statistics.Comparison.Interfaces
{
    public interface ICompParamContainer
    {
        List<ICompParam> GetParamsList();
        Dictionary<string, ICompParam> GetParamsDict();        
    }
}