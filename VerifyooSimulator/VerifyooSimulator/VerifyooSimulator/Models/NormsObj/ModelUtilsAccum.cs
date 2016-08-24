using Logic.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VerifyooSimulator.Models.NormsObj
{
    public class ModelUtilsAccum
    {
        public double Mu;
        public int N;
        public double Sum;

        public ModelUtilsAccum()
        {

        }

        public ModelUtilsAccum(UtilsAccumulator utilsAccum)
        {
            Mu = utilsAccum.Mu;
            N = utilsAccum.N;
            Sum = utilsAccum.Sum;
        }
    }
}
