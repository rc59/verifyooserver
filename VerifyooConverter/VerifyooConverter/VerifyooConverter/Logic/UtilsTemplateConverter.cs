using JsonConverter.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VerifyooLogic.UserProfile;

namespace VerifyooConverter.Logic
{
    class UtilsTemplateConverter
    {
        public static Template ConvertTemplate(ModelTemplate modelTemplate)
        {
            Template template = new Template();
            template.ListGestures = new java.util.ArrayList();
            CompactGesture tempGesture;

            template.XDpi = modelTemplate.Xdpi;
            template.YDpi = modelTemplate.Ydpi;

            for (int idx = 0; idx < modelTemplate.ExpShapeList.Count; idx++)
            {
                tempGesture = ConvertGesture(modelTemplate.ExpShapeList[idx]);
                template.ListGestures.add(tempGesture);
            }
            return template;
        }

        public static CompactGesture ConvertGesture(ModelGesture modelGesture)
        {
            CompactGesture tempObj = new CompactGesture();
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

            return tempObj;
        }

        public static MotionEventCompact ConvertMotionEvent(ModelMotionEventCompact modelMotionEvent)
        {
            MotionEventCompact tempObj = new MotionEventCompact();
            tempObj.AngleX = modelMotionEvent.AngleX;
            tempObj.AngleY = modelMotionEvent.AngleY;
            tempObj.AngleZ = modelMotionEvent.AngleZ;
            tempObj.EventTime = modelMotionEvent.EventTime;
            tempObj.Pressure = modelMotionEvent.Pressure;
            tempObj.TouchSurface = modelMotionEvent.TouchSurface;
            tempObj.Xpixel = modelMotionEvent.X;
            tempObj.Ypixel = modelMotionEvent.Y;

            return tempObj;
        }
    }
}
