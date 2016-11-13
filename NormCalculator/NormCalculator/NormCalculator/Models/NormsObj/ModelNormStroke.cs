using Data.MetaData;
using Logic.Utils.DTW;
using System.Collections.Generic;
using System;

namespace NormCalculator.Models
{
    public class ModelNormStroke
    {
        public List<DTWCoordObj> ListObjDTW = new List<DTWCoordObj>();

        public double[] SpatialSamplingVector;
        public double[] SpatialSamplingVectorX;
        public double[] SpatialSamplingVectorY;

        public ModelNormStroke()
        {
            ListObjDTW = new List<DTWCoordObj>();
        }

        public ModelNormStroke(NormStroke tempNormStroke) {
            SpatialSamplingVector = tempNormStroke.SpatialSamplingVector;
            SpatialSamplingVectorX = tempNormStroke.SpatialSamplingVectorX;
            SpatialSamplingVectorY = tempNormStroke.SpatialSamplingVectorY;

            ListObjDTW = new List<DTWCoordObj>();
            DTWObjCoordinate tempDTWObjCoordinate;

            for (int idx = 0; idx < tempNormStroke.ListObjDTW.size(); idx++)
            {
                tempDTWObjCoordinate = (DTWObjCoordinate)tempNormStroke.ListObjDTW.get(idx);
                ListObjDTW.Add(new DTWCoordObj(tempDTWObjCoordinate.X, tempDTWObjCoordinate.Y));
            }
        }

        public NormStroke ToNormStroke()
        {
            NormStroke tempNormStroke = new NormStroke();
            tempNormStroke.ListObjDTW = new java.util.ArrayList();

            tempNormStroke.SpatialSamplingVector = SpatialSamplingVector;
            tempNormStroke.SpatialSamplingVectorX = SpatialSamplingVectorX;
            tempNormStroke.SpatialSamplingVectorY = SpatialSamplingVectorY;

            IDTWObj tempIDTWObj;
            for (int idx = 0; idx < ListObjDTW.Count; idx++)
            {
                tempIDTWObj = new DTWObjCoordinate(ListObjDTW[idx].X, ListObjDTW[idx].Y);
                tempNormStroke.ListObjDTW.add(tempIDTWObj);
            }

            return tempNormStroke;
        }
    }
}