'use strict';

/**
 * Module dependencies.
 */
    var mongoose = require('mongoose'),
    _ = require('lodash'),
    euclidean = require('euclidean-distance');

var ORIENTATIONS = [
    0, (Math.PI / 4), (Math.PI / 2), (Math.PI * 3 / 4),
    Math.PI, -0, (-Math.PI / 4), (-Math.PI / 2),
    (-Math.PI * 3 / 4), -Math.PI
];

exports.compareBasicShape = function(stroke, strokeVerify) {
    var convertToVector = function(eventsList, length) {

        var minValue = -9999999;
        var numPoints = 16;

        var increment = length / (numPoints - 1);
        var vectorLength = numPoints * 2;
        var vector = []
        var distanceSoFar = 0;

        var pts = [];
        for(var idx = 0; idx < eventsList.length; idx++) {
            pts[idx * 2] = eventsList[idx].X;
            pts[idx * 2 + 1] = eventsList[idx].Y;
        }

        var lstPointX = pts[0];
        var lstPointY = pts[1];
        var index = 0;
        var currentPointX = minValue;
        var currentPointY = minValue;

        vector[index] = lstPointX;
        index++;
        vector[index] = lstPointY;
        index++;
        var i = 0;
        var count = pts.length / 2;
        while (i < count) {
            if (currentPointX == minValue) {
                i++;
                if (i >= count) {
                    break;
                }
                currentPointX = pts[i * 2];
                currentPointY = pts[i * 2 + 1];
            }
            var deltaX = currentPointX - lstPointX;
            var deltaY = currentPointY - lstPointY;
            var distance = Math.sqrt(deltaX * deltaX + deltaY * deltaY);
            if (distanceSoFar + distance >= increment) {
                var ratio = (increment - distanceSoFar) / distance;
                var nx = lstPointX + ratio * deltaX;
                var ny = lstPointY + ratio * deltaY;
                vector[index] = nx;
                index++;
                vector[index] = ny;
                index++;
                lstPointX = nx;
                lstPointY = ny;
                distanceSoFar = 0;
            } else {
                lstPointX = currentPointX;
                lstPointY = currentPointY;
                currentPointX = minValue;
                currentPointY = minValue;
                distanceSoFar += distance;
            }
        }

        for (i = index; i < vectorLength; i += 2) {
            vector[i] = lstPointX;
            vector[i + 1] = lstPointY;
        }
        return vector;
    }

    var computeCentroid = function(points) {
        var centerX = 0;
        var centerY = 0;
        var count = points.length;
        for (var i = 0; i < count; i++) {
            centerX += points[i];
            i++;
            centerY += points[i];
        }
        var center = [];
        center[0] = 2 * centerX / count;
        center[1] = 2 * centerY / count;

        return center;
    }

    var translate = function(points, dx, dy) {
        var size = points.length;
        for (var i = 0; i < size; i += 2) {
            points[i] += dx;
            points[i + 1] += dy;
        }
        return points;
    }

    var rotate = function(points, angle) {
        var cos = Math.cos(angle);
        var sin = Math.sin(angle);
        var size = points.length;
        for (var i = 0; i < size; i += 2) {
            var x = points[i] * cos - points[i + 1] * sin;
            var y = points[i] * sin + points[i + 1] * cos;
            points[i] = x;
            points[i + 1] = y;
        }
        return points;
    }

    var normalize = function(vector) {
        var sample = vector;
        var sum = 0;

        var size = sample.length;
        for (var i = 0; i < size; i++) {
            sum += sample[i] * sample[i];
        }

        var magnitude = Math.sqrt(sum);
        for (var i = 0; i < size; i++) {
            sample[i] /= magnitude;
        }

        return sample;
    }

    var prepareData = function(eventsList, length) {
        var pts = convertToVector(eventsList, length);
        var center = computeCentroid(pts);
        var orientation = Math.atan2(pts[1] - center[1], pts[0] - center[0]);

        var adjustment = -orientation;
        //if (orientationType != GestureStore.ORIENTATION_INVARIANT) {
        var count = ORIENTATIONS.length;
        for (var i = 0; i < count; i++) {
            var delta = ORIENTATIONS[i] - orientation;
            if (Math.abs(delta) < Math.abs(adjustment)) {
                adjustment = delta;
            }
        }


        translate(pts, -center[0], -center[1]);
        rotate(pts, adjustment);

        pts = normalize(pts);

        return pts;
    }

    var getScore = function(vector1, vector2) {
        var distance = minimumCosineDistance(vector1, vector2, 2);

        var weight = 0;
        if (distance == 0) {
            weight = 300;
        } else {
            weight = 1 / distance;
        }

        return weight;
    }

    var minimumCosineDistance = function(vector1, vector2, numOrientations) {
        var len = vector1.length;
        var a = 0;
        var b = 0;
        for (var i = 0; i < len; i += 2) {
            a += vector1[i] * vector2[i] + vector1[i + 1] * vector2[i + 1];
            b += vector1[i] * vector2[i + 1] - vector1[i + 1] * vector2[i];
        }
        if (a != 0) {
            var tan = b/a;
            var angle = Math.atan(tan);
            if (numOrientations > 2 && Math.abs(angle) >= Math.PI / numOrientations) {
                return Math.acos(a);
            } else {
                var cosine = Math.cos(angle);
                var sine = cosine * tan;
                return Math.acos(a * cosine + b * sine);
            }
        } else {
            return Math.PI / 2;
        }
    }

    var eventsList = stroke.ListEvents;
    var length = stroke.Length;

    var eventsListVerify = strokeVerify.ListEvents;
    var lengthVerify = strokeVerify.Length;

    var vector1 = prepareData(eventsList, length);
    var vector2 = prepareData(eventsListVerify, lengthVerify);

    var score = getScore(vector1, vector2);

    var p1, p2;
    var tempDistance = 0;
    var distance = 0;
    var maxDistance = 0;
    for(var idx = 0; idx < vector1.length; idx = idx + 2) {
        p1 = [vector1[idx], vector1[idx+1]];
        p2 = [vector2[idx], vector2[idx+1]];

        tempDistance = euclidean(p1, p2);
        distance += tempDistance;
        if(tempDistance > maxDistance) {
            maxDistance = tempDistance;
        }
    }

    return score
}

exports.matchShapes = function(shape, shapeVerify) {

    if(shape.Strokes.length != shapeVerify.Strokes.length) {
        return -1;
    }

    var stroke, strokeVerify;
    var score;

    var scores = [];

    for(var idx = 0; idx < shape.Strokes.length; idx++) {
        stroke = shape.Strokes[idx];
        strokeVerify = shapeVerify.Strokes[idx];

        score = this.compareBasicShape(stroke, strokeVerify);
        scores[idx] = score;
//        if(score < MIN_SHAPE_COMPARISON) {
//            return false;
//        }
    }

    return scores;
}