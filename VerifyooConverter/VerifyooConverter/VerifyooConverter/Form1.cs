using JsonConverter.Models;
using MongoDB.Driver;
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
using VerifyooLogic.Comparison;
using VerifyooLogic.UserProfile;
using VerifyooLogic.Utils;

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
        }
        private void SetProgress(string progress)
        {
            this.lblProgress.Invoke(new MethodInvoker(() => this.lblProgress.Text = progress));
        }

        private Template GetBaseTemplate()
        {
            int objectIdx = UtilsRandom.RandomNumber(0, (int)(mListMongoCount - 1));

            Template tempTemplate = null;
            IEnumerable<ModelTemplate> modelTemplates;
            modelTemplates = mListMongo.FindAll().SetLimit(1).SetSkip(objectIdx);

            foreach (ModelTemplate template in modelTemplates)
            {
                mBaseObjectId = template._id.ToString();
                tempTemplate = UtilsTemplateConverter.ConvertTemplate(template);
                break;
            }

            return tempTemplate;
        }

        private void GetDataFromDB()
        {
            try
            {
                //StreamWriter sw = File.CreateText(txtPath.Text);
                //sw.WriteLine("Object ID,Score,User Name,Device Name,Instruction");
                //StringBuilder strBuilder;

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

                IEnumerable<ModelTemplate> modelTemplates;

                Template baseTemplate = null;
                Template tempTemplate = null;

                CompactGesture tempGestureAuth = null;
                CompactGesture tempGestureBase = null;

                CompactGesture tempGestureUserBase = null;
                CompactGesture tempGestureUserAuth = null;

                double tempScore = 0;
                int idxGesture = 0;
                double totalGesturesFp = 0;
                double totalGesturesFn = 0;

                double threasholdLow, threasholdMed, threasholdHigh;

                try
                {
                    threasholdLow = double.Parse(txtThreasholdLow.Text);
                    threasholdMed = double.Parse(txtThreasholdMedium.Text);
                    threasholdHigh = double.Parse(txtThreasholdHigh.Text);
                }
                catch (Exception exc)
                {
                    threasholdLow = UtilsConsts.THRESHOLD_LOW;
                    threasholdMed = UtilsConsts.THRESHOLD_MED;
                    threasholdHigh = UtilsConsts.THRESHOLD_HIGH;
                }

                baseTemplate = GetBaseTemplate();

                int idx1 = -1;

                GestureComparer comparer = new GestureComparer();
                while (!mIsFinished)
                {
                    SetProgress(String.Format("{0} out of {1}", skip.ToString(), totalNumbRecords.ToString()));

                    modelTemplates = mListMongo.FindAll().SetLimit(limit).SetSkip(skip);
                    
                    foreach (ModelTemplate template in modelTemplates)
                    {
                        tempTemplate = UtilsTemplateConverter.ConvertTemplate(template);

                        try
                        {
                            
                            for (idx1 = 14; idx1 < tempTemplate.ListGestures.size(); idx1++)
                            {
                                for (int idx2 = idx1; idx2 < tempTemplate.ListGestures.size(); idx2++)
                                {
                                    tempGestureUserBase = (CompactGesture)tempTemplate.ListGestures.get(idx1);
                                    tempGestureUserAuth = (CompactGesture)tempTemplate.ListGestures.get(idx2);

                                    if (idx1 != idx2 && tempGestureUserBase.Instruction == tempGestureUserAuth.Instruction)
                                    {
                                        tempGestureUserBase.XDpi = tempTemplate.XDpi;
                                        tempGestureUserBase.YDpi = tempTemplate.YDpi;

                                        tempGestureUserAuth.XDpi = tempTemplate.XDpi;
                                        tempGestureUserAuth.YDpi = tempTemplate.YDpi;

                                        tempScore = comparer.Compare(tempGestureUserBase, tempGestureUserAuth);

                                        totalGesturesFn++;
                                        if (tempScore <= threasholdLow)
                                        {
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
                        catch (Exception exc)
                        {
                            swLog.WriteLine(String.Format("FN Error in object {0} in gesture {1}. Reason: {2}", template._id.ToString(), idx1.ToString(), exc.Message));
                            swLog.Flush();
                        }                       

                        if(String.Compare(mBaseObjectId, template._id.ToString()) != 0)
                        {
                            try
                            {
                                
                                for (idxGesture = 0; idxGesture < baseTemplate.ListGestures.size(); idxGesture++)
                                {
                                    for (int idxGestureAuth = 0; idxGestureAuth < tempTemplate.ListGestures.size(); idxGestureAuth++)
                                    {
                                        tempGestureAuth = (CompactGesture)tempTemplate.ListGestures.get(idxGestureAuth);
                                        tempGestureBase = (CompactGesture)baseTemplate.ListGestures.get(idxGesture);

                                        if (String.Compare(tempGestureAuth.Instruction, tempGestureBase.Instruction) == 0)
                                        {
                                            tempGestureAuth.XDpi = tempTemplate.XDpi;
                                            tempGestureAuth.YDpi = tempTemplate.YDpi;

                                            tempGestureBase.XDpi = baseTemplate.XDpi;
                                            tempGestureBase.YDpi = baseTemplate.YDpi;

                                            tempScore = comparer.Compare(tempGestureAuth, tempGestureBase);

                                            totalGesturesFp++;
                                            if (tempScore > threasholdLow)
                                            {
                                                fpLow++;
                                                //strBuilder = new StringBuilder();
                                                //strBuilder.Append(template._id.ToString());
                                                //strBuilder.Append(",");
                                                //strBuilder.Append(tempScore.ToString());
                                                //strBuilder.Append(",");
                                                //strBuilder.Append(template.Name);
                                                //strBuilder.Append(",");
                                                //strBuilder.Append(template.ModelName);
                                                //strBuilder.Append(",");
                                                //strBuilder.Append((template.ExpShapeList[idxGesture]).Instruction);

                                                //sw.WriteLine(strBuilder.ToString());
                                                //sw.Flush();
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
                            catch (Exception exc)
                            {
                                swLog.WriteLine(String.Format("FP Error in object {0} in gesture {1}. Reason: {2}", template._id.ToString(), idxGesture.ToString(), exc.Message));
                                swLog.Flush();
                            }
                        }                        
                    }

                    skip += limit;                    

                    if (totalNumbRecords <= skip)
                    {
                        mIsFinished = true;
                    }

                    if (mIsFinished)
                    {
                        SetProgress(String.Format("{0} out of {1}", totalNumbRecords.ToString(), totalNumbRecords.ToString()));
                        //sw.Close();
                        swLog.Close();

                        lblFpLow.Text = string.Format("False positive Low: {0}", getPercentage(fpLow, totalGesturesFp));
                        lblFpMed.Text = string.Format("False positive Medium: {0}", getPercentage(fpMed, totalGesturesFp));
                        lblFpHigh.Text = string.Format("False positive High: {0}", getPercentage(fpHigh, totalGesturesFp));

                        lblFnLow.Text = string.Format("False negative Low: {0}", getPercentage(fnLow, totalGesturesFn));
                        lblFnMed.Text = string.Format("False negative Medium: {0}", getPercentage(fnMed, totalGesturesFn));
                        lblFnHigh.Text = string.Format("False negative High: {0}", getPercentage(fnHigh, totalGesturesFn));

                        lblFPTotalGestures.Text = string.Format("FP Gestures analyzed: {0}", totalGesturesFp.ToString());
                        lblFNTotalGestures.Text = string.Format("FN Gestures analyzed: {0}", totalGesturesFn.ToString());
                    }
                }
            }
            catch(Exception exc)
            {
                this.lblProgress.Invoke(new MethodInvoker(() => this.lblProgress.Text = ("Error: " + exc.Message)));
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
    }
}
