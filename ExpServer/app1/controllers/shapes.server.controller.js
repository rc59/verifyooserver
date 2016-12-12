'use strict';

var mongoose = require('mongoose'),
    querystring = require('querystring'),
    http = require('http'),
    matchLogic = require('./match-logic'),
    _ = require('lodash'),
    Shape = mongoose.model('Shape'),
    Stroke = mongoose.model('Stroke'),
    Event = mongoose.model('Event'),
    MatchResult = mongoose.model('MatchResult');

var MIN_SHAPE_COMPARISON = 3;

var idxCurr = 0;
var mInstructions = [
"צייר צורה",
    "צייר סיפרה",
    "צייר אות"
]

exports.isUserExistsByName = function(req, res) {
    var username = req.query.username;

    if(username != null && username.length > 0) {
        Shape.find({ Name: username }).sort( { Created: 1 } ).exec(function(err, shapes) {
            if (err) {
                return res.send(400, {
                    message: "Unexpected error"
                });
            } else {
                if(shapes.length >= 1) {

                    var shape = shapes[0];

                    var start = new Date();
                    start.setHours(0,0,0,0);

                    var end = new Date();
                    end.setHours(23,59,59,999);

                    var numGames = 0;
                    var temp;
                    for(var idx = 0; idx < shapes.length; idx++) {
                        temp = shapes[idx];
                        if(temp.IsSource == false && temp.Created > start && temp.Created < end) {
                            numGames++;
                        }
                    }

                    res.status(200).send({ message: true, games: numGames, name: shape.Name, instruction: temp.Instruction });
                }
                else {
                    var txtInstruction;

                    if(idxCurr >= mInstructions.length) {
                        txtInstruction = mInstructions[0];
                        idxCurr = 0;
                    }
                    else {
                        txtInstruction = mInstructions[idxCurr];
                    }

                    res.status(200).send({ message: false, games: 0, name: "", instruction: txtInstruction });
                }
            }
        });
    }
    else {
        res.status(400).send({ message: false });
    }
};

exports.isUserExists = function(req, res) {
    var deviceId = req.query.deviceId;

    if(deviceId != null && deviceId.length > 0) {
        Shape.find({ DeviceId: deviceId }).sort( { Created: 1 } ).exec(function(err, shapes) {
            if (err) {
                return res.send(400, {
                    message: "Unexpected error"
                });
            } else {
                if(shapes.length >= 1) {

                    var shape = shapes[0];

                    var start = new Date();
                    start.setHours(0,0,0,0);

                    var end = new Date();
                    end.setHours(23,59,59,999);

                    var numGames = 0;
                    var temp;
                    for(var idx = 0; idx < shapes.length; idx++) {
                        temp = shapes[idx];
                        if(temp.IsSource == false && temp.Created > start && temp.Created < end) {
                            numGames++;
                        }
                    }

                    res.status(200).send({ message: true, games: numGames, name: shape.Name, instruction: temp.Instruction });
                }
                else {
                    var txtInstruction;

                    if(idxCurr >= mInstructions.length) {
                        txtInstruction = mInstructions[0];
                        idxCurr = 0;
                    }
                    else {
                        txtInstruction = mInstructions[idxCurr];
                    }

                    res.status(200).send({ message: false, games: 0, name: "", instruction: txtInstruction });
                }
            }
        });
    }
    else {
        res.status(400).send({ message: false });
    }
};

exports.create = function(req, res) {
    var shape = new Shape(req.body);
    shape = _.extend(shape, req.body);

    var shapeVerify = new Shape(req.body);
    shapeVerify = _.extend(shapeVerify, req.body);

    var numStrokes = shape.Strokes.length;
    var numStrokesToRemove = numStrokes / 2;

    shape.Strokes.splice(numStrokesToRemove, numStrokesToRemove);
    shapeVerify.Strokes.splice(0, numStrokesToRemove);

    shape.IsSource = true;
    shapeVerify.IsSource = true;
    shape.save(function(err) {
        if (err) {
            return res.status(400).send({
                message: "Unexpected error"
            });
        } else {
            shapeVerify.TotalGames = -1;
            shapeVerify.save(function(err) {
                if (err) {
                    return res.status(400).send({
                        message: "Unexpected error"
                    });
                } else {
                    idxCurr++;
                    if(idxCurr >= mInstructions.length) {
                        idxCurr = 0;
                    }
                    res.status(200).send({ message: true });
                }
            });
        }
    });
};

exports.matchShape = function(req, res) {
    var shape = new Shape(req.body);
    shape = _.extend(shape, req.body);

    var deviceId = shape.DeviceId;
    Shape.find({ DeviceId: deviceId, IsSource: true }).exec(function(err, shapes) {
        if (err) {
            return res.send(400, {
                message: "Unexpected error"
            });
        } else {
            if(shapes.length >= 1) {
                var shapeVerify = shapes[0];
                var matchScores = matchLogic.matchShapes(shape, shapeVerify);

                var isShapesMatch = true;
                var tempScore;
                var tempIsMatch;

                if(matchScores == -1) {
                    isShapesMatch = false;
                }
                else {
                    for(var idx = 0; idx < matchScores.length; idx++) {
                        tempScore = matchScores[idx];
                        tempIsMatch = true;
                        if(tempScore < MIN_SHAPE_COMPARISON) {
                            isShapesMatch = false;
                            tempIsMatch = false;
                        }

                        shape.Strokes[idx].Match = tempIsMatch;
                        shape.Strokes[idx].MatchScore = tempScore;
                    }
                }

                shape.Match = isShapesMatch;
                shape.save(function(err) {
                    if (err) {
                        return res.status(400).send({
                            message: "Unexpected error"
                        });
                    } else {
                        res.status(200).send({ message: isShapesMatch });
                    }
                });
            }
            else {
                res.status(200).send({ message: false });
            }
        }
    });
};

