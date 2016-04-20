using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CognithoServer.Utils
{
    public class UtilsObjectMgr
    {
        public void RegisterObject(string name, object obj)
        {
            HttpContext.Current.Application.Add(name, obj);
        }

        public object GetObject(string name)
        {
            return HttpContext.Current.Application[name];
        }
    }
}