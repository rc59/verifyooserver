using CognithoServer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CognithoServer.Utils
{
    public class UtilsData
    {
        public Dictionary<int, bool> GetInstructionIdxsHash(Template template)
        {
            int tempInstructionIdx;
            Dictionary<int, bool> dictInstructionIdxs = new Dictionary<int, bool>();
            for (int idxGesture = 0; idxGesture < template.Gestures.Count; idxGesture++)
            {
                tempInstructionIdx = template.Gestures[idxGesture].InstructionIdx;

                if (!dictInstructionIdxs.ContainsKey(tempInstructionIdx))
                {
                    dictInstructionIdxs.Add(tempInstructionIdx, true);
                }
            }

            return dictInstructionIdxs;
        }
    }
}