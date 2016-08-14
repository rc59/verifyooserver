using Logic.Comparison.Stats.Norms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NormCalculator.Utils
{
    class NormsMgr
    {
        double[] PopulationMean;
        double[] PopulationSD;
        double[] InternalMean;
        double[] InternalSD;

        public NormsMgr(SpatialNormContainer normContainerMean, SpatialNormContainer normContainerStd, List<string> instructions)
        {
            PopulationMean = new double[UtilsConsts.SAMPLE_SIZE];
            PopulationSD = new double[UtilsConsts.SAMPLE_SIZE];
            InternalMean = new double[UtilsConsts.SAMPLE_SIZE];
            InternalSD = new double[UtilsConsts.SAMPLE_SIZE];

            for (int idxInstruction = 0; idxInstruction < instructions.Count; idxInstruction++)
            {
                string tempInstruction = instructions[idxInstruction];

                PopulationMean = normContainerMean.GetListMeans(tempInstruction, 0);
                PopulationSD = normContainerMean.GetListStds(tempInstruction, 0);

                InternalMean = normContainerStd.GetListMeans(tempInstruction, 0);
                InternalSD = normContainerStd.GetListStds(tempInstruction, 0);
            }
        }
    }
}
