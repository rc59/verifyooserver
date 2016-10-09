using Data.Comparison.Interfaces;
using Data.UserProfile.Extended;
using Data.UserProfile.Raw;
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
        int STROKE_NUM_PARAMS = 14;

        int mIdxComparison;

        const string EVENT_TYPE_RAW = "Raw";
        const string EVENT_TYPE_SPATIAL = "Spatial";
        const string EVENT_TYPE_TEMPORAL = "Temporal";

        const string SIMULATOR_TYPE_NAIVE_HACK = "FAR";
        const string SIMULATOR_TYPE_HACK = "Hack";
        const string SIMULATOR_TYPE_AUTH = "FRR";
        const string SIMULATOR_TYPE_DUPLICATE = "DuplicateDB";
        const string SIMULATOR_TYPE_EXPORT = "Export to CSV";

        StreamWriter mStreamWriterCsvTemplates = null;
        StreamWriter mStreamWriterCsvGestures = null;
        StreamWriter mStreamWriterCsvStrokes = null;
        StreamWriter mStreamWriterCsvMotionEventsRaw = null;
        StreamWriter mStreamWriterCsvMotionEventsSpatial = null;
        StreamWriter mStreamWriterCsvMotionEventsTemporal = null;

        StreamWriter mStreamWriterGestures = null;
        StreamWriter mStreamWriterStrokes = null;
        StreamWriter mStreamWriterLog = null;

        private MongoCollection<ModelTemplate> mListMongo;
        private bool mIsFinished;

        string mSimulationType;
        private int NUM_DECIMALS = 8;

        public Form1()
        {
            InitializeComponent();
        }

        private void UpdateThreasholds(double score)
        {
            if (mSimulationType.CompareTo(SIMULATOR_TYPE_AUTH) == 0)
            {
                if (score < 0.7)
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
            if (mSimulationType.CompareTo(SIMULATOR_TYPE_HACK) == 0 || mSimulationType.CompareTo(SIMULATOR_TYPE_NAIVE_HACK) == 0)
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

            switch (mSimulationType)
            {
                case SIMULATOR_TYPE_AUTH:
                    this.lblFrr.Invoke(new MethodInvoker(() => this.lblFrr.Text = GetPercentageStringFrr(mThreashold80)));
                    break;
                case SIMULATOR_TYPE_NAIVE_HACK:
                    this.lblFar.Invoke(new MethodInvoker(() => this.lblFar.Text = GetPercentageStringFar(mThreashold80)));
                    break;
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

        private double GetFAR(double score)
        {
            if (score == 1 || score == 0)
            {
                return score;
            }
            double far = score * score * score * score + ((1 - score) * score * score * score) * 4 * 3;
            return far;
        }

        private double GetFRR(double score)
        {
            if (score == 1 || score == 0)
            {
                return score;
            } 
            double temp1 = Math.Pow((1 - score), 3) * score * 4;
            double temp2 = Math.Pow((1 - score), 4);

            double temp3 = temp1 + temp2;
            double frr = temp3 * (1 + (1 - temp3) + (1 - temp3) * (1 - temp3));
            frr = 1 - frr;
            return frr;
        }

        private string GetPercentageString(double value)
        {           
            double percentage = value / mTotalNumComparisons * 100;
            string percentageString = string.Format("{0}%", Math.Round(percentage, 4));
            return percentageString;
        }

        private string GetPercentageStringFar(double value)
        {
            double percentage = value / mTotalNumComparisons;
            percentage = GetFAR(percentage) * 100;

            string percentageString = string.Format("{0}%", Math.Round(percentage, 4));
            return percentageString;
        }

        private string GetPercentageStringFrr(double value)
        {
            double percentage = value / mTotalNumComparisons;
            percentage = GetFRR(percentage) * 100;

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

                        EnableButtons();

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

                        EnableButtons();

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

            AppendWithComma(stringBuilder, "AvgZScore");
            AppendWithComma(stringBuilder, "MostUniqueParam");

            AppendWithComma(stringBuilder, "PcaScore");
            AppendWithComma(stringBuilder, "DtwScore");

            for (int idx = 0; idx < GESTURE_NUM_PARAMS; idx++)
            {
                AddStatParamHeader(stringBuilder, string.Format("Param{0}", (idx + 1).ToString()));
            }

            streamWriter.WriteLine(stringBuilder.ToString());
        }

        private void AddStatParamHeader(StringBuilder stringBuilder, string paramName)
        {
            //AppendWithComma(stringBuilder, string.Format("{0}{1}", paramName, "Name"));
            AppendWithComma(stringBuilder, string.Format("{0}{1}", paramName, "Score"));
            //AppendWithComma(stringBuilder, string.Format("{0}{1}", paramName, "AuthValue"));
            //AppendWithComma(stringBuilder, string.Format("{0}{1}", paramName, "BaseMean"));
            //AppendWithComma(stringBuilder, string.Format("{0}{1}", paramName, "PopMean"));
            //AppendWithComma(stringBuilder, string.Format("{0}{1}", paramName, "Weight"));
            //AppendWithComma(stringBuilder, string.Format("{0}{1}", paramName, "TemplateZscore"));
            //AppendWithComma(stringBuilder, string.Format("{0}{1}", paramName, "Zscore"));
            //AppendWithComma(stringBuilder, string.Format("{0}{1}", paramName, "PopStd"));
            //AppendWithComma(stringBuilder, string.Format("{0}{1}", paramName, "InternalStd"));
            //AppendWithComma(stringBuilder, string.Format("{0}{1}", paramName, "InternalStdUserOnly"));
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

            AppendWithComma(stringBuilder, "AccMovDiffX");
            AppendWithComma(stringBuilder, "AccMovDiffY");
            AppendWithComma(stringBuilder, "AccMovDiffZ");
            AppendWithComma(stringBuilder, "AccMovDiffTotal");

            AppendWithComma(stringBuilder, "PcaScore");

            AppendWithComma(stringBuilder, "RadialVelocityDiff");
            AppendWithComma(stringBuilder, "RadialAccelerationDiff");

            AppendWithComma(stringBuilder, "DtwTemporalVelocity0");
            AppendWithComma(stringBuilder, "DtwTemporalVelocity1");
            AppendWithComma(stringBuilder, "DtwTemporalVelocity2");
            AppendWithComma(stringBuilder, "DtwTemporalVelocity3");
            AppendWithComma(stringBuilder, "DtwTemporalVelocity4");
            AppendWithComma(stringBuilder, "DtwTemporalVelocity5");
            AppendWithComma(stringBuilder, "DtwTemporalVelocity6");
            AppendWithComma(stringBuilder, "DtwTemporalVelocity7");
            AppendWithComma(stringBuilder, "DtwTemporalVelocity8");
            AppendWithComma(stringBuilder, "DtwTemporalVelocity9");
            AppendWithComma(stringBuilder, "DtwTemporalVelocity10");
            AppendWithComma(stringBuilder, "DtwTemporalVelocity11");
            AppendWithComma(stringBuilder, "DtwTemporalVelocity12");
            AppendWithComma(stringBuilder, "DtwTemporalVelocity13");
            AppendWithComma(stringBuilder, "DtwTemporalVelocity14");
            AppendWithComma(stringBuilder, "DtwTemporalVelocity15");

            for (int idx = 0; idx < STROKE_NUM_PARAMS; idx++)
            {
                AddStatParamHeader(stringBuilder, string.Format("Param{0}", (idx + 1).ToString()));
            }

            streamWriter.WriteLine(stringBuilder.ToString());
        }

        private StreamWriter InitStreamWriterCSV(string level)
        {
            StreamWriter streamWriter = File.CreateText(@"C:\Temp\Exported-" + level + ".csv");
            return streamWriter;
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

        private double CompareGesturesToStrings(TemplateExtended tempTemplate, GestureExtended gestureTest, TemplateExtended tempTemplateHack)
        {
            GestureExtended gestureTraining = null;

            GestureComparer gestureComparer = new GestureComparer(true);
            string instruction = gestureTest.Instruction;

            for (int idxTrainingGesture = 0; idxTrainingGesture < tempTemplate.ListGestureExtended.size(); idxTrainingGesture++)
            {
                if (((GestureExtended)tempTemplate.ListGestureExtended.get(idxTrainingGesture)).Instruction.CompareTo(instruction) == 0)
                {
                    gestureTraining = (GestureExtended)tempTemplate.ListGestureExtended.get(idxTrainingGesture);
                    break;
                }
            }

            if (gestureTraining != null)
            {
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

                    listResultStrings = CreateStrokeResultStrings(gestureComparer, tempTemplate, gestureTest, string.Empty, string.Empty);
                    WriteStringsToFile(listResultStrings, mStreamWriterStrokes);

                    return tempScore;
                }
                catch (Exception exc)
                {
                    string msg = exc.Message;
                    return -1;
                }
            }
            else {
                return -1;
            }
        }

        private string CreateGestureResultStrings(GestureComparer gestureComparer, TemplateExtended templateStored, GestureExtended gestureAuth, TemplateExtended templateStoredHack)
        {
            List<ICompareResult> listGestureParams = ConvertParamsList(gestureComparer.GetResultsSummary().ListCompareResults);

            string resultString;
            if (templateStoredHack != null) {
                resultString = CreateGestureLine(mIdxComparison.ToString(), templateStored.Name, templateStored.Id.ToString(), templateStoredHack.Name, templateStoredHack.Id.ToString(), gestureAuth.Id.ToString(), mSimulationType, gestureAuth.Instruction, gestureComparer.GetScore().ToString(), listGestureParams, gestureAuth.GetGestureAvgZScore(), gestureAuth.GetMostUniqueParameter(), gestureComparer.PcaScore, gestureComparer.DtwScore);
            }
            else
            {
                resultString = CreateGestureLine(mIdxComparison.ToString(), templateStored.Name, templateStored.Id.ToString(), String.Empty, String.Empty, gestureAuth.Id.ToString(), mSimulationType, gestureAuth.Instruction, gestureComparer.GetScore().ToString(), listGestureParams, gestureAuth.GetGestureAvgZScore(), gestureAuth.GetMostUniqueParameter(), gestureComparer.PcaScore, gestureComparer.DtwScore);
            }


            return resultString;
        }

        private List<ICompareResult> ConvertParamsList(ArrayList listInput)
        {
            List<ICompareResult> listGestureParams = new List<ICompareResult>();
            for (int idx = 0; idx < listInput.size(); idx++)
            {
                listGestureParams.Add((ICompareResult)listInput.get(idx));
            }

            listGestureParams = listGestureParams.OrderByDescending(o => o.GetName()).ToList();

            return listGestureParams;
        }

        private string CreateGestureLine(string idxComparison, string userName, string templateStoredId, string userNameHack, string templateStoredHackId, string gestureAuthId, string comparisonType, string instruction, string totalScore, List<ICompareResult> listParams, double avgZScore, double mostUniqueParam, double pcaScore, double dtwScore)
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

            AppendWithComma(stringBuilder, avgZScore.ToString());

            AppendWithComma(stringBuilder, mostUniqueParam.ToString());

            AppendWithComma(stringBuilder, pcaScore.ToString());
            AppendWithComma(stringBuilder, dtwScore.ToString());

            for (int idx = 0; idx < GESTURE_NUM_PARAMS; idx++)
            {
                if(idx < listParams.Count)
                {
                    AppendStatParam(stringBuilder, listParams[idx]);
                }
                else
                {
                    AppendStatParam(stringBuilder, null);
                }
            }

            return stringBuilder.ToString();
        }

        private void AppendStatParam(StringBuilder stringBuilder, ICompareResult gestureParam)
        {
            if (gestureParam != null)
            {
                //AppendWithComma(stringBuilder, gestureParam.GetName());
                AppendWithComma(stringBuilder, gestureParam.GetValue().ToString());
                //AppendWithComma(stringBuilder, gestureParam.GetOriginalValue().ToString());
                //AppendWithComma(stringBuilder, gestureParam.GetMean().ToString());
                //AppendWithComma(stringBuilder, gestureParam.GetPopMean().ToString());
                //AppendWithComma(stringBuilder, gestureParam.GetWeight().ToString());
                //AppendWithComma(stringBuilder, gestureParam.GetTemplateZScore().ToString());
                //AppendWithComma(stringBuilder, gestureParam.GetZScore().ToString());
                //AppendWithComma(stringBuilder, gestureParam.GetSD().ToString());
                //AppendWithComma(stringBuilder, gestureParam.GetInternalSD().ToString());
                //AppendWithComma(stringBuilder, gestureParam.GetInternalSdUserOnly().ToString());
            }
            else {
                //AppendWithComma(stringBuilder, string.Empty);
                AppendWithComma(stringBuilder, string.Empty);
                //AppendWithComma(stringBuilder, string.Empty);
                //AppendWithComma(stringBuilder, string.Empty);
                //AppendWithComma(stringBuilder, string.Empty);
                //AppendWithComma(stringBuilder, string.Empty);
                //AppendWithComma(stringBuilder, string.Empty);
                //AppendWithComma(stringBuilder, string.Empty);
                //AppendWithComma(stringBuilder, string.Empty);
                //AppendWithComma(stringBuilder, string.Empty);
                //AppendWithComma(stringBuilder, string.Empty);
            }
        }

        private List<string> CreateStrokeResultStrings(GestureComparer gestureComparer, TemplateExtended templateStored, GestureExtended gestureAuth, string usernameHack, string templateStoredHackId)
        {
            List<string> listResultString = new List<string>();
            string line;

            List<ICompareResult> listStrokeParams;
            StrokeComparer tempStrokeComparer;

            double finalScore;

            for (int idxStrokeComparer = 0; idxStrokeComparer < gestureComparer.GetStrokeComparers().size(); idxStrokeComparer++)
            {
                listStrokeParams = ConvertParamsList(((StrokeComparer)gestureComparer.GetStrokeComparers().get(idxStrokeComparer)).GetResultsSummary().ListCompareResults);

                tempStrokeComparer = (StrokeComparer)gestureComparer.GetStrokeComparers().get(idxStrokeComparer);

                finalScore = tempStrokeComparer.GetScore();
                if(Double.IsNaN(finalScore))
                {
                    finalScore = 0;
                }
 
                line = CreateStrokeLine(mIdxComparison.ToString(), idxStrokeComparer.ToString(), templateStored.Name, templateStored.Id.ToString(), usernameHack, templateStoredHackId, gestureAuth.Id.ToString(), ((StrokeExtended)gestureAuth.ListStrokesExtended.get(idxStrokeComparer)).Id.ToString(), mSimulationType, gestureAuth.Instruction, Math.Round(tempStrokeComparer.GetScore(), 5).ToString(), Math.Round(tempStrokeComparer.GetMinCosineDistance(), 5).ToString(), tempStrokeComparer.DtwAccelerations, tempStrokeComparer.DtwCoordinates, tempStrokeComparer.DtwEvents, tempStrokeComparer.DtwNormalizedCoordinates, tempStrokeComparer.DtwNormalizedCoordinatesSpatialDistance, tempStrokeComparer.DtwSpatialAcceleration, tempStrokeComparer.DtwTemporalAcceleration, tempStrokeComparer.DtwSpatialAccumNormArea, tempStrokeComparer.DtwTemporalAccumNormArea, tempStrokeComparer.DtwSpatialDeltaTeta, tempStrokeComparer.DtwTemporalDeltaTeta, tempStrokeComparer.DtwSpatialRadialAcceleration, tempStrokeComparer.DtwTemporalRadialAcceleration, tempStrokeComparer.DtwSpatialRadialVelocity, tempStrokeComparer.DtwTemporalRadialVelocity, tempStrokeComparer.DtwSpatialRadius, tempStrokeComparer.DtwTemporalRadius, tempStrokeComparer.DtwSpatialTeta, tempStrokeComparer.DtwTemporalTeta, tempStrokeComparer.DtwSpatialVelocity, tempStrokeComparer.DtwTemporalVelocity, tempStrokeComparer.DtwVelocities, tempStrokeComparer.SpatialScoreVelocity, tempStrokeComparer.SpatialScoreAcceleration, tempStrokeComparer.SpatialScoreRadialVelocity, tempStrokeComparer.SpatialScoreRadialAcceleration, tempStrokeComparer.SpatialScoreRadius, tempStrokeComparer.SpatialScoreTeta, tempStrokeComparer.SpatialScoreDeltaTeta, tempStrokeComparer.SpatialScoreAccumulatedNormArea, tempStrokeComparer.TemporalScoreVelocity, tempStrokeComparer.TemporalScoreAcceleration, tempStrokeComparer.TemporalScoreRadialVelocity, tempStrokeComparer.TemporalScoreRadialAcceleration, tempStrokeComparer.TemporalScoreRadius, tempStrokeComparer.TemporalScoreTeta, tempStrokeComparer.TemporalScoreDeltaTeta, tempStrokeComparer.TemporalScoreAccumulatedNormArea, tempStrokeComparer.StrokeSpatialScore, tempStrokeComparer.StrokeDistanceTotalScore, tempStrokeComparer.StrokeDistanceTotalScoreStartToStart, tempStrokeComparer.StrokeDistanceTotalScoreStartToEnd, tempStrokeComparer.StrokeDistanceTotalScoreEndToStart, tempStrokeComparer.StrokeDistanceTotalScoreEndToEnd, listStrokeParams, tempStrokeComparer.AccDiffX, tempStrokeComparer.AccDiffY, tempStrokeComparer.AccDiffZ, tempStrokeComparer.AccDiffTotal, tempStrokeComparer.PcaScore, tempStrokeComparer);
                listResultString.Add(line);
            }

            return listResultString;
        }

        private string CreateStrokeLine(string idxComparison, string idxStroke, string userName, string templateStoredId, string userNameHack, string templateStoredHackId, string gestureAuthId, string strokeAuthId, string comparisonType, string instruction, string totalScore, string strokeCosineDistance, double DtwAccelerations, double DtwCoordinates, double DtwEvents, double DtwNormalizedCoordinates, double DtwNormalizedCoordinatesSpatialDistance, double DtwSpatialAccelerationDistance, double DtwSpatialAccelerationTime, double DtwSpatialAccumNormAreaDistance, double DtwSpatialAccumNormAreaTime, double DtwSpatialDeltaTetaDistance, double DtwSpatialDeltaTetaTime, double DtwSpatialRadialAccelerationDistance, double DtwSpatialRadialAccelerationTime, double DtwSpatialRadialVelocityDistance, double DtwSpatialRadialVelocityTime, double DtwSpatialRadiusDistance, double DtwSpatialRadiusTime, double DtwSpatialTetaDistance, double DtwSpatialTetaTime, double DtwSpatialVelocityDistance, double DtwSpatialVelocityTime, double DtwVelocities, double SpatialScoreDistanceVelocity, double SpatialScoreDistanceAcceleration, double SpatialScoreDistanceRadialVelocity, double SpatialScoreDistanceRadialAcceleration, double SpatialScoreDistanceRadius, double SpatialScoreDistanceTeta, double SpatialScoreDistanceDeltaTeta, double SpatialScoreDistanceAccumulatedNormArea, double SpatialScoreTimeVelocity, double SpatialScoreTimeAcceleration, double SpatialScoreTimeRadialVelocity, double SpatialScoreTimeRadialAcceleration, double SpatialScoreTimeRadius, double SpatialScoreTimeTeta, double SpatialScoreTimeDeltaTeta, double SpatialScoreTimeAccumulatedNormArea, double totalSpatialTemporalScore, double StrokeDistanceTotalScore, double StrokeDistanceTotalScoreStartToStart, double StrokeDistanceTotalScoreStartToEnd, double StrokeDistanceTotalScoreEndToStart, double StrokeDistanceTotalScoreEndToEnd, List<ICompareResult> listParams, double accMovDiffX, double accMovDiffY, double accMovDiffZ, double accMovDiffTotal, double pcaScore, StrokeComparer tempStrokeComparer)
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

            AppendWithComma(stringBuilder, Math.Round(accMovDiffX, 5).ToString());
            AppendWithComma(stringBuilder, Math.Round(accMovDiffY, 5).ToString());
            AppendWithComma(stringBuilder, Math.Round(accMovDiffZ, 5).ToString());
            AppendWithComma(stringBuilder, Math.Round(accMovDiffTotal, 5).ToString());

            AppendWithComma(stringBuilder, Math.Round(pcaScore, 5).ToString());

            AppendWithComma(stringBuilder, Math.Round(tempStrokeComparer.RadialVelocityDiff, 5).ToString());
            AppendWithComma(stringBuilder, Math.Round(tempStrokeComparer.RadialAccelerationDiff, 5).ToString());

            AppendWithComma(stringBuilder, Math.Round(tempStrokeComparer.DtwTemporalVelocity0, 5).ToString());
            AppendWithComma(stringBuilder, Math.Round(tempStrokeComparer.DtwTemporalVelocity1, 5).ToString());
            AppendWithComma(stringBuilder, Math.Round(tempStrokeComparer.DtwTemporalVelocity2, 5).ToString());
            AppendWithComma(stringBuilder, Math.Round(tempStrokeComparer.DtwTemporalVelocity3, 5).ToString());
            AppendWithComma(stringBuilder, Math.Round(tempStrokeComparer.DtwTemporalVelocity4, 5).ToString());
            AppendWithComma(stringBuilder, Math.Round(tempStrokeComparer.DtwTemporalVelocity5, 5).ToString());
            AppendWithComma(stringBuilder, Math.Round(tempStrokeComparer.DtwTemporalVelocity6, 5).ToString());
            AppendWithComma(stringBuilder, Math.Round(tempStrokeComparer.DtwTemporalVelocity7, 5).ToString());
            AppendWithComma(stringBuilder, Math.Round(tempStrokeComparer.DtwTemporalVelocity8, 5).ToString());
            AppendWithComma(stringBuilder, Math.Round(tempStrokeComparer.DtwTemporalVelocity9, 5).ToString());
            AppendWithComma(stringBuilder, Math.Round(tempStrokeComparer.DtwTemporalVelocity10, 5).ToString());
            AppendWithComma(stringBuilder, Math.Round(tempStrokeComparer.DtwTemporalVelocity11, 5).ToString());
            AppendWithComma(stringBuilder, Math.Round(tempStrokeComparer.DtwTemporalVelocity12, 5).ToString());
            AppendWithComma(stringBuilder, Math.Round(tempStrokeComparer.DtwTemporalVelocity13, 5).ToString());
            AppendWithComma(stringBuilder, Math.Round(tempStrokeComparer.DtwTemporalVelocity14, 5).ToString());
            AppendWithComma(stringBuilder, Math.Round(tempStrokeComparer.DtwTemporalVelocity15, 5).ToString());

            for (int idx = 0; idx < STROKE_NUM_PARAMS; idx++)
            {
                if (idx < listParams.Count) {
                    AppendStatParam(stringBuilder, listParams[idx]);
                }
                else
                {
                    AppendStatParam(stringBuilder, null);
                }
            }

            return stringBuilder.ToString();
        }        

        private void AppendWithComma(StringBuilder stringBuilder, string input)
        {
            stringBuilder.Append(input);
            stringBuilder.Append(",");
        }

        protected void RunFullSimulation()
        {            
            Init();
            IEnumerable<ModelTemplate> modelTemplates;

            mListMongo = UtilsNewDB.GetTemplates();
            int totalNumbRecords = (int)mListMongo.Count();
            mIsFinished = false;

            mIdxComparison = 1;
            int limit = 50;
            int skip = 0;

            UserContainerMgr userContainerMgr = new UserContainerMgr();

            try
            {                
                while (!mIsFinished)
                {
                    modelTemplates = mListMongo.FindAll().SetLimit(limit).SetSkip(skip);

                    foreach (ModelTemplate template in modelTemplates)
                    {
                        switch(template.State)
                        {
                            case "Register":
                                userContainerMgr.AddTemplateRegister(template);
                                break;
                            case "Authenticate":
                                userContainerMgr.AddTemplateAuth(template);
                                break;
                            case "Hack":
                                userContainerMgr.AddTemplateHack(template);
                                break;
                        }
                    }                    

                    skip += limit;

                    if (totalNumbRecords <= skip)
                    {
                        mIsFinished = true;                        
                    }
                }

                if (mSimulationType.CompareTo(SIMULATOR_TYPE_NAIVE_HACK) != 0)
                {
                    RunFullComparisons(userContainerMgr);
                }
                else
                {
                    RunFullComparisonsNaiveHack(userContainerMgr);
                }

            }
            catch (Exception exc)
            {
                MessageBox.Show(string.Format("Error: {0}", exc.Message));
            }
        }      

        private void CleanCommonGestures(TemplateExtended baseTemplate)
        {
            for (int idxGesture = baseTemplate.ListGestureExtended.size() - 1; idxGesture >= 0; idxGesture--)
            {
                if (((GestureExtended)baseTemplate.ListGestureExtended.get(idxGesture)).Instruction.CompareTo("SLETTER") == 0 ||
                    ((GestureExtended)baseTemplate.ListGestureExtended.get(idxGesture)).Instruction.CompareTo("MLETTER") == 0 ||
                    ((GestureExtended)baseTemplate.ListGestureExtended.get(idxGesture)).Instruction.CompareTo("ALETTER") == 0)
                {
                    baseTemplate.ListGestureExtended.remove(idxGesture);
                }
            }

            //int numToRemove = 2;
            //Dictionary<String, bool> dictRemovedGestures;

            //string tempInstruction;
            //List<GestureExtended> listGestures = new List<GestureExtended>();

            //for (int idxGesture = 0; idxGesture < baseTemplate.ListGestureExtended.size(); idxGesture++)
            //{
            //    listGestures.Add((GestureExtended)baseTemplate.ListGestureExtended.get(idxGesture));
            //}

            //listGestures = listGestures.OrderBy(o => o.GetGestureAvgZScore()).ToList();
            //dictRemovedGestures = new Dictionary<string, bool>();

            //for (int idxRemove = 0; idxRemove < numToRemove; idxRemove++)
            //{
            //    tempInstruction = listGestures[0].Instruction;
            //    for (int idx = listGestures.Count - 1; idx >= 0; idx--)
            //    {
            //        if (listGestures[idx].Instruction.CompareTo(tempInstruction) == 0)
            //        {
            //            listGestures.RemoveAt(idx);
            //        }
            //    }
            //    for (int idx = baseTemplate.ListGestureExtended.size() - 1; idx >= 0; idx--)
            //    {
            //        if (((GestureExtended)baseTemplate.ListGestureExtended.get(idx)).Instruction.CompareTo(tempInstruction) == 0)
            //        {
            //            baseTemplate.ListGestureExtended.remove(idx);
            //        }
            //    }
            //}
        }

        private void CleanCommonGestures1(TemplateExtended baseTemplate)
        {
            List<GestureExtended> listGestures = new List<GestureExtended>();
            for (int idxGesture = baseTemplate.ListGestureExtended.size() - 1; idxGesture >= 0; idxGesture--)
            {
                if (((GestureExtended)baseTemplate.ListGestureExtended.get(idxGesture)).GetMostUniqueParameter() < 1.5)
                {
                    baseTemplate.ListGestureExtended.remove(idxGesture);
                }
            }
        }

        private void RunFullComparisonsNaiveHack(UserContainerMgr userContainerMgr)
        {
            try
            {
                mStreamWriterGestures = InitStreamWriterResultGestures();
                mStreamWriterStrokes = InitStreamWriterResultStrokes();
                mStreamWriterLog = InitStreamWriterLog();

                InitGestureHeader(mStreamWriterGestures);
                InitStrokesHeader(mStreamWriterStrokes);

                int totalNumbRecords = userContainerMgr.GetUserContainers().Values.Count;
                totalNumbRecords = totalNumbRecords * totalNumbRecords;
                int currentRecord = 0;

                UpdateProgress(totalNumbRecords, currentRecord);
                String numComparisons;

                foreach (UserContainer userContainerBase in userContainerMgr.GetUserContainers().Values)
                {
                    try
                    {
                        TemplateExtended baseTemplate = UtilsTemplateConverter.ConvertTemplateNew(userContainerBase.TemplateRegistration);
                        CleanCommonGestures(baseTemplate);

                        double tempScore;
                        List<double> listTempScores;
                        TemplateExtended tempTemplateAuth;

                        List<ModelTemplate> listTempModelTemplates;

                        foreach (UserContainer userContainerAuth in userContainerMgr.GetUserContainers().Values)
                        {
                            listTempModelTemplates = userContainerAuth.ListTemplatesAuthentication;

                            if (userContainerAuth.Name.CompareTo(userContainerBase.Name) != 0)
                            {
                                for (int idxAuth = 0; idxAuth < listTempModelTemplates.Count; idxAuth++)
                                {
                                    tempTemplateAuth = UtilsTemplateConverter.ConvertTemplateNew(listTempModelTemplates[idxAuth]);
                                    listTempScores = new List<double>();
                                    for (int idxGesture = 0; idxGesture < tempTemplateAuth.ListGestureExtended.size(); idxGesture++)
                                    {
                                        tempScore = CompareGesturesToStrings(baseTemplate, (GestureExtended)tempTemplateAuth.ListGestureExtended.get(idxGesture), tempTemplateAuth);
                                        if (tempScore != -1)
                                        {
                                            mIdxComparison++;
                                            listTempScores.Add(tempScore);

                                            numComparisons = String.Format("Comparisons: {0}", mIdxComparison.ToString());
                                            this.lblComparisons.Invoke(new MethodInvoker(() => this.lblComparisons.Text = numComparisons));
                                        }
                                    }
                                }
                            }

                            currentRecord++;
                            UpdateProgress(totalNumbRecords, currentRecord);
                        }                        
                    }
                    catch (Exception exc)
                    {
                        string msg = exc.Message;
                    }

                    mStreamWriterGestures.Flush();
                    mStreamWriterStrokes.Flush();
                    mStreamWriterLog.Flush();
                }

                mStreamWriterGestures.Close();
                mStreamWriterStrokes.Close();
                mStreamWriterLog.Close();

                EnableButtons();

                this.lblEndTime.Invoke(new MethodInvoker(() => this.lblEndTime.Text = DateTime.Now.ToLongTimeString()));
                UpdateProgress(totalNumbRecords, currentRecord);
            }
            catch (Exception exc)
            {
                MessageBox.Show(string.Format("Error: {0}", exc.Message));
            }
        }

        private void RunFullComparisons(UserContainerMgr userContainerMgr)
        {
            try
            {
                mStreamWriterGestures = InitStreamWriterResultGestures();
                mStreamWriterStrokes = InitStreamWriterResultStrokes();
                mStreamWriterLog = InitStreamWriterLog();

                InitGestureHeader(mStreamWriterGestures);
                InitStrokesHeader(mStreamWriterStrokes);

                int totalNumbRecords = userContainerMgr.GetUserContainers().Values.Count;
                int currentRecord = 0;

                UpdateProgress(totalNumbRecords, currentRecord);

                foreach (UserContainer userContainer in userContainerMgr.GetUserContainers().Values)
                {
                    try
                    {
                        TemplateExtended baseTemplate = UtilsTemplateConverter.ConvertTemplateNew(userContainer.TemplateRegistration);
                        CleanCommonGestures(baseTemplate);

                        double tempScore;
                        List<double> listTempScores;
                        TemplateExtended tempTemplateAuth;

                        List<ModelTemplate> listTempModelTemplates;

                        if (mSimulationType.CompareTo(SIMULATOR_TYPE_AUTH) == 0)
                        {
                            listTempModelTemplates = userContainer.ListTemplatesAuthentication;
                        }
                        else
                        {
                            listTempModelTemplates = userContainer.ListTemplatesHack;
                        }

                        for (int idxAuth = 0; idxAuth < listTempModelTemplates.Count; idxAuth++)
                        {   
                            tempTemplateAuth = UtilsTemplateConverter.ConvertTemplateNew(listTempModelTemplates[idxAuth]);
                            listTempScores = new List<double>();
                            for (int idxGesture = 0; idxGesture < tempTemplateAuth.ListGestureExtended.size(); idxGesture++)
                            {
                                tempScore = CompareGesturesToStrings(baseTemplate, (GestureExtended)tempTemplateAuth.ListGestureExtended.get(idxGesture), tempTemplateAuth);
                                if (tempScore != -1) {
                                    mIdxComparison++;
                                    listTempScores.Add(tempScore);
                                }
                            }
                        }

                        currentRecord++;
                        UpdateProgress(totalNumbRecords, currentRecord);
                    }
                    catch (Exception exc)
                    {
                        string msg = exc.Message;
                    }

                    mStreamWriterGestures.Flush();
                    mStreamWriterStrokes.Flush();
                    mStreamWriterLog.Flush();
                }

                mStreamWriterGestures.Close();
                mStreamWriterStrokes.Close();
                mStreamWriterLog.Close();

                EnableButtons();

                this.lblEndTime.Invoke(new MethodInvoker(() => this.lblEndTime.Text = DateTime.Now.ToLongTimeString()));               
                UpdateProgress(totalNumbRecords, currentRecord);
            }
            catch (Exception exc)
            {
                MessageBox.Show(string.Format("Error: {0}", exc.Message));
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            mTotalNumComparisons = 1;
            lblStartTime.Text = DateTime.Now.ToLongTimeString();
            mSimulationType = SIMULATOR_TYPE_HACK;
            lblType.Text = SIMULATOR_TYPE_HACK;
            Task task = Task.Run((Action)RunFullSimulation);
            DisableButtons();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            mTotalNumComparisons = 1;
            lblStartTime.Text = DateTime.Now.ToLongTimeString();
            mSimulationType = SIMULATOR_TYPE_AUTH;
            lblType.Text = SIMULATOR_TYPE_AUTH;
            Task task = Task.Run((Action)RunFullSimulation);
            DisableButtons();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            mTotalNumComparisons = 1;
            lblStartTime.Text = DateTime.Now.ToLongTimeString();
            mSimulationType = SIMULATOR_TYPE_DUPLICATE;
            lblType.Text = SIMULATOR_TYPE_DUPLICATE;
            Task task = Task.Run((Action)DuplicateDB);
            DisableButtons();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            mTotalNumComparisons = 1;
            lblStartTime.Text = DateTime.Now.ToLongTimeString();
            mSimulationType = SIMULATOR_TYPE_NAIVE_HACK;
            lblType.Text = SIMULATOR_TYPE_NAIVE_HACK;
            Task task = Task.Run((Action)RunFullSimulation);
            DisableButtons();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            DisableButtons();

            mTotalNumComparisons = 1;
            lblStartTime.Text = DateTime.Now.ToLongTimeString();
            mSimulationType = SIMULATOR_TYPE_EXPORT;
            lblType.Text = SIMULATOR_TYPE_EXPORT;
            Task task = Task.Run((Action)ExportToCSV);
        }

        private void ExportToCSV()
        {
            Init();
            IEnumerable<ModelTemplate> modelTemplates;

            TemplateExtended tempTemplate = null;

            List<GestureExtended> gesturesTestSample;

            mListMongo = UtilsNewDB.GetTemplates();
            mIsFinished = false;

            mIdxComparison = 1;
            int currentRecord = 0;
            int totalNumbRecords = (int)mListMongo.Count();
            int limit = 50;
            int skip = 0;

            try
            {
                InitCsvStreams();

                InitCSVHeaderTemplates(mStreamWriterCsvTemplates);
                InitCSVHeaderGestures(mStreamWriterCsvGestures);
                InitCSVHeaderStrokes(mStreamWriterCsvStrokes);
                InitCSVHeaderEvents(mStreamWriterCsvMotionEventsRaw);
                InitCSVHeaderEvents(mStreamWriterCsvMotionEventsSpatial);
                InitCSVHeaderEvents(mStreamWriterCsvMotionEventsTemporal);

                UpdateProgress(totalNumbRecords, currentRecord);
                while (!mIsFinished)
                {
                    modelTemplates = mListMongo.FindAll().SetLimit(limit).SetSkip(skip);

                    foreach (ModelTemplate template in modelTemplates)
                    {
                        tempTemplate = UtilsTemplateConverter.ConvertTemplateNew(template);

                        ExportTemplateToCSV(tempTemplate, currentRecord, template);

                        currentRecord++;
                        UpdateProgress(totalNumbRecords, currentRecord);
                    }

                    FlushCsvStreams();

                    skip += limit;

                    if (totalNumbRecords <= skip)
                    {
                        mIsFinished = true;

                        CloseCsvStreams();
                        EnableButtons();

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

        private void CloseCsvStreams()
        {
            mStreamWriterCsvTemplates.Close();
            mStreamWriterCsvGestures.Close();
            mStreamWriterCsvStrokes.Close();
            mStreamWriterCsvMotionEventsRaw.Close();
            mStreamWriterCsvMotionEventsSpatial.Close();
            mStreamWriterCsvMotionEventsTemporal.Close();
            mStreamWriterLog.Close();            
        }

        private void FlushCsvStreams()
        {
            mStreamWriterCsvTemplates.Flush();
            mStreamWriterCsvGestures.Flush();
            mStreamWriterCsvStrokes.Flush();
            mStreamWriterCsvMotionEventsRaw.Flush();
            mStreamWriterCsvMotionEventsSpatial.Flush();
            mStreamWriterCsvMotionEventsTemporal.Flush();
            mStreamWriterLog.Flush();
        }

        private void InitCsvStreams()
        {
            mStreamWriterCsvTemplates = InitStreamWriterCSV("Templates");
            mStreamWriterCsvGestures = InitStreamWriterCSV("Gestures");
            mStreamWriterCsvStrokes = InitStreamWriterCSV("Strokes");
            mStreamWriterCsvMotionEventsRaw = InitStreamWriterCSV("EventsRaw");
            mStreamWriterCsvMotionEventsTemporal = InitStreamWriterCSV("EventsTemporal");
            mStreamWriterCsvMotionEventsSpatial = InitStreamWriterCSV("EventsSpatial");
            mStreamWriterLog = InitStreamWriterLog();
        }
 
        private void InitCSVHeaderEvents(StreamWriter streamWriter)
        {
            StringBuilder stringBuilder = new StringBuilder();
            AppendWithComma(stringBuilder, "Name");
            AppendWithComma(stringBuilder, "Instruction");
            AppendWithComma(stringBuilder, "State");

            AppendWithComma(stringBuilder, "Template Index");
            AppendWithComma(stringBuilder, "Gesture Index");
            AppendWithComma(stringBuilder, "Stroke Index");
            AppendWithComma(stringBuilder, "Event Index");

            AppendWithComma(stringBuilder, "Template ID");
            AppendWithComma(stringBuilder, "Gesture ID");
            AppendWithComma(stringBuilder, "Stroke ID");
            AppendWithComma(stringBuilder, "Event ID");

            AppendWithComma(stringBuilder, "EventTime");
            AppendWithComma(stringBuilder, "GestureTime");

            AppendWithComma(stringBuilder, "Xpixel");
            AppendWithComma(stringBuilder, "Ypixel");

            AppendWithComma(stringBuilder, "Xmm");
            AppendWithComma(stringBuilder, "Ymm");

            AppendWithComma(stringBuilder, "Xnormalized");
            AppendWithComma(stringBuilder, "Ynormalized");

            AppendWithComma(stringBuilder, "Velocity");
            AppendWithComma(stringBuilder, "VelocityX");
            AppendWithComma(stringBuilder, "VelocityY");

            AppendWithComma(stringBuilder, "Acceleration");

            AppendWithComma(stringBuilder, "AccelerometerX");
            AppendWithComma(stringBuilder, "AccelerometerY");
            AppendWithComma(stringBuilder, "AccelerometerZ");

            AppendWithComma(stringBuilder, "GyroX");
            AppendWithComma(stringBuilder, "GyroY");
            AppendWithComma(stringBuilder, "GyroZ");

            AppendWithComma(stringBuilder, "Angle");
            AppendWithComma(stringBuilder, "AngleDiff");

            AppendWithComma(stringBuilder, "IsEndOfStroke");
            AppendWithComma(stringBuilder, "IsStartOfStroke");

            AppendWithComma(stringBuilder, "Pressure");
            AppendWithComma(stringBuilder, "TouchSurface");

            AppendWithComma(stringBuilder, "RadialVelocity");
            AppendWithComma(stringBuilder, "RadialAcceleration");

            AppendWithComma(stringBuilder, "Radius");

            AppendWithComma(stringBuilder, "Teta");
            AppendWithComma(stringBuilder, "DeltaTeta");

            AppendWithComma(stringBuilder, "AccumulatedNormalizedArea");

            //AppendWithComma(stringBuilder, "IsHistory");

            streamWriter.WriteLine(stringBuilder.ToString());
        }

        private void InitCSVHeaderStrokes(StreamWriter streamWriter)
        {
            StringBuilder stringBuilder = new StringBuilder();
            AppendWithComma(stringBuilder, "Name");
            AppendWithComma(stringBuilder, "Instruction");
            AppendWithComma(stringBuilder, "State");

            AppendWithComma(stringBuilder, "Template Index");
            AppendWithComma(stringBuilder, "Gesture Index");
            AppendWithComma(stringBuilder, "Stroke Index");

            AppendWithComma(stringBuilder, "Template ID");
            AppendWithComma(stringBuilder, "Gesture ID");
            AppendWithComma(stringBuilder, "Stroke ID");

            AppendWithComma(stringBuilder, "Length Pixel");

            AppendWithComma(stringBuilder, "IsPoint");

            AppendWithComma(stringBuilder, "PointMaxXMM");
            AppendWithComma(stringBuilder, "PointMaxYMM");
            AppendWithComma(stringBuilder, "PointMinXMM");
            AppendWithComma(stringBuilder, "PointMinYMM");

            AppendWithComma(stringBuilder, "StrokeCenterXpixel");
            AppendWithComma(stringBuilder, "StrokeCenterYpixel");

            AppendWithComma(stringBuilder, "StrokeDistanceStartToStart");
            AppendWithComma(stringBuilder, "StrokeDistanceStartToEnd");
            AppendWithComma(stringBuilder, "StrokeDistanceEndToStart");
            AppendWithComma(stringBuilder, "StrokeDistanceEndToEnd");

            AppendWithComma(stringBuilder, "StrokeTransitionTime");

            AppendWithComma(stringBuilder, "Num Events");

            AppendWithComma(stringBuilder, "Length MM");

            AppendWithComma(stringBuilder, "ShapeArea");
            AppendWithComma(stringBuilder, "ShapeAreaMinXMinY");

            AppendWithComma(stringBuilder, "StrokeAverageAcceleration");
            AppendWithComma(stringBuilder, "StrokeMaxAcceleration");

            AppendWithComma(stringBuilder, "StrokeAverageVelocity");
            AppendWithComma(stringBuilder, "StrokeMaxVelocity");
            AppendWithComma(stringBuilder, "StrokeMidVelocity");

            AppendWithComma(stringBuilder, "MiddlePressure");
            AppendWithComma(stringBuilder, "MiddleSurface");

            AppendWithComma(stringBuilder, "StrokeTimeInterval");

            streamWriter.WriteLine(stringBuilder.ToString());
        }

        private void InitCSVHeaderGestures(StreamWriter streamWriter)
        {
            StringBuilder stringBuilder = new StringBuilder();
            AppendWithComma(stringBuilder, "Name");
            AppendWithComma(stringBuilder, "State");

            AppendWithComma(stringBuilder, "Template Index");
            AppendWithComma(stringBuilder, "Gesture Index");

            AppendWithComma(stringBuilder, "Template ID");
            AppendWithComma(stringBuilder, "Gesture ID");

            AppendWithComma(stringBuilder, "Instruction");
            AppendWithComma(stringBuilder, "Num Strokes");

            streamWriter.WriteLine(stringBuilder.ToString());
        }

        private void InitCSVHeaderTemplates(StreamWriter streamWriter)
        {
            StringBuilder stringBuilder = new StringBuilder();
            AppendWithComma(stringBuilder, "Name");

            AppendWithComma(stringBuilder, "Template Index");

            AppendWithComma(stringBuilder, "Template ID");

            AppendWithComma(stringBuilder, "State");
            AppendWithComma(stringBuilder, "Num Gestures");

            AppendWithComma(stringBuilder, "Xdpi");
            AppendWithComma(stringBuilder, "Ydpi");

            streamWriter.WriteLine(stringBuilder.ToString());
        }

        private void ExportTemplateToCSV(TemplateExtended tempTemplate, int templateIdx, ModelTemplate templateRaw)
        {
            WriteTemplate(tempTemplate, templateIdx, templateRaw.State, templateRaw.Xdpi, templateRaw.Ydpi);
        }

        private void WriteTemplate(TemplateExtended inputTemplate, int idxTemplate, string recordType, double xdpi, double ydpi)
        {
            StringBuilder stringBuilder = new StringBuilder();

            AppendWithComma(stringBuilder, inputTemplate.Name);

            AppendWithComma(stringBuilder, idxTemplate.ToString());

            AppendWithComma(stringBuilder, inputTemplate.Id.ToString());
            
            AppendWithComma(stringBuilder, recordType);
            AppendWithComma(stringBuilder, inputTemplate.ListGestureExtended.size().ToString());

            AppendWithComma(stringBuilder, xdpi.ToString());
            AppendWithComma(stringBuilder, ydpi.ToString());

            mStreamWriterCsvTemplates.WriteLine(stringBuilder.ToString());

            GestureExtended tempGesture;
            for(int idxGesture = 0; idxGesture < inputTemplate.ListGestureExtended.size(); idxGesture++)
            {
                tempGesture = (GestureExtended) inputTemplate.ListGestureExtended.get(idxGesture);
                WriteGesture(inputTemplate, tempGesture, idxTemplate, idxGesture, recordType);
            }
        }

        private void WriteGesture(TemplateExtended inputTemplate, GestureExtended inputGesture, int idxTemplate, int idxGesture, string recordType)
        {
            StringBuilder stringBuilder = new StringBuilder();

            AppendWithComma(stringBuilder, inputTemplate.Name);

            AppendWithComma(stringBuilder, idxTemplate.ToString());
            AppendWithComma(stringBuilder, idxGesture.ToString());

            AppendWithComma(stringBuilder, inputTemplate.Id.ToString());
            AppendWithComma(stringBuilder, inputGesture.Id.ToString());
            
            AppendWithComma(stringBuilder, inputGesture.Instruction);
            AppendWithComma(stringBuilder, inputGesture.ListStrokesExtended.size().ToString());

            mStreamWriterCsvGestures.WriteLine(stringBuilder.ToString());

            StrokeExtended tempStroke;
            double gestureStartTime = GetGestureStartTime(inputGesture); 
            for (int idxStroke = 0; idxStroke < inputGesture.ListStrokesExtended.size(); idxStroke++)
            {
                tempStroke = (StrokeExtended)inputGesture.ListStrokesExtended.get(idxStroke);
                WriteStroke(inputTemplate, inputGesture, tempStroke, idxTemplate, idxGesture, idxStroke, ((Stroke)inputGesture.ListStrokes.get(idxStroke)).Length, gestureStartTime, recordType);
            }
        }

        private double GetGestureStartTime(GestureExtended inputGesture)
        {
            return ((MotionEventExtended)((StrokeExtended)inputGesture.ListStrokesExtended.get(0)).ListEventsExtended.get(0)).EventTime;
        }

        private void WriteStroke(TemplateExtended inputTemplate, GestureExtended inputGesture, StrokeExtended inputStroke, int idxTemplate, int idxGesture, int idxStroke, double length, double gestureStartTime, string recordType)
        {
            StringBuilder stringBuilder = new StringBuilder();

            AppendWithComma(stringBuilder, inputTemplate.Name);
            AppendWithComma(stringBuilder, inputGesture.Instruction);
            AppendWithComma(stringBuilder, recordType);

            AppendWithComma(stringBuilder, idxTemplate.ToString());
            AppendWithComma(stringBuilder, idxGesture.ToString());
            AppendWithComma(stringBuilder, idxStroke.ToString());

            AppendWithComma(stringBuilder, inputTemplate.Id.ToString());
            AppendWithComma(stringBuilder, inputGesture.Id.ToString());
            AppendWithComma(stringBuilder, inputStroke.Id.ToString());

            AppendWithComma(stringBuilder, length.ToString());

            AppendWithComma(stringBuilder, inputStroke.IsPoint.ToString());

            AppendWithComma(stringBuilder, inputStroke.PointMaxXMM.ToString());
            AppendWithComma(stringBuilder, inputStroke.PointMaxYMM.ToString());
            AppendWithComma(stringBuilder, inputStroke.PointMinXMM.ToString());
            AppendWithComma(stringBuilder, inputStroke.PointMinYMM.ToString());

            AppendWithComma(stringBuilder, inputStroke.StrokeCenterXpixel.ToString());
            AppendWithComma(stringBuilder, inputStroke.StrokeCenterYpixel.ToString());

            AppendWithComma(stringBuilder, inputStroke.StrokeDistanceStartToStart.ToString());
            AppendWithComma(stringBuilder, inputStroke.StrokeDistanceStartToEnd.ToString());
            AppendWithComma(stringBuilder, inputStroke.StrokeDistanceEndToStart.ToString());
            AppendWithComma(stringBuilder, inputStroke.StrokeDistanceEndToEnd.ToString());

            AppendWithComma(stringBuilder, inputStroke.StrokeTransitionTime.ToString());

            AppendWithComma(stringBuilder, inputStroke.ListEventsExtended.size().ToString());

            AppendWithComma(stringBuilder, inputStroke.StrokePropertiesObj.LengthMM.ToString());

            AppendWithComma(stringBuilder, inputStroke.ShapeDataObj.ShapeArea.ToString());
            AppendWithComma(stringBuilder, inputStroke.ShapeDataObj.ShapeAreaMinXMinY.ToString());

            AppendWithComma(stringBuilder, inputStroke.StrokeAverageAcceleration.ToString());
            AppendWithComma(stringBuilder, inputStroke.StrokeMaxAcceleration.ToString());

            AppendWithComma(stringBuilder, inputStroke.StrokeAverageVelocity.ToString());
            AppendWithComma(stringBuilder, inputStroke.StrokeMaxVelocity.ToString());
            AppendWithComma(stringBuilder, inputStroke.StrokeMidVelocity.ToString());

            AppendWithComma(stringBuilder, inputStroke.MiddlePressure.ToString());
            AppendWithComma(stringBuilder, inputStroke.MiddleSurface.ToString());

            AppendWithComma(stringBuilder, inputStroke.StrokeTimeInterval.ToString());
            
            mStreamWriterCsvStrokes.WriteLine(stringBuilder.ToString());

            MotionEventExtended tempEvent;
            for (int idxEvent = 0; idxEvent < inputStroke.ListEventsExtended.size() - 1; idxEvent++)
            {
                tempEvent = (MotionEventExtended)inputStroke.ListEventsExtended.get(idxEvent);
                WriteEvent(inputTemplate, inputGesture, inputStroke, tempEvent, EVENT_TYPE_RAW, idxTemplate, idxGesture, idxStroke, idxEvent, gestureStartTime, recordType);
            }

            for (int idxEvent = 0; idxEvent < inputStroke.ListEventsSpatialExtended.size() - 1; idxEvent++)
            {
                tempEvent = (MotionEventExtended)inputStroke.ListEventsSpatialExtended.get(idxEvent);
                WriteEvent(inputTemplate, inputGesture, inputStroke, tempEvent, EVENT_TYPE_SPATIAL, idxTemplate, idxGesture, idxStroke, idxEvent, gestureStartTime, recordType);
            }

            for (int idxEvent = 0; idxEvent < inputStroke.ListEventsTemporalExtended.size() - 1; idxEvent++)
            {
                tempEvent = (MotionEventExtended)inputStroke.ListEventsTemporalExtended.get(idxEvent);
                WriteEvent(inputTemplate, inputGesture, inputStroke, tempEvent, EVENT_TYPE_TEMPORAL, idxTemplate, idxGesture, idxStroke, idxEvent, gestureStartTime, recordType);
            }
        }

        private void WriteEvent(TemplateExtended inputTemplate, GestureExtended inputGesture, StrokeExtended inputStroke, MotionEventExtended inputEvent, string eventType, int idxTemplate, int idxGesture, int idxStroke, int idxEvent, double gestureStartTime, string recordType)
        {
            StringBuilder stringBuilder = new StringBuilder();

            AppendWithComma(stringBuilder, inputTemplate.Name);
            AppendWithComma(stringBuilder, inputGesture.Instruction);
            AppendWithComma(stringBuilder, recordType);

            AppendWithComma(stringBuilder, idxTemplate.ToString());
            AppendWithComma(stringBuilder, idxGesture.ToString());
            AppendWithComma(stringBuilder, idxStroke.ToString());
            AppendWithComma(stringBuilder, idxEvent.ToString());

            AppendWithComma(stringBuilder, inputTemplate.Id.ToString());
            AppendWithComma(stringBuilder, inputGesture.Id.ToString());
            AppendWithComma(stringBuilder, inputStroke.Id.ToString());
            AppendWithComma(stringBuilder, inputEvent.Id.ToString());

            AppendWithComma(stringBuilder, inputEvent.EventTime.ToString());
            AppendWithComma(stringBuilder, (inputEvent.EventTime - gestureStartTime).ToString());

            AppendWithComma(stringBuilder, inputEvent.XpixelRaw.ToString());
            AppendWithComma(stringBuilder, inputEvent.YpixelRaw.ToString());

            AppendWithComma(stringBuilder, inputEvent.Xmm.ToString());
            AppendWithComma(stringBuilder, inputEvent.Ymm.ToString());

            AppendWithComma(stringBuilder, inputEvent.Xnormalized.ToString());
            AppendWithComma(stringBuilder, inputEvent.Ynormalized.ToString());

            AppendWithComma(stringBuilder, Math.Round(inputEvent.Velocity, NUM_DECIMALS).ToString());
            AppendWithComma(stringBuilder, Math.Round(inputEvent.VelocityX, NUM_DECIMALS).ToString());
            AppendWithComma(stringBuilder, Math.Round(inputEvent.VelocityY, NUM_DECIMALS).ToString());

            AppendWithComma(stringBuilder, Math.Round(inputEvent.Acceleration, NUM_DECIMALS).ToString());

            AppendWithComma(stringBuilder, inputEvent.AccelerometerX().ToString());
            AppendWithComma(stringBuilder, inputEvent.AccelerometerY().ToString());
            AppendWithComma(stringBuilder, inputEvent.AccelerometerZ().ToString());

            AppendWithComma(stringBuilder, inputEvent.GyroX().ToString());
            AppendWithComma(stringBuilder, inputEvent.GyroY().ToString());
            AppendWithComma(stringBuilder, inputEvent.GyroZ().ToString());

            AppendWithComma(stringBuilder, inputEvent.Angle.ToString());
            AppendWithComma(stringBuilder, inputEvent.AngleDiff.ToString());

            AppendWithComma(stringBuilder, inputEvent.IsEndOfStroke.ToString());
            AppendWithComma(stringBuilder, inputEvent.IsStartOfStroke.ToString());

            AppendWithComma(stringBuilder, Math.Round(inputEvent.Pressure, NUM_DECIMALS).ToString());
            AppendWithComma(stringBuilder, Math.Round(inputEvent.TouchSurface, NUM_DECIMALS).ToString());

            AppendWithComma(stringBuilder, Math.Round(inputEvent.RadialVelocity, NUM_DECIMALS).ToString());
            AppendWithComma(stringBuilder, Math.Round(inputEvent.RadialAcceleration, NUM_DECIMALS).ToString());

            AppendWithComma(stringBuilder, Math.Round(inputEvent.Radius, NUM_DECIMALS).ToString());

            AppendWithComma(stringBuilder, Math.Round(inputEvent.Teta, NUM_DECIMALS).ToString());
            AppendWithComma(stringBuilder, Math.Round(inputEvent.DeltaTeta, NUM_DECIMALS).ToString());

            AppendWithComma(stringBuilder, Math.Round(inputEvent.AccumulatedNormalizedArea, NUM_DECIMALS).ToString());

            //AppendWithComma(stringBuilder, inputEvent.IsHistory.ToString());

            switch (eventType) {
                case EVENT_TYPE_RAW:
                    mStreamWriterCsvMotionEventsRaw.WriteLine(stringBuilder.ToString());
                    break;
                case EVENT_TYPE_SPATIAL:
                    mStreamWriterCsvMotionEventsSpatial.WriteLine(stringBuilder.ToString());
                    break;
                case EVENT_TYPE_TEMPORAL:
                    mStreamWriterCsvMotionEventsTemporal.WriteLine(stringBuilder.ToString());
                    break;
            }           
        }        

        private void DisableButtons()
        {
            SetButtonStatus(false);
        }

        private void EnableButtons()
        {
            this.button1.Invoke(new MethodInvoker(() => this.button1.Enabled = true));
            this.button2.Invoke(new MethodInvoker(() => this.button2.Enabled = true));
            this.button3.Invoke(new MethodInvoker(() => this.button3.Enabled = true));
            this.button4.Invoke(new MethodInvoker(() => this.button4.Enabled = true));
            this.button5.Invoke(new MethodInvoker(() => this.button5.Enabled = true));
            this.button6.Invoke(new MethodInvoker(() => this.button6.Enabled = true));
        }

        private void SetButtonStatus(bool isEnabled) {
            btnGo.Enabled = isEnabled;
            button1.Enabled = isEnabled;
            button2.Enabled = isEnabled;
            button3.Enabled = isEnabled;
            button4.Enabled = isEnabled;
            button5.Enabled = isEnabled;
            button6.Enabled = isEnabled;
        }


        public void DuplicateDB()
        {
            MongoCollection<ModelTemplate> mongoLocal = UtilsNewDB.GetTemplates();
            MongoCollection<ModelTemplate> mongoCloud = UtilsNewDB.GetTemplatesCloud();
            IEnumerable<ModelTemplate> modelTemplates;

            bool isFinished = false;
            double totalNumbRecords = mongoCloud.Count();
            double currentRecord = 0;
            int limit = 50;
            int skip = 0;

            try
            {
                while (!isFinished)
                {
                    modelTemplates = mongoCloud.FindAll().SetLimit(limit).SetSkip(skip);

                    foreach (ModelTemplate template in modelTemplates)
                    {
                        currentRecord++;
                        UpdateProgress(totalNumbRecords, currentRecord);

                        if (template.DeviceId.CompareTo("062bba750ae4e2b3") == 0)
                        {
                            template.Name = template.Name.ToLower();

                            if (template.Name.CompareTo("shir_mor88@walla.com") != 0 &&
                                template.Name.CompareTo("yinon_m") != 0 &&
                                template.Name.CompareTo("michalsh96@gmail.com") != 0)
                            {
                                if (template.ExpShapeList.Count > 0 && IsValid(template)) {
                                    template.GcmToken = String.Empty;
                                    template.Version = String.Empty;
                                    template.UserCountry = String.Empty;
                                    template.AppLocale = String.Empty;
                                    mongoLocal.Insert(template);
                                }                                
                            }
                        }
                    }
                    skip += limit;

                    if (totalNumbRecords <= skip)
                    {
                        isFinished = true;
                    }
                }
            }
            catch (Exception exc)
            {
                MessageBox.Show(string.Format("Error: {0}", exc.Message));
            }
        }

        private bool IsValid(ModelTemplate template)
        {
            if(template.ExpShapeList[0].Strokes[0].ListEvents.Count >= 3)
            {
                bool isValid = true;

                ModelMotionEventCompact event1 = template.ExpShapeList[0].Strokes[0].ListEvents[0];
                ModelMotionEventCompact event2 = template.ExpShapeList[0].Strokes[0].ListEvents[1];
                ModelMotionEventCompact event3 = template.ExpShapeList[0].Strokes[0].ListEvents[2];

                if(event1.EventTime == event2.EventTime || event1.EventTime == event3.EventTime || event2.EventTime == event3.EventTime)
                {
                    isValid = false;
                }
                return isValid;
            }
            else
            {
                return true;
            }
        }
    }
}