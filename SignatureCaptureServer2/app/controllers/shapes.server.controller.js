'use strict';

var mongoose = require('mongoose'),
    querystring = require('querystring'),
    http = require('http'),
    _ = require('lodash'),
    fs = require('fs'),
    TemplateDemo = mongoose.model('TemplateDemo'),
    ShapeDemo = mongoose.model('ShapeDemo'),
    StrokeDemo = mongoose.model('StrokeDemo'),
    EventDemo = mongoose.model('EventDemo');

exports.createTemplateDemo = function(req, res) {
    var dataString =  JSON.stringify(req.body);
    fs.writeFile("/temp/signature.json", dataString, function(err) {
        if(err) {
            return console.log(err);
            return res.status(400).send({
                message: false
            });
        }
        else {
            return res.status(200).send({
                message: true
            });
        }

        console.log("The file was saved!");
    });
};