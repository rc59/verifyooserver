using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VerifyooLogic.UserProfile;

namespace CognithoServer.Utils
{
    public class UtilsObjectConverters
    {
        public static Template ConvertTemplate(Models.Template modelTemplate)
        {
            Template template = new Template();
            template.ListGestures = new java.util.ArrayList();
            CompactGesture tempGesture;

            for(int idx = 0; idx < modelTemplate.Gestures.Count; idx++)
            {
                tempGesture = ConvertGesture(modelTemplate.Gestures[idx]);
                template.ListGestures.add(tempGesture);
            }
            return template;
        }

        public static CompactGesture ConvertGesture(Models.GestureObj modelGesture)
        {
            CompactGesture tempObj = new CompactGesture();
            tempObj.ListStrokes = new java.util.ArrayList();
            tempObj.IsInTemplate = modelGesture.IsInTemplate;
            Stroke tempStroke;

            for (int idx = 0; idx < modelGesture.Strokes.Count; idx++)
            {
                tempStroke = ConvertStroke(modelGesture.Strokes[idx]);
                tempObj.ListStrokes.add(tempStroke);
            }
            
            return tempObj;
        }

        public static Stroke ConvertStroke(Models.Stroke modelStroke)
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

            return tempObj;
        }

        public static MotionEventCompact ConvertMotionEvent(Models.MotionEventCompact modelMotionEvent)
        {
            MotionEventCompact tempObj = new MotionEventCompact();
            tempObj.AngleX = modelMotionEvent.AngleX;
            tempObj.AngleY = modelMotionEvent.AngleY;
            tempObj.AngleZ = modelMotionEvent.AngleZ;
            tempObj.EventTime = modelMotionEvent.EventTime;
            tempObj.PointerCount = modelMotionEvent.PointerCount;
            tempObj.Pressure = modelMotionEvent.Pressure;
            tempObj.TouchSurface = modelMotionEvent.TouchSurface;
            tempObj.Xpixel = modelMotionEvent.X;
            tempObj.Ypixel = modelMotionEvent.Y;

            return tempObj;
        }
    }
}