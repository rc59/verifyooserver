using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CognithoServer.Models
{
    public class Instruction
    {
        public ObjectId _id { get; set; }
        public string Text { get; set; }        
        public int InstructionIdx { get; set; }
        public int __v { get; set; }
    }
}