using Data.Comparison.Interfaces;
using Data.UserProfile.Extended;
using Data.UserProfile.Raw;
using java.util;
using JsonConverter.Models;
using Logic.Comparison;
using Logic.Comparison.Stats;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
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
using VerifyooConverter.Logic;

namespace VerifyooConverter
{
    public partial class Form1 : Form
    {
        private MongoCollection<ModelTemplate> mListMongo;
        private long mListMongoCount;
        bool mIsFinished = false;
        bool mIsFinishedFp = false;
        string mBaseObjectId;
        string mFilterDevice;
        string mFilterInstruction;
        string mPathFaultyGestures;
        string mUserName;
        string currentBaseName;
        string currentBaseTemplateId;
        string currentBaseGestureId;

        string currentAuthName;
        string currentAuthTemplateId;
        string currentAuthGestureId;

        UtilsInvalidGestures mUtilInvalid;

        int mTemplateValid;
        int mTemplateInvalid;

        int mValid;
        int mInvalid;

        HashMap mDict;

        public Form1()
        {
            InitializeComponent();
            Init();
        }

        private void Init()
        {
            mTemplateValid = 0;
            mTemplateInvalid = 0;

            mValid = 0;
            mInvalid = 0;
            mListMongo = UtilsDB.GetCollShapes();
            mListMongoCount = mListMongo.Count();

            mPathFaultyGestures = txtFaultyGestures.Text;

            txtThreasholdLow.Text = UtilsConsts.THRESHOLD_LOW.ToString();
            txtThreasholdMedium.Text = UtilsConsts.THRESHOLD_MED.ToString();
            txtThreasholdHigh.Text = UtilsConsts.THRESHOLD_HIGH.ToString();
            txtLimit.Text = "0";

            mUtilInvalid = new UtilsInvalidGestures();
        }
        private void SetProgress(string progress)
        {
            this.lblProgress.Invoke(new MethodInvoker(() => this.lblProgress.Text = progress));
        }

        private TemplateExtended GetBaseTemplate()
        {
            TemplateExtended tempTemplate = null;

            mUserName = txtObjectID.Text;

            if (txtObjectID.Text.Length > 0)
            {                
                MongoCollection<ModelTemplate> templates = mListMongo;

                IMongoQuery query = Query<ModelTemplate>.EQ(c => c.Name, txtObjectID.Text);
                var filter = Builders<ModelTemplate>.Filter.Eq("Name", txtObjectID.Text);

                ModelTemplate tempModelTemplate = templates.FindOne(query);
                mBaseObjectId = tempModelTemplate._id.ToString();
                tempTemplate = UtilsTemplateConverter.ConvertTemplate(tempModelTemplate, mUtilInvalid);
            }
            else
            {
                int objectIdx = UtilsRandom.RandomNumber(0, (int)(mListMongoCount - 1));

                tempTemplate = null;
                IEnumerable<ModelTemplate> modelTemplates;
                modelTemplates = mListMongo.FindAll().SetLimit(1).SetSkip(objectIdx);

                foreach (ModelTemplate template in modelTemplates)
                {
                    mBaseObjectId = template._id.ToString();
                    tempTemplate = UtilsTemplateConverter.ConvertTemplate(template, mUtilInvalid);
                    break;
                }
            }
            
            return tempTemplate;
        }

        private bool IsRecordValid(ModelTemplate template)
        {
            bool isValid = true;
            if (template.Name.ToLower().Contains("stam") || template.Name.ToLower().Contains("prob"))
            {
                isValid = false;
            }

            if(mFilterDevice.Length > 0 && string.Compare(template.ModelName, mFilterDevice) != 0)
            {
                isValid = false;
            }

            if(isValid)
            {
                mTemplateValid++;
            }
            else
            {
                mTemplateInvalid++;
            }
            return isValid;
        }


        private bool IsUseInstruction(string instruction)
        {
            bool useInstruction;
            if (mFilterInstruction.Length > 0)
            {
                if (string.Compare(mFilterInstruction, instruction) == 0)
                {
                    useInstruction = true;
                }
                else
                {
                    useInstruction = false;
                }
            }
            else
            {
                useInstruction = true;
            }
            return useInstruction;
        }


