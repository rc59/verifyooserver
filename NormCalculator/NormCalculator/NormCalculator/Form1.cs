using Consts;
using Data.UserProfile.Extended;
using flexjson;
using java.util;
using JsonConverter.Models;
using Logic.Comparison.Stats.Norms;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using Newtonsoft.Json;
using NormCalculator.Models;
using NormCalculator.Utils;
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

namespace NormCalculator
{
    public partial class Form1 : Form
    {
        Dictionary<String, bool> mHashInvalid = new Dictionary<string, bool>();
        MongoCollection<ModelTemplate> mListMongo;

        /************************************* SPATIAL PARAMETERS - DISTANCE *************************************/

        SpatialNormContainer mNormContainerSpatialVelocitiesMeanDistance = new SpatialNormContainer();
        SpatialNormContainer mNormContainerSpatialVelocitiesStdDistance = new SpatialNormContainer();

        SpatialNormContainer mNormContainerSpatialAccelerationsMeanDistance = new SpatialNormContainer();
        SpatialNormContainer mNormContainerSpatialAccelerationsStdDistance = new SpatialNormContainer();

        SpatialNormContainer mNormContainerSpatialRadialVelocitiesMeanDistance = new SpatialNormContainer();
        SpatialNormContainer mNormContainerSpatialRadialVelocitiesStdDistance = new SpatialNormContainer();

        SpatialNormContainer mNormContainerSpatialRadialAccelerationsMeanDistance = new SpatialNormContainer();
        SpatialNormContainer mNormContainerSpatialRadialAccelerationsStdDistance = new SpatialNormContainer();

        SpatialNormContainer mNormContainerSpatialRadiusMeanDistance = new SpatialNormContainer();
        SpatialNormContainer mNormContainerSpatialRadiusStdDistance = new SpatialNormContainer();

        SpatialNormContainer mNormContainerSpatialTetaMeanDistance = new SpatialNormContainer();
        SpatialNormContainer mNormContainerSpatialTetaStdDistance = new SpatialNormContainer();

        SpatialNormContainer mNormContainerSpatialDeltaTetaMeanDistance = new SpatialNormContainer();
        SpatialNormContainer mNormContainerSpatialDeltaTetaStdDistance = new SpatialNormContainer();

        SpatialNormContainer mNormContainerSpatialAccumulatedNormalizedAreaMeanDistance = new SpatialNormContainer();
        SpatialNormContainer mNormContainerSpatialAccumulatedNormalizedAreaStdDistance = new SpatialNormContainer();

        /************************************* SPATIAL PARAMETERS - TIME *************************************/

        SpatialNormContainer mNormContainerSpatialVelocitiesMeanTime = new SpatialNormContainer();
        SpatialNormContainer mNormContainerSpatialVelocitiesStdTime = new SpatialNormContainer();

        SpatialNormContainer mNormContainerSpatialAccelerationsMeanTime = new SpatialNormContainer();
        SpatialNormContainer mNormContainerSpatialAccelerationsStdTime = new SpatialNormContainer();

        SpatialNormContainer mNormContainerSpatialRadialVelocitiesMeanTime = new SpatialNormContainer();
        SpatialNormContainer mNormContainerSpatialRadialVelocitiesStdTime = new SpatialNormContainer();

        SpatialNormContainer mNormContainerSpatialRadialAccelerationsMeanTime = new SpatialNormContainer();
        SpatialNormContainer mNormContainerSpatialRadialAccelerationsStdTime = new SpatialNormContainer();

        SpatialNormContainer mNormContainerSpatialRadiusMeanTime = new SpatialNormContainer();
        SpatialNormContainer mNormContainerSpatialRadiusStdTime = new SpatialNormContainer();

        SpatialNormContainer mNormContainerSpatialTetaMeanTime = new SpatialNormContainer();
        SpatialNormContainer mNormContainerSpatialTetaStdTime = new SpatialNormContainer();

        SpatialNormContainer mNormContainerSpatialDeltaTetaMeanTime = new SpatialNormContainer();
        SpatialNormContainer mNormContainerSpatialDeltaTetaStdTime = new SpatialNormContainer();

        SpatialNormContainer mNormContainerSpatialAccumulatedNormalizedAreaMeanTime = new SpatialNormContainer();
        SpatialNormContainer mNormContainerSpatialAccumulatedNormalizedAreaStdTime = new SpatialNormContainer();

        /************************************* NORMAL PARAMETERS *************************************/

        NumericNormContainer mNormContainerGestureNumEventsMean = new NumericNormContainer();
        NumericNormContainer mNormContainerGestureNumEventsStd = new NumericNormContainer();

        NumericNormContainer mNormContainerGestureLengthMean = new NumericNormContainer();
        NumericNormContainer mNormContainerGestureLengthStd = new NumericNormContainer();
        
        NumericNormContainer mNormContainerGestureTotalTimeIntervalMean = new NumericNormContainer();
        NumericNormContainer mNormContainerGestureTotalTimeIntervalStd = new NumericNormContainer();

        NumericNormContainer mNormContainerStrokeTotalTimeIntervalMean = new NumericNormContainer();
        NumericNormContainer mNormContainerStrokeTotalTimeIntervalStd = new NumericNormContainer();

        NumericNormContainer mNormContainerGestureTotalAreaMean = new NumericNormContainer();
        NumericNormContainer mNormContainerGestureTotalAreaStd = new NumericNormContainer();

        NumericNormContainer mNormContainerGestureTotalAreaMinXMinYMean = new NumericNormContainer();
        NumericNormContainer mNormContainerGestureTotalAreaMinXMinYStd = new NumericNormContainer();

        NumericNormContainer mNormContainerGestureStrokesTotalAreaMean = new NumericNormContainer();
        NumericNormContainer mNormContainerGestureStrokesTotalAreaStd = new NumericNormContainer();

        NumericNormContainer mNormContainerGestureStrokesTotalAreaMinXMinYMean = new NumericNormContainer();
        NumericNormContainer mNormContainerGestureStrokesTotalAreaMinXMinYStd = new NumericNormContainer();
        
        NumericNormContainer mNormContainerGestureMiddlePressureMean = new NumericNormContainer();
        NumericNormContainer mNormContainerGestureMiddlePressureStd = new NumericNormContainer();

        NumericNormContainer mNormContainerGestureMiddleSurfaceMean = new NumericNormContainer();
        NumericNormContainer mNormContainerGestureMiddleSurfaceStd = new NumericNormContainer();

        NumericNormContainer mNormContainerGestureAvgVelocityMean = new NumericNormContainer();
        NumericNormContainer mNormContainerGestureAvgVelocityStd = new NumericNormContainer();

        NumericNormContainer mNormContainerGestureMaxVelocityMean = new NumericNormContainer();
        NumericNormContainer mNormContainerGestureMaxVelocityStd = new NumericNormContainer();

        NumericNormContainer mNormContainerGestureMidFirstStrokeVelocityMean = new NumericNormContainer();
        NumericNormContainer mNormContainerGestureMidFirstStrokeVelocityStd = new NumericNormContainer();

