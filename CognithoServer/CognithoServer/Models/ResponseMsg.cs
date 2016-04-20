using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CognithoServer.Models
{
    public class ResponseMsg
    {        
        public string Message;

        public ResponseMsg(string msg)
        {
            Message = msg;
        }
    }
}