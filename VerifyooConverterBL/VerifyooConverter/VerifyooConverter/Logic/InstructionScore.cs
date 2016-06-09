using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VerifyooConverter.Logic
{
    class InstructionScore
    {
        public InstructionScore(string instruction, double totalScore)
        {
            Instruction = instruction;
            TotalScore = totalScore;
            NumScores = 1;
        }

        public string Instruction;
        public double NumScores;
        public double TotalScore;
    }
}
