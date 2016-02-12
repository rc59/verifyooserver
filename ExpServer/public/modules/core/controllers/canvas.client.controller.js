'use strict';

angular.module('core').controller('CanvasController', ['$scope',
	function($scope) {

        $scope.clear = function() {
            var c = document.getElementById("myCanvas");
            var ctx = c.getContext("2d");

            ctx.clearRect(0, 0, 2000, 2000);
        }

        $scope.drawFromList = function() {

            var value = document.getElementById('myTextArea2').value;

            var list = value.split(",");
            var idxList = 0;


            var c = document.getElementById("myCanvas");
            var ctx = c.getContext("2d");

            var ObjectId = function() {};

            var value = document.getElementById('myTextArea').value;

            var timeToWait = 50;

            var drawByIdxFromList = function() {
                ctx.moveTo(list[idxList], list[idxList+1]);
                idxList += 2;
                ctx.lineTo(list[idxList], list[idxList+1]);
                ctx.stroke();

                if(idxList < list.length) {
                    setTimeout(drawByIdxFromList, timeToWait);
                }
            }

            drawByIdxFromList();

        }

        $scope.draw = function(){
            var c = document.getElementById("myCanvas");
            var ctx = c.getContext("2d");

            var ObjectId = function() {};

            var value = document.getElementById('myTextArea').value;

            var isFinished = false;
            var idxToRemove;
            while(!isFinished) {

                if(value.indexOf('_id') < 0) {
                    isFinished = true;
                }
                else {
                    idxToRemove = value.indexOf('_id');
                    value = value.replace(value.substring(idxToRemove-4, idxToRemove + 47), "");
                }
            }

            isFinished = false;
            while(!isFinished) {

                if(value.indexOf("\n") < 0) {
                    isFinished = true;
                }
                else {
                    value = value.replace("\n", "");
                }
            }

            value = value.replace(/"/g, '\\"');
            value = '"' + value + '"';

            var arr = JSON.parse(JSON.parse(value));

            var timeToWait = 0;
            var idx = 0;
            var stroke = 0;

            var drawByIdx = function() {
                if(arr[stroke].ListEvents[idx+1] != null) {
                    timeToWait = arr[stroke].ListEvents[idx+1].EventTime - arr[stroke].ListEvents[idx].EventTime;

                    ctx.moveTo(arr[stroke].ListEvents[idx].X,arr[stroke].ListEvents[idx].Y);
                    ctx.lineTo(arr[stroke].ListEvents[idx+1].X,arr[stroke].ListEvents[idx+1].Y);
                    ctx.stroke();
                }

                idx++;
                if(arr[stroke].ListEvents.length > idx) {
                    setTimeout(drawByIdx, timeToWait);
                }
                else {
                    stroke++;
                    idx = 0;

                    if(arr.length > stroke) {
                        drawByIdx();
                    }
                }
            }

            drawByIdx();
        }
	}
]);