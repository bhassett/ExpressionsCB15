/// <reference path="../core.js" />

(function ($) {

    var config = {};
    var thisMatrixListingPlugin;
   
    var pluginConstants = {
        EMPTY_VALUE: "",
        DOT_VALUE: ".",
        ZERO_VALUE: 0,
        TRUE_STRING_VALUE : "true",
        UNDEFINED: "undefined",
        BODY: "body",
        TYPE_FUNCTION: "function",
        TYPE_CART: 0,
        TYPE_WISHLIST: 1,
        STAY: "stay",
        QUERY_STRING_ID: "matrixid",
        DOUBLE_COLON_SEPARATOR: "::",
        FORWARD_SLASH_SEPARATOR: "/",
        SPACE_SEPARATOR : " ",
        SIGLE_SPACE: "&nbsp;"
    };

    var selectedItemCounter = pluginConstants.ZERO_VALUE;
    var selectedItemCode = pluginConstants.EMPTY_VALUE;
    var thisMatrixListingQuanitityRegEx = pluginConstants.EMPTY_VALUE;

    var constantClassnames = {
        DISPLAY_NONE: "display-none",
        MATRIX_ITEM_UNITMEASURE_SELECTOR: "matrix-item-unitmeasure-selector",
        CLOUD_ZOOM: "cloud-zoom",
        INVALID_QUANTITY: "invalid-quantity",
        MULTIPLE_IMAGES_NAV_SELECTED: "multiple-image-nav-selected"
    };

    var constantIDs = {
        REQUEST_INDICATOR : "matrix-page-loading-indicator",
        MATRIX_ITEMS_WRAPPER: "matrix-items-wrapper",
        VIEW_MORE_LINK: "view-more",
        PAGE_SELECTOR: "page-size",
        PAGE_SIZE_ALL: "page-size-all",
        DIV_MESSAGE : "ise-message",
        DIV_MESSAGE_TIPS: "ise-message-tips",
        DIV_MATRIX_ITEM_PRICE: "divMatrixItemPrice",
        DIV_MATRIX_ITEMS_BOTTOM_CONTROLS: "matrix-items-bottom-controls-wrapper",
        DIV_MATRIX_ITEMS_PLACEHOLDER: "page-items-place-holder",
        BUTTON_ADD_TO_CART: "matrix-item-listing-add-to-cart"
    };

    var constantsTemplateID = {
        UNIT_MEASURE_TEMPLATE: "unitMeasureTemplate",
        MATRIX_ITEMS_TEMPLATE: "matrixItemsTemplate",
        ITEM_PHOTO_TEMPLATE: "itemPhotoTemplate",
        REQUEST_ITEMS_INDICATOR_TEMPLATE: "requestMatrixItemIndicatorTemplate",
        DISPLAY_ITEMS_COUNTER_INDICATOR_TEMPLATE: "displayItemCounterIndicatorTemplate",
        ITEM_ADDED_INDICATOR_TEMPLATE: "itemsAddedIndicatorTemplate",
        IMAGE_NAVIGATION_LINK_TEMPLATE: "aImageNavitionLinkTemplate",
        MULTIPLE_NAVIGATION_WRAPPER: "multipleImagesNavigationWrapper"
    };

    var constantAttributes = {
        ATTR_COUNTER: "data-counter",
        ATTR_ITEMCODE: "data-itemCode",
        ATTR_PAGENUMBER: "data-pageNumber",
        ATTR_ITEMCOUNTER: "data-itemCounter",
        ATTR_PRICE_FORMATTED: "data-priceformatted",
        ATTR_ZOOM_OPTION: "data-ZoomOption",
        ATTR_HREF: "href",
        ATTR_LEFT: "left",
        ATTR_TOP: "top",
        ID: "id",
        SLOW: "slow",
        SHOPPING_CART: "shoppingcart"
    };

    var constantImages = {
        IN_STOCK: "<img  src= 'images/instock.png'/>",
        OUT_OF_STOCK: "<img src= 'images/outofstock.png'/>"
    }

    var stringResource = {};

    var matrixItemsCounter = {
        ITEMS_ON_DISPLAY: pluginConstants.ZERO_VALUE,
        ITEMS_TOTAL: pluginConstants.ZERO_VALUE,
        PAGE_SIZE: pluginConstants.ZERO_VALUE,
    }

    var serviceMethods = {
        GET_MATRIX_GROUP_ITEMS: "GetMatrixGroupItems",
        ADD_ITEM_TO_CART: "AddItemToCart"
    };

    var constantsUrl = {
        SHOPPING_CART : "shoppingcart.aspx"
    }

    var defaults = {
        pluginTemplate: 'components/matrix-listing/skin/jquery.matrix.listing.tmpl.html',
        basePluginTemplate: 'components/matrix-listing/skin/jquery.matrix.listing.tmpl.html', //if pluginTemplate is overridden but does not exist use the base which is the original path
        itemCode : pluginConstants.EMPTY_VALUE,
        pageSize: pluginConstants.ZERO_VALUE,
        imageSize: pluginConstants.EMPTY_VALUE,
        hideUnitMeasure: false,
        hideOutOfStock : false,
        useImagesMultiNav: false,
        useRolloverMultiNav: false,
        addToCartAction: pluginConstants.EMPTY_VALUE,
        addToCartText: pluginConstants.EMPTY_VALUE,
        addToWishListText: pluginConstants.EMPTY_VALUE,
        addToCartAction: pluginConstants.EMPTY_VALUE,
        quantityText : pluginConstants.EMPTY_VALUE,
        unitMeasureText: pluginConstants.EMPTY_VALUE,
        callToOrderText: pluginConstants.EMPTY_VALUE,
        messages: {
            ADDING_ITEM: pluginConstants.EMPTY_VALUE,
            INVALID_QUANTITY_FORMAT: pluginConstants.EMPTY_VALUE,
            INVALID_QUANTITY_ZERO: pluginConstants.EMPTY_VALUE,
            NO_ITEM_SELECTED: pluginConstants.EMPTY_VALUE,
            UNDEFINED_UNIT_MEASURE_CODE: pluginConstants.EMPTY_VALUE,
            ITEMS_ADDED_INDICATOR: "Item added successfully",
            ITEMS_LOADING_INDICATOR: "Loading Matrix Items..."
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

    $.fn.MatrixListing = {

        setup: function (optionalConfig) {

            thisMatrixListingPlugin = this;
            
            if (optionalConfig) setConfig($.extend(defaults, optionalConfig));

            var callback = function () {
                thisMatrixListingPlugin.initialize();
                thisMatrixListingPlugin.attachEvents();
            };

            thisMatrixListingPlugin.downloadPluginSkin(callback);

        },

        downloadPluginSkin: function (callback) {

            var config = getConfig();

            $.get(config.pluginTemplate, function (data, textStatus, XMLHttpRequest) {
                $(pluginConstants.BODY).append(data);
                callback();
            });

        },

        initialize: function () {

            var callback = function (response) {
                thisMatrixListingPlugin.renderMatrixItems(response, true);
            }

            thisMatrixListingPlugin.getMatrixItems(callback, 1, 0);

        },

        attachEvents: function () {

            $(thisMatrixListingPlugin.selectorChecker(constantIDs.VIEW_MORE_LINK)).click(function () {

                thisMatrixListingPlugin.hideMessageTips();
              
                var callback = function (response) {
                    thisMatrixListingPlugin.renderMatrixItems(response, false);
                }

                var attrPageNumber = $(this).attr(constantAttributes.ATTR_PAGENUMBER);

                attrPageNumber = parseInt(attrPageNumber);
                var pageNumber = isNaN(attrPageNumber) ? 1 : attrPageNumber;

                thisMatrixListingPlugin.getMatrixItems(callback, pageNumber, 0);

            });

            $(thisMatrixListingPlugin.selectorChecker(constantIDs.PAGE_SELECTOR)).change(function () {

                thisMatrixListingPlugin.hideMessageTips();

                var selected = $(this).val();

                if (selected > matrixItemsCounter.ITEMS_TOTAL && matrixItemsCounter.ITEMS_TOTAL != 0) {
                    selected = matrixItemsCounter.ITEMS_TOTAL;
                }
           
                matrixItemsCounter.ITEMS_ON_DISPLAY = selected;

                var callback = function (response) {
                    thisMatrixListingPlugin.renderMatrixItems(response, true);
                }

                thisMatrixListingPlugin.getMatrixItems(callback, 1, selected);
         
                $(thisMatrixListingPlugin.selectorChecker(constantIDs.VIEW_MORE_LINK)).attr(constantAttributes.ATTR_PAGENUMBER, 2);

            });

        },

        bindAddToCartEvent: function () {

            $(pluginConstants.DOT_VALUE + constantIDs.BUTTON_ADD_TO_CART).unbind("click").click(function () {
      
                var $this = $(this);
                var itemCounter = parseInt($this.attr(constantAttributes.ATTR_ITEMCOUNTER));
                var itemCode = $.trim($this.attr(constantAttributes.ATTR_ITEMCODE));

                thisMatrixListingPlugin.addItemToCart(itemCounter, itemCode);

            });


            $(pluginConstants.DOT_VALUE + constantClassnames.MATRIX_ITEM_UNITMEASURE_SELECTOR).unbind("change").change(function () {
                var $this = $(this);
                var itemCode = $this.attr(constantAttributes.ATTR_ITEMCODE);
                itemCode = $.trim(itemCode);
                
                var price = $('option:selected', $this).attr(constantAttributes.ATTR_PRICE_FORMATTED);
                price = $.trim(price);
                $(thisMatrixListingPlugin.selectorChecker(constantIDs.DIV_MATRIX_ITEM_PRICE + itemCode)).html(price);

            });
        },

        getMatrixItems: function (callback, page, size) {

            var config = getConfig();
            var pageSize = (size == 0) ? parseInt(config.pageSize) : size;
           
            matrixItemsCounter.PAGE_SIZE = isNaN(pageSize) ? pluginConstants.ZERO_VALUE : pageSize;
            matrixItemsCounter.ITEMS_ON_DISPLAY = matrixItemsCounter.PAGE_SIZE;

            var successFunction = function (response) {
                callback(response.d);
            };

            var errorFunction = function (error) {
                console.log(error);
            };

            var data = new Object();
            data.itemCode = config.itemCode;
            data.pageSize = matrixItemsCounter.PAGE_SIZE;
            data.pageNumber = page;
            data.imageSize = config.imageSize;

            $(thisMatrixListingPlugin.selectorChecker(constantIDs.REQUEST_INDICATOR)).html(thisMatrixListingPlugin.parseJqueryTemplate(constantsTemplateID.REQUEST_ITEMS_INDICATOR_TEMPLATE, { message: config.messages.ITEMS_LOADING_INDICATOR })).slideDown(constantAttributes.SLOW);
            thisMatrixListingPlugin.ajaxSecureRequest(serviceMethods.GET_MATRIX_GROUP_ITEMS, data, successFunction, errorFunction);
            
            data = null;

        },

        renderMatrixItems: function (data, onInit) {

            var config = getConfig();

            var jsonText = $.parseJSON(data);
            var listing = [];

            var counter = pluginConstants.ZERO_VALUE;

            $.each(jsonText, function (key, value) {

                if (config.hideOutOfStock && value.ItemStock == pluginConstants.ZERO_VALUE) {
                    return true;
                }

                var img = thisMatrixListingPlugin.parseImagesString(value.Images, value.ItemCounter, value.itemCode, config.useRolloverMultiNav);
                var largeImage = img.default.split(pluginConstants.FORWARD_SLASH_SEPARATOR);
                largeImage = largeImage[largeImage.length - 1];

                listing.push({
                    MatrixItemCounter: value.ItemCounter,
                    MatrixItemCode: value.ItemCode,
                    MatrixItemName: value.ItemName,
                    MatrixItemDescription: (value.ItemHTMLDescription == pluginConstants.EMPTY_VALUE) ? value.ItemDescription : value.ItemHTMLDescription,
                    MatrixItemPrice: value.ItemPrice,
                    htmlMatrixItemStock: thisMatrixListingPlugin.renderStockImage(false, value.ItemStock),
                    htmlUnitOfMeasurement: thisMatrixListingPlugin.renderUnitMeasureSelector(value.ItemCode, value.ItemUnitMeasure),
                    htmlMultipleImagesLink: img.multiple,
                    htmlMatrixItemDefaultPhoto: thisMatrixListingPlugin.parseJqueryTemplate(constantsTemplateID.ITEM_PHOTO_TEMPLATE, {itemCounter: value.ItemCounter, 
                        itemCode: value.ItemCode, 
                        itemName: value.ItemName,
                        largeImage: largeImage,
                        zoomOption: value.ZoomOption,
                        source : img.default}),
                    htmlAddToCartReturnMessagePlaceHolder: '<div class="clear-both" id="' + value.ItemCounter + '-' + value.ItemCode + '-added-place-holder"></div>',
                    stringResource : {
                        unitMeasure: config.unitMeasureText,
                        quantity: config.quantityText,
                        addToWishList: config.addToWishList,
                        addToCart: config.addToCartText,
                        callToOrder: config.callToOrderText
                    },
                    IsShowButton: value.ShowBuyButton,
                    HidePriceUntilCart: value.HidePriceUntilCart,
                    IsCallToOrder: value.IsCallToOrder
                });

                thisMatrixListingPlugin.getLoyaltyPoints(value.ItemCounter, value.ItemPrice, value.PurchaseMultiplier, value.IsDontEarnPoints);

                counter = value.TotalNumberOfItems;
                if (onInit == false) {
                    matrixItemsCounter.ITEMS_ON_DISPLAY++;
                }

                thisMatrixListingQuanitityRegEx = value.QuantityRegExp;

            });
            
            if (onInit) {

                $(thisMatrixListingPlugin.selectorChecker(constantIDs.REQUEST_INDICATOR)).fadeOut(constantAttributes.SLOW, function () {

                    $(thisMatrixListingPlugin.selectorChecker(constantIDs.MATRIX_ITEMS_WRAPPER)).html($(thisMatrixListingPlugin.selectorChecker(constantsTemplateID.MATRIX_ITEMS_TEMPLATE)).tmpl(listing)).fadeIn("slow", function () {
                        thisMatrixListingPlugin.initImageZoom();
                        thisMatrixListingPlugin.bindAddToCartEvent();
                    });

                });

            }else{

                $(thisMatrixListingPlugin.selectorChecker(constantIDs.REQUEST_INDICATOR)).slideUp(constantAttributes.SLOW, function () {
                    $(thisMatrixListingPlugin.selectorChecker(constantIDs.MATRIX_ITEMS_WRAPPER)).append($(thisMatrixListingPlugin.selectorChecker(constantsTemplateID.MATRIX_ITEMS_TEMPLATE)).tmpl(listing)).fadeIn("slow", function () {
                        thisMatrixListingPlugin.initImageZoom();
                        thisMatrixListingPlugin.bindAddToCartEvent();
                    });
                });
               
                var currentPageNumber = $(thisMatrixListingPlugin.selectorChecker(constantIDs.VIEW_MORE_LINK)).attr(constantAttributes.ATTR_PAGENUMBER);

                currentPageNumber = $.trim(currentPageNumber);
                currentPageNumber = parseInt(currentPageNumber);
                
                $(thisMatrixListingPlugin.selectorChecker(constantIDs.VIEW_MORE_LINK)).attr(constantAttributes.ATTR_PAGENUMBER, (isNaN(currentPageNumber) ? 1 : ++currentPageNumber));

            }

            matrixItemsCounter.ITEMS_TOTAL = counter;

            if (matrixItemsCounter.ITEMS_ON_DISPLAY > counter) {
                matrixItemsCounter.ITEMS_ON_DISPLAY = counter;
            }

            if (counter > 0) {

                var indicator = thisMatrixListingPlugin.parseJqueryTemplate(constantsTemplateID.DISPLAY_ITEMS_COUNTER_INDICATOR_TEMPLATE, { itemsOnDisplay: matrixItemsCounter.ITEMS_ON_DISPLAY, totalItems: counter })
 
                $(thisMatrixListingPlugin.selectorChecker(constantIDs.DIV_MATRIX_ITEMS_PLACEHOLDER)).html(indicator);
                $(thisMatrixListingPlugin.selectorChecker(constantIDs.PAGE_SIZE_ALL)).val(counter);
            }

            if (counter <= matrixItemsCounter.PAGE_SIZE) {
                $(thisMatrixListingPlugin.selectorChecker(constantIDs.DIV_MATRIX_ITEMS_BOTTOM_CONTROLS)).fadeOut(constantAttributes.SLOW);
            }
        },  
        
        parseImagesString: function(source, itemCounter, itemCode, rollover){
            var config = getConfig();
           
            var images = source.split(pluginConstants.DOUBLE_COLON_SEPARATOR);
            var links = pluginConstants.EMPTY_VALUE;
            var imagesource = pluginConstants.EMPTY_VALUE;
            
            if (images.length > 1 && images[1] != pluginConstants.EMPTY_VALUE){
                    
                for (var j = 1; j <= (images.length - 1) ; j++) {

                    if (images[j - 1] != pluginConstants.EMPTY_VALUE) {

                        var source = images[j - 1].split(pluginConstants.SPACE_SEPARATOR)[0];

                        var selected = pluginConstants.EMPTY_VALUE;

                        var imgSelected = images[j - 1].split(pluginConstants.SPACE_SEPARATOR)[1];

                        if (typeof (imgSelected) != pluginConstants.UNDEFINED && imgSelected != pluginConstants.EMPTY_VALUE) {
                            if (imgSelected.toLowerCase() == pluginConstants.TRUE_STRING_VALUE) {
                                imgsource = source;
                                selected = constantClassnames.MULTIPLE_IMAGES_NAV_SELECTED;
                            }
                        }

                        var data = new Object();

                        data.counter = j;
                        data.itemCounter = itemCounter;
                        data.itemCode = itemCode;
                        data.source = source;
                        data.selected = selected; 
                        data.IsChangeImageSourceOnRollOver_ImageLink = config.useImagesMultiNav && config.useRolloverMultiNav;
                        data.IsChangeImageSourceOnClick_ImageLink = config.useImagesMultiNav && !config.useRolloverMultiNav;
                        data.IsChangeImageSourceOnRollOver_NumberLink = !config.useImagesMultiNav && config.useRolloverMultiNav;
                        data.IsChangeImageSourceOnClick_NumberLink = !config.useImagesMultiNav && !config.useRolloverMultiNav;


                        links += thisMatrixListingPlugin.parseJqueryTemplate(constantsTemplateID.IMAGE_NAVIGATION_LINK_TEMPLATE, data);
                        data = null;

                    }
                }

            } else {
                imgsource = images[0].split(" ")[0];
            }

            var data = new Object();
            data.default = imgsource;
            data.multiple = thisMatrixListingPlugin.parseJqueryTemplate(constantsTemplateID.MULTIPLE_NAVIGATION_WRAPPER, { links: links });

            return data;
        },

        renderStockImage : function(showActualInventory, stock){

            if (showActualInventory) {
                return stock;
            }

            return (stock > 0) ? constantImages.IN_STOCK : constantImages.OUT_OF_STOCK;
        },

        renderUnitMeasureSelector: function (itemCode, unitMeasures) {
            var config = getConfig();
            if (config.hideUnitMeasure) return pluginConstants.SIGLE_SPACE;
           
            var data = new Object();
            data.itemCode = itemCode;
            data.unitMeasures = [];
            data.unitMeasureText = config.unitMeasureText;
            
            $.each(unitMeasures, function (index, o) {
                var price = (isNaN(o.promotionalPrice) || o.promotionalPrice == 0) ? o.priceFormatted : o.promotionalPriceFormatted;
                data.unitMeasures.push({ code: o.code, description: o.description, priceFormatted: price, promotionalPriceFormatted: o.promotionalPriceFormatted });
            });

            return thisMatrixListingPlugin.parseJqueryTemplate(constantsTemplateID.UNIT_MEASURE_TEMPLATE, data);
        },

        addItemToCart: function (itemCounter, itemCode) {

            var config = getConfig();
            var input = thisMatrixListingPlugin.verifyUserInput(itemCode);

            if (input.status == false) {
                return false;
            }

            var successFunction = function (response) {

                var message = $.trim(response.d);
                if(message !=pluginConstants.EMPTY_VALUE){

                    message = message.replace("\\n", "\n");
                    thisMatrixListingPlugin.displayBubbleTips($(thisMatrixListingPlugin.selectorChecker("qty-" + itemCode)), message);
                    $(thisMatrixListingPlugin.selectorChecker("qty-" + itemCode)).addClass(constantClassnames.INVALID_QUANTITY);

                    return false;
                }


                if (config.addToCartAction != pluginConstants.EMPTY_VALUE && config.addToCartAction.toLowerCase() == pluginConstants.STAY) {

                    var message = thisMatrixListingPlugin.parseJqueryTemplate(constantsTemplateID.ITEM_ADDED_INDICATOR_TEMPLATE, { message: config.messages.ITEMS_ADDED_INDICATOR });
                    $(thisMatrixListingPlugin.selectorChecker(itemCounter + "-" + itemCode + "-added-place-holder")).html(message).delay();

                    return false;
                }

                window.location = constantsUrl.SHOPPING_CART;

            };

            var errorFunction = function (error) {
                console.log(error);
            }

            var data = new Object();
            data.itemCounter = itemCounter;
            data.itemCode = itemCode;
            data.quantity = input.quantity;
            data.unitMeasureCode = input.unitMeasure;
            data.cartTypeIndex = 0;

            thisMatrixListingPlugin.ajaxSecureRequest(serviceMethods.ADD_ITEM_TO_CART, data, successFunction, errorFunction);

            input = null;
            data = null;
        },

        verifyUserInput: function (itemCode) {

            var $quantityInputBox = $(thisMatrixListingPlugin.selectorChecker("qty-" + itemCode));
            var $unitMeasureSelector = $(thisMatrixListingPlugin.selectorChecker("uom-" + itemCode));

            var quantity = $quantityInputBox.val();
            quantity = $.trim(quantity);

            var unitMeasure = $unitMeasureSelector.val();
            unitMeasure = (typeof (unitMeasure) != pluginConstants.UNDEFINED) ? $.trim(unitMeasure) : pluginConstants.EMPTY_VALUE;

            if (thisMatrixListingQuanitityRegEx != pluginConstants.EMPTY_VALUE && !quantity.match(thisMatrixListingQuanitityRegEx)) {

                thisMatrixListingPlugin.displayBubbleTips($quantityInputBox, config.messages.INVALID_QUANTITY_FORMAT);
                $quantityInputBox.addClass(constantClassnames.INVALID_QUANTITY);
                return {status : false};
            }

            if (quantity <= pluginConstants.ZERO_VALUE || quantity == pluginConstants.EMPTY_VALUE) {

                thisMatrixListingPlugin.displayBubbleTips($quantityInputBox, config.messages.INVALID_QUANTITY_ZERO);
                $quantityInputBox.addClass(constantClassnames.INVALID_QUANTITY);

                return { status: false };
            }

            thisMatrixListingPlugin.hideMessageTips();
            $quantityInputBox.removeClass(constantClassnames.INVALID_QUANTITY);

            return { status: true, quantity: quantity, unitMeasure: unitMeasure };
        },

        displayBubbleTips: function(o, message){

            var thisLeft = o.offset().left;
            var thisTop =  o.offset().top;
                
            $(thisMatrixListingPlugin.selectorChecker(constantIDs.DIV_MESSAGE)).html(message);
            $(thisMatrixListingPlugin.selectorChecker(constantIDs.DIV_MESSAGE_TIPS)).css(constantAttributes.ATTR_TOP, thisTop - 54).css(constantAttributes.ATTR_LEFT, thisLeft - 17).fadeIn(constantAttributes.SLOW);

        },

        initImageZoom: function () {
            $(pluginConstants.DOT_VALUE + constantClassnames.CLOUD_ZOOM).each(function () {
                var $this = $(this);
                thisMatrixListingPlugin.verifyPhoto($this.attr(constantAttributes.ATTR_HREF), $this.attr(constantAttributes.ID), false, $this.attr(constantAttributes.ATTR_ZOOM_OPTION), pluginConstants.EMPTY_VALUE);
            });
        },

        verifyPhoto: function(src, id, onClick, zoomOption, itemPhoto){
            
            var img = new Image();

            img.onload = function () {
                thisMatrixListingPlugin.setZoomOptions(id, zoomOption);
            };
            img.onerror = function () {
                if (onClick) $("#" + id + "-wrapper").html(itemPhoto);
                thisMatrixListingPlugin.setZoomOptions(id, "popup");
            };

            img.src = src; // fires off loading of image
        },

       setZoomOptions : function(id, zoomOption){
    
            if (zoomOption == null)  return;

            var link = $("#" + id);
         
            switch (zoomOption.toLowerCase()) {
                case 'lens zoom': //lens zoom

                    link.attr("rel", "adjustX: 10, adjustY:-4" + thisMatrixListingPlugin.getImageZoomLensSize());

                    $('.mousetrap').remove();
                    $(".cloud-zoom").CloudZoom();

                    break;
                case 'lens blur': //lens blur
             
                    link.attr("rel", "tint: '#FF9933',tintOpacity:0.5 ,smoothMove:5, adjustY:-4, adjustX:10" + thisMatrixListingPlugin.getImageZoomLensSize());

                    $('.mousetrap').remove();
                    $(".cloud-zoom").CloudZoom();


                    break;
                case 'inner zoom': //inner zoomOption

                    link.attr("rel", "position: 'inside' , showTitle: false, adjustX:-4, adjustY:-4");

                    $('.mousetrap').remove();
                    $(".cloud-zoom").CloudZoom();

                    break;
                case 'blur focus': //blur focus

                    link.attr("rel", "softFocus: true, smoothMove:2, adjustX: 10, adjustY:-4" + thisMatrixListingPlugin.getImageZoomLensSize());
           
                    $('.mousetrap').remove();
                    $(".cloud-zoom").CloudZoom();

                    break;
                case 'zoom out': //zoom out
                
                    link.fancybox({
                        'overlayShow': false,
                        'transitionIn': 'elastic',
                        'transitionOut': 'elastic'
                    });

                    break;
                case 'popup': //popout

                    link.fancybox({
                        'titlePosition': 'inside',
                        'transitionIn': 'none',
                        'transitionOut': 'fade'
                    });

                    break;
                default:

                    link.fancybox({
                        'titlePosition': 'inside',
                        'transitionIn': 'none',
                        'transitionOut': 'fade'
                    });

                    break;
            }
        },

        getImageZoomLensSize: function(){

            var imageZoomLensWidth = ise.Configuration.getConfigValue("ImageZoomLensWidth");
            var imageZoomLensHeight = ise.Configuration.getConfigValue("ImageZoomLensHeight");
            var lensZoomSize = '';

            if (imageZoomLensHeight > 0) {
                lensZoomSize += ",zoomHeight:" + imageZoomLensHeight;
            }

            if (imageZoomLensWidth > 0) {
                lensZoomSize += ",zoomWidth:" + imageZoomLensWidth;
            }

            return lensZoomSize;

        },

        getLoyaltyPoints: function (itemCounter, price, purchaseMultiplier, isDontEarnPoints) {

            if (isDontEarnPoints == false) {
                price = price.replace(/^\D+/g, '');
                var points = price * purchaseMultiplier;
                if (points > 0) {
                    window.setTimeout(function () {
                        var pnlLoyaltyPoints = $("#pnlLoyaltyPoints_" + itemCounter);

                        $(pnlLoyaltyPoints).find(".points").text(points);
                        $(pnlLoyaltyPoints).show();
                    }, 1000);
                }
            }
        },

        hideMessageTips: function(){
            $(thisMatrixListingPlugin.selectorChecker(constantIDs.DIV_MESSAGE_TIPS)).fadeOut(constantAttributes.SLOW);
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
        }
    };

    //jqueryBasePlugin is located inside core.js
    $.extend($.fn.MatrixListing, new jqueryBasePlugin());

    function setConfig(value) {
        config = value;
    }

    function getConfig() {
        return config;
    }

})(jQuery);

function removeInvalidQuantityMessage(id) {

    $("#ise-message-tips").fadeOut("slow");
    $("#qty-" + id).removeClass("invalid-quantity");
}
