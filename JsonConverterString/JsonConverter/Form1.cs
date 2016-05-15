using flexjson;
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
using VerifyooLogic.UserProfile;

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
                result = string.Format("{0}{1}", originalFile.Substring(0, idxPeriod), "-convertedFromDb.txt");
            }
            else
            {
                result = string.Format("{0}{1}", originalFile.Substring(0, idxPeriod), "-converted.txt");
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
                //sw.WriteLine("Template");

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

                IEnumerable<ModelShapes> shapesList;

                JSONSerializer jsonSerializer = new JSONSerializer();
                String tempString;                

                try
                {
                    while (!isFinished)
                    {
                        shapesList = listMongo.FindAll().SetLimit(limit).SetSkip(skip);

                        Template tempTemplate;
                        CompactGesture tempGesture;
                        Stroke tempStroke;
                        MotionEventCompact tempEvent;                        

                        foreach (ModelShapes shapes in shapesList)
                        {                           
                            tempTemplate = new Template();
                            tempTemplate.ListGestures = new java.util.ArrayList();

                            tempTemplate.XDpi = shapes.Xdpi;
                            tempTemplate.YDpi = shapes.Ydpi;
                            
                            if (shapes.Name.ToLower().Contains("rafi"))
                            {
                                StringBuilder shapesBuilder = new StringBuilder();
                                shapesBuilder.Append(shapes.toString());
                                //shapesBuilder.Append(",");
                                objectId++;
                                shapesCounter = 0;
                                foreach (ModelShape obj in shapes.ExpShapeList)
                                {
                                    tempGesture = new CompactGesture();
                                    tempGesture.ListStrokes = new java.util.ArrayList();

                                    shapesCounter++;
                                    ModelMotionEventCompact prevEvent = null;
                                    StringBuilder shapeBuilder = new StringBuilder();
                                    shapeBuilder.Append(shapesBuilder.ToString());
                                    shapeBuilder.Append(obj.toString());
                                    //shapeBuilder.Append(",");

                                    sw.BaseStream.Seek(sw.BaseStream.Length, SeekOrigin.Begin);
                                    strokeCounter = 0;
                                    for (int idxStroke = 0; idxStroke < obj.Strokes.Count; idxStroke++)
                                    {
                                        tempStroke = new Stroke();
                                        tempStroke.ListEvents = new java.util.ArrayList();

                                        strokeCounter++;
                                        eventCounter = 0;
                                        for (int idxEvent = 0; idxEvent < obj.Strokes[idxStroke].ListEvents.Count; idxEvent++)
                                        {
                                            tempEvent = new MotionEventCompact();
                                            tempEvent.AngleX = obj.Strokes[idxStroke].ListEvents[idxEvent].AngleX;
                                            tempEvent.AngleY = obj.Strokes[idxStroke].ListEvents[idxEvent].AngleY;
                                            tempEvent.AngleZ = obj.Strokes[idxStroke].ListEvents[idxEvent].AngleZ;
                                            tempEvent.EventTime = obj.Strokes[idxStroke].ListEvents[idxEvent].EventTime;
                                            tempEvent.Pressure = obj.Strokes[idxStroke].ListEvents[idxEvent].Pressure;
                                            tempEvent.TouchSurface = obj.Strokes[idxStroke].ListEvents[idxEvent].TouchSurface;
                                            tempEvent.Xpixel = obj.Strokes[idxStroke].ListEvents[idxEvent].X;
                                            tempEvent.Ypixel = obj.Strokes[idxStroke].ListEvents[idxEvent].Y;

                                            tempStroke.ListEvents.add(tempEvent);
                                        }

                                        tempStroke.Length = obj.Strokes[idxStroke].Length;
                                        tempGesture.ListStrokes.add(tempStroke);
                                    }

                                    tempTemplate.ListGestures.add(tempGesture);
                                }

                                tempString = jsonSerializer.deepSerialize(tempTemplate);
                                tempString = tempString.Replace("\"", "'");
                                sw.WriteLine(tempString);
                                sw.Flush();
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
                    this.lblStatus.Invoke(new MethodInvoker(() => this.lblStatus.Text = "Convertion from DB completed. Result file: " + path));
                }
                catch (Exception exc)
                {
                    this.lblStatus.Invoke(new MethodInvoker(() => this.lblStatus.Text = "Error: " + exc.Message));

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