using com.google.gson;
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
        String mCurrId;
        Dictionary<String, bool> mHashInvalid = new Dictionary<string, bool>();
        MongoCollection<ModelTemplate> mListMongo;

        NormContainerMgr mNormContainerMgr = new NormContainerMgr();

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

        /************************************* NORMAL GESTURE PARAMETERS *************************************/

        NumericNormContainer mNormContainerGestureTotalTimeIntervalMean = new NumericNormContainer();
        NumericNormContainer mNormContainerGestureTotalTimeIntervalStd = new NumericNormContainer();

        NumericNormContainer mNormContainerGestureTotalAreaMean = new NumericNormContainer();
        NumericNormContainer mNormContainerGestureTotalAreaStd = new NumericNormContainer();

        NumericNormContainer mNormContainerGestureTotalAreaMinXMinYMean = new NumericNormContainer();
        NumericNormContainer mNormContainerGestureTotalAreaMinXMinYStd = new NumericNormContainer();

        NumericNormContainer mNormContainerGestureDelayTimeMean = new NumericNormContainer();
        NumericNormContainer mNormContainerGestureDelayTimeStd = new NumericNormContainer();

        NumericNormContainer mNormContainerGestureNumEventsMean = new NumericNormContainer();
        NumericNormContainer mNormContainerGestureNumEventsStd = new NumericNormContainer();

        NumericNormContainer mNormContainerGestureLengthMean = new NumericNormContainer();
        NumericNormContainer mNormContainerGestureLengthStd = new NumericNormContainer();

        NumericNormContainer mNormContainerStrokeTotalTimeIntervalMean = new NumericNormContainer();
        NumericNormContainer mNormContainerStrokeTotalTimeIntervalStd = new NumericNormContainer();

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

        /************************************* NORMAL STROKE PARAMETERS *************************************/

        NumericNormContainer mNormContainerStrokeInterestPointParamMean = new NumericNormContainer();
        NumericNormContainer mNormContainerStrokeInterestPointParamStd = new NumericNormContainer();

        NumericNormContainer mNormContainerStrokeTransitionTimeMean = new NumericNormContainer();
        NumericNormContainer mNormContainerStrokeTransitionTimeStd = new NumericNormContainer();

        NumericNormContainer mNormContainerStrokeLengthMean = new NumericNormContainer();
        NumericNormContainer mNormContainerStrokeLengthStd = new NumericNormContainer();

        NumericNormContainer mNormContainerStrokeNumEventsMean = new NumericNormContainer();
        NumericNormContainer mNormContainerStrokeNumEventsStd = new NumericNormContainer();

        NumericNormContainer mNormContainerStrokeTimeIntervalMean = new NumericNormContainer();
        NumericNormContainer mNormContainerStrokeTimeIntervalStd = new NumericNormContainer();

        NumericNormContainer mNormContainerStrokeTotalAreaMean = new NumericNormContainer();
        NumericNormContainer mNormContainerStrokeTotalAreaStd = new NumericNormContainer();

        NumericNormContainer mNormContainerStrokeAreaMinXMinYMean = new NumericNormContainer();
        NumericNormContainer mNormContainerStrokeAreaMinXMinYStd = new NumericNormContainer();

        NumericNormContainer mNormContainerStrokeVelocityAvgMean = new NumericNormContainer();
        NumericNormContainer mNormContainerStrokeVelocityAvgStd = new NumericNormContainer();

        NumericNormContainer mNormContainerStrokeVelocityMaxMean = new NumericNormContainer();
        NumericNormContainer mNormContainerStrokeVelocityMaxStd = new NumericNormContainer();

        NumericNormContainer mNormContainerStrokeVelocityMidMean = new NumericNormContainer();
        NumericNormContainer mNormContainerStrokeVelocityMidStd = new NumericNormContainer();

        NumericNormContainer mNormContainerStrokeAccelerationAvgMean = new NumericNormContainer();
        NumericNormContainer mNormContainerStrokeAccelerationAvgStd = new NumericNormContainer();

        NumericNormContainer mNormContainerStrokeAccelerationMaxMean = new NumericNormContainer();
        NumericNormContainer mNormContainerStrokeAccelerationMaxStd = new NumericNormContainer();

        NumericNormContainer mNormContainerStrokeMiddlePressureMean = new NumericNormContainer();
        NumericNormContainer mNormContainerStrokeMiddlePressureStd = new NumericNormContainer();

        NumericNormContainer mNormContainerStrokeMiddleSurfaceMean = new NumericNormContainer();
        NumericNormContainer mNormContainerStrokeMiddleSurfaceStd = new NumericNormContainer();

        NumericNormContainer mNormContainerStrokeMaxRadialVelocityMean = new NumericNormContainer();
        NumericNormContainer mNormContainerStrokeMaxRadialVelocityStd = new NumericNormContainer();

        NumericNormContainer mNormContainerStrokeMaxRadialAccelerationMean = new NumericNormContainer();
        NumericNormContainer mNormContainerStrokeMaxRadialAccelerationStd = new NumericNormContainer();

        NumericNormContainer mNormContainerStrokeMaxInterestPointIndexMean = new NumericNormContainer();
        NumericNormContainer mNormContainerStrokeMaxInterestPointIndexStd = new NumericNormContainer();

        NumericNormContainer mNormContainerStrokeMaxInterestPointDensityMean = new NumericNormContainer();
        NumericNormContainer mNormContainerStrokeMaxInterestPointDensityStd = new NumericNormContainer();

        NumericNormContainer mNormContainerStrokeMaxInterestPointLocationMean = new NumericNormContainer();
        NumericNormContainer mNormContainerStrokeMaxInterestPointLocationStd = new NumericNormContainer();

        NumericNormContainer mNormContainerStrokeMaxInterestPointPressureMean = new NumericNormContainer();
        NumericNormContainer mNormContainerStrokeMaxInterestPointPressureStd = new NumericNormContainer();

        NumericNormContainer mNormContainerStrokeMaxInterestPointSurfaceMean = new NumericNormContainer();
        NumericNormContainer mNormContainerStrokeMaxInterestPointSurfaceStd = new NumericNormContainer();

        NumericNormContainer mNormContainerStrokeMaxInterestPointVelocityMean = new NumericNormContainer();
        NumericNormContainer mNormContainerStrokeMaxInterestPointVelocityStd = new NumericNormContainer();

        NumericNormContainer mNormContainerStrokeMaxInterestPointAccelerationMean = new NumericNormContainer();
        NumericNormContainer mNormContainerStrokeMaxInterestPointAccelerationStd = new NumericNormContainer();

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

        private double SafeAddValue(double value)
        {
            if(Double.IsNaN(value))
            {
                return value = 0;
            }

            return value;
        }

        private void Start()
        {
            IEnumerable<ModelTemplate> modelTemplates;

            IMongoQuery query = Query<ModelTemplate>.EQ(c => c.DeviceId, "062bba750ae4e2b3");

            mListMongo = UtilsDB.GetCollShapes();
            double totalNumbRecords = mListMongo.Count(query);
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

            /*********************************** SPATIAL PARAMS ***********************************/

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

            /*********************************** GESTURE PARAMS ***********************************/

            NumericNormContainer tempNormContainerGestureTotalTimeInterval;
            NumericNormContainer tempNormContainerGestureTotalArea;
            NumericNormContainer tempNormContainerGestureTotalAreaMinXMinY;

            NumericNormContainer tempNormContainerGestureStrokesTotalArea;
            NumericNormContainer tempNormContainerGestureStrokesTotalAreaMinXMinY;

            NumericNormContainer tempNormContainerGestureLength;
            NumericNormContainer tempNormContainerGestureNumEvents;

            NumericNormContainer tempNormContainerGestureDelayTime;
            NumericNormContainer tempNormContainerStrokeTotalTimeInterval;

            NumericNormContainer tempNormContainerGestureMiddlePressure;
            NumericNormContainer tempNormContainerGestureMiddleSurface;

            NumericNormContainer tempNormContainerGestureAverageVelocity;
            NumericNormContainer tempNormContainerGestureMaxVelocity;
            NumericNormContainer tempNormContainerMidFirstStrokeVelocity;

            NumericNormContainer tempNormContainerGestureAvgAcceleration;
            NumericNormContainer tempNormContainerGestureMaxAcceleration;
            NumericNormContainer tempNormContainerGestureAvgStartAcceleration;

            NumericNormContainer tempNormContainerGestureAccumulatedLengthSlope;

            /*********************************** STROKE PARAMS ***********************************/

            NumericNormContainer tempNormContainerStrokeInterestPointParam;
            NumericNormContainer tempNormContainerStrokeTransitionTime;
            NumericNormContainer tempNormContainerStrokeLength;
            NumericNormContainer tempNormContainerStrokeNumEvents;
            NumericNormContainer tempNormContainerStrokeTimeInterval;

            NumericNormContainer tempNormContainerStrokeTotalArea;
            NumericNormContainer tempNormContainerStrokeTotalAreaMinXMinY;

            NumericNormContainer tempNormContainerStrokeVelocityAvg;
            NumericNormContainer tempNormContainerStrokeVelocityMax;
            NumericNormContainer tempNormContainerStrokeVelocityMid;

            NumericNormContainer tempNormContainerStrokeAccelerationAvg;
            NumericNormContainer tempNormContainerStrokeAccelerationMax;

            NumericNormContainer tempNormContainerStrokeMiddlePressure;
            NumericNormContainer tempNormContainerStrokeMiddleSurface;

            NumericNormContainer tempNormContainerStrokeMaxRadialVelocity;
            NumericNormContainer tempNormContainerStrokeMaxRadialAcceleration;

            NumericNormContainer tempNormContainerStrokeMaxInterestPointIndex;
            NumericNormContainer tempNormContainerStrokeMaxInterestPointDensity;
            NumericNormContainer tempNormContainerStrokeMaxInterestPointLocation;
            NumericNormContainer tempNormContainerStrokeMaxInterestPointPressure;
            NumericNormContainer tempNormContainerStrokeMaxInterestPointSurface;
            NumericNormContainer tempNormContainerStrokeMaxInterestPointVelocity;
            NumericNormContainer tempNormContainerStrokeMaxInterestPointAcceleration;

            try
            {
                mNormContainerMgr.InitBuckets();
            }
            catch(Exception exc)
            {
                string msgExc = exc.Message;
            }

            bool isInitialized = false;
            string tempInstruction = "BUCKET";
            int strokeKey;

            List<string> listTempInstructions;
            string gestureKey;            

            try
            {
                while (!isFinished)
                {
                    
                    modelTemplates = mListMongo.Find(query).SetLimit(limit).SetSkip(skip);                   

                    foreach (ModelTemplate template in modelTemplates)
                    {
                        mCurrId = template._id.ToString();
                        mCurrentTemplateNum++;                            

                        if((template.State.CompareTo("Authenticate") == 0 || template.State.CompareTo("Register") == 0) && template.ExpShapeList.Count > 0)
                        {
                            try
                            {
                                tempTemplate = UtilsTemplateConverter.ConvertTemplateNew(template);

                                tempAccumulator = new UtilsAccumulator();

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
                                tempNormContainerGestureTotalArea = new NumericNormContainer();
                                tempNormContainerGestureTotalAreaMinXMinY = new NumericNormContainer();

                                tempNormContainerGestureStrokesTotalArea = new NumericNormContainer();
                                tempNormContainerGestureStrokesTotalAreaMinXMinY = new NumericNormContainer();

                                tempNormContainerGestureLength = new NumericNormContainer();
                                tempNormContainerGestureNumEvents = new NumericNormContainer();

                                tempNormContainerGestureDelayTime = new NumericNormContainer();

                                tempNormContainerStrokeTotalTimeInterval = new NumericNormContainer();

                                tempNormContainerGestureMiddlePressure = new NumericNormContainer();
                                tempNormContainerGestureMiddleSurface = new NumericNormContainer();

                                tempNormContainerGestureAverageVelocity = new NumericNormContainer();
                                tempNormContainerGestureMaxVelocity = new NumericNormContainer();
                                tempNormContainerMidFirstStrokeVelocity = new NumericNormContainer();

                                tempNormContainerGestureAvgStartAcceleration = new NumericNormContainer();
                                tempNormContainerGestureAvgAcceleration = new NumericNormContainer();
                                tempNormContainerGestureMaxAcceleration = new NumericNormContainer();

                                tempNormContainerGestureAccumulatedLengthSlope = new NumericNormContainer();

                                tempNormContainerStrokeInterestPointParam = new NumericNormContainer();
                                tempNormContainerStrokeTransitionTime = new NumericNormContainer();
                                tempNormContainerStrokeLength = new NumericNormContainer();
                                tempNormContainerStrokeNumEvents = new NumericNormContainer();
                                tempNormContainerStrokeTimeInterval = new NumericNormContainer();

                                tempNormContainerStrokeTotalArea = new NumericNormContainer();
                                tempNormContainerStrokeTotalAreaMinXMinY = new NumericNormContainer();

                                tempNormContainerStrokeVelocityAvg = new NumericNormContainer();
                                tempNormContainerStrokeVelocityMax = new NumericNormContainer();
                                tempNormContainerStrokeVelocityMid = new NumericNormContainer();

                                tempNormContainerStrokeAccelerationAvg = new NumericNormContainer();
                                tempNormContainerStrokeAccelerationMax = new NumericNormContainer();

                                tempNormContainerStrokeMiddlePressure = new NumericNormContainer();
                                tempNormContainerStrokeMiddleSurface = new NumericNormContainer();

                                tempNormContainerStrokeMaxRadialVelocity = new NumericNormContainer();
                                tempNormContainerStrokeMaxRadialAcceleration = new NumericNormContainer();

                                tempNormContainerStrokeMaxInterestPointIndex = new NumericNormContainer();
                                tempNormContainerStrokeMaxInterestPointDensity = new NumericNormContainer();
                                tempNormContainerStrokeMaxInterestPointLocation = new NumericNormContainer();
                                tempNormContainerStrokeMaxInterestPointPressure = new NumericNormContainer();
                                tempNormContainerStrokeMaxInterestPointSurface = new NumericNormContainer();
                                tempNormContainerStrokeMaxInterestPointVelocity = new NumericNormContainer();
                                tempNormContainerStrokeMaxInterestPointAcceleration = new NumericNormContainer();

                                listTempInstructions = new List<string>();
                                for (int idxGesture = 0; idxGesture < tempTemplate.ListGestureExtended.size(); idxGesture++)
                                {
                                    tempGesture = (GestureExtended)tempTemplate.ListGestureExtended.get(idxGesture);
                                    gestureKey = GetGestureKey("RLETTER", tempGesture.ListStrokesExtended.size());
                                    //tempInstruction = tempGesture.Instruction;
                                    listTempInstructions.Add(gestureKey);

                                    for (int idxInstr = 0; idxInstr < mListInstructions.Count; idxInstr++)
                                    {
                                        if (mListInstructions[idxInstr].CompareTo(tempGesture.Instruction) == 0) {
                                            tempNormContainerGestureTotalTimeInterval.AddValue(SafeAddValue(tempGesture.GestureTotalTimeInterval), gestureKey);
                                            tempNormContainerGestureTotalArea.AddValue(SafeAddValue(tempGesture.GestureTotalArea), gestureKey);
                                            tempNormContainerGestureTotalAreaMinXMinY.AddValue(SafeAddValue(tempGesture.GestureTotalAreaMinXMinY), gestureKey);
                                        }

                                        //tempInstruction = "RLETTER";                                        

                                        //tempNormContainerGestureStrokesTotalArea.AddValue(SafeAddValue(tempGesture.GestureTotalStrokeArea), tempInstruction);
                                        //tempNormContainerGestureStrokesTotalAreaMinXMinY.AddValue(SafeAddValue(tempGesture.GestureTotalStrokeAreaMinXMinY), tempInstruction);

                                        //tempNormContainerGestureLength.AddValue(SafeAddValue(tempGesture.GestureLengthMM), tempInstruction);
                                        //tempNormContainerGestureNumEvents.AddValue(tempGesture.ListGestureEventsExtended.size(), tempInstruction);

                                        //if (idxGesture > 0)
                                        //{
                                        //    tempNormContainerGestureDelayTime.AddValue(SafeAddValue(tempGesture.GestureDelay), tempInstruction);
                                        //}
                                        //tempNormContainerStrokeTotalTimeInterval.AddValue(SafeAddValue(tempGesture.GestureTotalStrokeTimeInterval), tempInstruction);

                                        //tempNormContainerGestureMiddlePressure.AddValue(SafeAddValue(tempGesture.GestureAvgMiddlePressure), tempInstruction);
                                        //tempNormContainerGestureMiddleSurface.AddValue(SafeAddValue(tempGesture.GestureAvgMiddleSurface), tempInstruction);

                                        //tempNormContainerGestureAverageVelocity.AddValue(SafeAddValue(tempGesture.GestureAverageVelocity), tempInstruction);
                                        //tempNormContainerGestureMaxVelocity.AddValue(SafeAddValue(tempGesture.GestureMaxVelocity), tempInstruction);
                                        //tempNormContainerMidFirstStrokeVelocity.AddValue(SafeAddValue(tempGesture.MidOfFirstStrokeVelocity), tempInstruction);

                                        //tempNormContainerGestureAvgAcceleration.AddValue(SafeAddValue(tempGesture.GestureAverageAcceleration), tempInstruction);
                                        //tempNormContainerGestureMaxAcceleration.AddValue(SafeAddValue(tempGesture.GestureMaxAcceleration), tempInstruction);
                                        //tempNormContainerGestureAvgStartAcceleration.AddValue(SafeAddValue(tempGesture.GestureAverageStartAcceleration), tempInstruction);

                                        //tempNormContainerGestureAccumulatedLengthSlope.AddValue(SafeAddValue(tempGesture.GestureAccumulatedLengthLinearRegSlope), tempInstruction);

                                        for (int idxStroke = 0; idxStroke < tempGesture.ListStrokesExtended.size(); idxStroke++)
                                        {
                                            tempStroke = (StrokeExtended)tempGesture.ListStrokesExtended.get(idxStroke);

                                            strokeKey = mNormContainerMgr.GetStrokeIndex(tempStroke);

                                            if(tempStroke.StrokeTransitionTime > 0)
                                            {
                                                tempNormContainerStrokeTransitionTime.AddValue(SafeAddValue(tempStroke.StrokeTransitionTime), tempInstruction, strokeKey);
                                            }

                                            tempNormContainerStrokeInterestPointParam.AddValue(SafeAddValue(tempStroke.InterestPointDensityStrengthsParam), tempInstruction, strokeKey);
                                            tempNormContainerStrokeLength.AddValue(SafeAddValue(tempStroke.StrokePropertiesObj.LengthMM), tempInstruction, strokeKey);
                                            tempNormContainerStrokeNumEvents.AddValue(SafeAddValue(tempStroke.ListEventsExtended.size()), tempInstruction, strokeKey);
                                            tempNormContainerStrokeTimeInterval.AddValue(SafeAddValue(tempStroke.StrokeTimeInterval), tempInstruction, strokeKey);

                                            tempNormContainerStrokeTotalArea.AddValue(SafeAddValue(tempStroke.ShapeDataObj.ShapeArea), tempInstruction, strokeKey);
                                            tempNormContainerStrokeTotalAreaMinXMinY.AddValue(SafeAddValue(tempStroke.ShapeDataObj.ShapeAreaMinXMinY), tempInstruction, strokeKey);

                                            tempNormContainerStrokeVelocityAvg.AddValue(SafeAddValue(tempStroke.StrokeAverageVelocity), tempInstruction, strokeKey);
                                            tempNormContainerStrokeVelocityMax.AddValue(SafeAddValue(tempStroke.StrokeMaxVelocity), tempInstruction, strokeKey);
                                            tempNormContainerStrokeVelocityMid.AddValue(SafeAddValue(tempStroke.StrokeMidVelocity), tempInstruction, strokeKey);

                                            tempNormContainerStrokeAccelerationAvg.AddValue(SafeAddValue(tempStroke.StrokeAverageAcceleration), tempInstruction, strokeKey);
                                            tempNormContainerStrokeAccelerationMax.AddValue(SafeAddValue(tempStroke.StrokeMaxAcceleration), tempInstruction, strokeKey);

                                            tempNormContainerStrokeMiddlePressure.AddValue(SafeAddValue(tempStroke.MiddlePressure), tempInstruction, strokeKey);
                                            tempNormContainerStrokeMiddleSurface.AddValue(SafeAddValue(tempStroke.MiddleSurface), tempInstruction, strokeKey);

                                            tempNormContainerStrokeMaxRadialVelocity.AddValue(SafeAddValue(tempStroke.StrokeMaxRadialVelocity), tempInstruction, strokeKey);
                                            tempNormContainerStrokeMaxRadialAcceleration.AddValue(SafeAddValue(tempStroke.StrokeMaxRadialAcceleration), tempInstruction, strokeKey);

                                            tempNormContainerStrokeMaxInterestPointIndex.AddValue(SafeAddValue(tempStroke.MaxInterestPointIndex), tempInstruction, strokeKey);
                                            tempNormContainerStrokeMaxInterestPointDensity.AddValue(SafeAddValue(tempStroke.MaxInterestPointDensity), tempInstruction, strokeKey);
                                            tempNormContainerStrokeMaxInterestPointLocation.AddValue(SafeAddValue(tempStroke.MaxInterestPointLocation), tempInstruction, strokeKey);
                                            tempNormContainerStrokeMaxInterestPointPressure.AddValue(SafeAddValue(tempStroke.MaxInterestPointPressure), tempInstruction, strokeKey);
                                            tempNormContainerStrokeMaxInterestPointSurface.AddValue(SafeAddValue(tempStroke.MaxInterestPointSurface), tempInstruction, strokeKey);
                                            tempNormContainerStrokeMaxInterestPointVelocity.AddValue(SafeAddValue(tempStroke.MaxInterestPointVelocity), tempInstruction, strokeKey);
                                            tempNormContainerStrokeMaxInterestPointAcceleration.AddValue(SafeAddValue(tempStroke.MaxInterestPointAcceleration), tempInstruction, strokeKey);

                                            for (int idxEvent = 0; idxEvent < UtilsConsts.SAMPLE_SIZE; idxEvent++)
                                            {
                                                tempEventDistance = (MotionEventExtended)tempStroke.ListEventsSpatialExtended.get(idxEvent);
                                                tempEventTime = (MotionEventExtended)tempStroke.ListEventsTemporalExtended.get(idxEvent);

                                                isInitialized = true;

                                                /********************************************* DISTANCE *********************************************/

                                                tempNormContainerVelocitiesDistance.AddValue(SafeAddValue(tempEventDistance.Velocity), tempInstruction, strokeKey, idxEvent);
                                                tempNormContainerRadialVelocitiesDistance.AddValue(SafeAddValue(tempEventDistance.RadialVelocity), tempInstruction, strokeKey, idxEvent);
                                                tempNormContainerRadialAccelerationDistance.AddValue(SafeAddValue(tempEventDistance.RadialAcceleration), tempInstruction, strokeKey, idxEvent);
                                                tempNormContainerRadiusDistance.AddValue(SafeAddValue(tempEventDistance.Radius), tempInstruction, strokeKey, idxEvent);
                                                tempNormContainerTetaDistance.AddValue(SafeAddValue(tempEventDistance.Teta), tempInstruction, strokeKey, idxEvent);
                                                tempNormContainerDeltaTetaDistance.AddValue(SafeAddValue(tempEventDistance.DeltaTeta), tempInstruction, strokeKey, idxEvent);
                                                tempNormContainerAccumulatedNormalizedAreaDistance.AddValue(SafeAddValue(tempEventDistance.AccumulatedNormalizedArea), tempInstruction, strokeKey, idxEvent);
                                                tempNormContainerAccelerationDistance.AddValue(SafeAddValue(tempEventDistance.Acceleration), tempInstruction, strokeKey, idxEvent);

                                                /********************************************* TIME *********************************************/

                                                tempNormContainerVelocitiesTime.AddValue(SafeAddValue(tempEventTime.Velocity), tempInstruction, strokeKey, idxEvent);
                                                tempNormContainerRadialVelocitiesTime.AddValue(SafeAddValue(tempEventTime.RadialVelocity), tempInstruction, strokeKey, idxEvent);
                                                tempNormContainerRadialAccelerationTime.AddValue(SafeAddValue(tempEventTime.RadialAcceleration), tempInstruction, strokeKey, idxEvent);
                                                tempNormContainerRadiusTime.AddValue(SafeAddValue(tempEventTime.Radius), tempInstruction, strokeKey, idxEvent);
                                                tempNormContainerTetaTime.AddValue(SafeAddValue(tempEventTime.Teta), tempInstruction, strokeKey, idxEvent);
                                                tempNormContainerDeltaTetaTime.AddValue(SafeAddValue(tempEventTime.DeltaTeta), tempInstruction, strokeKey, idxEvent);
                                                tempNormContainerAccumulatedNormalizedAreaTime.AddValue(SafeAddValue(tempEventTime.AccumulatedNormalizedArea), tempInstruction, strokeKey, idxEvent);
                                                tempNormContainerAccelerationTime.AddValue(SafeAddValue(tempEventTime.Acceleration), tempInstruction, strokeKey, idxEvent);
                                            }

                                            mStrokesCount++;

                                            msg = string.Format("Finished {0}/{1} Records", mCurrentTemplateNum, totalNumbRecords);
                                            this.lblStatus.Invoke(new MethodInvoker(() => this.lblStatus.Text = msg));
                                        }
                                    }
                                    
                                }

                                if (isInitialized)
                                {
                                    isInitialized = false;

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

                                    AddValueToStrokesNormContainer(tempInstruction, tempNormContainerStrokeInterestPointParam, mNormContainerStrokeInterestPointParamMean, mNormContainerStrokeInterestPointParamStd);

                                    AddValueToStrokesNormContainer(tempInstruction, tempNormContainerStrokeTransitionTime, mNormContainerStrokeTransitionTimeMean, mNormContainerStrokeTransitionTimeStd);
                                    AddValueToStrokesNormContainer(tempInstruction, tempNormContainerStrokeLength, mNormContainerStrokeLengthMean, mNormContainerStrokeLengthStd);
                                    AddValueToStrokesNormContainer(tempInstruction, tempNormContainerStrokeNumEvents, mNormContainerStrokeNumEventsMean, mNormContainerStrokeNumEventsStd);
                                    AddValueToStrokesNormContainer(tempInstruction, tempNormContainerStrokeTimeInterval, mNormContainerStrokeTimeIntervalMean, mNormContainerStrokeTimeIntervalStd);

                                    AddValueToStrokesNormContainer(tempInstruction, tempNormContainerStrokeTotalArea, mNormContainerStrokeTotalAreaMean, mNormContainerStrokeTotalAreaStd);
                                    AddValueToStrokesNormContainer(tempInstruction, tempNormContainerStrokeTotalAreaMinXMinY, mNormContainerStrokeAreaMinXMinYMean, mNormContainerStrokeAreaMinXMinYStd);

                                    AddValueToStrokesNormContainer(tempInstruction, tempNormContainerStrokeVelocityAvg, mNormContainerStrokeVelocityAvgMean, mNormContainerStrokeVelocityAvgStd);
                                    AddValueToStrokesNormContainer(tempInstruction, tempNormContainerStrokeVelocityMax, mNormContainerStrokeVelocityMaxMean, mNormContainerStrokeVelocityMaxStd);
                                    AddValueToStrokesNormContainer(tempInstruction, tempNormContainerStrokeVelocityMid, mNormContainerStrokeVelocityMidMean, mNormContainerStrokeVelocityMidStd);

                                    AddValueToStrokesNormContainer(tempInstruction, tempNormContainerStrokeAccelerationAvg, mNormContainerStrokeAccelerationAvgMean, mNormContainerStrokeAccelerationAvgStd);
                                    AddValueToStrokesNormContainer(tempInstruction, tempNormContainerStrokeAccelerationMax, mNormContainerStrokeAccelerationMaxMean, mNormContainerStrokeAccelerationMaxStd);

                                    AddValueToStrokesNormContainer(tempInstruction, tempNormContainerStrokeMiddlePressure, mNormContainerStrokeMiddlePressureMean, mNormContainerStrokeMiddlePressureStd);
                                    AddValueToStrokesNormContainer(tempInstruction, tempNormContainerStrokeMiddleSurface, mNormContainerStrokeMiddleSurfaceMean, mNormContainerStrokeMiddleSurfaceStd);

                                    AddValueToStrokesNormContainer(tempInstruction, tempNormContainerStrokeMaxRadialVelocity, mNormContainerStrokeMaxRadialVelocityMean, mNormContainerStrokeMaxRadialVelocityStd);
                                    AddValueToStrokesNormContainer(tempInstruction, tempNormContainerStrokeMaxRadialAcceleration, mNormContainerStrokeMaxRadialAccelerationMean, mNormContainerStrokeMaxRadialAccelerationStd);

                                    AddValueToStrokesNormContainer(tempInstruction, tempNormContainerStrokeMaxInterestPointIndex, mNormContainerStrokeMaxInterestPointIndexMean, mNormContainerStrokeMaxInterestPointIndexStd);
                                    AddValueToStrokesNormContainer(tempInstruction, tempNormContainerStrokeMaxInterestPointDensity, mNormContainerStrokeMaxInterestPointDensityMean, mNormContainerStrokeMaxInterestPointDensityStd);
                                    AddValueToStrokesNormContainer(tempInstruction, tempNormContainerStrokeMaxInterestPointLocation, mNormContainerStrokeMaxInterestPointLocationMean, mNormContainerStrokeMaxInterestPointLocationStd);
                                    AddValueToStrokesNormContainer(tempInstruction, tempNormContainerStrokeMaxInterestPointPressure, mNormContainerStrokeMaxInterestPointPressureMean, mNormContainerStrokeMaxInterestPointPressureStd);
                                    AddValueToStrokesNormContainer(tempInstruction, tempNormContainerStrokeMaxInterestPointSurface, mNormContainerStrokeMaxInterestPointSurfaceMean, mNormContainerStrokeMaxInterestPointSurfaceStd);
                                    AddValueToStrokesNormContainer(tempInstruction, tempNormContainerStrokeMaxInterestPointVelocity, mNormContainerStrokeMaxInterestPointVelocityMean, mNormContainerStrokeMaxInterestPointVelocityStd);
                                    AddValueToStrokesNormContainer(tempInstruction, tempNormContainerStrokeMaxInterestPointAcceleration, mNormContainerStrokeMaxInterestPointAccelerationMean, mNormContainerStrokeMaxInterestPointAccelerationStd);

                                    for (int idxInstruction = 0; idxInstruction < listTempInstructions.Count; idxInstruction++)
                                    {
                                        //tempInstruction = mListInstructions[idxInstruction];
                                        //tempInstruction = "RLETTER";
                                        
                                        AddValueToNormContainer(listTempInstructions[idxInstruction], tempNormContainerGestureTotalTimeInterval, mNormContainerGestureTotalTimeIntervalMean, mNormContainerGestureTotalTimeIntervalStd);
                                        AddValueToNormContainer(listTempInstructions[idxInstruction], tempNormContainerGestureTotalArea, mNormContainerGestureTotalAreaMean, mNormContainerGestureTotalAreaStd);
                                        AddValueToNormContainer(listTempInstructions[idxInstruction], tempNormContainerGestureTotalAreaMinXMinY, mNormContainerGestureTotalAreaMinXMinYMean, mNormContainerGestureTotalAreaMinXMinYStd);

                                        //AddValueToNormContainer(tempInstruction, tempNormContainerStrokeTotalTimeInterval, mNormContainerStrokeTotalTimeIntervalMean, mNormContainerStrokeTotalTimeIntervalStd);

                                        //AddValueToNormContainer(tempInstruction, tempNormContainerGestureLength, mNormContainerGestureLengthMean, mNormContainerGestureLengthStd);
                                        //AddValueToNormContainer(tempInstruction, tempNormContainerGestureNumEvents, mNormContainerGestureNumEventsMean, mNormContainerGestureNumEventsStd);

                                        //AddValueToNormContainer(tempInstruction, tempNormContainerGestureDelayTime, mNormContainerGestureDelayTimeMean, mNormContainerGestureDelayTimeStd);

                                        //AddValueToNormContainer(tempInstruction, tempNormContainerGestureStrokesTotalArea, mNormContainerGestureStrokesTotalAreaMean, mNormContainerGestureStrokesTotalAreaStd);
                                        //AddValueToNormContainer(tempInstruction, tempNormContainerGestureStrokesTotalAreaMinXMinY, mNormContainerGestureStrokesTotalAreaMinXMinYMean, mNormContainerGestureStrokesTotalAreaMinXMinYStd);

                                        //AddValueToNormContainer(tempInstruction, tempNormContainerGestureStrokesTotalArea, mNormContainerGestureStrokesTotalAreaMean, mNormContainerGestureStrokesTotalAreaStd);
                                        //AddValueToNormContainer(tempInstruction, tempNormContainerGestureStrokesTotalAreaMinXMinY, mNormContainerGestureStrokesTotalAreaMinXMinYMean, mNormContainerGestureStrokesTotalAreaMinXMinYStd);

                                        //AddValueToNormContainer(tempInstruction, tempNormContainerGestureMiddlePressure, mNormContainerGestureMiddlePressureMean, mNormContainerGestureMiddlePressureStd);
                                        //AddValueToNormContainer(tempInstruction, tempNormContainerGestureMiddleSurface, mNormContainerGestureMiddleSurfaceMean, mNormContainerGestureMiddleSurfaceStd);

                                        //AddValueToNormContainer(tempInstruction, tempNormContainerGestureAverageVelocity, mNormContainerGestureAvgVelocityMean, mNormContainerGestureAvgVelocityStd);
                                        //AddValueToNormContainer(tempInstruction, tempNormContainerGestureMaxVelocity, mNormContainerGestureMaxVelocityMean, mNormContainerGestureMaxVelocityStd);
                                        //AddValueToNormContainer(tempInstruction, tempNormContainerMidFirstStrokeVelocity, mNormContainerGestureMidFirstStrokeVelocityMean, mNormContainerGestureMidFirstStrokeVelocityStd);

                                        //AddValueToNormContainer(tempInstruction, tempNormContainerGestureAvgAcceleration, mNormContainerGestureAvgAccelerationMean, mNormContainerGestureAvgAccelerationStd);
                                        //AddValueToNormContainer(tempInstruction, tempNormContainerGestureMaxAcceleration, mNormContainerGestureMaxAccelerationMean, mNormContainerGestureMaxAccelerationStd);
                                        //AddValueToNormContainer(tempInstruction, tempNormContainerGestureAvgStartAcceleration, mNormContainerGestureAvgStartAccelerationMean, mNormContainerGestureAvgStartAccelerationStd);

                                        //AddValueToNormContainer(tempInstruction, tempNormContainerGestureAccumulatedLengthSlope, mNormContainerGestureAccumulatedLengthSlopeMean, mNormContainerGestureAccumulatedLengthSlopeStd);                                        
                                    }
                                }
                            }
                            catch(Exception exc)
                            {
                                string msgExc = exc.Message;
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

            if (Double.IsNaN(tempStd))
            {
                tempStd = tempMean / 4;
            }

            normContainerOutputMean.AddValue(tempMean, instruction);
            normContainerOutputSd.AddValue(tempStd, instruction);
        }

        private void AddValueToStrokesNormContainer(string instruction, NumericNormContainer normContainerInput, NumericNormContainer normContainerOutputMean, NumericNormContainer normContainerOutputSd)
        {
            double tempMean, tempStd;
            int idxCurrentStroke = 0;

            object[] keySet = normContainerInput.HashNorms.keySet().toArray();
            string tempKey;

            for (int idx = 0; idx < keySet.Length; idx++) {
                tempKey = (string) keySet[idx];

                idxCurrentStroke = GetStrokeIdx(tempKey);
                tempMean = normContainerInput.GetMean(instruction, idxCurrentStroke);
                tempStd = normContainerInput.GetStd(instruction, idxCurrentStroke);

                if (Double.IsNaN(tempStd))
                {
                    tempStd = tempMean / 4;
                }

                normContainerOutputMean.AddValue(SafeAddValue(tempMean), instruction, idxCurrentStroke);
                normContainerOutputSd.AddValue(SafeAddValue(tempStd), instruction, idxCurrentStroke);
            }


            //while (isContinue)
            //{
            //    if (normContainerInput.HashNorms.containsKey(string.Format("{0}-{1}", instruction, idxCurrentStroke.ToString())))
            //    {
            //        tempMean = normContainerInput.GetMean(instruction, idxCurrentStroke);
            //        tempStd = normContainerInput.GetStd(instruction, idxCurrentStroke);

            //        if (Double.IsNaN(tempStd))
            //        {
            //            tempStd = tempMean / 4;
            //        }
                    
            //        normContainerOutputMean.AddValue(SafeAddValue(tempMean), instruction, idxCurrentStroke);
            //        normContainerOutputSd.AddValue(SafeAddValue(tempStd), instruction, idxCurrentStroke);

            //        idxCurrentStroke++;
            //    }
            //    else
            //    {
            //        isContinue = false;
            //    }
            //}

            
        }

        private int GetStrokeIdx(string tempKey)
        {
            string[] listStrs = tempKey.Split('-');
            int idxTemp = 0;
            int.TryParse(listStrs[1], out idxTemp);

            return idxTemp;
        }

        private void AddValueToNormContainer(string instruction, int idxSpatial, SpatialNormContainer normContainerInput, SpatialNormContainer normContainerOutputMean, SpatialNormContainer normContainerOutputSd)
        {
            double tempMean, tempStd;          
            int idxCurrentStroke = 0;

            object[] keySet = normContainerInput.HashNorms.keySet().toArray();
            string tempKey;

            for (int idx = 0; idx < keySet.Length; idx++)
            {
                tempKey = (string)keySet[idx];
                idxCurrentStroke = GetStrokeIdx(tempKey);

                tempMean = normContainerInput.GetMean(instruction, idxCurrentStroke, idxSpatial);
                tempStd = normContainerInput.GetStd(instruction, idxCurrentStroke, idxSpatial);

                normContainerOutputMean.AddValue(SafeAddValue(tempMean), instruction, idxCurrentStroke, idxSpatial);
                normContainerOutputSd.AddValue(SafeAddValue(tempStd), instruction, idxCurrentStroke, idxSpatial);

                idxCurrentStroke++;
            }
        }

        private void CalculateNorms()
        {            
            /******************************************** SPATIAL PARAMETERS - DISTANCE ********************************************/

            mNormContainerMgr.HashMapSpatialNormsMeansDistance.put(ConstsParamNames.StrokeSampling.VELOCITIES, mNormContainerSpatialVelocitiesMeanDistance);
            mNormContainerMgr.HashMapSpatialNormsSdsDistance.put(ConstsParamNames.StrokeSampling.VELOCITIES, mNormContainerSpatialVelocitiesStdDistance);

            mNormContainerMgr.HashMapSpatialNormsMeansDistance.put(ConstsParamNames.StrokeSampling.ACCELERATIONS, mNormContainerSpatialAccelerationsMeanDistance);
            mNormContainerMgr.HashMapSpatialNormsSdsDistance.put(ConstsParamNames.StrokeSampling.ACCELERATIONS, mNormContainerSpatialAccelerationsStdDistance);

            mNormContainerMgr.HashMapSpatialNormsMeansDistance.put(ConstsParamNames.StrokeSampling.RADIAL_VELOCITIES, mNormContainerSpatialRadialVelocitiesMeanDistance);
            mNormContainerMgr.HashMapSpatialNormsSdsDistance.put(ConstsParamNames.StrokeSampling.RADIAL_VELOCITIES, mNormContainerSpatialRadialVelocitiesStdDistance);

            mNormContainerMgr.HashMapSpatialNormsMeansDistance.put(ConstsParamNames.StrokeSampling.RADIAL_ACCELERATION, mNormContainerSpatialRadialAccelerationsMeanDistance);
            mNormContainerMgr.HashMapSpatialNormsSdsDistance.put(ConstsParamNames.StrokeSampling.RADIAL_ACCELERATION, mNormContainerSpatialRadialAccelerationsStdDistance);

            mNormContainerMgr.HashMapSpatialNormsMeansDistance.put(ConstsParamNames.StrokeSampling.RADIUS, mNormContainerSpatialRadiusMeanDistance);
            mNormContainerMgr.HashMapSpatialNormsSdsDistance.put(ConstsParamNames.StrokeSampling.RADIUS, mNormContainerSpatialRadiusStdDistance);

            mNormContainerMgr.HashMapSpatialNormsMeansDistance.put(ConstsParamNames.StrokeSampling.TETA, mNormContainerSpatialTetaMeanDistance);
            mNormContainerMgr.HashMapSpatialNormsSdsDistance.put(ConstsParamNames.StrokeSampling.TETA, mNormContainerSpatialTetaStdDistance);

            mNormContainerMgr.HashMapSpatialNormsMeansDistance.put(ConstsParamNames.StrokeSampling.DELTA_TETA, mNormContainerSpatialDeltaTetaMeanDistance);
            mNormContainerMgr.HashMapSpatialNormsSdsDistance.put(ConstsParamNames.StrokeSampling.DELTA_TETA, mNormContainerSpatialDeltaTetaStdDistance);

            mNormContainerMgr.HashMapSpatialNormsMeansDistance.put(ConstsParamNames.StrokeSampling.ACCUMULATED_NORM_AREA, mNormContainerSpatialAccumulatedNormalizedAreaMeanDistance);
            mNormContainerMgr.HashMapSpatialNormsSdsDistance.put(ConstsParamNames.StrokeSampling.ACCUMULATED_NORM_AREA, mNormContainerSpatialAccumulatedNormalizedAreaStdDistance);

            /******************************************** SPATIAL PARAMETERS - TIME ********************************************/

            mNormContainerMgr.HashMapSpatialNormsMeansTime.put(ConstsParamNames.StrokeSampling.VELOCITIES, mNormContainerSpatialVelocitiesMeanTime);
            mNormContainerMgr.HashMapSpatialNormsSdsTime.put(ConstsParamNames.StrokeSampling.VELOCITIES, mNormContainerSpatialVelocitiesStdTime);

            mNormContainerMgr.HashMapSpatialNormsMeansTime.put(ConstsParamNames.StrokeSampling.ACCELERATIONS, mNormContainerSpatialAccelerationsMeanTime);
            mNormContainerMgr.HashMapSpatialNormsSdsTime.put(ConstsParamNames.StrokeSampling.ACCELERATIONS, mNormContainerSpatialAccelerationsStdTime);

            mNormContainerMgr.HashMapSpatialNormsMeansTime.put(ConstsParamNames.StrokeSampling.RADIAL_VELOCITIES, mNormContainerSpatialRadialVelocitiesMeanTime);
            mNormContainerMgr.HashMapSpatialNormsSdsTime.put(ConstsParamNames.StrokeSampling.RADIAL_VELOCITIES, mNormContainerSpatialRadialVelocitiesStdTime);

            mNormContainerMgr.HashMapSpatialNormsMeansTime.put(ConstsParamNames.StrokeSampling.RADIAL_ACCELERATION, mNormContainerSpatialRadialAccelerationsMeanTime);
            mNormContainerMgr.HashMapSpatialNormsSdsTime.put(ConstsParamNames.StrokeSampling.RADIAL_ACCELERATION, mNormContainerSpatialRadialAccelerationsStdTime);

            mNormContainerMgr.HashMapSpatialNormsMeansTime.put(ConstsParamNames.StrokeSampling.RADIUS, mNormContainerSpatialRadiusMeanTime);
            mNormContainerMgr.HashMapSpatialNormsSdsTime.put(ConstsParamNames.StrokeSampling.RADIUS, mNormContainerSpatialRadiusStdTime);

            mNormContainerMgr.HashMapSpatialNormsMeansTime.put(ConstsParamNames.StrokeSampling.TETA, mNormContainerSpatialTetaMeanTime);
            mNormContainerMgr.HashMapSpatialNormsSdsTime.put(ConstsParamNames.StrokeSampling.TETA, mNormContainerSpatialTetaStdTime);

            mNormContainerMgr.HashMapSpatialNormsMeansTime.put(ConstsParamNames.StrokeSampling.DELTA_TETA, mNormContainerSpatialDeltaTetaMeanTime);
            mNormContainerMgr.HashMapSpatialNormsSdsTime.put(ConstsParamNames.StrokeSampling.DELTA_TETA, mNormContainerSpatialDeltaTetaStdTime);

            mNormContainerMgr.HashMapSpatialNormsMeansTime.put(ConstsParamNames.StrokeSampling.ACCUMULATED_NORM_AREA, mNormContainerSpatialAccumulatedNormalizedAreaMeanTime);
            mNormContainerMgr.HashMapSpatialNormsSdsTime.put(ConstsParamNames.StrokeSampling.ACCUMULATED_NORM_AREA, mNormContainerSpatialAccumulatedNormalizedAreaStdTime);

            /******************************************** NORMAL GESTURE PARAMETERS ********************************************/

            mNormContainerMgr.HashMapNumericNormsMeans.put(ConstsParamNames.Gesture.GESTURE_TOTAL_TIME_INTERVAL, mNormContainerGestureTotalTimeIntervalMean);
            mNormContainerMgr.HashMapNumericNormsSds.put(ConstsParamNames.Gesture.GESTURE_TOTAL_TIME_INTERVAL, mNormContainerGestureTotalTimeIntervalStd);

            mNormContainerMgr.HashMapNumericNormsMeans.put(ConstsParamNames.Gesture.GESTURE_TOTAL_AREA, mNormContainerGestureTotalAreaMean);
            mNormContainerMgr.HashMapNumericNormsSds.put(ConstsParamNames.Gesture.GESTURE_TOTAL_AREA, mNormContainerGestureTotalAreaStd);

            mNormContainerMgr.HashMapNumericNormsMeans.put(ConstsParamNames.Gesture.GESTURE_TOTAL_AREA_MINX_MINY, mNormContainerGestureTotalAreaMinXMinYMean);
            mNormContainerMgr.HashMapNumericNormsSds.put(ConstsParamNames.Gesture.GESTURE_TOTAL_AREA_MINX_MINY, mNormContainerGestureTotalAreaMinXMinYStd);

            //mNormContainerMgr.HashMapNumericNormsMeans.put(ConstsParamNames.Gesture.GESTURE_DELAY_TIME, mNormContainerGestureDelayTimeMean);
            //mNormContainerMgr.HashMapNumericNormsSds.put(ConstsParamNames.Gesture.GESTURE_DELAY_TIME, mNormContainerGestureDelayTimeStd);

            //mNormContainerMgr.HashMapNumericNormsMeans.put(ConstsParamNames.Gesture.GESTURE_LENGTH, mNormContainerGestureLengthMean);
            //mNormContainerMgr.HashMapNumericNormsSds.put(ConstsParamNames.Gesture.GESTURE_LENGTH, mNormContainerGestureLengthStd);

            //mNormContainerMgr.HashMapNumericNormsMeans.put(ConstsParamNames.Gesture.GESTURE_NUM_EVENTS, mNormContainerGestureNumEventsMean);
            //mNormContainerMgr.HashMapNumericNormsSds.put(ConstsParamNames.Gesture.GESTURE_NUM_EVENTS, mNormContainerGestureNumEventsStd);

            //mNormContainerMgr.HashMapNumericNormsMeans.put(ConstsParamNames.Gesture.GESTURE_TOTAL_STROKES_TIME_INTERVAL, mNormContainerStrokeTotalTimeIntervalMean);
            //mNormContainerMgr.HashMapNumericNormsSds.put(ConstsParamNames.Gesture.GESTURE_TOTAL_STROKES_TIME_INTERVAL, mNormContainerStrokeTotalTimeIntervalStd);

            //mNormContainerMgr.HashMapNumericNormsMeans.put(ConstsParamNames.Gesture.GESTURE_TOTAL_STROKE_AREA, mNormContainerGestureStrokesTotalAreaMean);
            //mNormContainerMgr.HashMapNumericNormsSds.put(ConstsParamNames.Gesture.GESTURE_TOTAL_STROKE_AREA, mNormContainerGestureStrokesTotalAreaStd);

            //mNormContainerMgr.HashMapNumericNormsMeans.put(ConstsParamNames.Gesture.GESTURE_TOTAL_STROKE_AREA_MINX_MINY, mNormContainerGestureStrokesTotalAreaMinXMinYMean);
            //mNormContainerMgr.HashMapNumericNormsSds.put(ConstsParamNames.Gesture.GESTURE_TOTAL_STROKE_AREA_MINX_MINY, mNormContainerGestureStrokesTotalAreaMinXMinYStd);

            //mNormContainerMgr.HashMapNumericNormsMeans.put(ConstsParamNames.Gesture.GESTURE_MIDDLE_PRESSURE, mNormContainerGestureMiddlePressureMean);
            //mNormContainerMgr.HashMapNumericNormsSds.put(ConstsParamNames.Gesture.GESTURE_MIDDLE_PRESSURE, mNormContainerGestureMiddlePressureStd);

            //mNormContainerMgr.HashMapNumericNormsMeans.put(ConstsParamNames.Gesture.GESTURE_MIDDLE_SURFACE, mNormContainerGestureMiddleSurfaceMean);
            //mNormContainerMgr.HashMapNumericNormsSds.put(ConstsParamNames.Gesture.GESTURE_MIDDLE_SURFACE, mNormContainerGestureMiddleSurfaceStd);

            //mNormContainerMgr.HashMapNumericNormsMeans.put(ConstsParamNames.Gesture.GESTURE_AVERAGE_VELOCITY, mNormContainerGestureAvgVelocityMean);
            //mNormContainerMgr.HashMapNumericNormsSds.put(ConstsParamNames.Gesture.GESTURE_AVERAGE_VELOCITY, mNormContainerGestureAvgVelocityStd);

            //mNormContainerMgr.HashMapNumericNormsMeans.put(ConstsParamNames.Gesture.GESTURE_MAX_VELOCITY, mNormContainerGestureMaxVelocityMean);
            //mNormContainerMgr.HashMapNumericNormsSds.put(ConstsParamNames.Gesture.GESTURE_MAX_VELOCITY, mNormContainerGestureMaxVelocityStd);

            //mNormContainerMgr.HashMapNumericNormsMeans.put(ConstsParamNames.Gesture.GESTURE_MID_OF_FIRST_STROKE_VELOCITY, mNormContainerGestureMidFirstStrokeVelocityMean);
            //mNormContainerMgr.HashMapNumericNormsSds.put(ConstsParamNames.Gesture.GESTURE_MID_OF_FIRST_STROKE_VELOCITY, mNormContainerGestureMidFirstStrokeVelocityStd);

            //mNormContainerMgr.HashMapNumericNormsMeans.put(ConstsParamNames.Gesture.GESTURE_AVG_ACCELERATION, mNormContainerGestureAvgAccelerationMean);
            //mNormContainerMgr.HashMapNumericNormsSds.put(ConstsParamNames.Gesture.GESTURE_AVG_ACCELERATION, mNormContainerGestureAvgAccelerationStd);

            //mNormContainerMgr.HashMapNumericNormsMeans.put(ConstsParamNames.Gesture.GESTURE_MAX_ACCELERATION, mNormContainerGestureMaxAccelerationMean);
            //mNormContainerMgr.HashMapNumericNormsSds.put(ConstsParamNames.Gesture.GESTURE_MAX_ACCELERATION, mNormContainerGestureMaxAccelerationStd);

            //mNormContainerMgr.HashMapNumericNormsMeans.put(ConstsParamNames.Gesture.GESTURE_AVG_START_ACCELERATION, mNormContainerGestureAvgStartAccelerationMean);
            //mNormContainerMgr.HashMapNumericNormsSds.put(ConstsParamNames.Gesture.GESTURE_AVG_START_ACCELERATION, mNormContainerGestureAvgStartAccelerationStd);

            //mNormContainerMgr.HashMapNumericNormsMeans.put(ConstsParamNames.Gesture.GESTURE_ACCUMULATED_LENGTH_SLOPE, mNormContainerGestureAccumulatedLengthSlopeMean);
            //mNormContainerMgr.HashMapNumericNormsSds.put(ConstsParamNames.Gesture.GESTURE_ACCUMULATED_LENGTH_SLOPE, mNormContainerGestureAccumulatedLengthSlopeStd);

            /******************************************** NORMAL STROKE PARAMETERS ********************************************/

            mNormContainerMgr.HashMapNumericNormsMeans.put(ConstsParamNames.Stroke.STROKE_INTEREST_POINT_PARAM, mNormContainerStrokeInterestPointParamMean);
            mNormContainerMgr.HashMapNumericNormsSds.put(ConstsParamNames.Stroke.STROKE_INTEREST_POINT_PARAM, mNormContainerStrokeInterestPointParamStd);

            mNormContainerMgr.HashMapNumericNormsMeans.put(ConstsParamNames.Stroke.STROKE_TRANSITION_TIME, mNormContainerStrokeTransitionTimeMean);
            mNormContainerMgr.HashMapNumericNormsSds.put(ConstsParamNames.Stroke.STROKE_TRANSITION_TIME, mNormContainerStrokeTransitionTimeStd);

            mNormContainerMgr.HashMapNumericNormsMeans.put(ConstsParamNames.Stroke.STROKE_LENGTH, mNormContainerStrokeLengthMean);
            mNormContainerMgr.HashMapNumericNormsSds.put(ConstsParamNames.Stroke.STROKE_LENGTH, mNormContainerStrokeLengthStd);

            mNormContainerMgr.HashMapNumericNormsMeans.put(ConstsParamNames.Stroke.STROKE_NUM_EVENTS, mNormContainerStrokeNumEventsMean);
            mNormContainerMgr.HashMapNumericNormsSds.put(ConstsParamNames.Stroke.STROKE_NUM_EVENTS, mNormContainerStrokeNumEventsStd);

            mNormContainerMgr.HashMapNumericNormsMeans.put(ConstsParamNames.Stroke.STROKE_TIME_INTERVAL, mNormContainerStrokeTimeIntervalMean);
            mNormContainerMgr.HashMapNumericNormsSds.put(ConstsParamNames.Stroke.STROKE_TIME_INTERVAL, mNormContainerStrokeTimeIntervalStd);

            mNormContainerMgr.HashMapNumericNormsMeans.put(ConstsParamNames.Stroke.STROKE_TOTAL_AREA, mNormContainerStrokeTotalAreaMean);
            mNormContainerMgr.HashMapNumericNormsSds.put(ConstsParamNames.Stroke.STROKE_TOTAL_AREA, mNormContainerStrokeTotalAreaStd);

            mNormContainerMgr.HashMapNumericNormsMeans.put(ConstsParamNames.Stroke.STROKE_TOTAL_AREA_MINX_MINY, mNormContainerStrokeAreaMinXMinYMean);
            mNormContainerMgr.HashMapNumericNormsSds.put(ConstsParamNames.Stroke.STROKE_TOTAL_AREA_MINX_MINY, mNormContainerStrokeAreaMinXMinYStd);

            mNormContainerMgr.HashMapNumericNormsMeans.put(ConstsParamNames.Stroke.STROKE_AVERAGE_VELOCITY, mNormContainerStrokeVelocityAvgMean);
            mNormContainerMgr.HashMapNumericNormsSds.put(ConstsParamNames.Stroke.STROKE_AVERAGE_VELOCITY, mNormContainerStrokeVelocityAvgStd);

            mNormContainerMgr.HashMapNumericNormsMeans.put(ConstsParamNames.Stroke.STROKE_MAX_VELOCITY, mNormContainerStrokeVelocityMaxMean);
            mNormContainerMgr.HashMapNumericNormsSds.put(ConstsParamNames.Stroke.STROKE_MAX_VELOCITY, mNormContainerStrokeVelocityMaxStd);

            mNormContainerMgr.HashMapNumericNormsMeans.put(ConstsParamNames.Stroke.STROKE_MID_VELOCITY, mNormContainerStrokeVelocityMidMean);
            mNormContainerMgr.HashMapNumericNormsSds.put(ConstsParamNames.Stroke.STROKE_MID_VELOCITY, mNormContainerStrokeVelocityMidStd);

            mNormContainerMgr.HashMapNumericNormsMeans.put(ConstsParamNames.Stroke.STROKE_AVERAGE_ACCELERATION, mNormContainerStrokeAccelerationAvgMean);
            mNormContainerMgr.HashMapNumericNormsSds.put(ConstsParamNames.Stroke.STROKE_AVERAGE_ACCELERATION, mNormContainerStrokeAccelerationAvgStd);

            mNormContainerMgr.HashMapNumericNormsMeans.put(ConstsParamNames.Stroke.STROKE_MAX_ACCELERATION, mNormContainerStrokeAccelerationMaxMean);
            mNormContainerMgr.HashMapNumericNormsSds.put(ConstsParamNames.Stroke.STROKE_MAX_ACCELERATION, mNormContainerStrokeAccelerationMaxStd);

            mNormContainerMgr.HashMapNumericNormsMeans.put(ConstsParamNames.Stroke.STROKE_MIDDLE_PRESSURE, mNormContainerStrokeMiddlePressureMean);
            mNormContainerMgr.HashMapNumericNormsSds.put(ConstsParamNames.Stroke.STROKE_MIDDLE_PRESSURE, mNormContainerStrokeMiddlePressureStd);

            mNormContainerMgr.HashMapNumericNormsMeans.put(ConstsParamNames.Stroke.STROKE_MIDDLE_SURFACE, mNormContainerStrokeMiddleSurfaceMean);
            mNormContainerMgr.HashMapNumericNormsSds.put(ConstsParamNames.Stroke.STROKE_MIDDLE_SURFACE, mNormContainerStrokeMiddleSurfaceStd);

            mNormContainerMgr.HashMapNumericNormsMeans.put(ConstsParamNames.Stroke.STROKE_MAX_RADIAL_VELOCITY, mNormContainerStrokeMaxRadialVelocityMean);
            mNormContainerMgr.HashMapNumericNormsSds.put(ConstsParamNames.Stroke.STROKE_MAX_RADIAL_VELOCITY, mNormContainerStrokeMaxRadialVelocityStd);

            mNormContainerMgr.HashMapNumericNormsMeans.put(ConstsParamNames.Stroke.STROKE_MAX_RADIAL_ACCELERATION, mNormContainerStrokeMaxRadialAccelerationMean);
            mNormContainerMgr.HashMapNumericNormsSds.put(ConstsParamNames.Stroke.STROKE_MAX_RADIAL_ACCELERATION, mNormContainerStrokeMaxRadialAccelerationStd);

            mNormContainerMgr.HashMapNumericNormsMeans.put(ConstsParamNames.Stroke.InterestPoints.STROKE_MAX_INTEREST_POINT_INDEX, mNormContainerStrokeMaxInterestPointIndexMean);
            mNormContainerMgr.HashMapNumericNormsSds.put(ConstsParamNames.Stroke.InterestPoints.STROKE_MAX_INTEREST_POINT_INDEX, mNormContainerStrokeMaxInterestPointIndexStd);

            mNormContainerMgr.HashMapNumericNormsMeans.put(ConstsParamNames.Stroke.InterestPoints.STROKE_MAX_INTEREST_POINT_DENSITY, mNormContainerStrokeMaxInterestPointDensityMean);
            mNormContainerMgr.HashMapNumericNormsSds.put(ConstsParamNames.Stroke.InterestPoints.STROKE_MAX_INTEREST_POINT_DENSITY, mNormContainerStrokeMaxInterestPointDensityStd);

            mNormContainerMgr.HashMapNumericNormsMeans.put(ConstsParamNames.Stroke.InterestPoints.STROKE_MAX_INTEREST_POINT_LOCATION, mNormContainerStrokeMaxInterestPointLocationMean);
            mNormContainerMgr.HashMapNumericNormsSds.put(ConstsParamNames.Stroke.InterestPoints.STROKE_MAX_INTEREST_POINT_LOCATION, mNormContainerStrokeMaxInterestPointLocationStd);

            mNormContainerMgr.HashMapNumericNormsMeans.put(ConstsParamNames.Stroke.InterestPoints.STROKE_MAX_INTEREST_POINT_PRESSURE, mNormContainerStrokeMaxInterestPointPressureMean);
            mNormContainerMgr.HashMapNumericNormsSds.put(ConstsParamNames.Stroke.InterestPoints.STROKE_MAX_INTEREST_POINT_PRESSURE, mNormContainerStrokeMaxInterestPointPressureStd);

            mNormContainerMgr.HashMapNumericNormsMeans.put(ConstsParamNames.Stroke.InterestPoints.STROKE_MAX_INTEREST_POINT_SURFACE, mNormContainerStrokeMaxInterestPointSurfaceMean);
            mNormContainerMgr.HashMapNumericNormsSds.put(ConstsParamNames.Stroke.InterestPoints.STROKE_MAX_INTEREST_POINT_SURFACE, mNormContainerStrokeMaxInterestPointSurfaceStd);

            mNormContainerMgr.HashMapNumericNormsMeans.put(ConstsParamNames.Stroke.InterestPoints.STROKE_MAX_INTEREST_POINT_VELOCITY, mNormContainerStrokeMaxInterestPointVelocityMean);
            mNormContainerMgr.HashMapNumericNormsSds.put(ConstsParamNames.Stroke.InterestPoints.STROKE_MAX_INTEREST_POINT_VELOCITY, mNormContainerStrokeMaxInterestPointVelocityStd);

            mNormContainerMgr.HashMapNumericNormsMeans.put(ConstsParamNames.Stroke.InterestPoints.STROKE_MAX_INTEREST_POINT_ACCELERATION, mNormContainerStrokeMaxInterestPointAccelerationMean);
            mNormContainerMgr.HashMapNumericNormsSds.put(ConstsParamNames.Stroke.InterestPoints.STROKE_MAX_INTEREST_POINT_ACCELERATION, mNormContainerStrokeMaxInterestPointAccelerationStd);

            /********************************************************************************************************************************/

            //double popMeanGestureLength = normContainerMgr.GetNumericNormPopMean("SLETTER", ConstsParamNames.Gesture.GESTURE_LENGTH);
            //double popSdGestureLength = normContainerMgr.GetNumericNormPopSd("SLETTER", ConstsParamNames.Gesture.GESTURE_LENGTH);
            //double internalMeanGestureLength = normContainerMgr.GetNumericNormInternalSd("SLETTER", ConstsParamNames.Gesture.GESTURE_LENGTH);

            //double popMeanGestureNumEvents = normContainerMgr.GetNumericNormPopMean("SLETTER", ConstsParamNames.Gesture.GESTURE_NUM_EVENTS);
            //double popSdGestureNumEvents = normContainerMgr.GetNumericNormPopSd("SLETTER", ConstsParamNames.Gesture.GESTURE_NUM_EVENTS);
            //double internalMeanGestureNumEvents = normContainerMgr.GetNumericNormInternalSd("SLETTER", ConstsParamNames.Gesture.GESTURE_NUM_EVENTS);

            //double popMeanGestureTotalTimeInterval = normContainerMgr.GetNumericNormPopMean("SLETTER", ConstsParamNames.Gesture.GESTURE_TOTAL_TIME_INTERVAL);
            //double popSdGestureTotalTimeInterval = normContainerMgr.GetNumericNormPopSd("SLETTER", ConstsParamNames.Gesture.GESTURE_TOTAL_TIME_INTERVAL);
            //double internalMeanGestureTotalTimeInterval = normContainerMgr.GetNumericNormInternalSd("SLETTER", ConstsParamNames.Gesture.GESTURE_TOTAL_TIME_INTERVAL);

            //double popMeanGestureStrokesTotalTimeInterval = normContainerMgr.GetNumericNormPopMean("SLETTER", ConstsParamNames.Gesture.GESTURE_TOTAL_STROKES_TIME_INTERVAL);
            //double popSdGestureStrokesTotalTimeInterval = normContainerMgr.GetNumericNormPopSd("SLETTER", ConstsParamNames.Gesture.GESTURE_TOTAL_STROKES_TIME_INTERVAL);
            //double internalMeanGestureStrokesTotalTimeInterval = normContainerMgr.GetNumericNormInternalSd("SLETTER", ConstsParamNames.Gesture.GESTURE_TOTAL_STROKES_TIME_INTERVAL);

            //double popMeanGestureTotalArea = normContainerMgr.GetNumericNormPopMean("SLETTER", ConstsParamNames.Gesture.GESTURE_TOTAL_AREA);
            //double popSdGestureTotalArea = normContainerMgr.GetNumericNormPopSd("SLETTER", ConstsParamNames.Gesture.GESTURE_TOTAL_AREA);
            //double internalMeanTotalArea = normContainerMgr.GetNumericNormInternalSd("SLETTER", ConstsParamNames.Gesture.GESTURE_TOTAL_AREA);

            //double popMeanGestureTotalAreaMinXMinY = normContainerMgr.GetNumericNormPopMean("SLETTER", ConstsParamNames.Gesture.GESTURE_TOTAL_AREA_MINX_MINY);
            //double popSdGestureTotalAreaMinXMinY = normContainerMgr.GetNumericNormPopSd("SLETTER", ConstsParamNames.Gesture.GESTURE_TOTAL_AREA_MINX_MINY);
            //double internalMeanTotalAreaMinXMinY = normContainerMgr.GetNumericNormInternalSd("SLETTER", ConstsParamNames.Gesture.GESTURE_TOTAL_AREA_MINX_MINY);

            //double popMeanGestureTotalStrokeArea = normContainerMgr.GetNumericNormPopMean("SLETTER", ConstsParamNames.Gesture.GESTURE_TOTAL_STROKE_AREA);
            //double popSdGestureTotalAreaStroke = normContainerMgr.GetNumericNormPopSd("SLETTER", ConstsParamNames.Gesture.GESTURE_TOTAL_STROKE_AREA);
            //double internalMeanTotalAreaStroke = normContainerMgr.GetNumericNormInternalSd("SLETTER", ConstsParamNames.Gesture.GESTURE_TOTAL_STROKE_AREA);

            //double popMeanGestureTotalStrokeAreaMinXMinY = normContainerMgr.GetNumericNormPopMean("SLETTER", ConstsParamNames.Gesture.GESTURE_TOTAL_STROKE_AREA_MINX_MINY);
            //double popSdGestureTotalAreaStrokeMinXMinY = normContainerMgr.GetNumericNormPopSd("SLETTER", ConstsParamNames.Gesture.GESTURE_TOTAL_STROKE_AREA_MINX_MINY);
            //double internalMeanTotalAreaStrokeMinXMinY = normContainerMgr.GetNumericNormInternalSd("SLETTER", ConstsParamNames.Gesture.GESTURE_TOTAL_STROKE_AREA_MINX_MINY);

            //double popMeanGestureMiddlePressure = normContainerMgr.GetNumericNormPopMean("SLETTER", ConstsParamNames.Gesture.GESTURE_MIDDLE_PRESSURE);
            //double popSdGestureMiddlePressure = normContainerMgr.GetNumericNormPopSd("SLETTER", ConstsParamNames.Gesture.GESTURE_MIDDLE_PRESSURE);
            //double internalMeanMiddlePressure = normContainerMgr.GetNumericNormInternalSd("SLETTER", ConstsParamNames.Gesture.GESTURE_MIDDLE_PRESSURE);

            //double popMeanGestureMiddleSurface = normContainerMgr.GetNumericNormPopMean("SLETTER", ConstsParamNames.Gesture.GESTURE_MIDDLE_SURFACE);
            //double popSdGestureMiddleSurface = normContainerMgr.GetNumericNormPopSd("SLETTER", ConstsParamNames.Gesture.GESTURE_MIDDLE_SURFACE);
            //double internalMeanMiddleSurface = normContainerMgr.GetNumericNormInternalSd("SLETTER", ConstsParamNames.Gesture.GESTURE_MIDDLE_SURFACE);

            //double popMeanGestureAvgVelocity = normContainerMgr.GetNumericNormPopMean("SLETTER", ConstsParamNames.Gesture.GESTURE_AVERAGE_VELOCITY);
            //double popSdGestureAvgVelocity = normContainerMgr.GetNumericNormPopSd("SLETTER", ConstsParamNames.Gesture.GESTURE_AVERAGE_VELOCITY);
            //double internalMeanAvgVelocity = normContainerMgr.GetNumericNormInternalSd("SLETTER", ConstsParamNames.Gesture.GESTURE_AVERAGE_VELOCITY);

            //double popMeanGestureMaxVelocity = normContainerMgr.GetNumericNormPopMean("SLETTER", ConstsParamNames.Gesture.GESTURE_MAX_VELOCITY);
            //double popSdGestureMaxVelocity = normContainerMgr.GetNumericNormPopSd("SLETTER", ConstsParamNames.Gesture.GESTURE_MAX_VELOCITY);
            //double internalMeanMaxVelocity = normContainerMgr.GetNumericNormInternalSd("SLETTER", ConstsParamNames.Gesture.GESTURE_MAX_VELOCITY);

            //double popMeanGestureMidFirstStrokeVelocity = normContainerMgr.GetNumericNormPopMean("SLETTER", ConstsParamNames.Gesture.GESTURE_MID_OF_FIRST_STROKE_VELOCITY);
            //double popSdGestureMidFirstStrokeVelocity = normContainerMgr.GetNumericNormPopSd("SLETTER", ConstsParamNames.Gesture.GESTURE_MID_OF_FIRST_STROKE_VELOCITY);
            //double internalMeanMidFirstStrokeVelocity = normContainerMgr.GetNumericNormInternalSd("SLETTER", ConstsParamNames.Gesture.GESTURE_MID_OF_FIRST_STROKE_VELOCITY);

            //double popMeanGestureAvgAcceleration = normContainerMgr.GetNumericNormPopMean("SLETTER", ConstsParamNames.Gesture.GESTURE_AVG_ACCELERATION);
            //double popSdGestureAvgAcceleration = normContainerMgr.GetNumericNormPopSd("SLETTER", ConstsParamNames.Gesture.GESTURE_AVG_ACCELERATION);
            //double internalMeanAvgAcceleration = normContainerMgr.GetNumericNormInternalSd("SLETTER", ConstsParamNames.Gesture.GESTURE_AVG_ACCELERATION);

            //double popMeanGestureMaxAcceleration = normContainerMgr.GetNumericNormPopMean("SLETTER", ConstsParamNames.Gesture.GESTURE_MAX_ACCELERATION);
            //double popSdGestureMaxAcceleration = normContainerMgr.GetNumericNormPopSd("SLETTER", ConstsParamNames.Gesture.GESTURE_MAX_ACCELERATION);
            //double internalMeanMaxAcceleration = normContainerMgr.GetNumericNormInternalSd("SLETTER", ConstsParamNames.Gesture.GESTURE_MAX_ACCELERATION);

            //double popMeanGestureAvgStartAcceleration = normContainerMgr.GetNumericNormPopMean("SLETTER", ConstsParamNames.Gesture.GESTURE_AVG_START_ACCELERATION);
            //double popSdGestureAvgStartAcceleration = normContainerMgr.GetNumericNormPopSd("SLETTER", ConstsParamNames.Gesture.GESTURE_AVG_START_ACCELERATION);
            //double internalMeanAvgStartAcceleration = normContainerMgr.GetNumericNormInternalSd("SLETTER", ConstsParamNames.Gesture.GESTURE_AVG_START_ACCELERATION);

            //double popMeanGestureAccumulatedLengthSlope = normContainerMgr.GetNumericNormPopMean("SLETTER", ConstsParamNames.Gesture.GESTURE_ACCUMULATED_LENGTH_SLOPE);
            //double popSdGestureAccumulatedLengthSlope = normContainerMgr.GetNumericNormPopSd("SLETTER", ConstsParamNames.Gesture.GESTURE_ACCUMULATED_LENGTH_SLOPE);
            //double internalMeanGestureAccumulatedLengthSlope = normContainerMgr.GetNumericNormInternalSd("SLETTER", ConstsParamNames.Gesture.GESTURE_ACCUMULATED_LENGTH_SLOPE);            

            StreamWriter streamWriterNormsJavaGson = InitStreamWriterResultNormsJavaGson();
            StreamWriter streamWriterNormsJava = InitStreamWriterResultNormsJava();
            StreamWriter streamWriterNormsCS = InitStreamWriterNormsCS();
            
            GsonBuilder gsonBuilder = new GsonBuilder();
            Gson gson = gsonBuilder.enableComplexMapKeySerialization().create();
            
            string gsonStr = gson.toJson(mNormContainerMgr);
            streamWriterNormsJavaGson.WriteLine(gsonStr);
            streamWriterNormsJavaGson.Flush();
            streamWriterNormsJavaGson.Close();

            JSONSerializer jsonSerializer = new JSONSerializer();
            string jsonNormContainerMgr = jsonSerializer.deepSerialize(mNormContainerMgr);
            streamWriterNormsJava.WriteLine(jsonNormContainerMgr);
            streamWriterNormsJava.Flush();
            streamWriterNormsJava.Close();

            ModelNormContainerMgr modelNormContainerMgr = new ModelNormContainerMgr(mNormContainerMgr);
            //NormContainerMgr normContainerMgr2 = modelNormContainerMgr.ToNormContainerMgr();
            //ModelNormContainerMgr modelNormContainerMgr2 = new ModelNormContainerMgr(normContainerMgr2);
            //NormContainerMgr normContainerMgr3 = modelNormContainerMgr2.ToNormContainerMgr();

            try
            {
                string jsonNormContainerMgrCsharp = JsonConvert.SerializeObject(modelNormContainerMgr);
                streamWriterNormsCS.WriteLine(jsonNormContainerMgrCsharp);
                streamWriterNormsCS.Flush();
                streamWriterNormsCS.Close();

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

        private string GetGestureKey(string instruction, int numStrokes) {
            return string.Format("{0}-{1}", instruction, numStrokes.ToString());
        }

        private StreamWriter InitStreamWriterResultNormsJavaGson()
        {
            StreamWriter streamWriter = File.CreateText(@"C:\Temp\normsjavagson.txt");
            return streamWriter;
        }

        private StreamWriter InitStreamWriterResultNormsJava()
        {
            StreamWriter streamWriter = File.CreateText(@"C:\Temp\normsjava.txt");
            return streamWriter;
        }

        private StreamWriter InitStreamWriterNormsCS()
        {
            StreamWriter streamWriter = File.CreateText(@"C:\Temp\normscs.txt");
            return streamWriter;
        }
    }
}
