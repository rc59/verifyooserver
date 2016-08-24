using Logic.Comparison.Stats.Norms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VerifyooSimulator.Models.NormsObj
{
    class ModelSpatialNormContainer
    {
        public Dictionary<String, ModelAccumulatorsContainer> HashNorms;

        public ModelSpatialNormContainer()
        {
            HashNorms = new Dictionary<string, ModelAccumulatorsContainer>();
        }
    }
}
