using Data.UserProfile.Extended;
using Data.UserProfile.Raw;
using Logic.Comparison.Stats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VerifyooSimulator.Models;

namespace VerifyooSimulator.Logic
{
    class UtilsTemplateConverter
    {
        static double tempXdpi;
        static double tempYdpi;        

        protected bool CheckInstruction(string instruction)
        {
            bool isValid = true;
            if (instruction.CompareTo("LINES") == 0 || instruction.CompareTo("HEART") == 0)
            {
                isValid = false;
            }
            return isValid;
        }

        public static TemplateExtended ConvertTemplateNew(ModelTemplate modelTemplate)
        {
            
            try
            {
                Template template = new Template();
                template.ListGestures = new java.util.ArrayList();

                Template templateTestSamples = new Template();
                templateTestSamples.ListGestures = new java.util.ArrayList();

                Gesture tempGesture;

                //EnvVars.TemplateId = modelTemplate._id.ToString();

                tempXdpi = modelTemplate.Xdpi;
                tempYdpi = modelTemplate.Ydpi;

                double startTime, endTime, tempInterval;

                int tempSize;

                MotionEventCompact tempEvent;
                Stroke tempStrokeLast;
                int tempStrokeSize;

                for (int idx = 0; idx < modelTemplate.ExpShapeList.Count; idx++)
                {                    
                    tempGesture = ConvertGesture(modelTemplate.ExpShapeList[idx]);

                    startTime = ((MotionEventCompact)((Stroke)tempGesture.ListStrokes.get(0)).ListEvents.get(0)).EventTime;
                    tempSize = tempGesture.ListStrokes.size();
                    tempStrokeLast = (Stroke)tempGesture.ListStrokes.get(tempSize - 1);
                    tempStrokeSize = tempStrokeLast.ListEvents.size();
                    tempEvent = (MotionEventCompact)tempStrokeLast.ListEvents.get(tempStrokeSize - 1);
                    endTime = tempEvent.EventTime;
                    tempInterval = endTime - startTime;

                    template.ListGestures.add(tempGesture);
                }

                TemplateExtended templateExtended = new TemplateExtended(template);
                TemplateExtended templateExtendedTestSamples = new TemplateExtended(templateTestSamples);                

                templateExtended.Id = modelTemplate._id.ToString();
                templateExtended.Name = modelTemplate.Name;
                templateExtended.ModelName = modelTemplate.ModelName;
                return templateExtended;
            }
            catch (Exception exc)
            {
                return null;
            }
        }

        public static TemplateExtended ConvertTemplate(ModelTemplate modelTemplate, out List<GestureExtended> testSamples)
        {
            testSamples = new List<GestureExtended>();
            try
            {
                Template template = new Template();
                template.ListGestures = new java.util.ArrayList();

                Template templateTestSamples = new Template();
                templateTestSamples.ListGestures = new java.util.ArrayList();

                Gesture tempGesture;

                //EnvVars.TemplateId = modelTemplate._id.ToString();

                tempXdpi = modelTemplate.Xdpi;
                tempYdpi = modelTemplate.Ydpi;

                double startTime, endTime, tempInterval;

                int tempSize;

                MotionEventCompact tempEvent;
                Stroke tempStrokeLast;
                int tempStrokeSize;

                for (int idx = 7; idx < modelTemplate.ExpShapeList.Count; idx++)
                {
                    if(modelTemplate.ExpShapeList[idx].Instruction != "LINE")
                    {
                        tempGesture = ConvertGesture(modelTemplate.ExpShapeList[idx]);

                        startTime = ((MotionEventCompact)((Stroke)tempGesture.ListStrokes.get(0)).ListEvents.get(0)).EventTime;
                        tempSize = tempGesture.ListStrokes.size();
                        tempStrokeLast = (Stroke)tempGesture.ListStrokes.get(tempSize - 1);
                        tempStrokeSize = tempStrokeLast.ListEvents.size();
                        tempEvent = (MotionEventCompact)tempStrokeLast.ListEvents.get(tempStrokeSize - 1);
                        endTime = tempEvent.EventTime;
                        tempInterval = endTime - startTime;

                        if (idx < 28)
                        {
                            template.ListGestures.add(tempGesture);
                        }
                        else
                        {
                            templateTestSamples.ListGestures.Add(tempGesture);
                        }
                    }
                }

                TemplateExtended templateExtended = new TemplateExtended(template);
                TemplateExtended templateExtendedTestSamples = new TemplateExtended(templateTestSamples);

                for(int idxTestGesture = 0; idxTestGesture < templateExtendedTestSamples.ListGestureExtended.size(); idxTestGesture++)
                {
                    testSamples.Add((GestureExtended)templateExtendedTestSamples.ListGestureExtended.get(idxTestGesture));
                }                 

                templateExtended.Id = modelTemplate._id.ToString();
                templateExtended.Name = modelTemplate.Name;
                templateExtended.ModelName = modelTemplate.ModelName;
                return templateExtended;
            }
            catch(Exception exc)
            {
                return null;
            }
        }

        public static Gesture ConvertGesture(ModelGesture modelGesture)
        {
            //EnvVars.GestureId = modelGesture._id.ToString();

            Gesture tempObj = new Gesture();
            tempObj.ListStrokes = new java.util.ArrayList();
            Stroke tempStroke;
            tempObj.Instruction = modelGesture.Instruction;

            for (int idx = 0; idx < modelGesture.Strokes.Count; idx++)
            {
                tempStroke = ConvertStroke(modelGesture.Strokes[idx]);
                tempObj.ListStrokes.add(tempStroke);
            }

            tempObj.Id = modelGesture._id.ToString();
            return tempObj;
        }

        public static Stroke ConvertStroke(ModelStroke modelStroke)
        {
            Stroke tempObj = new Stroke();
            tempObj.Length = modelStroke.Length;
            tempObj.ListEvents = new java.util.ArrayList();

            MotionEventCompact tempEvent;

            for (int idx = 0; idx < modelStroke.ListEvents.Count; idx++)
            {
                tempEvent = ConvertMotionEvent(modelStroke.ListEvents[idx]);
                tempObj.ListEvents.add(tempEvent);
            }

            tempObj.Xdpi = tempXdpi;
            tempObj.Ydpi = tempYdpi;
            tempObj.Id = modelStroke._id.ToString();
            return tempObj;
        }

        public static MotionEventCompact ConvertMotionEvent(ModelMotionEventCompact modelMotionEvent)
        {
            MotionEventCompact tempObj = new MotionEventCompact();
            tempObj.SetAccelerometerX(modelMotionEvent.AngleX);
            tempObj.SetAccelerometerY(modelMotionEvent.AngleY);
            tempObj.SetAccelerometerZ(modelMotionEvent.AngleZ);

            tempObj.SetGyroX(modelMotionEvent.GyroX);
            tempObj.SetGyroY(modelMotionEvent.GyroY);
            tempObj.SetGyroZ(modelMotionEvent.GyroZ);

            tempObj.EventTime = modelMotionEvent.EventTime;
            tempObj.Pressure = modelMotionEvent.Pressure;
            tempObj.TouchSurface = modelMotionEvent.TouchSurface;
            tempObj.Xpixel = modelMotionEvent.X;
            tempObj.Ypixel = modelMotionEvent.Y;

            tempObj.IsHistory = modelMotionEvent.IsHistory;

            tempObj.Id = modelMotionEvent._id.ToString();
            return tempObj;
        }
    }
}
