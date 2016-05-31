using Data.UserProfile.Extended;
using JsonConverter.Logic;
using JsonConverter.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JsonConverter
{
    public partial class Form1 : Form
    {
        private string _text = string.Empty;
        private string _fileName = string.Empty;
        private MongoCollection<ModelShapes> listMongo;
        private long listMongoCount;

        public Form1()
        {
            
            InitializeComponent();
            setFields();
        }

        private void setFields()
        {
            listMongo = UtilsDB.GetCollShapes();
            listMongoCount = listMongo.Count();
            fromTextBox.Text = "1";
            ToTextBox.Text = listMongoCount.ToString();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var FD = new System.Windows.Forms.OpenFileDialog();
            if (FD.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string fileToOpen = FD.FileName;

                _text = File.ReadAllText(fileToOpen);
                _fileName = fileToOpen;
                label1.Text = string.Format("Selected file: {0}", _fileName);
            }
        }

        private string getConvertedPath(string originalFile, bool isFromDb = false)
        {
            string result = string.Empty;
            int idxPeriod = 0;

            for (int idx = originalFile.Length - 1; idx >= 0; idx--)
            {
                if(originalFile[idx] == '.')
                {
                    idxPeriod = idx;
                    break;
                }
            }

            if(isFromDb)
            {
                result = string.Format("{0}{1}", originalFile.Substring(0, idxPeriod), "-convertedFromDb.csv");
            }
            else
            {
                result = string.Format("{0}{1}", originalFile.Substring(0, idxPeriod), "-converted.csv");
            }            

            return result;
        }

        private void button2_Click(object sender, EventArgs e)
        {
        }

        private void setProgress(string progress)
        {
            this.lblProgress.Invoke(new MethodInvoker(() => this.lblProgress.Text = progress));
        }

        private void btnConvertFromDB_Click(object sender, EventArgs e)
        {
            Task task = Task.Run((Action)getDataFromDB);
        }


        private  void getDataFromDB()
        {
            if (!string.IsNullOrEmpty(_fileName))
            {
                string path = getConvertedPath(_fileName, true);
                StreamWriter sw = File.CreateText(path);
                //sw.WriteLine("CreationDateShapes,CreationTimeShapes,Name,Version,ObjectId,ModelName,DeviceId,OS,ScreenHeight,ScreenWidth,Xdpi,Ydpi,UserCountry,AppLocale,ShapeObjectId,Instruction,StrokeObjectId,EventObjectId, EventTime, X, Y, Pressure, TouchSurface, AngleZ, AngleX, AngleY,IsHistory,ObjectIndex,StrokeIndex,EventIndex,ShapesIndex,PreX,PreY,PRE_EventTime,DownTime");

                string logfile = @"C:\temp\log.txt";
                StreamWriter swLog = File.CreateText(logfile);

                sw.WriteLine("TemplateId,GestureId,Name,ModelName,Xdpi,Ydpi,NumEvents,GestureIndex,Instruction,GestureAverageVelocity,GestureLength,GestureTotalTimeWithoutPauses,GestureTotalTimeWithPauses,GestureTotalStrokeArea,GestureMaxPressure,GestureMaxSurface,GestureAvgPressure,GestureAvgSurface,GestureAvgMiddlePressure,GestureAvgMiddleSurface,GestureMaxAccX,GestureMaxAccY,GestureMaxAccZ,GestureAvgAccX,GestureAvgAccY,GestureAvgAccZ,GestureAverageStartAcceleration,GestureAccumulatedLengthLinearRegIntercept,GestureAccumulatedLengthLinearRegRSqr,GestureAccumulatedLengthLinearRegSlope,GestureStartDirection,GestureEndDirection,VelocityMax,GestureVelocityPeakMaxIntervalPercentage,GestureVelocityPeakMax,GestureAccelerationPeakIntervalPercentage,GestureAccelerationPeakMax,AverageAcceleration,MaxAcceleration,GestureMaxDirection,GestureStartDirection,GestureEndDirection");

                StringBuilder strBuilder;

                List<ModelShapes> list = new List<ModelShapes>();
                
                this.lblStatus.Invoke(new MethodInvoker(() => this.lblStatus.Text = "Converting from DB..."));

                int totalNumbRecords = (int)listMongoCount;
                int limit = 1;
                int skip = 0;

                int startNumberToPrint = limit;

                if (!string.IsNullOrWhiteSpace(fromTextBox.Text) && Int32.Parse(fromTextBox.Text) > 0)
                {
                    skip = Int32.Parse(fromTextBox.Text) - 1;
                    startNumberToPrint = Int32.Parse(fromTextBox.Text);

                }

                if (!string.IsNullOrWhiteSpace(ToTextBox.Text) && Int32.Parse(ToTextBox.Text) > 0)
                {
                    totalNumbRecords = Int32.Parse(ToTextBox.Text);
                    if (totalNumbRecords >= listMongoCount)
                    {
                        totalNumbRecords = (int)(listMongoCount);
                    } 
                }

                setProgress(string.Format("Completed {0} out of {1} records", startNumberToPrint, totalNumbRecords));

                bool isFinished = false;
                int strokeCounter = 0;
                int shapesCounter = 0;
                int eventCounter = 0;
                int objectId = skip;

                string strGesture = string.Empty;

                IEnumerable<ModelShapes> shapesList;
                string id = string.Empty;

                try
                {
                    while (!isFinished)
                    {
                        shapesList = listMongo.FindAll().SetLimit(limit).SetSkip(skip);

                        foreach (ModelShapes shapes in shapesList)
                        {                            
                            try
                            {
                                if(ShapesValid(shapes))
                                {
                                    id = shapes._id.ToString();
                                    TemplateExtended template = UtilsConvert.ConvertTemplate(shapes);

                                    for (int idxGesture = 0; idxGesture < template.ListGestureExtended.size(); idxGesture++)
                                    {
                                        strGesture = UtilsConvert.GestureToString(shapes, shapes.ExpShapeList[idxGesture], (GestureExtended)template.ListGestureExtended.get(idxGesture), idxGesture);
                                        if(!string.IsNullOrEmpty(strGesture))
                                        {
                                            sw.WriteLine(strGesture);
                                        }
                                    }

                                    sw.Flush();
                                }                                    
                            }
                            catch (Exception exc)
                            {
                                swLog.WriteLine(string.Format("Error: {0}, ObjId:{1}", exc.StackTrace, id));
                                swLog.Flush();                                
                            }
                            
                        }
                        skip++;
                        if (totalNumbRecords == skip)
                        {
                            isFinished = true;
                        }
                        setProgress(string.Format("Completed {0} out of {1} records", skip, totalNumbRecords));
                    }

                    sw.Close();
                    swLog.Close();
                    this.lblStatus.Invoke(new MethodInvoker(() => this.lblStatus.Text = "Convertion from DB completed. Result file: " + path));
                }
                catch (Exception exc)
                {
                    this.lblStatus.Invoke(new MethodInvoker(() => this.lblStatus.Text = string.Format("Error: {0}, ObjId:{1}", exc.StackTrace, id)));

                    sw.Close();
                    swLog.Close();
                }

            }
            else
            {
                MessageBox.Show("You need to select a file");
            }

        }

        private bool ShapesValid(ModelShapes shapes)
        {
            bool isValid = true;
            if (string.Compare(shapes.ModelName, "LGE Nexus 5") != 0)
            {
                isValid = false;
            }
            if (shapes.Name.ToLower().Contains("prob"))
            {
                isValid = false;
            }
            if (shapes.Name.ToLower().Contains("stam"))
            {
                isValid = false;
            }
            return isValid;
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}