using Data.MetaData;
using java.util;
using Logic.Comparison.Stats.Norms;
using Logic.Utils;
using NormCalculator.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VerifyooSimulator.Models.NormsObj
{
    class ModelNormContainerMgr
    {
        public List<ModelNormStroke> ListNormStrokes = new List<ModelNormStroke>();

        public Dictionary<String, ModelSpatialNormContainer> HashMapSpatialNormsMeansDistance = new Dictionary<string, ModelSpatialNormContainer>();
        public Dictionary<String, ModelSpatialNormContainer> HashMapSpatialNormsSdsDistance = new Dictionary<string, ModelSpatialNormContainer>();

        public Dictionary<String, ModelSpatialNormContainer> HashMapSpatialNormsMeansTime = new Dictionary<string, ModelSpatialNormContainer>();
        public Dictionary<String, ModelSpatialNormContainer> HashMapSpatialNormsSdsTime = new Dictionary<string, ModelSpatialNormContainer>();

        public Dictionary<String, ModelNumericNormContainer> HashMapNumericNormsMeans = new Dictionary<string, ModelNumericNormContainer>();
        public Dictionary<String, ModelNumericNormContainer> HashMapNumericNormsSds = new Dictionary<string, ModelNumericNormContainer>();

        public ModelNormContainerMgr()
        {
            HashMapSpatialNormsMeansDistance = new Dictionary<string, ModelSpatialNormContainer>();
            HashMapSpatialNormsSdsDistance = new Dictionary<string, ModelSpatialNormContainer>();

            HashMapSpatialNormsMeansTime = new Dictionary<string, ModelSpatialNormContainer>();
            HashMapSpatialNormsSdsTime = new Dictionary<string, ModelSpatialNormContainer>();

            HashMapNumericNormsMeans = new Dictionary<string, ModelNumericNormContainer>();
            HashMapNumericNormsSds = new Dictionary<string, ModelNumericNormContainer>();
        }

        public ModelNormContainerMgr(NormContainerMgr normContainerMgr)
        {
            SpatialNormContainer tempSpatialNormContainerMeansDistance;
            SpatialNormContainer tempSpatialNormContainerSdsDistance;

            SpatialNormContainer tempSpatialNormContainerMeansTime;
            SpatialNormContainer tempSpatialNormContainerSdsTime;

            NumericNormContainer tempNumericNormContainerMeans;
            NumericNormContainer tempNumericNormContainerSds;

            object[] keySetSpatialNormsDistance = normContainerMgr.HashMapSpatialNormsMeansDistance.keySet().toArray();
            object[] keySetSpatialNormsTime = normContainerMgr.HashMapSpatialNormsMeansTime.keySet().toArray();

            object[] keySetNumericNorms = normContainerMgr.HashMapNumericNormsMeans.keySet().toArray();

            object[] keySetTempListUtilsAccumulators;
            object[] keySetTempUtilsAccumulators;

            UtilsAccumulator tempUtilsAccumulator;
            AccumulatorsContainer tempAccumulatorContainer;

            ModelNumericNormContainer tempModelNumericNormContainerMeans;
            ModelNumericNormContainer tempModelNumericNormContainerSds;

            ModelSpatialNormContainer tempModelSpatialNormContainerMeansDistance;
            ModelSpatialNormContainer tempModelSpatialNormContainerSdsDistance;

            ModelSpatialNormContainer tempModelSpatialNormContainerMeansTime;
            ModelSpatialNormContainer tempModelSpatialNormContainerSdsTime;

            ModelAccumulatorsContainer tempModelAccumulatorContainer;

            string tempKey, tempKeyInternal;

            NormStroke tempNormStroke;
            ModelNormStroke tempModelNormStroke;
            for (int idx = 0; idx < normContainerMgr.ListNormStrokes.size(); idx++)
            {
                tempNormStroke = (NormStroke)normContainerMgr.ListNormStrokes.get(idx);
                tempModelNormStroke = new ModelNormStroke(tempNormStroke);
                ListNormStrokes.Add(tempModelNormStroke);
            }

            for (int idx = 0; idx < keySetSpatialNormsDistance.Length; idx++)
            {
                tempKey = (string)keySetSpatialNormsDistance[idx];

                tempSpatialNormContainerMeansDistance = (SpatialNormContainer)normContainerMgr.HashMapSpatialNormsMeansDistance.get(tempKey);
                tempSpatialNormContainerSdsDistance = (SpatialNormContainer)normContainerMgr.HashMapSpatialNormsSdsDistance.get(tempKey);

                tempSpatialNormContainerMeansTime = (SpatialNormContainer)normContainerMgr.HashMapSpatialNormsMeansTime.get(tempKey);
                tempSpatialNormContainerSdsTime = (SpatialNormContainer)normContainerMgr.HashMapSpatialNormsSdsTime.get(tempKey);

                keySetTempListUtilsAccumulators = tempSpatialNormContainerMeansDistance.HashNorms.keySet().toArray();
                tempModelSpatialNormContainerMeansDistance = new ModelSpatialNormContainer();

                /****************************************** SPATIAL DISTANCE ******************************************/

                for (int idxAccumulatorContainer = 0; idxAccumulatorContainer < keySetTempListUtilsAccumulators.Length; idxAccumulatorContainer++)
                {
                    tempKeyInternal = (string)keySetTempListUtilsAccumulators[idxAccumulatorContainer];
                    tempAccumulatorContainer = (AccumulatorsContainer)tempSpatialNormContainerMeansDistance.HashNorms.get(tempKeyInternal);

                    tempModelAccumulatorContainer = new ModelAccumulatorsContainer();
                    tempModelAccumulatorContainer.ListUtilsAccumulator = new List<ModelUtilsAccum>();
                    for (int idxUtilAccum = 0; idxUtilAccum < tempAccumulatorContainer.ListUtilsAccumulator.size(); idxUtilAccum++)
                    {
                        tempModelAccumulatorContainer.ListUtilsAccumulator.Add(new ModelUtilsAccum((UtilsAccumulator)tempAccumulatorContainer.ListUtilsAccumulator.get(idxUtilAccum)));
                    }

                    tempModelSpatialNormContainerMeansDistance.HashNorms.Add(tempKeyInternal, tempModelAccumulatorContainer);
                }

                keySetTempListUtilsAccumulators = tempSpatialNormContainerSdsDistance.HashNorms.keySet().toArray();
                tempModelSpatialNormContainerSdsDistance = new ModelSpatialNormContainer();

                for (int idxAccumulatorContainer = 0; idxAccumulatorContainer < keySetTempListUtilsAccumulators.Length; idxAccumulatorContainer++)
                {
                    tempKeyInternal = (string)keySetTempListUtilsAccumulators[idxAccumulatorContainer];
                    tempAccumulatorContainer = (AccumulatorsContainer)tempSpatialNormContainerSdsDistance.HashNorms.get(tempKeyInternal);

                    tempModelAccumulatorContainer = new ModelAccumulatorsContainer();
                    tempModelAccumulatorContainer.ListUtilsAccumulator = new List<ModelUtilsAccum>();
                    for (int idxUtilAccum = 0; idxUtilAccum < tempAccumulatorContainer.ListUtilsAccumulator.size(); idxUtilAccum++)
                    {
                        tempModelAccumulatorContainer.ListUtilsAccumulator.Add(new ModelUtilsAccum((UtilsAccumulator)tempAccumulatorContainer.ListUtilsAccumulator.get(idxUtilAccum)));
                    }

                    tempModelSpatialNormContainerSdsDistance.HashNorms.Add(tempKeyInternal, tempModelAccumulatorContainer);
                }

                /****************************************** SPATIAL TIME ******************************************/

                keySetTempListUtilsAccumulators = tempSpatialNormContainerMeansTime.HashNorms.keySet().toArray();
                tempModelSpatialNormContainerMeansTime = new ModelSpatialNormContainer();

                for (int idxAccumulatorContainer = 0; idxAccumulatorContainer < keySetTempListUtilsAccumulators.Length; idxAccumulatorContainer++)
                {
                    tempKeyInternal = (string)keySetTempListUtilsAccumulators[idxAccumulatorContainer];
                    tempAccumulatorContainer = (AccumulatorsContainer)tempSpatialNormContainerMeansTime.HashNorms.get(tempKeyInternal);

                    tempModelAccumulatorContainer = new ModelAccumulatorsContainer();
                    tempModelAccumulatorContainer.ListUtilsAccumulator = new List<ModelUtilsAccum>();
                    for (int idxUtilAccum = 0; idxUtilAccum < tempAccumulatorContainer.ListUtilsAccumulator.size(); idxUtilAccum++)
                    {
                        tempModelAccumulatorContainer.ListUtilsAccumulator.Add(new ModelUtilsAccum((UtilsAccumulator)tempAccumulatorContainer.ListUtilsAccumulator.get(idxUtilAccum)));
                    }

                    tempModelSpatialNormContainerMeansTime.HashNorms.Add(tempKeyInternal, tempModelAccumulatorContainer);
                }

                keySetTempListUtilsAccumulators = tempSpatialNormContainerSdsTime.HashNorms.keySet().toArray();
                tempModelSpatialNormContainerSdsTime = new ModelSpatialNormContainer();

                for (int idxAccumulatorContainer = 0; idxAccumulatorContainer < keySetTempListUtilsAccumulators.Length; idxAccumulatorContainer++)
                {
                    tempKeyInternal = (string)keySetTempListUtilsAccumulators[idxAccumulatorContainer];
                    tempAccumulatorContainer = (AccumulatorsContainer)tempSpatialNormContainerSdsTime.HashNorms.get(tempKeyInternal);

                    tempModelAccumulatorContainer = new ModelAccumulatorsContainer();
                    tempModelAccumulatorContainer.ListUtilsAccumulator = new List<ModelUtilsAccum>();
                    for (int idxUtilAccum = 0; idxUtilAccum < tempAccumulatorContainer.ListUtilsAccumulator.size(); idxUtilAccum++)
                    {
                        tempModelAccumulatorContainer.ListUtilsAccumulator.Add(new ModelUtilsAccum((UtilsAccumulator)tempAccumulatorContainer.ListUtilsAccumulator.get(idxUtilAccum)));
                    }

                    tempModelSpatialNormContainerSdsTime.HashNorms.Add(tempKeyInternal, tempModelAccumulatorContainer);
                }

                HashMapSpatialNormsMeansDistance.Add(tempKey, tempModelSpatialNormContainerMeansDistance);
                HashMapSpatialNormsSdsDistance.Add(tempKey, tempModelSpatialNormContainerSdsDistance);

                HashMapSpatialNormsMeansTime.Add(tempKey, tempModelSpatialNormContainerMeansTime);
                HashMapSpatialNormsSdsTime.Add(tempKey, tempModelSpatialNormContainerSdsTime);
            }

            for (int idx = 0; idx < keySetNumericNorms.Length; idx++)
            {
                tempKey = (string)keySetNumericNorms[idx];
                tempNumericNormContainerMeans = (NumericNormContainer)normContainerMgr.HashMapNumericNormsMeans.get(tempKey);
                tempNumericNormContainerSds = (NumericNormContainer)normContainerMgr.HashMapNumericNormsSds.get(tempKey);

                keySetTempUtilsAccumulators = tempNumericNormContainerMeans.HashNorms.keySet().toArray();
                tempModelNumericNormContainerMeans = new ModelNumericNormContainer();

                for (int idxUtilsAccumulators = 0; idxUtilsAccumulators < keySetTempUtilsAccumulators.Length; idxUtilsAccumulators++)
                {
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

            ModelSpatialNormContainer tempModelSpatialNormContainerMeansDistance;
            ModelSpatialNormContainer tempModelSpatialNormContainerSdsDistance;

            ModelSpatialNormContainer tempModelSpatialNormContainerMeansTime;
            ModelSpatialNormContainer tempModelSpatialNormContainerSdsTime;

            ModelNumericNormContainer tempModelNumericNormContainerMeans;
            ModelNumericNormContainer tempModelNumericNormContainerSds;

            ModelAccumulatorsContainer tempModelAccumulatorsContainer;
            UtilsAccumulator tempUtilsAccumulator;
            ModelUtilsAccum tempModelUtilsAccumulator;

            SpatialNormContainer tempSpatialNormContainer;
            NumericNormContainer tempNumericNormContainer;

            AccumulatorsContainer tempAccumulatorsContainer;

            resultNormContainerMgr.ListNormStrokes = new ArrayList();
            NormStroke tempNormStroke;
            for (int idx = 0; idx < ListNormStrokes.Count; idx++)
            {
                tempNormStroke = ListNormStrokes[idx].ToNormStroke();
                resultNormContainerMgr.ListNormStrokes.add(tempNormStroke);
            }

            /****************************************** SPATIAL DISTANCE ******************************************/

            foreach (string key in HashMapSpatialNormsMeansDistance.Keys)
            {
                tempModelSpatialNormContainerMeansDistance = HashMapSpatialNormsMeansDistance[key];

                tempSpatialNormContainer = new SpatialNormContainer();
                tempSpatialNormContainer.HashNorms = new HashMap();
                foreach (string keyInternal in tempModelSpatialNormContainerMeansDistance.HashNorms.Keys)
                {
                    tempModelAccumulatorsContainer = tempModelSpatialNormContainerMeansDistance.HashNorms[keyInternal];

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

                resultNormContainerMgr.HashMapSpatialNormsMeansDistance.put(key, tempSpatialNormContainer);
            }

            foreach (string key in HashMapSpatialNormsSdsDistance.Keys)
            {
                tempModelSpatialNormContainerSdsDistance = HashMapSpatialNormsSdsDistance[key];

                tempSpatialNormContainer = new SpatialNormContainer();
                tempSpatialNormContainer.HashNorms = new HashMap();
                foreach (string keyInternal in tempModelSpatialNormContainerSdsDistance.HashNorms.Keys)
                {
                    tempModelAccumulatorsContainer = tempModelSpatialNormContainerSdsDistance.HashNorms[keyInternal];

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

                resultNormContainerMgr.HashMapSpatialNormsSdsDistance.put(key, tempSpatialNormContainer);
            }

            /****************************************** SPATIAL TIME ******************************************/

            foreach (string key in HashMapSpatialNormsMeansDistance.Keys)
            {
                tempModelSpatialNormContainerMeansTime = HashMapSpatialNormsMeansDistance[key];

                tempSpatialNormContainer = new SpatialNormContainer();
                tempSpatialNormContainer.HashNorms = new HashMap();
                foreach (string keyInternal in tempModelSpatialNormContainerMeansTime.HashNorms.Keys)
                {
                    tempModelAccumulatorsContainer = tempModelSpatialNormContainerMeansTime.HashNorms[keyInternal];

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

                resultNormContainerMgr.HashMapSpatialNormsMeansTime.put(key, tempSpatialNormContainer);
            }

            foreach (string key in HashMapSpatialNormsSdsDistance.Keys)
            {
                tempModelSpatialNormContainerSdsTime = HashMapSpatialNormsSdsDistance[key];

                tempSpatialNormContainer = new SpatialNormContainer();
                tempSpatialNormContainer.HashNorms = new HashMap();
                foreach (string keyInternal in tempModelSpatialNormContainerSdsTime.HashNorms.Keys)
                {
                    tempModelAccumulatorsContainer = tempModelSpatialNormContainerSdsTime.HashNorms[keyInternal];

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

                resultNormContainerMgr.HashMapSpatialNormsSdsTime.put(key, tempSpatialNormContainer);
            }

            /****************************************** NUMERIC ******************************************/

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
