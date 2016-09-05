using Data.Comparison.Interfaces;
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
        double mThreashold70;
        double mThreashold75;
        double mThreashold80;
        double mThreashold85;
        double mThreashold90;
        double mThreashold95;

        double mTotalNumComparisons;

        int GESTURE_NUM_PARAMS = 3;
        int STROKE_NUM_PARAMS = 12;

        int mIdxComparison;

        const string SIMULATOR_TYPE_HACK = "Hack";
        const string SIMULATOR_TYPE_AUTH = "Auth";

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

        private void UpdateThreasholds(double score)
        {
            if(mSimulationType.CompareTo(SIMULATOR_TYPE_AUTH) == 0)
            {
                if(score < 0.7)
                {
                    mThreashold70++;
                }
                if (score < 0.75)
                {
                    mThreashold75++;
                }
                if (score < 0.8)
                {
                    mThreashold80++;
                }
                if (score < 0.85)
                {
                    mThreashold85++;
                }
                if (score < 0.90)
                {
                    mThreashold90++;
                }
                if (score < 0.95)
                {
                    mThreashold95++;
                }
            }
            if (mSimulationType.CompareTo(SIMULATOR_TYPE_HACK) == 0)
            {
                if (score > 0.7)
                {
                    mThreashold70++;
                }
                if (score > 0.75)
                {
                    mThreashold75++;
                }
                if (score > 0.8)
                {
                    mThreashold80++;
                }
                if (score > 0.85)
                {
                    mThreashold85++;
                }
                if (score > 0.90)
                {
                    mThreashold90++;
                }
                if (score > 0.95)
                {
                    mThreashold95++;
                }
            }

            double percentage = mThreashold70 / mTotalNumComparisons;        
            this.lblThreashold70.Invoke(new MethodInvoker(() => this.lblThreashold70.Text = GetPercentageString(mThreashold70)));

            percentage = mThreashold75 / mTotalNumComparisons;
            this.lblThreashold75.Invoke(new MethodInvoker(() => this.lblThreashold75.Text = GetPercentageString(mThreashold75)));

            percentage = mThreashold80 / mTotalNumComparisons;
            this.lblThreashold80.Invoke(new MethodInvoker(() => this.lblThreashold80.Text = GetPercentageString(mThreashold80)));

            percentage = mThreashold85 / mTotalNumComparisons;
            this.lblThreashold85.Invoke(new MethodInvoker(() => this.lblThreashold85.Text = GetPercentageString(mThreashold85)));

            percentage = mThreashold90 / mTotalNumComparisons;
            this.lblThreashold90.Invoke(new MethodInvoker(() => this.lblThreashold90.Text = GetPercentageString(mThreashold90)));

            percentage = mThreashold95 / mTotalNumComparisons;
            this.lblThreashold95.Invoke(new MethodInvoker(() => this.lblThreashold95.Text = GetPercentageString(mThreashold95)));


        }

        private string GetPercentageString(double value)
        {
            double percentage = value / mTotalNumComparisons * 100;
            string percentageString = string.Format("{0}%", Math.Round(percentage, 4));
            return percentageString;
        }

        private void btnGo_Click(object sender, EventArgs e)
        {
            mTotalNumComparisons = 1;
            lblStartTime.Text = DateTime.Now.ToLongTimeString();
            mSimulationType = SIMULATOR_TYPE_AUTH;
            lblType.Text = SIMULATOR_TYPE_AUTH;
            Task task = Task.Run((Action)RunSimulatorValidUsers);
            btnGo.Enabled = false;
            button1.Enabled = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            mTotalNumComparisons = 1;
            lblStartTime.Text = DateTime.Now.ToLongTimeString();
            mSimulationType = SIMULATOR_TYPE_HACK;
            lblType.Text = SIMULATOR_TYPE_HACK;
            Task task = Task.Run((Action)RunSimulatorHackAttempts);
            button1.Enabled = false;
            btnGo.Enabled = false;
        }

        protected void RunSimulatorValidUsers()
        {          
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

                        this.lblEndTime.Invoke(new MethodInvoker(() => this.lblEndTime.Text = DateTime.Now.ToLongTimeString()));
                        UpdateProgress(totalNumbRecords, currentRecord);
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

                        this.lblEndTime.Invoke(new MethodInvoker(() => this.lblEndTime.Text = DateTime.Now.ToLongTimeString()));
                        UpdateProgress(totalNumbRecords, currentRecord);
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
                AddStatParamHeader(stringBuilder, string.Format("Param{0}", (idx + 1).ToString()));
            }

            streamWriter.WriteLine(stringBuilder.ToString());
        }

        private void AddStatParamHeader(StringBuilder stringBuilder, string paramName)
        {
            AppendWithComma(stringBuilder, string.Format("{0}{1}", paramName, "Name"));
            AppendWithComma(stringBuilder, string.Format("{0}{1}", paramName, "Score"));
            AppendWithComma(stringBuilder, string.Format("{0}{1}", paramName, "AuthValue"));
            AppendWithComma(stringBuilder, string.Format("{0}{1}", paramName, "BaseMean"));
            AppendWithComma(stringBuilder, string.Format("{0}{1}", paramName, "PopMean"));
            AppendWithComma(stringBuilder, string.Format("{0}{1}", paramName, "Weight"));
            AppendWithComma(stringBuilder, string.Format("{0}{1}", paramName, "Zscore"));
            AppendWithComma(stringBuilder, string.Format("{0}{1}", paramName, "PopStd"));
            AppendWithComma(stringBuilder, string.Format("{0}{1}", paramName, "InternalStd"));
            AppendWithComma(stringBuilder, string.Format("{0}{1}", paramName, "InternalStdUserOnly"));
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

            AppendWithComma(stringBuilder, "TotalSpatialTemporalScore");

            for (int idx = 0; idx < STROKE_NUM_PARAMS; idx++)
            {
                AddStatParamHeader(stringBuilder, string.Format("Param{0}", (idx + 1).ToString()));
            }

            streamWriter.WriteLine(stringBuilder.ToString());
        }

        private StreamWriter InitStreamWriterResultGestures()
        {
            StreamWriter streamWriter = File.CreateText(@"C:\Temp\ResultGestures-" + mSimulationType + ".csv");
            return streamWriter;
        }

        private StreamWriter InitStreamWriterResultStrokes()
        {
            StreamWriter streamWriter = File.CreateText(@"C:\Temp\ResultStrokes-" + mSimulationType + ".csv");
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
                ModelNormContainerMgr tempNormContainerMgr = JsonConvert.DeserializeObject<ModelNormContainerMgr>(ConstsNormsStr.Norms);
                NormContainerMgr normContainerMgr = tempNormContainerMgr.ToNormContainerMgr();

                NormMgr.GetInstance().GetNorms(normContainerMgr);
            }
            catch (Exception exc)
            {
                string msg = exc.Message;
            }
        }

        private void UpdateProgress(double totalRecords, double currentRecord)
        {
            String msg = String.Format("Completed {0}/{1}", currentRecord.ToString(), totalRecords.ToString());
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
                double tempScore = 0;
                try
                {
                    gestureComparer.CompareGestures(gestureTraining, gestureTest);
                    tempScore = gestureComparer.GetScore();
                }
                catch (Exception exc)
                {
                    string msg = exc.Message;
                }

                mTotalNumComparisons++;

                UpdateThreasholds(tempScore);

                List<string> listResultStrings = new List<string>();

                try
                {
                    listResultStrings.Add(CreateGestureResultStrings(gestureComparer, tempTemplate, gestureTest, tempTemplateHack));
                }
                catch (Exception exc)
                {
                    string msg = exc.Message;
                }
                
                
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
            List<ICompareResult> listGestureParams = ConvertParamsList(gestureComparer.GetResultsSummary().ListCompareResults);            
            
            string resultString = CreateGestureLine(mIdxComparison.ToString(), templateStored.Name, templateStored.Id.ToString(), templateStoredHack.Name, templateStoredHack.Id.ToString(), gestureAuth.Id.ToString(), mSimulationType, gestureAuth.Instruction, gestureComparer.GetScore().ToString(), listGestureParams);            
            
            return resultString;
        }

        private List<ICompareResult> ConvertParamsList(ArrayList listInput)
        {
            List<ICompareResult> listGestureParams = new List<ICompareResult>();
            for (int idx = 0; idx < listInput.size(); idx++)
            {
                listGestureParams.Add((ICompareResult)listInput.get(idx));
            }

            listGestureParams = listGestureParams.OrderBy(o => o.GetName()).ToList();

            return listGestureParams;
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
                if(idx < listParams.Count)
                {
                    AppendGestureParam(stringBuilder, listParams[idx]);
                }
                else
                {
                    AppendGestureParam(stringBuilder, null);
                }
            }

            return stringBuilder.ToString();
        }

        private void AppendGestureParam(StringBuilder stringBuilder, ICompareResult gestureParam)
        {
            if (gestureParam != null)
            {
                AppendWithComma(stringBuilder, gestureParam.GetName());
                AppendWithComma(stringBuilder, gestureParam.GetValue().ToString());
                AppendWithComma(stringBuilder, gestureParam.GetOriginalValue().ToString());
                AppendWithComma(stringBuilder, gestureParam.GetMean().ToString());
                AppendWithComma(stringBuilder, gestureParam.GetPopMean().ToString());
                AppendWithComma(stringBuilder, gestureParam.GetWeight().ToString());
                AppendWithComma(stringBuilder, gestureParam.GetZScore().ToString());
                AppendWithComma(stringBuilder, gestureParam.GetSD().ToString());
                AppendWithComma(stringBuilder, gestureParam.GetInternalSD().ToString());
                AppendWithComma(stringBuilder, gestureParam.GetInternalSdUserOnly().ToString());
            }
            else {
                AppendWithComma(stringBuilder, string.Empty);
                AppendWithComma(stringBuilder, string.Empty);
                AppendWithComma(stringBuilder, string.Empty);
                AppendWithComma(stringBuilder, string.Empty);
                AppendWithComma(stringBuilder, string.Empty);
                AppendWithComma(stringBuilder, string.Empty);
                AppendWithComma(stringBuilder, string.Empty);
                AppendWithComma(stringBuilder, string.Empty);
                AppendWithComma(stringBuilder, string.Empty);
                AppendWithComma(stringBuilder, string.Empty);
            }
        }

        private List<string> CreateStrokeResultStrings(GestureComparer gestureComparer, TemplateExtended templateStored, GestureExtended gestureAuth, string usernameHack, string templateStoredHackId)
        {
            List<string> listResultString = new List<string>();
            string line;

            List<ICompareResult> listStrokeParams;
            StrokeComparer tempStrokeComparer;
            for (int idxStrokeComparer = 0; idxStrokeComparer < gestureComparer.GetStrokeComparers().size(); idxStrokeComparer++)
            {
                listStrokeParams = ConvertParamsList(((StrokeComparer)gestureComparer.GetStrokeComparers().get(idxStrokeComparer)).GetResultsSummary().ListCompareResults);

                tempStrokeComparer = (StrokeComparer)gestureComparer.GetStrokeComparers().get(idxStrokeComparer);
                line = CreateStrokeLine(mIdxComparison.ToString(), idxStrokeComparer.ToString(), templateStored.Name, templateStored.Id.ToString(), usernameHack, templateStoredHackId, gestureAuth.Id.ToString(), ((StrokeExtended)gestureAuth.ListStrokesExtended.get(idxStrokeComparer)).Id.ToString(), mSimulationType, gestureAuth.Instruction, Math.Round(tempStrokeComparer.GetScore(), 5).ToString(), Math.Round(tempStrokeComparer.GetMinCosineDistance(), 5).ToString(), tempStrokeComparer.DtwAccelerations, tempStrokeComparer.DtwCoordinates, tempStrokeComparer.DtwEvents, tempStrokeComparer.DtwNormalizedCoordinates, tempStrokeComparer.DtwNormalizedCoordinatesSpatialDistance, tempStrokeComparer.DtwSpatialAcceleration, tempStrokeComparer.DtwTemporalAcceleration, tempStrokeComparer.DtwSpatialAccumNormArea, tempStrokeComparer.DtwTemporalAccumNormArea, tempStrokeComparer.DtwSpatialDeltaTeta, tempStrokeComparer.DtwTemporalDeltaTeta, tempStrokeComparer.DtwSpatialRadialAcceleration, tempStrokeComparer.DtwTemporalRadialAcceleration, tempStrokeComparer.DtwSpatialRadialVelocity, tempStrokeComparer.DtwTemporalRadialVelocity, tempStrokeComparer.DtwSpatialRadius, tempStrokeComparer.DtwTemporalRadius, tempStrokeComparer.DtwSpatialTeta, tempStrokeComparer.DtwTemporalTeta, tempStrokeComparer.DtwSpatialVelocity, tempStrokeComparer.DtwTemporalVelocity, tempStrokeComparer.DtwVelocities, tempStrokeComparer.SpatialScoreVelocity, tempStrokeComparer.SpatialScoreAcceleration, tempStrokeComparer.SpatialScoreRadialVelocity, tempStrokeComparer.SpatialScoreRadialAcceleration, tempStrokeComparer.SpatialScoreRadius, tempStrokeComparer.SpatialScoreTeta, tempStrokeComparer.SpatialScoreDeltaTeta, tempStrokeComparer.SpatialScoreAccumulatedNormArea, tempStrokeComparer.TemporalScoreVelocity, tempStrokeComparer.TemporalScoreAcceleration, tempStrokeComparer.TemporalScoreRadialVelocity, tempStrokeComparer.TemporalScoreRadialAcceleration, tempStrokeComparer.TemporalScoreRadius, tempStrokeComparer.TemporalScoreTeta, tempStrokeComparer.TemporalScoreDeltaTeta, tempStrokeComparer.TemporalScoreAccumulatedNormArea, tempStrokeComparer.StrokeSpatialScore, tempStrokeComparer.StrokeDistanceTotalScore, tempStrokeComparer.StrokeDistanceTotalScoreStartToStart, tempStrokeComparer.StrokeDistanceTotalScoreStartToEnd, tempStrokeComparer.StrokeDistanceTotalScoreEndToStart, tempStrokeComparer.StrokeDistanceTotalScoreEndToEnd, listStrokeParams);
                listResultString.Add(line);
            }

            return listResultString;
        }

        private string CreateStrokeLine(string idxComparison, string idxStroke, string userName, string templateStoredId, string userNameHack, string templateStoredHackId, string gestureAuthId, string strokeAuthId, string comparisonType, string instruction, string totalScore, string strokeCosineDistance, double DtwAccelerations, double DtwCoordinates, double DtwEvents, double DtwNormalizedCoordinates, double DtwNormalizedCoordinatesSpatialDistance, double DtwSpatialAccelerationDistance, double DtwSpatialAccelerationTime, double DtwSpatialAccumNormAreaDistance, double DtwSpatialAccumNormAreaTime, double DtwSpatialDeltaTetaDistance, double DtwSpatialDeltaTetaTime, double DtwSpatialRadialAccelerationDistance, double DtwSpatialRadialAccelerationTime, double DtwSpatialRadialVelocityDistance, double DtwSpatialRadialVelocityTime, double DtwSpatialRadiusDistance, double DtwSpatialRadiusTime, double DtwSpatialTetaDistance, double DtwSpatialTetaTime, double DtwSpatialVelocityDistance, double DtwSpatialVelocityTime, double DtwVelocities, double SpatialScoreDistanceVelocity, double SpatialScoreDistanceAcceleration, double SpatialScoreDistanceRadialVelocity, double SpatialScoreDistanceRadialAcceleration, double SpatialScoreDistanceRadius, double SpatialScoreDistanceTeta, double SpatialScoreDistanceDeltaTeta, double SpatialScoreDistanceAccumulatedNormArea, double SpatialScoreTimeVelocity, double SpatialScoreTimeAcceleration, double SpatialScoreTimeRadialVelocity, double SpatialScoreTimeRadialAcceleration, double SpatialScoreTimeRadius, double SpatialScoreTimeTeta, double SpatialScoreTimeDeltaTeta, double SpatialScoreTimeAccumulatedNormArea, double totalSpatialTemporalScore, double StrokeDistanceTotalScore, double StrokeDistanceTotalScoreStartToStart, double StrokeDistanceTotalScoreStartToEnd, double StrokeDistanceTotalScoreEndToStart, double StrokeDistanceTotalScoreEndToEnd, List<ICompareResult> listParams)
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
            AppendWithComma(stringBuilder, Math.Round(totalSpatialTemporalScore, 5).ToString());

            for (int idx = 0; idx < STROKE_NUM_PARAMS; idx++)
            {
                if (idx < listParams.Count) {
                    AppendGestureParam(stringBuilder, listParams[idx]);
                }
                else
                {
                    AppendGestureParam(stringBuilder, null);
                }
            }

            return stringBuilder.ToString();
        }        

        private void AppendWithComma(StringBuilder stringBuilder, string input)
        {
            stringBuilder.Append(input);
            stringBuilder.Append(",");
        }
    }
}