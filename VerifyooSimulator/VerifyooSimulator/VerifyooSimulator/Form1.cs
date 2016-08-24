﻿using Data.Comparison.Interfaces;
using Data.UserProfile.Extended;
using java.util;
using Logic.Comparison;
using Logic.Comparison.Stats.Norms;
using MongoDB.Driver;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using VerifyooSimulator.DAL;
using VerifyooSimulator.Logic;
using VerifyooSimulator.Models;
using VerifyooSimulator.Models.NormsObj;

namespace VerifyooSimulator
{
    public partial class Form1 : Form
    {
        int GESTURE_NUM_PARAMS = 15;

        int mIdxComparison;

        DateTime mStartTime;
        DateTime mEndTime;

        StreamWriter mStreamWriterGestures = null;
        StreamWriter mStreamWriterStrokes = null;
        StreamWriter mStreamWriterLog = null;

        private MongoCollection<ModelTemplate> mListMongo;
        private bool mIsFinished;

        string mSimulationType;

        public Form1()
        {
            InitializeComponent();
        }

        private void btnGo_Click(object sender, EventArgs e)
        {
            mSimulationType = "Auth";
            Task task = Task.Run((Action)RunSimulatorValidUsers);
        }

        protected void RunSimulatorValidUsers()
        {
            mStartTime = DateTime.Now;

            Init();
            IEnumerable<ModelTemplate> modelTemplates;

            TemplateExtended tempTemplate = null;

            List<GestureExtended> gesturesTestSample;

            mListMongo = UtilsDB.GetTemplates();
            mIsFinished = false;

            mIdxComparison = 1;
            int currentRecord = 0;
            int totalNumbRecords = (int)mListMongo.Count();
            int limit = 50;
            int skip = 0;            
            
            try
            {
                mStreamWriterGestures = InitStreamWriterResultGestures();
                mStreamWriterStrokes = InitStreamWriterResultStrokes();
                mStreamWriterLog = InitStreamWriterLog();

                InitGestureHeader(mStreamWriterGestures);
                InitStrokesHeader(mStreamWriterStrokes);

                UpdateProgress(totalNumbRecords, currentRecord);
                while (!mIsFinished)
                {
                    modelTemplates = mListMongo.FindAll().SetLimit(limit).SetSkip(skip);

                    foreach (ModelTemplate template in modelTemplates)
                    {
                        tempTemplate = UtilsTemplateConverter.ConvertTemplate(template, out gesturesTestSample);

                        for (int idxTestGesture = 0; idxTestGesture < gesturesTestSample.Count; idxTestGesture++)
                        {
                            CompareGesturesToStrings(tempTemplate, tempTemplate, gesturesTestSample[idxTestGesture]);

                            mIdxComparison++;
                        }

                        currentRecord++;
                        UpdateProgress(totalNumbRecords, currentRecord);
                    }

                    mStreamWriterGestures.Flush();
                    mStreamWriterStrokes.Flush();
                    mStreamWriterLog.Flush();

                    skip += limit;

                    if (totalNumbRecords <= skip)
                    {
                        mIsFinished = true;
                        mStreamWriterGestures.Close();
                        mStreamWriterStrokes.Close();
                        mStreamWriterLog.Close();

                        mEndTime = DateTime.Now;
                        UpdateProgress(totalNumbRecords, currentRecord, true);
                    }
                }
            }
            catch (Exception exc)
            {
                MessageBox.Show(string.Format("Error: {0}", exc.Message));
            }            
        }

