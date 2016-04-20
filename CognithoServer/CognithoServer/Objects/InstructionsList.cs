using CognithoServer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CognithoServer.Objects
{
    public class InstructionsList
    {
        public List<Instruction> ListInstructions { get; set; }
        public int NumOfInstructionsInTemplate { get; set; }
        public int NumOfFutilityInstructions { get; set; }

        public InstructionsList(List<Instruction> listInstructions, int numOfInstructionsInTemplate, int numOfFutilityInstructions)
        {
            ListInstructions = listInstructions;
            NumOfInstructionsInTemplate = numOfInstructionsInTemplate;
            NumOfFutilityInstructions = numOfFutilityInstructions;
        }
    }
}