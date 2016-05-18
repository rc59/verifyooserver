using Data.UserProfile.Extended;
using Data.UserProfile.Raw;
using JsonConverter.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonConverter.Logic
{
    class UtilsConvert
    {
        static double tempXdpi;
        static double tempYdpi;

        public static TemplateExtended ConvertTemplate(ModelShapes modelTemplate)
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

        public static Gesture ConvertGesture(ModelShape modelGesture)
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

        public static string GestureToString(ModelShapes shapes, ModelShape shape, GestureExtended gesture)
        {
            StringBuilder strBuilder = new StringBuilder();

            strBuilder.Append(shapes._id.ToString());
            strBuilder.Append(",");

            strBuilder.Append(shape._id.ToString());
            strBuilder.Append(",");

            strBuilder.Append(shapes.Name);
            strBuilder.Append(",");

            strBuilder.Append(shapes.ModelName);
            strBuilder.Append(",");

            strBuilder.Append(shapes.Xdpi);
            strBuilder.Append(",");

            strBuilder.Append(shapes.Ydpi);
            strBuilder.Append(",");

            strBuilder.Append(gesture.Instruction);
            strBuilder.Append(",");

            strBuilder.Append(gesture.GestureAverageVelocity.ToString());
            strBuilder.Append(",");

            strBuilder.Append(gesture.GestureLengthMM.ToString());
            strBuilder.Append(",");

            strBuilder.Append(gesture.GestureTotalTimeWithoutPauses.ToString());
            strBuilder.Append(",");

            strBuilder.Append(gesture.GestureTotalTimeWithPauses.ToString());
            strBuilder.Append(",");

            strBuilder.Append(gesture.GestureTotalStrokeArea.ToString());
            strBuilder.Append(",");

            strBuilder.Append(gesture.GestureMaxPressure.ToString());
            strBuilder.Append(",");

            strBuilder.Append(gesture.GestureMaxSurface.ToString());
            strBuilder.Append(",");

            strBuilder.Append(gesture.GestureAvgPressure.ToString());
            strBuilder.Append(",");

            strBuilder.Append(gesture.GestureAvgSurface.ToString());
            strBuilder.Append(",");

            strBuilder.Append(gesture.GestureAvgMiddlePressure.ToString());
            strBuilder.Append(",");

            strBuilder.Append(gesture.GestureAvgMiddleSurface.ToString());
            strBuilder.Append(",");

            strBuilder.Append(gesture.GestureMaxAccX.ToString());
            strBuilder.Append(",");

            strBuilder.Append(gesture.GestureMaxAccY.ToString());
            strBuilder.Append(",");

            strBuilder.Append(gesture.GestureMaxAccZ.ToString());
            strBuilder.Append(",");

            strBuilder.Append(gesture.GestureAvgAccX.ToString());
            strBuilder.Append(",");

            strBuilder.Append(gesture.GestureAvgAccY.ToString());
            strBuilder.Append(",");

            strBuilder.Append(gesture.GestureAvgAccZ.ToString());
            strBuilder.Append(",");

            strBuilder.Append(gesture.GestureAverageStartAcceleration.ToString());
            strBuilder.Append(",");

            strBuilder.Append(gesture.GestureAccumulatedLengthLinearRegIntercept.ToString());
            strBuilder.Append(",");

            strBuilder.Append(gesture.GestureAccumulatedLengthLinearRegRSqr.ToString());
            strBuilder.Append(",");

            strBuilder.Append(gesture.GestureAccumulatedLengthLinearRegSlope.ToString());
            strBuilder.Append(",");

            strBuilder.Append(gesture.GestureVelocityPeakMax.ToString());         

            return strBuilder.ToString();
        }
    }
}
