'use strict';


var mongoose = require('mongoose'),
    Schema = mongoose.Schema;

/************************************************/

var MatchResultSchema = new Schema({
    Match: {
        type: Boolean,
        default: false
    }
});

mongoose.model('MatchResult', MatchResultSchema);

/************************************************/

var EventSchema = new Schema({
    X: {
        type: Number,
        default: 0
    },
    Y: {
        type: Number,
        default: 0
    },
    Velocity: {
        type: Number,
        default: 0
    },
    VelocityX: {
        type: Number,
        default: 0
    },
    VelocityY: {
        type: Number,
        default: 0
    },
    Pressure: {
        type: Number,
        default: 0
    },
    EventTime: {
        type: Number,
        default: 0
    },
    TouchSurface: {
        type: Number,
        default: 0
    },
    AngleZ: {
        type: Number,
        default: 0
    },
    AngleX: {
        type: Number,
        default: 0
    },
    AngleY: {
        type: Number,
        default: 0
    }/*,
    Orientation: {
        type: Number,
        default: 0
    },
    Action: {
        type: Number,
        default: 0
    },
    ActionIdx: {
        type: Number,
        default: 0
    },
    ActionMasked: {
        type: Number,
        default: 0
    },
    EdgeFlags: {
        type: Number,
        default: 0
    },
    Flags: {
        type: Number,
        default: 0
    },
    HistorySize: {
        type: Number,
        default: 0
    },
    MetaState: {
        type: Number,
        default: 0
    },
    ToolMajor: {
        type: Number,
        default: 0
    },
    TouchMajor: {
        type: Number,
        default: 0
    },
    TouchMinor: {
        type: Number,
        default: 0
    },
    PointerCount: {
        type: Number,
        default: 0
    },
    AxisBreak: {
        type: Number,
        default: 0
    },
    AxisGas: {
        type: Number,
        default: 0
    },
    AxisX: {
        type: Number,
        default: 0
    },
    AxisY: {
        type: Number,
        default: 0
    },
    AxisZ: {
        type: Number,
        default: 0
    },
    AxisPressure: {
        type: Number,
        default: 0
    },
    AxisRudder: {
        type: Number,
        default: 0
    },
    AxisThrottle: {
        type: Number,
        default: 0
    },
    AxisDistance: {
        type: Number,
        default: 0
    },
    AxisTilt: {
        type: Number,
        default: 0
    },
    AxisOrientation: {
        type: Number,
        default: 0
    },
    AxisRtTrigger: {
        type: Number,
        default: 0
    },
    AxisLtTrigger: {
        type: Number,
        default: 0
    },
    AxisHatX: {
        type: Number,
        default: 0
    },
    AxisHatY: {
        type: Number,
        default: 0
    }*/
});

mongoose.model('Event', EventSchema);

/************************************************/

var StrokeSchema = new Schema({
    MatchScore: {
        type: Number,
    default: 0
    },
    Match: {
        type: Boolean,
    default: false
    },
    Length: {
        type: Number,
        default: 0
    },
    TimeInterval: {
        type: Number,
        default: 0
    },
    PauseBeforeStroke: {
        type: Number,
        default: 0
    },
    NumEvents: {
        type: Number,
        default: 0
    },
    DownTime: {
        type: Number,
        default: 0
    },
    UpTime: {
        type: Number,
        default: 0
    },
    PressureMax: {
        type: Number,
        default: 0
    },
    PressureMin: {
        type: Number,
        default: 0
    },
    PressureAvg: {
        type: Number,
        default: 0
    },
    TouchSurfaceMax: {
        type: Number,
        default: 0
    },
    TouchSurfaceMin: {
        type: Number,
        default: 0
    },
    TouchSurfaceAvg: {
        type: Number,
        default: 0
    },
    Width: {
        type: Number,
        default: 0
    },
    Height: {
        type: Number,
        default: 0
    },
    Area: {
        type: Number,
        default: 0
    },
    RelativePosX: {
        type: Number,
        default: 0
    },
    RelativePosY: {
        type: Number,
        default: 0
    },
    ListEvents: [ EventSchema ]
});

mongoose.model('Stroke', StrokeSchema);

/************************************************/

/**
 * Shape Schema
 */
var ShapeSchema = new Schema({
    Created: {
        type: Date,
        default: Date.now
    },
    Instruction: {
        type: String,
        default: ''
    },
    OS: {
        type: String,
        default: ''
    },
    DeviceId: {
        type: String,
        default: ''
    },
    ModelName: {
        type: String,
        default: ''
    },
    GcmToken: {
        type: String,
        default: ''
    },
    Name: {
        type: String,
        default: ''
    },
    ScreenWidth: {
        type: Number,
        default: 0
    },
    ScreenHeight: {
        type: Number,
        default: 0
    },
    TotalGames: {
        type: Number,
        default: 0
    },
    IsSource: {
        type: Boolean,
        default: false
    },
    Match: {
        type: Boolean,
        default: false
    },
    Strokes: [ StrokeSchema ]
});

mongoose.model('Shape', ShapeSchema);