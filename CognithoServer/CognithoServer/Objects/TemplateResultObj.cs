using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CognithoServer.Objects
{
    public class TemplateResultObj
    {
        public bool TemplatesMatch;
        public List<GestureResultObj> GestureResults = new List<GestureResultObj>();
    }
}