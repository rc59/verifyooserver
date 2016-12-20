'use strict';

    var http = require('http'),
    _ = require('lodash'),
    fs = require('fs');

exports.createTemplateDemo = function(req, res) {
    var dataString =  JSON.stringify(req.body);
    fs.writeFile("/temp/signature.json", dataString, function(err) {
        if(err) {
                res.status(400).send({ message: false });
        }
        else {
            res.status(200).send({ message: true });
        }

        console.log("The file was saved!");
    });
};