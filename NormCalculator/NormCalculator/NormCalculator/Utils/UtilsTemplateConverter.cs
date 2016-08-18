using Data.UserProfile.Extended;
using Data.UserProfile.Raw;
using JsonConverter.Models;
using Logic.Comparison.Stats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VerifyooConverter.Logic
{
    class UtilsTemplateConverter
    {
        static double tempXdpi;
        static double tempYdpi;

        public static Template ConvertTemplateNormal(ModelTemplate modelTemplate, Dictionary<String, bool> invalidGestures)
        {
            Template template = new Template();
            template.Id = modelTemplate._id.ToString();
            template.Name = modelTemplate.Name;
            template.ListGestures = new java.util.ArrayList();
            Gesture tempGesture;

            EnvVars.TemplateId = modelTemplate._id.ToString();

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

            return template;
        }

        public static TemplateExtended ConvertTemplate(ModelTemplate modelTemplate, Dictionary<String, bool> invalidGestures)
        {            
            Template template = new Template();
            template.ListGestures = new java.util.ArrayList();
            Gesture tempGesture;

            EnvVars.TemplateId = modelTemplate._id.ToString();

            tempXdpi = modelTemplate.Xdpi;
            tempYdpi = modelTemplate.Ydpi;

            double startTime, endTime, tempInterval;

            int tempSize;

            MotionEventCompact tempEvent;
            Stroke tempStrokeLast;
            int tempStrokeSize;

            for (int idx = 7; idx < modelTemplate.ExpShapeList.Count; idx++)
            {
                tempGesture = ConvertGesture(modelTemplate.ExpShapeList[idx]);

                startTime = ((MotionEventCompact)((Stroke)tempGesture.ListStrokes.get(0)).ListEvents.get(0)).EventTime;
                tempSize = tempGesture.ListStrokes.size();
                tempStrokeLast = (Stroke)tempGesture.ListStrokes.get(tempSize - 1);
                tempStrokeSize = tempStrokeLast.ListEvents.size();
                tempEvent = (MotionEventCompact)tempStrokeLast.ListEvents.get(tempStrokeSize - 1);
                endTime = tempEvent.EventTime;
                tempInterval = endTime - startTime;
                if (!invalidGestures.ContainsKey(tempGesture.Id))
                {
                    template.ListGestures.add(tempGesture);
                }
            }            

            TemplateExtended templateExtended = new TemplateExtended(template);
            templateExtended.Id = modelTemplate._id.ToString();
            templateExtended.Name = modelTemplate.Name;
            templateExtended.ModelName = modelTemplate.ModelName;
            return templateExtended;
        }

        public static Gesture ConvertGesture(ModelGesture modelGesture)
        {
            EnvVars.GestureId = modelGesture._id.ToString();

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
            tempObj.EventTime = modelMotionEvent.EventTime;
            tempObj.Pressure = modelMotionEvent.Pressure;
            tempObj.TouchSurface = modelMotionEvent.TouchSurface;
            tempObj.Xpixel = modelMotionEvent.X;
            tempObj.Ypixel = modelMotionEvent.Y;

            tempObj.Id = modelMotionEvent._id.ToString();
            return tempObj;
        }
    }
}