        protected void RunSimulatorHackAttempts()
        {
            mStartTime = DateTime.Now;

            Init();
            IEnumerable<ModelTemplate> modelTemplates;
            IEnumerable<ModelTemplate> modelTemplatesHacks;

            TemplateExtended tempTemplate = null;
            TemplateExtended tempTemplateHack = null;

            List<GestureExtended> gesturesTestSample;
            List<GestureExtended> gesturesTestSampleHackAttempts;

            mListMongo = UtilsDB.GetTemplates();
            mIsFinished = false;

            mIdxComparison = 1;
            int currentRecord = 0;
            int totalNumbRecords = (int)mListMongo.Count();
            totalNumbRecords = totalNumbRecords * totalNumbRecords;
            int limit = 50;
            int skip = 0;

            int limitHackAttempt = 50;
            int skipHackAttempt = 0;

            bool isFinishedHackAttempts = false;

            try
            {
                mStreamWriterGestures = InitStreamWriterResultGestures();
                mStreamWriterStrokes = InitStreamWriterResultStrokes();
                mStreamWriterLog = InitStreamWriterLog();

                InitGestureHeader(mStreamWriterGestures);
                InitStrokesHeader(mStreamWriterStrokes);

                UpdateProgress(totalNumbRecords, currentRecord);
                while (!mIsFinished)
                {
                    modelTemplates = mListMongo.FindAll().SetLimit(limit).SetSkip(skip);                    

                    foreach (ModelTemplate template in modelTemplates)
                    {
                        tempTemplate = UtilsTemplateConverter.ConvertTemplate(template, out gesturesTestSample);

                        limitHackAttempt = 50;
                        skipHackAttempt = 0;
                        isFinishedHackAttempts = false;
                        while (!isFinishedHackAttempts)
                        {
                            modelTemplatesHacks = mListMongo.FindAll().SetLimit(limitHackAttempt).SetSkip(skipHackAttempt);

                            foreach (ModelTemplate templateHack in modelTemplatesHacks)
                            {
                                tempTemplateHack = UtilsTemplateConverter.ConvertTemplate(templateHack, out gesturesTestSampleHackAttempts);

                                if(tempTemplateHack.Id != tempTemplate.Id)
                                {
                                    for (int idxTestGesture = 0; idxTestGesture < gesturesTestSampleHackAttempts.Count; idxTestGesture++)
                                    {
                                        CompareGesturesToStrings(tempTemplate, tempTemplateHack, gesturesTestSampleHackAttempts[idxTestGesture]);
                                        mIdxComparison++;
                                    }
                                }

                                currentRecord++;
                                UpdateProgress(totalNumbRecords, currentRecord);
                            }

                            skipHackAttempt += limitHackAttempt;

                            if (totalNumbRecords <= skipHackAttempt)
                            {
                                isFinishedHackAttempts = true;                                
                            }
                        }
                                                         
                        UpdateProgress(totalNumbRecords, currentRecord);
                    }

                    mStreamWriterGestures.Flush();
                    mStreamWriterStrokes.Flush();
                    mStreamWriterLog.Flush();

                    skip += limit;

                    if (totalNumbRecords <= skip)
                    {
                        mIsFinished = true;
                        mStreamWriterGestures.Close();
                        mStreamWriterStrokes.Close();
                        mStreamWriterLog.Close();

                        mEndTime = DateTime.Now;
                        UpdateProgress(totalNumbRecords, currentRecord, true);
                    }
                }
            }
            catch (Exception exc)
            {
                MessageBox.Show(string.Format("Error: {0}", exc.Message));
            }
        }

        private void InitGestureHeader(StreamWriter streamWriter)
        {
            StringBuilder stringBuilder = new StringBuilder();
            AppendWithComma(stringBuilder, "Comparison Index");
            AppendWithComma(stringBuilder, "UserName");
            AppendWithComma(stringBuilder, "TemplateStoredId");
            AppendWithComma(stringBuilder, "UserNameHack");
            AppendWithComma(stringBuilder, "TemplateStoredHackId");
            AppendWithComma(stringBuilder, "GestureAuthId");
            AppendWithComma(stringBuilder, "ComparisonType");
            AppendWithComma(stringBuilder, "Instruction");
            AppendWithComma(stringBuilder, "TotalScore");

            for(int idx = 0; idx < GESTURE_NUM_PARAMS; idx++)
            {
                AddGestureParamHeader(stringBuilder, string.Format("Param{0}", (idx + 1).ToString()));
            }

            streamWriter.WriteLine(stringBuilder.ToString());
        }

