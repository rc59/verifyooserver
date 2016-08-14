using java.util;
using Logic.Comparison.Stats.Norms;
using Logic.Utils;
using NormCalculator.Models.NormsObj;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NormCalculator.Models
{
    class ModelNormContainerMgr
    {
        public Dictionary<String, ModelSpatialNormContainer> HashMapSpatialNormsMeans = new Dictionary<string, ModelSpatialNormContainer>();
        public Dictionary<String, ModelSpatialNormContainer> HashMapSpatialNormsSds = new Dictionary<string, ModelSpatialNormContainer>();

        public Dictionary<String, ModelNumericNormContainer> HashMapNumericNormsMeans = new Dictionary<string, ModelNumericNormContainer>();
        public Dictionary<String, ModelNumericNormContainer> HashMapNumericNormsSds = new Dictionary<string, ModelNumericNormContainer>();

        public ModelNormContainerMgr()
        {
            HashMapSpatialNormsMeans = new Dictionary<string, ModelSpatialNormContainer>();
            HashMapSpatialNormsSds = new Dictionary<string, ModelSpatialNormContainer>();

            HashMapNumericNormsMeans = new Dictionary<string, ModelNumericNormContainer>();
            HashMapNumericNormsSds = new Dictionary<string, ModelNumericNormContainer>();
        }

        public ModelNormContainerMgr(NormContainerMgr normContainerMgr)
        {
            SpatialNormContainer tempSpatialNormContainerMeans;
            SpatialNormContainer tempSpatialNormContainerSds;

            NumericNormContainer tempNumericNormContainerMeans;
            NumericNormContainer tempNumericNormContainerSds;

            object[] keySetSpatialNorms = normContainerMgr.HashMapSpatialNormsMeans.keySet().toArray();
            object[] keySetNumericNorms = normContainerMgr.HashMapNumericNormsMeans.keySet().toArray();

            object[] keySetTempListUtilsAccumulators;
            object[] keySetTempUtilsAccumulators;
                        
            UtilsAccumulator tempUtilsAccumulator;
            AccumulatorsContainer tempAccumulatorContainer;

            ModelNumericNormContainer tempModelNumericNormContainerMeans;
            ModelNumericNormContainer tempModelNumericNormContainerSds;

            ModelSpatialNormContainer tempModelSpatialNormContainerMeans;
            ModelSpatialNormContainer tempModelSpatialNormContainerSds;

            ModelAccumulatorsContainer tempModelAccumulatorContainer;

            string tempKey, tempKeyInternal;

            for (int idx = 0; idx < keySetSpatialNorms.Length; idx++) {
                tempKey = (string)keySetSpatialNorms[idx];
                tempSpatialNormContainerMeans = (SpatialNormContainer) normContainerMgr.HashMapSpatialNormsMeans.get(tempKey);
                tempSpatialNormContainerSds = (SpatialNormContainer) normContainerMgr.HashMapSpatialNormsSds.get(tempKey);

                keySetTempListUtilsAccumulators = tempSpatialNormContainerMeans.HashNorms.keySet().toArray();
                tempModelSpatialNormContainerMeans = new ModelSpatialNormContainer();                

                for (int idxAccumulatorContainer = 0; idxAccumulatorContainer < keySetTempListUtilsAccumulators.Length; idxAccumulatorContainer++) {
                    tempKeyInternal = (string)keySetTempListUtilsAccumulators[idxAccumulatorContainer];
                    tempAccumulatorContainer = (AccumulatorsContainer)tempSpatialNormContainerMeans.HashNorms.get(tempKeyInternal);

                    tempModelAccumulatorContainer = new ModelAccumulatorsContainer();
                    tempModelAccumulatorContainer.ListUtilsAccumulator = new List<ModelUtilsAccum>();
                    for (int idxUtilAccum = 0; idxUtilAccum < tempAccumulatorContainer.ListUtilsAccumulator.size(); idxUtilAccum++)
                    {
                        tempModelAccumulatorContainer.ListUtilsAccumulator.Add(new ModelUtilsAccum((UtilsAccumulator)tempAccumulatorContainer.ListUtilsAccumulator.get(idxUtilAccum)));
                    }

                    tempModelSpatialNormContainerMeans.HashNorms.Add(tempKeyInternal, tempModelAccumulatorContainer);
                }

                keySetTempListUtilsAccumulators = tempSpatialNormContainerSds.HashNorms.keySet().toArray();
                tempModelSpatialNormContainerSds = new ModelSpatialNormContainer();

                for (int idxAccumulatorContainer = 0; idxAccumulatorContainer < keySetTempListUtilsAccumulators.Length; idxAccumulatorContainer++)
                {
                    tempKeyInternal = (string)keySetTempListUtilsAccumulators[idxAccumulatorContainer];
                    tempAccumulatorContainer = (AccumulatorsContainer)tempSpatialNormContainerSds.HashNorms.get(tempKeyInternal);

                    tempModelAccumulatorContainer = new ModelAccumulatorsContainer();
                    tempModelAccumulatorContainer.ListUtilsAccumulator = new List<ModelUtilsAccum>();
                    for (int idxUtilAccum = 0; idxUtilAccum < tempAccumulatorContainer.ListUtilsAccumulator.size(); idxUtilAccum++)
                    {
                        tempModelAccumulatorContainer.ListUtilsAccumulator.Add(new ModelUtilsAccum((UtilsAccumulator)tempAccumulatorContainer.ListUtilsAccumulator.get(idxUtilAccum)));
                    }

                    tempModelSpatialNormContainerSds.HashNorms.Add(tempKeyInternal, tempModelAccumulatorContainer);
                }

                HashMapSpatialNormsMeans.Add(tempKey, tempModelSpatialNormContainerMeans);
                HashMapSpatialNormsSds.Add(tempKey, tempModelSpatialNormContainerSds);
            }

            for (int idx = 0; idx < keySetNumericNorms.Length; idx++)
            {
                tempKey = (string)keySetNumericNorms[idx];
                tempNumericNormContainerMeans = (NumericNormContainer)normContainerMgr.HashMapNumericNormsMeans.get(tempKey);
                tempNumericNormContainerSds = (NumericNormContainer)normContainerMgr.HashMapNumericNormsSds.get(tempKey);

                keySetTempUtilsAccumulators = tempNumericNormContainerMeans.HashNorms.keySet().toArray();
                tempModelNumericNormContainerMeans = new ModelNumericNormContainer();                

                for (int idxUtilsAccumulators = 0; idxUtilsAccumulators < keySetTempUtilsAccumulators.Length; idxUtilsAccumulators++) {
                    tempKeyInternal = (string)keySetTempUtilsAccumulators[idxUtilsAccumulators];
                    tempUtilsAccumulator = (UtilsAccumulator)tempNumericNormContainerMeans.HashNorms.get(tempKeyInternal);

                    tempModelNumericNormContainerMeans.HashNorms.Add(tempKeyInternal, new ModelUtilsAccum(tempUtilsAccumulator));
                }
                
                keySetTempUtilsAccumulators = tempNumericNormContainerSds.HashNorms.keySet().toArray();
                tempModelNumericNormContainerSds = new ModelNumericNormContainer();

                for (int idxUtilsAccumulators = 0; idxUtilsAccumulators < keySetTempUtilsAccumulators.Length; idxUtilsAccumulators++)
                {
                    tempKeyInternal = (string)keySetTempUtilsAccumulators[idxUtilsAccumulators];
                    tempUtilsAccumulator = (UtilsAccumulator)tempNumericNormContainerSds.HashNorms.get(tempKeyInternal);

                    tempModelNumericNormContainerSds.HashNorms.Add(tempKeyInternal, new ModelUtilsAccum(tempUtilsAccumulator));
                }

                HashMapNumericNormsMeans.Add(tempKey, tempModelNumericNormContainerMeans);
                HashMapNumericNormsSds.Add(tempKey, tempModelNumericNormContainerSds);
            }
        }

        public NormContainerMgr ToNormContainerMgr()
        {
            NormContainerMgr resultNormContainerMgr = new NormContainerMgr();

            ModelSpatialNormContainer tempModelSpatialNormContainerMeans;
            ModelSpatialNormContainer tempModelSpatialNormContainerSds;

            ModelNumericNormContainer tempModelNumericNormContainerMeans;
            ModelNumericNormContainer tempModelNumericNormContainerSds;

            ModelAccumulatorsContainer tempModelAccumulatorsContainer;
            UtilsAccumulator tempUtilsAccumulator;
            ModelUtilsAccum tempModelUtilsAccumulator;

            SpatialNormContainer tempSpatialNormContainer;
            NumericNormContainer tempNumericNormContainer;

            AccumulatorsContainer tempAccumulatorsContainer;

            foreach (string key in HashMapSpatialNormsMeans.Keys)
            {
                tempModelSpatialNormContainerMeans = HashMapSpatialNormsMeans[key];
                
                tempSpatialNormContainer = new SpatialNormContainer();
                tempSpatialNormContainer.HashNorms = new HashMap();
                foreach (string keyInternal in tempModelSpatialNormContainerMeans.HashNorms.Keys)
                {
                    tempModelAccumulatorsContainer = tempModelSpatialNormContainerMeans.HashNorms[keyInternal];

                    tempAccumulatorsContainer = new AccumulatorsContainer();
                    tempAccumulatorsContainer.ListUtilsAccumulator = new ArrayList();
                    for (int idxUtilAccum = 0; idxUtilAccum < tempModelAccumulatorsContainer.ListUtilsAccumulator.Count; idxUtilAccum++)
                    {
                        tempModelUtilsAccumulator = tempModelAccumulatorsContainer.ListUtilsAccumulator[idxUtilAccum];
                        tempUtilsAccumulator = new UtilsAccumulator();
                        tempUtilsAccumulator.Mu = tempModelUtilsAccumulator.Mu;
                        tempUtilsAccumulator.N = tempModelUtilsAccumulator.N;
                        tempUtilsAccumulator.Sum = tempModelUtilsAccumulator.Sum;

                        tempAccumulatorsContainer.ListUtilsAccumulator.add(tempUtilsAccumulator);
                    }

                    tempSpatialNormContainer.HashNorms.put(keyInternal, tempAccumulatorsContainer);
                }

                resultNormContainerMgr.HashMapSpatialNormsMeans.put(key, tempSpatialNormContainer);
            }

            foreach (string key in HashMapSpatialNormsSds.Keys)
            {
                tempModelSpatialNormContainerSds = HashMapSpatialNormsSds[key];

                tempSpatialNormContainer = new SpatialNormContainer();
                tempSpatialNormContainer.HashNorms = new HashMap();
                foreach (string keyInternal in tempModelSpatialNormContainerSds.HashNorms.Keys)
                {
                    tempModelAccumulatorsContainer = tempModelSpatialNormContainerSds.HashNorms[keyInternal];

                    tempAccumulatorsContainer = new AccumulatorsContainer();
                    tempAccumulatorsContainer.ListUtilsAccumulator = new ArrayList();
                    for (int idxUtilAccum = 0; idxUtilAccum < tempModelAccumulatorsContainer.ListUtilsAccumulator.Count; idxUtilAccum++)
                    {
                        tempModelUtilsAccumulator = tempModelAccumulatorsContainer.ListUtilsAccumulator[idxUtilAccum];
                        tempUtilsAccumulator = new UtilsAccumulator();
                        tempUtilsAccumulator.Mu = tempModelUtilsAccumulator.Mu;
                        tempUtilsAccumulator.N = tempModelUtilsAccumulator.N;
                        tempUtilsAccumulator.Sum = tempModelUtilsAccumulator.Sum;

                        tempAccumulatorsContainer.ListUtilsAccumulator.add(tempUtilsAccumulator);
                    }

                    tempSpatialNormContainer.HashNorms.put(keyInternal, tempAccumulatorsContainer);
                }

                resultNormContainerMgr.HashMapSpatialNormsSds.put(key, tempSpatialNormContainer);
            }

            foreach (string key in HashMapNumericNormsMeans.Keys)
            {
                tempModelNumericNormContainerMeans = HashMapNumericNormsMeans[key];

                tempNumericNormContainer = new NumericNormContainer();
                tempNumericNormContainer.HashNorms = new HashMap();
                foreach (string keyInternal in tempModelNumericNormContainerMeans.HashNorms.Keys)
                {
                    tempModelUtilsAccumulator = tempModelNumericNormContainerMeans.HashNorms[keyInternal];
                    tempUtilsAccumulator = new UtilsAccumulator();
                    tempUtilsAccumulator.Mu = tempModelUtilsAccumulator.Mu;
                    tempUtilsAccumulator.N = tempModelUtilsAccumulator.N;
                    tempUtilsAccumulator.Sum = tempModelUtilsAccumulator.Sum;

                    tempNumericNormContainer.HashNorms.put(keyInternal, tempUtilsAccumulator);
                }

                resultNormContainerMgr.HashMapNumericNormsMeans.put(key, tempNumericNormContainer);
            }

            foreach (string key in HashMapNumericNormsSds.Keys)
            {
                tempModelNumericNormContainerSds = HashMapNumericNormsSds[key];

                tempNumericNormContainer = new NumericNormContainer();
                tempNumericNormContainer.HashNorms = new HashMap();
                foreach (string keyInternal in tempModelNumericNormContainerSds.HashNorms.Keys)
                {
                    tempModelUtilsAccumulator = tempModelNumericNormContainerSds.HashNorms[keyInternal];
                    tempUtilsAccumulator = new UtilsAccumulator();
                    tempUtilsAccumulator.Mu = tempModelUtilsAccumulator.Mu;
                    tempUtilsAccumulator.N = tempModelUtilsAccumulator.N;
                    tempUtilsAccumulator.Sum = tempModelUtilsAccumulator.Sum;

                    tempNumericNormContainer.HashNorms.put(keyInternal, tempUtilsAccumulator);
                }

                resultNormContainerMgr.HashMapNumericNormsSds.put(key, tempNumericNormContainer);
            }


            return resultNormContainerMgr;
        }
    }
}