        NumericNormContainer mNormContainerGestureAvgAccelerationMean = new NumericNormContainer();
        NumericNormContainer mNormContainerGestureAvgAccelerationStd = new NumericNormContainer();

        NumericNormContainer mNormContainerGestureMaxAccelerationMean = new NumericNormContainer();
        NumericNormContainer mNormContainerGestureMaxAccelerationStd = new NumericNormContainer();

        NumericNormContainer mNormContainerGestureAvgStartAccelerationMean = new NumericNormContainer();
        NumericNormContainer mNormContainerGestureAvgStartAccelerationStd = new NumericNormContainer();

        NumericNormContainer mNormContainerGestureAccumulatedLengthSlopeMean = new NumericNormContainer();
        NumericNormContainer mNormContainerGestureAccumulatedLengthSlopeStd = new NumericNormContainer();

        int mStrokesCount;
        int mCurrentTemplateNum;

        List<string> mListInstructions;

        public Form1()
        {
            InitializeComponent();
        }

        private void btnCalc_Click(object sender, EventArgs e)
        {
            Init();            
            Task task = Task.Run((Action)Start);            
        }

        private void Start()
        {
            IEnumerable<ModelTemplate> modelTemplates;           

            mListMongo = UtilsDB.GetCollShapes();
            double totalNumbRecords = mListMongo.Count();
            //double totalNumbRecords = mListMongo.Count();

            int limit = 1;
            int skip = 0;

            bool isFinished = false;            
            TemplateExtended tempTemplate;
            GestureExtended tempGesture;
            StrokeExtended tempStroke;
            MotionEventExtended tempEventDistance;
            MotionEventExtended tempEventTime;
            UtilsAccumulator tempAccumulator;            
            string msg;

            NumericNormContainer tempNormContainerGestureLength;
            NumericNormContainer tempNormContainerGestureNumEvents;

            SpatialNormContainer tempNormContainerVelocitiesDistance;
            SpatialNormContainer tempNormContainerRadialVelocitiesDistance;
            SpatialNormContainer tempNormContainerAccelerationDistance;
            SpatialNormContainer tempNormContainerRadialAccelerationDistance;
            SpatialNormContainer tempNormContainerRadiusDistance;
            SpatialNormContainer tempNormContainerTetaDistance;
            SpatialNormContainer tempNormContainerDeltaTetaDistance;
            SpatialNormContainer tempNormContainerAccumulatedNormalizedAreaDistance;

            SpatialNormContainer tempNormContainerVelocitiesTime;
            SpatialNormContainer tempNormContainerRadialVelocitiesTime;
            SpatialNormContainer tempNormContainerAccelerationTime;
            SpatialNormContainer tempNormContainerRadialAccelerationTime;
            SpatialNormContainer tempNormContainerRadiusTime;
            SpatialNormContainer tempNormContainerTetaTime;
            SpatialNormContainer tempNormContainerDeltaTetaTime;
            SpatialNormContainer tempNormContainerAccumulatedNormalizedAreaTime;

            NumericNormContainer tempNormContainerGestureTotalTimeInterval;
            NumericNormContainer tempNormContainerStrokeTotalTimeInterval;

            NumericNormContainer tempNormContainerGestureTotalArea;
            NumericNormContainer tempNormContainerGestureTotalAreaMinXMinY;

            NumericNormContainer tempNormContainerGestureStrokesTotalArea;
            NumericNormContainer tempNormContainerGestureStrokesTotalAreaMinXMinY;

            NumericNormContainer tempNormContainerGestureMiddlePressure;
            NumericNormContainer tempNormContainerGestureMiddleSurface;

            NumericNormContainer tempNormContainerGestureAverageVelocity;
            NumericNormContainer tempNormContainerGestureMaxVelocity;
            NumericNormContainer tempNormContainerMidFirstStrokeVelocity;
            
            NumericNormContainer tempNormContainerGestureAvgAcceleration;
            NumericNormContainer tempNormContainerGestureMaxAcceleration;
            NumericNormContainer tempNormContainerGestureAvgStartAcceleration;

            NumericNormContainer tempNormContainerGestureAccumulatedLengthSlope;
            
            bool isInitialized = false;
            string tempInstruction;

            try
            {
                while (!isFinished)
                {
                    modelTemplates = mListMongo.FindAll().SetLimit(limit).SetSkip(skip);
                    //modelTemplates = mListMongo.FindAll().SetLimit(limit).SetSkip(skip);

                    foreach (ModelTemplate template in modelTemplates)
                    {                        
                        mCurrentTemplateNum++;                            

                        tempTemplate = UtilsTemplateConverter.ConvertTemplate(template, mHashInvalid);

                        tempAccumulator = new UtilsAccumulator();

                        tempNormContainerGestureLength = new NumericNormContainer();
                        tempNormContainerGestureNumEvents = new NumericNormContainer();

                        tempNormContainerVelocitiesDistance = new SpatialNormContainer();
                        tempNormContainerRadialVelocitiesDistance = new SpatialNormContainer();
                        tempNormContainerAccelerationDistance = new SpatialNormContainer();
                        tempNormContainerRadialAccelerationDistance = new SpatialNormContainer();
                        tempNormContainerRadiusDistance = new SpatialNormContainer();
                        tempNormContainerTetaDistance = new SpatialNormContainer();
                        tempNormContainerDeltaTetaDistance = new SpatialNormContainer();
                        tempNormContainerAccumulatedNormalizedAreaDistance = new SpatialNormContainer();

                        tempNormContainerVelocitiesTime = new SpatialNormContainer();
                        tempNormContainerRadialVelocitiesTime = new SpatialNormContainer();
                        tempNormContainerAccelerationTime = new SpatialNormContainer();
                        tempNormContainerRadialAccelerationTime = new SpatialNormContainer();
                        tempNormContainerRadiusTime = new SpatialNormContainer();
                        tempNormContainerTetaTime = new SpatialNormContainer();
                        tempNormContainerDeltaTetaTime = new SpatialNormContainer();
                        tempNormContainerAccumulatedNormalizedAreaTime = new SpatialNormContainer();

                        tempNormContainerGestureTotalTimeInterval = new NumericNormContainer();
                        tempNormContainerStrokeTotalTimeInterval = new NumericNormContainer();

                        tempNormContainerGestureTotalArea = new NumericNormContainer();
                        tempNormContainerGestureTotalAreaMinXMinY = new NumericNormContainer();

                        tempNormContainerGestureStrokesTotalArea = new NumericNormContainer();
                        tempNormContainerGestureStrokesTotalAreaMinXMinY = new NumericNormContainer();

                        tempNormContainerGestureMiddlePressure = new NumericNormContainer();
                        tempNormContainerGestureMiddleSurface = new NumericNormContainer();

                        tempNormContainerGestureAverageVelocity = new NumericNormContainer();
                        tempNormContainerGestureMaxVelocity = new NumericNormContainer();
                        tempNormContainerMidFirstStrokeVelocity = new NumericNormContainer();

                        tempNormContainerGestureAvgStartAcceleration = new NumericNormContainer();
                        tempNormContainerGestureAvgAcceleration = new NumericNormContainer();
                        tempNormContainerGestureMaxAcceleration = new NumericNormContainer();

                        tempNormContainerGestureAccumulatedLengthSlope = new NumericNormContainer();

                        for (int idxGesture = 0; idxGesture < tempTemplate.ListGestureExtended.size(); idxGesture++)
                        {                              
                            tempGesture = (GestureExtended)tempTemplate.ListGestureExtended.get(idxGesture);
                            tempInstruction = tempGesture.Instruction;

                            for (int idxInstr = 0; idxInstr < mListInstructions.Count; idxInstr++) {
                                tempInstruction = mListInstructions[idxInstr];
                                if (tempInstruction.CompareTo("ALETTER") == 0 || tempGesture.Instruction.CompareTo("RLETTER") == 0)
                                {
                                    tempNormContainerGestureLength.AddValue(tempGesture.GestureLengthMM, tempInstruction);
                                    tempNormContainerGestureNumEvents.AddValue(tempGesture.ListGestureEventsExtended.size(), tempInstruction);

                                    tempNormContainerGestureTotalTimeInterval.AddValue(tempGesture.GestureTotalTimeInterval, tempInstruction);
                                    tempNormContainerStrokeTotalTimeInterval.AddValue(tempGesture.GestureTotalStrokeTimeInterval, tempInstruction);

                                    tempNormContainerGestureTotalArea.AddValue(tempGesture.GestureTotalArea, tempInstruction);
                                    tempNormContainerGestureTotalAreaMinXMinY.AddValue(tempGesture.GestureTotalAreaMinXMinY, tempInstruction);

                                    tempNormContainerGestureStrokesTotalArea.AddValue(tempGesture.GestureTotalStrokeArea, tempInstruction);
                                    tempNormContainerGestureStrokesTotalAreaMinXMinY.AddValue(tempGesture.GestureTotalStrokeAreaMinXMinY, tempInstruction);

                                    tempNormContainerGestureMiddlePressure.AddValue(tempGesture.GestureAvgMiddlePressure, tempInstruction);
                                    tempNormContainerGestureMiddleSurface.AddValue(tempGesture.GestureAvgMiddleSurface, tempInstruction);

                                    tempNormContainerGestureAverageVelocity.AddValue(tempGesture.GestureAverageVelocity, tempInstruction);
                                    tempNormContainerGestureMaxVelocity.AddValue(tempGesture.GestureMaxVelocity, tempInstruction);
                                    tempNormContainerMidFirstStrokeVelocity.AddValue(tempGesture.MidOfFirstStrokeVelocity, tempInstruction);

                                    tempNormContainerGestureAvgAcceleration.AddValue(tempGesture.GestureAverageAcceleration, tempInstruction);
                                    tempNormContainerGestureMaxAcceleration.AddValue(tempGesture.GestureMaxAcceleration, tempInstruction);
                                    tempNormContainerGestureAvgStartAcceleration.AddValue(tempGesture.GestureAverageStartAcceleration, tempInstruction);

                                    tempNormContainerGestureAccumulatedLengthSlope.AddValue(tempGesture.GestureAccumulatedLengthLinearRegSlope, tempInstruction);

                                    for (int idxStroke = 0; idxStroke < tempGesture.ListStrokesExtended.size(); idxStroke++)
                                    {
                                        tempStroke = (StrokeExtended)tempGesture.ListStrokesExtended.get(idxStroke);

                                        for (int idxEvent = 0; idxEvent < UtilsConsts.SAMPLE_SIZE; idxEvent++)
                                        {
                                            tempEventDistance = (MotionEventExtended)tempStroke.ListEventsSpatialByDistanceExtended.get(idxEvent);
                                            tempEventTime = (MotionEventExtended)tempStroke.ListEventsSpatialByTimeExtended.get(idxEvent);

                                            isInitialized = true;
                                            tempNormContainerVelocitiesDistance.AddValue(tempEventDistance.Velocity, tempInstruction, idxStroke, idxEvent);
                                            tempNormContainerRadialVelocitiesDistance.AddValue(tempEventDistance.RadialVelocity, tempInstruction, idxStroke, idxEvent);

                                            /********************************************* DISTANCE *********************************************/

                                            tempNormContainerRadialAccelerationDistance.AddValue(tempEventDistance.RadialAcceleration, tempInstruction, idxStroke, idxEvent);
                                            tempNormContainerRadiusDistance.AddValue(tempEventDistance.Radius, tempInstruction, idxStroke, idxEvent);
                                            tempNormContainerTetaDistance.AddValue(tempEventDistance.Teta, tempInstruction, idxStroke, idxEvent);
                                            tempNormContainerDeltaTetaDistance.AddValue(tempEventDistance.DeltaTeta, tempInstruction, idxStroke, idxEvent);
                                            tempNormContainerAccumulatedNormalizedAreaDistance.AddValue(tempEventDistance.AccumulatedNormalizedArea, tempInstruction, idxStroke, idxEvent);

                                            if (!Double.IsNaN(tempEventDistance.Acceleration))
                                            {
                                                tempNormContainerAccelerationDistance.AddValue(tempEventDistance.Acceleration, tempInstruction, idxStroke, idxEvent);
                                            }
                                            else
                                            {
                                                tempNormContainerAccelerationDistance.AddValue(0, tempInstruction, idxStroke, idxEvent);
                                            }

                                            /********************************************* TIME *********************************************/

                                            tempNormContainerVelocitiesTime.AddValue(tempEventTime.Velocity, tempInstruction, idxStroke, idxEvent);
                                            tempNormContainerRadialVelocitiesTime.AddValue(tempEventTime.RadialVelocity, tempInstruction, idxStroke, idxEvent);

                                            tempNormContainerRadialAccelerationTime.AddValue(tempEventTime.RadialAcceleration, tempInstruction, idxStroke, idxEvent);
                                            tempNormContainerRadiusTime.AddValue(tempEventTime.Radius, tempInstruction, idxStroke, idxEvent);
                                            tempNormContainerTetaTime.AddValue(tempEventTime.Teta, tempInstruction, idxStroke, idxEvent);
                                            tempNormContainerDeltaTetaTime.AddValue(tempEventTime.DeltaTeta, tempInstruction, idxStroke, idxEvent);
                                            tempNormContainerAccumulatedNormalizedAreaTime.AddValue(tempEventTime.AccumulatedNormalizedArea, tempInstruction, idxStroke, idxEvent);

                                            if (!Double.IsNaN(tempEventTime.Acceleration))
                                            {
                                                tempNormContainerAccelerationTime.AddValue(tempEventTime.Acceleration, tempInstruction, idxStroke, idxEvent);
                                            }
                                            else
                                            {
                                                tempNormContainerAccelerationTime.AddValue(0, tempInstruction, idxStroke, idxEvent);
                                            }
                                        }

                                        mStrokesCount++;

                                        msg = string.Format("Finished {0}/{1} Records", mCurrentTemplateNum, totalNumbRecords);
                                        this.lblStatus.Invoke(new MethodInvoker(() => this.lblStatus.Text = msg));
                                    }
                                }                                    
                            }                                
                        }

                        if (isInitialized)
                        {
                            isInitialized = false;

                            for (int idxInstruction = 0; idxInstruction < mListInstructions.Count; idxInstruction++)
                            {
                                tempInstruction = mListInstructions[idxInstruction];
                                for (int idxSpatial = 0; idxSpatial < ConstsGeneral.SPATIAL_SAMPLING_SIZE; idxSpatial++)
                                {
                                    /********************************************* DISTANCE *********************************************/

                                    AddValueToNormContainer(tempInstruction, idxSpatial, tempNormContainerVelocitiesDistance, mNormContainerSpatialVelocitiesMeanDistance, mNormContainerSpatialVelocitiesStdDistance);
                                    AddValueToNormContainer(tempInstruction, idxSpatial, tempNormContainerRadialVelocitiesDistance, mNormContainerSpatialRadialVelocitiesMeanDistance, mNormContainerSpatialRadialVelocitiesStdDistance);
                                    AddValueToNormContainer(tempInstruction, idxSpatial, tempNormContainerAccelerationDistance, mNormContainerSpatialAccelerationsMeanDistance, mNormContainerSpatialAccelerationsStdDistance);
                                    AddValueToNormContainer(tempInstruction, idxSpatial, tempNormContainerRadialAccelerationDistance, mNormContainerSpatialRadialAccelerationsMeanDistance, mNormContainerSpatialRadialAccelerationsStdDistance);
                                    AddValueToNormContainer(tempInstruction, idxSpatial, tempNormContainerRadiusDistance, mNormContainerSpatialRadiusMeanDistance, mNormContainerSpatialRadiusStdDistance);
                                    AddValueToNormContainer(tempInstruction, idxSpatial, tempNormContainerTetaDistance, mNormContainerSpatialTetaMeanDistance, mNormContainerSpatialTetaStdDistance);
                                    AddValueToNormContainer(tempInstruction, idxSpatial, tempNormContainerDeltaTetaDistance, mNormContainerSpatialDeltaTetaMeanDistance, mNormContainerSpatialDeltaTetaStdDistance);                                    
                                    AddValueToNormContainer(tempInstruction, idxSpatial, tempNormContainerAccumulatedNormalizedAreaDistance, mNormContainerSpatialAccumulatedNormalizedAreaMeanDistance, mNormContainerSpatialAccumulatedNormalizedAreaStdDistance);

                                    /********************************************* TIME *********************************************/

                                    AddValueToNormContainer(tempInstruction, idxSpatial, tempNormContainerVelocitiesTime, mNormContainerSpatialVelocitiesMeanTime, mNormContainerSpatialVelocitiesStdTime);
                                    AddValueToNormContainer(tempInstruction, idxSpatial, tempNormContainerRadialVelocitiesTime, mNormContainerSpatialRadialVelocitiesMeanTime, mNormContainerSpatialRadialVelocitiesStdTime);
                                    AddValueToNormContainer(tempInstruction, idxSpatial, tempNormContainerAccelerationTime, mNormContainerSpatialAccelerationsMeanTime, mNormContainerSpatialAccelerationsStdTime);
                                    AddValueToNormContainer(tempInstruction, idxSpatial, tempNormContainerRadialAccelerationTime, mNormContainerSpatialRadialAccelerationsMeanTime, mNormContainerSpatialRadialAccelerationsStdTime);
                                    AddValueToNormContainer(tempInstruction, idxSpatial, tempNormContainerRadiusTime, mNormContainerSpatialRadiusMeanTime, mNormContainerSpatialRadiusStdTime);
                                    AddValueToNormContainer(tempInstruction, idxSpatial, tempNormContainerTetaTime, mNormContainerSpatialTetaMeanTime, mNormContainerSpatialTetaStdTime);
                                    AddValueToNormContainer(tempInstruction, idxSpatial, tempNormContainerDeltaTetaTime, mNormContainerSpatialDeltaTetaMeanTime, mNormContainerSpatialDeltaTetaStdTime);
                                    AddValueToNormContainer(tempInstruction, idxSpatial, tempNormContainerAccumulatedNormalizedAreaTime, mNormContainerSpatialAccumulatedNormalizedAreaMeanTime, mNormContainerSpatialAccumulatedNormalizedAreaStdTime);
                                }

                                AddValueToNormContainer(tempInstruction, tempNormContainerGestureLength, mNormContainerGestureLengthMean, mNormContainerGestureLengthStd);
                                AddValueToNormContainer(tempInstruction, tempNormContainerGestureNumEvents, mNormContainerGestureNumEventsMean, mNormContainerGestureNumEventsStd);

                                AddValueToNormContainer(tempInstruction, tempNormContainerGestureTotalTimeInterval, mNormContainerGestureTotalTimeIntervalMean, mNormContainerGestureTotalTimeIntervalStd);
                                AddValueToNormContainer(tempInstruction, tempNormContainerStrokeTotalTimeInterval, mNormContainerStrokeTotalTimeIntervalMean, mNormContainerStrokeTotalTimeIntervalStd);

                                AddValueToNormContainer(tempInstruction, tempNormContainerGestureTotalArea, mNormContainerGestureTotalAreaMean, mNormContainerGestureTotalAreaStd);
                                AddValueToNormContainer(tempInstruction, tempNormContainerGestureTotalAreaMinXMinY, mNormContainerGestureTotalAreaMinXMinYMean, mNormContainerGestureTotalAreaMinXMinYStd);

                                AddValueToNormContainer(tempInstruction, tempNormContainerGestureStrokesTotalArea, mNormContainerGestureStrokesTotalAreaMean, mNormContainerGestureStrokesTotalAreaStd);
                                AddValueToNormContainer(tempInstruction, tempNormContainerGestureStrokesTotalAreaMinXMinY, mNormContainerGestureStrokesTotalAreaMinXMinYMean, mNormContainerGestureStrokesTotalAreaMinXMinYStd);

                                AddValueToNormContainer(tempInstruction, tempNormContainerGestureStrokesTotalArea, mNormContainerGestureStrokesTotalAreaMean, mNormContainerGestureStrokesTotalAreaStd);
                                AddValueToNormContainer(tempInstruction, tempNormContainerGestureStrokesTotalAreaMinXMinY, mNormContainerGestureStrokesTotalAreaMinXMinYMean, mNormContainerGestureStrokesTotalAreaMinXMinYStd);

                                AddValueToNormContainer(tempInstruction, tempNormContainerGestureMiddlePressure, mNormContainerGestureMiddlePressureMean, mNormContainerGestureMiddlePressureStd);
                                AddValueToNormContainer(tempInstruction, tempNormContainerGestureMiddleSurface, mNormContainerGestureMiddleSurfaceMean, mNormContainerGestureMiddleSurfaceStd);

                                AddValueToNormContainer(tempInstruction, tempNormContainerGestureAverageVelocity, mNormContainerGestureAvgVelocityMean, mNormContainerGestureAvgVelocityStd);
                                AddValueToNormContainer(tempInstruction, tempNormContainerGestureMaxVelocity, mNormContainerGestureMaxVelocityMean, mNormContainerGestureMaxVelocityStd);
                                AddValueToNormContainer(tempInstruction, tempNormContainerMidFirstStrokeVelocity, mNormContainerGestureMidFirstStrokeVelocityMean, mNormContainerGestureMidFirstStrokeVelocityStd);

                                AddValueToNormContainer(tempInstruction, tempNormContainerGestureAvgAcceleration, mNormContainerGestureAvgAccelerationMean, mNormContainerGestureAvgAccelerationStd);
                                AddValueToNormContainer(tempInstruction, tempNormContainerGestureMaxAcceleration, mNormContainerGestureMaxAccelerationMean, mNormContainerGestureMaxAccelerationStd);
                                AddValueToNormContainer(tempInstruction, tempNormContainerGestureAvgStartAcceleration, mNormContainerGestureAvgStartAccelerationMean, mNormContainerGestureAvgStartAccelerationStd);

                                AddValueToNormContainer(tempInstruction, tempNormContainerGestureAccumulatedLengthSlope, mNormContainerGestureAccumulatedLengthSlopeMean, mNormContainerGestureAccumulatedLengthSlopeStd);
                            }
                        }
                                                                   
                    }

                    skip += limit;

                    if (totalNumbRecords <= skip)
                    {
                        msg = string.Format("Finished {0}/{1} Records", totalNumbRecords, totalNumbRecords);
                        this.lblStatus.Invoke(new MethodInvoker(() => this.lblStatus.Text = msg));
                        isFinished = true;
                    }
                }

                CalculateNorms();
            }
            catch(Exception exc)
            {
                string err = exc.Message;
            }
        }

