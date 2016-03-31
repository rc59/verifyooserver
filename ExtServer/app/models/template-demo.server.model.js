'use strict';


var mongoose = require('mongoose'),
    Schema = mongoose.Schema;

/************************************************/

var EventDemoSchema = new Schema({
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
    }
});

mongoose.model('EventDemo', EventDemoSchema);

/************************************************/

var StrokeDemoSchema = new Schema({
    Length: {
        type: Number,
        default: 0
    },
    ListEvents: [ EventDemoSchema ]
});

mongoose.model('StrokeDemo', StrokeDemoSchema);

/************************************************/

/**
 * Shape Schema
 */
var ShapeDemoSchema = new Schema({
    Created: {
        type: Date,
        default: Date.now
    },
    Instruction: {
        type: String,
        default: ''
    },
    Strokes: [ StrokeDemoSchema ]
});

mongoose.model('ShapeDemo', ShapeDemoSchema);

var TemplateDemoSchema = new Schema({
    Created: {
        type: Date,
        default: Date.now
    },
    Company: {
        type: String,
        default: ''
    },
    State: {
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
    ExpShapeList: [ ShapeDemoSchema ]
});

mongoose.model('TemplateDemo', TemplateDemoSchema);