        private void AddGestureParamHeader(StringBuilder stringBuilder, string paramName)
        {
            AppendWithComma(stringBuilder, string.Format("{0}{1}", paramName, "Name"));
            AppendWithComma(stringBuilder, string.Format("{0}{1}", paramName, "Score"));
            AppendWithComma(stringBuilder, string.Format("{0}{1}", paramName, "AuthValue"));
            AppendWithComma(stringBuilder, string.Format("{0}{1}", paramName, "BaseMean"));
            AppendWithComma(stringBuilder, string.Format("{0}{1}", paramName, "Zscore"));
            AppendWithComma(stringBuilder, string.Format("{0}{1}", paramName, "PopStd"));
            AppendWithComma(stringBuilder, string.Format("{0}{1}", paramName, "InternalStd"));
        }

        private void InitStrokesHeader(StreamWriter streamWriter)
        {
            StringBuilder stringBuilder = new StringBuilder();
            AppendWithComma(stringBuilder, "Comparison Index");
            AppendWithComma(stringBuilder, "Stroke Index");
            AppendWithComma(stringBuilder, "UserName");
            AppendWithComma(stringBuilder, "TemplateStoredId");
            AppendWithComma(stringBuilder, "UserNameHack");
            AppendWithComma(stringBuilder, "TemplateStoredHackId");
            AppendWithComma(stringBuilder, "GestureAuthId");
            AppendWithComma(stringBuilder, "StrokeAuthId");
            AppendWithComma(stringBuilder, "ComparisonType");
            AppendWithComma(stringBuilder, "Instruction");

            AppendWithComma(stringBuilder, "StrokeScore");
            AppendWithComma(stringBuilder, "StrokeCosineDistance");

            AppendWithComma(stringBuilder, "StrokeDistanceTotalScore");
            AppendWithComma(stringBuilder, "StrokeDistanceTotalScoreStartToStart");
            AppendWithComma(stringBuilder, "StrokeDistanceTotalScoreStartToEnd");            
            AppendWithComma(stringBuilder, "StrokeDistanceTotalScoreEndToStart");
            AppendWithComma(stringBuilder, "StrokeDistanceTotalScoreEndToEnd");

            AppendWithComma(stringBuilder, "DtwAccelerations");
            AppendWithComma(stringBuilder, "DtwCoordinates");
            AppendWithComma(stringBuilder, "DtwEvents");
            AppendWithComma(stringBuilder, "DtwNormalizedCoordinates");
            AppendWithComma(stringBuilder, "DtwNormalizedCoordinatesSpatialDistance");
            AppendWithComma(stringBuilder, "DtwSpatialAccelerationDistance");
            AppendWithComma(stringBuilder, "DtwSpatialAccelerationTime");
            AppendWithComma(stringBuilder, "DtwSpatialAccumNormAreaDistance");
            AppendWithComma(stringBuilder, "DtwSpatialAccumNormAreaTime");
            AppendWithComma(stringBuilder, "DtwSpatialDeltaTetaDistance");
            AppendWithComma(stringBuilder, "DtwSpatialDeltaTetaTime");
            AppendWithComma(stringBuilder, "DtwSpatialRadialAccelerationDistance");
            AppendWithComma(stringBuilder, "DtwSpatialRadialAccelerationTime");
            AppendWithComma(stringBuilder, "DtwSpatialRadialVelocityDistance");
            AppendWithComma(stringBuilder, "DtwSpatialRadialVelocityTime");
            AppendWithComma(stringBuilder, "DtwSpatialRadiusDistance");
            AppendWithComma(stringBuilder, "DtwSpatialRadiusTime");
            AppendWithComma(stringBuilder, "DtwSpatialTetaDistance");
            AppendWithComma(stringBuilder, "DtwSpatialTetaTime");
            AppendWithComma(stringBuilder, "DtwSpatialVelocityDistance");
            AppendWithComma(stringBuilder, "DtwSpatialVelocityTime");
            AppendWithComma(stringBuilder, "DtwVelocities");

            AppendWithComma(stringBuilder, "SpatialScoreDistanceVelocity");
            AppendWithComma(stringBuilder, "SpatialScoreDistanceAcceleration");
            AppendWithComma(stringBuilder, "SpatialScoreDistanceRadialVelocity");
            AppendWithComma(stringBuilder, "SpatialScoreDistanceRadialAcceleration");
            AppendWithComma(stringBuilder, "SpatialScoreDistanceRadius");
            AppendWithComma(stringBuilder, "SpatialScoreDistanceTeta");
            AppendWithComma(stringBuilder, "SpatialScoreDistanceDeltaTeta");
            AppendWithComma(stringBuilder, "SpatialScoreDistanceAccumulatedNormArea");

            AppendWithComma(stringBuilder, "SpatialScoreTimeVelocity");
            AppendWithComma(stringBuilder, "SpatialScoreTimeAcceleration");
            AppendWithComma(stringBuilder, "SpatialScoreTimeRadialVelocity");
            AppendWithComma(stringBuilder, "SpatialScoreTimeRadialAcceleration");
            AppendWithComma(stringBuilder, "SpatialScoreTimeRadius");
            AppendWithComma(stringBuilder, "SpatialScoreTimeTeta");
            AppendWithComma(stringBuilder, "SpatialScoreTimeDeltaTeta");
            AppendWithComma(stringBuilder, "SpatialScoreTimeAccumulatedNormArea");

            streamWriter.WriteLine(stringBuilder.ToString());
        }

