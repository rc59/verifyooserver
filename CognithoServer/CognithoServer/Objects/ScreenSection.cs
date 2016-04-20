using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CognithoServer.Objects
{
    public class ScreenSection
    {
        public ScreenSection(double min, double max)
        {
            Min = Math.Floor(min);
            Max = Math.Ceiling(max);
        }

        public double Min { get; set; }
        public double Max { get; set; }
    }
}