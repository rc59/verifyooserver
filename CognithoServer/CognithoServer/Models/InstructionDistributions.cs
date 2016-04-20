using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CognithoServer.Models
{
    public class InstructionDistributions
    {
        public ObjectId _id { get; set; }
        public int InstructionIdx { get; set; }
        public List<DistributionObj> StrokeDistributions { get; set; }
        public List<DistributionObj> GestureDistributions { get; set; }
        public int __v { get; set; }

        public InstructionDistributions(int instructionIdx)
        {
            InstructionIdx = instructionIdx;
            StrokeDistributions = new List<DistributionObj>();
            GestureDistributions = new List<DistributionObj>();
        }
    }
}