        private StreamWriter InitStreamWriterResultGestures()
        {
            StreamWriter streamWriter = File.CreateText(@"C:\Temp\ResultGestures.csv");
            return streamWriter;
        }

        private StreamWriter InitStreamWriterResultStrokes()
        {
            StreamWriter streamWriter = File.CreateText(@"C:\Temp\ResultStrokes.csv");
            return streamWriter;
        }

        private StreamWriter InitStreamWriterLog()
        {
            StreamWriter streamWriter = File.CreateText(@"C:\Temp\Log.txt");
            return streamWriter;
        }

        private void WriteStringsToFile(List<string> listResultStrings, StreamWriter streamWriter)
        {
            for (int idxLine = 0; idxLine < listResultStrings.Count; idxLine++)
            {
                streamWriter.WriteLine(listResultStrings[idxLine]);
            }
        }

        private void Init()
        {

            try
            {
                ModelNormContainerMgr tempNormContainerMgr = JsonConvert.DeserializeObject<ModelNormContainerMgr>(normsString);
                NormContainerMgr normContainerMgr = tempNormContainerMgr.ToNormContainerMgr();

                NormMgr.GetInstance().GetNorms(normContainerMgr);
            }
            catch (Exception exc)
            {
                string msg = exc.Message;
            }
        }

        private void UpdateProgress(double totalRecords, double currentRecord, bool isFinished = false)
        {
            String msg = string.Empty;
            if (isFinished)
            {
                int diff = Math.Abs(mEndTime.Second - mStartTime.Second);
                msg = String.Format("Completed {0}/{1} in {2} seconds", currentRecord.ToString(), totalRecords.ToString(), diff.ToString());
            }
            else
            {
                msg = String.Format("Completed {0}/{1}", currentRecord.ToString(), totalRecords.ToString());
            }
            
            this.lblCompleted.Invoke(new MethodInvoker(() => this.lblCompleted.Text = msg));
        }

