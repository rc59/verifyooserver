using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CognithoServer.Models
{
    public class GestureObj
    {
        public int InstructionIdx;

        public List<Stroke> Strokes;

        public bool IsInTemplate;

        public GestureObj()
        {
            Strokes = new List<Stroke>();
        }
    }
}