exports.matchShapeByName = function(req, res) {
    var shape = new Shape(req.body);
    shape = _.extend(shape, req.body);

    var name = shape.Name;
    Shape.find({ Name: name, IsSource: true }).sort( { Created: 1 } ).exec(function(err, shapes) {
        if (err) {
            return res.send(400, {
                message: "Unexpected error"
            });
        } else {
            if(shapes.length >= 1) {
                var shapeVerify = shapes[0];
                var matchScores = matchLogic.matchShapes(shape, shapeVerify);

                var isShapesMatch = true;
                var tempScore;
                var tempIsMatch;

                if(matchScores == -1) {
                    isShapesMatch = false;
                }
                else {
                    for(var idx = 0; idx < matchScores.length; idx++) {
                        tempScore = matchScores[idx];
                        tempIsMatch = true;
                        if(tempScore < MIN_SHAPE_COMPARISON) {
                            isShapesMatch = false;
                            tempIsMatch = false;
                        }

                        shape.Strokes[idx].Match = tempIsMatch;
                        shape.Strokes[idx].MatchScore = tempScore;
                    }
                }

                shape.Match = isShapesMatch;
                shape.save(function(err) {
                    if (err) {
                        return res.status(400).send({
                            message: "Unexpected error"
                        });
                    } else {
                        //res.status(200).send({ message: isShapesMatch });
                        shapeVerify.TotalGames++;
                        shapeVerify.save(function(err) {
                            if (err) {
                                return res.status(400).send({
                                    message: "Unexpected error"
                                });
                            } else {
                                res.status(200).send({ message: isShapesMatch });
                            }
                        });
                    }
                });
            }
            else {
                res.status(200).send({ message: false });
            }
        }
    });
};

exports.selfMatchShape = function(req, res) {

    var shape = new Shape(req.body);
    shape = _.extend(shape, req.body);
    var numStrokes = shape.Strokes.length;
    var numStrokesToRemove = numStrokes / 2;

    var shapeVerify = new Shape(req.body);
    shapeVerify = _.extend(shapeVerify, req.body);

    shape.Strokes.splice(numStrokesToRemove, numStrokesToRemove);
    shapeVerify.Strokes.splice(0, numStrokesToRemove);

    var matchScores = matchLogic.matchShapes(shape, shapeVerify);

    var isShapesMatch = true;
    var tempScore;
    var tempIsMatch;

    if(matchScores == -1) {
        isShapesMatch = false;
    }
    else {
        for(var idx = 0; idx < matchScores.length; idx++) {
            tempScore = matchScores[idx];
            tempIsMatch = true;
            if(tempScore < MIN_SHAPE_COMPARISON) {
                isShapesMatch = false;
                tempIsMatch = false;
            }

            shape.Strokes[idx].Match = tempIsMatch;
            shape.Strokes[idx].MatchScore = tempScore;
        }
        shape.Match = isShapesMatch;
    }

    res.status(200).send({ message: isShapesMatch });
};

exports.getListOfUsers = function(req, res, next) {

    if(req.query.token != null && req.query.token == '561e660018d497581feedc78') {
        Shape.find({ IsSource: true, GcmToken: {$exists: true}, TotalGames: { $lt: 17, $gt: -1 }  }).sort( { Created: 1 } ).exec(function(err, shapes) {
            if (err) {
                return res.send(400, {
                    message: "Unexpected error"
                });
            } else {
                if(shapes.length == 0) {
                    return res.send(200, {
                        message: "No messages were sent"
                    });
                }
                else {
                    req.users = shapes;
                    next();
                }
            }
        });
    }
    else {
        return res.send(400, {
            message: "Bad token"
        });
    }


};

exports.filterUsers = function(req, res, next) {
    var users = req.users;
    var listGcmTokens = [];

    var hashTokens = [];

    for(var idx = 0; idx < users.length; idx++) {
        if(users[idx].GcmToken.length > 0 &&  hashTokens[users[idx].GcmToken] == undefined) {
            hashTokens[users[idx].GcmToken] = true;
            listGcmTokens.push(users[idx].GcmToken);
        }
    }

    req.listGcmTokens = listGcmTokens;
    next();
};

exports.sendNotification = function(req, res) {

    var listGcmTokens = req.listGcmTokens;

    if(req.listGcmTokens.length > 0) {
        var data = {
            "collapseKey": "applice",
            "delayWhileIdle": false,
            "timeToLive": 3,
            "registration_ids": req.listGcmTokens,
            "data": {
                "message" : "Alert"
            }
        };

        var dataString =  JSON.stringify(data);
        var headers = {
            'Authorization': 'key=AIzaSyDT8cGXfosqTnmLxKr536lEcR4dq4gEvbg',
            'Content-Type' : 'application/json',
            'Content-Length' : dataString.length
        };

        var options = {
            host: 'android.googleapis.com',
            port: 80,
            path: '/gcm/send',
            method: 'POST',
            headers: headers
        };

        var responseObject = res;

        var req = http.request(options, function(res) {
            res.setEncoding('utf-8');

            var responseString = '';

            res.on('data', function(data) {
                responseString += data;
            });

            var msg = listGcmTokens.length.toString() + " notification(s) were sent";
            res.on('end', function() {
                responseObject.status(200).send({ message: msg });
            });
        });

        req.on('error', function(e) {
            res.status(400).send({ message: "An error has occured" });
        });

        req.write(dataString);
        req.end();
    }


}