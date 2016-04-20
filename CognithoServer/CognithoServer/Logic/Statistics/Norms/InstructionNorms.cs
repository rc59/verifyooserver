using CognithoServer.Models;
using CognithoServer.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CognithoServer.Logic.Statistics.Norms
{
    public class InstructionNorms : IInstructionNorms
    {
        Dictionary<string, INormObj> mListNormObjectsStrokes;
        Dictionary<string, INormObj> mListNormObjectsGestures;

        public InstructionNorms(int instructionIdx)
        {
            mListNormObjectsStrokes = new Dictionary<string, INormObj>();
            mListNormObjectsGestures = new Dictionary<string, INormObj>();

            InstructionDistributions instructionDists = new UtilsDB().GetDistributionsByInstructionIdx(instructionIdx);
            if(instructionDists != null)
            {
                List<DistributionObj> listDists = instructionDists.StrokeDistributions;

                for (int idx = 0; idx < listDists.Count; idx++)
                {
                    AddNormObjectStrokes(listDists[idx].Name, listDists[idx].Average, listDists[idx].StandardDeviation);
                }

                listDists = instructionDists.GestureDistributions;

                for (int idx = 0; idx < listDists.Count; idx++)
                {
                    AddNormObjectGestures(listDists[idx].Name, listDists[idx].Average, listDists[idx].StandardDeviation);
                }
            }
        }

        protected void AddNormObjectStrokes(string name, double average, double standardDeviation)
        {
            mListNormObjectsStrokes.Add(name, new NormObj(name, average, standardDeviation));
        }

        protected void AddNormObjectGestures(string name, double average, double standardDeviation)
        {
            mListNormObjectsGestures.Add(name, new NormObj(name, average, standardDeviation));
        }

        public INormObj GetNormObject(string name)
        {
            if(mListNormObjectsStrokes.ContainsKey(name))
            {
                return mListNormObjectsStrokes[name];
            }
            else
            {
                if (mListNormObjectsGestures.ContainsKey(name))
                {
                    return mListNormObjectsGestures[name];
                }
            }

            return null;
        }
    }
}