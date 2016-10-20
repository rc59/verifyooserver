using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VerifyooSimulator.Logic
{
    public class PerformanceMgr
    {
        protected int mResolution;

        protected double[] mThreasholds;

        protected double[] mFARCount;
        protected double[] mFRRCount;

        protected double[] mFAR;
        protected double[] mFRR;

        protected double mFARTotal;
        protected double mFRRTotal;

        public PerformanceMgr(int resolution)
        {
            mResolution = resolution;

            double threasholdInc = 1 / (double)resolution;
            double currentThreashold = 0;

            mThreasholds = new double[resolution];
            mFARCount = new double[resolution];
            mFRRCount = new double[resolution];
            mFAR = new double[resolution];
            mFRR = new double[resolution];

            mFARTotal = 0;
            mFRRTotal = 0;

            for (int idx = 0; idx < mResolution; idx++)
            {
                mThreasholds[idx] = currentThreashold;
                currentThreashold += threasholdInc;
                mFAR[idx] = 0;
                mFRR[idx] = 0;
                mFARCount[idx] = 0;
                mFRRCount[idx] = 0;
            }            
        }

        public void AddFARValue(double score)
        {
            mFARTotal++;
            double currentThreashold;

            for (int idx = 0; idx < mResolution; idx++)
            {
                currentThreashold = mThreasholds[idx];
                if(score >= currentThreashold)
                {
                    mFARCount[idx]++;
                    mFAR[idx] = mFARCount[idx] / mFARTotal;
                }
            }
        }

        public void AddFRRValue(double score)
        {
            mFRRTotal++;
            double currentThreashold;

            for (int idx = 0; idx < mResolution; idx++)
            {
                currentThreashold = mThreasholds[idx];
                if (score < currentThreashold)
                {
                    mFRRCount[idx]++;
                    mFRR[idx] = mFRRCount[idx] / mFRRTotal;
                }
            }
        }

        public void WriteFRR(StreamWriter streamWriter)
        {
            StringBuilder stringBuilder;

            for(int idx = 0; idx < mResolution; idx++)
            {
                stringBuilder = new StringBuilder();
                stringBuilder.Append(mThreasholds[idx].ToString());
                stringBuilder.Append(",");

                stringBuilder.Append(mFRR[idx].ToString());
                stringBuilder.Append(",");

                streamWriter.WriteLine(stringBuilder.ToString());
            }
        }

        public void WriteFAR(StreamWriter streamWriter)
        {
            StringBuilder stringBuilder;

            for (int idx = 0; idx < mResolution; idx++)
            {
                stringBuilder = new StringBuilder();
                stringBuilder.Append(mThreasholds[idx].ToString());
                stringBuilder.Append(",");

                stringBuilder.Append(mFAR[idx].ToString());
                stringBuilder.Append(",");

                streamWriter.WriteLine(stringBuilder.ToString());
            }
        }

        public double GetThreasholdForFAR(double frr)
        {
            double tempFrr;

            double tempFrrDiff;
            double minFrrDiff = Double.MaxValue;

            int idxMinFrrDiff = 0;

            for (int idx = 0; idx < mResolution; idx++)
            {
                tempFrr = mFRR[idx];
                tempFrrDiff = Math.Abs(frr - tempFrr);
                
                if(tempFrrDiff < minFrrDiff)
                {
                    minFrrDiff = tempFrrDiff;
                    idxMinFrrDiff = idx;
                }
            }

            return mThreasholds[idxMinFrrDiff];
        }

    }
}