        private void CompareGesturesToStrings(TemplateExtended tempTemplate, TemplateExtended tempTemplateHack, GestureExtended gestureTest)
        {
            GestureExtended gestureTraining = null;

            GestureComparer gestureComparer = new GestureComparer(true);
            string instruction = gestureTest.Instruction;

            for (int idxTrainingGesture = 0; idxTrainingGesture < tempTemplate.ListGestureExtended.size(); idxTrainingGesture++) {
                if (((GestureExtended)tempTemplate.ListGestureExtended.get(idxTrainingGesture)).Instruction.CompareTo(instruction) == 0)
                {
                    gestureTraining = (GestureExtended)tempTemplate.ListGestureExtended.get(idxTrainingGesture);
                    break;
                }
            }

            try
            {
                gestureComparer.CompareGestures(gestureTraining, gestureTest);
                double tempScore = gestureComparer.GetScore();

                List<string> listResultStrings = new List<string>();

                listResultStrings.Add(CreateGestureResultStrings(gestureComparer, tempTemplate, gestureTest, tempTemplateHack));
                WriteStringsToFile(listResultStrings, mStreamWriterGestures);

                listResultStrings = CreateStrokeResultStrings(gestureComparer, tempTemplate, gestureTest, tempTemplateHack.Name, tempTemplateHack.Id.ToString());
                WriteStringsToFile(listResultStrings, mStreamWriterStrokes);
            }
            catch (Exception exc)
            {
                string msg = exc.Message;
            }
        }

        private string CreateGestureResultStrings(GestureComparer gestureComparer, TemplateExtended templateStored, GestureExtended gestureAuth, TemplateExtended templateStoredHack)
        {
            List<ICompareResult> listGestureParams = new List<ICompareResult>();
            for (int idx = 0; idx < gestureComparer.GetResultsSummary().ListCompareResults.size(); idx++) {
                listGestureParams.Add((ICompareResult)gestureComparer.GetResultsSummary().ListCompareResults.get(idx));
            }

            listGestureParams = listGestureParams.OrderBy(o => o.GetName()).ToList();           

            string resultString = CreateGestureLine(mIdxComparison.ToString(), templateStored.Name, templateStored.Id.ToString(), templateStoredHack.Name, templateStoredHack.Id.ToString(), gestureAuth.Id.ToString(), mSimulationType, gestureAuth.Instruction, gestureComparer.GetScore().ToString(), listGestureParams);
            return resultString;
        }

        private string CreateGestureLine(string idxComparison, string userName, string templateStoredId, string userNameHack, string templateStoredHackId, string gestureAuthId, string comparisonType, string instruction, string totalScore, List<ICompareResult> listParams)
        {
            StringBuilder stringBuilder = new StringBuilder();
            AppendWithComma(stringBuilder, idxComparison);
            AppendWithComma(stringBuilder, userName);
            AppendWithComma(stringBuilder, templateStoredId);
            AppendWithComma(stringBuilder, userNameHack);
            AppendWithComma(stringBuilder, templateStoredHackId);
            AppendWithComma(stringBuilder, gestureAuthId);
            AppendWithComma(stringBuilder, comparisonType);
            AppendWithComma(stringBuilder, instruction);
            AppendWithComma(stringBuilder, totalScore);

            for (int idx = 0; idx < GESTURE_NUM_PARAMS; idx++)
            {
                AppendGestureParam(stringBuilder, listParams[idx]);
            }

            return stringBuilder.ToString();
        }

        private void AppendGestureParam(StringBuilder stringBuilder, ICompareResult gestureParam)
        {
            AppendWithComma(stringBuilder, gestureParam.GetName());
            AppendWithComma(stringBuilder, gestureParam.GetValue().ToString());
            AppendWithComma(stringBuilder, gestureParam.GetOriginalValue().ToString());
            AppendWithComma(stringBuilder, gestureParam.GetMean().ToString());

            AppendWithComma(stringBuilder, gestureParam.GetWeight().ToString());
            AppendWithComma(stringBuilder, gestureParam.GetSD().ToString());
            AppendWithComma(stringBuilder, gestureParam.GetInternalSD().ToString());
        }