        private void AddValueToNormContainer(string instruction, NumericNormContainer normContainerInput, NumericNormContainer normContainerOutputMean, NumericNormContainer normContainerOutputSd)
        {
            double tempMean, tempStd;

            tempMean = normContainerInput.GetMean(instruction);
            tempStd = normContainerInput.GetStd(instruction);

            normContainerOutputMean.AddValue(tempMean, instruction);
            normContainerOutputSd.AddValue(tempStd, instruction);
        }

        private void AddValueToNormContainer(string instruction, int idxSpatial, SpatialNormContainer normContainerInput, SpatialNormContainer normContainerOutputMean, SpatialNormContainer normContainerOutputSd)
        {
            double tempMean, tempStd;

            bool isContinue = true;
            int idxCurrentStroke = 0;

            while(isContinue)
            {
                if(idxCurrentStroke > 10)
                {
                    isContinue = false;
                }

                try
                {
                    tempMean = normContainerInput.GetMean(instruction, idxCurrentStroke, idxSpatial);
                    tempStd = normContainerInput.GetStd(instruction, idxCurrentStroke, idxSpatial);

                    normContainerOutputMean.AddValue(tempMean, instruction, idxCurrentStroke, idxSpatial);
                    normContainerOutputSd.AddValue(tempStd, instruction, idxCurrentStroke, idxSpatial);

                    idxCurrentStroke++;
                }
                catch (Exception exc)
                {
                    isContinue = false;
                }
            }            
        }

