using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CognithoServer.Logic.Statistics.Norms
{
    public class NormMgr
    {
        protected Dictionary<int, IInstructionNorms> mListInstructionNorms;

        public NormMgr()
        {
            mListInstructionNorms = new Dictionary<int, IInstructionNorms>();

            mListInstructionNorms.Add(0, new InstructionNorms(0));
            mListInstructionNorms.Add(1, new InstructionNorms(1));
            mListInstructionNorms.Add(2, new InstructionNorms(2));
        }

        public INormObj GetNormObject(int instructionIdx, string name)
        {
            return mListInstructionNorms[instructionIdx].GetNormObject(name);
        }


        public Dictionary<int, IInstructionNorms> InstructionNorms
        {
            get
            {
                return mListInstructionNorms;
            }
        }
    }
}