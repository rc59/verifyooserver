using JsonConverter.Logic;
using JsonConverter.Models;
using JsonConverter.Objects;
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

        MongoCollection<ModelShape> _Shapes;

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
            //int idxTemp, idxTempNext;
            //int numRecord = 1;
            //bool isComplete = false;
            //string strNumRecord, strNumRecordNext;
            //string tempStrObject;
            //string error;

            //if (!string.IsNullOrEmpty(_fileName))
            //{
            //    string path = getConvertedPath(_fileName);

            //    StreamWriter sw = File.CreateText(path);
            //    sw.WriteLine("Created,ObjectId,Name,ModelName,DeviceId,OS,Instruction,Match,MatchScore,IsSource,ScreenHeight,ScreenWidth,__v,Length,TimeInterval,PauseBeforeStroke,NumEvents,DownTime,UpTime,PressureMax,PressureMin,PressureAvg,TouchSurfaceMax,TouchSurfaceMin,TouchSurfaceAvg,Width,Height,Area,RelativePosX,RelativePosY,TouchEventId,EventTime,X,Y,Pressure,TouchSurface,AngleZ,AngleX,AngleY");

            //    StringBuilder strBuilder;

            //    lblStatus.Text = "Working...";
            //    while (!isComplete)
            //    {
            //        strNumRecord = string.Format("* {0} *", numRecord.ToString());
            //        strNumRecordNext = string.Format("* {0} *", (numRecord + 1).ToString());

            //        idxTemp = _text.IndexOf(strNumRecord);
            //        idxTempNext = _text.IndexOf(strNumRecordNext);

            //        if (idxTempNext > 0)
            //        {
            //            tempStrObject = _text.Substring(idxTemp + 8, idxTempNext - idxTemp - 9);
            //        }
            //        else
            //        {
            //            tempStrObject = _text.Substring(idxTemp + 8, _text.Length - 8 - idxTemp);
            //            isComplete = true;
            //        }

            //        tempStrObject = tempStrObject.Replace("\r\n", "");

            //        int tempIdxObjId;
            //        while (tempStrObject.IndexOf("ObjectId") > 0)
            //        {
            //            tempIdxObjId = tempStrObject.IndexOf("ObjectId");

            //            tempStrObject = tempStrObject.Remove(tempIdxObjId + 35, 1);
            //            tempStrObject = tempStrObject.Remove(tempIdxObjId, 9);
            //        }

            //        Shape obj = Activator.CreateInstance<Shape>();
            //        MemoryStream ms = new MemoryStream(Encoding.Unicode.GetBytes(tempStrObject));
            //        DataContractJsonSerializer serializer = new DataContractJsonSerializer(obj.GetType());

            //        try
            //        {
            //            obj = (Shape)serializer.ReadObject(ms);

            //            MotionEventCompact tempEvent;
            //            for (int idxStroke = 0; idxStroke < obj.Strokes.Count; idxStroke++)
            //            {
            //                for (int idxEvent = 0; idxEvent < obj.Strokes[idxStroke].ListEvents.Count; idxEvent++)
            //                {
            //                    tempEvent = obj.Strokes[idxStroke].ListEvents[idxEvent];

            //                    strBuilder = new StringBuilder();
            //                    strBuilder.Append(obj.toString());
            //                    strBuilder.Append(",");
            //                    strBuilder.Append(obj.Strokes[idxStroke].toString());
            //                    strBuilder.Append(",");
            //                    strBuilder.Append(obj.Strokes[idxStroke].ListEvents[idxEvent].toString());

            //                    sw.WriteLine(strBuilder.ToString());
            //                }

            //                sw.Flush();
            //            }
            //        }
            //        catch (Exception exc)
            //        {
            //            error = exc.Message;
            //            lblStatus.Text = string.Format("An error has occurred: {0}", error);
            //        }

            //        ms.Close();
            //        numRecord++;
            //    }

            //    lblStatus.Text = "Convertion completed. Result file: " + path;
            //    sw.Close();
            //}
            //else
            //{
            //    MessageBox.Show("You need to select a file");
            //}
        }

        private void setProgress(string progress)
        {
            lblProgress.Text = progress;
            lblProgress.Invalidate();
            lblProgress.Update();
            lblProgress.Refresh();
        }


        private double[] getScore(ModelShape shape)
        {
            int idx = -1;
            double[] scores = null;
            string instruction = shape.Instruction;

            if (instruction.ToLower().CompareTo(Consts.STR_INSTRUCTION_HEART.ToLower()) == 0)
            {
                idx = Consts.IDX_INSTRUCTION_HEART;
            }

            if (instruction.ToLower().CompareTo(Consts.STR_INSTRUCTION_TWO_HEARTS.ToLower()) == 0)
            {
                idx = Consts.IDX_INSTRUCTION_TWO_HEARTS;
            }

            if (instruction.ToLower().CompareTo(Consts.STR_INSTRUCTION_SHAPE.ToLower()) == 0)
            {
                idx = Consts.IDX_INSTRUCTION_SHAPE;
            }

            scores = getScoreByInstruction(shape, idx);

            return scores;
        }

        private double[] getScoreByInstruction(ModelShape shape, int idx)
        {
            double[] listScores = null;

            try
            {
                List<ModelShape> listToCompare = _baseShapes[idx];

                ModelShape tempShape;

                double[] listScoresTemp = null;
                double maxScore = 0;
                double tempScore = 0;
                int idxMax = 0;

                for (int idxShape = 0; idxShape < listToCompare.Count; idxShape++)
                {
                    tempShape = listToCompare[idxShape];
                    listScoresTemp = compareTwoShapes(shape, tempShape);
                    if (listScoresTemp == null)
                    {
                        break;
                    }
                    tempScore = getSum(listScoresTemp);

                    if (tempScore > maxScore)
                    {
                        listScores = listScoresTemp;
                        maxScore = tempScore;
                        idxMax = idxShape;
                    }
                }
            }
            catch (Exception exc)
            {
                string msg = exc.Message;
            }

            return listScores;
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

        private double[] compareTwoShapes(ModelShape shape, ModelShape shapeVerify)
        {
            double[] score = null;

            if (shape.Strokes.Count != shapeVerify.Strokes.Count)
            {
                return score;
            }
            UtilsShapesCompare comparer = new UtilsShapesCompare();

            score = new double[shape.Strokes.Count];
            for (int idxStroke = 0; idxStroke < shape.Strokes.Count; idxStroke++)
            {
                score[idxStroke] = comparer.CompareStrokes(shape.Strokes[idxStroke], shapeVerify.Strokes[idxStroke]);
            }

            return score;
        }

        private void initBaseShapes()
        {
            const string connectionString = "mongodb://52.26.178.48/?safe=true";
            var mongoClient = new MongoClient(connectionString);
            var mongoServer = mongoClient.GetServer();

            const string databaseName = "extserver-dev";
            MongoDatabase db = mongoServer.GetDatabase(databaseName);

            _Shapes = db.GetCollection<ModelShape>("shapes");

            string[] ids1 = { "56038cfad56cba140fc41d70", "560a95abcbf451500f071c99" };
            string[] ids2 = { "5617fb9ecbf451500f0a7504", "5618139acbf451500f0a8c64" };
            string[] ids3 = { "561a53fecbf451500f0afd09", "5603bba0d56cba140fc44301" };

            initInstruction(Consts.IDX_INSTRUCTION_SHAPE, ids1);
            initInstruction(Consts.IDX_INSTRUCTION_HEART, ids2);
            initInstruction(Consts.IDX_INSTRUCTION_TWO_HEARTS, ids3);
        }

        private void initInstruction(int idx, string[] ids)
        {
            ModelShape tempShape;

            List<ModelShape> listShapes = _baseShapes[idx];

            for (int idxShape = 0; idxShape < ids.Length; idxShape++)
            {
                tempShape = _Shapes.FindOneById(ObjectId.Parse(ids[idxShape]));
                if (tempShape != null)
                {
                    listShapes.Add(tempShape);
                }
            }            
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
            initBaseShapes();

            if (!string.IsNullOrEmpty(_fileName))
            {
                string path = getConvertedPath(_fileName, true);

                //FileStream fs = new FileStream(path, FileMode.CreateNew, FileAccess.Write);
                //StreamWriter sw = new StreamWriter(fs);
                StreamWriter sw = File.CreateText(path);
                sw.WriteLine("CreationDate,CreationTime,CreationTimeMS,ShapeObjectId,Name,ModelName,DeviceId,OS,Instruction,MatchShape,IsSource,ScreenHeight,ScreenWidth,StrokeObjectId,Length,MatchStroke,MatchStrokeScore,TimeInterval,PauseBeforeStroke,NumEvents,DownTime,UpTime,PressureMax,PressureMin,PressureAvg,TouchSurfaceMax,TouchSurfaceMin,TouchSurfaceAvg,Width,Height,Area,RelativePosX,RelativePosY,EventTime,X,Y,Pressure,TouchSurface,AngleZ,AngleX,AngleY,VelocityX,VelocityY,Velocity,AccelerationX,AccelerationY,Acceleration,Score1,Score2,Score3,Score4");

                StringBuilder strBuilder;

                MongoCollection<ModelShape> listMongo = UtilsDB.GetCollShapes();

                List<ModelShape> list = new List<ModelShape>();
                lblStatus.Text = "Converting from DB...";

                int totalNumbRecords = (int)listMongo.Count();
                int nPerPage = 5;
                int pageNumber = 1;
                int skip = 0;

                setProgress(string.Format("Completed {0} out of {1} records", pageNumber*nPerPage, totalNumbRecords));

                bool isFinished = false;
                int count = 0;

                double[] currentScores = null;

                IEnumerable<ModelShape> tempList;

                try
                {
                    while (!isFinished)
                    {
                        tempList = listMongo.FindAll().SetLimit(nPerPage).SetSkip(skip);
                        foreach (ModelShape obj in tempList)
                        {
                            currentScores = getScore(obj);

                            count++;
                            ModelMotionEventCompact tempEvent;
                            ModelMotionEventCompact prevEvent = null;

                            sw.BaseStream.Seek(sw.BaseStream.Length, SeekOrigin.Begin);
                            for (int idxStroke = 0; idxStroke < obj.Strokes.Count; idxStroke++)
                            {
                                for (int idxEvent = 0; idxEvent < obj.Strokes[idxStroke].ListEvents.Count; idxEvent++)
                                {
                                    if(idxEvent > 0)
                                    {
                                        prevEvent = obj.Strokes[idxStroke].ListEvents[idxEvent - 1];
                                    }

                                    tempEvent = obj.Strokes[idxStroke].ListEvents[idxEvent];

                                    strBuilder = new StringBuilder();
                                    strBuilder.Append(obj.toString());
                                    strBuilder.Append(",");
                                    strBuilder.Append(obj.Strokes[idxStroke].toString());
                                    strBuilder.Append(",");
                                    strBuilder.Append(obj.Strokes[idxStroke].ListEvents[idxEvent].toString(prevEvent));
                                    strBuilder.Append(",");
                                    appendScores(strBuilder, currentScores);

                                    sw.WriteLine(strBuilder.ToString());
                                }
                            }

                            sw.Flush();
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
                    //fs.Close();
                    lblStatus.Text = "Convertion from DB completed. Result file: " + path;
                }
                catch (Exception exc)
                {
                    lblStatus.Text = "Error: " + exc.Message;
                    sw.Close();
                    //fs.Close();                    
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