        private void CalculateNorms()
        {
            NormContainerMgr normContainerMgr = new NormContainerMgr();

            /******************************************** SPATIAL PARAMETERS - DISTANCE ********************************************/

            normContainerMgr.HashMapSpatialNormsMeansDistance.put(ConstsParamNames.StrokeSpatial.VELOCITIES, mNormContainerSpatialVelocitiesMeanDistance);
            normContainerMgr.HashMapSpatialNormsSdsDistance.put(ConstsParamNames.StrokeSpatial.VELOCITIES, mNormContainerSpatialVelocitiesStdDistance);

            normContainerMgr.HashMapSpatialNormsMeansDistance.put(ConstsParamNames.StrokeSpatial.ACCELERATIONS, mNormContainerSpatialAccelerationsMeanDistance);
            normContainerMgr.HashMapSpatialNormsSdsDistance.put(ConstsParamNames.StrokeSpatial.ACCELERATIONS, mNormContainerSpatialAccelerationsStdDistance);

            normContainerMgr.HashMapSpatialNormsMeansDistance.put(ConstsParamNames.StrokeSpatial.RADIAL_VELOCITIES, mNormContainerSpatialRadialVelocitiesMeanDistance);
            normContainerMgr.HashMapSpatialNormsSdsDistance.put(ConstsParamNames.StrokeSpatial.RADIAL_VELOCITIES, mNormContainerSpatialRadialVelocitiesStdDistance);

            normContainerMgr.HashMapSpatialNormsMeansDistance.put(ConstsParamNames.StrokeSpatial.RADIAL_ACCELERATION, mNormContainerSpatialRadialAccelerationsMeanDistance);
            normContainerMgr.HashMapSpatialNormsSdsDistance.put(ConstsParamNames.StrokeSpatial.RADIAL_ACCELERATION, mNormContainerSpatialRadialAccelerationsStdDistance);

            normContainerMgr.HashMapSpatialNormsMeansDistance.put(ConstsParamNames.StrokeSpatial.RADIUS, mNormContainerSpatialRadiusMeanDistance);
            normContainerMgr.HashMapSpatialNormsSdsDistance.put(ConstsParamNames.StrokeSpatial.RADIUS, mNormContainerSpatialRadiusStdDistance);

            normContainerMgr.HashMapSpatialNormsMeansDistance.put(ConstsParamNames.StrokeSpatial.TETA, mNormContainerSpatialTetaMeanDistance);
            normContainerMgr.HashMapSpatialNormsSdsDistance.put(ConstsParamNames.StrokeSpatial.TETA, mNormContainerSpatialTetaStdDistance);

            normContainerMgr.HashMapSpatialNormsMeansDistance.put(ConstsParamNames.StrokeSpatial.DELTA_TETA, mNormContainerSpatialDeltaTetaMeanDistance);
            normContainerMgr.HashMapSpatialNormsSdsDistance.put(ConstsParamNames.StrokeSpatial.DELTA_TETA, mNormContainerSpatialDeltaTetaStdDistance);

            normContainerMgr.HashMapSpatialNormsMeansDistance.put(ConstsParamNames.StrokeSpatial.ACCUMULATED_NORM_AREA, mNormContainerSpatialAccumulatedNormalizedAreaMeanDistance);
            normContainerMgr.HashMapSpatialNormsSdsDistance.put(ConstsParamNames.StrokeSpatial.ACCUMULATED_NORM_AREA, mNormContainerSpatialAccumulatedNormalizedAreaStdDistance);

            /******************************************** SPATIAL PARAMETERS - TIME ********************************************/

            normContainerMgr.HashMapSpatialNormsMeansTime.put(ConstsParamNames.StrokeSpatial.VELOCITIES, mNormContainerSpatialVelocitiesMeanTime);
            normContainerMgr.HashMapSpatialNormsSdsTime.put(ConstsParamNames.StrokeSpatial.VELOCITIES, mNormContainerSpatialVelocitiesStdTime);

            normContainerMgr.HashMapSpatialNormsMeansTime.put(ConstsParamNames.StrokeSpatial.ACCELERATIONS, mNormContainerSpatialAccelerationsMeanTime);
            normContainerMgr.HashMapSpatialNormsSdsTime.put(ConstsParamNames.StrokeSpatial.ACCELERATIONS, mNormContainerSpatialAccelerationsStdTime);

            normContainerMgr.HashMapSpatialNormsMeansTime.put(ConstsParamNames.StrokeSpatial.RADIAL_VELOCITIES, mNormContainerSpatialRadialVelocitiesMeanTime);
            normContainerMgr.HashMapSpatialNormsSdsTime.put(ConstsParamNames.StrokeSpatial.RADIAL_VELOCITIES, mNormContainerSpatialRadialVelocitiesStdTime);

            normContainerMgr.HashMapSpatialNormsMeansTime.put(ConstsParamNames.StrokeSpatial.RADIAL_ACCELERATION, mNormContainerSpatialRadialAccelerationsMeanTime);
            normContainerMgr.HashMapSpatialNormsSdsTime.put(ConstsParamNames.StrokeSpatial.RADIAL_ACCELERATION, mNormContainerSpatialRadialAccelerationsStdTime);

            normContainerMgr.HashMapSpatialNormsMeansTime.put(ConstsParamNames.StrokeSpatial.RADIUS, mNormContainerSpatialRadiusMeanTime);
            normContainerMgr.HashMapSpatialNormsSdsTime.put(ConstsParamNames.StrokeSpatial.RADIUS, mNormContainerSpatialRadiusStdTime);

            normContainerMgr.HashMapSpatialNormsMeansTime.put(ConstsParamNames.StrokeSpatial.TETA, mNormContainerSpatialTetaMeanTime);
            normContainerMgr.HashMapSpatialNormsSdsTime.put(ConstsParamNames.StrokeSpatial.TETA, mNormContainerSpatialTetaStdTime);

            normContainerMgr.HashMapSpatialNormsMeansTime.put(ConstsParamNames.StrokeSpatial.DELTA_TETA, mNormContainerSpatialDeltaTetaMeanTime);
            normContainerMgr.HashMapSpatialNormsSdsTime.put(ConstsParamNames.StrokeSpatial.DELTA_TETA, mNormContainerSpatialDeltaTetaStdTime);

            normContainerMgr.HashMapSpatialNormsMeansTime.put(ConstsParamNames.StrokeSpatial.ACCUMULATED_NORM_AREA, mNormContainerSpatialAccumulatedNormalizedAreaMeanTime);
            normContainerMgr.HashMapSpatialNormsSdsTime.put(ConstsParamNames.StrokeSpatial.ACCUMULATED_NORM_AREA, mNormContainerSpatialAccumulatedNormalizedAreaStdTime);

            /******************************************** NORMAL PARAMETERS ********************************************/

            normContainerMgr.HashMapNumericNormsMeans.put(ConstsParamNames.Gesture.GESTURE_LENGTH, mNormContainerGestureLengthMean);
            normContainerMgr.HashMapNumericNormsSds.put(ConstsParamNames.Gesture.GESTURE_LENGTH, mNormContainerGestureLengthStd);

            normContainerMgr.HashMapNumericNormsMeans.put(ConstsParamNames.Gesture.GESTURE_NUM_EVENTS, mNormContainerGestureNumEventsMean);
            normContainerMgr.HashMapNumericNormsSds.put(ConstsParamNames.Gesture.GESTURE_NUM_EVENTS, mNormContainerGestureNumEventsStd);

            normContainerMgr.HashMapNumericNormsMeans.put(ConstsParamNames.Gesture.GESTURE_TOTAL_TIME_INTERVAL, mNormContainerGestureTotalTimeIntervalMean);
            normContainerMgr.HashMapNumericNormsSds.put(ConstsParamNames.Gesture.GESTURE_TOTAL_TIME_INTERVAL, mNormContainerGestureTotalTimeIntervalStd);

            normContainerMgr.HashMapNumericNormsMeans.put(ConstsParamNames.Gesture.GESTURE_TOTAL_STROKES_TIME_INTERVAL, mNormContainerStrokeTotalTimeIntervalMean);
            normContainerMgr.HashMapNumericNormsSds.put(ConstsParamNames.Gesture.GESTURE_TOTAL_STROKES_TIME_INTERVAL, mNormContainerStrokeTotalTimeIntervalStd);

            normContainerMgr.HashMapNumericNormsMeans.put(ConstsParamNames.Gesture.GESTURE_TOTAL_AREA, mNormContainerGestureTotalAreaMean);
            normContainerMgr.HashMapNumericNormsSds.put(ConstsParamNames.Gesture.GESTURE_TOTAL_AREA, mNormContainerGestureTotalAreaStd);

            normContainerMgr.HashMapNumericNormsMeans.put(ConstsParamNames.Gesture.GESTURE_TOTAL_AREA_MINX_MINY, mNormContainerGestureTotalAreaMinXMinYMean);
            normContainerMgr.HashMapNumericNormsSds.put(ConstsParamNames.Gesture.GESTURE_TOTAL_AREA_MINX_MINY, mNormContainerGestureTotalAreaMinXMinYStd);

            normContainerMgr.HashMapNumericNormsMeans.put(ConstsParamNames.Gesture.GESTURE_TOTAL_STROKE_AREA, mNormContainerGestureStrokesTotalAreaMean);
            normContainerMgr.HashMapNumericNormsSds.put(ConstsParamNames.Gesture.GESTURE_TOTAL_STROKE_AREA, mNormContainerGestureStrokesTotalAreaStd);

            normContainerMgr.HashMapNumericNormsMeans.put(ConstsParamNames.Gesture.GESTURE_TOTAL_STROKE_AREA_MINX_MINY, mNormContainerGestureStrokesTotalAreaMinXMinYMean);
            normContainerMgr.HashMapNumericNormsSds.put(ConstsParamNames.Gesture.GESTURE_TOTAL_STROKE_AREA_MINX_MINY, mNormContainerGestureStrokesTotalAreaMinXMinYStd);

            normContainerMgr.HashMapNumericNormsMeans.put(ConstsParamNames.Gesture.GESTURE_MIDDLE_PRESSURE, mNormContainerGestureMiddlePressureMean);
            normContainerMgr.HashMapNumericNormsSds.put(ConstsParamNames.Gesture.GESTURE_MIDDLE_PRESSURE, mNormContainerGestureMiddlePressureStd);

            normContainerMgr.HashMapNumericNormsMeans.put(ConstsParamNames.Gesture.GESTURE_MIDDLE_SURFACE, mNormContainerGestureMiddleSurfaceMean);
            normContainerMgr.HashMapNumericNormsSds.put(ConstsParamNames.Gesture.GESTURE_MIDDLE_SURFACE, mNormContainerGestureMiddleSurfaceStd);

            normContainerMgr.HashMapNumericNormsMeans.put(ConstsParamNames.Gesture.GESTURE_AVERAGE_VELOCITY, mNormContainerGestureAvgVelocityMean);
            normContainerMgr.HashMapNumericNormsSds.put(ConstsParamNames.Gesture.GESTURE_AVERAGE_VELOCITY, mNormContainerGestureAvgVelocityStd);

            normContainerMgr.HashMapNumericNormsMeans.put(ConstsParamNames.Gesture.GESTURE_MAX_VELOCITY, mNormContainerGestureMaxVelocityMean);
            normContainerMgr.HashMapNumericNormsSds.put(ConstsParamNames.Gesture.GESTURE_MAX_VELOCITY, mNormContainerGestureMaxVelocityStd);

            normContainerMgr.HashMapNumericNormsMeans.put(ConstsParamNames.Gesture.GESTURE_MID_OF_FIRST_STROKE_VELOCITY, mNormContainerGestureMidFirstStrokeVelocityMean);
            normContainerMgr.HashMapNumericNormsSds.put(ConstsParamNames.Gesture.GESTURE_MID_OF_FIRST_STROKE_VELOCITY, mNormContainerGestureMidFirstStrokeVelocityStd);

            normContainerMgr.HashMapNumericNormsMeans.put(ConstsParamNames.Gesture.GESTURE_AVG_ACCELERATION, mNormContainerGestureAvgAccelerationMean);
            normContainerMgr.HashMapNumericNormsSds.put(ConstsParamNames.Gesture.GESTURE_AVG_ACCELERATION, mNormContainerGestureAvgAccelerationStd);

            normContainerMgr.HashMapNumericNormsMeans.put(ConstsParamNames.Gesture.GESTURE_MAX_ACCELERATION, mNormContainerGestureMaxAccelerationMean);
            normContainerMgr.HashMapNumericNormsSds.put(ConstsParamNames.Gesture.GESTURE_MAX_ACCELERATION, mNormContainerGestureMaxAccelerationStd);

            normContainerMgr.HashMapNumericNormsMeans.put(ConstsParamNames.Gesture.GESTURE_AVG_START_ACCELERATION, mNormContainerGestureAvgStartAccelerationMean);
            normContainerMgr.HashMapNumericNormsSds.put(ConstsParamNames.Gesture.GESTURE_AVG_START_ACCELERATION, mNormContainerGestureAvgStartAccelerationStd);

            normContainerMgr.HashMapNumericNormsMeans.put(ConstsParamNames.Gesture.GESTURE_ACCUMULATED_LENGTH_SLOPE, mNormContainerGestureAccumulatedLengthSlopeMean);
            normContainerMgr.HashMapNumericNormsSds.put(ConstsParamNames.Gesture.GESTURE_ACCUMULATED_LENGTH_SLOPE, mNormContainerGestureAccumulatedLengthSlopeStd);

            /********************************************************************************************************************************/

            double popMeanGestureLength = normContainerMgr.GetNumericNormPopMean("SLETTER", ConstsParamNames.Gesture.GESTURE_LENGTH);
            double popSdGestureLength = normContainerMgr.GetNumericNormPopSd("SLETTER", ConstsParamNames.Gesture.GESTURE_LENGTH);
            double internalMeanGestureLength = normContainerMgr.GetNumericNormInternalSd("SLETTER", ConstsParamNames.Gesture.GESTURE_LENGTH);

            double popMeanGestureNumEvents = normContainerMgr.GetNumericNormPopMean("SLETTER", ConstsParamNames.Gesture.GESTURE_NUM_EVENTS);
            double popSdGestureNumEvents = normContainerMgr.GetNumericNormPopSd("SLETTER", ConstsParamNames.Gesture.GESTURE_NUM_EVENTS);
            double internalMeanGestureNumEvents = normContainerMgr.GetNumericNormInternalSd("SLETTER", ConstsParamNames.Gesture.GESTURE_NUM_EVENTS);

            double popMeanGestureTotalTimeInterval = normContainerMgr.GetNumericNormPopMean("SLETTER", ConstsParamNames.Gesture.GESTURE_TOTAL_TIME_INTERVAL);
            double popSdGestureTotalTimeInterval = normContainerMgr.GetNumericNormPopSd("SLETTER", ConstsParamNames.Gesture.GESTURE_TOTAL_TIME_INTERVAL);
            double internalMeanGestureTotalTimeInterval = normContainerMgr.GetNumericNormInternalSd("SLETTER", ConstsParamNames.Gesture.GESTURE_TOTAL_TIME_INTERVAL);

            double popMeanGestureStrokesTotalTimeInterval = normContainerMgr.GetNumericNormPopMean("SLETTER", ConstsParamNames.Gesture.GESTURE_TOTAL_STROKES_TIME_INTERVAL);
            double popSdGestureStrokesTotalTimeInterval = normContainerMgr.GetNumericNormPopSd("SLETTER", ConstsParamNames.Gesture.GESTURE_TOTAL_STROKES_TIME_INTERVAL);
            double internalMeanGestureStrokesTotalTimeInterval = normContainerMgr.GetNumericNormInternalSd("SLETTER", ConstsParamNames.Gesture.GESTURE_TOTAL_STROKES_TIME_INTERVAL);

            double popMeanGestureTotalArea = normContainerMgr.GetNumericNormPopMean("SLETTER", ConstsParamNames.Gesture.GESTURE_TOTAL_AREA);
            double popSdGestureTotalArea = normContainerMgr.GetNumericNormPopSd("SLETTER", ConstsParamNames.Gesture.GESTURE_TOTAL_AREA);
            double internalMeanTotalArea = normContainerMgr.GetNumericNormInternalSd("SLETTER", ConstsParamNames.Gesture.GESTURE_TOTAL_AREA);

            double popMeanGestureTotalAreaMinXMinY = normContainerMgr.GetNumericNormPopMean("SLETTER", ConstsParamNames.Gesture.GESTURE_TOTAL_AREA_MINX_MINY);
            double popSdGestureTotalAreaMinXMinY = normContainerMgr.GetNumericNormPopSd("SLETTER", ConstsParamNames.Gesture.GESTURE_TOTAL_AREA_MINX_MINY);
            double internalMeanTotalAreaMinXMinY = normContainerMgr.GetNumericNormInternalSd("SLETTER", ConstsParamNames.Gesture.GESTURE_TOTAL_AREA_MINX_MINY);

            double popMeanGestureTotalStrokeArea = normContainerMgr.GetNumericNormPopMean("SLETTER", ConstsParamNames.Gesture.GESTURE_TOTAL_STROKE_AREA);
            double popSdGestureTotalAreaStroke = normContainerMgr.GetNumericNormPopSd("SLETTER", ConstsParamNames.Gesture.GESTURE_TOTAL_STROKE_AREA);
            double internalMeanTotalAreaStroke = normContainerMgr.GetNumericNormInternalSd("SLETTER", ConstsParamNames.Gesture.GESTURE_TOTAL_STROKE_AREA);

            double popMeanGestureTotalStrokeAreaMinXMinY = normContainerMgr.GetNumericNormPopMean("SLETTER", ConstsParamNames.Gesture.GESTURE_TOTAL_STROKE_AREA_MINX_MINY);
            double popSdGestureTotalAreaStrokeMinXMinY = normContainerMgr.GetNumericNormPopSd("SLETTER", ConstsParamNames.Gesture.GESTURE_TOTAL_STROKE_AREA_MINX_MINY);
            double internalMeanTotalAreaStrokeMinXMinY = normContainerMgr.GetNumericNormInternalSd("SLETTER", ConstsParamNames.Gesture.GESTURE_TOTAL_STROKE_AREA_MINX_MINY);

            double popMeanGestureMiddlePressure = normContainerMgr.GetNumericNormPopMean("SLETTER", ConstsParamNames.Gesture.GESTURE_MIDDLE_PRESSURE);
            double popSdGestureMiddlePressure = normContainerMgr.GetNumericNormPopSd("SLETTER", ConstsParamNames.Gesture.GESTURE_MIDDLE_PRESSURE);
            double internalMeanMiddlePressure = normContainerMgr.GetNumericNormInternalSd("SLETTER", ConstsParamNames.Gesture.GESTURE_MIDDLE_PRESSURE);

            double popMeanGestureMiddleSurface = normContainerMgr.GetNumericNormPopMean("SLETTER", ConstsParamNames.Gesture.GESTURE_MIDDLE_SURFACE);
            double popSdGestureMiddleSurface = normContainerMgr.GetNumericNormPopSd("SLETTER", ConstsParamNames.Gesture.GESTURE_MIDDLE_SURFACE);
            double internalMeanMiddleSurface = normContainerMgr.GetNumericNormInternalSd("SLETTER", ConstsParamNames.Gesture.GESTURE_MIDDLE_SURFACE);

            double popMeanGestureAvgVelocity = normContainerMgr.GetNumericNormPopMean("SLETTER", ConstsParamNames.Gesture.GESTURE_AVERAGE_VELOCITY);
            double popSdGestureAvgVelocity = normContainerMgr.GetNumericNormPopSd("SLETTER", ConstsParamNames.Gesture.GESTURE_AVERAGE_VELOCITY);
            double internalMeanAvgVelocity = normContainerMgr.GetNumericNormInternalSd("SLETTER", ConstsParamNames.Gesture.GESTURE_AVERAGE_VELOCITY);

            double popMeanGestureMaxVelocity = normContainerMgr.GetNumericNormPopMean("SLETTER", ConstsParamNames.Gesture.GESTURE_MAX_VELOCITY);
            double popSdGestureMaxVelocity = normContainerMgr.GetNumericNormPopSd("SLETTER", ConstsParamNames.Gesture.GESTURE_MAX_VELOCITY);
            double internalMeanMaxVelocity = normContainerMgr.GetNumericNormInternalSd("SLETTER", ConstsParamNames.Gesture.GESTURE_MAX_VELOCITY);

            double popMeanGestureMidFirstStrokeVelocity = normContainerMgr.GetNumericNormPopMean("SLETTER", ConstsParamNames.Gesture.GESTURE_MID_OF_FIRST_STROKE_VELOCITY);
            double popSdGestureMidFirstStrokeVelocity = normContainerMgr.GetNumericNormPopSd("SLETTER", ConstsParamNames.Gesture.GESTURE_MID_OF_FIRST_STROKE_VELOCITY);
            double internalMeanMidFirstStrokeVelocity = normContainerMgr.GetNumericNormInternalSd("SLETTER", ConstsParamNames.Gesture.GESTURE_MID_OF_FIRST_STROKE_VELOCITY);

            double popMeanGestureAvgAcceleration = normContainerMgr.GetNumericNormPopMean("SLETTER", ConstsParamNames.Gesture.GESTURE_AVG_ACCELERATION);
            double popSdGestureAvgAcceleration = normContainerMgr.GetNumericNormPopSd("SLETTER", ConstsParamNames.Gesture.GESTURE_AVG_ACCELERATION);
            double internalMeanAvgAcceleration = normContainerMgr.GetNumericNormInternalSd("SLETTER", ConstsParamNames.Gesture.GESTURE_AVG_ACCELERATION);

            double popMeanGestureMaxAcceleration = normContainerMgr.GetNumericNormPopMean("SLETTER", ConstsParamNames.Gesture.GESTURE_MAX_ACCELERATION);
            double popSdGestureMaxAcceleration = normContainerMgr.GetNumericNormPopSd("SLETTER", ConstsParamNames.Gesture.GESTURE_MAX_ACCELERATION);
            double internalMeanMaxAcceleration = normContainerMgr.GetNumericNormInternalSd("SLETTER", ConstsParamNames.Gesture.GESTURE_MAX_ACCELERATION);

            double popMeanGestureAvgStartAcceleration = normContainerMgr.GetNumericNormPopMean("SLETTER", ConstsParamNames.Gesture.GESTURE_AVG_START_ACCELERATION);
            double popSdGestureAvgStartAcceleration = normContainerMgr.GetNumericNormPopSd("SLETTER", ConstsParamNames.Gesture.GESTURE_AVG_START_ACCELERATION);
            double internalMeanAvgStartAcceleration = normContainerMgr.GetNumericNormInternalSd("SLETTER", ConstsParamNames.Gesture.GESTURE_AVG_START_ACCELERATION);

            double popMeanGestureAccumulatedLengthSlope = normContainerMgr.GetNumericNormPopMean("SLETTER", ConstsParamNames.Gesture.GESTURE_ACCUMULATED_LENGTH_SLOPE);
            double popSdGestureAccumulatedLengthSlope = normContainerMgr.GetNumericNormPopSd("SLETTER", ConstsParamNames.Gesture.GESTURE_ACCUMULATED_LENGTH_SLOPE);
            double internalMeanGestureAccumulatedLengthSlope = normContainerMgr.GetNumericNormInternalSd("SLETTER", ConstsParamNames.Gesture.GESTURE_ACCUMULATED_LENGTH_SLOPE);            

            JSONSerializer jsonSerializer = new JSONSerializer();
            string jsonNormContainerMgr = jsonSerializer.deepSerialize(normContainerMgr);

            ModelNormContainerMgr modelNormContainerMgr = new ModelNormContainerMgr(normContainerMgr);
            //NormContainerMgr normContainerMgr2 = modelNormContainerMgr.ToNormContainerMgr();
            //ModelNormContainerMgr modelNormContainerMgr2 = new ModelNormContainerMgr(normContainerMgr2);
            //NormContainerMgr normContainerMgr3 = modelNormContainerMgr2.ToNormContainerMgr();

            try
            {
                string jsonNormContainerMgrCsharp = JsonConvert.SerializeObject(modelNormContainerMgr);
                ModelNormContainerMgr tempNormContainerMgr = JsonConvert.DeserializeObject<ModelNormContainerMgr>(jsonNormContainerMgrCsharp);
                NormContainerMgr normContainerMgr3 = tempNormContainerMgr.ToNormContainerMgr();
            }
            catch (Exception exc)
            {
                string msg = exc.Message;
            }            
        }

        private void Init()
        {
            mStrokesCount = 0;
            mCurrentTemplateNum = 0;                        

            UtilsInstructions utilsInstructions = new UtilsInstructions();
            mListInstructions = utilsInstructions.GetInstructions();
        }
    }
}
