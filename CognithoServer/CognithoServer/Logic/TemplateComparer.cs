using CognithoServer.Logic.Statistics.Norms;
using CognithoServer.Models;
using CognithoServer.Objects;
using CognithoServer.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace CognithoServer.Logic
{
    public class TemplateComparer
    {
        public TemplateResultObj TemplateResult;

        Template mTemplateVerify, mTemplateStored;

        protected StringBuilder mStringBuilder;

        public void Init(Template templateVerify, Template templateStored)
        {
            TemplateResult = new TemplateResultObj();
            TemplateResult.TemplatesMatch = true;

            mTemplateVerify = templateVerify;
            mTemplateStored = templateStored;

            mStringBuilder = new StringBuilder();

            RunComparison();
        }

        public void RunComparison()
        {

            if (mTemplateVerify.Gestures.Count != mTemplateStored.Gestures.Count && mTemplateVerify.Gestures.Count == UtilsConfig.INSTRUCTIONS_FOR_AUTH)
            {
                TemplateResult.TemplatesMatch = false;
            }
            else
            {
                GestureObj tempGestVerify, tempGestStored;

                GestureComparer gestureComparer = new GestureComparer();

                double allowedMismatches = 0;
                double totalMismatches = 0;

                for (int idx = 0; idx < mTemplateVerify.Gestures.Count; idx++)
                {
                    tempGestVerify = mTemplateVerify.Gestures[idx];
                    tempGestStored = GetGestureObjByInstructionIdx(mTemplateStored, tempGestVerify.InstructionIdx);

                    gestureComparer.Init(tempGestVerify, tempGestStored);                                        
                    gestureComparer.InstructionIdx = tempGestVerify.InstructionIdx;

                    TemplateResult.GestureResults.Add(gestureComparer.GestureResult);

                    totalMismatches += gestureComparer.NumGestureMismatch;

                    mStringBuilder.Append(string.Format("[Gesture {0}: {1}]", idx.ToString(), gestureComparer.ToString()));
                }

                if (totalMismatches > allowedMismatches)
                {
                    TemplateResult.TemplatesMatch = false;
                }
            }                        
        }

        private GestureObj GetGestureObjByInstructionIdx(Template template, int instructionIdx)
        {
            GestureObj tempObj = new GestureObj();

            for (int idxGesture = 0; idxGesture < template.Gestures.Count; idxGesture++)
            {
                if (template.Gestures[idxGesture].InstructionIdx == instructionIdx)
                {
                    tempObj = template.Gestures[idxGesture];
                    break;
                }
            }

            return tempObj;
        }

        private bool CheckIfShapesIdentical(List<MotionEventCompact> listVerify, List<MotionEventCompact> listStored)
        {
            bool isShapesIdentical = true;

            if (listStored.Count == listVerify.Count)
            {
                for (int idxEvent = 0; idxEvent < listStored.Count; idxEvent++)
                {
                    if (listStored[idxEvent].X != listVerify[idxEvent].X || listStored[idxEvent].Y != listVerify[idxEvent].Y)
                    {
                        isShapesIdentical = false;
                        break;
                    }
                }
            }
            else
            {
                isShapesIdentical = false;
            }

            return isShapesIdentical;
        }

        public string ToString()
        {
            return mStringBuilder.ToString();
        }
    }
}