        protected string GetParameterDetails(GestureComparer comparer, int idx)
        {
            string str;
            if (idx < comparer.GetResultsSummary().ListCompareResults.size())
            {
                double value = ((ICompareResult)comparer.GetResultsSummary().ListCompareResults.get(idx)).GetValue();
                value = Math.Round(value, 3);

                double zScore = ((ICompareResult)comparer.GetResultsSummary().ListCompareResults.get(idx)).GetWeight();
                zScore = Math.Round(zScore, 3);

                string name = ((ICompareResult)comparer.GetResultsSummary().ListCompareResults.get(idx)).GetName();

                str = String.Format("[Score={0}]  [Z={1}]  [Name={2}]", value.ToString(), zScore.ToString(), name);
            }
            else
            {
                str = "[No Value]";
            }
            
            return str;
        }


        private void GetDataFromDB()
        {
            try
            {
                
                StreamReader srFaultyGestures = new StreamReader(mPathFaultyGestures);

                string faultyId;
                while(!srFaultyGestures.EndOfStream)
                {
                    faultyId = srFaultyGestures.ReadLine();
                    mUtilInvalid.HashInvalid.Add(faultyId, true);
                }
                srFaultyGestures.Close();

                StreamWriter sw = File.CreateText(txtPath.Text);
                sw.WriteLine("Object ID,Base Gesture ID,Auth Gesture ID,Type,Score,ShapeScore,Threshold,User Name,Device Name,Instruction,P01,P02,P03,P04,P05,P06,P07,P08,P09,P10,P11,P12,P13,P14,P15,P16,P17,P18,P19");
                StringBuilder strBuilder;

                double fpLow = 0;
                double fpMed = 0;
                double fpHigh = 0;

                double fnLow = 0;
                double fnMed = 0;
                double fnHigh = 0;

                string logfile = txtPath.Text.Replace("csv", "log");
                StreamWriter swLog = File.CreateText(@"C:\temp\log.txt");

                bool isFirst = true;

                int totalNumbRecords = (int)mListMongoCount;
                int limit = 10;
                int skip = 0;

                int limitFp = 10;
                int skipFp = 0;

                IEnumerable<ModelTemplate> modelTemplates;
                IEnumerable<ModelTemplate> modelTemplatesFp;             

                TemplateExtended baseTemplate = null;
                TemplateExtended tempTemplate = null;

                GestureExtended tempGestureAuth = null;
                GestureExtended tempGestureBase = null;

                GestureExtended tempGestureUserBase = null;
                GestureExtended tempGestureUserAuth = null;

                double tempScore = 0;
                int idxGesture = 0;
                double totalGesturesFp = 0;
                double totalGesturesFn = 0;

                double threasholdLow, threasholdMed, threasholdHigh;
                double recordsLimit;

                mFilterDevice = string.Empty;
                if (txtDevice.Text.Length > 0)
                {
                    mFilterDevice = txtDevice.Text;
                }

                mFilterInstruction = string.Empty;
                if (txtInstruction.Text.Length > 0)
                {
                    mFilterInstruction = txtInstruction.Text;
                }

                try
                {
                    threasholdLow = double.Parse(txtThreasholdLow.Text);
                    threasholdMed = double.Parse(txtThreasholdMedium.Text);
                    threasholdHigh = double.Parse(txtThreasholdHigh.Text);
                    recordsLimit = double.Parse(txtLimit.Text);
                }
                catch (Exception exc)
                {
                    threasholdLow = UtilsConsts.THRESHOLD_LOW;
                    threasholdMed = UtilsConsts.THRESHOLD_MED;
                    threasholdHigh = UtilsConsts.THRESHOLD_HIGH;
                    recordsLimit = 0;
                }

                baseTemplate = GetBaseTemplate();

                int idx1 = -1;
               
                GestureComparer comparer = new GestureComparer(false);

                bool isFpFinished;

                InitMethodFilters();
                List<Double> listScores;
                List<string> listInstructions;

                while (!mIsFinished)
                {
                    SetProgress(String.Format("{0} out of {1}", skip.ToString(), totalNumbRecords.ToString()));
                    //this.lblSubCounter.Invoke(new MethodInvoker(() => this.lblSubCounter.Text = String.Format("Sub-counter: {0}", "0")));

                    modelTemplates = mListMongo.FindAll().SetLimit(limit).SetSkip(skip);
                    
                    foreach (ModelTemplate template in modelTemplates)
                    {
                        if (IsRecordValid(template))
                        {
                            try
                            {
                                tempTemplate = UtilsTemplateConverter.ConvertTemplate(template, mUtilInvalid);
                            
                                listScores = new List<Double>();
                                listInstructions = new List<string>();
                                if (tempTemplate.ListGestureExtended.size() > 21)
                                {
                                    for (idx1 = 7; idx1 < 14; idx1++)
                                    {
                                        tempGestureUserBase = (GestureExtended)tempTemplate.ListGestureExtended.get(idx1);
                                        for (int idx2 = 14; idx2 < tempTemplate.ListGestureExtended.size(); idx2++)
                                        {
                                            tempGestureUserAuth = (GestureExtended)tempTemplate.ListGestureExtended.get(idx2);

                                            if (tempGestureUserBase.Instruction == tempGestureUserAuth.Instruction && IsUseInstruction(tempGestureUserBase.Instruction))
                                            {
                                                if (IsGestureValid(tempGestureUserBase, tempTemplate.Id) && IsGestureValid(tempGestureUserAuth, tempTemplate.Id) && tempGestureUserBase.ListStrokesExtended.size() == tempGestureUserAuth.ListStrokesExtended.size())
                                                {
                                                    this.lblCurrentBaseUser.Invoke(new MethodInvoker(() => this.lblCurrentBaseUser.Text = template.Name));
                                                    this.lblCurrAuthUser.Invoke(new MethodInvoker(() => this.lblCurrAuthUser.Text = template.Name));

                                                    comparer = new GestureComparer(false);
                                                    comparer.CompareGestures(tempGestureUserBase, tempGestureUserAuth, mDict);
                                                    tempScore = comparer.GetScore();
                                                    if (!Double.IsNaN(tempScore))
                                                    {
                                                        listScores.Add(tempScore);
                                                        listInstructions.Add(tempGestureUserBase.Instruction);
                                                    }
                                                }
                                            }
                                        }
                                    }                                    

                                    tempScore = CalculateScore(listScores, listInstructions);
                                    totalGesturesFn++;
                                    if (tempScore <= threasholdLow)
                                    {
                                        strBuilder = new StringBuilder();
                                        strBuilder.Append(template._id.ToString());
                                        strBuilder.Append(",");
                                        strBuilder.Append(tempGestureUserBase.Id.ToString());
                                        strBuilder.Append(",");
                                        strBuilder.Append(tempGestureUserAuth.Id.ToString());                                       
                                        strBuilder.Append(",");
                                        strBuilder.Append("False Negative");
                                        strBuilder.Append(",");
                                        strBuilder.Append(tempScore.ToString());
                                        strBuilder.Append(",");
                                        strBuilder.Append(comparer.GetMinCosineDistance().ToString());
                                        strBuilder.Append(",");
                                        strBuilder.Append(threasholdLow.ToString());
                                        strBuilder.Append(",");
                                        strBuilder.Append(template.Name);
                                        strBuilder.Append(",");
                                        strBuilder.Append(template.ModelName);
                                        strBuilder.Append(",");
                                        strBuilder.Append(tempGestureUserBase.Instruction);

                                        AddParamsToResults(strBuilder, comparer);

                                        sw.WriteLine(strBuilder.ToString());
                                        sw.Flush();
                                        fnLow++;
                                    }
                                    if (tempScore <= threasholdMed)
                                    {
                                        fnMed++;
                                    }
                                    if (tempScore <= threasholdHigh)
                                    {
                                        fnHigh++;
                                    }
                                }                                
                            }
                            catch (Exception exc)
                            {
                                swLog.WriteLine(String.Format("FN Error in object {0} in gesture {1}. Reason: {2}", template._id.ToString(), idx1.ToString(), exc.Message));
                                swLog.Flush();
                            }

                            this.lblFnLow.Invoke(new MethodInvoker(() => this.lblFnLow.Text = string.Format("False negative Low ({1}%) : {0}", getPercentage(fnLow, totalGesturesFn), threasholdLow.ToString())));
                            this.lblFnMed.Invoke(new MethodInvoker(() => this.lblFnMed.Text = string.Format("False negative Medium ({1}%) : {0}", getPercentage(fnMed, totalGesturesFn), threasholdMed.ToString())));
                            this.lblFnHigh.Invoke(new MethodInvoker(() => this.lblFnHigh.Text = string.Format("False negative High ({1}%) : {0}", getPercentage(fnHigh, totalGesturesFn), threasholdHigh.ToString())));
                            this.lblFNTotalGestures.Invoke(new MethodInvoker(() => this.lblFNTotalGestures.Text = string.Format("FN Gestures analyzed: {0}", totalGesturesFn.ToString())));

                            this.lblValidGestures.Invoke(new MethodInvoker(() => this.lblValidGestures.Text = mValid.ToString()));
                            this.lblInvalidGestures.Invoke(new MethodInvoker(() => this.lblInvalidGestures.Text = mInvalid.ToString()));

                            this.lblValidTemplates.Invoke(new MethodInvoker(() => this.lblValidTemplates.Text = mTemplateValid.ToString()));
                            this.lblInvalidTemplates.Invoke(new MethodInvoker(() => this.lblInvalidTemplates.Text = mTemplateInvalid.ToString()));

                            mIsFinishedFp = false;
                            skipFp = skip;

                            if(mUserName.Length == 0)
                            {
                                while (!mIsFinishedFp)
                                {
                                    modelTemplates = mListMongo.FindAll().SetLimit(limitFp).SetSkip(skipFp);

                                    foreach (ModelTemplate templateBaseFp in modelTemplates)
                                    {
                                        if (IsRecordValid(templateBaseFp))
                                        {
                                            try
                                            {
                                                baseTemplate = UtilsTemplateConverter.ConvertTemplate(templateBaseFp, mUtilInvalid);

                                                if (String.Compare(templateBaseFp._id.ToString(), template._id.ToString()) != 0)
                                                {
                                                
                                                        listScores = new List<Double>();
                                                        listInstructions = new List<String>();
                                                        for (idxGesture = 0; idxGesture < 7; idxGesture++)
                                                        {
                                                            for (int idxGestureAuth = 0; idxGestureAuth < tempTemplate.ListGestureExtended.size(); idxGestureAuth++)
                                                            {
                                                                if (idxGesture < baseTemplate.ListGestureExtended.size() && idxGestureAuth < tempTemplate.ListGestureExtended.size())
                                                                {
                                                                    tempGestureAuth = (GestureExtended)tempTemplate.ListGestureExtended.get(idxGestureAuth);
                                                                    tempGestureBase = (GestureExtended)baseTemplate.ListGestureExtended.get(idxGesture);

                                                                    if (String.Compare(tempGestureAuth.Instruction, tempGestureBase.Instruction) == 0 && IsUseInstruction(tempGestureBase.Instruction))
                                                                    {
                                                                        if (IsGestureValid(tempGestureAuth, tempTemplate.Id) && IsGestureValid(tempGestureBase, tempTemplate.Id))
                                                                        {
                                                                            comparer = new GestureComparer(false);

                                                                            currentBaseName = baseTemplate.Name;
                                                                            currentBaseTemplateId = baseTemplate.Id;
                                                                            currentBaseGestureId = tempGestureBase.Id;

                                                                            currentAuthName = tempTemplate.Name;
                                                                            currentAuthTemplateId = tempTemplate.Id;
                                                                            currentAuthGestureId = tempGestureAuth.Id;

                                                                            this.lblCurrentBaseUser.Invoke(new MethodInvoker(() => this.lblCurrentBaseUser.Text = baseTemplate.Name));
                                                                            this.lblCurrAuthUser.Invoke(new MethodInvoker(() => this.lblCurrAuthUser.Text = template.Name));

                                                                            comparer.CompareGestures(tempGestureBase, tempGestureAuth, mDict);
                                                                            tempScore = comparer.GetScore();

                                                                            if (!Double.IsNaN(tempScore))
                                                                            {
                                                                                listScores.Add(tempScore);
                                                                                listInstructions.Add(tempGestureBase.Instruction);
                                                                            }
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                        }

                                                        if (listScores.Count > 10)
                                                        {
                                                            tempScore = CalculateScore(listScores, listInstructions);
                                                            totalGesturesFp++;
                                                            if (tempScore > threasholdLow)
                                                            {
                                                                fpLow++;
                                                                strBuilder = new StringBuilder();
                                                                strBuilder.Append(template._id.ToString());
                                                                strBuilder.Append(",");
                                                                strBuilder.Append(tempGestureBase.Id.ToString());
                                                                strBuilder.Append(",");
                                                                strBuilder.Append(tempGestureAuth.Id.ToString());
                                                                strBuilder.Append(",");
                                                                strBuilder.Append("False Positive");
                                                                strBuilder.Append(",");
                                                                strBuilder.Append(tempScore.ToString());
                                                                strBuilder.Append(",");
                                                                strBuilder.Append(comparer.GetMinCosineDistance().ToString());
                                                                strBuilder.Append(",");
                                                                strBuilder.Append(threasholdLow.ToString());
                                                                strBuilder.Append(",");
                                                                strBuilder.Append(template.Name);
                                                                strBuilder.Append(",");
                                                                strBuilder.Append(template.ModelName);
                                                                strBuilder.Append(",");
                                                                strBuilder.Append((template.ExpShapeList[idxGesture]).Instruction);

                                                                AddParamsToResults(strBuilder, comparer);

                                                                sw.WriteLine(strBuilder.ToString());
                                                                sw.Flush();
                                                            }
                                                            if (tempScore > threasholdMed)
                                                            {
                                                                fpMed++;
                                                            }
                                                            if (tempScore > threasholdHigh)
                                                            {
                                                                fpHigh++;
                                                            }
                                                        }
                                                
                                                }
                                            }
                                            catch (Exception exc)
                                            {
                                                swLog.WriteLine(String.Format("FP Error in object {0} in gesture {1}. Reason: {2}", template._id.ToString(), idxGesture.ToString(), exc.Message));
                                                swLog.Flush();
                                            }
                                        }
                                    }

                                    this.lblFpLow.Invoke(new MethodInvoker(() => this.lblFpLow.Text = string.Format("False positive Low ({1}%) : {0}", getPercentage(fpLow, totalGesturesFp), threasholdLow.ToString())));
                                    this.lblFpMed.Invoke(new MethodInvoker(() => this.lblFpMed.Text = string.Format("False positive Medium ({1}%) : {0}", getPercentage(fpMed, totalGesturesFp), threasholdMed.ToString())));
                                    this.lblFpHigh.Invoke(new MethodInvoker(() => this.lblFpHigh.Text = string.Format("False positive High ({1}%) : {0}", getPercentage(fpHigh, totalGesturesFp), threasholdHigh.ToString())));
                                    this.lblFPTotalGestures.Invoke(new MethodInvoker(() => this.lblFPTotalGestures.Text = string.Format("FP Gestures analyzed: {0}", totalGesturesFp.ToString())));

                                    this.lblValidGestures.Invoke(new MethodInvoker(() => this.lblValidGestures.Text = mValid.ToString()));
                                    this.lblInvalidGestures.Invoke(new MethodInvoker(() => this.lblInvalidGestures.Text = mInvalid.ToString()));

                                    this.lblValidTemplates.Invoke(new MethodInvoker(() => this.lblValidTemplates.Text = mTemplateValid.ToString()));
                                    this.lblInvalidTemplates.Invoke(new MethodInvoker(() => this.lblInvalidTemplates.Text = mTemplateInvalid.ToString()));

                                    skipFp += limitFp;

                                    if (totalNumbRecords <= skipFp)
                                    {
                                        mIsFinishedFp = true;
                                    }
                                }
                            }
                            else
                            {
                                if (String.Compare(baseTemplate.Id.ToString(), template._id.ToString()) != 0)
                                {
                                    if (IsRecordValid(template))
                                    {
                                        try
                                        {
                                            listScores = new List<Double>();
                                            listInstructions = new List<String>();
                                            for (idxGesture = 0; idxGesture < 7; idxGesture++)
                                            {
                                                for (int idxGestureAuth = 0; idxGestureAuth < tempTemplate.ListGestureExtended.size(); idxGestureAuth++)
                                                {
                                                    if (idxGesture < baseTemplate.ListGestureExtended.size() && idxGestureAuth < tempTemplate.ListGestureExtended.size())
                                                    {
                                                        tempGestureAuth = (GestureExtended)tempTemplate.ListGestureExtended.get(idxGestureAuth);
                                                        tempGestureBase = (GestureExtended)baseTemplate.ListGestureExtended.get(idxGesture);

                                                        if (String.Compare(tempGestureAuth.Instruction, tempGestureBase.Instruction) == 0 && IsUseInstruction(tempGestureBase.Instruction))
                                                        {
                                                            if (IsGestureValid(tempGestureAuth, tempTemplate.Id) && IsGestureValid(tempGestureBase, tempTemplate.Id))
                                                            {
                                                                comparer = new GestureComparer(false);

                                                                currentBaseName = baseTemplate.Name;
                                                                currentBaseTemplateId = baseTemplate.Id;
                                                                currentBaseGestureId = tempGestureBase.Id;

                                                                currentAuthName = tempTemplate.Name;
                                                                currentAuthTemplateId = tempTemplate.Id;
                                                                currentAuthGestureId = tempGestureAuth.Id;

                                                                this.lblCurrentBaseUser.Invoke(new MethodInvoker(() => this.lblCurrentBaseUser.Text = baseTemplate.Name));
                                                                this.lblCurrAuthUser.Invoke(new MethodInvoker(() => this.lblCurrAuthUser.Text = template.Name));

                                                                comparer.CompareGestures(tempGestureBase, tempGestureAuth, mDict);
                                                                tempScore = comparer.GetScore();

                                                                if (!Double.IsNaN(tempScore))
                                                                {
                                                                    listScores.Add(tempScore);
                                                                    listInstructions.Add(tempGestureBase.Instruction);
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }

                                            if (listScores.Count > 10)
                                            {
                                                tempScore = CalculateScore(listScores, listInstructions);
                                                totalGesturesFp++;
                                                if (tempScore > threasholdLow)
                                                {
                                                    fpLow++;
                                                    strBuilder = new StringBuilder();
                                                    strBuilder.Append(template._id.ToString());
                                                    strBuilder.Append(",");
                                                    strBuilder.Append(tempGestureBase.Id.ToString());
                                                    strBuilder.Append(",");
                                                    strBuilder.Append(tempGestureAuth.Id.ToString());
                                                    strBuilder.Append(",");
                                                    strBuilder.Append("False Positive");
                                                    strBuilder.Append(",");
                                                    strBuilder.Append(tempScore.ToString());
                                                    strBuilder.Append(",");
                                                    strBuilder.Append(comparer.GetMinCosineDistance().ToString());
                                                    strBuilder.Append(",");
                                                    strBuilder.Append(threasholdLow.ToString());
                                                    strBuilder.Append(",");
                                                    strBuilder.Append(template.Name);
                                                    strBuilder.Append(",");
                                                    strBuilder.Append(template.ModelName);
                                                    strBuilder.Append(",");
                                                    strBuilder.Append((template.ExpShapeList[idxGesture]).Instruction);

                                                    AddParamsToResults(strBuilder, comparer);

                                                    sw.WriteLine(strBuilder.ToString());
                                                    sw.Flush();
                                                }
                                                if (tempScore > threasholdMed)
                                                {
                                                    fpMed++;
                                                }
                                                if (tempScore > threasholdHigh)
                                                {
                                                    fpHigh++;
                                                }
                                            }
                                        }
                                        catch (Exception exc)
                                        {
                                            swLog.WriteLine(String.Format("FP Error in object {0} in gesture {1}. Reason: {2}", template._id.ToString(), idxGesture.ToString(), exc.Message));
                                            swLog.Flush();
                                        }
                                    }                                    
                                }
                            }
                        }                          
                    }

                    this.lblFnLow.Invoke(new MethodInvoker(() => this.lblFnLow.Text = string.Format("False negative Low ({1}%) : {0}", getPercentage(fnLow, totalGesturesFn), threasholdLow.ToString())));
                    this.lblFnMed.Invoke(new MethodInvoker(() => this.lblFnMed.Text = string.Format("False negative Medium ({1}%) : {0}", getPercentage(fnMed, totalGesturesFn), threasholdMed.ToString())));
                    this.lblFnHigh.Invoke(new MethodInvoker(() => this.lblFnHigh.Text = string.Format("False negative High ({1}%) : {0}", getPercentage(fnHigh, totalGesturesFn), threasholdHigh.ToString())));
                    this.lblFNTotalGestures.Invoke(new MethodInvoker(() => this.lblFNTotalGestures.Text = string.Format("FN Gestures analyzed: {0}", totalGesturesFn.ToString())));

                    this.lblFpLow.Invoke(new MethodInvoker(() => this.lblFpLow.Text = string.Format("False positive Low ({1}%) : {0}", getPercentage(fpLow, totalGesturesFp), threasholdLow.ToString())));
                    this.lblFpMed.Invoke(new MethodInvoker(() => this.lblFpMed.Text = string.Format("False positive Medium ({1}%) : {0}", getPercentage(fpMed, totalGesturesFp), threasholdMed.ToString())));
                    this.lblFpHigh.Invoke(new MethodInvoker(() => this.lblFpHigh.Text = string.Format("False positive High ({1}%) : {0}", getPercentage(fpHigh, totalGesturesFp), threasholdHigh.ToString())));
                    this.lblFPTotalGestures.Invoke(new MethodInvoker(() => this.lblFPTotalGestures.Text = string.Format("FP Gestures analyzed: {0}", totalGesturesFp.ToString())));

                    this.lblValidGestures.Invoke(new MethodInvoker(() => this.lblValidGestures.Text = mValid.ToString()));
                    this.lblInvalidGestures.Invoke(new MethodInvoker(() => this.lblInvalidGestures.Text = mInvalid.ToString()));

                    this.lblValidTemplates.Invoke(new MethodInvoker(() => this.lblValidTemplates.Text = mTemplateValid.ToString()));
                    this.lblInvalidTemplates.Invoke(new MethodInvoker(() => this.lblInvalidTemplates.Text = mTemplateInvalid.ToString()));

                    skip += limit;

                    if (totalNumbRecords <= skip)
                    {
                        mIsFinished = true;
                    }

                    if(recordsLimit > 0 && skip >= recordsLimit)
                    {
                        mIsFinished = true;
                    }
                   
                    if (mIsFinished)
                    {
                        SetProgress(String.Format("{0} out of {1}", totalNumbRecords.ToString(), totalNumbRecords.ToString()));
                        sw.Close();
                        swLog.Close();                        
                    }
                }
            }
            catch(Exception exc)
            {
                string templateId = EnvVars.TemplateId;
                string gestureId = EnvVars.GestureId;

                this.lblProgress.Invoke(new MethodInvoker(() => this.lblProgress.Text = string.Format("Error in base template: {0} - {1} - {2}, and auth template: {3} - {4} - {5}. Reason:{6}", currentBaseName, currentBaseTemplateId, currentBaseGestureId, currentAuthName, currentAuthTemplateId, currentAuthGestureId, exc.Message)));
            }            
        }

        private double CalculateScore(List<Double> listScores, List<string> listInstructions)
        {
            Dictionary<string, InstructionScore> dictScores = new Dictionary<string, InstructionScore>();

            string currInstruction;
            for(int idx = 0; idx < listScores.Count; idx++)
            {
                currInstruction = listInstructions[idx];
                if (dictScores.ContainsKey(currInstruction))
                {
                    dictScores[currInstruction].NumScores++;
                    dictScores[currInstruction].TotalScore += listScores[idx];
                }
                else
                {
                    dictScores.Add(currInstruction, new InstructionScore(currInstruction, listScores[idx]));
                }
            }

            string maxValueKey;
            double currentScore;
            double maxScore;

            int numToUse = 4;
            int currentlyUsed = 0;

            double totalScore = 0;

            while (currentlyUsed < numToUse) {
                maxValueKey = string.Empty;
                foreach (InstructionScore score in dictScores.Values)
                {
                    if (String.IsNullOrEmpty(maxValueKey))
                    {
                        maxValueKey = score.Instruction;
                    }
                    else
                    {
                        currentScore = (score.TotalScore / score.NumScores);
                        maxScore = (dictScores[maxValueKey].TotalScore / dictScores[maxValueKey].NumScores);
    
                    if (currentScore > maxScore)
                        {
                            maxValueKey = score.Instruction;
                        }
                    }
                }

                totalScore += ((dictScores[maxValueKey].TotalScore / dictScores[maxValueKey].NumScores));
                dictScores.Remove(maxValueKey);
                currentlyUsed++;
            }            

            totalScore = totalScore / numToUse;
            return totalScore;
        }

        private void AddParamsToResults(StringBuilder strBuilder, GestureComparer comparer)
        {
            strBuilder.Append(",");
            strBuilder.Append(GetParameterDetails(comparer, 0));
            strBuilder.Append(",");
            strBuilder.Append(GetParameterDetails(comparer, 1));
            strBuilder.Append(",");
            strBuilder.Append(GetParameterDetails(comparer, 2));
            strBuilder.Append(",");
            strBuilder.Append(GetParameterDetails(comparer, 3));
            strBuilder.Append(",");
            strBuilder.Append(GetParameterDetails(comparer, 4));
            strBuilder.Append(",");
            strBuilder.Append(GetParameterDetails(comparer, 5));
            strBuilder.Append(",");
            strBuilder.Append(GetParameterDetails(comparer, 6));
            strBuilder.Append(",");
            strBuilder.Append(GetParameterDetails(comparer, 7));
            strBuilder.Append(",");
            strBuilder.Append(GetParameterDetails(comparer, 8));
            strBuilder.Append(",");
            strBuilder.Append(GetParameterDetails(comparer, 9));
            strBuilder.Append(",");
            strBuilder.Append(GetParameterDetails(comparer, 10));
            strBuilder.Append(",");
            strBuilder.Append(GetParameterDetails(comparer, 11));
            strBuilder.Append(",");
            strBuilder.Append(GetParameterDetails(comparer, 12));
            strBuilder.Append(",");
            strBuilder.Append(GetParameterDetails(comparer, 13));
            strBuilder.Append(",");
            strBuilder.Append(GetParameterDetails(comparer, 14));
            strBuilder.Append(",");
            strBuilder.Append(GetParameterDetails(comparer, 15));
            strBuilder.Append(",");
            strBuilder.Append(GetParameterDetails(comparer, 16));
            strBuilder.Append(",");
            strBuilder.Append(GetParameterDetails(comparer, 17));
            strBuilder.Append(",");
            strBuilder.Append(GetParameterDetails(comparer, 18));
        }

        private void InitMethodFilters()
        {
            mDict = new HashMap();
            //mDict.Add("CompareGestureMinCosineDistance", 1);

            //mDict.Add("CompareGestureLengths", 1);
            //mDict.Add("CompareNumEvents", 1);
            //mDict.Add("CompareGestureAvgVelocity", 1);
            //mDict.Add("CompareGestureTotalTimeInterval", 1);
            //mDict.Add("CompareGestureTotalStrokesTime", 1);
            //mDict.Add("CompareGestureAreas", 1);
            //mDict.Add("CompareGesturePressure", 1);
            //mDict.Add("CompareGestureSurface", 1);
            //mDict.Add("CompareGestureVelocityPeaks", 1);
            //mDict.Add("CompareGestureAverageStartAcceleration", 1);
            mDict.Add("CompareGestureVelocityPeaksIntervalPercentage", 1);
            mDict.Add("CompareGestureStartDirection", 1);
            mDict.Add("CompareGestureEndDirection", 1);
            mDict.Add("CompareGestureMaxDirection", 1);

            //mDict.Add("CompareGestureAccumulatedLengthRSqr", 1);
            //mDict.Add("CompareGestureAccumulatedLengthSlope", 1);
        }


        private bool IsGestureValid(GestureExtended g, string templateId)
        {
            if (mUtilInvalid.HashInvalid.ContainsKey(string.Format("{0}-{1}", templateId, g.Instruction)))  
            {
                return false;
            }

            bool isTimeIntervalsValid = true;

            int limit = 5;
            if (g.ListGestureEventsExtended.size() < limit)
            {
                limit = g.ListGestureEventsExtended.size();
            }

            double totalTimeDiffs = 0;
            double currentTimeDiff;
            for (int idx = 1; idx < limit; idx++)
            {
                currentTimeDiff = ((MotionEventExtended)g.ListGestureEventsExtended.get(idx)).EventTime - ((MotionEventExtended)g.ListGestureEventsExtended.get(idx - 1)).EventTime;
                totalTimeDiffs += currentTimeDiff;

                if(currentTimeDiff == 1000)
                {
                    isTimeIntervalsValid = false;
                }
            }
            if (totalTimeDiffs == 0)
            {
                isTimeIntervalsValid = false;
            }
            
            if(g.GestureLengthMM > 0 && g.GestureTotalTimeInterval > 0 && g.GestureAverageVelocity > 0 && isTimeIntervalsValid)
            {
                mValid++;
                return true;
            }
            else
            {
                mInvalid++;
                return false;
            }
        }

        private string getPercentage(double value, double totalGestures)
        {
            double percentage = value / totalGestures;
            percentage = percentage * 100;
            percentage = Math.Round(percentage, 4);

            return string.Format("{0}%", percentage.ToString());
        }

        private void CalculateFalsePositives()
        {
            this.lblProgress.Invoke(new MethodInvoker(() => this.lblProgress.Text = "Preparing data.."));
            GetDataFromDB();
        }

        private void CancelOperation()
        {
            this.lblProgress.Invoke(new MethodInvoker(() => this.lblProgress.Text = "Canceled"));
            mIsFinished = true;
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            btnStart.Enabled = false;
            Task task = Task.Run((Action)CalculateFalsePositives);
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Task task = Task.Run((Action)CancelOperation);            
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void label7_Click(object sender, EventArgs e)
        {

        }

        private void label9_Click(object sender, EventArgs e)
        {

        }

        private void label10_Click(object sender, EventArgs e)
        {

        }

        private void lblInvalidGestures_Click(object sender, EventArgs e)
        {

        }

        private void lblValidGestures_Click(object sender, EventArgs e)
        {

        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void txtInstruction_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtDevice_TextChanged(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void txtLimit_TextChanged(object sender, EventArgs e)
        {

        }

        private void label2_Click_1(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void txtObjectID_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtPath_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
