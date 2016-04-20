using CognithoServer.Logic.Statistics.Comparison;
using CognithoServer.Logic.Statistics.Comparison.Comparers;
using CognithoServer.Logic.Statistics.Comparison.Objects;
using CognithoServer.Logic.Statistics.Norms;
using CognithoServer.Models;
using CognithoServer.Objects;
using CognithoServer.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace CognithoServer.Logic
{
    public class GestureComparer
    {
        protected string _msg;

        public int InstructionIdx;
        public double NumGestureMismatch;
        public double mGestureZscore;
        public GestureResultObj GestureResult;

        protected GestureObj mGestureVerify, mGestureStored;
        protected UtilsCalc mUtilCalc;
        protected StrokeComparer mStrokeComparer;
        protected List<StrokeExtended> mlistStrokeExtended1, mlistStrokeExtended2;        
        protected List<CompResult> mListCompResult;
        protected bool mIsCalcGestureZscore;

        protected StringBuilder mStringBuilder;

        public void Init(GestureObj gestureVerify, GestureObj gestureStored)
        {
            mStringBuilder = new StringBuilder();
            mIsCalcGestureZscore = false;

            mListCompResult = new List<CompResult>();

            mGestureVerify = gestureVerify;
            mGestureStored = gestureStored;

            mlistStrokeExtended1 = new List<StrokeExtended>();
            mlistStrokeExtended2 = new List<StrokeExtended>();

            GestureResult = new GestureResultObj();

            mUtilCalc = new UtilsCalc();

            NumGestureMismatch = 0;

            RunComparison();
        }

        private void PerformGestureCalc()
        {
            //if(mlistStrokeExtended1.Count > 1)
            //{
            //    CalcBoundingBoxProportions();    
            //}

            int numOfZParams = 0;

            double totalZScore = 0;
            double totalShapeScore = 0;

            if (mIsCalcGestureZscore)
            {
                totalZScore = mGestureZscore;
                numOfZParams++;
            }

            bool isShapeMatch = true;

            for (int idx = 0; idx < mListCompResult.Count; idx++)
            {
                totalZScore += mListCompResult[idx].ZScore;
                totalShapeScore += mListCompResult[idx].ShapeScore;

                if(mListCompResult[idx].ShapeScore < (Consts.SHAPE_REC_THREASHOLD - 0.5))
                {
                    isShapeMatch = false;
                }

                numOfZParams++;
            }

            totalZScore = totalZScore / numOfZParams;
            totalShapeScore = totalShapeScore / mListCompResult.Count;

            if (totalZScore > Consts.Z_SCORE_THRESHOLD || totalShapeScore < Consts.SHAPE_REC_THREASHOLD || !isShapeMatch)
            {
                NumGestureMismatch++;
            }
        }

        private void CalcBoundingBoxProportions()
        {
            double[] listBoundingBox1 = new double[mlistStrokeExtended1.Count];
            double[] listBoundingBox2 = new double[mlistStrokeExtended1.Count];

            for(int idxStroke = 0; idxStroke < listBoundingBox1.Length; idxStroke++)
            {
                listBoundingBox1[idxStroke] = mlistStrokeExtended1[idxStroke].BoundingBox;
                listBoundingBox2[idxStroke] = mlistStrokeExtended2[idxStroke].BoundingBox;
            }

            double[] listBoundingBoxDiff1 = new double[mlistStrokeExtended1.Count - 1];
            double[] listBoundingBoxDiff2 = new double[mlistStrokeExtended1.Count - 1];

            for (int idxStroke = 1; idxStroke < listBoundingBox1.Length; idxStroke++)
            {
                listBoundingBoxDiff1[idxStroke - 1] = mUtilCalc.CalcPercentage(listBoundingBox1[idxStroke], listBoundingBox1[idxStroke - 1]);
                listBoundingBoxDiff2[idxStroke - 1] = mUtilCalc.CalcPercentage(listBoundingBox2[idxStroke], listBoundingBox2[idxStroke - 1]);
            }
        }

        private MotionEventCompact GetLastEvent(Stroke stroke)
        {
            MotionEventCompact tempEvent = new MotionEventCompact();
            if (stroke.ListEvents.Count > 0)
            {
                int idxLast = stroke.ListEvents.Count - 1;
                tempEvent = stroke.ListEvents[idxLast];
            }

            return tempEvent;
        }

        public void RunComparison()
        {
            
            _msg = "pointb11";
            MotionEventCompact tempEventVerify, tempEventStored;

            if (mGestureVerify.Strokes.Count != mGestureStored.Strokes.Count)
            {
                NumGestureMismatch++;
            }
            else
            {
                _msg = "pointb12";
                List<StrokeCompParamContainer> listCompParamContainer1 = new List<StrokeCompParamContainer>();
                List<StrokeCompParamContainer> listCompParamContainer2 = new List<StrokeCompParamContainer>();

                CompResult tempCompResult;

                _msg = "pointb13";
                for (int idxStroke = 0; idxStroke < mGestureVerify.Strokes.Count; idxStroke++)
                {
                    mStrokeComparer = new StrokeComparer();

                    if (idxStroke > 0)
                    {
                        tempEventVerify = GetLastEvent(mGestureVerify.Strokes[idxStroke - 1]);
                        tempEventStored = GetLastEvent(mGestureStored.Strokes[idxStroke - 1]);

                        mStrokeComparer.SetPreviousCoords(tempEventVerify.X, tempEventVerify.Y, tempEventStored.X, tempEventStored.Y);
                        mStrokeComparer.SetPreviousDownTime(tempEventVerify.EventTime, tempEventStored.EventTime);
                    }

                    _msg = "pointb14";
                    int count1 = mGestureVerify.Strokes.Count;
                    int count2 = mGestureStored.Strokes.Count;
                    _msg = count1.ToString() + " - " + count2.ToString();


                    Stroke temp1 = mGestureVerify.Strokes[idxStroke];
                    _msg = "pointb15";
                    Stroke temp2 = mGestureStored.Strokes[idxStroke];
                    _msg = "pointb16";

                    mStrokeComparer.Init(temp1, temp2);
                    mStrokeComparer.InstructionIdx = InstructionIdx;
                    mlistStrokeExtended1.Add(mStrokeComparer.StrokeExtended1);
                    mlistStrokeExtended2.Add(mStrokeComparer.StrokeExtended2);

                    GestureResult.StrokeResults.Add(mStrokeComparer.StrokeResult);

                    if (mStrokeComparer.CompParamsCtr1 != null)
                    {
                        listCompParamContainer1.Add(mStrokeComparer.CompParamsCtr1);
                    }

                    if (mStrokeComparer.CompParamsCtr2 != null)
                    {
                        listCompParamContainer2.Add(mStrokeComparer.CompParamsCtr2);
                    }

                    tempCompResult = new CompResult(mStrokeComparer.StrokeResult.ShapeScore, mStrokeComparer.StrokeResult.ZScore);
                    mListCompResult.Add(tempCompResult);

                    mStringBuilder.Append(string.Format("[Stroke {0}:{1}", idxStroke.ToString(), mStrokeComparer.StrParameters));
                }

                if (listCompParamContainer1.Count > 1)
                {
                    mIsCalcGestureZscore = true;
                    CalcZScore(listCompParamContainer1, listCompParamContainer2);
                }
                PerformGestureCalc();
            }                      
        }

        protected void CalcZScore(List<StrokeCompParamContainer> listCompParamContainer1, List<StrokeCompParamContainer> listCompParamContainer2)
        {
            GestureCompParamContainer gestureCompParamContainer1 = new GestureCompParamContainer(listCompParamContainer1, mGestureVerify.Strokes);
            GestureCompParamContainer gestureCompParamContainer2 = new GestureCompParamContainer(listCompParamContainer2, mGestureStored.Strokes);

            CompParamMgr mgr = new CompParamMgr(gestureCompParamContainer1, gestureCompParamContainer2);
            mgr.RunComparison();
            mGestureZscore = mgr.GetAvgZDiffScore();

            mStringBuilder.Append(string.Format("[Gesture Level {0}, ", mgr.StrParameters));
        }

        public string ToString()
        {
            return mStringBuilder.ToString();
        }

    }
}