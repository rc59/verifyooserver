using CognithoServer.Logic.Statistics.Comparison.Objects;
using CognithoServer.Logic.Statistics.Norms;
using CognithoServer.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CognithoServer.Logic.Statistics.Comparison.Interfaces
{
    public abstract class CompParamContainerAbstract : ICompParamContainer
    {
        protected Dictionary<string, ICompParam> mDictCompParams;
        protected List<ICompParam> mListCompParams;

        protected int mInstructionIdx;

        protected NormMgr mNormMgr;

        protected UtilsCalc mUtilsCalc;

        public void Init()
        {
            mNormMgr = (NormMgr)new UtilsObjectMgr().GetObject(Utils.Consts.APPLICATION_OBJ_NORM_MGR);
            mListCompParams = new List<ICompParam>();
            mDictCompParams = new Dictionary<string, ICompParam>();
            mUtilsCalc = new UtilsCalc();
        }

        public Dictionary<string, ICompParam> GetParamsDict()
        {
            return mDictCompParams;
        }

        public List<ICompParam> GetParamsList()
        {
            return mListCompParams;
        }
        protected void CreateComparisonParameter(string name, double value)
        {
            ICompParam tempCompParam = new CompParam(mNormMgr.GetNormObject(mInstructionIdx, name), value);

            mDictCompParams.Add(name, tempCompParam);
            mListCompParams.Add(new CompParam(mNormMgr.GetNormObject(mInstructionIdx, name), value));
        }
    }
}