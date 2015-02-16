/// <reference path="../core.js" />

//immediate function execution
var matrixoptionsPluginLoader = function () { };
matrixoptionsPluginLoader.prototype = {
    start: function (callback) {

        var basePlugin = new jqueryBasePlugin()
        basePlugin.downloadPlugin('components/matrix-options/matrixoptions-plugin.js', function () {

            var resources = [''];
            var configurations = [''];

            basePlugin.downloadAppConfigs(configurations, function () {
                basePlugin.downloadStringResources(resources, function () {

                    var defaultSkinId = ise.Configuration.getConfigValue('DefaultSkinID');

                    var config = {
                        pluginTemplateCss: "skins/Skin_" + defaultSkinId + "/plugin/matrix-options/skin/index.css",
                    }

                    if (callback && typeof callback != 'undefined') callback(config);
                });
            });

        });
    }
}

