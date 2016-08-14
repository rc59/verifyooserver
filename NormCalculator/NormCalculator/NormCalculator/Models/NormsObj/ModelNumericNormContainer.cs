using Logic.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NormCalculator.Models.NormsObj
{
    class ModelNumericNormContainer
    {
        public Dictionary<String, ModelUtilsAccum> HashNorms;

        public ModelNumericNormContainer()
        {
            HashNorms = new Dictionary<string, ModelUtilsAccum>();
        }
    }
}
