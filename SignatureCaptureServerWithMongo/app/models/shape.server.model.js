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
    },
    IsHistory: {
        type: Boolean,
        default: false
    }
});

mongoose.model('Event', EventSchema);

/************************************************/

var StrokeSchema = new Schema({
    Length: {
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
    Strokes: [ StrokeSchema ]
});

mongoose.model('Shape', ShapeSchema);

var TemplateSchema = new Schema({
    Created: {
        type: Date,
        default: Date.now
    },
    Version: {
        type: String,
        default: ''
    },
    OS: {
        type: String,
        default: ''
    },
    Name: {
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
    ScreenWidth: {
        type: Number,
        default: 0
    },
    ScreenHeight: {
        type: Number,
        default: 0
    },
    Xdpi: {
        type: Number,
        default: 0
    },
    Ydpi: {
        type: Number,
        default: 0
    },
    ExpShapeList: [ ShapeSchema ]
});

mongoose.model('Template', TemplateSchema);