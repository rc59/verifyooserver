using CognithoServer.Logic.Statistics.Comparison.Comparers;
using CognithoServer.Logic.Statistics.Comparison.Interfaces;
using CognithoServer.Logic.Statistics.Comparison.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace CognithoServer.Logic.Statistics.Comparison
{
    public class CompParamMgr
    {
        protected const double Z_SCORE_UNIQUE_THREASHOLD_HIGH = 2;
        protected const double Z_SCORE_UNIQUE_THREASHOLD_MEDIUM = 1.5;
        protected const double Z_SCORE_UNIQUE_THREASHOLD_LOW = 1;
        protected const double Z_SCORE_DIFF_MAX = 4;

        protected ICompParamContainer mCompParamsCtrStored;
        protected ICompParamContainer mCompParamsCtrVerify;

        protected double zDiffScore;

        public string StrParameters;

        public CompParamMgr(ICompParamContainer compParamsCtrStored, ICompParamContainer compParamsCtrVerify)
        {
            StrParameters = string.Empty;
            mCompParamsCtrStored = compParamsCtrStored;
            mCompParamsCtrVerify = compParamsCtrVerify;
        }

        public CompParamMgr(ICompParamContainer compParamsCtr)
        {
            StrParameters = string.Empty;
            mCompParamsCtrStored = compParamsCtr;
            mCompParamsCtrVerify = null;
        }

        public void RunComparison()
        {
            List<ICompParam> paramsList = mCompParamsCtrStored.GetParamsList();

            CompStatParamsResult tempParamResult;
            List<CompStatParamsResult> listResults = new List<CompStatParamsResult>();

            zDiffScore = 0;
            double tempZValue;
            double tempStoredZValue;

            int multiplier;
            int count = 0;

            int numLow = 0;
            int numMed = 0;
            int numHigh = 0;

            StringBuilder sb = new StringBuilder();

            for (int idxParam = 0; idxParam < paramsList.Count; idxParam++)
            {
                tempParamResult = new CompStatParamsResult();

                tempParamResult.Name = paramsList[idxParam].GetName();
                tempParamResult.ZscoreStored = Math.Round(paramsList[idxParam].GetZScore(), 2);

                if (mCompParamsCtrVerify != null)
                {
                    tempParamResult.ZscoreVerify = Math.Round(mCompParamsCtrVerify.GetParamsDict()[tempParamResult.Name].GetZScore(), 2);
                }
                else
                {
                    tempParamResult.ZscoreVerify = 0;
                }
                
                tempParamResult.ZscoreDiff = Math.Round(tempParamResult.ZscoreStored - tempParamResult.ZscoreVerify, 2);

                string temp = string.Format("[Name:{0}, Z1:{1}, Z2:{2}, ΔZ:{3}], ", tempParamResult.Name, tempParamResult.ZscoreStored.ToString(), tempParamResult.ZscoreVerify.ToString(), tempParamResult.ZscoreDiff.ToString());
                sb.Append(temp);

                tempZValue = Math.Abs(tempParamResult.ZscoreDiff);
                multiplier = 1;

                tempStoredZValue = Math.Abs(tempParamResult.ZscoreStored);

                if (tempZValue > Z_SCORE_DIFF_MAX)
                {
                    tempZValue = Z_SCORE_DIFF_MAX;
                }

                if (tempStoredZValue > Z_SCORE_UNIQUE_THREASHOLD_LOW && tempStoredZValue <= Z_SCORE_UNIQUE_THREASHOLD_MEDIUM)
                {
                    numLow++;
                    multiplier = 2;
                }

                if (tempStoredZValue > Z_SCORE_UNIQUE_THREASHOLD_MEDIUM && tempStoredZValue <= Z_SCORE_UNIQUE_THREASHOLD_HIGH)
                {
                    numMed++;
                    multiplier = 3;
                }

                if (tempStoredZValue > Z_SCORE_UNIQUE_THREASHOLD_HIGH)
                {
                    numHigh++;
                    multiplier = 4;
                }

                count += multiplier;
                zDiffScore += (tempZValue * multiplier);

                listResults.Add(tempParamResult);
            }

            StrParameters = sb.ToString();

            //zDiffScore = Math.Sqrt(zDiffScore);
            zDiffScore = zDiffScore / count;

        }

        public double GetAvgZDiffScore()
        {
            return zDiffScore;
        }
    }
}