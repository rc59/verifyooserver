'use strict';

module.exports = function(app) {
    var shapes = require('../../app/controllers/shapes');

    app.route('/shapes/createTemplateDemo').post(shapes.createTemplateDemo);
};