using Data.UserProfile.Extended;
using Data.UserProfile.Raw;
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
        string mBaseObjectId;

        public Form1()
        {
            InitializeComponent();
            Init();
        }

        private void Init()
        {
            mListMongo = UtilsDB.GetCollShapes();
            mListMongoCount = mListMongo.Count();

            txtThreasholdLow.Text = UtilsConsts.THRESHOLD_LOW.ToString();
            txtThreasholdMedium.Text = UtilsConsts.THRESHOLD_MED.ToString();
            txtThreasholdHigh.Text = UtilsConsts.THRESHOLD_HIGH.ToString();
            txtLimit.Text = "0";
        }
        private void SetProgress(string progress)
        {
            this.lblProgress.Invoke(new MethodInvoker(() => this.lblProgress.Text = progress));
        }

        private TemplateExtended GetBaseTemplate()
        {
            TemplateExtended tempTemplate = null;

            if (txtObjectID.Text.Length > 0)
            {                
                MongoCollection<ModelTemplate> templates = mListMongo;

                IMongoQuery query = Query<ModelTemplate>.EQ(c => c.Name, txtObjectID.Text);
                var filter = Builders<ModelTemplate>.Filter.Eq("Name", txtObjectID.Text);

                ModelTemplate tempModelTemplate = templates.FindOne(query);
                mBaseObjectId = tempModelTemplate._id.ToString();
                tempTemplate = UtilsTemplateConverter.ConvertTemplate(tempModelTemplate);                
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
                    tempTemplate = UtilsTemplateConverter.ConvertTemplate(template);
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
            return isValid;
        }


        private void GetDataFromDB()
        {
            try
            {
                StreamWriter sw = File.CreateText(txtPath.Text);
                sw.WriteLine("Object ID,Type,Score,Threshold,User Name,Device Name,Instruction");
                StringBuilder strBuilder;

                double fpLow = 0;
                double fpMed = 0;
                double fpHigh = 0;

                double fnLow = 0;
                double fnMed = 0;
                double fnHigh = 0;

                string logfile = txtPath.Text.Replace("csv", "log");
                StreamWriter swLog = File.CreateText(logfile);

                bool isFirst = true;

                int totalNumbRecords = (int)mListMongoCount;
                int limit = 10;
                int skip = 0;
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

                while (!mIsFinished)
                {
                    SetProgress(String.Format("{0} out of {1}", skip.ToString(), totalNumbRecords.ToString()));
                    //this.lblSubCounter.Invoke(new MethodInvoker(() => this.lblSubCounter.Text = String.Format("Sub-counter: {0}", "0")));

                    modelTemplates = mListMongo.FindAll().SetLimit(limit).SetSkip(skip);
                    
                    foreach (ModelTemplate template in modelTemplates)
                    {
                        if (IsRecordValid(template))
                        {
                            tempTemplate = UtilsTemplateConverter.ConvertTemplate(template);

                            try
                            {

                                for (idx1 = 14; idx1 < tempTemplate.ListGestureExtended.size(); idx1++)
                                {
                                    for (int idx2 = idx1; idx2 < tempTemplate.ListGestureExtended.size(); idx2++)
                                    {                                       
                                        tempGestureUserBase = (GestureExtended)tempTemplate.ListGestureExtended.get(idx1);
                                        tempGestureUserAuth = (GestureExtended)tempTemplate.ListGestureExtended.get(idx2);

                                        if (idx1 != idx2 && tempGestureUserBase.Instruction == tempGestureUserAuth.Instruction)
                                        {
                                            //tempGestureUserBase.XDpi = tempTemplate.XDpi;
                                            //tempGestureUserBase.YDpi = tempTemplate.YDpi;

                                            //tempGestureUserAuth.XDpi = tempTemplate.XDpi;
                                            //tempGestureUserAuth.YDpi = tempTemplate.YDpi;

                                            if (IsGestureValid(tempGestureUserBase) && IsGestureValid(tempGestureUserAuth) && tempGestureUserBase.ListStrokesExtended.size() == tempGestureUserAuth.ListStrokesExtended.size())                                                
                                            {
                                                comparer = new GestureComparer(false);
                                                comparer.CompareGestures(tempGestureUserBase, tempGestureUserAuth);
                                                tempScore = comparer.GetScore();

                                                if (!Double.IsNaN(tempScore))
                                                {
                                                    totalGesturesFn++;
                                                    if (tempScore <= threasholdLow)
                                                    {
                                                        strBuilder = new StringBuilder();
                                                        strBuilder.Append(template._id.ToString());
                                                        strBuilder.Append(",");
                                                        strBuilder.Append("False Negative");
                                                        strBuilder.Append(",");
                                                        strBuilder.Append(tempScore.ToString());
                                                        strBuilder.Append(",");
                                                        strBuilder.Append(threasholdLow.ToString());
                                                        strBuilder.Append(",");
                                                        strBuilder.Append(template.Name);
                                                        strBuilder.Append(",");
                                                        strBuilder.Append(template.ModelName);
                                                        strBuilder.Append(",");
                                                        strBuilder.Append((template.ExpShapeList[idxGesture]).Instruction);

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
                                        }
                                    }
                                }
                            }
                            catch (Exception exc)
                            {
                                swLog.WriteLine(String.Format("FN Error in object {0} in gesture {1}. Reason: {2}", template._id.ToString(), idx1.ToString(), exc.Message));
                                swLog.Flush();
                            }

                            //isFpFinished = false;
                            //skipFp = skip;
                            //while (!isFpFinished)
                            //{
                            //    modelTemplatesFp = mListMongo.FindAll().SetLimit(limit).SetSkip(skipFp);

                            //    foreach (ModelTemplate templateFp in modelTemplatesFp)
                            //    {
                            //        baseTemplate = UtilsTemplateConverter.ConvertTemplate(templateFp);

                            //        if (String.Compare(templateFp._id.ToString(), template._id.ToString()) != 0)
                            //        {
                            //            try
                            //            {
                            //                for (idxGesture = 0; idxGesture < baseTemplate.ListGestureExtended.size(); idxGesture++)
                            //                {
                            //                    for (int idxGestureAuth = 0; idxGestureAuth < tempTemplate.ListGestureExtended.size(); idxGestureAuth++)
                            //                    {
                            //                        tempGestureAuth = (Gesture)tempTemplate.ListGestureExtended.get(idxGestureAuth);
                            //                        tempGestureBase = (Gesture)baseTemplate.ListGestureExtended.get(idxGesture);

                            //                        if (String.Compare(tempGestureAuth.Instruction, tempGestureBase.Instruction) == 0)
                            //                        {                                                    
                            //                            comparer = new GestureComparer();
                            //                            comparer.CompareGestures(tempGestureAuth, tempGestureBase);
                            //                            tempScore = comparer.GetScore();

                            //                            totalGesturesFp++;
                            //                            if (tempScore > threasholdLow)
                            //                            {
                            //                                fpLow++;                                                            
                            //                            }
                            //                            if (tempScore > threasholdMed)
                            //                            {
                            //                                fpMed++;
                            //                            }
                            //                            if (tempScore > threasholdHigh)
                            //                            {
                            //                                fpHigh++;
                            //                            }
                            //                        }
                            //                    }
                            //                }
                            //            }
                            //            catch (Exception exc)
                            //            {
                            //                swLog.WriteLine(String.Format("FP Error in object {0} in gesture {1}. Reason: {2}", template._id.ToString(), idxGesture.ToString(), exc.Message));
                            //                swLog.Flush();
                            //            }
                            //        }
                            //    }

                            //    skipFp += limit;
                            //    this.lblSubCounter.Invoke(new MethodInvoker(() => this.lblSubCounter.Text = String.Format("Sub-counter: {0}", skipFp.ToString())));

                            //    if (totalNumbRecords <= skipFp)
                            //    {
                            //        isFpFinished = true;
                            //    }
                            //}

                            if (String.Compare(mBaseObjectId, template._id.ToString()) != 0)
                            {
                                try
                                {
                                    for (idxGesture = 0; idxGesture < baseTemplate.ListGestureExtended.size(); idxGesture++)
                                    {
                                        for (int idxGestureAuth = 0; idxGestureAuth < tempTemplate.ListGestureExtended.size(); idxGestureAuth++)
                                        {
                                            tempGestureAuth = (GestureExtended)tempTemplate.ListGestureExtended.get(idxGestureAuth);
                                            tempGestureBase = (GestureExtended)baseTemplate.ListGestureExtended.get(idxGesture);

                                            if (String.Compare(tempGestureAuth.Instruction, tempGestureBase.Instruction) == 0)
                                            {
                                                //tempGestureAuth.XDpi = tempTemplate.XDpi;
                                                //tempGestureAuth.YDpi = tempTemplate.YDpi;

                                                //tempGestureBase.XDpi = baseTemplate.XDpi;
                                                //tempGestureBase.YDpi = baseTemplate.YDpi;

                                                if (IsGestureValid(tempGestureAuth) && IsGestureValid(tempGestureBase))
                                                {
                                                    comparer = new GestureComparer(false);
                                                    comparer.CompareGestures(tempGestureBase, tempGestureAuth);
                                                    tempScore = comparer.GetScore();

                                                    if (!Double.IsNaN(tempScore))
                                                    {
                                                        totalGesturesFp++;
                                                        if (tempScore > threasholdLow)
                                                        {
                                                            fpLow++;
                                                            strBuilder = new StringBuilder();
                                                            strBuilder.Append(template._id.ToString());
                                                            strBuilder.Append(",");
                                                            strBuilder.Append("False Positive");
                                                            strBuilder.Append(",");
                                                            strBuilder.Append(tempScore.ToString());
                                                            strBuilder.Append(",");
                                                            strBuilder.Append(threasholdLow.ToString());
                                                            strBuilder.Append(",");
                                                            strBuilder.Append(template.Name);
                                                            strBuilder.Append(",");
                                                            strBuilder.Append(template.ModelName);
                                                            strBuilder.Append(",");
                                                            strBuilder.Append((template.ExpShapeList[idxGesture]).Instruction);

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

                    skip += limit;                    

                    if (totalNumbRecords <= skip)
                    {
                        mIsFinished = true;
                    }

                    if(recordsLimit > 0 && skip >= recordsLimit)
                    {
                        mIsFinished = true;
                    }

                    this.lblFpLow.Invoke(new MethodInvoker(() => this.lblFpLow.Text = string.Format("False positive Low ({1}%) : {0}", getPercentage(fpLow, totalGesturesFp), threasholdLow.ToString())));
                    this.lblFpMed.Invoke(new MethodInvoker(() => this.lblFpMed.Text = string.Format("False positive Medium ({1}%) : {0}", getPercentage(fpMed, totalGesturesFp), threasholdMed.ToString())));
                    this.lblFpHigh.Invoke(new MethodInvoker(() => this.lblFpHigh.Text = string.Format("False positive High ({1}%) : {0}", getPercentage(fpHigh, totalGesturesFp), threasholdHigh.ToString())));

                    this.lblFnLow.Invoke(new MethodInvoker(() => this.lblFnLow.Text = string.Format("False negative Low ({1}%) : {0}", getPercentage(fnLow, totalGesturesFn), threasholdLow.ToString())));
                    this.lblFnMed.Invoke(new MethodInvoker(() => this.lblFnMed.Text = string.Format("False negative Medium ({1}%) : {0}", getPercentage(fnMed, totalGesturesFn), threasholdMed.ToString())));
                    this.lblFnHigh.Invoke(new MethodInvoker(() => this.lblFnHigh.Text = string.Format("False negative High ({1}%) : {0}", getPercentage(fnHigh, totalGesturesFn), threasholdHigh.ToString())));

                    this.lblFPTotalGestures.Invoke(new MethodInvoker(() => this.lblFPTotalGestures.Text = string.Format("FP Gestures analyzed: {0}", totalGesturesFp.ToString())));
                    this.lblFNTotalGestures.Invoke(new MethodInvoker(() => this.lblFNTotalGestures.Text = string.Format("FN Gestures analyzed: {0}", totalGesturesFn.ToString())));

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
                this.lblProgress.Invoke(new MethodInvoker(() => this.lblProgress.Text = ("Error: " + exc.Message)));
            }            
        }

        private bool IsGestureValid(GestureExtended g)
        {
            if(g.GestureLengthMM > 0 && g.GestureTotalTimeWithPauses > 0 && g.GestureAverageVelocity > 0)
            {
                return true;
            }
            else
            {
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
            Task task = Task.Run((Action)CalculateFalsePositives);
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Task task = Task.Run((Action)CancelOperation);            
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }
    }
}
