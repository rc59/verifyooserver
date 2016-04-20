using CognithoServer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace CognithoServer.Utils
{
    public class UtilsNormalize
    {
        private List<double> _orientations = new List<double>() {
            0, (Math.PI / 4), (Math.PI / 2), (Math.PI * 3 / 4),
            Math.PI, -0, (-Math.PI / 4), (-Math.PI / 2),
            (-Math.PI * 3 / 4), -Math.PI
        };

        public double[] PrepareData(List<MotionEventCompact> eventsList, double length, out List<MotionEventCompact> normalizedEventsList)
        {
            normalizedEventsList = new List<MotionEventCompact>();
            double[] pts = ConvertToVector(eventsList, length, out normalizedEventsList);
            double[] center = ComputeCentroid(pts);
            double orientation = Math.Atan2(pts[1] - center[1], pts[0] - center[0]);

            double adjustment = -orientation;
            int count = _orientations.Count;
            for (int i = 0; i < count; i++)
            {
                double delta = _orientations[i] - orientation;
                if (Math.Abs(delta) < Math.Abs(adjustment))
                {
                    adjustment = delta;
                }
            }

            Translate(pts, -center[0], -center[1]);
            Rotate(pts, adjustment);

            pts = Normalize(pts);

            return pts;
        }

        private double[] Normalize(double[] vector)
        {
            double[] sample = vector;
            double sum = 0;

            int size = sample.Length;
            for (int i = 0; i < size; i++)
            {
                sum += sample[i] * sample[i];
            }

            double magnitude = Math.Sqrt(sum);
            for (int i = 0; i < size; i++)
            {
                sample[i] /= magnitude;
            }

            return sample;
        }

        private double[] Rotate(double[] points, double angle)
        {
            double cos = Math.Cos(angle);
            double sin = Math.Sin(angle);
            int size = points.Length;
            for (int i = 0; i < size; i += 2)
            {
                double x = points[i] * cos - points[i + 1] * sin;
                double y = points[i] * sin + points[i + 1] * cos;
                points[i] = x;
                points[i + 1] = y;
            }
            return points;
        }

        private double[] Translate(double[] points, double dx, double dy)
        {
            int size = points.Length;
            for (int i = 0; i < size; i += 2)
            {
                points[i] += dx;
                points[i + 1] += dy;
            }
            return points;
        }

        private double[] ComputeCentroid(double[] points)
        {
            double centerX = 0;
            double centerY = 0;
            int count = points.Length;
            for (int i = 0; i < count; i++)
            {
                centerX += points[i];
                i++;
                centerY += points[i];
            }
            double[] center = new double[2];
            center[0] = 2 * centerX / count;
            center[1] = 2 * centerY / count;

            return center;
        }

        public double MinimumCosineDistance(double[] vector1, double[] vector2, double numOrientations)
        {
            int len = vector1.Length;
            double a = 0;
            double b = 0;
            for (int i = 0; i < len; i += 2)
            {
                a += vector1[i] * vector2[i] + vector1[i + 1] * vector2[i + 1];
                b += vector1[i] * vector2[i + 1] - vector1[i + 1] * vector2[i];
            }
            if (a != 0)
            {
                double tan = b / a;
                double angle = Math.Atan(tan);
                if (numOrientations > 2 && Math.Abs(angle) >= Math.PI / numOrientations)
                {
                    return Math.Acos(a);
                }
                else
                {
                    double cosine = Math.Cos(angle);
                    double sine = cosine * tan;
                    return Math.Acos(a * cosine + b * sine);
                }
            }
            else
            {
                return Math.PI / 2;
            }
        }

        public double[] ConvertToVector(List<MotionEventCompact> eventsList, double length, out List<MotionEventCompact> normalizedEventsList)
        {
            int minValue = -9999999;
            int numPoints = 16;

            int increment = (int)length / (numPoints - 1);
            int vectorLength = numPoints * 2;
            double[] vector = new double[vectorLength];

            double[] vectorX = new double[vectorLength / 2];
            double[] vectorY = new double[vectorLength / 2];

            double distanceSoFar = 0;

            double[] pts = new double[eventsList.Count * 2];
            for (int idx = 0; idx < eventsList.Count; idx++)
            {
                pts[idx * 2] = eventsList[idx].X;
                pts[idx * 2 + 1] = eventsList[idx].Y;
            }

            double lstPointX = pts[0];
            double lstPointY = pts[1];
            int index = 0;
            double currentPointX = minValue;
            double currentPointY = minValue;

            vector[index] = lstPointX;
            index++;
            vector[index] = lstPointY;
            index++;
            int i = 0;
            int count = pts.Length / 2;

            while (i < count && index < vectorLength)
            {
                if (currentPointX == minValue)
                {
                    i++;
                    if (i >= count)
                    {
                        break;
                    }
                    currentPointX = pts[i * 2];
                    currentPointY = pts[i * 2 + 1];
                }
                double deltaX = currentPointX - lstPointX;
                double deltaY = currentPointY - lstPointY;
                double distance = Math.Sqrt(deltaX * deltaX + deltaY * deltaY);
                if (distanceSoFar + distance >= increment)
                {
                    double ratio = (increment - distanceSoFar) / distance;
                    double nx = lstPointX + ratio * deltaX;
                    double ny = lstPointY + ratio * deltaY;
                    vector[index] = nx;
                    index++;
                    vector[index] = ny;
                    index++;
                    lstPointX = nx;
                    lstPointY = ny;
                    distanceSoFar = 0;
                }
                else
                {
                    lstPointX = currentPointX;
                    lstPointY = currentPointY;
                    currentPointX = minValue;
                    currentPointY = minValue;
                    distanceSoFar += distance;
                }
            }


            for (i = index; i < vectorLength; i += 2)
            {
                vector[i] = lstPointX;
                vector[i + 1] = lstPointY;
            }

            int countIdx = 0;
            StringBuilder builder = new StringBuilder();
            int idxNearestEvent;

            normalizedEventsList = new List<MotionEventCompact>();

            for (int idx = 0; idx < vector.Length; idx += 2)
            {
                idxNearestEvent = FindNearestIdx(vector[idx], vector[idx + 1], eventsList);
                normalizedEventsList.Add(eventsList[idxNearestEvent]);

                builder.Append(vector[idx].ToString());
                builder.Append(",");
                builder.Append(vector[idx + 1].ToString());

                if ((idx + 2) <= vector.Length)
                {
                    builder.Append(",");
                }
                vectorX[countIdx] = vector[idx];
                vectorY[countIdx] = vector[idx + 1];
                countIdx++;
            }
            string str = builder.ToString();

            return vector;
        }

        private double GetDelta(double coord1, double coord2)
        {
            return Math.Abs(coord1 - coord2);
        }

        private int FindNearestIdx(double x, double y, List<MotionEventCompact> eventsList)
        {
            UtilsCalc utilsCalc = new UtilsCalc();
            int idxNearestEvent = 0;
            double tempDeltaX = GetDelta(eventsList[0].X, x);
            double tempDeltaY = GetDelta(eventsList[0].Y, y);

            double currDelta = utilsCalc.CalcPitagoras(tempDeltaX, tempDeltaY);
            double tempDelta;

            for (int idxEvent = 1; idxEvent < eventsList.Count; idxEvent++)
            {
                tempDeltaX = GetDelta(eventsList[idxEvent].X, x);
                tempDeltaY = GetDelta(eventsList[idxEvent].Y, y);

                tempDelta = utilsCalc.CalcPitagoras(tempDeltaX, tempDeltaY);

                if (tempDelta < currDelta)
                {
                    idxNearestEvent = idxEvent;
                    currDelta = tempDelta;
                }
            }

            return idxNearestEvent;
        }
    }
}