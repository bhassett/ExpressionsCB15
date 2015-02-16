/// <reference path="../core.js" />

//immediate function execution
var instorePluginLoader = function () { };
instorePluginLoader.prototype = {
    start: function (callback) {

        var basePlugin = new jqueryBasePlugin()
        basePlugin.downloadPlugin('components/instore_pickup/instore-plugin.js', function () {

            var resources = ['checkoutstore.aspx.4', 'checkoutstore.aspx.2', 'checkoutstore.aspx.3', 'checkoutstore.aspx.4', 'checkoutstore.aspx.5', 'checkoutstore.aspx.6',
                'checkoutstore.aspx.7', 'checkoutstore.aspx.8', 'checkoutstore.aspx.9', 'checkoutstore.aspx.10', 'checkoutstore.aspx.11', 'search.aspx.5',
                'checkoutstore.aspx.12', 'checkoutstore.aspx.13', 'checkoutstore.aspx.14', 'checkoutstore.aspx.15', 'checkoutstore.aspx.18', 'checkoutstore.aspx.19',
                'checkoutstore.aspx.20', 'checkoutstore.aspx.21', 'checkoutstore.aspx.22', 'checkoutstore.aspx.23', 'checkoutstore.aspx.24', 'checkoutstore.aspx.25',
                'checkoutstore.aspx.26', 'checkoutstore.aspx.27', 'checkoutstore.aspx.28', 'showproduct.aspx.55', 'AppConfig.CartButtonPrompt', 'checkout1.aspx.66',
                'checkoutstore.aspx.33', 'checkout1.aspx.65', 'checkoutstore.aspx.34', 'shoppingcart.aspx.1', 'checkoutstore.aspx.35', 'checkoutstore.aspx.36', 'checkoutstore.aspx.37'];

            var configurations = ['ShowActualInventory', 'Inventory.LimitCartToQuantityOnHand'];

            basePlugin.downloadAppConfigs(configurations, function () {
                basePlugin.downloadStringResources(resources, function () {

                    var defaultSkinId = ise.Configuration.getConfigValue('DefaultSkinID');

                    var config = {
                        pluginTemplate: "skins/Skin_" + defaultSkinId + "/plugin/instore_pickup/skin/template.js",
                        pluginTemplateCss: "skins/Skin_" + defaultSkinId + "/plugin/instore_pickup/skin/index.css",
                        messages: {
                            MESSAGE_STORE_LIST_HEADER_TEXT: ise.StringResource.getString('checkoutstore.aspx.4'),
                            MESSAGE_STORE_LIST_SELECT_BUTTON_TEXT: ise.StringResource.getString('checkoutstore.aspx.14'),
                            MESSAGE_PICKUP_WHERE_TEXT: ise.StringResource.getString('checkoutstore.aspx.2'),
                            MESSAGE_PICKUP_HOW_TEXT: ise.StringResource.getString('checkoutstore.aspx.6'),
                            MESSAGE_SAMPLE_ADDRESS_INPUT_TEXT: ise.StringResource.getString('checkoutstore.aspx.3'),
                            MESSAGE_MENU_HEADER_AVAILABILITY: ise.StringResource.getString('checkoutstore.aspx.5'),
                            MESSAGE_MENU_HEADER_DIRECTION: ise.StringResource.getString('checkoutstore.aspx.7'),
                            MESSAGE_MENU_ZIP_POSTAL_TEXT: ise.StringResource.getString('checkoutstore.aspx.8'),
                            MESSAGE_MENU_CITY_TEXT: ise.StringResource.getString('checkoutstore.aspx.9'),
                            MESSAGE_MENU_STATE_TEXT: ise.StringResource.getString('checkoutstore.aspx.10'),
                            MESSAGE_MENU_COUNTRY_TEXT: ise.StringResource.getString('checkoutstore.aspx.11'),
                            MESSAGE_MENU_BUTTON_SEARCH_TEXT: ise.StringResource.getString('search.aspx.5'),
                            MESSAGE_MENU_COUNTRY_SELECT_TEXT: ise.StringResource.getString('checkoutstore.aspx.12'),
                            MESSAGE_MENU_SEARCH_WHERE_VALIDATION_MESSAGE: ise.StringResource.getString('checkoutstore.aspx.13'),
                            MESSAGE_STORE_LIST_DIRECTION_LINK_TEXT: ise.StringResource.getString('checkoutstore.aspx.15'),
                            MESSAGE_STORE_LIST_HEADER_STORE_TEXT: ise.StringResource.getString('checkoutstore.aspx.18'),
                            MESSAGE_STORE_LIST_HEADER_AVAILABILITY_TEXT: ise.StringResource.getString('checkoutstore.aspx.19'),
                            MESSAGE_MAP_CLOSE_TEXT: ise.StringResource.getString('checkoutstore.aspx.20'),
                            MESSAGE_MAP_GET_DIRECTION_BUTTON_TEXT: ise.StringResource.getString('checkoutstore.aspx.21'),
                            MESSAGE_MAP_DIRECTION_ERROR_MESSAGE: ise.StringResource.getString('checkoutstore.aspx.22'),
                            MESSAGE_STORE_INFO_TITLE_TEXT: ise.StringResource.getString('checkoutstore.aspx.23'),
                            MESSAGE_STORE_HOUR_TITLE_TEXT: ise.StringResource.getString('checkoutstore.aspx.24'),
                            MESSAGE_MAP_DIRECTION_HEADER_TEXT: ise.StringResource.getString('checkoutstore.aspx.25'),
                            MESSAGE_MENU_SEARCH_NO_RECORD_FOUND_MESSAGE: ise.StringResource.getString('checkoutstore.aspx.26'),
                            MESSAGE_STORE_CLOSE_TEXT: ise.StringResource.getString('checkoutstore.aspx.27'),
                            MESSAGE_STORE_HOLIDAY_HEADER_TEXT: ise.StringResource.getString('checkoutstore.aspx.28'),
                            MESSAGE_STORE_LIST_HEADER_STOCK_TEXT: ise.StringResource.getString('showproduct.aspx.55'),
                            MESSAGE_STORE_TELEPHONE_HEADER_TEXT: ise.StringResource.getString('checkout1.aspx.66'),
                            MESSAGE_STORE_FAX_HEADER_TEXT: ise.StringResource.getString('checkoutstore.aspx.33'),
                            MESSAGE_STORE_EMAIL_HEADER_TEXT: ise.StringResource.getString('checkout1.aspx.65'),
                            MESSAGE_STORE_WEBSITE_HEADER_TEXT: ise.StringResource.getString('checkoutstore.aspx.34'),
                            MESSAGE_STORE_STOCK_QUANTITY_ON_HAND: ise.StringResource.getString('shoppingcart.aspx.1'),
                            MESSAGE_STORE_SELECT_CHANGE_WAREHOUSE: ise.StringResource.getString('checkoutstore.aspx.35'),
                            MESSAGE_STORE_SELECT_CHANGE_WAREHOUSE1: ise.StringResource.getString('checkoutstore.aspx.36'),
                            MESSAGE_STORE_SELECT_CHANGE_WAREHOUSE2: ise.StringResource.getString('checkoutstore.aspx.37')
                        },
                        appConfig : {
                            ShowActualStock: (ise.Configuration.getConfigValue('ShowActualInventory') == 'true'),
                            LimitCartToQuantityOnHand: (ise.Configuration.getConfigValue('Inventory.LimitCartToQuantityOnHand') == 'true')
                        }
                    }

                    if (callback && typeof callback != 'undefined') callback(config);
                });
            });

        });
    }
}