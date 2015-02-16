/// <reference path="../core.js" />

(function ($) {

    var map;
    var config = {};
    var marker;
    var foundWarehouses;
    var allStoresAreLoaded = false;
    var markerObject = null;

    var thisObject;

    var geocoder = null;

    var isGoogleQueError = false;

    var pluginConstants = {
        EMPTY_VALUE: '',
        DOT_VALUE: '.',
        ZERO_VALUE: 0,
        DEFAULT_MAP_ZOOM_ON_ADD_MARKER: 15,
        DEFAULT_MAP_ZOOM_ON_MARKER_CLICK:17
    }

    var isSorted = false;

    var Address = function (address, city, state, postal, country) {
        this.Address = address;
        this.City = city;
        this.State = state;
        this.Postal = postal;
        this.Country = country;

        this.getCompleteAddress = function () {
            return (this.Address + ', ' + this.City + ', ' + this.State + ', ' + this.Postal + ' ' + this.Country);
        }
    }

    //var infoWindow = new google.maps.InfoWindow({});
    var directionsService = null; //new google.maps.DirectionsService();
    var directionsDisplay = null; //new google.maps.DirectionsRenderer();

    var defaults = {
        zoom: 5,
        zoomControlOptions: { style: google.maps.ZoomControlStyle.SMALL },
        center: new google.maps.LatLng(39.322, -100.580),
        mapTypeId: google.maps.MapTypeId.ROADMAP,
        mapTypeControlOptions: {
            style: google.maps.MapTypeControlStyle.DROPDOWN_MENU,
            poistion: google.maps.ControlPosition.TOP_RIGHT,
            mapTypeIds: [google.maps.MapTypeId.ROADMAP,
              google.maps.MapTypeId.TERRAIN,
              google.maps.MapTypeId.HYBRID,
              google.maps.MapTypeId.SATELLITE]
        },
        navigationControl: true,
        navigationControlOptions: { style: google.maps.NavigationControlStyle.ZOOM_PAN },
        pluginTemplate: 'components/instore_pickup/skin/template.js',
        pluginTemplateCss: 'components/instore_pickup/skin/index.css',
        basePluginTemplate: 'components/instore_pickup/skin/template.js', //if pluginTemplate is overridden but does not exist use the base which is the original path
        basePluginTemplateCss: 'components/instore_pickup/skin/index.css', //if pluginTemplateCss is overridden but does not exist use the base which is the original path
        mainStorePickupDialogTemplateId: 'storeDialogContainerTemplateId',
        mainStorePickupDialogId: 'storeDialogContainerId',
        mainStorePickupMapId: 'storePickupMapContainer',
        mainStorePickupMenuContainerTemplateId: 'storeDialogMenuContainerTemplateId',
        mainStorePickupWhereMenuTemplateId: 'storeMapMenuContainerTemplateId',
        mainStorePickupSideMenuContainerId: 'storePickupSideMenuId',
        mainStorePickupMapBodyWrapperId: 'storePickupMapContainerWrapperId',
        mainStorePickupMenuWhereWrapperId: 'storeWhereMenuWrapperId',
        mainStorePickupMenuHowWrapperId: 'storeHowMenuWrapperId',
        mainStorePickupListContainerTemplateId: 'storeListContainerTemplateId',
        mainStorePickupListDataTemplateId: 'storeListDataTemplate',
        mainStorePickupListEmptrySearchTemplateId: 'storeListEmptySearchTemplateId',
        mainStorePickupListContainerId: 'storeListContainerId',
        mainStorePickupListScrollerContainerId: 'storeListContainerScrollerId',
        mainStorePickupListTableContainerId: 'storeListTableContainerId',
        mainStorePickupMenuWhereButtonId: 'btnInstoreSearchId',
        mainStorePickupMenuHowButtonId: 'btnHowSearchId',
        mainStorePickupMenuWhereCountryDropDownId: "ddlInstoreCountryId",
        mainStorePickupMenuWhereStateTextBoxId: "searchWhereStateId",
        mainStorePickupMenuWhereCityTextBoxId: "searchWhereCityId",
        mainStorePickupMenuWherePostalTextBoxId: "searchWherePostalId",
        mainStorePickupMenuHowAddressTextBoxId: "txtSearchHowAddressId",
        mainStorePickupMapContainerTemplateId: "storeMapContainerTemplateId",
        mainStorePickupMapContainerId: "storeMapContainerId",
        mainStorePickupMapId: "storeMapId",
        mainStorePickupMapLinkCloseId: "lnkMapCloseId",
        mainStorePickupMapDirectionContainerId: "storeMapDirectionContainerId",
        mainStorePickupInfoHeaderContainerId: "storeInfoHeaderContainerId",
        mainStorePickupInfoHeaderContainerTemplateId: "storeInfoHeaderContainerTemplateId",
        cartItem: {
            itemCounter: 0,
            itemCode: '',
            itemType: '',
            unitMeassureCode: '',
            quantity: 0,
            kitComposition: '',
        },
        defaultShippingMethod: '',
        shippingMethodSelectedStoreContainerId: 'selectedInStoreInfoContainer_',
        hiddenInStoreSelectedWarehouseElementId: 'hiddenInStoreSelectedWarehouseElementId',
        selectedWarehouseShippingMethodTemplateId : 'selectedWarehouseShippingMethodTemplate',
        messages: {
            MESSAGE_MENU_SEARCH_WHERE_VALIDATION_MESSAGE: "Unable to search, please input a valid Postal Code or City, State, Country.",
            MESSAGE_STORE_LIST_HEADER_TEXT: 'IN-STORE PICKUP',
            MESSAGE_STORE_LIST_SELECT_BUTTON_TEXT: 'select',
            MESSAGE_PICKUP_WHERE_TEXT: 'Enter your address like below to see where you can pick up the item.',
            MESSAGE_PICKUP_HOW_TEXT: 'Enter your address like below to see how you can pick up the item.',
            MESSAGE_SAMPLE_ADDRESS_INPUT_TEXT: 'Street address, City, State or Zip Code.',
            MESSAGE_MENU_HEADER_AVAILABILITY: 'CHECK AVAILABILITY',
            MESSAGE_MENU_HEADER_DIRECTION: 'CHECK DIRECTIONS',
            MESSAGE_MENU_ZIP_POSTAL_TEXT: 'Zip or Postal Code',
            MESSAGE_MENU_CITY_TEXT: 'City',
            MESSAGE_MENU_STATE_TEXT: 'State',
            MESSAGE_MENU_COUNTRY_TEXT: 'Country',
            MESSAGE_MENU_BUTTON_SEARCH_TEXT: 'Search',
            MESSAGE_MENU_COUNTRY_SELECT_TEXT: 'Select Country',
            MESSAGE_STORE_LIST_DIRECTION_LINK_TEXT: 'Map and Directions',
            MESSAGE_STORE_LIST_HEADER_STORE_TEXT: 'STORE INFORMATION',
            MESSAGE_STORE_LIST_HEADER_AVAILABILITY_TEXT: 'ITEM AVAILABILITY',
            MESSAGE_MAP_CLOSE_TEXT: 'Close',
            MESSAGE_MAP_GET_DIRECTION_BUTTON_TEXT: 'Get Directions',
            MESSAGE_MAP_DIRECTION_ERROR_MESSAGE: 'Unable to get direction. please check you inputs.',
            MESSAGE_STORE_INFO_TITLE_TEXT: 'Store Information',
            MESSAGE_STORE_HOUR_TITLE_TEXT: 'Hours',
            MESSAGE_MAP_DIRECTION_HEADER_TEXT: 'Directions',
            MESSAGE_MENU_SEARCH_NO_RECORD_FOUND_MESSAGE: 'There\'s no available Store in the area you selected.',
            MESSAGE_STORE_CLOSE_TEXT: 'Close',
            MESSAGE_STORE_HOLIDAY_HEADER_TEXT: 'Holidays',
            MESSAGE_STORE_LIST_HEADER_STOCK_TEXT: 'Stock',
            MESSAGE_ADD_TO_CART_BUTTON_TEXT: 'Add to Cart',
            MESSAGE_GOOGLE_QUE_ERROR: 'Google Map encounter ',
            MESSAGE_STORE_TELEPHONE_HEADER_TEXT: 'Telephone',
            MESSAGE_STORE_FAX_HEADER_TEXT : "Fax",
            MESSAGE_STORE_EMAIL_HEADER_TEXT :  "Email",
            MESSAGE_STORE_WEBSITE_HEADER_TEXT: "Website",
            MESSAGE_STORE_STOCK_QUANTITY_ON_HAND: 'Some of your item quantities were reduced, as they exceeded stock on hand.',
            MESSAGE_STORE_SELECT_CHANGE_WAREHOUSE: 'The warehouse you selected has less available stock( {0} ) than the quantity( {1} ) you want to order.',
            MESSAGE_STORE_SELECT_CHANGE_WAREHOUSE1: "If you choose to continue, the quantity will be adjusted based on the available stock.",
            MESSAGE_STORE_SELECT_CHANGE_WAREHOUSE2: "Click 'OK' if you want to continue."
        },
        selectButtonClass: '.store-select',
        addToCartButtonIdPrefix: 'btnAddToCart_',
        mapLinkClass: '.map-link',
        buttonWarehouseAttribute: 'data-storeid',
        distanceSpinnerPrefix: 'spinner-',
        distanceElementPrefix: 'distance-',
        distanceHeaderId: 'distanceHeaderId',
        storeItemtableRowsAttr: 'data-store-item-attr',
        storeItemStockAttr: 'data-stock',
        appConfig: {
            ShowActualStock: false,
            LimitCartToQuantityOnHand: false
        },
        showAddToCart: false,
        cartNumItemID: "cartNumItem"
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

    $.fn.InStorePickup = {

        setup: function (optionalConfig) {

            thisObject = this;

            if (optionalConfig) setConfig($.extend(defaults, optionalConfig));

            var config = getConfig();

            this.showLoader();

            this.downloadPluginSkin(function () {

                var param = new Object();
                param.itemCode = config.cartItem.itemCode;
                param.unitMeassureCode = config.cartItem.unitMeassureCode;

                thisObject.ajaxRequest("ActionService.asmx/GetStorePickUpInitialnfoToJSON", param, function (response) {

                    var dto = thisObject.ToJsonObject(response.d);
                    thisObject.loadDialogTemplate(dto)
                              .loadMenuContainerTemplate(dto)
                              .clear()
                              .searchInventoryWarehouse(true, dto.Address, function () {

                                    thisObject.hideLoader();
                                    thisObject.attachEvents()
                                              .showInModal()
                                              .autoScrollTopAfterSearch();

                              });

                });

            });
          
        },

        searchInventoryWarehouse: function (firstLoad, address, callback) {

            var config = getConfig();

            var param = null
            var addressObject = null;

            if (firstLoad) {

                addressObject = address;
                param = this.getFirstLoadParamObjectForWhereSearch();

            } else {

                param = this.getParamObjectForWhereSearch();
                addressObject = new Object();
                addressObject.Address = '';
                addressObject.PostalCode = param.postalCode;
                addressObject.City = param.city;
                addressObject.State = param.state;
                addressObject.Country = param.country;

            }

            this.getStoreInventoryWarehouseList(param, function (response) {

                var formattedAddress = thisObject.getFormattedAddress(addressObject);

                var dto = thisObject.ToJsonObject(response.d);

                thisObject.clearFoundWarehouse()
                          .loadStoreListHeaderContainerTemplate(dto)
                          .loadStoreListContainerTemplate(dto)
                          .startLoadDistance(dto, formattedAddress)
                          .appendToFoundWarehouse(dto);

                if (typeof callback != 'undefined') callback();

            });

        },

        startLoadDistance: function(dto, clientInputAddress) {

            var config = getConfig();

            var loaderId = 0;

            var distanceLoader = function () {

                thisObject.addressGoogleSearch(clientInputAddress, function (results, status) {

                    if (status == google.maps.GeocoderStatus.OK) {

                        clearInterval(loaderId);

                        var lat1 = results[0].geometry.location.lat();
                        var lon1 = results[0].geometry.location.lng();

                        $.each(dto, function (index, warehouse) {

                            warehouse.displayDistance = function (distance, interval) {
                                thisObject.getElemBySelChecker(config.distanceSpinnerPrefix + this.RowNumber).hide();
                                thisObject.getElemBySelChecker(config.distanceElementPrefix + this.RowNumber).fadeIn()
                                                                                                             .text(distance);
                                clearInterval(interval);
                            };

                            warehouse.computeDistance = function () {

                                var thisWareHouse = this;

                                var timeOutid = setInterval(function () {

                                    thisObject.addressGoogleSearch(thisObject.getFormattedAddress(thisWareHouse.Address), function (result, status) {

                                        if (status == google.maps.GeocoderStatus.ZERO_RESULTS || result.length  == 0) {

                                            thisWareHouse.displayDistance(0, timeOutid);

                                        } else if (status == google.maps.GeocoderStatus.OK) {

                                            var lat2 = result[0].geometry.location.lat();
                                            var lon2 = result[0].geometry.location.lng();

                                            var distanceValue = thisObject.calculateDistance(lat1, lon1, lat2, lon2).toFixed(2);
                                            thisWareHouse.displayDistance(distanceValue, timeOutid);

                                        }

                                    });

                                }, 1000);
                            };

                            warehouse.computeDistance();

                        });

                    }

                })

            }

            loaderId = setInterval(distanceLoader, 200);

            return this;
        },

        getStoreInventoryWarehouseList: function (param, callback) {
            this.ajaxRequest("ActionService.asmx/GetStorePickupInventoryWarehouseListToJSON", param, callback);
        },

        getFirstLoadParamObjectForWhereSearch: function() {
            var config = getConfig();

            var param = new Object();
            param.isFirstLoad = true;
            param.itemCode = config.cartItem.itemCode;
            param.kitComposition = (config.cartItem.kitComposition != null && config.cartItem.kitComposition != '')? config.cartItem.kitComposition : new Array(),
            param.unitMeasureCode = config.cartItem.unitMeassureCode,
            param.postalCode = pluginConstants.EMPTY_VALUE;
            param.city = pluginConstants.EMPTY_VALUE;
            param.state = pluginConstants.EMPTY_VALUE;
            param.country = pluginConstants.EMPTY_VALUE;
            param.nextRecord = 0;
            return param;
        },

        getParamObjectForWhereSearch: function () {
            var config = getConfig();

            var postalCode = '', city = '', state = '', country = '';

            var param = new Object();
            param.isFirstLoad = false;

            var element = this.getElemBySelChecker(config.mainStorePickupMenuWherePostalTextBoxId);
            if (element.length > 0 && element.val() != pluginConstants.EMPTY_VALUE) {
                postalCode = element.val();
            }

            element = this.getElemBySelChecker(config.mainStorePickupMenuWhereCityTextBoxId);
            if (element.length > 0 && element.val() != pluginConstants.EMPTY_VALUE) {
                city = element.val();
            }

            element = this.getElemBySelChecker(config.mainStorePickupMenuWhereStateTextBoxId);
            if (element.length > 0 && element.val() != pluginConstants.EMPTY_VALUE) {
                state = element.val();
            }

            element = this.getElemBySelChecker(config.mainStorePickupMenuWhereCountryDropDownId);
            if (element.length > 0 && element[0].selectedIndex > 0) {
                country = element.val();
            }

            param.itemCode = config.cartItem.itemCode;
            param.unitMeasureCode = config.cartItem.unitMeassureCode;
            param.kitComposition = (config.cartItem.kitComposition != '' && config.cartItem.kitComposition != null) ? config.cartItem.kitComposition : new Array(),
            param.postalCode = $.trim(postalCode);
            param.city = $.trim(city);
            param.state = $.trim(state);
            param.country = $.trim(country);
            param.nextRecord = 0;

            return param;
        },

        appendToFoundWarehouse: function (newWarehouses) {
            if (foundWarehouses != null) {
                foundWarehouses = $.merge(foundWarehouses, newWarehouses);
            }
            else {
                foundWarehouses = newWarehouses;
            }
            return foundWarehouses;
        },

        getFoundWareHouses: function() {
            return foundWarehouses;
        },

        clearFoundWarehouse: function () {
            foundWarehouses = null;
            return this;
        },

        downloadPluginSkin: function (callback) {

            var config = getConfig();
            this.downloadPlugin(config.pluginTemplate, function () {

                thisObject.downloadCss(config.pluginTemplateCss, function () {

                    if (typeof callback === 'function') callback();

                });

            }, function (message) {

                if (message.toLowerCase() == "not found") {

                    thisObject.downloadPlugin(config.basePluginTemplate, function () {
                        
                        thisObject.downloadCss(config.basePluginTemplateCss, function () {

                            if (typeof callback === 'function') callback();

                        });

                    }, function (message) {

                        alert('unable to load the plugin please check the source. \n\n Error message: ' + message);
                        if (typeof callback === 'function') callback();

                    });

                }

            });

        },
       
        attachEvents: function () {
            var config = getConfig();

            var buttonSearchWhereElement = this.getElemBySelChecker(config.mainStorePickupMenuWhereButtonId);
            //ensure that the search button exist
            if (buttonSearchWhereElement.length > 0)
            {
                buttonSearchWhereElement
                                .unbind('click')
                                .bind('click', this.CreateJqueryDelegate(this.searchWhereClick, this));
            }

            var selectList = this.getElemBySelChecker(config.selectButtonClass);
            if (selectList.length > 0)
            {
                selectList.unbind('click')
                          .bind('click', this.CreateJqueryDelegate(this.selectWarehouseClick, this));
            }

            var mapLinks = this.getElemBySelChecker(config.mapLinkClass)
            if (mapLinks.length > 0) {
                mapLinks.unbind('click')
                        .bind('click', this.CreateJqueryDelegate(this.linkMapClick, this));
            }

            var div = this.getElemBySelChecker(config.mainStorePickupSideMenuContainerId)
            if (div.length > 0) {
                div.unbind("keypress").keypress(function (e) {
                    if (e.which == '13') {
                        e.preventDefault();
                        thisObject.searchWhereClick();
                    }
                })
            }
            
            var scrollerDiv = this.getElemBySelChecker(config.mainStorePickupListScrollerContainerId);
            if (scrollerDiv.length > 0) {
                scrollerDiv.unbind("scroll").scroll(function () {

                    if (thisObject.isAllStoresAreLoaded()) return;

                    var latestScroller = thisObject.getElemBySelChecker(config.mainStorePickupListScrollerContainerId);
                    var table = thisObject.getElemBySelChecker(config.mainStorePickupListTableContainerId);

                    if (latestScroller.scrollTop() < (table.height() - latestScroller.height())) return;

                    thisObject.fetchNextWareHouse(scrollerDiv.scrollTop());
                });
            }

            $("input[id^='" + config.addToCartButtonIdPrefix + "']").unbind('click')
                                                                    .bind("click", this.CreateJqueryDelegate(this.clickAddToCart, this));

            this.getElemBySelChecker(config.distanceHeaderId).unbind('click')
                                                             .bind("click", this.CreateJqueryDelegate(this.clickDistanceHeaderSort, this));

            return this;
        },

        clickDistanceHeaderSort: function () {

            var config = getConfig();

            var tr = this.getElemBySelChecker(config.mainStorePickupListTableContainerId)
                                                    .find("tr[" + config.storeItemtableRowsAttr + "]");

            this.getElemBySelChecker(config.distanceHeaderId).unbind('click');
            if (tr.length == 0) return;

            var newSortedTr = $(tr).sort(function (a, b) {

                                        var distanceA = parseFloat($(a).find("td span[id^=" + config.distanceElementPrefix + "]").text());
                                        var distanceB = parseFloat($(b).find("td span[id^=" + config.distanceElementPrefix + "]").text());
                                        
                                        return (!isSorted) ? (distanceA - distanceB) :
                                                             (distanceB - distanceA);

            });

            $(tr).remove();

            thisObject.getElemBySelChecker(config.mainStorePickupListTableContainerId).append($(newSortedTr));

            $(newSortedTr).hide()
                          .fadeIn( 'slow' ,function () {
                                thisObject.attachEvents();
                          });

            isSorted = !isSorted;
        },

        clickAddToCart: function(evt) {

            var config = getConfig();
            var self = this;
            if (!evt) return;

            var currentItem = config.cartItem;
            if (typeof currentItem !== 'object') return;

            var warehouseCode = $(evt.target).attr(config.buttonWarehouseAttribute);
            var warehouseStock = $(evt.target).attr(config.storeItemStockAttr);

            var param = new Object();
            param.counter = currentItem.itemCounter;
            param.quantity = currentItem.quantity;
            param.unitmeasure = currentItem.unitMeassureCode;
            param.kitcomposition = (currentItem.kitComposition == '') ? new Array() : currentItem.kitComposition;
            param.wareHouseCode = warehouseCode;
            param.actualStock = warehouseStock;

            this.showLoader();
            
            this.ajaxRequest("ActionService.asmx/AddToCartWithWarehouseCode", param, function (response) {

                thisObject.hideLoader(function () {

                    var isQuantityLimit = (response.d != null && response.d != '' && response.d == 'limit');

                    var message = 'Item successfully added to your cart.';
                    message = (isQuantityLimit) ? config.messages.MESSAGE_STORE_STOCK_QUANTITY_ON_HAND + " \n\n" + message : message;

                    self.ajaxRequest("ActionService.asmx/GetShoppingCartNumberOfItems", null, function (result) {
                        var numCart = result.d;
                        var numCartContainer = self.getElemBySelChecker(config.cartNumItemID);
                        if (numCart != null && numCartContainer != null) {
                            $(numCartContainer).text(numCart);
                        }
                    });

                    alert(message);
                });

            });

        },

        attachEventsOnMapMenu: function () {

            var lnkClose = this.getElemBySelChecker(config.mainStorePickupMapLinkCloseId);
            if (lnkClose.length > 0) {
                lnkClose.unbind('click')
                        .bind("click", this.CreateJqueryDelegate(this.mapLinkCloseClick, this));
            }

            var btnSearchHow = this.getElemBySelChecker(config.mainStorePickupMenuHowButtonId);
            if (btnSearchHow.length > 0) {
                btnSearchHow.unbind('click')
                        .bind("click", this.CreateJqueryDelegate(this.searchHowClick, this));
            }

            var txtSearcHow = this.getElemBySelChecker(config.mainStorePickupMenuHowAddressTextBoxId);
            if (txtSearcHow.length > 0) {
                txtSearcHow.unbind("keypress").keypress(function (e) {
                    if (e.which == '13') {
                        e.preventDefault();
                        thisObject.searchHowClick();
                    }
                })
            }

        },

        fetchNextWareHouse: function (scrollTo)  {
            var config = getConfig();

            var param = this.getParamObjectForWhereSearch();
            var addressObject = new Object();
            addressObject.Address = '';
            addressObject.PostalCode = param.postalCode;
            addressObject.City = param.city;
            addressObject.State = param.state;
            addressObject.Country = param.country;

            var nextRecordRow = this.getNextWarehouseRecordRowNumber();
            param.nextRecord = nextRecordRow;

            this.showLoader();

            this.getStoreInventoryWarehouseList(param, function (response) {

                var newWarehouses = thisObject.ToJsonObject(response.d);

                var latestRecordNum = pluginConstants.ZERO_VALUE;
                if (newWarehouses.length == 0) { latestRecordNum = nextRecordRow; }
                else { latestRecordNum = newWarehouses[newWarehouses.length - 1].RowNumber }

                //append to the previous record
                var formattedAddress = thisObject.getFormattedAddress(addressObject);

                thisObject.appendToFoundWarehouse(newWarehouses);
                thisObject.startLoadDistance(newWarehouses, formattedAddress);

                //sort by distance
                //thisObject.sortByDistance(dto);

                if (nextRecordRow < latestRecordNum) {

                    thisObject.loadStoreListContainerTemplate(newWarehouses)
                              .setSearchListContainerScrollTo(scrollTo);

                } else if (nextRecordRow > latestRecordNum) {

                    thisObject.clearFoundWarehouse()
                              .loadStoreListContainerTemplate(newWarehouses);

                }
                else {

                    thisObject.setAllStoresAreLoaded(true);

                }

                thisObject.attachEvents()
                          .hideLoader();

            });
        },

        sortByDistance : function(warehouses) {
            warehouses.sort(function (a, b) { return a.Distance - b.Distance; });
        },

        sortByRowNumber: function (warehouses) {
            warehouses.sort(function (a, b) { return a.RowNumber - b.RowNumber; });
        },

        setAllStoresAreLoaded: function(value) {
            allStoresAreLoaded = value;
        },

        resetAllStoreAreLoaded: function()
        {
            this.setAllStoresAreLoaded(false);
            return this;
        },

        isAllStoresAreLoaded: function () {
            return allStoresAreLoaded;
        },

        getNextWarehouseRecordRowNumber: function ()
        {

            if (foundWarehouses != null && foundWarehouses.length > 0)
            {
                thisObject.sortByRowNumber(foundWarehouses);
                return foundWarehouses[foundWarehouses.length - 1].RowNumber;
            }
            
            return pluginConstants.ZERO_VALUE;
        },

        autoScrollTopAfterSearch: function () {
            this.setSearchListContainerScrollTo(0);
            return this;
        },

        setSearchListContainerScrollTo : function(value) {
            var scrollerDiv = this.getElemBySelChecker(config.mainStorePickupListScrollerContainerId);
            if (scrollerDiv.length > 0) {
                scrollerDiv.scrollTop(value);
            }
            return this;
        },

        selectWarehouseClick: function (evt) {
            var config = getConfig();

            if (!evt) return;

            var warehouseCode = $(evt.target).attr(config.buttonWarehouseAttribute);
            var warehouseStock = $(evt.target).attr(config.storeItemStockAttr);
            var cartQuantity = config.cartItem.quantity;

            var container = this.getElemBySelChecker(config.shippingMethodSelectedStoreContainerId);
            if (container.length > 0) {

                var warehouse = this.getWarehouseAddressByCode(warehouseCode);
                var html = this.parseTemplateReturnHtml(config.selectedWarehouseShippingMethodTemplateId, warehouse);

                var trueQuantity = parseFloat(cartQuantity);
                var trueStock = parseFloat(warehouseStock);

                if (trueQuantity > trueStock && config.appConfig.LimitCartToQuantityOnHand)
                {
                    var message = this.formatString(config.messages.MESSAGE_STORE_SELECT_CHANGE_WAREHOUSE, trueStock, trueQuantity) + '\n' +
                                  config.messages.MESSAGE_STORE_SELECT_CHANGE_WAREHOUSE1 + '\n\n' +
                                  config.messages.MESSAGE_STORE_SELECT_CHANGE_WAREHOUSE2

                    if (!confirm(message))
                    {
                        return;
                    }
                }

                container.html(null)
                         .html(html);

            }
            else
            {
                alert('Error in template - unable to get the shippingMethodSelectedStoreContainer element \n\n please check the shipping-method-template.js template');
            }

            var hiddenWarehouseElement = this.getElemBySelChecker(config.hiddenInStoreSelectedWarehouseElementId);
            if (hiddenWarehouseElement.length > 0) {
                hiddenWarehouseElement.val(warehouseCode);
            }
            else {
                alert('Error retrieving the hidden WarehouseCode element \n\n Please check the ShippingMethodControl.ascx User Control');
            }

            this.unloadDialog();
        },

        linkMapClick: function (evt) {
            var config = getConfig();

            if (!evt) return;

            this.unbindMainMenuEvent();

            this.showLoader();

            this.getElemBySelChecker(config.mainStorePickupListContainerId).hide();

            this.loadMapContainerTemplate(function() {

                thisObject.initializeGoogleMap(function() {

                    thisObject.loadMapMenuContainerTemplate(function () {

                        var wareHouseCode = $(evt.target).attr(config.buttonWarehouseAttribute);
                        if (wareHouseCode != pluginConstants.EMPTY_VALUE) {

                            setTimeout(function () {

                                thisObject.displayStoreInMap(wareHouseCode)
                                          .displayStoreInfoInHeader(wareHouseCode);

                            }, 300);

                        }

                        thisObject.attachEventsOnMapMenu();
                        thisObject.hideLoader();

                    });

                });

            });
        },

        unbindMainMenuEvent : function () {
            var div = this.getElemBySelChecker(config.mainStorePickupSideMenuContainerId)
            if (div.length > 0) {
                div.unbind("keypress");
            }
        },

        displayStoreInfoInHeader: function (wareHouseCode) {

            var config = getConfig();

            var warehouse = this.getWarehouseAddressByCode(wareHouseCode);
            if (warehouse != null) {

                this.getStoreWorkingHourInfoFromDB(wareHouseCode, function (response) {

                    if (response.d != null && response.d != '' && (typeof response.d != 'undefined')) {

                        var header = thisObject.ToJsonObject(response.d);

                        var param = new Object();
                        param.Warehouse = warehouse;
                        param.WorkingHours = header.WorkingHours;
                        param.Holidays = header.Holidays;
                        param.Caption = new Object();
                        param.Caption.StoreInfoTitleText = config.messages.MESSAGE_STORE_INFO_TITLE_TEXT;
                        param.Caption.StoreHoursTitleText = config.messages.MESSAGE_STORE_HOUR_TITLE_TEXT;
                        param.Caption.StoreCloseText = config.messages.MESSAGE_STORE_CLOSE_TEXT;
                        param.Caption.StoreHolidayTitleText = config.messages.MESSAGE_STORE_HOLIDAY_HEADER_TEXT;

                        param.Caption.StoreTelephoneText = config.messages.MESSAGE_STORE_TELEPHONE_HEADER_TEXT;
                        param.Caption.StoreFaxText = config.messages.MESSAGE_STORE_FAX_HEADER_TEXT;
                        param.Caption.StoreEmailText = config.messages.MESSAGE_STORE_EMAIL_HEADER_TEXT;
                        param.Caption.StoreWebsiteText = config.messages.MESSAGE_STORE_WEBSITE_HEADER_TEXT;

                        var html = thisObject.parseTemplateReturnHtml(config.mainStorePickupInfoHeaderContainerTemplateId, param);
                        thisObject.getElemBySelChecker(config.mainStorePickupInfoHeaderContainerId).html(null).append(html);

                    }

                });

            } else {
                alert('error retrieving the warehouse, unable to display on the map.')
            }
            
        },

        getStoreWorkingHourInfoFromDB: function(warehouseCode, callBack) {

            var param = new Object();
            param.warehouseCode = warehouseCode;
            thisObject.ajaxRequest("ActionService.asmx/GetStorePickupWarehouseStoreHoursToJSON", param, function (response) {
                if (typeof callBack != 'undefined') callBack(response);
            });

        },

        mapLinkCloseClick: function () {

            this.getElemBySelChecker(config.mainStorePickupMapContainerId).remove();
            this.getElemBySelChecker(config.mainStorePickupListContainerId).fadeIn('slow');

            this.getElemBySelChecker(config.mainStorePickupMenuHowWrapperId).remove();
            this.getElemBySelChecker(config.mainStorePickupMenuWhereWrapperId).fadeIn('slow');

            this.getElemBySelChecker(config.mainStorePickupInfoHeaderContainerId).html(null);

            this.attachEvents();
        },

        searchHowClick: function() {
            if (this.validateHowInput()) {

                var addressInput = this.getElemBySelChecker(config.mainStorePickupMenuHowAddressTextBoxId).val().trim();

                this.addressGoogleSearch(addressInput, function (results, status) {

                    if (status == google.maps.GeocoderStatus.OK) {

                        var request = {
                            origin: new google.maps.LatLng(results[0].geometry.location.lat(), results[0].geometry.location.lng()),
                            destination: thisObject.getMarker().getPosition(),
                            travelMode: google.maps.DirectionsTravelMode.DRIVING
                        };

                        if (directionsService == null) {
                            directionsService = new google.maps.DirectionsService();
                        }

                        directionsService.route(request, function (response, status) {

                            if (status == google.maps.DirectionsStatus.ZERO_RESULTS ||
                                status != google.maps.DirectionsStatus.OK)
                            {
                                alert(config.messages.MESSAGE_MAP_DIRECTION_ERROR_MESSAGE);
                                return;
                            }

                            if (directionsDisplay == null) {
                                directionsDisplay = new google.maps.DirectionsRenderer();
                            }

                            directionsDisplay.setPanel(thisObject.getElemBySelChecker(config.mainStorePickupMapDirectionContainerId)[0]);
                            directionsDisplay.setMap(map);
                            directionsDisplay.setDirections(response);
                            thisObject.getMarker().setMap(null);

                        });

                    }
                    else {
                        alert(config.messages.MESSAGE_MAP_DIRECTION_ERROR_MESSAGE);
                    }

                })

            } else {
                alert(con.messages.MESSAGE_MENU_SEARCH_WHERE_VALIDATION_MESSAGE);
                this.getElemBySelChecker(config.mainStorePickupMenuHowAddressTextBoxId).focus();
            }
        },

        getMarker: function(){
            return marker;
        },

        initializeGoogleMap: function (callBack) {

            var config = getConfig();
            var mapElement = this.getElemBySelChecker(config.mainStorePickupMapId);

            if (mapElement.length > 0) {

                map = new google.maps.Map(mapElement[0], config);

                this.fixIEGoogleMapDesignBug(map)
                    .fixIEGoogleMapPanoramaView(map);

                if (typeof callBack != 'undefined') callBack();

            }

            return this;
           
        },

        fixIEGoogleMapPanoramaView: function(map) {

            var panorama = map.getStreetView();
            google.maps.event.addListener(panorama, 'visible_changed', function () {

                if (panorama.getVisible()) {
                    thisObject.fixGoogleMapControls(map, true);
                }

            });

            return this;

        },

        fixIEGoogleMapDesignBug: function(map) {

            google.maps.event.addListener(map, 'idle', function () {
                thisObject.fixGoogleMapControls(map, false);
            });

            return this;
        },

        fixGoogleMapControls: function(map, isPanorama) {

            setTimeout(function () {

                var el = $("#" + map.getDiv().id)[0];

                if (el == null) return;

                var nodes = el.getElementsByTagName('*'), i, t;

                for (i = 0; i < nodes.length; i++) {
                    t = nodes[i].getAttribute('title');

                    if (t === "Pan left" || t === "Pan right" || t === "Pan up" || t === "Pan down") {
                        if (nodes[i].style.opacity !== 1) {
                            nodes[i].style.opacity = 0;
                        }
                    }

                    if (t === "Zoom out") {
                        nodes[i].style.top = isPanorama ? -4 : 0;
                    }

                }

            }, 300);

        },

        displayStoreInMap: function (wareHouseCode, callback) {

            var warehouse = this.getWarehouseAddressByCode(wareHouseCode);
            if (warehouse != null) {

                this.addressGoogleSearch(this.getFormattedAddress(warehouse.Address), function (results, status) {
    
                    if (status == google.maps.GeocoderStatus.OK) {

                        marker = new google.maps.Marker({
                            position: new google.maps.LatLng(results[0].geometry.location.lat(), results[0].geometry.location.lng()),
                            title: warehouse.WareHouseDescription,
                            map: map
                        });

                        thisObject.zoomMapOnAddMarker()
                                  .addMarkerClick()
                                  .setCenter(marker);

                    }

                })

            } else {
                alert('error retrieving the warehouse, unable to display on the map.')
            }

            return this;

        },

        zoomMapOnAddMarker: function () {
            map.setZoom(pluginConstants.DEFAULT_MAP_ZOOM_ON_ADD_MARKER);
            return this;
        },

        addMarkerClick: function () {
            var markerObject = this.getMarker();
            google.maps.event.addListener(markerObject, 'click', function () {
                map.setZoom(pluginConstants.DEFAULT_MAP_ZOOM_ON_MARKER_CLICK);
                map.setCenter(markerObject.getPosition());
            });

            return this;
        },

        addressGoogleSearch: function (address, callback) {

            var config = getConfig();

            if (address == pluginConstants.EMPTY_VALUE) {
                if (typeof callback != 'undefined') callback();
            }

            if (geocoder == null) { geocoder = new google.maps.Geocoder() }

            geocoder.geocode({ address: address }, function (results, status) {
                if (typeof callback != 'undefined') callback(results, status);
            });
        },

        getFormattedAddress : function(addressObject) {

            var fullyQualifiedAddress = '';

            if (typeof addressObject == 'undefined' || addressObject == null) return fullyQualifiedAddress;

            if (addressObject.Address != null &&
                addressObject.Address != pluginConstants.EMPTY_VALUE)
            {
                fullyQualifiedAddress += addressObject.Address
            }

            if (addressObject.City != null &&
                addressObject.City != pluginConstants.EMPTY_VALUE)
            {
                fullyQualifiedAddress += ", " + addressObject.City;
            }

            if (addressObject.State != null &&
                addressObject.State != pluginConstants.EMPTY_VALUE)
            {
                fullyQualifiedAddress += ", " + addressObject.State;
            }

            if (addressObject.PostalCode != null &&
                addressObject.PostalCode != pluginConstants.EMPTY_VALUE)
            {
                fullyQualifiedAddress += ", " + addressObject.PostalCode;
            }

            if (addressObject.Country != null &&
                addressObject.Country != pluginConstants.EMPTY_VALUE) {

                if (fullyQualifiedAddress != pluginConstants.EMPTY_VALUE) {
                    fullyQualifiedAddress += ", " + addressObject.Country;
                } else {
                    fullyQualifiedAddress += addressObject.Country;
                }
                
            }

            return $.trim(fullyQualifiedAddress);

        },

        getWarehouseAddressByCode: function (wareHouseCode) {

            var allWarehouses = this.getFoundWareHouses();
            return $.grep(allWarehouses, function (eachWarehouse, i) {
                if (eachWarehouse.WareHouseCode == wareHouseCode) return eachWarehouse;
            })[0];

        },

        searchWhereClick: function() {
            var config = getConfig();

            if (this.validateWhereInputs()) {
                
                this.showLoader();

                this.resetAllStoreAreLoaded()
                    .clear()
                    .searchInventoryWarehouse(false, null, function () {

                        thisObject.attachEvents()
                                  .autoScrollTopAfterSearch()
                                  .hideLoader();

                    });

            }
            else
            {
                alert(config.messages.MESSAGE_MENU_SEARCH_WHERE_VALIDATION_MESSAGE);
                this.getElemBySelChecker(config.mainStorePickupMenuWherePostalTextBoxId).focus();
            }
        },

        validateWhereInputs : function() {

            var isValid = false;

            var element = this.getElemBySelChecker(config.mainStorePickupMenuWherePostalTextBoxId);
            if (element.length > 0 && element.val().trim() != pluginConstants.EMPTY_VALUE) {
                isValid = true;
            }

            element = this.getElemBySelChecker(config.mainStorePickupMenuWhereCityTextBoxId);
            if (element.length > 0 && element.val().trim() != pluginConstants.EMPTY_VALUE) {
                isValid = true;
            }

            element = this.getElemBySelChecker(config.mainStorePickupMenuWhereStateTextBoxId);
            if (element.length > 0 && element.val().trim() != pluginConstants.EMPTY_VALUE) {
                isValid = true;
            }

            element = this.getElemBySelChecker(config.mainStorePickupMenuWhereCountryDropDownId);
            if (element.length > 0 && element[0].selectedIndex > 0) {
                isValid = true;
            }

            return isValid;
        },

        validateHowInput: function() {

            var isValid = false;
            var element = this.getElemBySelChecker(config.mainStorePickupMenuHowAddressTextBoxId);
            if (element.length > 0 && element.val().trim() != pluginConstants.EMPTY_VALUE) {
                isValid = true;
            }

            return isValid;

        },

        centerDialog: function () {
            var config = getConfig();
            this.getElemBySelChecker(config.mainStorePickupDialogId).dialog("option", "position", "center");
        },

        autoCenterDialogOnResizeAndScroll: function() {
            $(window).resize(function () {
                thisObject.centerDialog();
            });
            $(window).scroll(function () {
                thisObject.centerDialog();
            });
        },

        loadDialogTemplate: function (jsonObject) {
            var config = getConfig();

            var html = this.parseTemplateReturnHtml(config.mainStorePickupDialogTemplateId, jsonObject);
            $('body').append(html);

            return this;
        },

        loadMenuContainerTemplate: function (jsonObject) {
            var config = getConfig();

            var copy = jsonObject;
            copy.Countries = this.ToJsonObject(jsonObject.JsonCountries);

            //compose the select country text as object to be part of the countr arrary
            var selectTextObject = new Object()
            selectTextObject.CountryCode = config.messages.MESSAGE_MENU_COUNTRY_SELECT_TEXT;

            copy.Countries = this.ArrayInsertAt(copy.Countries, 0, [selectTextObject]);
            copy.Caption = new Object();
            copy.Caption.MenuHeaderText = config.messages.MESSAGE_MENU_HEADER_AVAILABILITY;
            copy.Caption.PickupWhereText = config.messages.MESSAGE_PICKUP_WHERE_TEXT;
            copy.Caption.ZipText = config.messages.MESSAGE_MENU_ZIP_POSTAL_TEXT;
            copy.Caption.CiyText = config.messages.MESSAGE_MENU_CITY_TEXT;
            copy.Caption.StateText = config.messages.MESSAGE_MENU_STATE_TEXT;
            copy.Caption.CountryText = config.messages.MESSAGE_MENU_COUNTRY_TEXT;
            copy.Caption.ButtonSearchText = config.messages.MESSAGE_MENU_BUTTON_SEARCH_TEXT;
            copy.Address.FullAddress = thisObject.getFormattedAddress(copy.Address);

            var html = this.parseTemplateReturnHtml(config.mainStorePickupMenuContainerTemplateId, copy);
            this.getElemBySelChecker(config.mainStorePickupSideMenuContainerId).html(null).append(html);

            return this;
        },

        getPostalCodeFromTextBox: function () {
            var element = this.getElemBySelChecker(config.mainStorePickupMenuWherePostalTextBoxId);
            if (element.length > 0 && element.val() != pluginConstants.EMPTY_VALUE) {
                return element.val();
            }
        },

        loadMapMenuContainerTemplate: function (callBack) {
            var config = getConfig();

            var param = new Object()
            param.Caption = new Object();
            param.Address = new Object();
            param.Address.PostalCode = this.getPostalCodeFromTextBox();
            param.Caption.MenuHeaderText = config.messages.MESSAGE_MENU_HEADER_DIRECTION;
            param.Caption.PickupHowText = config.messages.MESSAGE_PICKUP_HOW_TEXT;
            param.Caption.AddressText = config.messages.MESSAGE_SAMPLE_ADDRESS_INPUT_TEXT;
            param.Caption.ButtonSearchText = config.messages.MESSAGE_MAP_GET_DIRECTION_BUTTON_TEXT;
            param.Caption.DirectionText = config.messages.MESSAGE_MAP_DIRECTION_HEADER_TEXT;

            var html = thisObject.parseTemplateReturnHtml(config.mainStorePickupWhereMenuTemplateId, param);
            thisObject.getElemBySelChecker(config.mainStorePickupMenuWhereWrapperId).hide();
            thisObject.getElemBySelChecker(config.mainStorePickupSideMenuContainerId).append(html);
            
            if (typeof callBack != 'undefined') callBack();
        },

        loadMapContainerTemplate: function (callBack) {
            var param = new Object()
            param.Caption = new Object();
            param.Caption.MapCloseText = config.messages.MESSAGE_MAP_CLOSE_TEXT;

            var html = this.parseTemplateReturnHtml(config.mainStorePickupMapContainerTemplateId, param);
            this.getElemBySelChecker(config.mainStorePickupMapBodyWrapperId).append(html);

            $(html).hide().fadeIn('slow');

            if (typeof callBack != 'undefined') callBack();
        },

        loadStoreListHeaderContainerTemplate : function(data) {

            var config = getConfig();

            var html = '';
            var objectCopy = new Object();
            objectCopy.Caption = new Object();

            if (data.length > 0) {

                objectCopy.Caption.StoreListHeaderText = config.messages.MESSAGE_STORE_LIST_HEADER_TEXT;
                objectCopy.Caption.StoreListStoreHeaderText = config.messages.MESSAGE_STORE_LIST_HEADER_STORE_TEXT;
                objectCopy.Caption.StoreListStockAvailabilityHeaderText = config.messages.MESSAGE_STORE_LIST_HEADER_AVAILABILITY_TEXT;
                objectCopy.Caption.StoreListStockText = config.messages.MESSAGE_STORE_LIST_HEADER_STOCK_TEXT;
                objectCopy.Caption.UnitMeasureText = config.cartItem.unitMeassureCode;

                var appConfigObject = new Object();
                appConfigObject.ShowActualStock = config.appConfig.ShowActualStock;
                appConfigObject.ShowInAddToCart = config.showAddToCart;
                objectCopy.AppConfig = appConfigObject;

                html = this.parseTemplateReturnHtml(config.mainStorePickupListContainerTemplateId, objectCopy);

            } else {

                objectCopy.Caption.NoRecordsFoundMessage = config.messages.MESSAGE_MENU_SEARCH_NO_RECORD_FOUND_MESSAGE;
                html = this.parseTemplateReturnHtml(config.mainStorePickupListEmptrySearchTemplateId, objectCopy);

            }

            this.getElemBySelChecker(config.mainStorePickupMapBodyWrapperId).html(null)
                                                                            .append(html);
            return this;
        },

        loadStoreListContainerTemplate: function (jsonObject) {
            var config = getConfig();

            if (jsonObject.length == 0) return this;

            var appConfigObject = new Object();
            appConfigObject.ShowActualStock = config.appConfig.ShowActualStock;
            appConfigObject.ShowInAddToCart = config.showAddToCart;

            var dataListObject = new Object();
            dataListObject.Caption = new Object();
            dataListObject.Caption.AddToCartButtonText = config.messages.MESSAGE_ADD_TO_CART_BUTTON_TEXT;
            dataListObject.Caption.StoreListSelectButtonText = config.messages.MESSAGE_STORE_LIST_SELECT_BUTTON_TEXT;
            dataListObject.Caption.StoreListDirectionLinkText = config.messages.MESSAGE_STORE_LIST_DIRECTION_LINK_TEXT;
            dataListObject.AppConfig = appConfigObject;
            dataListObject.WarehouseList = jsonObject;

            var dataListHtml = this.parseTemplateReturnHtml(config.mainStorePickupListDataTemplateId, dataListObject);

            var trs = $(dataListHtml).find("tr");
            if (trs.length > 0) {
                $('#' + config.mainStorePickupListTableContainerId).append($(trs));
            }

            return this;
        },

        clear: function () {
            Address = null;
            Store = null;
            map = null;

            marker = null;
            allStoresAreLoaded = false;
            markerObject = null;
            geocoder = null;
            isGoogleQueError = false;

            this.resetAllStoreAreLoaded()
                .clearFoundWarehouse();

            return this;
        },

        unloadDialog: function () {
            thisObject.getElemBySelChecker(config.mainStorePickupDialogId).dialog("close");
        },

        showInModal: function () {
            var config = getConfig();

            var curentDocWidth = '955'; //($(document).width() - $(document).width() * .5);
            var curentDocHeight = '700'; //($(document).height() - $(document).height() * .63);

            $(document).ready(function () {

                thisObject.getElemBySelChecker(config.mainStorePickupDialogId).dialog({
                    autoOpen: false,
                    modal: true,
                    position: "center",
                    resizable: false,
                    zIndex: 99999,
                    width: curentDocWidth,
                    height: curentDocHeight,
                    open: function (event, ui) {
                        $(this).parent().hide().fadeIn("slow");
                    },
                    close: function() {
                    
                        var thisDialog = this;
                        $(this).parent().show().fadeOut("slow", function () {
                            $(thisDialog).remove();
                            $('.ui-effects-wrapper').remove();

                            thisObject.removeCssReference(config.pluginTemplateCss);
                            thisObject.clear()
                                      .clearFoundWarehouse();
                        });

                    }
                });

                thisObject.getElemBySelChecker(config.mainStorePickupDialogId).dialog('open');
                thisObject.autoCenterDialogOnResizeAndScroll();

            });

            return this;
        },

        setCenter: function(marker) {
            map.setCenter(marker.getPosition());
            return this;
        },

        config: function (args) {
            setConfig($.extend(defaults, args));
            return (getConfig());
        }

    };

    //jqueryBasePlugin is located inside core.js
    $.extend($.fn.InStorePickup, new jqueryBasePlugin());

    function setConfig(value) {
        config = value;
    }

    function getConfig() {
        return config;
    }

})(jQuery);