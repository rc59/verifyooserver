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

        public static TemplateExtended ConvertTemplate(ModelTemplate modelTemplate)
        {            
            Template template = new Template();
            template.ListGestures = new java.util.ArrayList();
            Gesture tempGesture;

            tempXdpi = modelTemplate.Xdpi;
            tempYdpi = modelTemplate.Ydpi;

            for (int idx = 0; idx < modelTemplate.ExpShapeList.Count; idx++)
            {
                tempGesture = ConvertGesture(modelTemplate.ExpShapeList[idx]);
                template.ListGestures.add(tempGesture);
            }

            TemplateExtended templateExtended = new TemplateExtended(template);
            return templateExtended;
        }

        public static Gesture ConvertGesture(ModelGesture modelGesture)
        {
            Gesture tempObj = new Gesture();
            tempObj.ListStrokes = new java.util.ArrayList();
            Stroke tempStroke;
            tempObj.Instruction = modelGesture.Instruction;

            for (int idx = 0; idx < modelGesture.Strokes.Count; idx++)
            {
                tempStroke = ConvertStroke(modelGesture.Strokes[idx]);
                tempObj.ListStrokes.add(tempStroke);
            }

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
            return tempObj;
        }

        public static MotionEventCompact ConvertMotionEvent(ModelMotionEventCompact modelMotionEvent)
        {
            MotionEventCompact tempObj = new MotionEventCompact();
            tempObj.AccelerometerX = modelMotionEvent.AngleX;
            tempObj.AccelerometerY = modelMotionEvent.AngleY;
            tempObj.AccelerometerZ = modelMotionEvent.AngleZ;
            tempObj.EventTime = modelMotionEvent.EventTime;
            tempObj.Pressure = modelMotionEvent.Pressure;
            tempObj.TouchSurface = modelMotionEvent.TouchSurface;
            tempObj.Xpixel = modelMotionEvent.X;
            tempObj.Ypixel = modelMotionEvent.Y;

            return tempObj;
        }
    }
}
