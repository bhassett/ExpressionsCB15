/// <reference path="../core.js" />

//immediate function execution
var realtimeAddressVerificationPluginLoader = function () { };
realtimeAddressVerificationPluginLoader.prototype = {
    start: function (callback) {

        var basePlugin = new jqueryBasePlugin();

        basePlugin.downloadPlugin('components/address-verification/real-time-address-verification-plugin.min.js', function () {

            var resources = [''];
            var configurations = [''];

            basePlugin.downloadAppConfigs(configurations, function () {
                basePlugin.downloadStringResources(resources, function () {

                    var defaultSkinId = ise.Configuration.getConfigValue('DefaultSkinID');

                    var config = {
                        pluginTemplateCss: "skins/Skin_" + defaultSkinId + "/plugin/address-verification/index.css"
                    }

                    if (callback && typeof callback != 'undefined') callback(config);
                });
            });

        });
    }
}

