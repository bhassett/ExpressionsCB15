/// <reference path="../core.js" />

//immediate function execution
var instorePluginLoader = function () { };
instorePluginLoader.prototype = {
    start: function (callback) {

        var basePlugin = new jqueryBasePlugin()

        basePlugin.downloadPlugin('components/rateStar/rateStar-plugin.js', function () {

            basePlugin.downloadPlugin('components/rating/rating-plugin.js', function () {

                var resources = ['rateit.aspx.17', 'rateit.aspx.19', 'rateit.aspx.20', "rateit.aspx.21", 'rateit.aspx.18', 'rateit.aspx.26', 'rateit.aspx.22', 'rateit.aspx.23',
                                 'rateit.aspx.24', 'rateit.aspx.25', 'rateit.aspx.11', 'rateit.aspx.10', 'rateit.aspx.9', 'rateit.aspx.8', 'rateit.aspx.7', "ratings.cs.7", "ratings.cs.30",
                                 'ratings.cs.1', 'ratings.cs.2', 'ratings.cs.3', 'ratings.cs.4', 'ratings.cs.5', 'ratings.cs.6', 'ratings.cs.10'];

                var configurations = ['RatingsCanBeDoneByAnons', 'RatingsPageSize'];

                basePlugin.downloadAppConfigs(configurations, function () {

                    basePlugin.downloadStringResources(resources, function () {

                        var defaultSkinId = ise.Configuration.getConfigValue('DefaultSkinID');

                        var config = {
                            pluginTemplate: "skins/Skin_" + defaultSkinId + "/plugin/rating/skin/template.js",
                            pluginTemplateCss: "skins/Skin_" + defaultSkinId + "/plugin/rating/skin/index.css",
                            Captions: {
                                HEADER: ise.StringResource.getString('rateit.aspx.17'),
                                HAS_RATING: ise.StringResource.getString("rateit.aspx.21"),
                                AVE_RATING: ise.StringResource.getString("rateit.aspx.18"),
                                CLICK_HERE: ise.StringResource.getString("rateit.aspx.26"),
                                ONLY_REG_CAN_RATE: ise.StringResource.getString('rateit.aspx.22'),
                                TO_BE_THE_FIRST: ise.StringResource.getString('rateit.aspx.23'),
                                TO_RATE_THIS_PRODUCT: ise.StringResource.getString('rateit.aspx.24'),
                                CHANGE_YOUR_RATING: ise.StringResource.getString('rateit.aspx.25'),
                                OUT_OF: ise.StringResource.getString('rateit.aspx.19'),
                                VOTES: ise.StringResource.getString('rateit.aspx.20'),
                                GREAT: ise.StringResource.getString('rateit.aspx.11'),
                                GOOD: ise.StringResource.getString('rateit.aspx.10'),
                                OK: ise.StringResource.getString('rateit.aspx.9'),
                                BAD: ise.StringResource.getString('rateit.aspx.8'),
                                TERRIBLE: ise.StringResource.getString('rateit.aspx.7'),
                                SORT_HEADER: ise.StringResource.getString('ratings.cs.7'),
                                VIEW_COMMENTS: ise.StringResource.getString('ratings.cs.30'),
                                NO_COMMENT: ise.StringResource.getString('ratings.cs.10'),

                                HELPFUL: ise.StringResource.getString('ratings.cs.1'),
                                LESS_HELPFUL: ise.StringResource.getString('ratings.cs.2'),
                                NEW_OLD: ise.StringResource.getString('ratings.cs.3'),
                                OLD_NEW: ise.StringResource.getString('ratings.cs.4'),
                                HIGH_LOW: ise.StringResource.getString('ratings.cs.5'),
                                LOW_HIGH: ise.StringResource.getString('ratings.cs.6')
                            },
                            Config: {
                                SkinId: defaultSkinId,
                                AllowAnonToRate: ise.Configuration.getConfigValue('RatingsCanBeDoneByAnons'),
                                RateItUrl: 'RateIt.aspx',
                                RatingPageSize: ise.Configuration.getConfigValue('RatingsPageSize')
                            },
                            messages: {
                                MESSAGE_STORE_LIST_HEADER_TEXT: '',
                            }
                        }

                        if (callback && typeof callback != 'undefined') callback(config);
                    });
                });

            });

        })

        
    }
}