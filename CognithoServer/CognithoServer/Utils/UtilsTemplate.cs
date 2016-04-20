using CognithoServer.Logic.Statistics.Comparison;
using CognithoServer.Logic.Statistics.Comparison.Comparers;
using CognithoServer.Models;
using CognithoServer.Objects;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CognithoServer.Utils
{
    public class UtilsTemplate
    {
        public Template GetAuthTemplate(Template templateStored, Template template)
        {
            List<GestureObj> tempListGestures = new List<GestureObj>();
            Dictionary<int, bool> dictInstructionIdxs = new UtilsData().GetInstructionIdxsHash(template);
            for (int idxGesture = 0; idxGesture < templateStored.Gestures.Count; idxGesture++)
            {
                if (dictInstructionIdxs.ContainsKey(templateStored.Gestures[idxGesture].InstructionIdx))
                {
                    tempListGestures.Add(templateStored.Gestures[idxGesture]);
                }
            }
            templateStored.Gestures = tempListGestures;

            return templateStored;
        }

        public bool Update(Template templateStored, Template template)
        {
            MongoCollection<Template> templates = new UtilsDB().GetCollectionTemplates();

            templateStored.__v++;
            templateStored.Updated = DateTime.Now;
            templateStored.Gestures = template.Gestures;

            templates.Save(templateStored);
            return true;
        }

        public double GetGestureUniqueFactor(GestureObj gesture)
        {
            double uniqueFactor = 0;
            double numUniqueFactorParams = 0;

            List<StrokeCompParamContainer> listStrokeComps;
            List<GestureCompParamContainer> listGestureComps = new List<GestureCompParamContainer>();           

            GestureCompParamContainer gestureCompParamContainer;
            StrokeCompParamContainer strokeCompParamContainer;

            Stroke tempStroke;

            listStrokeComps = new List<StrokeCompParamContainer>();

            CompParamMgr tempCompParamMgr;

            for (int idxStroke = 0; idxStroke < gesture.Strokes.Count; idxStroke++)
            {
                tempStroke = gesture.Strokes[idxStroke];

                if (tempStroke.Length > Consts.MIN_STROKE_LENGTH)
                {
                    strokeCompParamContainer = new StrokeCompParamContainer(tempStroke, gesture.InstructionIdx);
                    listStrokeComps.Add(strokeCompParamContainer);

                    tempCompParamMgr = new CompParamMgr(strokeCompParamContainer);
                    tempCompParamMgr.RunComparison();

                    uniqueFactor += tempCompParamMgr.GetAvgZDiffScore();
                    numUniqueFactorParams++;
                }
            }

            if (gesture.Strokes.Count > 1)
            {
                gestureCompParamContainer = new GestureCompParamContainer(listStrokeComps, gesture.Strokes);
                tempCompParamMgr = new CompParamMgr(gestureCompParamContainer);
                tempCompParamMgr.RunComparison();

                uniqueFactor += tempCompParamMgr.GetAvgZDiffScore();
                numUniqueFactorParams++;
            }

            uniqueFactor = uniqueFactor / numUniqueFactorParams;

            return uniqueFactor;
        }

        public List<GestureUniqueFactor> GetTemplateUniqueFactors(Template template)
        {
            double tempGestureUniqueFactor;

            GestureObj tempGesture;
            GestureUniqueFactor gestureUniqueFactor;
            List<GestureUniqueFactor> listGestureUniqueFactor = new List<GestureUniqueFactor>();

            double[] listTemplateUniqueFactors = new double[template.Gestures.Count];

            for (int idxGesture = 0; idxGesture < template.Gestures.Count; idxGesture++)
            {
                tempGesture = template.Gestures[idxGesture];

                tempGestureUniqueFactor = GetGestureUniqueFactor(tempGesture);

                listTemplateUniqueFactors[idxGesture] = tempGestureUniqueFactor;

                gestureUniqueFactor = new GestureUniqueFactor(tempGesture.InstructionIdx, tempGestureUniqueFactor, tempGesture.Strokes.Count);
                listGestureUniqueFactor.Add(gestureUniqueFactor);
            }

            List<GestureUniqueFactor> listGestureUniqueFactorSorted = listGestureUniqueFactor.OrderByDescending(o => o.ZScore).ToList();
            listGestureUniqueFactorSorted = listGestureUniqueFactorSorted.OrderByDescending(o => o.NumStrokes).ToList();

            return listGestureUniqueFactorSorted;
        }
    }
}