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
        string _text = string.Empty;
        string _fileName = string.Empty;

        MongoCollection<ModelShapes> _Shapes;

        List<List<ModelShape>> _baseShapes = new List<List<ModelShape>>();

        public Form1()
        {
            InitializeComponent();
            _baseShapes.Add(new List<ModelShape>());
            _baseShapes.Add(new List<ModelShape>());
            _baseShapes.Add(new List<ModelShape>());
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var FD = new System.Windows.Forms.OpenFileDialog();
            if (FD.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string fileToOpen = FD.FileName;

                //System.IO.FileInfo file = new System.IO.FileInfo(FD.FileName);

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
            lblProgress.Text = progress;
            lblProgress.Invalidate();
            lblProgress.Update();
            lblProgress.Refresh();
        }



        private double getSum(double[] scoreList)
        {
            double sum = 0;

            for(int idxScore = 0; idxScore < scoreList.Length; idxScore++)
            {
                sum += scoreList[idxScore];
            }

            return sum;
        }


        private void appendScores(StringBuilder strBuilder, double[] scores)
        {
            try
            {
                if (scores == null)
                {
                    strBuilder.Append(" , , , ");
                }
                else
                {
                    for (int idxScore = 0; idxScore < scores.Length; idxScore++)
                    {
                        strBuilder.Append(scores[idxScore]);
                        if (idxScore + 1 < scores.Length)
                        {
                            strBuilder.Append(",");
                        }
                    }

                    for (int idx = 0; idx < (4 - scores.Length); idx++)
                    {
                        if (idx < (4 - scores.Length))
                        {
                            strBuilder.Append(", ");
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                string msg = exc.Message;
            }

            
        }

        private void btnConvertFromDB_Click(object sender, EventArgs e)
        {

            if (!string.IsNullOrEmpty(_fileName))
            {
                string path = getConvertedPath(_fileName, true);
                StreamWriter sw = File.CreateText(path);
                sw.WriteLine("CreationDateShapes,CreationTimeShapes,CreationTimeMSShapes,Version,Name,ModelName,DeviceId,OS,IsSource, ScreenHeight, ScreenWidth,Xdpi,Ydpi,CreationDateStroke,CreationTimeStroke,CreationTimeMSStroke,ShapeObjectId,Instruction,StrokeObjectId,Length,EventObjectId, EventTime, X, Y, Pressure, TouchSurface, AngleZ, AngleX, AngleY, VelocityX, VelocityY, Velocity,ObjectIndex,StrokeIndex,EventIndex,shapesIndex");

                StringBuilder strBuilder;

                MongoCollection<ModelShapes> listMongo = UtilsDB.GetCollShapes();

                List<ModelShapes> list = new List<ModelShapes>();
                lblStatus.Text = "Converting from DB...";

                int totalNumbRecords = (int)listMongo.Count();
                int nPerPage = 5;
                int pageNumber = 1;
                int skip = 0;

                setProgress(string.Format("Completed {0} out of {1} records", pageNumber*nPerPage, totalNumbRecords));

                bool isFinished = false;
                int strokeCounter = 0;
                int shapesCounter = 0;
                int eventCounter = 0;
                int objectId = 0;

                IEnumerable<ModelShapes> shapesList;

                try
                {
                    while (!isFinished)
                    {
                        shapesList = listMongo.FindAll().SetLimit(nPerPage).SetSkip(skip);


                        foreach (ModelShapes shapes in shapesList)
                        {
                            StringBuilder shapesBuilder = new StringBuilder();
                            shapesBuilder.Append(shapes.toString());
                            shapesBuilder.Append(",");
                            objectId++;
                            shapesCounter = 0;
                            foreach (ModelShape obj in shapes.ExpShapeList)
                            {
                                shapesCounter++;
                                ModelMotionEventCompact tempEvent;
                                ModelMotionEventCompact prevEvent = null;
                                StringBuilder shapeBuilder = new StringBuilder();
                                shapeBuilder.Append(shapesBuilder.ToString());
                                shapeBuilder.Append(obj.toString());
;                               shapeBuilder.Append(",");

                                sw.BaseStream.Seek(sw.BaseStream.Length, SeekOrigin.Begin);
                                strokeCounter = 0;
                                for (int idxStroke = 0; idxStroke < obj.Strokes.Count; idxStroke++)
                                {
                                    strokeCounter++;
                                    eventCounter = 0;
                                    for (int idxEvent = 0; idxEvent < obj.Strokes[idxStroke].ListEvents.Count; idxEvent++)
                                    {
                                        eventCounter++;
                                        if (idxEvent > 0)
                                        {
                                            prevEvent = obj.Strokes[idxStroke].ListEvents[idxEvent - 1];
                                        }

                                        tempEvent = obj.Strokes[idxStroke].ListEvents[idxEvent];

                                        strBuilder = new StringBuilder();
                                        strBuilder.Append(shapeBuilder.ToString());
                                        strBuilder.Append(obj.Strokes[idxStroke].
                                            toString());
                                        strBuilder.Append(",");
                                        strBuilder.Append(obj.Strokes[idxStroke].ListEvents[idxEvent].toString(prevEvent));
                                        strBuilder.Append(",");
                                        strBuilder.Append(objectId.ToString());
                                        strBuilder.Append(",");
                                        strBuilder.Append(strokeCounter.ToString());
                                        strBuilder.Append(",");
                                        strBuilder.Append(eventCounter.ToString());
                                        strBuilder.Append(",");
                                        strBuilder.Append(shapesCounter.ToString());
                                        sw.WriteLine(strBuilder.ToString());
                                    }
                                }

                                sw.Flush();
                            }
                        }

                        pageNumber++;
                        skip = (pageNumber - 1) * nPerPage;
                        if (totalNumbRecords <= skip)
                        {
                            setProgress(string.Format("Completed {0} out of {1} records", totalNumbRecords, totalNumbRecords));
                            isFinished = true;
                        }
                        else
                        {
                            setProgress(string.Format("Completed {0} out of {1} records", pageNumber * nPerPage, totalNumbRecords));
                        }
                    }

                    sw.Close();
                    lblStatus.Text = "Convertion from DB completed. Result file: " + path;
                }
                catch (Exception exc)
                {
                    lblStatus.Text = "Error: " + exc.Message;
                    sw.Close();
                }

            }
            else
            {
                MessageBox.Show("You need to select a file");
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}