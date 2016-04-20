using CognithoServer.Models;
using CognithoServer.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using VerifyooLogic.Comparison;
using VerifyooLogic.UserProfile;
using VerifyooLogic.Utils;

namespace CognithoServer.Utils
{
    public class UtilsAuth
    {
        

        public bool IsTokenExpired(AuthToken authToken)
        {
            return false;
        }

        private bool CheckIfShapesIdentical(List<Models.MotionEventCompact> listVerify, List<Models.MotionEventCompact> listStored)
        {
            bool isShapesIdentical = true;

            if(listStored.Count == listVerify.Count)
            {
                for (int idxEvent = 0; idxEvent < listStored.Count; idxEvent++)
                {
                    if(listStored[idxEvent].X != listVerify[idxEvent].X || listStored[idxEvent].Y != listVerify[idxEvent].Y)
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

        public GestureObj GetGestureObjByInstructionIdx(Models.Template template, int instructionIdx)
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

        public bool CompareTemplates(Models.Template tVerify, Models.Template tStored)
        {
            if(tVerify.Gestures.Count != tStored.Gestures.Count)
            {
                return false;
            }
            else
            {
                CompactGesture tempGestureVerify, tempGestureStored;
                GestureComparer gestureComparer = new GestureComparer();

                double score = 0;

                UtilsDeviceProperties.Xdpi = tVerify.Xdpi;
                UtilsDeviceProperties.Ydpi = tVerify.Ydpi;

                int failures = 0;
                int successes = 0;
                for (int idxGesture = 0; idxGesture < tVerify.Gestures.Count; idxGesture++)
                {
                    for(int idxGestureStored = 0; idxGestureStored < tStored.Gestures.Count; idxGestureStored++)
                    {
                        if(tVerify.Gestures[idxGesture].InstructionIdx == tStored.Gestures[idxGestureStored].InstructionIdx)
                        {
                            tempGestureStored = UtilsObjectConverters.ConvertGesture(tStored.Gestures[idxGestureStored]);

                            if(tempGestureStored.IsInTemplate)
                            {
                                tempGestureVerify = UtilsObjectConverters.ConvertGesture(tVerify.Gestures[idxGesture]);

                                score = gestureComparer.Compare(tempGestureVerify, tempGestureStored);

                                if (score < UtilsConfig.SCORE_THRESHOLD)
                                {
                                    failures++;
                                }
                                else
                                {
                                    successes++;
                                }
                            }
                        }
                    }
                }

                score = score / tVerify.Gestures.Count;

                bool isValid = false;
                if (failures <= 1 && successes >= (UtilsConfig.INSTRUCTIONS_FOR_AUTH - 1))
                {
                    isValid = true;
                }
                return isValid;
            }
        }

        public string CompareTemplatesString(Models.Template tVerify, Models.Template tStored)
        {
            return "";
        }

        public TemplateResultObj CompareTemplatesDetailed(Models.Template tVerify, Models.Template tStored)
        {
            return null;
        }
    }
}