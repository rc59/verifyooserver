'use strict';

module.exports = function(app) {
    var shapes = require('../../app/controllers/shapes');

    app.route('/shapes/isUserExistsByName').get(shapes.isUserExistsByName);
    app.route('/shapes/isUserExists').get(shapes.isUserExists);
    app.route('/shapes/create').post(shapes.create);
    app.route('/shapes/matchByName').post(shapes.matchShapeByName);
    app.route('/shapes/match').post(shapes.matchShape);
    app.route('/shapes/selfMatch').post(shapes.selfMatchShape);

    app.route('/shapes/sendNotification').get(shapes.getListOfUsers, shapes.filterUsers, shapes.sendNotification);
};