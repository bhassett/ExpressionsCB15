/// <reference path="../core.js" />

(function ($) {

    var config = {};
    var thisMatrixOptionPlugin;
    var thisMatrixGroup = null;
    var isCurrentylAddingItem = false;

    var pluginConstants = {
        EMPTY_VALUE: "",
        DOT_VALUE: ".",
        ZERO_VALUE: 0,
        UNDEFINED: "undefined",
        BODY: "body",
        TYPE_FUNCTION: "function",
        TYPE_CART: 0,
        TYPE_WISHLIST: 1,
        STAY: "stay",
        QUERY_STRING_QUESTION_SYMBOL: "?",
        QUERY_STRING_EQUALS_SYMBOL : "=",
        QUERY_STRING_ID: "matrixid",
    }

    var selectedItemCounter = pluginConstants.ZERO_VALUE;
    var selectedItemCode = pluginConstants.EMPTY_VALUE;

    var constantClassnames = {
        DISPLAY_NONE: "display-none",
        MATRIX_OPTIONS: "matrix-options",
        MATRIX_OPTION_SELECTED: "matrix-option-selected",
        MATRIX_IMAGE: "product-image-for-matrix-options",
        MATRIX_HIDDEN_OPTIONS: "matrix-hidden-options",
        MATRIX_ITEM_HEAD_PRICE_COLUMN: "th-matrix-selections-price-col",
        MATRIX_ITEM_BODY_PRICE_COLUMN: "td-matrix-selections-price-col",
        INPUT_RADIO_MATRIX_OPTIONS: "input-radio-matrix-options"
    }

    var constantIDs = {
        UOM_CONTAINER: "uom-container",
        ADD_TO_CART: "btnAddMatrixItemToCart",
        ADD_TO_WISHLIST: "btnAddMatrixItemToWishlist",
        INPUT_QUANTITY: "quantity",
        INPUT_UNITMEASURE_OPTIONS: "unitMeasureOptions",
        INPUT_OPTIONS: "inputRadio",
        BUTTON_ADD_TO_CART: "btnAddMatrixItemToCart",
        BUTTON_ADD_TO_WISHLIST: "btnAddMatrixItemToWishlist",
        DIV_ERRORMESSAGE: "divErrorMessage",
        UNIT_MEASURE_OPTIONS: "unitMeasureOptions"

    }

    var constantsTemplateID = {
        DEFAULT_UNIT_MEASURES: "matrixOptionsDefaultUnitMeasureTemplate",
        DROPDOWN_UNIT_MEASURES: "matrixOptionsDropdownUnitMeasureTemplate",
        ERROR_SUMMARY: "errorSummaryBoardTemplate"
    };

    var constantAttributes = {
        ATTR_COUNTER: "data-counter",
        ATTR_ITEMCODE: "data-itemCode",
        ATTR_STOCK: "data-stock",
        IMG_ICON_SIZE: "icon",
        IMG_MEDIUM_SIZE: "medium",
        IMG_LARGE_SIZE: "large",
        ATTR_CHECKED: "checked",
        ATTR_PRICE_FORMATTED: "data-priceFormatted",
        ATTR_CHANGE: "change"
    }

    var serviceMethods = {
        ADD_ITEM_TO_CART: "AddItemToCart"
    };

    var urlPage = {
        SHOPPING_CART: "shoppingcart.aspx",
        WISHLIST_CART: "wishlist.aspx"
    };

    var defaults = {
        pluginTemplateCss: 'components/matrix-options/skin/index.css',
        basePluginTemplateCss: 'components/matrix-options/skin/index.css', //if pluginTemplateCss is overridden but does not exist use the base which is the original path
        pluginTemplate: 'components/matrix-options/skin/jquery.matrixoptions.tmpl.html',
        basePluginTemplate: 'components/matrix-options/skin/jquery.matrixoptions.tmpl.html', //if pluginTemplate is overridden but does not exist use the base which is the original path
        matrixGroupCounter: 0,
        product: null,
        productUrl: pluginConstants.EMPTY_VALUE,
        itemCounterOnQueryString: 0,
        imageSize: constantAttributes.IMG_MEDIUM_SIZE,
        hideUnitMeasure: false,
        addToCartText: pluginConstants.EMPTY_VALUE,
        addToWishListText: pluginConstants.EMPTY_VALUE,
        addToCartAction: pluginConstants.EMPTY_VALUE,
        messages: {
            ADDING_ITEM: pluginConstants.EMPTY_VALUE,
            INVALID_QUANTITY_FORMAT: pluginConstants.EMPTY_VALUE,
            INVALID_QUANTITY_ZERO: pluginConstants.EMPTY_VALUE,
            NO_ITEM_SELECTED: pluginConstants.EMPTY_VALUE,
            UNDEFINED_UNIT_MEASURE_CODE: pluginConstants.EMPTY_VALUE
        }
    };

    var global = {
        selected: '',
        selector: ''
    };

    var init = $.prototype.init;
    $.prototype.init = function (selector, context) {
        var r = init.apply(this, arguments);
        if (selector && selector.selector) {
            r.context = selector.context, r.selector = selector.selector;
        }
        if (typeof selector == 'string') {
            r.context = context || document, r.selector = selector;
            global.selector = r.selector;
        }
        global.selected = r;
        return r;
    }

    $.prototype.init.prototype = $.prototype;

    $.fn.MatrixOptions = {

        setup: function (optionalConfig) {

            thisMatrixOptionPlugin = this;

            if (optionalConfig) setConfig($.extend(defaults, optionalConfig));
            var config = getConfig();

            thisMatrixOptionPlugin.initializeProduct(config.product);
            thisMatrixOptionPlugin.attachEvents(config);
            thisMatrixOptionPlugin.hideOutofStockItem();

            thisMatrixOptionPlugin.downloadPluginSkin(function () {
                thisMatrixOptionPlugin.selectItemFromQueryString();
            });

        },

        initializeProduct: function (o) {

            if (typeof (o) == pluginConstants.UNDEFINED || o == null) {
                thisMatrixGroup = null;
                return false;
            }

            thisMatrixGroup = o.getProduct(config.matrixGroupCounter);
            if (thisMatrixGroup.hidePriceUntilCart) {
                thisMatrixOptionPlugin.hidePriceColumn();
            }
        },

        hidePriceColumn: function () {
            $(pluginConstants.DOT_VALUE + constantClassnames.MATRIX_ITEM_HEAD_PRICE_COLUMN).html(pluginConstants.EMPTY_VALUE);
            $(pluginConstants.DOT_VALUE + constantClassnames.MATRIX_ITEM_BODY_PRICE_COLUMN).html(pluginConstants.EMPTY_VALUE);
        },

        addItemToCart: function (params) {

            var successCallback = function (response) {
                isCurrentylAddingItem = false;

                if (response == pluginConstants.EMPTY_VALUE) {
                    thisMatrixOptionPlugin.redirectToPage(params.cartTypeIndex);
                    return true;
                }

                var data = new Object();
                data.messages = [];
                var strMsg = response.d;
                strMsg = $.trim(strMsg);

                if (strMsg != pluginConstants.EMPTY_VALUE) {

                    data.messages.push({ description: strMsg });

                    var errorListHTML = thisMatrixOptionPlugin.parseJqueryTemplate(constantsTemplateID.ERROR_SUMMARY, data);
                    thisMatrixOptionPlugin.setElementHTMLContent(constantIDs.DIV_ERRORMESSAGE, errorListHTML);
                    $(thisMatrixOptionPlugin.selectorChecker(constantIDs.DIV_ERRORMESSAGE)).removeClass(constantClassnames.DISPLAY_NONE);

                    thisMatrixOptionPlugin.resetButtonText(params.cartTypeIndex);
                    thisMatrixOptionPlugin.jQueryScrollToTop();
                    return false;
                }

                data = null;

                if (thisMatrixOptionPlugin.isStayOnPageUponAddingItemToCart()) {

                    var productUrl = $.trim(config.productUrl);
                    var counter = parseInt(params.itemCounter);

                    if (productUrl == pluginConstants.EMPTY_VALUE || (typeof (counter) == pluginConstants.UNDEFINED || isNaN(counter) || counter == 0)) {
                        location.reload();
                        return false;
                    }

                    parent.location = productUrl + pluginConstants.QUERY_STRING_QUESTION_SYMBOL + pluginConstants.QUERY_STRING_ID + pluginConstants.QUERY_STRING_EQUALS_SYMBOL + counter;
                    return false;
                }

                thisMatrixOptionPlugin.redirectToPage(params.cartTypeIndex);

            };

            var failedCallback = function (response) {
                thisMatrixOptionPlugin.resetButtonText(params.cartTypeIndex);
                isCurrentylAddingItem = false;
            };

            thisMatrixOptionPlugin.ajaxSecureRequest(serviceMethods.ADD_ITEM_TO_CART, params, successCallback, failedCallback);
        },

        attachEvents: function (config) {

            $(pluginConstants.DOT_VALUE + constantClassnames.MATRIX_OPTIONS).unbind("click").click(function () {

                if (isCurrentylAddingItem == true) {
                    return false;
                }

                thisMatrixOptionPlugin.selectMatrixItem($(this));

            });

            $(thisMatrixOptionPlugin.selectorChecker(constantIDs.ADD_TO_CART)).unbind("click").click(function () {
                var params = thisMatrixOptionPlugin.getMatrixItemParams(pluginConstants.TYPE_CART);
                if (params == false || isCurrentylAddingItem == true) {
                    return false;
                }

                thisMatrixOptionPlugin.setButtonSpinnerText($(this), config.messages.ADDING_ITEM);
                isCurrentylAddingItem = true;

                thisMatrixOptionPlugin.addItemToCart(params);

            });

            $(thisMatrixOptionPlugin.selectorChecker(constantIDs.ADD_TO_WISHLIST)).unbind("click").click(function () {
                var params = thisMatrixOptionPlugin.getMatrixItemParams(pluginConstants.TYPE_WISHLIST);
                if (params == false || isCurrentylAddingItem == true) {
                    return false;
                }

                thisMatrixOptionPlugin.setButtonSpinnerText($(this), config.messages.ADDING_ITEM);
                isCurrentylAddingItem = true;

                thisMatrixOptionPlugin.addItemToCart(params);

            });
        },

        isStayOnPageUponAddingItemToCart: function () {

            var config = getConfig();
            var value = config.addToCartAction;

            if (typeof (value) == pluginConstants.UNDEFINED || value == pluginConstants.EMPTY_VALUE) {
                return false;
            }

            value = value.toLowerCase();
            value = $.trim(value);

            return (value == pluginConstants.STAY);
        },

        getMatrixItemParams: function (cartType) {

            var config = getConfig();

            var quantity = parseInt($(thisMatrixOptionPlugin.selectorChecker(constantIDs.INPUT_QUANTITY)).val());
            var itemCounter = parseInt(selectedItemCounter);

            var unitMeasureCode = $(thisMatrixOptionPlugin.selectorChecker(constantIDs.INPUT_UNITMEASURE_OPTIONS)).val();
            unitMeasureCode = $.trim(unitMeasureCode);

            var data = new Object();
            data.messages = [];

            if (typeof (selectedItemCode) == pluginConstants.UNDEFINED || selectedItemCode == pluginConstants.EMPTY_VALUE || isNaN(itemCounter) || itemCounter == pluginConstants.ZERO_VALUE) {
                data.messages.push({ description: config.messages.NO_ITEM_SELECTED });
            }

            if ((typeof (unitMeasureCode) == pluginConstants.UNDEFINED || unitMeasureCode == pluginConstants.EMPTY_VALUE) && config.hideUnitMeasure == false) {
                data.messages.push({ description: config.messages.UNDEFINED_UNIT_MEASURE_CODE });
            }

            if (isNaN(quantity) || quantity == pluginConstants.ZERO_VALUE) {
                data.messages.push({ description: config.messages.INVALID_QUANTITY_ZERO });
            } else {

                stringQty = $(thisMatrixOptionPlugin.selectorChecker(constantIDs.INPUT_QUANTITY)).val();
                if (!stringQty.match(thisMatrixGroup.QuantityRegEx)) {
                    data.messages.push({ description: config.messages.INVALID_QUANTITY_FORMAT });
                }

            }

            if (data.messages.length > 0) {

                var errorListHTML = thisMatrixOptionPlugin.parseJqueryTemplate(constantsTemplateID.ERROR_SUMMARY, data);
                thisMatrixOptionPlugin.setElementHTMLContent(constantIDs.DIV_ERRORMESSAGE, errorListHTML);
                $(thisMatrixOptionPlugin.selectorChecker(constantIDs.DIV_ERRORMESSAGE)).removeClass(constantClassnames.DISPLAY_NONE);

                isCurrentylAddingItem = false;
                data = null;
                thisMatrixOptionPlugin.jQueryScrollToTop();
                return false;
            }

            data = null;

            cartTypeIndex = parseInt(cartType);
            if (isNaN(cartTypeIndex)) {
                cartTypeIndex = 0;
            }

            return { itemCounter: itemCounter, itemCode: selectedItemCode, quantity: quantity, unitMeasureCode: unitMeasureCode, cartTypeIndex: cartTypeIndex };
        },

        setButtonSpinnerText: function (o, text) {
            o.html("<i class='icon-spin icon-spinner '></i> " + text);
        },

        selectItemFromQueryString: function () {

            var config = getConfig();
            if (config.itemCounterOnQueryString == 0) {
                return false;
            }

            $(pluginConstants.DOT_VALUE + constantClassnames.MATRIX_OPTIONS).each(function () {
                var $this = $(this);

                var itemCounter = $this.attr("data-counter");
                itemCounter = parseInt(itemCounter);

                if (!isNaN(itemCounter) && itemCounter == config.itemCounterOnQueryString) {
                    $this.triggerHandler("click");
                    return false;
                }
            });

           
        },

        selectMatrixItem: function (o) {
            var config = getConfig();

        

            $(pluginConstants.DOT_VALUE + constantClassnames.INPUT_RADIO_MATRIX_OPTIONS).removeAttr(constantAttributes.ATTR_CHECKED);

            $(pluginConstants.DOT_VALUE + constantClassnames.MATRIX_OPTIONS).removeClass(constantClassnames.MATRIX_OPTION_SELECTED);
            o.addClass(constantClassnames.MATRIX_OPTION_SELECTED);

            var itemCode = o.attr(constantAttributes.ATTR_ITEMCODE);
            itemCode = thisMatrixOptionPlugin.trimString(itemCode);

            if (config.hideUnitMeasure == false) {
                var unitmeasures = thisMatrixOptionPlugin.getMatrixItemUnitMeasure(itemCode, thisMatrixGroup.matrixProducts);
                thisMatrixOptionPlugin.setMatrixItemUnitMeasures(unitmeasures, itemCode);
            }

            itemWebConfig = thisMatrixOptionPlugin.getItemWebConfig(itemCode, thisMatrixGroup.matrixProducts);

            if (itemWebConfig.isCallToOrder) {

                $("#xmlMatrixOptionsButtonControlsWrapper").fadeOut("fast");

            } else {

                $("#xmlMatrixOptionsButtonControlsWrapper").fadeIn("fast", function () {

                    if (itemWebConfig.isShowBuyButton == false) {
                        $(thisMatrixOptionPlugin.selectorChecker(constantIDs.ADD_TO_CART)).fadeOut("fast");
                    } else {
                        $(thisMatrixOptionPlugin.selectorChecker(constantIDs.ADD_TO_CART)).fadeIn("fast");
                    }
                });

            }

            var image = thisMatrixOptionPlugin.getMatrixItemImage(itemCode, thisMatrixGroup.matrixProducts);
            thisMatrixOptionPlugin.setMatrixItemImageSource(image);

            selectedItemCode = itemCode;
            selectedItemCounter = o.attr(constantAttributes.ATTR_COUNTER);

            $(thisMatrixOptionPlugin.selectorChecker(constantIDs.DIV_ERRORMESSAGE)).addClass(constantClassnames.DISPLAY_NONE);
            $(thisMatrixOptionPlugin.selectorChecker(constantIDs.INPUT_OPTIONS + selectedItemCounter)).attr(constantAttributes.ATTR_CHECKED, constantAttributes.ATTR_CHECKED);

            $(thisMatrixOptionPlugin.selectorChecker(constantIDs.UNIT_MEASURE_OPTIONS)).triggerHandler(constantAttributes.ATTR_CHANGE);
        },

        getMatrixItemImage: function (itemCode, o) {
            var config = getConfig();

            var item = $.grep(o, function (e) { return e.itemCode == itemCode; });
            if (typeof (item) != undefined && item.length > 0) {

                var image = item[0].imageData;
                var source = null;

                switch (config.size) {
                    case constantAttributes.IMG_ICON_SIZE:
                        source = image.icon.src;
                        break;
                    case constantAttributes.IMG_MEDIUM_SIZE:
                        source = image.medium.src; ;
                        break;
                    case constantAttributes.IMG_LARGE_SIZE:
                        source = image.icon.src;
                        break;
                    default:
                        source = image.medium.src;
                        break;
                }

                return source;

            } else {
                return null;
            }
        },

        getMatrixItemUnitMeasure: function (itemCode, o) {

            var item = $.grep(o, function (e) { return e.itemCode == itemCode; });

            if (typeof (item) != pluginConstants.UNDEFINED && item.length > 0) {
                return item[0].unitMeasures;
            }

            return pluginConstants.EMPTY_VALUE;
        },

        getItemWebConfig: function(itemCode, o){

            var data = new Object();
            data.isShowBuyButton = true;
            data.isCallToOrder = false;

            var item = $.grep(o, function (e) { return e.itemCode == itemCode; });

            if (typeof (item) != pluginConstants.UNDEFINED && item.length > 0) {
                data.isShowBuyButton = item[0].showBuyButton;
                data.isCallToOrder = item[0].isCallToOrder;
            }

            return data;
        },

        redirectToPage: function (cartTypeIndex) {

            if (cartTypeIndex == pluginConstants.TYPE_WISHLIST) {
                parent.location = urlPage.WISHLIST_CART;
                return false;
            }

            parent.location = urlPage.SHOPPING_CART;

        },

        resetButtonText: function (cartTypeIndex) {

            var config = getConfig();

            if (cartTypeIndex == pluginConstants.TYPE_CART) {
                $(thisMatrixOptionPlugin.selectorChecker(constantIDs.BUTTON_ADD_TO_CART)).html(config.addToCartText);
            }

            if (cartTypeIndex == pluginConstants.TYPE_WISHLIST) {
                $(thisMatrixOptionPlugin.selectorChecker(constantIDs.BUTTON_ADD_TO_WISHLIST)).html(config.addToWishListText);
            }

        },

        setMatrixItemImageSource: function (src) {
            $(pluginConstants.DOT_VALUE + constantClassnames.MATRIX_IMAGE).attr("src", src);
        },

        setMatrixItemUnitMeasures: function (uom, selectedItemCode) {

            var unitMeasure = new Object();
            var unitMeasureDisplay = pluginConstants.EMPTY_VALUE;

            if (uom.length == 1) {

                unitMeasure.value = uom[0].description;
                unitMeasureDisplay = thisMatrixOptionPlugin.parseJqueryTemplate(constantsTemplateID.DEFAULT_UNIT_MEASURES, { unitMeasure: unitMeasure });

            } else {

                var data = new Object();
                data.unitMeasures = [];

                $.each(uom, function (index, o) {
                    var price = (o.hasPromotionalPrice && isNaN(o.promotionalPrice) == false && o.promotionalPrice > 0) ? o.promotionalPriceFormatted : o.priceFormatted;
                    data.unitMeasures.push({ code: o.code, description: o.description, price: price, itemCode: selectedItemCode });
                });

                unitMeasureDisplay = thisMatrixOptionPlugin.parseJqueryTemplate(constantsTemplateID.DROPDOWN_UNIT_MEASURES, data);

            }

            thisMatrixOptionPlugin.setElementHTMLContent(constantIDs.UOM_CONTAINER, unitMeasureDisplay);
            unitmeasure = null;

            $(thisMatrixOptionPlugin.selectorChecker(constantIDs.UNIT_MEASURE_OPTIONS)).unbind("change").change(function () {
                var $this = $('option:selected', $(this));

                var price = $this.attr(constantAttributes.ATTR_PRICE_FORMATTED);
                price = $.trim(price);

                var itemCode = $this.attr(constantAttributes.ATTR_ITEMCODE);
                itemCode = $.trim(itemCode);

                $(thisMatrixOptionPlugin.selectorChecker("tdPrice" + itemCode)).html(price);
            });

        },

        hideOutofStockItem: function () {
            $(pluginConstants.DOT_VALUE + constantClassnames.MATRIX_HIDDEN_OPTIONS).remove();
        },

        downloadPluginSkin: function (callback) {

            var config = getConfig();

            $.get(config.pluginTemplate, function (data, textStatus, XMLHttpRequest) {
                $(pluginConstants.BODY).append(data);
                thisMatrixOptionPlugin.downloadCss(config.pluginTemplateCss, function () {
                    if (typeof callback === pluginConstants.TYPE_FUNCTION) callback();
                });
            });

        },

        config: function (args) {
            setConfig($.extend(defaults, args));
            return (getConfig());
        },

        trimString: function (str) {
            if (typeof (str) == pluginConstants.UNDEFINED || str == null || str == pluginConstants.EMPTY_VALUE) {
                return pluginConstants.EMPTY_VALUE;
            }

            return $.trim(str);
        },

        jQueryScrollToTop: function () {
            $('html, body').animate({ scrollTop: 0 }, 'slow');
        }


    };

    //jqueryBasePlugin is located inside core.js
    $.extend($.fn.MatrixOptions, new jqueryBasePlugin());

    function setConfig(value) {
        config = value;
    }

    function getConfig() {
        return config;
    }

})(jQuery);