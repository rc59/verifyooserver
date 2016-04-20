using CognithoServer.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;

namespace CognithoServer.Utils
{
    public class UtilsCV
    {
        //private Stroke mStroke;
        //protected double mMaxX, mMaxY;
        //protected UtilsCalc mUtilsCalc;               

        //public double HoughCircles { get; set; }
        //public double HoughLines { get; set; }
        //public double ContourTree { get; set; }

        //public UtilsCV(Stroke stroke, double maxX, double maxY)
        //{
        //    mStroke = stroke;

        //    mMaxX = maxX;
        //    mMaxY = maxY;

        //    mUtilsCalc = new UtilsCalc();
        //}

        //public void CalcCVShapeFeatures()
        //{
        //    VectorOfPoint cvShape = EventsListToCVShape(mStroke.ListEvents);

        //    int rows = (int) mMaxX;
        //    int colums = (int)mMaxY;

        //    UMat uimage = EventsListToUMat(mStroke.ListEvents, rows, colums);

        //    int[,] contourTree = GetContourTree(uimage);

        //    CircleF[] circles = GetHoughCircles(uimage);

        //    LineSegment2D[] linesp = GetHoughLines(uimage);

        //    HoughCircles = circles.Length;
        //    HoughLines = linesp.Length;
        //    ContourTree = contourTree.Length;
        //}

        //private UMat EventsListToUMat(List<MotionEventCompact> listEvents, int rows, int cols)
        //{
        //    Mat mat = new Mat(rows, cols, DepthType.Cv8U, 3);

        //    Image<Bgr, byte> img = mat.ToImage<Bgr, byte>();
        //    Point tempPoint1, tempPoint2;

        //    for (int idx = 1; idx < listEvents.Count; idx++)
        //    {
        //        tempPoint1 = new Point((int)listEvents[idx - 1].X, (int)listEvents[idx - 1].Y);
        //        tempPoint2 = new Point((int)listEvents[idx].X, (int)listEvents[idx].Y);
        //        img.Draw(new LineSegment2D(tempPoint1, tempPoint2), new Bgr(Color.Red), 3);
        //    }

        //    UMat uimage = new UMat();
        //    CvInvoke.CvtColor(img, uimage, ColorConversion.Bgr2Gray);

        //    UMat pyrDown = new UMat();
        //    CvInvoke.PyrDown(uimage, pyrDown);
        //    CvInvoke.PyrUp(pyrDown, uimage);

        //    return uimage;
        //}

        //private CircleF[] GetHoughCircles(UMat uimage)
        //{
        //    double cannyThreshold = 180.0;
        //    double circleAccumulatorThreshold = 120;
        //    CircleF[] circles = CvInvoke.HoughCircles(uimage, HoughType.Gradient, 2.0, 20.0, cannyThreshold, circleAccumulatorThreshold, 5);

        //    return circles;
        //}

        //private LineSegment2D[] GetHoughLines(UMat uimage)
        //{
        //    double cannyThreshold = 180.0;
        //    double cannyThresholdLinking = 120.0;
        //    UMat cannyEdges = new UMat();
        //    CvInvoke.Canny(uimage, cannyEdges, cannyThreshold, cannyThresholdLinking);

        //    LineSegment2D[] lines = CvInvoke.HoughLinesP(cannyEdges, 1, Math.PI / 45.0, 30, 50, 10);

        //    return lines;
        //}

        //private int[,] GetContourTree(UMat uimage)
        //{
        //    Mat canny = new Mat();
        //    VectorOfVectorOfPoint contours = new VectorOfVectorOfPoint();

        //    CvInvoke.Canny(uimage, canny, 100, 50, 3, false);
        //    int[,] hierachy = CvInvoke.FindContourTree(canny, contours, ChainApproxMethod.ChainApproxSimple);

        //    return hierachy;
        //}
        
        //private VectorOfPoint EventsListToCVShape(List<MotionEventCompact> listEvents)
        //{
        //    VectorOfPoint cvShape = new VectorOfPoint();
        //    Point[] listPoints = new Point[listEvents.Count];

        //    for (int idx = 0; idx < listEvents.Count; idx++)
        //    {
        //        listPoints[idx] = new Point((int)listEvents[idx].X, (int)listEvents[idx].Y);
        //    }

        //    cvShape.Push(listPoints);

        //    return cvShape;
        //}        
    }
}