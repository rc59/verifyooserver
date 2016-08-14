﻿using Consts;
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
        string mPathFaultyGestures;
        Dictionary<String, bool> mHashInvalid;
        MongoCollection<ModelTemplate> mListMongo;

        /************************************* SPATIAL PARAMETERS *************************************/

        SpatialNormContainer mNormContainerSpatialVelocitiesMean = new SpatialNormContainer();
        SpatialNormContainer mNormContainerSpatialVelocitiesStd = new SpatialNormContainer();

        SpatialNormContainer mNormContainerSpatialAccelerationsMean = new SpatialNormContainer();
        SpatialNormContainer mNormContainerSpatialAccelerationsStd = new SpatialNormContainer();

        SpatialNormContainer mNormContainerSpatialRadialVelocitiesMean = new SpatialNormContainer();
        SpatialNormContainer mNormContainerSpatialRadialVelocitiesStd = new SpatialNormContainer();

        SpatialNormContainer mNormContainerSpatialRadialAccelerationsMean = new SpatialNormContainer();
        SpatialNormContainer mNormContainerSpatialRadialAccelerationsStd = new SpatialNormContainer();

        SpatialNormContainer mNormContainerSpatialRadiusMean = new SpatialNormContainer();
        SpatialNormContainer mNormContainerSpatialRadiusStd = new SpatialNormContainer();

        SpatialNormContainer mNormContainerSpatialTetaMean = new SpatialNormContainer();
        SpatialNormContainer mNormContainerSpatialTetaStd = new SpatialNormContainer();

        SpatialNormContainer mNormContainerSpatialDeltaTetaMean = new SpatialNormContainer();
        SpatialNormContainer mNormContainerSpatialDeltaTetaStd = new SpatialNormContainer();

        SpatialNormContainer mNormContainerSpatialAccumulatedNormalizedAreaMean = new SpatialNormContainer();
        SpatialNormContainer mNormContainerSpatialAccumulatedNormalizedAreaStd = new SpatialNormContainer();

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
            ReadFaultyGestures();
            Task task = Task.Run((Action)Start);            
        }

        private void Start()
        {
            IEnumerable<ModelTemplate> modelTemplates;

            IMongoQuery query = Query<ModelTemplate>.EQ(c => c.ModelName, "LGE Nexus 5");

            mListMongo = UtilsDB.GetCollShapes();
            double totalNumbRecords = mListMongo.Count(query);
            //double totalNumbRecords = mListMongo.Count();

            int limit = 1;
            int skip = 0;

            bool isFinished = false;            
            TemplateExtended tempTemplate;
            GestureExtended tempGesture;
            StrokeExtended tempStroke;
            MotionEventExtended tempEvent;
            UtilsAccumulator tempAccumulator;            
            string msg;

            NumericNormContainer tempNormContainerGestureLength;
            NumericNormContainer tempNormContainerGestureNumEvents;

            SpatialNormContainer tempNormContainerVelocities;
            SpatialNormContainer tempNormContainerRadialVelocities;
            SpatialNormContainer tempNormContainerAcceleration;
            SpatialNormContainer tempNormContainerRadialAcceleration;
            SpatialNormContainer tempNormContainerRadius;
            SpatialNormContainer tempNormContainerTeta;
            SpatialNormContainer tempNormContainerDeltaTeta;
            SpatialNormContainer tempNormContainerAccumulatedNormalizedArea;

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
                    modelTemplates = mListMongo.Find(query).SetLimit(limit).SetSkip(skip);
                    //modelTemplates = mListMongo.FindAll().SetLimit(limit).SetSkip(skip);

                    foreach (ModelTemplate template in modelTemplates)
                    {
                        if (!template.Name.ToLower().Contains("stam")) {
                            mCurrentTemplateNum++;                            

                            tempTemplate = UtilsTemplateConverter.ConvertTemplate(template, mHashInvalid);

                            tempAccumulator = new UtilsAccumulator();

                            tempNormContainerGestureLength = new NumericNormContainer();
                            tempNormContainerGestureNumEvents = new NumericNormContainer();

                            tempNormContainerVelocities = new SpatialNormContainer();
                            tempNormContainerRadialVelocities = new SpatialNormContainer();
                            tempNormContainerAcceleration = new SpatialNormContainer();
                            tempNormContainerRadialAcceleration = new SpatialNormContainer();
                            tempNormContainerRadius = new SpatialNormContainer();
                            tempNormContainerTeta = new SpatialNormContainer();
                            tempNormContainerDeltaTeta = new SpatialNormContainer();
                            tempNormContainerAccumulatedNormalizedArea = new SpatialNormContainer();

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
                                    if (tempInstruction.CompareTo("ALETTER") != 0 || tempGesture.Instruction.CompareTo("ALETTER") != 0)
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
                                                tempEvent = (MotionEventExtended)tempStroke.ListEventsSpatialByDistanceExtended.get(idxEvent);

                                                isInitialized = true;
                                                tempNormContainerVelocities.AddValue(tempEvent.Velocity, tempInstruction, idxStroke, idxEvent);
                                                tempNormContainerRadialVelocities.AddValue(tempEvent.RadialVelocity, tempInstruction, idxStroke, idxEvent);

                                                tempNormContainerRadialAcceleration.AddValue(tempEvent.RadialAcceleration, tempInstruction, idxStroke, idxEvent);
                                                tempNormContainerRadius.AddValue(tempEvent.Radius, tempInstruction, idxStroke, idxEvent);
                                                tempNormContainerTeta.AddValue(tempEvent.Teta, tempInstruction, idxStroke, idxEvent);
                                                tempNormContainerDeltaTeta.AddValue(tempEvent.DeltaTeta, tempInstruction, idxStroke, idxEvent);
                                                tempNormContainerAccumulatedNormalizedArea.AddValue(tempEvent.AccumulatedNormalizedArea, tempInstruction, idxStroke, idxEvent);                                               

                                                if (!Double.IsNaN(tempEvent.Acceleration))
                                                {
                                                    tempNormContainerAcceleration.AddValue(tempEvent.Acceleration, tempInstruction, idxStroke, idxEvent);
                                                }
                                                else
                                                {
                                                    tempNormContainerAcceleration.AddValue(0, tempInstruction, idxStroke, idxEvent);
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
                                        AddValueToNormContainer(tempInstruction, idxSpatial, tempNormContainerVelocities, mNormContainerSpatialVelocitiesMean, mNormContainerSpatialVelocitiesStd);
                                        AddValueToNormContainer(tempInstruction, idxSpatial, tempNormContainerRadialVelocities, mNormContainerSpatialRadialVelocitiesMean, mNormContainerSpatialRadialVelocitiesStd);
                                        AddValueToNormContainer(tempInstruction, idxSpatial, tempNormContainerAcceleration, mNormContainerSpatialAccelerationsMean, mNormContainerSpatialAccelerationsStd);
                                        AddValueToNormContainer(tempInstruction, idxSpatial, tempNormContainerRadialAcceleration, mNormContainerSpatialRadialAccelerationsMean, mNormContainerSpatialRadialAccelerationsStd);                                    
                                        AddValueToNormContainer(tempInstruction, idxSpatial, tempNormContainerRadius, mNormContainerSpatialRadiusMean, mNormContainerSpatialRadiusStd);
                                        AddValueToNormContainer(tempInstruction, idxSpatial, tempNormContainerTeta, mNormContainerSpatialTetaMean, mNormContainerSpatialTetaStd);
                                        AddValueToNormContainer(tempInstruction, idxSpatial, tempNormContainerDeltaTeta, mNormContainerSpatialDeltaTetaMean, mNormContainerSpatialDeltaTetaStd);                                    
                                        AddValueToNormContainer(tempInstruction, idxSpatial, tempNormContainerAccumulatedNormalizedArea, mNormContainerSpatialAccumulatedNormalizedAreaMean, mNormContainerSpatialAccumulatedNormalizedAreaStd);
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

            /******************************************** SPATIAL PARAMETERS ********************************************/

            normContainerMgr.HashMapSpatialNormsMeans.put(ConstsParamNames.StrokeSpatial.VELOCITIES, mNormContainerSpatialVelocitiesMean);
            normContainerMgr.HashMapSpatialNormsSds.put(ConstsParamNames.StrokeSpatial.VELOCITIES, mNormContainerSpatialVelocitiesStd);

            normContainerMgr.HashMapSpatialNormsMeans.put(ConstsParamNames.StrokeSpatial.ACCELERATIONS, mNormContainerSpatialAccelerationsMean);
            normContainerMgr.HashMapSpatialNormsSds.put(ConstsParamNames.StrokeSpatial.ACCELERATIONS, mNormContainerSpatialAccelerationsStd);

            normContainerMgr.HashMapSpatialNormsMeans.put(ConstsParamNames.StrokeSpatial.RADIAL_VELOCITIES, mNormContainerSpatialRadialVelocitiesMean);
            normContainerMgr.HashMapSpatialNormsSds.put(ConstsParamNames.StrokeSpatial.RADIAL_VELOCITIES, mNormContainerSpatialRadialVelocitiesStd);

            normContainerMgr.HashMapSpatialNormsMeans.put(ConstsParamNames.StrokeSpatial.RADIAL_ACCELERATION, mNormContainerSpatialRadialAccelerationsMean);
            normContainerMgr.HashMapSpatialNormsSds.put(ConstsParamNames.StrokeSpatial.RADIAL_ACCELERATION, mNormContainerSpatialRadialAccelerationsStd);

            normContainerMgr.HashMapSpatialNormsMeans.put(ConstsParamNames.StrokeSpatial.RADIUS, mNormContainerSpatialRadiusMean);
            normContainerMgr.HashMapSpatialNormsSds.put(ConstsParamNames.StrokeSpatial.RADIUS, mNormContainerSpatialRadiusStd);

            normContainerMgr.HashMapSpatialNormsMeans.put(ConstsParamNames.StrokeSpatial.TETA, mNormContainerSpatialTetaMean);
            normContainerMgr.HashMapSpatialNormsSds.put(ConstsParamNames.StrokeSpatial.TETA, mNormContainerSpatialTetaStd);

            normContainerMgr.HashMapSpatialNormsMeans.put(ConstsParamNames.StrokeSpatial.DELTA_TETA, mNormContainerSpatialDeltaTetaMean);
            normContainerMgr.HashMapSpatialNormsSds.put(ConstsParamNames.StrokeSpatial.DELTA_TETA, mNormContainerSpatialDeltaTetaStd);

            normContainerMgr.HashMapSpatialNormsMeans.put(ConstsParamNames.StrokeSpatial.ACCUMULATED_NORM_AREA, mNormContainerSpatialAccumulatedNormalizedAreaMean);
            normContainerMgr.HashMapSpatialNormsSds.put(ConstsParamNames.StrokeSpatial.ACCUMULATED_NORM_AREA, mNormContainerSpatialAccumulatedNormalizedAreaStd);

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

            double popMeanGestureLength = normContainerMgr.GetNumericNormPopMean("ZLETTER", ConstsParamNames.Gesture.GESTURE_LENGTH);
            double popSdGestureLength = normContainerMgr.GetNumericNormPopSd("ZLETTER", ConstsParamNames.Gesture.GESTURE_LENGTH);
            double internalMeanGestureLength = normContainerMgr.GetNumericNormInternalSd("ZLETTER", ConstsParamNames.Gesture.GESTURE_LENGTH);

            double popMeanGestureNumEvents = normContainerMgr.GetNumericNormPopMean("ZLETTER", ConstsParamNames.Gesture.GESTURE_NUM_EVENTS);
            double popSdGestureNumEvents = normContainerMgr.GetNumericNormPopSd("ZLETTER", ConstsParamNames.Gesture.GESTURE_NUM_EVENTS);
            double internalMeanGestureNumEvents = normContainerMgr.GetNumericNormInternalSd("ZLETTER", ConstsParamNames.Gesture.GESTURE_NUM_EVENTS);

            double popMeanGestureTotalTimeInterval = normContainerMgr.GetNumericNormPopMean("ZLETTER", ConstsParamNames.Gesture.GESTURE_TOTAL_TIME_INTERVAL);
            double popSdGestureTotalTimeInterval = normContainerMgr.GetNumericNormPopSd("ZLETTER", ConstsParamNames.Gesture.GESTURE_TOTAL_TIME_INTERVAL);
            double internalMeanGestureTotalTimeInterval = normContainerMgr.GetNumericNormInternalSd("ZLETTER", ConstsParamNames.Gesture.GESTURE_TOTAL_TIME_INTERVAL);

            double popMeanGestureStrokesTotalTimeInterval = normContainerMgr.GetNumericNormPopMean("ZLETTER", ConstsParamNames.Gesture.GESTURE_TOTAL_STROKES_TIME_INTERVAL);
            double popSdGestureStrokesTotalTimeInterval = normContainerMgr.GetNumericNormPopSd("ZLETTER", ConstsParamNames.Gesture.GESTURE_TOTAL_STROKES_TIME_INTERVAL);
            double internalMeanGestureStrokesTotalTimeInterval = normContainerMgr.GetNumericNormInternalSd("ZLETTER", ConstsParamNames.Gesture.GESTURE_TOTAL_STROKES_TIME_INTERVAL);

            double popMeanGestureTotalArea = normContainerMgr.GetNumericNormPopMean("ZLETTER", ConstsParamNames.Gesture.GESTURE_TOTAL_AREA);
            double popSdGestureTotalArea = normContainerMgr.GetNumericNormPopSd("ZLETTER", ConstsParamNames.Gesture.GESTURE_TOTAL_AREA);
            double internalMeanTotalArea = normContainerMgr.GetNumericNormInternalSd("ZLETTER", ConstsParamNames.Gesture.GESTURE_TOTAL_AREA);

            double popMeanGestureTotalAreaMinXMinY = normContainerMgr.GetNumericNormPopMean("ZLETTER", ConstsParamNames.Gesture.GESTURE_TOTAL_AREA_MINX_MINY);
            double popSdGestureTotalAreaMinXMinY = normContainerMgr.GetNumericNormPopSd("ZLETTER", ConstsParamNames.Gesture.GESTURE_TOTAL_AREA_MINX_MINY);
            double internalMeanTotalAreaMinXMinY = normContainerMgr.GetNumericNormInternalSd("ZLETTER", ConstsParamNames.Gesture.GESTURE_TOTAL_AREA_MINX_MINY);

            double popMeanGestureTotalStrokeArea = normContainerMgr.GetNumericNormPopMean("ZLETTER", ConstsParamNames.Gesture.GESTURE_TOTAL_STROKE_AREA);
            double popSdGestureTotalAreaStroke = normContainerMgr.GetNumericNormPopSd("ZLETTER", ConstsParamNames.Gesture.GESTURE_TOTAL_STROKE_AREA);
            double internalMeanTotalAreaStroke = normContainerMgr.GetNumericNormInternalSd("ZLETTER", ConstsParamNames.Gesture.GESTURE_TOTAL_STROKE_AREA);

            double popMeanGestureTotalStrokeAreaMinXMinY = normContainerMgr.GetNumericNormPopMean("ZLETTER", ConstsParamNames.Gesture.GESTURE_TOTAL_STROKE_AREA_MINX_MINY);
            double popSdGestureTotalAreaStrokeMinXMinY = normContainerMgr.GetNumericNormPopSd("ZLETTER", ConstsParamNames.Gesture.GESTURE_TOTAL_STROKE_AREA_MINX_MINY);
            double internalMeanTotalAreaStrokeMinXMinY = normContainerMgr.GetNumericNormInternalSd("ZLETTER", ConstsParamNames.Gesture.GESTURE_TOTAL_STROKE_AREA_MINX_MINY);

            double popMeanGestureMiddlePressure = normContainerMgr.GetNumericNormPopMean("ZLETTER", ConstsParamNames.Gesture.GESTURE_MIDDLE_PRESSURE);
            double popSdGestureMiddlePressure = normContainerMgr.GetNumericNormPopSd("ZLETTER", ConstsParamNames.Gesture.GESTURE_MIDDLE_PRESSURE);
            double internalMeanMiddlePressure = normContainerMgr.GetNumericNormInternalSd("ZLETTER", ConstsParamNames.Gesture.GESTURE_MIDDLE_PRESSURE);

            double popMeanGestureMiddleSurface = normContainerMgr.GetNumericNormPopMean("ZLETTER", ConstsParamNames.Gesture.GESTURE_MIDDLE_SURFACE);
            double popSdGestureMiddleSurface = normContainerMgr.GetNumericNormPopSd("ZLETTER", ConstsParamNames.Gesture.GESTURE_MIDDLE_SURFACE);
            double internalMeanMiddleSurface = normContainerMgr.GetNumericNormInternalSd("ZLETTER", ConstsParamNames.Gesture.GESTURE_MIDDLE_SURFACE);

            double popMeanGestureAvgVelocity = normContainerMgr.GetNumericNormPopMean("ZLETTER", ConstsParamNames.Gesture.GESTURE_AVERAGE_VELOCITY);
            double popSdGestureAvgVelocity = normContainerMgr.GetNumericNormPopSd("ZLETTER", ConstsParamNames.Gesture.GESTURE_AVERAGE_VELOCITY);
            double internalMeanAvgVelocity = normContainerMgr.GetNumericNormInternalSd("ZLETTER", ConstsParamNames.Gesture.GESTURE_AVERAGE_VELOCITY);

            double popMeanGestureMaxVelocity = normContainerMgr.GetNumericNormPopMean("ZLETTER", ConstsParamNames.Gesture.GESTURE_MAX_VELOCITY);
            double popSdGestureMaxVelocity = normContainerMgr.GetNumericNormPopSd("ZLETTER", ConstsParamNames.Gesture.GESTURE_MAX_VELOCITY);
            double internalMeanMaxVelocity = normContainerMgr.GetNumericNormInternalSd("ZLETTER", ConstsParamNames.Gesture.GESTURE_MAX_VELOCITY);

            double popMeanGestureMidFirstStrokeVelocity = normContainerMgr.GetNumericNormPopMean("ZLETTER", ConstsParamNames.Gesture.GESTURE_MID_OF_FIRST_STROKE_VELOCITY);
            double popSdGestureMidFirstStrokeVelocity = normContainerMgr.GetNumericNormPopSd("ZLETTER", ConstsParamNames.Gesture.GESTURE_MID_OF_FIRST_STROKE_VELOCITY);
            double internalMeanMidFirstStrokeVelocity = normContainerMgr.GetNumericNormInternalSd("ZLETTER", ConstsParamNames.Gesture.GESTURE_MID_OF_FIRST_STROKE_VELOCITY);

            double popMeanGestureAvgAcceleration = normContainerMgr.GetNumericNormPopMean("ZLETTER", ConstsParamNames.Gesture.GESTURE_AVG_ACCELERATION);
            double popSdGestureAvgAcceleration = normContainerMgr.GetNumericNormPopSd("ZLETTER", ConstsParamNames.Gesture.GESTURE_AVG_ACCELERATION);
            double internalMeanAvgAcceleration = normContainerMgr.GetNumericNormInternalSd("ZLETTER", ConstsParamNames.Gesture.GESTURE_AVG_ACCELERATION);

            double popMeanGestureMaxAcceleration = normContainerMgr.GetNumericNormPopMean("ZLETTER", ConstsParamNames.Gesture.GESTURE_MAX_ACCELERATION);
            double popSdGestureMaxAcceleration = normContainerMgr.GetNumericNormPopSd("ZLETTER", ConstsParamNames.Gesture.GESTURE_MAX_ACCELERATION);
            double internalMeanMaxAcceleration = normContainerMgr.GetNumericNormInternalSd("ZLETTER", ConstsParamNames.Gesture.GESTURE_MAX_ACCELERATION);

            double popMeanGestureAvgStartAcceleration = normContainerMgr.GetNumericNormPopMean("ZLETTER", ConstsParamNames.Gesture.GESTURE_AVG_START_ACCELERATION);
            double popSdGestureAvgStartAcceleration = normContainerMgr.GetNumericNormPopSd("ZLETTER", ConstsParamNames.Gesture.GESTURE_AVG_START_ACCELERATION);
            double internalMeanAvgStartAcceleration = normContainerMgr.GetNumericNormInternalSd("ZLETTER", ConstsParamNames.Gesture.GESTURE_AVG_START_ACCELERATION);

            double popMeanGestureAccumulatedLengthSlope = normContainerMgr.GetNumericNormPopMean("ZLETTER", ConstsParamNames.Gesture.GESTURE_ACCUMULATED_LENGTH_SLOPE);
            double popSdGestureAccumulatedLengthSlope = normContainerMgr.GetNumericNormPopSd("ZLETTER", ConstsParamNames.Gesture.GESTURE_ACCUMULATED_LENGTH_SLOPE);
            double internalMeanGestureAccumulatedLengthSlope = normContainerMgr.GetNumericNormInternalSd("ZLETTER", ConstsParamNames.Gesture.GESTURE_ACCUMULATED_LENGTH_SLOPE);            

            JSONSerializer jsonSerializer = new JSONSerializer();
            string jsonNormContainerMgr = jsonSerializer.deepSerialize(normContainerMgr);

            ModelNormContainerMgr modelNormContainerMgr = new ModelNormContainerMgr(normContainerMgr);
            //NormContainerMgr normContainerMgr2 = modelNormContainerMgr.ToNormContainerMgr();
            //ModelNormContainerMgr modelNormContainerMgr2 = new ModelNormContainerMgr(normContainerMgr2);
            //NormContainerMgr normContainerMgr3 = modelNormContainerMgr2.ToNormContainerMgr();

            try
            {
                string jsonNormContainerMgrCsharp = JsonConvert.SerializeObject(modelNormContainerMgr);
                //NormContainerMgr normContainerMgr3 = tempNormContainerMgr.ToNormContainerMgr();
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

            mPathFaultyGestures = @"C:\TEMP\FaultyGestures.csv";
            mHashInvalid = new Dictionary<string, bool>();

            UtilsInstructions utilsInstructions = new UtilsInstructions();
            mListInstructions = utilsInstructions.GetInstructions();
        }

        private void ReadFaultyGestures()
        {
            StreamReader srFaultyGestures = new StreamReader(mPathFaultyGestures);

            string faultyId;
            while (!srFaultyGestures.EndOfStream)
            {
                faultyId = srFaultyGestures.ReadLine();
                mHashInvalid.Add(faultyId, true);
            }
            srFaultyGestures.Close();
        }
    }
}