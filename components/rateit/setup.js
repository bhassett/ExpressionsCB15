/// <reference path="../core.js" />

//immediate function execution
var instorePluginLoader = function () { };
instorePluginLoader.prototype = {
    start: function (callback) {

        var basePlugin = new jqueryBasePlugin()

        basePlugin.downloadPlugin('components/rateStar/rateStar-plugin.js', function () {

            basePlugin.downloadPlugin('components/rateit/rateit-plugin.js', function () {

                var resources = ['rateit.aspx.3'];

                var configurations = [];

                basePlugin.downloadAppConfigs(configurations, function () {
                    basePlugin.downloadStringResources(resources, function () {

                        var defaultSkinId = ise.Configuration.getConfigValue('DefaultSkinID');

                        var config = {
                            pluginTemplate: "skins/Skin_" + defaultSkinId + "/plugin/rateit/skin/template.js",
                            pluginTemplateCss: "skins/Skin_" + defaultSkinId + "/plugin/rateit/skin/index.css",
                            Captions: {
                                HEADER: ise.StringResource.getString('rateit.aspx.3')
                            },
                            Config: {
                                SkinId: defaultSkinId
                            },
                            messages: {
                            }
                        }

                        if (callback && typeof callback != 'undefined') callback(config);
                    });
                });

            });

        });
    }
}