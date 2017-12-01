/// <reference path="base_ajax.js" />
/// <reference path="core.js" />
/// <reference path="../components/instore_pickup/setup.js" />

// register namespace
Type.registerNamespace('ise.Controls');

ise.Controls.ShippingMethodOption = function (ctrl, shippingMethod, freightCalculation, freight, rateID, current, freightChargeType) {
    this.id = ctrl.id;
    this.ctrl = ctrl;
    this.shippingMethod = shippingMethod;
    this.freightCalculation = freightCalculation;
    this.freight = freight;
    this.rateID = rateID;
    this.shippingInfo = current;
    this.freightChargeType = freightChargeType;

    var del = Function.createDelegate(this, this.handleOptionClick);
    $addHandler(this.ctrl, 'click', del);
    this.selectedEventHandler = null;
}
ise.Controls.ShippingMethodOption.registerClass('ise.Controls.ShippingMethodOption');
ise.Controls.ShippingMethodOption.prototype = {
    handleOptionClick: function (option) { this.onSelected(option); },

    onSelected: function (option) {
        if (option) {
            if (this.selectedEventHandler) {
                this.selectedEventHandler(this);
                //hdenOnpage - hidden field indicator that where in the checkout1.aspx
                //if undefined where in the checkoutshipping.aspx
                if ($getElement('hdenOnpage') != undefined) {
                    this.updateSummary(this);
                }
            }
        }
    },

    getIsChecked: function () {
        if (this.ctrl) { return this.ctrl.checked; }
        return false;
    },

    onCalculationSummaryComplete: function (summary) {
        if (summary == undefined) return;

        $getElement('subtotal_elem').innerHTML = summary.SubTotal;

        if ($getElement('fr_elem') != undefined) { $getElement('fr_elem').innerHTML = summary.Freight; }

        $getElement('tax_elem').innerHTML = summary.Tax;
        $getElement('total_elem').innerHTML = summary.DueTotal;

        var main = this.id.split('_')[0];

        var loadingRow = $getElement(main + '_LoadingRow');
        if (null != loadingRow) { ise.Utils.hideRow(loadingRow); }

        var container = $getElement(main + '_Content');
        if (null != container) { ise.Utils.showRow(container); }
    },

    updateSummary: function (seloption) {
        var main = this.id.split('_')[0];

        var loadingRow = $getElement(main + '_LoadingRow');
        if (null != loadingRow) {
            ise.Utils.showRow(loadingRow);
        }

        var container = $getElement(main + '_Content');
        if (null != container) {
            ise.Utils.hideRow(container);
        }

        var service = new ActionService();
        var calculationSummary = service.GetShippingCalculation(seloption.shippingMethod, seloption.freightCalculation, seloption.rateID, Function.createDelegate(this, this.onCalculationSummaryComplete));
    },

    setSelectedEventHandler: function (handler) { return this.selectedEventHandler = handler; },

    getShippingMethod: function () { return this.shippingMethod; },

    getFreightCalculation: function () { return this.freightCalculation; },

    getFreight: function () { return this.freight; },

    getRateID: function () { return this.rateID; },

    getFreightChargeType : function () { return this.freightChargeType; },

    isPickUpFreightChargeType : function() { return this.freightChargeType != null && this.freightChargeType.toUpperCase() == "PICK UP"; }
}
// Constructor
ise.Controls.ShippingMethodControl = function (id) {
    this.id = id;
    this.ctrl = $getElement(id);
    this.addressControlId = null;
    this.addressControl = null;
    this.shippingMethodID = null;
    this.skipShipping = false;
    this.options = new Array();
    this.currentOption = null;

    this.validationController = new ise.Validators.ValidationController();

    if (this.ctrl) {
        this.hookUpRefreshLink();
    }
    // delegate
    this.refreshClickedEventHandler = null;
    this.shippingOptionsLoadedEventHandlers = new Array();
    this.addressValue = null;
    this.shippingMethodLoadingComplete = false;
    this.hidePickupStoreLink = false;
    this.instoreCartItem = null;
    this.hidePickUpShippingOption = false;
    this.isShippingOnDemand = ise.Configuration.getConfigValue("ShippingRatesOnDemand");
    this.isAutoClickOfOption = (ise.Configuration.getConfigValue("ShippingRatesOnDemand") == 'true');
    this.selectedInstoreContainerPrefixId = 'selectedInStoreInfoContainer_';
    this.IsFromPayPal = false;
    this.selectedOption = 0;
    this.isShippingRateLoadingComplete = false;
    this.isMultipleShipping = true;
    this.isSkipValidation = false;
}
ise.Controls.ShippingMethodControl.registerClass('ise.Controls.ShippingMethodControl');
ise.Controls.ShippingMethodControl.prototype = {
    tryDisplaySelectedWarehouse: function () {
        var currentFreightChargeType = this.getFreightChargeType();
        if (currentFreightChargeType.toUpperCase() != 'PICK UP'.toUpperCase()) return;

        var warehouseCode = this.getSelectedInStoreWarehouseCode();
        if (warehouseCode == null || warehouseCode == '') return;

        var basePlugin = new jqueryBasePlugin();
        var param = new Object();
        param.warehouseCode = warehouseCode;

        var self = this;
        basePlugin.ajaxRequest("ActionService.asmx/GetStorePickupWarehouseInfo", param, function (result) {
            if (typeof result != 'undefined')
            {
                var warehouseModel = basePlugin.ToJsonObject(result.d);
                var html = basePlugin.parseTemplateReturnHtml('selectedWarehouseTemplateId', warehouseModel);

                var container = basePlugin.getElemBySelChecker(self.getSelectedStoreContainerId());
                if (container.length > 0)
                {
                    container.html(null)
                             .html(html)
                             .fadeIn('fast');
                }
            }
        });
    },

    getIsSkipShipping: function () { return this.skipShipping; },

    setIsSkipShipping: function (skip) { this.skipShipping = skip; },

    setHidePickupStoreLink: function (value) { this.hidePickupStoreLink = value; },

    hideInStorePickUpShippingOption : function(value) { this.hidePickUpShippingOption = value; },

    setIfMultipleShipping: function (value) { this.isMultipleShipping = true; },

    setInstoreCartItem: function (cartItem) {
        this.instoreCartItem = cartItem;
    },

    getInstoreCartItem: function () {
        return this.instoreCartItem;
    },

    hookUpRefreshLink: function () {
        var link = $getElement(this.id + '_Refresh');
        if (link) { link.onclick = Function.createDelegate(this, this.handleRefreshClick); }
    },

    handleRefreshClick: function () { this.onRefreshClicked(); },

    onRefreshClicked: function (adressid) {
        var shippingInfoValid = false;
        if (this.refreshClickedEventHandler) { shippingInfoValid = this.refreshClickedEventHandler(this); }
        if (shippingInfoValid) { this.requestShippingMethod(adressid); }
    },

    setRefreshClickedEventHandler: function (handler) { this.refreshClickedEventHandler = handler; },

    clearOptions: function () {
        this.options.length = 0;
        this.setShippingMethod(ise.Constants.EMPTY_STRING);
    },

    getCurrentOption: function () { return this.currentOption; },

    setCurrentOption: function (option, isCalculateAll, isMultipleShipping, callback) {
        if (!isCalculateAll || option.shippingInfo.FreightDisplay == "FREE") {
            this.selectedOption = this.selectedOption + 1;
        }

        this.currentOption = option;
        this.setShippingMethod(option.getShippingMethod());
        this.setFreightCalculation(option.getFreightCalculation());
        this.setFreightChargeType(option.getFreightChargeType());
        this.setFreight(option.getFreight());
        this.setRateID(option.getRateID());

        var isPickupShippingMethod = option.isPickUpFreightChargeType();
        var thisObject = this;
        //by pass shipping on demand if pick up shipping method.
        if (this.isShippingOnDemand == "true" && option.shippingInfo.FreightDisplay != "FREE" && !option.shippingInfo.IsDummyShippingMethod && !isPickupShippingMethod) {
            var freight = $("#" + option.id).parent("div").children("span").children("span:last")
            if (freight.hasClass("shipping-rate")) {
                var rate = $("#" + option.id).parent("div").children("span").children("span:last").html();
                thisObject.setFreight(rate);
                return false;
            }

            if (!this.isAutoClickOfOption && !isCalculateAll) { $("body").data("globalLoader").show(); }

            AjaxCallWithSecuritySimplified(
                "GetShippingMethodRates",
                { "shippingMethodInfo": option.shippingInfo, "addressNameValuPairOverride": this.addressValue ? this.addressValue : '', "addressId": this.shippingMethodID },
                function (result) {
                    thisObject.onLoadShippingRate(result.d, option);
                    if (!this.isAutoClickOfOption && !isCalculateAll && !isMultipleShipping) { $("body").data("globalLoader").hide(); }
                    thisObject.setFreight(result.d);
                    if (typeof callback != 'undefined') callback();
                },
                null, this.serviceToken
            );
        }
        else {
            if (this.isShippingOnDemand == "true" && isPickupShippingMethod && !isCalculateAll) {
                if (typeof callback != 'undefined') callback();
                $("body").data("globalLoader").hide();
            }
            else {
                if (typeof callback != 'undefined') callback();
                $("body").data("globalLoader").hide();
            }
        }

        if (isPickupShippingMethod) { this.tryDisplaySelectedWarehouse(); }
        this.hideSelectedStoreWhenChangeOption(isPickupShippingMethod);
    },

    hideSelectedStoreWhenChangeOption: function (isPickupShippingMethod) {
        if (this.hidePickupStoreLink) return;

        var basePlugin = new jqueryBasePlugin();
        var selectedInstoreContainer = basePlugin.getElemBySelChecker(this.getSelectedStoreContainerId());
        var hiddenElement = basePlugin.getElemBySelChecker(this.getHiddenInStoreWareHouseElementId());
        if (!isPickupShippingMethod) {
            if (selectedInstoreContainer.length > 0) {
                if (hiddenElement.length > 0 && hiddenElement.val().trim().length > 0) {
                    if (selectedInstoreContainer.length > 0 && selectedInstoreContainer.html().trim().length > 0) {
                        selectedInstoreContainer.fadeOut('slow');
                    }
                }
            }
        }
        else {
            if (selectedInstoreContainer.length > 0) {
                if (hiddenElement.length > 0 && hiddenElement.val().trim().length > 0) {
                    if (selectedInstoreContainer.length > 0 && selectedInstoreContainer.html().trim().length > 0) {
                        selectedInstoreContainer.fadeIn('slow');
                    }
                }
            }

        }
    },

    onLoadShippingRate: function (rate, option, isCalculateAll) {
        var tempOption = (typeof option == 'undefined') ? this.currentOption : option;
        $("#" + tempOption.id).parent("div").children("span").children("span:last").html(rate).addClass("shipping-rate");

        if (rate == "(Not Applicable)") {
            $("#" + tempOption.id).parent("div").children("span").children("span:last").addClass("shipping-rate-not-applicable");
            $("#" + tempOption.id).attr("checked", false);
            $("#" + tempOption.id).attr("disabled", "disabled");
        }
    },

    onShowAllRatesClick: function () {
        if (this.validate) {
            var thisObject = this;
            if (!thisObject.isShippingRateLoadingComplete) {
                var counter = 0;
                $("body").data("globalLoader").show();
                for (var opt = 0; opt < this.options.length; opt++) {
                    if (this.options[opt].isPickUpFreightChargeType() || this.options[opt].shippingInfo.FreightDisplay == "FREE") {
                        continue;
                    }
                    counter++;
                    this.setCurrentOption(this.options[opt], true, false, function () {
                        counter--;
                        if (counter == thisObject.selectedOption) {
                            thisObject.isShippingRateLoadingComplete = true;
                            $("body").data("globalLoader").hide();
                        }
                    });
                }
            }
        }
    },

    registerOption: function (option) {
        this.options[this.options.length] = option;

        if (option.getIsChecked()) {
            var thisObject = this;
            var callback = function () {
                thisObject.isAutoClickOfOption = false;
            }
            this.setCurrentOption(option, false, this.isMultipleShipping, callback);
        }
        else {
            if (this.getShippingMethod() == '') { this.isAutoClickOfOption = false; }
        }

        var del = Function.createDelegate(this, this.handleOptionSelected);
        option.setSelectedEventHandler(del);
    },

    handleOptionSelected: function (option) { this.setCurrentOption(option); },

    setAddressControlId: function (addressId) {
        this.addressControlId = addressId;

        if (ise.Controls.AddressController) {
            var control = ise.Controls.AddressController.getControl(addressId);
            if (null != control) {
                this.addressControl = control;
                this.requestShippingMethod(true);
            }
            else {
                ise.Controls.AddressController.addObserver(this);
            }
        }
    },

    notify: function (ctrl) {
        if (null != this.addressControlId && ctrl) {
            if (ctrl.id == this.addressControlId) {
                this.addressControl = ctrl;
                this.requestShippingMethod(true);
            }
        }
    },

    setAddressValue: function (value) { this.addressValue = value; },

    addShippingOptionsLoadedEventHandler: function (handler) { this.shippingOptionsLoadedEventHandlers.push(handler); },

    onShippingOptionsLoaded: function () {
        for (var ctr = 0; ctr < this.shippingOptionsLoadedEventHandlers.length; ctr++) {
            var handler = this.shippingOptionsLoadedEventHandlers[ctr];
            handler(this);
        }
    },

    onLoadShippingMethodComplete: function (shippingMethods) {
        this.clearOptions();
        this.shippingMethodLoadingComplete = true;

        var contentArea = $getElement(this.id + '_content');
        contentArea.className = 'content';
        contentArea.innerHTML = '';

        var tbl = $('<div />')[0];
        contentArea.appendChild(tbl);

        var anyShippingMethodValid = false;
        shippingMethods = this.tryRemoveInStorePickUpShippingOptionIfHide(shippingMethods);

        var cartItem = this.getInstoreCartItem();
        var otherMethods = null;

        var overSizedShippingMethods = $.grep(shippingMethods, function (item, index) { return item.ForOversizedItem; });
        var hasOverSizedShippingMethods = (overSizedShippingMethods.length > 0);
        var isCurrentItemBelongToTheOversizedItems = (cartItem != null) && ($.grep(overSizedShippingMethods, function (item, i) { return item.OverSizedItemCode == cartItem.ItemCode }).length > 0);
        var isCurrentItemBelongToTheOversizedItemsWithPickupShippingMethod = (cartItem != null) && 
                                                ($.grep(overSizedShippingMethods,
                                                    function (item, i) { return (item.OverSizedItemCode == cartItem.ItemCode && item.FreightChargeType.toUpperCase() != "PICK UP") }).length > 0);

        if (hasOverSizedShippingMethods) {
            if (isCurrentItemBelongToTheOversizedItems) {
                //get oversize item shipping method
                otherMethods = $.grep(overSizedShippingMethods, function (item, index) { return item.OverSizedItemCode == cartItem.ItemCode; });
                otherMethods = $.grep(shippingMethods, function (item, index) { return item.Code == otherMethods[0].Code && item.OverSizedItemCode == cartItem.ItemCode });
                if (otherMethods.length > 0) { otherMethods[0].IsDefault = true; }
            } else {
                otherMethods = $.grep(shippingMethods, function (item, i) { return !item.ForOversizedItem; });
            }
        }
        else {
            otherMethods = $.grep(shippingMethods, function (item, i) { return !item.ForOversizedItem; });
        }

        try {
            for (var ctr = 0; ctr < otherMethods.length; ctr++) {
                var current = otherMethods[ctr];

                var td = $('<div/>').addClass('shipping-option')[0];
                $(tbl).append(td);

                var shippingMethodObject = new Object();
                var html = null;
                if (current.IsError) {
                    shippingMethodObject.disabled = 'disabled=disabled';
                    shippingMethodObject.description = current.Description;
                    html = $.tmpl("shippingMethodOptionTemplateID", shippingMethodObject);
                    $(td).append(html);
                }
                else {
                    shippingMethodObject.id = this.id + "_shippingMethod_" + ctr;
                    shippingMethodObject.name = this.id + ":shippingMethod";
                    shippingMethodObject.description = current.Description;

                    if (cartItem != null && (typeof cartItem.ShippingMethod != 'undefined' && cartItem.ShippingMethod != '') && current.Description.trim() == cartItem.ShippingMethod.trim()) {
                        shippingMethodObject.isDefault = 'checked=checked';
                    } else {
                        shippingMethodObject.isDefault = (current.IsDefault) ? 'checked=checked' : "";
                    }

                    if (this.isShippingOnDemand == "true") {
                        shippingMethodObject.freight = (current.FreightDisplay == "FREE") ? current.FreightDisplay : "";
                    }
                    else {
                        shippingMethodObject.freight = current.FreightDisplay;
                    }

                    var isPickupFreightCharge = (current.FreightChargeType != null && current.FreightChargeType.toUpperCase() == "PICK UP");
                    if (isPickupFreightCharge) {
                        shippingMethodObject.freight = "";
                        if (!this.hidePickupStoreLink) {
                            shippingMethodObject.InStoreCaption = ise.StringResource.getString('checkoutstore.aspx.1');
                            if (cartItem != null) { //Use the item counter due to inaccessibility of shippingmethod id
                                shippingMethodObject.ItemCounter = cartItem.Counter;
                            }
                        }
                    }

                    html = $.tmpl("shippingMethodOptionTemplateID", shippingMethodObject);
                    $(td).append(html);

                    var opt = $("#" + this.id + "_shippingMethod_" + ctr)[0];
                    if (opt)
                    {
                        if (this.isShippingOnDemand == "true") {
                            var option = new ise.Controls.ShippingMethodOption(opt, current.Code, current.FreightCalculation, current.Freight, current.RateID, current, current.FreightChargeType);
                        }
                        else {
                            var option = new ise.Controls.ShippingMethodOption(opt, current.Code, current.FreightCalculation, current.Freight, current.RateID, null, current.FreightChargeType);
                        }
                        this.registerOption(option);
                        if (!this.hidePickupStoreLink) { this.attachMapLoaderOnInStoreLink($(td).find("#instore-map-link_" + cartItem.Counter), option, opt); }
                    }
                    anyShippingMethodValid = true;
                }
            }

            $(".shipping-description").unbind("click")
                                      .click(function () {
                var id = $(this).attr("id").split("$");
                id = (id.length > 0)? id[1] : id[0];
                $("#" + id).trigger("click");
                                      });

            var shippingMethodOversizedInfo = new Array();
            for (var ctr = 0; ctr < overSizedShippingMethods.length; ctr++) {
                var current = overSizedShippingMethods[ctr];
                if (cartItem != null && cartItem.ItemCode != current.OverSizedItemCode) continue;

                var shippingMethodOversizedObject = new Object();
                shippingMethodOversizedObject.ItemDescription = current.OversizedItemName;
                shippingMethodOversizedObject.ShippingDescription = current.Description;
                shippingMethodOversizedObject.Freight = current.FreightDisplay;
                shippingMethodOversizedInfo.push(shippingMethodOversizedObject);
                shippingMethodOversizedObject = null;
            }

            if (shippingMethodOversizedInfo.length > 0) {
                var stringResource = new Object();
                stringResource.oversizedMessage = ise.StringResource.getString('checkoutshipping.aspx.9');
                stringResource.productHeader = ise.StringResource.getString('checkoutshipping.aspx.10');
                stringResource.shippingMethodHeader = ise.StringResource.getString('checkoutshipping.aspx.11');
                stringResource.freightHeader = ise.StringResource.getString('checkoutshipping.aspx.12');

                var shippingMethodOversizedContent = new Object();
                shippingMethodOversizedContent.shippingMethodInfo = shippingMethodOversizedInfo;
                shippingMethodOversizedContent.stringResource = stringResource;

                html = $.tmpl("shippingMethodOversizedTemplateID", shippingMethodOversizedContent);
                $(tbl).append(html);
            }
        }
        catch (e) {
            alert(e);
            contentArea.innerHTML = e; //'Could not load shipping methods';
        }
        if (anyShippingMethodValid) { this.onShippingOptionsLoaded(); }
    },

    attachMapLoaderOnInStoreLink: function (linkElement, option, optionElement) {
        if (!linkElement || $(linkElement).length == 0) return;
        $(linkElement).unbind("click")
                      .bind("click", function () {
                          $(optionElement).attr('checked', 'checked');
                          option.onSelected(optionElement);
                      }).bind("click",new jqueryBasePlugin().CreateJqueryDelegate(this.inStoreOnClick, this));
    },
    
    tryRemoveInStorePickUpShippingOptionIfHide: function (shippingMethods) {
        if (this.hidePickUpShippingOption) {
            return $.grep(shippingMethods, function (smethod, index) {
                return (smethod.FreightChargeType == null || smethod.FreightChargeType.toUpperCase() != "PICK UP");
            });
        }
        return shippingMethods;
    },

    inStoreOnClick: function (sender) {
        if (!sender) return;

        var selectedInstoreContainerid = this.getSelectedStoreContainerId();
        var hiddenInStoreSelectedWarehouseElementId = this.getHiddenInStoreWareHouseElementId();

        var cartItem = this.getInstoreCartItem();
        //Format: ['groupd id + KitCounter', 'groupd id + KitCounter'] ...n
        //Sample Composition ['1+4', '3+23']
        var composition = cartItem.kitComposition;
        var basePlugin = new jqueryBasePlugin();
        basePlugin.downloadPlugin('components/instore_pickup/setup.js', function () {
            var loader = new instorePluginLoader();
            loader.start(function (config) {
                config.shippingMethodSelectedStoreContainerId = selectedInstoreContainerid;
                config.hiddenInStoreSelectedWarehouseElementId = hiddenInStoreSelectedWarehouseElementId;
                config.cartItem = {
                    itemCode: cartItem.ItemCode,
                    unitMeassureCode: cartItem.UnitMeassureCode,
                    kitComposition: composition,
                    quantity: cartItem.Quantity
                };
                $.fn.InStorePickup.setup(config);
            })
        });
    },

    onComplete: function (result) { alert(result); },

    requestShippingMethod: function (addressId) {
        if (addressId != null) { this.shippingMethodID = addressId; }
        var onCompleteDelegate = Function.createDelegate(this, this.onLoadShippingMethodComplete);
        var service = new ActionService();
        service.ShippingMethod(this.addressValue ? this.addressValue : '', this.id, this.shippingMethodID, onCompleteDelegate);
    },

    requestShippingMethod: function (addressId, itemSpecificType) {
        if (addressId != null) { this.shippingMethodID = addressId; }
        var onCompleteDelegate = Function.createDelegate(this, this.onLoadShippingMethodComplete);
        var service = new ActionService();
        service.ShippingMethod(this.addressValue ? this.addressValue : '', this.id, this.shippingMethodID, itemSpecificType ? itemSpecificType : '', onCompleteDelegate);
    },

    registerShowAllRatesButton: function (buttonID) {
        var thisObject = this;
        $('#' + buttonID).unbind("click")
                                      .click(function () {
                                          thisObject.onShowAllRatesClick();
                                      });
    },

    getShippingMethod: function () {
        var elemShippingMethod = $getElement(this.id + '_shippingMethod');
        if (elemShippingMethod) { return elemShippingMethod.value; }
        return ise.Constants.EMPTY_STRING;
    },

    setFreight: function (value) {
        var elemFreight = $getElement(this.id + '_freight');
        if (elemFreight) { elemFreight.value = value; }
    },

    setShippingMethod: function (value) {
        var elemShippingMethod = $getElement(this.id + '_shippingMethod');
        if (elemShippingMethod) { elemShippingMethod.value = value; }
    },

    setSelectedInStoreWarehouseCode: function (value) {
        var elemShippingMethod = $getElement(this.id + '_hdenInStoreSelectedWarehouseCode');
        if (elemShippingMethod) { elemShippingMethod.value = value; }
    },

    getSelectedInStoreWarehouseCode: function () {
        var elemShippingMethod = $getElement(this.id + '_hdenInStoreSelectedWarehouseCode');
        if (elemShippingMethod) { return elemShippingMethod.value; }
        return '';
    },

    getSelectedStoreContainerId: function () {
        var cartItem = this.getInstoreCartItem();
        if (cartItem == null) {
            alert('Error occured in method: inStoreOnClick \n\n this.getInstoreCartItem() return null ');
            return
        }
        return this.selectedInstoreContainerPrefixId + cartItem.Counter;
    },

    getHiddenInStoreWareHouseElementId : function() { return this.id + '_hdenInStoreSelectedWarehouseCode'; },

    getFreightCalculation: function () {
        var elemFreightCalculation = $getElement(this.id + '_freightCalculation');
        if (elemFreightCalculation) { return elemFreightCalculation.value; }
        return ise.Constants.EMPTY_STRING;
    },

    setFreightCalculation: function (value) {
        var elemFreightCalculation = $getElement(this.id + '_freightCalculation');
        if (elemFreightCalculation) { elemFreightCalculation.value = value; }
    },

    setFreightChargeType: function(value) {
        var elemShippingMethod = $("#" + this.id + '_hdenFreightChargeType');
        if (elemShippingMethod) { $(elemShippingMethod).val(value); }
    },

    getFreightChargeType: function (value) {
        var elemShippingMethod = $("#" + this.id + '_hdenFreightChargeType');
        if (elemShippingMethod) { return elemShippingMethod.val(); }
        return '';
    },

    getRateID: function () {
        var elemRateID = $getElement(this.id + '_realTimeRateGUID');
        if (elemRateID) { return elemRateID.value; }
        return ise.Constants.EMPTY_STRING;
    },

    setRateID: function (value) {
        var elemRateID = $getElement(this.id + '_realTimeRateGUID');
        if (elemRateID) { elemRateID.value = value; }
    },

    setValidationSummary: function (summary) { this.validationController.setValidationSummary(summary); },

    registerValidator: function (validator) { this.validationController.register(validator); },

    clearValidationSummary: function () { this.validationController.clear(); },

    setSkipValidation: function () { this.isSkipValidation = true; },

    validate: function (clear) {
        if (this.isSkipValidation) {
            this.isSkipValidation = false;
            return true;
        }
        if (this.skipShipping) return true;
        return this.validationController.validate(clear);
    }
}
ise.Controls.ShippingMethodController = {
    initialize : function() {
        this.controls = new Array();
        this.observers = new Array();
    },
    
    registerControl : function(id) {
        var ctrl = new ise.Controls.ShippingMethodControl(id);
        this.controls[id] = ctrl;
        this.notifyObservers(ctrl);
        return ctrl;
    },
    
    addObserver : function(observer) {
        if(observer) {
            this.observers[this.observers.length] = observer;
        }
    },
    
    notifyObservers : function(control) {
        for(var ctr=0; ctr< this.observers.length; ctr++) {
            this.observers[ctr].notify(control);
        }
    },
    
    getControl : function(id) {
        var ctrl = this.controls[id];
        return ctrl;
    }
}
ise.Controls.ShippingMethodController.initialize();
