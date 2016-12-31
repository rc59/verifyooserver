'use strict';

    var http = require('http'),
    _ = require('lodash'),
    fs = require('fs');

exports.createTemplateDemoAuth = function(req, res) {
    var dataString =  JSON.stringify(req.body);
    if(dataString.length > 2) {
        fs.writeFile("/temp/signatures/signature.json", dataString, function(err) {
            if(err) {
                res.status(400).send({ message: false });
            }
            else {
                var readCounter = 0;
                var readFuncVar = setInterval(function() {
                    fs.readFile("/temp/signatures/result.json", 'utf8', function(err, data) {
                        if (err) {
                            readCounter++;
                            if(readCounter > 10) {
                                clearInterval(readFuncVar);
                                res.status(400).send({ message: false });
                            }
                        }
                        else {
                            clearInterval(readFuncVar);
                            var score = "@" + data + "@";
                            res.status(200).send({ message: score });

                            fs.unlink("/temp/signatures/result.json", 'utf8', function(err) {

                            });
                        }
                    });
                }, 500);
            }

            console.log("The file was saved!");
        });
    }
    else {
        res.status(200).send({ message: true });
    }
};

exports.createTemplateDemo = function(req, res) {
    var dataString =  JSON.stringify(req.body);
    fs.writeFile("/temp/signatures/signatureTest.json", dataString, function(err) {
        if(err) {
            res.status(400).send({ message: false });
        }
        else {
            res.status(200).send({ message: true });
        }

        console.log("The file was saved!");
    });
};