/// <reference path="../core.js" />

//immediate function execution
var matrixlistingPluginLoader = function () { };
matrixlistingPluginLoader.prototype = {
    start: function (callback) {

        var basePlugin = new jqueryBasePlugin()
        basePlugin.downloadPlugin('components/matrix-listing/matrixlisting-plugin.js', function () {
            var config = {}
            if (callback && typeof callback != 'undefined') callback(config);
        });
    }
}

