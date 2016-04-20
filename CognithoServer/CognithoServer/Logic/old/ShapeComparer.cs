using CognithoServer.Models;
using CognithoServer.Utils;
using GeoAPI.Geometries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CognithoServer.Logic.old
{

    public class ScreenSection
    {
        public ScreenSection(double min, double max)
        {
            Min = Math.Floor(min);
            Max = Math.Ceiling(max);
        }

        public double Min { get; set; }
        public double Max { get; set; }
    }

    public class ShapeComparer
    {
        public ShapeComparer(Stroke stroke1, Stroke stroke2)
        {
            //UtilsCalc utils = new UtilsCalc();
            //double[] vector1 = utils.ConvertToVector(stroke1.ListEvents, stroke1.Length);
            //double[] vector2 = utils.ConvertToVector(stroke2.ListEvents, stroke2.Length);

            //double[] vector1X = new double[vector1.Length / 2];
            //double[] vector1Y = new double[vector1.Length / 2];

            //double[] vector2X = new double[vector2.Length / 2];
            //double[] vector2Y = new double[vector2.Length / 2];

            //int idxList = 0;

            //double min1X = double.MaxValue;
            //double min2X = double.MaxValue;
            //double min1Y = double.MaxValue;
            //double min2Y = double.MaxValue;

            //double minX, minY;

            //double max1X = double.MinValue;
            //double max1Y = double.MinValue;
            //double max2X = double.MinValue;
            //double max2Y = double.MinValue;

            //double maxX = double.MinValue;
            //double maxY = double.MinValue;

            //Vector2D v1 = new Vector2D();
            //Vector2D v2 = new Vector2D();

            //IPoint[] points1 = new Point[vector1.Length/2];
            //IPoint[] points2 = new Point[vector1.Length/2];
            //int count = 0;

            //for (int idx = 0; idx < vector1.Length; idx += 2)
            //{
            //    points1[count] = new Point(vector1[idx], vector1[idx + 1]);
            //    points2[count] = new Point(vector2[idx], vector2[idx + 1]);
            //    count++;
            //    v1.Add(new Vector2D(vector1[idx], vector1[idx + 1]));
            //    v2.Add(new Vector2D(vector2[idx], vector2[idx + 1]));

            //    vector1X[idxList] = vector1[idx];
            //    vector1Y[idxList] = vector1[idx + 1];

            //    vector2X[idxList] = vector2[idx];
            //    vector2Y[idxList] = vector2[idx + 1];

            //    if(vector1[idx] < min1X)
            //    {
            //        min1X = vector1[idx];
            //    }

            //    if (vector2[idx] < min2X)
            //    {
            //        min2X = vector2[idx];
            //    }

            //    if (vector1[idx + 1] < min1Y)
            //    {
            //        min1Y = vector1[idx + 1];
            //    }

            //    if (vector2[idx + 1] < min2Y)
            //    {
            //        min2Y = vector2[idx + 1];
            //    }

            //    if (vector1[idx] > max1X)
            //    {
            //        max1X = vector1[idx];
            //    }

            //    if (vector2[idx] > max2X)
            //    {
            //        max2X = vector2[idx];
            //    }

            //    if (vector1[idx + 1] > max1Y)
            //    {
            //        max1Y = vector1[idx + 1];
            //    }

            //    if (vector2[idx + 1] > max2Y)
            //    {
            //        max2Y = vector2[idx + 1];
            //    }                

            //    idxList++;
            //}

            //if(min1X < min2X)
            //{
            //    minX = min1X;
            //}
            //else
            //{
            //    minX = min2X;
            //}

            //if (min1Y < min2Y)
            //{
            //    minY = min1Y;
            //}
            //else
            //{
            //    minY = min2Y;
            //}

            //if (max1X > max2X)
            //{
            //    maxX = max1X;
            //}
            //else
            //{
            //    maxX = max2X;
            //}

            //if (max1Y > max2Y)
            //{
            //    maxY = max1Y;
            //}
            //else
            //{
            //    maxY = max2Y;
            //}

            //MultiPoint mp1 = new MultiPoint(points1);
            //MultiPoint mp2 = new MultiPoint(points2);

            //maxX -= minX;
            //maxY -= minY;

            //for (int idx = 0; idx < vector1X.Length; idx++)
            //{
            //    vector1X[idx] = vector1X[idx] - min1X;
            //    vector2X[idx] = vector2X[idx] - min2X;

            //    vector1Y[idx] = vector1Y[idx] - min1Y;
            //    vector2Y[idx] = vector2Y[idx] - min2Y;
            //}

            
            //Polygon(vector1X, vector1Y, vector2X, vector2Y);

            //double score2 = GetScoreForNumSections(2, 0, vector1X, vector1Y, vector2X, vector2Y, max1X, max1Y, max2X, max2Y);
            //double score4 = GetScoreForNumSections(4, 0, vector1X, vector1Y, vector2X, vector2Y, max1X, max1Y, max2X, max2Y);
            //double score8 = GetScoreForNumSections(8, 1, vector1X, vector1Y, vector2X, vector2Y, max1X, max1Y, max2X, max2Y);
            //double score16 = GetScoreForNumSections(16, 4, vector1X, vector1Y, vector2X, vector2Y, max1X, max1Y, max2X, max2Y);
            //double score32 = GetScoreForNumSections(32, 6, vector1X, vector1Y, vector2X, vector2Y, max1X, max1Y, max2X, max2Y);

            //double scoreWithSize2 = GetScoreForNumSectionsWithSize(2, 0, vector1X, vector1Y, vector2X, vector2Y, maxX, maxY);
            //double scoreWithSize4 = GetScoreForNumSectionsWithSize(4, 0, vector1X, vector1Y, vector2X, vector2Y, maxX, maxY);
            //double scoreWithSize8 = GetScoreForNumSectionsWithSize(8, 1, vector1X, vector1Y, vector2X, vector2Y, maxX, maxY);
            //double scoreWithSize16 = GetScoreForNumSectionsWithSize(16, 3, vector1X, vector1Y, vector2X, vector2Y, maxX, maxY);
            //double scoreWithSize32 = GetScoreForNumSectionsWithSize(32, 4, vector1X, vector1Y, vector2X, vector2Y, maxX, maxY);

            //double maxDistance;
            //double distance = GetDistance(vector1X, vector1Y, vector2X, vector2Y, out maxDistance);
            //double avgDistance = distance / vector1X.Length;

            //double maxDistanceScore = maxDistance / stroke1.Length;
            //double distanceScore = distance / stroke1.Length;
            //double avgDistanceScore = avgDistance / stroke1.Length;

            string s = "asd";
        }

        private string CoordsToLine(int idx, double[] vectorX, double[] vectorY)
        {
            int idxNext = idx + 1;
            string line = string.Format("LINESTRING ({0} {1}, {2} {3})", (int)vectorX[idx], (int)vectorY[idx], (int)vectorX[idxNext], (int)vectorY[idxNext]);

            return line;
        }

        private void Polygon(double[] vector1X, double[] vector1Y, double[] vector2X, double[] vector2Y)
        {
            //IGeometryFactory geometryFactory = new GeometryFactory();

            //double[] vX = new double[5];
            //double[] vY = new double[5];

            //vX[0] = 0;
            //vY[0] = 0;

            //vX[1] = 8;
            //vY[1] = 3;

            //vX[2] = 8;
            //vY[2] = 3;

            //vX[3] = 5;
            //vY[3] = 5;

            //vX[4] = 0;
            //vY[4] = 0;

            //List<IGeometry> shape1 = ShapeToLines(vX, vY);
            //List<IGeometry> shape2 = ShapeToLines(vector2X, vector2Y);

            //Polygonizer polygonizer = new Polygonizer();
            //polygonizer.Add(shape1);

            //Polygonizer polygonizer2 = new Polygonizer();
            //polygonizer2.Add(shape2);

            //ICollection<IGeometry> polys = polygonizer.GetPolygons();
            //ICollection<IGeometry> polys2 = polygonizer2.GetPolygons();

            //List<IGeometry> myList = new List<IGeometry>();

            //foreach (object obj in polys)
            //{
            //    myList.Add((IGeometry)obj);
            //}

            //foreach (object obj in polys2)
            //{
            //    myList.Add((IGeometry)obj);
            //}

            //if (myList.Count == 2)
            //{
            //    int result = myList[0].CompareTo(myList[1]);

            //    int result2 = result;
            //}
        }


        private double CalcPitagoras(double x, double y)
        {
            double value = x * x + y * y;
            value = Math.Sqrt(value);

            return value;
        }

        private double GetDistance(double[] vector1X, double[] vector1Y, double[] vector2X, double[] vector2Y, out double maxDistance)
        {
            double totalDistance = 0;
            maxDistance = 0;

            double tempDistanceX, tempDistanceY, tempDistance;

            for (int idx = 0; idx < vector1X.Length; idx++)
            {
                tempDistanceX = Math.Abs(vector1X[idx] - vector2X[idx]);
                tempDistanceY = Math.Abs(vector1Y[idx] - vector2Y[idx]);

                tempDistance = CalcPitagoras(tempDistanceX, tempDistanceY);
                if(tempDistance > maxDistance)
                {
                    maxDistance = tempDistance;
                }
                totalDistance += tempDistance;
            }

            return totalDistance;
        }

        private double GetScoreForNumSections(double numSections, double error, double[] vector1X, double[] vector1Y, double[] vector2X, double[] vector2Y, double max1X, double max1Y, double max2X, double max2Y)
        {
            List<ScreenSection> list1X = CreateScreenSections(max1X, numSections);
            List<ScreenSection> list1Y = CreateScreenSections(max1Y, numSections);

            List<ScreenSection> list2X = CreateScreenSections(max2X, numSections);
            List<ScreenSection> list2Y = CreateScreenSections(max2Y, numSections);

            int sectionNum1X, sectionNum1Y, sectionNum2X, sectionNum2Y;
            double misses = 0;

            int[] section1X = new int[vector1X.Length];
            int[] section2X = new int[vector1X.Length];
            int[] section1Y = new int[vector1X.Length];
            int[] section2Y = new int[vector1X.Length];

            int diffX, diffY;

            for (int idx = 0; idx < vector1X.Length; idx++)
            {
                sectionNum1X = GetSectionNumber(vector1X[idx], list1X);
                sectionNum2X = GetSectionNumber(vector2X[idx], list2X);

                sectionNum1Y = GetSectionNumber(vector1Y[idx], list1Y);
                sectionNum2Y = GetSectionNumber(vector2Y[idx], list2Y);

                section1X[idx] = sectionNum1X;
                section2X[idx] = sectionNum2X;

                section1Y[idx] = sectionNum1Y;
                section2Y[idx] = sectionNum2Y;

                diffX = Math.Abs(sectionNum1X - sectionNum2X);
                diffY = Math.Abs(sectionNum1Y - sectionNum2Y);

                if (diffX + diffY > error)
                {
                    misses++;
                }
            }

            double score = (vector1X.Length - misses) / vector1X.Length;
            return score;
        }

        private double GetScoreForNumSectionsWithSize(double numSections, double error, double[] vector1X, double[] vector1Y, double[] vector2X, double[] vector2Y, double maxX, double maxY)
        {
            List<ScreenSection> listX = CreateScreenSections(maxX, numSections);
            List<ScreenSection> listY = CreateScreenSections(maxY, numSections);

            int sectionNum1X, sectionNum1Y, sectionNum2X, sectionNum2Y;
            double misses = 0;

            int[] section1X = new int[vector1X.Length];
            int[] section2X = new int[vector1X.Length];
            int[] section1Y = new int[vector1X.Length];
            int[] section2Y = new int[vector1X.Length];

            int diffX, diffY;

            for (int idx = 0; idx < vector1X.Length; idx++)
            {
                sectionNum1X = GetSectionNumber(vector1X[idx], listX);
                sectionNum2X = GetSectionNumber(vector2X[idx], listX);

                sectionNum1Y = GetSectionNumber(vector1Y[idx], listY);
                sectionNum2Y = GetSectionNumber(vector2Y[idx], listY);

                section1X[idx] = sectionNum1X;
                section2X[idx] = sectionNum2X;

                section1Y[idx] = sectionNum1Y;
                section2Y[idx] = sectionNum2Y;

                diffX = Math.Abs(sectionNum1X - sectionNum2X);
                diffY = Math.Abs(sectionNum1Y - sectionNum2Y);

                if (diffX + diffY > error)
                {
                    misses++;
                }
            }

            double score = (vector1X.Length - misses) / vector1X.Length;
            return score;
        }

        private int GetSectionNumber(double value, List<ScreenSection> list)
        {
            int section = -1;

            for (int idx = 0; idx < list.Count; idx++)
            {
                if(value >= list[idx].Min && value <= list[idx].Max)
                {
                    section = idx;
                    break;
                }
            }

            return section;
        }

        private List<ScreenSection> CreateScreenSections(double maxValue, double numSections)
        {
            List<ScreenSection> list = new List<ScreenSection>();
            ScreenSection tempSection;

            double hop = maxValue / numSections;
            double currentValue = 0;

            for(int idx = 0; idx < numSections; idx++)
            {
                tempSection = new ScreenSection(currentValue, currentValue + hop);
                list.Add(tempSection);
                currentValue += hop;
            }

            return list;
        }
    }
}