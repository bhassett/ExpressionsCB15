/// <reference path="../core.js" />

(function ($) {

    var config = {};
    var thisRealTimeAddressVerificationPlugin;
    var pluginIsActive = false;

    var pluginConstants = {
        COMMA: ",",
        DOT_VALUE: ".",
        EMPTY_VALUE: "",
        UNDEFINED: "undefined",
        OTHER: "other",
        ZERO_VALUE: 0,
        ADDRESS_TYPE_RESIDENCE : "residential",
        NONE: "none",
        BODY: "body",
        TYPE_FUNCTION: "function",
        ERROR: "[error]",
        USE_MY_SHIPPING : "use-my-shipping",
        USE_SUGGESTED_SHIPPING: "use-suggested-shipping",
        USE_MY_BILLING: "use-my-billing",
        USE_SUGGESTED_BILLING: "use-suggested-billing",
        HTML_HIDDEN_BILLING_CITY_STATES: "<input type='hidden' id='billing-city-states' value='other'/>",
        HTML_HIDDEN_SHIPPING_CITY_STATES : "<input type='hidden' id='shipping-city-states' value='other'/>"
    };

    var constantClassnames = {
        DISPLAY_NONE: "display-none",
        NO_CLOSE: "no-close",
        USE_SUGGESTED_ADDRESS: "use-suggested-address",
        DIV_SUGGESTED_ADDRESS_ROW : "divSuggestedAddressRow",
        DIV_SHIPPING_POSTAL_CITY_OTHER: "shipping-zip-city-other-place-holder",
        DIV_BILLING_POSTAL_CITY_OTHER: "billing-zip-city-other-place-holder",
        REQUIRED_INPUT: "required-input",
        INVALID_POSTAL: "invalid-postal",
        CURRENT_INPUT_ON_FOCUS: "current-input-on-focus",
        IS_DIV_SUGGESTED_ADDRESS_ROW_ACTIVE: "is-div-suggested-row-active"
    };

    var constantIDs = {
        BUTTON_SUBMIT_SUGGESTED_ADDRESS: "submit-suggested-address",
        DIV_SUGGESTED_BILLING_ADDRESS: "divSuggestedBillingAddress",
        DIV_SUGGESTED_SHIPPING_ADDRESS: "divSuggestedShippingAddress",
        DIV_SHIPPING_ENTER_POSTAL: "shipping-enter-postal-label-place-holder",
        DIV_BILLING_ENTER_POSTAL: "billing-enter-postal-label-place-holder",
        DIV_MESSAGE_TIPS : "ise-message-tips",
        HIDDEN_BILLING_CITY_STATES : "billing-city-states",
        HIDDEN_SHIPPING_CITY_STATES: "shipping-city-states",
        OPTION_SHIPPING_ADDRESS : "input:radio[name=shipping-address]",
        OPTION_BILLING_ADDRESS :  "input:radio[name=billing-address]",
        SELECTED_OPTION_SHIPPING_ADDRESS: "input[name=shipping-address]:checked",
        SELECTED_OPTION_BILLING_ADDRESS: "input[name=billing-address]:checked"
    };

    var constantsTemplateID = {
        REQUEST_PROGRESS_TEXT: "requestProgressTextTemplate",
        MULTIPLE_ADDRESS_MATCH_DIALOG: "multipleAddressMatchDialogTemplate",
        MULTIPLE_ADDRESS_MATCH_DIALOG_NO_SHIPPING_ADDRESS: "multipleAddressMatchDialogNoShippingAddressTemplate",
        SINGLE_ADDRESS_MATCH_DIALOG: "singleAddressMatchDialogTemplate",
        SINGLE_ADDRESS_MATCH_DIALOG_NO_SHIPPING_ADDRESS : "singleAddressMatchDialogNoShippingAddressTemplate",
        DEFAULT_ADDRESS_SELECTED_TEMPLATE : "defaultAddressSelectedTemplate",
        SINGLE_SHIPPING_ADDRESS_MATCH_DIALOG : "singleShippingAddressMatchDialogTemplate",
        MULTIPLE_SHIPPING_ADDRESS_MATCH_DIALOG: "multipleShippingAddressMatchDialogTemplate",
        SINGLE_ADDRESS_MATCH_DIALOG_GATEWAY_ERROR :  "singleAddressMatchDialogTemplate_GatewayErrorTemplate",
        SINGLE_ADDRESS_MATCH_DIALOG_NO_SHIPPING_ADDRESS_GATEWAY_ERROR: "singleAddressMatchDialogNoShippingAddress_GatewayErrorTemplate",
        SINGLE_SHIPPING_ADDRESS_MATCH_DIALOG_GATEWAY_ERROR :  "singleShippingAddressMatchDialogTemplate_GatewayErrorTemplate"

    }; 

    var constantAttributes = {
        DISPLAY : "display",
        SLOW: "slow",
        OPEN: "open",
        DATA_STREET_ADDRESS: "data-streetAddress",
        DATA_CITY: "data-city",
        DATA_STATE: "data-state",
        DATA_POSTAL_CODE: "data-postalCode",
        DATA_ORIGINAL_HTML_CONTENT: "data-originalHtmlContent",
        DATA_UPDATE_OPTION_ID : "data-updateoptionid",
        DESTROY: "destroy"
    };

    var serviceMethods = {
        REQUEST_ADDRESS_MATCH: "RequestAddressBestMatch"
    };


    var stringResource = {
        USE_BILLING_ADDRESS_PROVIDED: pluginConstants.EMPTY_VALUE,
        USE_SHIPPING_PROVIDED: pluginConstants.EMPTY_VALUE,
        CONFIRM_CORRECT_ADDRESS: pluginConstants.EMPTY_VALUE,
        UNABLE_TO_VERIFY_ADDRESS: pluginConstants.EMPTY_VALUE,
        SELECT_MATCHING_BILLING_ADDRESS: pluginConstants.EMPTY_VALUE,
        SELECT_MATCHING_SHIPPING_ADDRESS: pluginConstants.EMPTY_VALUE,
        GATEWAY_ERROR_TEXT: pluginConstants.EMPTY_VALUE,
        PROGRESS_TEXT: pluginConstants.EMPTY_VALUE
    };

    var defaults = {
        submitButtonID: pluginConstants.EMPTY_VALUE,
        isAllowShipping: false,
        addressMatchDialogContainerID : pluginConstants.EMPTY_VALUE,
        errorContainerId : pluginConstants.EMPTY_VALUE,
        progressContainterId : pluginConstants.EMPTY_VALUE,
        buttonContainerId : pluginConstants.EMPTY_VALUE,
        isWithShippingAddress: false,
        pluginTemplateCss: 'components/address-verification/index.css',
        basePluginTemplateCss: 'components/address-verification/skin/index.css', //if pluginTemplateCss is overridden but does not exist use the base which is the original path
        pluginTemplate: 'components/address-verification/skin/jquery.realtime.address.verification.tmpl.min.html',
        basePluginTemplate: 'components/address-verification/skin/jquery.realtime.address.verification.tmpl.min.html', //if pluginTemplate is overridden but does not exist use the base which is the original path
        billingInputID : {
            POSTAL_CODE : "BillingAddressControl_txtPostal",
            CITY : "BillingAddressControl_txtCity",
            STATE : "BillingAddressControl_txtState",
            COUNTRY : "BillingAddressControl_drpCountry",
            STREET_ADDRESS: "BillingAddressControl_txtStreet",
            CITY_STATE_SELECTOR: "billing-city-states"
        },
        billingLabelID : {
            POSTAL_CODE: "",
            CITY: "",
            STATE: "",
            STREET_ADDRESS: ""
        },
        shippingInputID : {
            POSTAL_CODE : "ShippingAddressControl_txtPostal",
            CITY : "ShippingAddressControl_txtCity",
            STATE : "ShippingAddressControl_txtState",
            COUNTRY : "ShippingAddressControl_drpCountry",
            STREET_ADDRESS: "ShippingAddressControl_txtStreet",
            RESIDENCE_TYPE : "ShippingAddressControl_drpType",
            CITY_STATE_SELECTOR : "shipping-city-states"
        },
        shippingLabelID : {
            POSTAL_CODE: "ShippingAddressControl_lblStreet",
            CITY: "ShippingAddressControl_lblCity",
            STATE: "ShippingAddressControl_lblState",
            STREET_ADDRESS: "ShippingAddressControl_lblPostal"
        },
        stringResourceKeys: pluginConstants.EMPTY_VALUE,
        onepagecheckout: {
            isTrue: false
        }
    };

    var isOnePageCheckoutShippingSection = false;

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

    $.fn.RealTimeAddressVerification = {

        setup: function (config) {

            thisRealTimeAddressVerificationPlugin = this;
            pluginIsActive = thisRealTimeAddressVerificationPlugin.toBoolean(ise.Configuration.getConfigValue("UseShippingAddressVerification"));
   
            if (config) setConfig($.extend(defaults, config));

            thisRealTimeAddressVerificationPlugin.downloadPluginSkin(function () {});

            var callback = function () {

                thisRealTimeAddressVerificationPlugin.setStringResourcesValue(config.stringResourceKeys);
       
            }

            thisRealTimeAddressVerificationPlugin.downloadStringResources(thisRealTimeAddressVerificationPlugin.convertObjectToArray(config.stringResourceKeys), callback);
        },

        attachEvents: function (config) {},

        attachAddressDialogControlsEvents: function(continueButtonCallback){
          
            $(constantIDs.OPTION_BILLING_ADDRESS).click(function () {
                var $this = $(this);
                var id = $this.attr("id");
                id = $.trim(id);

                if (id == pluginConstants.USE_MY_BILLING) {

                    $(thisRealTimeAddressVerificationPlugin.selectorChecker(constantIDs.DIV_SUGGESTED_BILLING_ADDRESS)).children("div").removeClass(constantClassnames.IS_DIV_SUGGESTED_ADDRESS_ROW_ACTIVE);
                    thisRealTimeAddressVerificationPlugin.clearDefaultAddressToBeUse(constantIDs.DIV_SUGGESTED_BILLING_ADDRESS);

                } else {

                    $(thisRealTimeAddressVerificationPlugin.selectorChecker(constantIDs.DIV_SUGGESTED_BILLING_ADDRESS)).children("div").addClass(constantClassnames.IS_DIV_SUGGESTED_ADDRESS_ROW_ACTIVE);
                    thisRealTimeAddressVerificationPlugin.setDefaultAddressToBeUse(constantIDs.DIV_SUGGESTED_BILLING_ADDRESS, null);
                }


                thisRealTimeAddressVerificationPlugin.setAddressControlValue(false, $this);

            });

            $(constantIDs.OPTION_SHIPPING_ADDRESS).click(function () {
                var $this = $(this);
                var id = $this.attr("id");
                id = $.trim(id);

                if (id == pluginConstants.USE_MY_SHIPPING) {

                    $(thisRealTimeAddressVerificationPlugin.selectorChecker(constantIDs.DIV_SUGGESTED_SHIPPING_ADDRESS)).children("div").removeClass(constantClassnames.IS_DIV_SUGGESTED_ADDRESS_ROW_ACTIVE);
                    thisRealTimeAddressVerificationPlugin.clearDefaultAddressToBeUse(constantIDs.DIV_SUGGESTED_SHIPPING_ADDRESS);

                } else {

                    $(thisRealTimeAddressVerificationPlugin.selectorChecker(constantIDs.DIV_SUGGESTED_SHIPPING_ADDRESS)).children("div").addClass(constantClassnames.IS_DIV_SUGGESTED_ADDRESS_ROW_ACTIVE);
                    thisRealTimeAddressVerificationPlugin.setDefaultAddressToBeUse(constantIDs.DIV_SUGGESTED_SHIPPING_ADDRESS, null);
                }

                thisRealTimeAddressVerificationPlugin.setAddressControlValue(true, $this);

            });


            $(thisRealTimeAddressVerificationPlugin.selectorChecker(constantIDs.BUTTON_SUBMIT_SUGGESTED_ADDRESS)).click(function () {
                
                var config = getConfig();


                thisRealTimeAddressVerificationPlugin.setAddressControlValue(false, $(constantIDs.SELECTED_OPTION_BILLING_ADDRESS));

                if (config.isWithShippingAddress) {
                  thisRealTimeAddressVerificationPlugin.setAddressControlValue(true, $(constantIDs.SELECTED_OPTION_SHIPPING_ADDRESS));
                }

                $(thisRealTimeAddressVerificationPlugin.selectorChecker(config.addressMatchDialogContainerID)).dialog(constantAttributes.DESTROY);
                
                // this is only applicable for one page checkout (a workaround to having shipping and billing address control on one page) ->
                if (config.onepagecheckout.isTrue && isOnePageCheckoutShippingSection) {
                    continueButtonCallback();
                    return false;
                }
                // <-

                continueButtonCallback();
            });


            $(pluginConstants.DOT_VALUE + constantClassnames.DIV_SUGGESTED_ADDRESS_ROW).click(function () {
                
                thisRealTimeAddressVerificationPlugin.setDefaultAddressToBeUse(null, $(this));
             
            });

        },

        setButtonSpinnerText: function (o, text) {
            o.html("<i class='icon-spin icon-spinner '></i> " + text);
        },
       
        setStringResourcesValue: function (stringResourceKeys) {
            var config = getConfig();

            stringResource.USE_BILLING_ADDRESS_PROVIDED = thisRealTimeAddressVerificationPlugin.getString(stringResourceKeys.useBillingAddressProvided);
            stringResource.CONFIRM_CORRECT_ADDRESS = thisRealTimeAddressVerificationPlugin.getString(stringResourceKeys.confirmCorrectAddress);
            stringResource.UNABLE_TO_VERIFY_ADDRESS = thisRealTimeAddressVerificationPlugin.getString(stringResourceKeys.unableToVerifyAddress);
            stringResource.SELECT_MATCHING_BILLING_ADDRESS = thisRealTimeAddressVerificationPlugin.getString(stringResourceKeys.selectMatchingBillingAddress);

            stringResource.GATEWAY_ERROR_TEXT = thisRealTimeAddressVerificationPlugin.getString(stringResourceKeys.gatewayErrorText);
            stringResource.PROGRESS_TEXT = thisRealTimeAddressVerificationPlugin.getString(stringResourceKeys.progressText);

            if (config.isWithShippingAddress || thisRealTimeAddressVerificationPlugin.isEmpty(stringResourceKeys.selectMatchingShippingAddress) == false) {
                stringResource.SELECT_MATCHING_SHIPPING_ADDRESS = thisRealTimeAddressVerificationPlugin.getString(stringResourceKeys.selectMatchingShippingAddress);
            }

            if (config.isWithShippingAddress || thisRealTimeAddressVerificationPlugin.isEmpty(stringResourceKeys.useShippingAddressProvided) == false) {
                stringResource.USE_SHIPPING_PROVIDED = thisRealTimeAddressVerificationPlugin.getString(stringResourceKeys.useShippingAddressProvided);
            }
        },

        downloadPluginSkin: function (callback) {

            var config = getConfig();

            $.get(config.pluginTemplate, function (data, textStatus, XMLHttpRequest) {
                $(pluginConstants.BODY).append(data);
                thisRealTimeAddressVerificationPlugin.downloadCss(config.pluginTemplateCss, function () {
                    if (typeof callback === pluginConstants.TYPE_FUNCTION) callback();
                });
            });

        },

        config: function (args) {
            setConfig($.extend(defaults, args));
            return (getConfig());
        },

        requestAddressMatch: function (callback) {

            if (pluginIsActive == false) {
                callback();
                return false;
            }

            isOnePageCheckoutShippingSection = false;

            var config = getConfig();

            var bPostal = thisRealTimeAddressVerificationPlugin.getInputControlValue(config.billingInputID.POSTAL_CODE);
            var bCity = thisRealTimeAddressVerificationPlugin.getInputControlValue(config.billingInputID.CITY);
            var bState = thisRealTimeAddressVerificationPlugin.getInputControlValue(config.billingInputID.STATE);

            var bCountry = thisRealTimeAddressVerificationPlugin.getInputControlValue(config.billingInputID.COUNTRY);
            var bStreetAddress = thisRealTimeAddressVerificationPlugin.getInputControlValue(config.billingInputID.STREET_ADDRESS);
            var bCityState = thisRealTimeAddressVerificationPlugin.getInputControlValue(config.billingInputID.CITY_STATE_SELECTOR);

            var o = thisRealTimeAddressVerificationPlugin.parsedCityState(bCityState, bCity, bState);

            bCity = o.City;
            bState = o.State;

            var sPostal = pluginConstants.EMPTY_VALUE;
            var sCity = pluginConstants.EMPTY_VALUE;
            var sState = pluginConstants.EMPTY_VALUE;
            var sCountry = pluginConstants.EMPTY_VALUE;
            var sStreetAddress = pluginConstants.EMPTY_VALUE;
 
            if (config.isAllowShipping && config.isWithShippingAddress) {

                sPostal = thisRealTimeAddressVerificationPlugin.getInputControlValue(config.shippingInputID.POSTAL_CODE);
                sCity = thisRealTimeAddressVerificationPlugin.getInputControlValue(config.shippingInputID.CITY);
                sState = thisRealTimeAddressVerificationPlugin.getInputControlValue(config.shippingInputID.STATE);
                sCountry = thisRealTimeAddressVerificationPlugin.getInputControlValue(config.shippingInputID.COUNTRY);
                sStreetAddress = thisRealTimeAddressVerificationPlugin.getInputControlValue(config.shippingInputID.STREET_ADDRESS);

                var sCityState = thisRealTimeAddressVerificationPlugin.getInputControlValue(config.shippingInputID.CITY_STATE_SELECTOR);
                var o = thisRealTimeAddressVerificationPlugin.parsedCityState(sCityState, sCity, sState);

                sCity = o.City;
                sState = o.State;
            }

            var successFunction = function (response) {

                if (thisRealTimeAddressVerificationPlugin.isEmpty(response.d)) {
                    callback();
                    return false;
                }

                var current = new Object();

                current.billing = { StreetAddress: bStreetAddress, City: bCity, State: bState, PostalCode: bPostal };
                current.shipping = { StreetAddress: sStreetAddress, City: sCity, State: sState, PostalCode: sPostal };

                thisRealTimeAddressVerificationPlugin.showAddressMatchResult(response, current, callback, config.isWithShippingAddress);
                
                current = null;

            };

            var errorFunction = function (error) {
                console.log(error);
            };

            var billingAddress = new Object();
            billingAddress.Address = bStreetAddress;
            billingAddress.Country = bCountry;
            billingAddress.PostalCode = bPostal;
            billingAddress.State = bState;
            billingAddress.City = bCity;


            var shippingAddress = new Object();
            shippingAddress.Address = sStreetAddress;
            shippingAddress.Country = sCountry;
            shippingAddress.PostalCode = sPostal;
            shippingAddress.State = sState;
            shippingAddress.City = sCity;

            var data = Object()
            data.billing = JSON.stringify(billingAddress);
            data.shipping = JSON.stringify(shippingAddress);

            data.isResidence = thisRealTimeAddressVerificationPlugin.isResidentialAddress(thisRealTimeAddressVerificationPlugin.getInputControlValue(config.shippingInputID.RESIDENCE_TYPE));
            
            thisRealTimeAddressVerificationPlugin.displayRequestMessage(stringResource.PROGRESS_TEXT);
            thisRealTimeAddressVerificationPlugin.ajaxSecureRequest(serviceMethods.REQUEST_ADDRESS_MATCH, data, successFunction, errorFunction);

            billingAddress = null;
            shippingAddress = null;
            data = null;
        },

        
        showAddressMatchResult: function(response, current, continueButtonCallback, isWithShipping){

            var error = thisRealTimeAddressVerificationPlugin.getAddressRequestGatewayError(response);

            if (thisRealTimeAddressVerificationPlugin.isEmpty(error) == false) {
                thisRealTimeAddressVerificationPlugin.showAddressMatchResultWithGatewayError(current, continueButtonCallback, isWithShipping, error);
                return false;
            }

            var data = JSON.parse(response.d);

            if (thisRealTimeAddressVerificationPlugin.isEmpty(data) == false) {
                var count = data.length;

                if (count == 0) {
                    continueButtonCallback();
                    return false;
                }

                if (count > 0) {

                    var content = new Object();
                   
                    content.billingMatch = JSON.parse(data[0].Billing);
                    content.shippingMatch = JSON.parse(data[0].Shipping);
                    content.enteredBilling = current.billing;
                    content.enteredShipping = current.shipping;

                    // if returned match is 1 and with shipping and billing or shipping match has one empty value returned 
                    // or returned billing and shipping is equals to the entered billing and shipping skip showing of dialog
                    // -> proceed on executing the callback

                    if (count == 1 && isWithShipping) {
                        
                        if (thisRealTimeAddressVerificationPlugin.isEmpty([content.billingMatch[0].Address,
                                                                                      content.billingMatch[0].City,
                                                                                      content.billingMatch[0].PostalCode,
                                                                                      content.billingMatch[0].State]) ||
                                       thisRealTimeAddressVerificationPlugin.isEmpty([content.shippingMatch[0].Address,
                                                                                      content.shippingMatch[0].City,
                                                                                      content.shippingMatch[0].PostalCode,
                                                                                      content.shippingMatch[0].State])){

                            continueButtonCallback();
                            return false;

                        }

                        if (content.billingMatch[0].Address == content.enteredBilling.StreetAddress || content.shippingMatch[0].Address == content.enteredShipping.StreetAddress) {
                            continueButtonCallback();
                            return false;
                        }
                    }

                    // if returned match is 1 and with out shipping and billing has an empty value 
                    // or returned billing is equals to the entered billing and shipping skip showing of dialog
                    // -> proceed on executing the callback

                    if (count == 1 && !isWithShipping){
                        
                        if (thisRealTimeAddressVerificationPlugin.isEmpty([content.billingMatch[0].Address,
                                                                                    content.billingMatch[0].City,
                                                                                    content.billingMatch[0].PostalCode,
                                                                                    content.billingMatch[0].State])) {
                            continueButtonCallback();
                            return false;
                        }

                        if (content.billingMatch[0].Address == content.enteredBilling.StreetAddress) {
                            continueButtonCallback();
                            return false;
                        }
                    }

  
                    var resources = new Object();

                    resources.useBillingAddressProvided = stringResource.USE_BILLING_ADDRESS_PROVIDED;
                    resources.useShippingAddressProvided = stringResource.USE_SHIPPING_PROVIDED;
                    resources.confirmCorrectAddress = stringResource.CONFIRM_CORRECT_ADDRESS;
                    resources.unableToVerifyAddress = stringResource.UNABLE_TO_VERIFY_ADDRESS;
                    resources.selectMatchingBillingAddress = stringResource.SELECT_MATCHING_BILLING_ADDRESS;
                    resources.selectMatchingShippingAddress = stringResource.SELECT_MATCHING_SHIPPING_ADDRESS;

                    content.stringResource = resources;

                    var templateId = constantsTemplateID.MULTIPLE_ADDRESS_MATCH_DIALOG;
                    if (isWithShipping == false) {
                        templateId = constantsTemplateID.MULTIPLE_ADDRESS_MATCH_DIALOG_NO_SHIPPING_ADDRESS;
                    }

                    if (parseInt(data[0].AddressMatchResultLimit) == 1 || count == 1) {

                        templateId = constantsTemplateID.SINGLE_ADDRESS_MATCH_DIALOG;

                        if (isWithShipping == false) {
                            templateId = constantsTemplateID.SINGLE_ADDRESS_MATCH_DIALOG_NO_SHIPPING_ADDRESS;
                        }

                    }

                    var dialogContentHTML = thisRealTimeAddressVerificationPlugin.parseJqueryTemplate(templateId, content);
                    $(thisRealTimeAddressVerificationPlugin.selectorChecker(config.addressMatchDialogContainerID)).html(dialogContentHTML).dialog({
                        autoOpen: false,
                        width: 700,
                        modal: true,
                        resize: false,
                        closeOnEscape: false,
                        dialogClass: constantClassnames.NO_CLOSE
                    }).dialog(constantAttributes.OPEN);

                    thisRealTimeAddressVerificationPlugin.setDefaultAddressToBeUse(constantIDs.DIV_SUGGESTED_BILLING_ADDRESS, null);
                    thisRealTimeAddressVerificationPlugin.setDefaultAddressToBeUse(constantIDs.DIV_SUGGESTED_SHIPPING_ADDRESS, null);

                    thisRealTimeAddressVerificationPlugin.attachAddressDialogControlsEvents(continueButtonCallback);
                }
            }
        },

        showAddressMatchResultWithGatewayError: function (current, continueButtonCallback, isWithShipping, error) {

            var content = new Object();
            content.enteredBilling = current.billing;
            content.enteredShipping = current.shipping;

            var resources = new Object();

            resources.useBillingAddressProvided = stringResource.USE_BILLING_ADDRESS_PROVIDED;
            resources.useShippingAddressProvided = stringResource.USE_SHIPPING_PROVIDED;
            resources.confirmCorrectAddress = stringResource.CONFIRM_CORRECT_ADDRESS;
            resources.unableToVerifyAddress = stringResource.UNABLE_TO_VERIFY_ADDRESS;
            resources.selectMatchingBillingAddress = stringResource.SELECT_MATCHING_BILLING_ADDRESS;
            resources.selectMatchingShippingAddress = stringResource.SELECT_MATCHING_SHIPPING_ADDRESS;
            resources.gatewayErrorText = stringResource.GATEWAY_ERROR_TEXT;

            content.stringResource = resources;

            templateId = constantsTemplateID.SINGLE_ADDRESS_MATCH_DIALOG_GATEWAY_ERROR;

            if (isWithShipping == false) {
                templateId = constantsTemplateID.SINGLE_ADDRESS_MATCH_DIALOG_NO_SHIPPING_ADDRESS_GATEWAY_ERROR;
            }

            var dialogContentHTML = thisRealTimeAddressVerificationPlugin.parseJqueryTemplate(templateId, content);
            $(thisRealTimeAddressVerificationPlugin.selectorChecker(config.addressMatchDialogContainerID)).html(dialogContentHTML).dialog({
                autoOpen: false,
                width: 700,
                modal: true,
                resize: false,
                closeOnEscape: false,
                dialogClass: constantClassnames.NO_CLOSE
            }).dialog(constantAttributes.OPEN);

            thisRealTimeAddressVerificationPlugin.setDefaultAddressToBeUse(constantIDs.DIV_SUGGESTED_BILLING_ADDRESS, null);
            thisRealTimeAddressVerificationPlugin.setDefaultAddressToBeUse(constantIDs.DIV_SUGGESTED_SHIPPING_ADDRESS, null);

            thisRealTimeAddressVerificationPlugin.attachAddressDialogControlsEvents(continueButtonCallback);

        },

        getAddressRequestGatewayError: function (result) {

            var test = result.d.split(pluginConstants.ERROR);
            if (test.length > 1) return $.trim(test[1]);

            return pluginConstants.EMPTY_VALUE;
        },

        isResidentialAddress: function (value) {

            if(thisRealTimeAddressVerificationPlugin.isEmpty(value)){
                return false;
            }

            return value.toLowerCase() == pluginConstants.ADDRESS_TYPE_RESIDENCE;

        },

        parsedCityState: function (cityState, city, state) {
    
            var returnData = new Object();
            returnData.City = city;
            returnData.State = state;

            if (cityState != pluginConstants.OTHER) {

                var str = cityState;
                str = str.split(pluginConstants.COMMA);

                if (str.length > 1) {

                    returnData.State = $.trim(str[0]);
                    returnData.City = $.trim(str[1]);

                } else {
            
                    returnData.State = pluginConstants.EMPTY_VALUE;
                    returnData.City = $.trim(str[0]);
           
                }
            }

            return returnData;
        },

        displayRequestMessage: function (message) {

           var config = getConfig();

           var msg = new Object();
           msg.value = message;

           var msgHTML = thisRealTimeAddressVerificationPlugin.parseJqueryTemplate(constantsTemplateID.REQUEST_PROGRESS_TEXT, { msg: msg });

           $button = $(thisRealTimeAddressVerificationPlugin.selectorChecker(config.buttonContainerId));
           $divProgressMessage =  $(thisRealTimeAddressVerificationPlugin.selectorChecker(config.progressContainterId));
           $divErrorMessage = $(thisRealTimeAddressVerificationPlugin.selectorChecker(config.errorContainerId));

           $button.css(constantAttributes.DISPLAY, pluginConstants.NONE);
           $divProgressMessage.html(msgHTML).fadeIn(constantAttributes.SLOW);
           $divErrorMessage.html(pluginConstants.EMPTY_VALUE).fadeOut(constantAttributes.SLOW);
            
        },

        setAddressControlValue : function(isShippingAddress, o){
            var config = getConfig();

            var msg = new Object();
            var streetAddress = o.attr(constantAttributes.DATA_STREET_ADDRESS);
            streetAddress = $.trim(streetAddress);

            var city = o.attr(constantAttributes.DATA_CITY);
            city = $.trim(city);

            var state = o.attr(constantAttributes.DATA_STATE);
            state = $.trim(state);

            var postalCode = o.attr(constantAttributes.DATA_POSTAL_CODE);
            postalCode = $.trim(postalCode);

            var $addressInputBox = null;
            var $cityInputBox = null;
            var $stateInputBox = null;
            var $postalInputBox = null;
            
            var labelStreetAddressID = pluginConstants.EMPTY_VALUE;
            var labelCityID = pluginConstants.EMPTY_VALUE;
            var labelStateID = pluginConstants.EMPTY_VALUE;
            var labelPostalCodeID = pluginConstants.EMPTY_VALUE;

            if(isShippingAddress){
  
                $(thisRealTimeAddressVerificationPlugin.selectorChecker(constantIDs.DIV_SHIPPING_ENTER_POSTAL)).html(pluginConstants.HTML_HIDDEN_SHIPPING_CITY_STATES);
                $(thisRealTimeAddressVerificationPlugin.selectorChecker(constantIDs.HIDDEN_SHIPPING_CITY_STATES)).fadeOut(constantAttributes.SLOW, function () {
                    $(pluginConstants.DOT_VALUE + constantClassnames.DIV_SHIPPING_POSTAL_CITY_OTHER).fadeIn(constantAttributes.SLOW);
                });

                $addressInputBox = $(thisRealTimeAddressVerificationPlugin.selectorChecker(config.shippingInputID.STREET_ADDRESS));
                $cityInputBox = $(thisRealTimeAddressVerificationPlugin.selectorChecker(config.shippingInputID.CITY));
                $stateInputBox = $(thisRealTimeAddressVerificationPlugin.selectorChecker(config.shippingInputID.STATE));
                $postalInputBox = $(thisRealTimeAddressVerificationPlugin.selectorChecker(config.shippingInputID.POSTAL_CODE));

                labelStreetAddressID = config.shippingLabelID.STREET_ADDRESS;
                labelCityID = config.shippingLabelID.CITY;
                labelStateID = config.shippingLabelID.STATE;
                labelPostalCodeID = config.shippingLabelID.POSTAL_CODE;

            }else{

                $(thisRealTimeAddressVerificationPlugin.selectorChecker(constantIDs.DIV_BILLING_ENTER_POSTAL)).html(pluginConstants.HTML_HIDDEN_BILLING_CITY_STATES);
                $(thisRealTimeAddressVerificationPlugin.selectorChecker(constantIDs.HIDDEN_BILLING_CITY_STATES)).fadeOut(constantAttributes.SLOW, function () {
                    $(pluginConstants.DOT_VALUE + constantClassnames.DIV_BILLING_POSTAL_CITY_OTHER).fadeIn(constantAttributes.SLOW);
                });

                $addressInputBox = $(thisRealTimeAddressVerificationPlugin.selectorChecker(config.billingInputID.STREET_ADDRESS));
                $cityInputBox = $(thisRealTimeAddressVerificationPlugin.selectorChecker(config.billingInputID.CITY));
                $stateInputBox = $(thisRealTimeAddressVerificationPlugin.selectorChecker(config.billingInputID.STATE));
                $postalInputBox = $(thisRealTimeAddressVerificationPlugin.selectorChecker(config.billingInputID.POSTAL_CODE));

                labelStreetAddressID = config.billingLabelID.STREET_ADDRESS;
                labelCityID = config.billingLabelID.CITY;
                labelStateID = config.billingLabelID.STATE;
                labelPostalCodeID = config.billingLabelID.POSTAL_CODE;
            }

 
            thisRealTimeAddressVerificationPlugin.clearAddressControlErrorTags($addressInputBox, $cityInputBox, $stateInputBox, $postalInputBox);

            if (!thisRealTimeAddressVerificationPlugin.isEmpty(streetAddress)) {
                $(thisRealTimeAddressVerificationPlugin.selectorChecker(labelStreetAddressID)).addClass(constantClassnames.DISPLAY_NONE);
                $addressInputBox.val(streetAddress);
            }

            if (!thisRealTimeAddressVerificationPlugin.isEmpty(city)) {
                $(thisRealTimeAddressVerificationPlugin.selectorChecker(labelCityID)).addClass(constantClassnames.DISPLAY_NONE);
                $cityInputBox.val(city);
            }

            if (!thisRealTimeAddressVerificationPlugin.isEmpty(state)) {
                $(thisRealTimeAddressVerificationPlugin.selectorChecker(labelStateID)).addClass(constantClassnames.DISPLAY_NONE);
                $stateInputBox.val(state);
            }

            if (!thisRealTimeAddressVerificationPlugin.isEmpty(postalCode)) {
                $(thisRealTimeAddressVerificationPlugin.selectorChecker(labelPostalCodeID)).addClass(constantClassnames.DISPLAY_NONE);
                $postalInputBox.val(postalCode);
            }

            $addressInputBox = null;
            $cityInputBox = null;
            $stateInputBox = null;
            $postalInputBox = null;

        },

        setDefaultAddressToBeUse: function (id, rowSelected) {

            var o = rowSelected;

            if (thisRealTimeAddressVerificationPlugin.isEmpty(rowSelected)) {
                o = $(thisRealTimeAddressVerificationPlugin.selectorChecker(id)).children("div").first();
            }

            if (o.hasClass(constantClassnames.IS_DIV_SUGGESTED_ADDRESS_ROW_ACTIVE) == false) {
                return false;
            }
            
            var content = new Object();
            origHtml = o.attr(constantAttributes.DATA_ORIGINAL_HTML_CONTENT);
            origHtml = $.trim(origHtml);

            if (thisRealTimeAddressVerificationPlugin.isEmpty(origHtml)) {
                content.value = $.trim(o.html());
                o.attr(constantAttributes.DATA_ORIGINAL_HTML_CONTENT, content.value);
            } else {
                content.value = $.trim(origHtml);
            }

            o.html(thisRealTimeAddressVerificationPlugin.parseJqueryTemplate(constantsTemplateID.DEFAULT_ADDRESS_SELECTED_TEMPLATE, { content: content }));

            if (thisRealTimeAddressVerificationPlugin.isEmpty(rowSelected)) {
                $(thisRealTimeAddressVerificationPlugin.selectorChecker(id)).children("div").addClass(constantClassnames.IS_DIV_SUGGESTED_ADDRESS_ROW_ACTIVE);
            } else {
                 o.parent("div").children("div").each(function () {

                    var $this = $(this);

                    if ($this.hasClass(constantClassnames.USE_SUGGESTED_ADDRESS)) {
                        var currentSelectedHtml = $this.attr(constantAttributes.DATA_ORIGINAL_HTML_CONTENT);
                        $this.removeClass(constantClassnames.USE_SUGGESTED_ADDRESS).addClass(constantClassnames.IS_DIV_SUGGESTED_ADDRESS_ROW_ACTIVE).removeClass(constantClassnames.USE_SUGGESTED_ADDRESS).html(currentSelectedHtml);
                        return false;
                    }

                });
            }

            o.addClass(constantClassnames.USE_SUGGESTED_ADDRESS).removeClass(constantClassnames.IS_DIV_SUGGESTED_ADDRESS_ROW_ACTIVE);

            content = null;

            var updateOptionId = o.attr(constantAttributes.DATA_UPDATE_OPTION_ID);
            updateOptionId = $.trim(updateOptionId);

            var streetAddress = o.attr(constantAttributes.DATA_STREET_ADDRESS);
            streetAddress = $.trim(streetAddress);

            var city = o.attr(constantAttributes.DATA_CITY);
            city = $.trim(city);

            var state = o.attr(constantAttributes.DATA_STATE);
            state = $.trim(state);

            var postalCode = o.attr(constantAttributes.DATA_POSTAL_CODE);
            postalCode = $.trim(postalCode);

            var $radioOption = $(thisRealTimeAddressVerificationPlugin.selectorChecker(updateOptionId));

            $radioOption.attr(constantAttributes.DATA_STREET_ADDRESS, streetAddress);
            $radioOption.attr(constantAttributes.DATA_CITY, city);
            $radioOption.attr(constantAttributes.DATA_STATE, state);
            $radioOption.attr(constantAttributes.DATA_POSTAL_CODE, postalCode);
           
        },

        clearAddressControlErrorTags: function (addressInputBox, cityInputBox, stateInputBox, postalInputBox) {

            $(thisRealTimeAddressVerificationPlugin.selectorChecker(constantIDs.DIV_MESSAGE_TIPS)).fadeOut(constantAttributes.SLOW);

            addressInputBox.removeClass(constantClassnames.REQUIRED_INPUT);
            cityInputBox.removeClass(constantClassnames.REQUIRED_INPUT);
            stateInputBox.removeClass(constantClassnames.REQUIRED_INPUT);
            postalInputBox.removeClass(constantClassnames.REQUIRED_INPUT).removeClass(constantClassnames.INVALID_POSTAL).removeClass(constantClassnames.CURRENT_INPUT_ON_FOCUS);
            
        },

        clearDefaultAddressToBeUse: function (id) {

            var o = $(thisRealTimeAddressVerificationPlugin.selectorChecker(id)).children("div");

            o.each(function () {

                var $this = $(this);

                if ($this.hasClass(constantClassnames.USE_SUGGESTED_ADDRESS)) {

                    $this.removeClass(constantClassnames.USE_SUGGESTED_ADDRESS);
                    $this.html($this.attr(constantAttributes.DATA_ORIGINAL_HTML_CONTENT));

                    return false;
                }
            });

            $(thisRealTimeAddressVerificationPlugin.selectorChecker(id)).children("div").removeClass(constantClassnames.IS_DIV_SUGGESTED_ADDRESS_ROW_ACTIVE);

        },

        requestAddressMatchForShipping: function (callback) {

            if (pluginIsActive == false) {
                callback();
                return false;
            }


            // this is only applicable for one page checkout (a workaround to having shipping and billing address control on one page) see  jquery.cbe.onepage.checkout.js ->
            isOnePageCheckoutShippingSection = true;

            var config = getConfig();

            var postal = thisRealTimeAddressVerificationPlugin.getInputControlValue(config.shippingInputID.POSTAL_CODE);
            var city = thisRealTimeAddressVerificationPlugin.getInputControlValue(config.shippingInputID.CITY);
            var state = thisRealTimeAddressVerificationPlugin.getInputControlValue(config.shippingInputID.STATE);
            var country = thisRealTimeAddressVerificationPlugin.getInputControlValue(config.shippingInputID.COUNTRY);
            var streetAddress = thisRealTimeAddressVerificationPlugin.getInputControlValue(config.shippingInputID.STREET_ADDRESS);

            var cityState = thisRealTimeAddressVerificationPlugin.getInputControlValue(config.shippingInputID.CITY_STATE_SELECTOR);
            var o = thisRealTimeAddressVerificationPlugin.parsedCityState(cityState, city, state);

            city = o.City;
            state = o.State;

            var successFunction = function (response) {

                if (thisRealTimeAddressVerificationPlugin.isEmpty(response.d)) {
                    callback();
                    return false;
                }

                var current = new Object();

                current.billing = {
                    StreetAddress: pluginConstants.EMPTY_VALUE,
                    City: pluginConstants.EMPTY_VALUE,
                    State: pluginConstants.EMPTY_VALUE,
                    PostalCode: pluginConstants.EMPTY_VALUE
                };

                current.shipping = {
                    StreetAddress: streetAddress,
                    City: city,
                    State: state,
                    PostalCode: postal
                };

                thisRealTimeAddressVerificationPlugin.showShippingAddressMatchResult(response, current, callback);

                current = null;

            };

            var errorFunction = function (error) {
                console.log(error);
            };

            var billingAddress = new Object();
            billingAddress.Address = pluginConstants.EMPTY_VALUE;
            billingAddress.Country = pluginConstants.EMPTY_VALUE;
            billingAddress.PostalCode = pluginConstants.EMPTY_VALUE;
            billingAddress.State = pluginConstants.EMPTY_VALUE;
            billingAddress.City = pluginConstants.EMPTY_VALUE;

            var shippingAddress = new Object();
            shippingAddress.Address = streetAddress;
            shippingAddress.Country = country;
            shippingAddress.PostalCode = postal;
            shippingAddress.State = state;
            shippingAddress.City = city;

            var data = Object()
            data.billing = JSON.stringify(billingAddress);
            data.shipping = JSON.stringify(shippingAddress);

            data.isResidence = thisRealTimeAddressVerificationPlugin.isResidentialAddress(thisRealTimeAddressVerificationPlugin.getInputControlValue(config.shippingInputID.RESIDENCE_TYPE));

            thisRealTimeAddressVerificationPlugin.displayRequestMessage(stringResource.PROGRESS_TEXT);
            thisRealTimeAddressVerificationPlugin.ajaxSecureRequest(serviceMethods.REQUEST_ADDRESS_MATCH, data, successFunction, errorFunction);

            billingAddress = null;
            shippingAddress = null;
            data = null;

        },

        showShippingAddressMatchResult: function (response, current, continueButtonCallback) {

            // this is only applicable for one page checkout (a workaround to having shipping and billing address control on one page) see  requestAddressMatchForShipping ->
            var error = thisRealTimeAddressVerificationPlugin.getAddressRequestGatewayError(response);

            if (thisRealTimeAddressVerificationPlugin.isEmpty(error) == false) {
                thisRealTimeAddressVerificationPlugin.showShippingAddressMatchResultWithGatewayError(current, continueButtonCallback, error);
               return false;
            }

            var data = JSON.parse(response.d);

            if (thisRealTimeAddressVerificationPlugin.isEmpty(data) == false) {
                var count = data.length;

                if (count == 0) {
                    continueButtonCallback();
                    return false;
                }

                if (count > 0) {

                    var content = new Object();

                    content.billingMatch = JSON.parse(data[0].Billing);
                    content.shippingMatch = JSON.parse(data[0].Shipping);
                    content.enteredBilling = current.billing;
                    content.enteredShipping = current.shipping;

                    if (count == 1 && thisRealTimeAddressVerificationPlugin.isEmpty([content.shippingMatch[0].StreetAddress,
                                                                                      content.shippingMatch[0].City,
                                                                                      content.shippingMatch[0].PostalCode,
                                                                                      content.shippingMatch[0].State])) {

                        continueButtonCallback();
                        return false;
                    }

                    var resources = new Object();

                    resources.useBillingAddressProvided = stringResource.USE_BILLING_ADDRESS_PROVIDED;
                    resources.useShippingAddressProvided = stringResource.USE_SHIPPING_PROVIDED;
                    resources.confirmCorrectAddress = stringResource.CONFIRM_CORRECT_ADDRESS;
                    resources.unableToVerifyAddress = stringResource.UNABLE_TO_VERIFY_ADDRESS;
                    resources.selectMatchingBillingAddress = stringResource.SELECT_MATCHING_BILLING_ADDRESS;
                    resources.selectMatchingShippingAddress = stringResource.SELECT_MATCHING_SHIPPING_ADDRESS;

                    content.stringResource = resources;

                    var templateId = constantsTemplateID.MULTIPLE_SHIPPING_ADDRESS_MATCH_DIALOG;

                    if (parseInt(data[0].AddressMatchResultLimit) == 1 || count == 1) {
                        templateId = constantsTemplateID.SINGLE_SHIPPING_ADDRESS_MATCH_DIALOG;
                    }

                    var dialogContentHTML = thisRealTimeAddressVerificationPlugin.parseJqueryTemplate(templateId, content);
                    $(thisRealTimeAddressVerificationPlugin.selectorChecker(config.addressMatchDialogContainerID)).html(dialogContentHTML).dialog({
                        autoOpen: false,
                        width: 700,
                        modal: true,
                        resize: false,
                        closeOnEscape: false,
                        dialogClass: constantClassnames.NO_CLOSE
                    }).dialog(constantAttributes.OPEN);

                    thisRealTimeAddressVerificationPlugin.setDefaultAddressToBeUse(constantIDs.DIV_SUGGESTED_BILLING_ADDRESS, null);
                    thisRealTimeAddressVerificationPlugin.setDefaultAddressToBeUse(constantIDs.DIV_SUGGESTED_SHIPPING_ADDRESS, null);

                    thisRealTimeAddressVerificationPlugin.attachAddressDialogControlsEvents(continueButtonCallback);
                }
            }
        },

        showShippingAddressMatchResultWithGatewayError: function (current, continueButtonCallback, error) {

            var content = new Object();
            content.enteredShipping = current.shipping;

            var resources = new Object();

            resources.useBillingAddressProvided = stringResource.USE_BILLING_ADDRESS_PROVIDED;
            resources.useShippingAddressProvided = stringResource.USE_SHIPPING_PROVIDED;
            resources.confirmCorrectAddress = stringResource.CONFIRM_CORRECT_ADDRESS;
            resources.unableToVerifyAddress = stringResource.UNABLE_TO_VERIFY_ADDRESS;
            resources.selectMatchingBillingAddress = stringResource.SELECT_MATCHING_BILLING_ADDRESS;
            resources.selectMatchingShippingAddress = stringResource.SELECT_MATCHING_SHIPPING_ADDRESS;
            resources.gatewayErrorText = stringResource.GATEWAY_ERROR_TEXT;

            content.stringResource = resources;

            var dialogContentHTML = thisRealTimeAddressVerificationPlugin.parseJqueryTemplate(constantsTemplateID.SINGLE_SHIPPING_ADDRESS_MATCH_DIALOG_GATEWAY_ERROR, content);
            $(thisRealTimeAddressVerificationPlugin.selectorChecker(config.addressMatchDialogContainerID)).html(dialogContentHTML).dialog({
                autoOpen: false,
                width: 700,
                modal: true,
                resize: false,
                closeOnEscape: false,
                dialogClass: constantClassnames.NO_CLOSE
            }).dialog(constantAttributes.OPEN);

            thisRealTimeAddressVerificationPlugin.attachAddressDialogControlsEvents(continueButtonCallback);

        }


    };

    //jqueryBasePlugin is located inside core.js
    $.extend($.fn.RealTimeAddressVerification, new jqueryBasePlugin());

    function setConfig(value) {
        config = value;
    }

    function getConfig() {
        return config;
    }

})(jQuery);