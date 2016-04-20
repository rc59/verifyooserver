using CognithoServer.Logic.Statistics.Norms;
using CognithoServer.Models;
using CognithoServer.Utils;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace CognithoServer.Controllers
{
    public class DistributionsController : ApiController
    {
        Dictionary<string, INormObj> mListNormObjectsStrokes;
        Dictionary<string, INormObj> mListNormObjectsGestures;

        [HttpGet]
        public List<DistributionObj> Get()
        {
            new UtilsLicense().CheckAppKey();
            MongoCollection<InstructionDistributions> mongoListDists = new UtilsDB().GetCollectionDistributions();
            List<InstructionDistributions> listDists = new UtilsDB().ConvertCollectionToList<InstructionDistributions>(mongoListDists);

            mListNormObjectsStrokes = new Dictionary<string, INormObj>();
            mListNormObjectsGestures = new Dictionary<string, INormObj>();

            #region Init Strokes

            AddNormObjectStroke(ConstsNorms.Stroke.MAX_PRESSURE, 0.7, 0.2);
            AddNormObjectStroke(ConstsNorms.Stroke.AVG_PRESSURE, 0.5, 0.2);
            AddNormObjectStroke(ConstsNorms.Stroke.MIN_PRESSURE, 0.3, 0.2);

            AddNormObjectStroke(ConstsNorms.Stroke.MAX_SURFACE, 0.6, 0.2);
            AddNormObjectStroke(ConstsNorms.Stroke.AVG_SURFACE, 0.5, 0.2);
            AddNormObjectStroke(ConstsNorms.Stroke.MIN_SURFACE, 0.3, 0.2);

            AddNormObjectStroke(ConstsNorms.Stroke.LENGTH, 2000, 500);
            AddNormObjectStroke(ConstsNorms.Stroke.NUM_EVENTS, 50, 15);
            AddNormObjectStroke(ConstsNorms.Stroke.TIME_INTERVAL, 420, 50);

            AddNormObjectStroke(ConstsNorms.Stroke.MAX_VELOCITY_X, 2000, 500);
            AddNormObjectStroke(ConstsNorms.Stroke.MAX_VELOCITY_Y, 2000, 500);
            AddNormObjectStroke(ConstsNorms.Stroke.MAX_VELOCITY, 3000, 1000);

            AddNormObjectStroke(ConstsNorms.Stroke.AVG_VELOCITY_X, 800, 150);
            AddNormObjectStroke(ConstsNorms.Stroke.AVG_VELOCITY_Y, 800, 150);
            AddNormObjectStroke(ConstsNorms.Stroke.AVG_VELOCITY, 1000, 250);

            AddNormObjectStroke(ConstsNorms.Stroke.MAX_ACCELERATION_X, 600, 150);
            AddNormObjectStroke(ConstsNorms.Stroke.MAX_ACCELERATION_Y, 600, 150);
            AddNormObjectStroke(ConstsNorms.Stroke.MAX_ACCELERATION, 800, 200);

            AddNormObjectStroke(ConstsNorms.Stroke.AVG_ACCELERATION_X, 150, 30);
            AddNormObjectStroke(ConstsNorms.Stroke.AVG_ACCELERATION_Y, 150, 30);
            AddNormObjectStroke(ConstsNorms.Stroke.AVG_ACCELERATION, 250, 60);

            //AddNormObjectStroke(ConstsNorms.Stroke.AVG_TREMOR_X, 0.5, 0.1);
            //AddNormObjectStroke(ConstsNorms.Stroke.AVG_TREMOR_Y, 0.5, 0.1);
            //AddNormObjectStroke(ConstsNorms.Stroke.AVG_TREMOR_Z, 0.5, 0.1);

            AddNormObjectStroke(ConstsNorms.Stroke.MIN_ANGLE_CHANGE, 150, 30);
            AddNormObjectStroke(ConstsNorms.Stroke.MAX_ANGLE_CHANGE, 250, 60);
            AddNormObjectStroke(ConstsNorms.Stroke.AVG_ANGLE_CHANGE, 150, 30);

            AddNormObjectStroke(ConstsNorms.Stroke.BOUNDING_BOX, 200000, 40000);
            AddNormObjectStroke(ConstsNorms.Stroke.WIDTH, 400, 100);
            AddNormObjectStroke(ConstsNorms.Stroke.HEIGHT, 500, 150);

            AddNormObjectStroke(ConstsNorms.Stroke.HOUGH_LINES, 30, 5);
            AddNormObjectStroke(ConstsNorms.Stroke.HOUGH_CIRCLES, 3, 3);
            AddNormObjectStroke(ConstsNorms.Stroke.CONTOUR_TREE, 15, 3);

            #endregion

            #region Init Gestures

            AddNormObjectGesture(ConstsNorms.Gesture.STROKES_PROPORTIONS_AVG, 1.2, 0.3);
            AddNormObjectGesture(ConstsNorms.Gesture.STROKES_PROPORTIONS_MAX, 2, 0.5);

            AddNormObjectGesture(ConstsNorms.Gesture.STROKES_DISTANCE_AVG, 500, 100);
            AddNormObjectGesture(ConstsNorms.Gesture.STROKES_DISTANCE_MAX, 600, 150);

            AddNormObjectGesture(ConstsNorms.Gesture.STROKES_PAUSE_AVG, 200, 50);
            AddNormObjectGesture(ConstsNorms.Gesture.STROKES_PAUSE_MAX, 400, 80);

            AddNormObjectGesture(ConstsNorms.Gesture.TOTAL_TIME, 700, 200);
            AddNormObjectGesture(ConstsNorms.Gesture.START_END_DISTANCE, 200, 50);

            AddNormObjectGesture(ConstsNorms.Gesture.BOUNDING_BOX, 400000, 40000);
            AddNormObjectGesture(ConstsNorms.Gesture.WIDTH, 1000, 100);
            AddNormObjectGesture(ConstsNorms.Gesture.HEIGHT, 1000, 150);

            AddNormObjectGesture(ConstsNorms.Gesture.LENGTH, 4000, 500);
            AddNormObjectGesture(ConstsNorms.Gesture.NUM_EVENTS, 100, 15);
            AddNormObjectGesture(ConstsNorms.Gesture.TIME_INTERVAL, 1500, 50);

            #endregion

            DistributionObj obj = new DistributionObj();
            
            INormObj temp;

            InstructionDistributions instructionDists;

            for (int idx = 0; idx < 8; idx++)
            {
                instructionDists = new InstructionDistributions(idx);

                foreach (string key in mListNormObjectsStrokes.Keys)
                {
                    obj = new DistributionObj();
                    temp = mListNormObjectsStrokes[key];
                    obj.Name = temp.GetName();
                    obj.Average = temp.GetAverage();
                    obj.StandardDeviation = temp.GetStandardDeviation();
                    obj.__v = 0;
                    instructionDists.StrokeDistributions.Add(obj);
                }

                foreach (string key in mListNormObjectsGestures.Keys)
                {
                    obj = new DistributionObj();
                    temp = mListNormObjectsGestures[key];
                    obj.Name = temp.GetName();
                    obj.Average = temp.GetAverage();
                    obj.StandardDeviation = temp.GetStandardDeviation();
                    obj.__v = 0;
                    instructionDists.GestureDistributions.Add(obj);
                }

                mongoListDists.Insert(instructionDists);
            }

            return new List<DistributionObj>();
        }

        protected void AddNormObjectStroke(string name, double average, double standardDeviation)
        {
            mListNormObjectsStrokes.Add(name, new NormObj(name, average, standardDeviation));
        }

        protected void AddNormObjectGesture(string name, double average, double standardDeviation)
        {
            mListNormObjectsGestures.Add(name, new NormObj(name, average, standardDeviation));
        }
    }
}