        private List<string> CreateStrokeResultStrings(GestureComparer gestureComparer, TemplateExtended templateStored, GestureExtended gestureAuth, string usernameHack, string templateStoredHackId)
        {
            List<string> listResultString = new List<string>();
            string line;

            StrokeComparer tempStrokeComparer;
            for (int idxStrokeComparer = 0; idxStrokeComparer < gestureComparer.GetStrokeComparers().size(); idxStrokeComparer++)
            {
                tempStrokeComparer = (StrokeComparer)gestureComparer.GetStrokeComparers().get(idxStrokeComparer);
                line = CreateStrokeLine(mIdxComparison.ToString(), idxStrokeComparer.ToString(), templateStored.Name, templateStored.Id.ToString(), usernameHack, templateStoredHackId, gestureAuth.Id.ToString(), ((StrokeExtended)gestureAuth.ListStrokesExtended.get(idxStrokeComparer)).Id.ToString(), mSimulationType, gestureAuth.Instruction, Math.Round(tempStrokeComparer.GetScore(), 5).ToString(), Math.Round(tempStrokeComparer.GetMinCosineDistance(), 5).ToString(), tempStrokeComparer.DtwAccelerations, tempStrokeComparer.DtwCoordinates, tempStrokeComparer.DtwEvents, tempStrokeComparer.DtwNormalizedCoordinates, tempStrokeComparer.DtwNormalizedCoordinatesSpatialDistance, tempStrokeComparer.DtwSpatialAccelerationDistance, tempStrokeComparer.DtwSpatialAccelerationTime, tempStrokeComparer.DtwSpatialAccumNormAreaDistance, tempStrokeComparer.DtwSpatialAccumNormAreaTime, tempStrokeComparer.DtwSpatialDeltaTetaDistance, tempStrokeComparer.DtwSpatialDeltaTetaTime, tempStrokeComparer.DtwSpatialRadialAccelerationDistance, tempStrokeComparer.DtwSpatialRadialAccelerationTime, tempStrokeComparer.DtwSpatialRadialVelocityDistance, tempStrokeComparer.DtwSpatialRadialVelocityTime, tempStrokeComparer.DtwSpatialRadiusDistance, tempStrokeComparer.DtwSpatialRadiusTime, tempStrokeComparer.DtwSpatialTetaDistance, tempStrokeComparer.DtwSpatialTetaTime, tempStrokeComparer.DtwSpatialVelocityDistance, tempStrokeComparer.DtwSpatialVelocityTime, tempStrokeComparer.DtwVelocities, tempStrokeComparer.SpatialScoreDistanceVelocity, tempStrokeComparer.SpatialScoreDistanceAcceleration, tempStrokeComparer.SpatialScoreDistanceRadialVelocity, tempStrokeComparer.SpatialScoreDistanceRadialAcceleration, tempStrokeComparer.SpatialScoreDistanceRadius, tempStrokeComparer.SpatialScoreDistanceTeta, tempStrokeComparer.SpatialScoreDistanceDeltaTeta, tempStrokeComparer.SpatialScoreDistanceAccumulatedNormArea, tempStrokeComparer.SpatialScoreTimeVelocity, tempStrokeComparer.SpatialScoreTimeAcceleration, tempStrokeComparer.SpatialScoreTimeRadialVelocity, tempStrokeComparer.SpatialScoreTimeRadialAcceleration, tempStrokeComparer.SpatialScoreTimeRadius, tempStrokeComparer.SpatialScoreTimeTeta, tempStrokeComparer.SpatialScoreTimeDeltaTeta, tempStrokeComparer.SpatialScoreTimeAccumulatedNormArea, tempStrokeComparer.StrokeDistanceTotalScore, tempStrokeComparer.StrokeDistanceTotalScoreStartToStart, tempStrokeComparer.StrokeDistanceTotalScoreStartToEnd, tempStrokeComparer.StrokeDistanceTotalScoreEndToStart, tempStrokeComparer.StrokeDistanceTotalScoreEndToEnd);
                listResultString.Add(line);
            }

            return listResultString;
        }

