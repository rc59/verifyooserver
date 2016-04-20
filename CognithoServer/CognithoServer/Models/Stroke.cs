using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CognithoServer.Models
{
    public class Stroke
    {
        public List<MotionEventCompact> ListEvents;
        public double Length;

        public Stroke()
        {
            ListEvents = new List<MotionEventCompact>();
        }
    }
}