        private string CreateStrokeLine(string idxComparison, string idxStroke, string userName, string templateStoredId, string userNameHack, string templateStoredHackId, string gestureAuthId, string strokeAuthId, string comparisonType, string instruction, string totalScore, string strokeCosineDistance, double DtwAccelerations, double DtwCoordinates, double DtwEvents, double DtwNormalizedCoordinates, double DtwNormalizedCoordinatesSpatialDistance, double DtwSpatialAccelerationDistance, double DtwSpatialAccelerationTime, double DtwSpatialAccumNormAreaDistance, double DtwSpatialAccumNormAreaTime, double DtwSpatialDeltaTetaDistance, double DtwSpatialDeltaTetaTime, double DtwSpatialRadialAccelerationDistance, double DtwSpatialRadialAccelerationTime, double DtwSpatialRadialVelocityDistance, double DtwSpatialRadialVelocityTime, double DtwSpatialRadiusDistance, double DtwSpatialRadiusTime, double DtwSpatialTetaDistance, double DtwSpatialTetaTime, double DtwSpatialVelocityDistance, double DtwSpatialVelocityTime, double DtwVelocities, double SpatialScoreDistanceVelocity, double SpatialScoreDistanceAcceleration, double SpatialScoreDistanceRadialVelocity, double SpatialScoreDistanceRadialAcceleration, double SpatialScoreDistanceRadius, double SpatialScoreDistanceTeta, double SpatialScoreDistanceDeltaTeta, double SpatialScoreDistanceAccumulatedNormArea, double SpatialScoreTimeVelocity, double SpatialScoreTimeAcceleration, double SpatialScoreTimeRadialVelocity, double SpatialScoreTimeRadialAcceleration, double SpatialScoreTimeRadius, double SpatialScoreTimeTeta, double SpatialScoreTimeDeltaTeta, double SpatialScoreTimeAccumulatedNormArea, double StrokeDistanceTotalScore, double StrokeDistanceTotalScoreStartToStart, double StrokeDistanceTotalScoreStartToEnd, double StrokeDistanceTotalScoreEndToStart, double StrokeDistanceTotalScoreEndToEnd)
        {
            StringBuilder stringBuilder = new StringBuilder();

            AppendWithComma(stringBuilder, idxComparison);
            AppendWithComma(stringBuilder, idxStroke);
            AppendWithComma(stringBuilder, userName);
            AppendWithComma(stringBuilder, templateStoredId);
            AppendWithComma(stringBuilder, userNameHack);
            AppendWithComma(stringBuilder, templateStoredHackId);
            AppendWithComma(stringBuilder, gestureAuthId);
            AppendWithComma(stringBuilder, strokeAuthId);
            AppendWithComma(stringBuilder, comparisonType);
            AppendWithComma(stringBuilder, instruction);

            AppendWithComma(stringBuilder, totalScore);
            AppendWithComma(stringBuilder, strokeCosineDistance);

            AppendWithComma(stringBuilder, (Math.Round(StrokeDistanceTotalScore, 5).ToString()));
            AppendWithComma(stringBuilder, (Math.Round(StrokeDistanceTotalScoreStartToStart, 5).ToString()));
            AppendWithComma(stringBuilder, (Math.Round(StrokeDistanceTotalScoreStartToEnd, 5).ToString()));
            AppendWithComma(stringBuilder, (Math.Round(StrokeDistanceTotalScoreEndToStart, 5).ToString()));
            AppendWithComma(stringBuilder, (Math.Round(StrokeDistanceTotalScoreEndToEnd, 5).ToString()));

            AppendWithComma(stringBuilder, Math.Round(DtwAccelerations, 5).ToString());
            AppendWithComma(stringBuilder, Math.Round(DtwCoordinates, 5).ToString());
            AppendWithComma(stringBuilder, Math.Round(DtwEvents, 5).ToString()); 
            AppendWithComma(stringBuilder, Math.Round(DtwNormalizedCoordinates, 5).ToString());
            AppendWithComma(stringBuilder, Math.Round(DtwNormalizedCoordinatesSpatialDistance, 5).ToString());
            AppendWithComma(stringBuilder, Math.Round(DtwSpatialAccelerationDistance, 5).ToString());
            AppendWithComma(stringBuilder, Math.Round(DtwSpatialAccelerationTime, 5).ToString());
            AppendWithComma(stringBuilder, Math.Round(DtwSpatialAccumNormAreaDistance, 5).ToString());
            AppendWithComma(stringBuilder, Math.Round(DtwSpatialAccumNormAreaTime, 5).ToString());
            AppendWithComma(stringBuilder, Math.Round(DtwSpatialDeltaTetaDistance, 5).ToString());
            AppendWithComma(stringBuilder, Math.Round(DtwSpatialDeltaTetaTime, 5).ToString());
            AppendWithComma(stringBuilder, Math.Round(DtwSpatialRadialAccelerationDistance, 5).ToString());
            AppendWithComma(stringBuilder, Math.Round(DtwSpatialRadialAccelerationTime, 5).ToString());
            AppendWithComma(stringBuilder, Math.Round(DtwSpatialRadialVelocityDistance, 5).ToString());
            AppendWithComma(stringBuilder, Math.Round(DtwSpatialRadialVelocityTime, 5).ToString());
            AppendWithComma(stringBuilder, Math.Round(DtwSpatialRadiusDistance, 5).ToString());
            AppendWithComma(stringBuilder, Math.Round(DtwSpatialRadiusTime, 5).ToString());
            AppendWithComma(stringBuilder, Math.Round(DtwSpatialTetaDistance, 5).ToString());
            AppendWithComma(stringBuilder, Math.Round(DtwSpatialTetaTime, 5).ToString());
            AppendWithComma(stringBuilder, Math.Round(DtwSpatialVelocityDistance, 5).ToString());
            AppendWithComma(stringBuilder, Math.Round(DtwSpatialVelocityTime, 5).ToString());
            AppendWithComma(stringBuilder, Math.Round(DtwVelocities, 5).ToString());            

            AppendWithComma(stringBuilder, Math.Round(SpatialScoreDistanceVelocity, 5).ToString());
            AppendWithComma(stringBuilder, Math.Round(SpatialScoreDistanceAcceleration, 5).ToString());
            AppendWithComma(stringBuilder, Math.Round(SpatialScoreDistanceRadialVelocity, 5).ToString());
            AppendWithComma(stringBuilder, Math.Round(SpatialScoreDistanceRadialAcceleration, 5).ToString());
            AppendWithComma(stringBuilder, Math.Round(SpatialScoreDistanceRadius, 5).ToString());
            AppendWithComma(stringBuilder, Math.Round(SpatialScoreDistanceTeta, 5).ToString());
            AppendWithComma(stringBuilder, Math.Round(SpatialScoreDistanceDeltaTeta, 5).ToString());
            AppendWithComma(stringBuilder, Math.Round(SpatialScoreDistanceAccumulatedNormArea, 5).ToString());

            AppendWithComma(stringBuilder, Math.Round(SpatialScoreTimeVelocity, 5).ToString());
            AppendWithComma(stringBuilder, Math.Round(SpatialScoreTimeAcceleration, 5).ToString());
            AppendWithComma(stringBuilder, Math.Round(SpatialScoreTimeRadialVelocity, 5).ToString());
            AppendWithComma(stringBuilder, Math.Round(SpatialScoreTimeRadialAcceleration, 5).ToString());
            AppendWithComma(stringBuilder, Math.Round(SpatialScoreTimeRadius, 5).ToString());
            AppendWithComma(stringBuilder, Math.Round(SpatialScoreTimeTeta, 5).ToString());
            AppendWithComma(stringBuilder, Math.Round(SpatialScoreTimeDeltaTeta, 5).ToString());
            AppendWithComma(stringBuilder, Math.Round(SpatialScoreTimeAccumulatedNormArea, 5).ToString());            

            return stringBuilder.ToString();
        }        

        private void AppendWithComma(StringBuilder stringBuilder, string input)
        {
            stringBuilder.Append(input);
            stringBuilder.Append(",");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            mSimulationType = "Hack";
            Task task = Task.Run((Action)RunSimulatorHackAttempts);
        }
    }
}