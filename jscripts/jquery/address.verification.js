var global_postal = "";

(function ($) {
    
    var methods = {
        init: function (options) {

            /* on initialization of this plugin it set default values into the options parameters 
               and calls the initEventsListener function of this plugin */

            var settings = $.extend({
                'country-id'                       :'#BillingAddressControl_drpCountry',
                'postal-id'                        :'#BillingAddressControl_txtPostal',
                'city-id'                          :'#BillingAddressControl_txtCity',
                'state-id'                         :'#BillingAddressControl_txtState',
                'city-state-place-holder'          :'.billing-zip-city-other-place-holder',
                'enter-postal-label-place-holder'  :'#billing-enter-postal-label-place-holder',
                'city-states-id'                   : 'billing-city-states'
            }, options);

            $(this).ISEAddressFinder("initEventsListener", settings);
            

            var country = $(settings['country-id']).val();
  
            $(this).ISEAddressFinder("removeErrorTagsOnAddressControl", settings);

             // Optional Postal Code: on plugin init handle optional postal code

            $(this).ISEAddressFinder("optionalPostalCodeHandler", { postalId : settings['postal-id'], 
                                                                    statePlaceHolder : settings['city-state-place-holder'],
                                                                    cityStatesId : settings['city-states-id'],
                                                                    country : country, isOnInit : true});

        },
        initEventsListener: function (settings) {

            var $this = $(this);

            $this.keypress(function (event) {
               
                var isPostalOptional = $(this).hasClass("is-postal-optional");

                if (event.which == 13) {
                    
                    // Optional Postal Code: if postal code is optional skip state , city request on ENTER

                    if(isPostalOptional){
                        return false;
                    }

                    /* note: if you have a new label id just add a new case and set your labelId on the  switch statement below: 
                       
                       On ENTER event of postal input box the following code segments does the following:
                       
                       1. Clears the value os state input box 
                       2. Remove error tag (class) state-not-found and required-input
                       3. Shows the State label for state input box
                       4. Class renderCityState function of this plugin

                    */

                    $(this).removeClass("skip-getting-city");


                    $this.ISEAddressFinder("resetPostalCodePlaceHoder", settings);
                    $this.ISEAddressFinder("renderCityState", settings);

              
                    return false;
                }

            });

            $this.blur(function () {
                
                var skip = $(this).hasClass("skip-getting-city");
                var isPostalOptional = $(this).hasClass("is-postal-optional");

                // Optional Postal Code: if postal code is optional return false to skip resetting postal code place holder and city, state request.

                if(skip || isPostalOptional) return false;

              //  $this.ISEAddressFinder("resetPostalCodePlaceHoder", settings)
                $this.ISEAddressFinder("renderCityState", settings);

            });

            $this.keyup(function (e) {
                
                /* on backspace reset postal code place holder */
                if (trimString($(this).val()) == "" && e.keyCode == 8) {
                    $this.ISEAddressFinder("resetPostalCodePlaceHoder", settings);
                }

            });

            $(settings['state-id']).keypress(function (event) {
                if (event.which == 13) {return false;}
            });

             $(settings['city-id']).keypress(function (event) {
                if (event.which == 13) {return false;}
            });

            $(settings['country-id']).change(function () {
                
                /* On Change event of Country SELECT control calls removeErrorTagsOnAddressControl function of this plugin,
                   clears postal code input box value, and trigger keyup event of postal input box.
                 */
               var country = trimString($(this).val());
               $this.ISEAddressFinder("removeErrorTagsOnAddressControl", settings);

                // Optional Postal Code: on country selector change event handle optional postal code

               $this.ISEAddressFinder("optionalPostalCodeHandler", { postalId : settings['postal-id'], 
                                                                     statePlaceHolder : settings['city-state-place-holder'],
                                                                     cityStatesId : settings['city-states-id'],
                                                                     country : country, isOnInit : false,
                                                                     params: settings});



               return false;
            });

        },
        
        resetPostalCodePlaceHoder : function(settings){

            /*
                1. Verifies if city state SELECT control is on display
                2. Clears the value fo State and City input box
                3. Remove error tags (class) state-not-found and required-input from state input box 
                4. Remove error tag (class) required-input from city input box 
            */
                if(  $(settings['postal-id']).hasClass("is-postal-optional") ) return false;

                var country = $(settings['country-id']).val();
                var postal  = $(settings['postal-id']).val();

                var params = [postal, settings['city-state-place-holder'], settings['enter-postal-label-place-holder'],  settings['postal-id']];
                $this.ISEAddressFinder("isCityStatesVisible", params);

                $(settings["state-id"]).val("");
                $(settings["city-id"]).val("");
                   
                $(settings["state-id"]).removeClass("state-not-found");
                $(settings["state-id"]).removeClass("required-input");
                $(settings["city-id"]).removeClass("required-input");

                var cityLabelId = "";
                var stateLabelId = "";

                switch(settings["city-id"]){
                    case "#BillingAddressControl_txtCity":
                    cityLabelId = "#BillingAddressControl_lblCity";   
                    stateLabelId  = "#BillingAddressControl_lblState";   
                    break;   
                    case "#ShippingAddressControl_txtCity":
                    cityLabelId = "#ShippingAddressControl_lblCity";     
                    stateLabelId  = "#ShippingAddressControl_lblState";   
                    break;
                    default:
                    cityLabelId = "#AddressControl_lblCity";  
                    stateLabelId  = "#AddressControl_lblState";   
                         
                    break;
                }

                $(cityLabelId).removeClass("display-none");
                $(stateLabelId).removeClass("display-none");
        },

        optionalPostalCodeHandler : function(o){

            var $postalInputBox  = $(o.postalId);
            var country = $.trim(o.country);
      
            if(typeof($postalInputBox) == "undefined" || typeof(country) == "undefined" || country == ""){
                return false;
            }

            var postalIsOptionalIn =  $postalInputBox .attr("data-postalIsOptionalIn");

            var isPostalOptional = ($(this).ISEAddressFinder("isPostalCodeOptional", {'postalIsOptionalIn' : postalIsOptionalIn, 'country': country}));
            
            var postalLabelText = $postalInputBox.attr("data-labeltext");
            postalLabelText = $.trim(postalLabelText); 

            var addressType = "";      
            var enterPostalPlaceHolder = "enter-postal-label-place-holder";
            var postaLableId = "AddressControl_lblPostal";

            if( o.cityStatesId == "billing-city-states"){
                addressType = "Billing";       
                enterPostalPlaceHolder = "billing-enter-postal-label-place-holder";        
            }

            if( o.cityStatesId == "shipping-city-states" ){
                addressType = "Shipping";      
                enterPostalPlaceHolder = "shipping-enter-postal-label-place-holder";          
            }
            
            if(o.isOnInit == false){
                $postalInputBox.val("");
            }

            $("#" + addressType + postaLableId).removeClass("display-none");

            $postalInputBox.attr("data-isOptional", isPostalOptional);
            if(isPostalOptional){  
                
                $postalInputBox.parent("span").children("label").html(postalLabelText + " (optional)");
                $postalInputBox.addClass("is-postal-optional");

                $("#" + enterPostalPlaceHolder).removeClass("enter-postal-message-width");
       
                SkipValidationOnPostal(addressType, false); 

            }else{

                $postalInputBox.parent("span").children("label").html(postalLabelText);
                $postalInputBox.removeClass("is-postal-optional");
                
                if(o.isOnInit == false){
                    $(this).ISEAddressFinder("resetPostalCodePlaceHoder", o.params);
                   
                }
               
            }

        },

        isPostalCodeOptional: function(o){

           var postalIsOptionalIn = $.trim(o.postalIsOptionalIn);
           var country = $.trim(o.country);

           var optionals = postalIsOptionalIn.split(",");

           if (country == "") {
               return false;
           }

           if(optionals.length > 0){
                return (IsInArray(optionals, country));
           }else{
                return (country == postalIsOptionalIn)
           }

           return false;
        },

        isCityStatesVisible: function (params) {
            
            /* This function verifies if city-state place SELECT control is undefined, 
               if true then place the "Enter Postal for City and State" string on the place holder
               of city-state SELECT control.*/

            var $this = $(this);

            var citystateIsVisible = $(params[1]).css("display");
            var citystateIsVisible = citystateIsVisible.toLowerCase();

            if (params[0] == "" && (citystateIsVisible != "none" || citystateIsVisible != "")) {

                $(params[1]).fadeOut("Slow", function () {
        
                    $(params[2]).html($.trim(ise.StringResource.getString("customersupport.aspx.40")));
                    $(params[2]).addClass("enter-postal-message-width");
                    $(params[3]).removeClass("skip");

                    $("#ise-message-tips").fadeOut("fast");
                });

                return false;
            }else{
             $(params[2]).removeClass("enter-postal-message-width");
            }


            if (citystateIsVisible != "none" || citystateIsVisible == "") return false;

            return true;
        },
        isPostalFormatInvalid: function (options) {

            var params = $.extend({
                'country': '',
                'postal' : ''
            }, options);

            var $this = $(this);
  
            var postalFormat = $this.ISEAddressFinder("getCountryPostalFormats", params['country']);
            var postalCode = params['postal'];

            var formats = postalFormat;

            formats = formats.split("-")

            if (formats.length > 0 && postalFormat != "free-form") {

                var postal    = postalCode.split("-");
                global_postal = postal[0];
 
                if (postal.length > 1) {

                    /*Check if the user postal input number of elements separated by hypen(-) is the 
                      same as your defined number of digits. */

                    if (postal.length != formats.length) {

                        return true;

                    } else {

                        /* loops through your postal elements separated  by hypen(-)
                        -> and verify if each element in user postal has the same length with each element in your postal format.*/

                        for (var i = 0; i < postal.length; i++) {

                            var userPostalLength = postal[i].length;
                            var yourPostalLength = formats[i].length;

                            if ((userPostalLength != yourPostalLength) || userPostalLength == 0) {
                                return true;
                                break;
                            }
                        }
                    }
                } else {

                    global_postal = postalCode;
                    if (postalCode.length == 0 || postalCode.length != formats[0].length) return true;

                }

            } else {

                global_postal = postalCode;

            }

            return false;

        },
        isCountrySearchable: function (options) {

            var params = $.extend({
                'country-id'    : '',
                'selected-index': ''
            }, options);

            var thisClass = $(params['country-id']).attr("class");

            if(typeof(thisClass) == "undefined" || thisClass == ""){
                return false;
            }

            var classes = thisClass.split(" ");
            var countryStatesFlag = classes[0];
            var withStates = countryStatesFlag.split("-");

            var flag = withStates[params['selected-index']];
            flag = flag.split("::");

            if(flag.length <= 1){
                return false;
            }

            var status = flag[1];

            if(typeof(status) == "undefined" || status == ""){
                return false;
            }

            if (status.toLowerCase() == "false") return false;

            return true;

        },
        isCountryHasState: function (options) {

            var params = $.extend({
                'country-id'    : '',
                'selected-index': ''
            }, options);

            var thisClass = $(params['country-id']).attr("class");

            var classes = thisClass.split(" ");
            var countryStatesFlag = classes[0];
            var withStates = countryStatesFlag.split("-");

            var flag = withStates[params['selected-index']];
            flag = flag.split("::");

            if(flag.length < 1){
                return false;
            }

            var status = flag[0];

            if (status.toLowerCase() == "false") return false;

            return true;

        },
        getCountryPostalFormats: function (country) {
            
            // static - temporary solution for country postal format

            var names = new Array();
            names.push("united states of america");

            var digits = new Array();
            digits.push("12345-6789");
            
            var index = 0;

            if (country.toLowerCase() != "united states of america") {
                return "free-form";
            }

            return digits[index];

        },
        highlightsError: function (options) {

            var params = $.extend({
                'control-id'                   : '',
                'error-type'                   : '',
                'focus'                        : false,
                'enter-caption-place-holder-id': ''
            }, options);

            switch (params['error-type']) {
                case "invalid-postal-format":

                    $(params['control-id']).addClass('invalid-postal');
                    $(params['enter-caption-place-holder-id']).html(ise.StringResource.getString("customersupport.aspx.40"));

                    break;
                default:
                    break;
            }


        },
        bindCityStateOnChange: function (options) {

            var params = $.extend({
                'city-states-id'         : 'billing-city-states',
                'city-state-place-holder': '.zip-city-other-place-holder'
            }, options);

            $("#" + params['city-states-id']).change(function () {
                var $this = $(this);

                if ($this.val().toLowerCase() == "other") {
                    $this.fadeOut("Slow", function () {
                        $(params['city-state-place-holder']).fadeIn("Slow");

                        var selectedState = $("#" + params["city-states-id"] + " option:first").val().split(",");
                        
                        if(selectedState.length > 1){
                            selectedState = $.trim(selectedState[0]);
                        }else{
                            selectedState = "";
                        }

                        $this.ISEAddressFinder("displayCityStateInputBox", params["city-states-id"], selectedState);

                    });

                }
            });

        },
        displayCityStateInputBox: function (id, selectedState){

            var cityInputId  = "#AddressControl_txtCity";
            var stateInputId = "#AddressControl_txtState";
            var stateLabelId = "#AddressControl_lblState";
            var controlType  = "AddressControl";
             
            if( id == "billing-city-states"){
                            
                cityInputId  = "#BillingAddressControl_txtCity";
                stateInputId = "#BillingAddressControl_txtState";
                stateLabelId = "#BillingAddressControl_lblState";
                controlType  = "BillingAddressControl";
            }

            if( id == "shipping-city-states" ){

                cityInputId  = "#ShippingAddressControl_txtCity";
                stateInputId = "#ShippingAddressControl_txtState";
                stateLabelId = "#ShippingAddressControl_lblState";
                controlType  = "ShippingAddressControl";
            }

            $(cityInputId).removeClass("city-width-if-no-state");
            $(cityInputId).removeClass("required-input");

            $(stateInputId).removeAttr("disabled");
            $(stateInputId).removeClass("display-none");
            $(stateInputId).removeClass("control-disabled");

            if(selectedState != ""){
               $(stateInputId).val(selectedState);
               $(stateLabelId).addClass("display-none");
            }else{
               $(stateInputId).val("");
               $(stateLabelId).removeClass("display-none");
            }

            $(stateInputId).addClass("skip");
            HideStateInputBoxForCountryWithState(controlType);

        },
        renderCityState: function (settings) {

            var $this = $(this);

            var country = $(settings['country-id']).val();
            var postal  = $(settings['postal-id']).val();

            var params  = [postal, settings['city-state-place-holder'], settings['enter-postal-label-place-holder'], settings['postal-id']];

            if ($this.ISEAddressFinder("isCityStatesVisible", params)) {

                var formatIsInvalid = $this.ISEAddressFinder("isPostalFormatInvalid", { 'country': country, 'postal': postal });
    
                if (!formatIsInvalid) {
                    $(settings['postal-id']).removeClass('invalid-postal');
                    $this.ISEAddressFinder("searchForCityAndState", settings);

                } else {
                    $this.ISEAddressFinder("highlightsError", { "control-id": settings['postal-id'], "error-type": "invalid-postal-format", "focus": true, "enter-caption-place-holder-id": settings['enter-postal-label-place-holder'] });
                }
            }

        },
        searchForCityAndState: function (settings) {

            var successFunction = function (result) {

                $(settings["postal-id"]).removeClass("current-object-on-focus");
                if (result.d != "" && result.d != "no-active-postal") {
                        
                    var renderHTML = "<select id='" + settings['city-states-id'] + "' class='light-style-input'>";

                    renderHTML += result.d;
                    renderHTML += "<option value='other'>" + ise.StringResource.getString("customersupport.aspx.41") + "</option>";
                    renderHTML += "</select>";

                    $(settings['city-state-place-holder']).fadeOut("Slow", function () {
                        $(settings['enter-postal-label-place-holder']).html(renderHTML);
                    });
           
                    $(settings['postal-id']).removeClass("invalid-postal-zero");
                    $(settings['postal-id']).removeClass("undefined-city-states");
                    $("#ise-message-tips").fadeOut("slow");

                    $(this).ISEAddressFinder("bindCityStateOnChange", { "city-states-id": settings['city-states-id'], "city-state-place-holder": settings['city-state-place-holder'] });

                } else {
                        
                    /* if postal is not found the following code segments does:
                    1. verify if country is searchable: 
                    2. if #1 is true: highlights postal inputs
                    3. if #1 is false: show hidden controls: city and state */

                    var _SelectedIndex = $(settings['country-id']).prop("selectedIndex");
                         
                    if ($this.ISEAddressFinder("isCountrySearchable", { "country-id": settings['country-id'], "selected-index": _SelectedIndex }) && result.d != "no-active-postal" && !$(settings["postal-id"]).hasClass("skip")) { 
                       
                        $(settings['enter-postal-label-place-holder']).html(ise.StringResource.getString("customersupport.aspx.40")); 
                        $(settings['postal-id']).addClass("invalid-postal-zero");
                    
                     }else{
                            
                        var citystates = $(settings["city-states-id"]).val();

                        if (typeof (citystates) == "undefined") {

                            $(settings["city-state-place-holder"]).fadeIn("Slow");
                            var cityStatesId = "";
                            var stateLabelId = "";

                            switch(settings["state-id"]){
                                case "#ShippingAddressControl_txtState":
                                    citystatesId = "shipping-city-states";
                                    stateLabelId = "ShippingAddressControl_lblState";
                                    break;
                                case "#BillingAddressControl_txtState":
                                    citystatesId = "billing-city-states";
                                    stateLabelId = "BillingAddressControl_lblState";
                                    break;
                                default:
                                    citystatesId = "city-states";
                                    stateLabelId = "AddressControl_lblState";
                                    break;
                                }

                            $(settings["enter-postal-label-place-holder"]).html("<input type='hidden' id='" + citystatesId + "' value='other'/>");

                            if($this.ISEAddressFinder("isCountryHasState", { "country-id": settings['country-id'], "selected-index": _SelectedIndex })){
                                   
                                $(settings["state-id"]).removeClass("display-none");
                                $(settings["city-id"]).removeClass("city-width-if-no-state");
                                    
                            }else{
                                    
                                $("#" + stateLabelId).addClass("display-none");

                                $(settings["state-id"]).val("");
                                $(settings["state-id"]).addClass("display-none");  
                                
                                $(settings["city-id"]).addClass("city-width-if-no-state");
                                $(settings["city-id"]).focus();
                            }

                        } else {

                            $(settings["city-states-id"]).fadeOut("Slow", function () { $(settings["city-state-place-holder"]).fadeIn("Slow"); });
                    
                        }              
                }
             }
            };

            var errorFunction = function (result) {  console.log(result.d); };

            var data = new Object();
            data.countryCode = $(settings['country-id']).val();
            data.postalCode = global_postal;
            data.stateCode = $(settings['state-id']).val();

            AjaxCallCommon("ActionService.asmx/GetCity", data, successFunction, errorFunction);

        },
        verifyStateCode: function (settings) {

            var successFunction = function (result) {

                if (result.d == false) {
                    $(settings['state-id']).addClass("state-not-found");
                } else {
                    $(settings['state-id']).removeClass("state-not-found");
                }

                return result.d;
            };

            var errorFunction = function () { return 0 };

            var data = new Object();
            data.countryCode =  $(settings['country-id']).val();
            data.postalCode = $(settings['postal-id']).val();
            data.stateCode = $(settings['state-id']).val();

            AjaxCallCommon("ActionService.asmx/IsStateCodeValid", data, successFunction, errorFunction);

        },
        lookForCorrectPostal: function ( options ){
            
              var params = $.extend({
                'country'   : '',
                'state'     : '',
                'postal'    : '',
                'city-id'   : '',
                'postal-id' : '',
                'state-id'  : ''
            }, options);

             _IsAddressVerificationAtShippingPostal = true;
     
             showPostalSearchEngineDialog(postalCode, state, country, 1);
             addressDialogControlsInit();

        }, 
        removeErrorTagsOnAddressControl: function (settings) {

            $(settings["city-id"]).removeClass("required-input");
            $(settings["city-id"]).removeClass("current-object-on-focus");

            $(settings["state-id"]).removeClass("required-input");
            $(settings["state-id"]).removeClass("state-not-found");
            $(settings["state-id"]).removeClass("current-object-on-focus");


            $(settings["postal-id"]).removeClass("required-input");
            $(settings["postal-id"]).removeClass("invalid-postal");
            $(settings["postal-id"]).removeClass("current-object-on-focus");

            $(settings["postal-id"]).removeClass("state-not-found");
            $(settings["postal-id"]).removeClass("invalid-postal-zero");

            $(settings["postal-id"]).removeClass("state-not-found");
            $(settings["postal-id"]).removeClass("undefined-city-states");
            $(settings["postal-id"]).removeClass("skip-getting-city");

            $("#ise-message-tips").fadeOut("slow");

        }
    };


    $.fn.ISEAddressFinder = function (method) {

        if (methods[method]) {

            return methods[method].apply(this, Array.prototype.slice.call(arguments, 1));

        } else if (typeof method === 'object' || !method) {

            return methods.init.apply(this, arguments);

        } else {

            $.error('Method ' + method + ' does not exist on jQuery.tooltip');

        }

    };

})(jQuery);

// !IMPORTANT :  Temporary transfer validation into this file in preparation of js refractoring after 14.0

function VerifyUserAddress(params){

    if (params.isCreateAccount) {

        RequestAddressMatch(params);
        return false;
    }

}

function ParsedCityState(cityState, city, state){
    
    var returnData = new Object();
    returnData.City = city;
    returnData.State = state;

    if (cityState != "other") {

        var str = cityState;
        str = str.split(",");

        if (str.length > 1) {

            returnData.State = str[0];
            returnData.City = str[1];

        } else {
            
            returnData.State = "";
            returnData.City  = str[0];
           
        }
    }

    return returnData;
}


function showSuggestedAddressDialog(params) {

    // prepare html content for billing address panel

    var classDisabled = "control-caption-disabled";
    var classHide = "display-none";
    var classError = "error-place-holder";

    if (params.billing.isNew) {

        $newBillingAddress.removeClass(classDisabled);
        $newBillingCityStatePostal.removeClass(classDisabled);

        $useSuggestedBilling.removeClass(classDisabled).removeAttr("disabled");

    } else {

        $newBillingAddress.addClass(classDisabled);
        $newBillingCityStatePostal.addClass(classDisabled);

        $useSuggestedBilling.addClass(classDisabled).attr("disabled", "disabled");

    }


    // prepare html content for shipping address panel

  
    if (params.shipping.isNew) {

        $newShippingAddress.removeClass(classDisabled);
        $newShippingCityStatePostal.removeClass(classDisabled);

        $useSuggestedShipping.removeClass(classDisabled).removeAttr("disabled");

    } else {

        $newShippingAddress.addClass(classDisabled);
        $newShippingCityStatePostal.addClass(classDisabled);

        $useSuggestedShipping.addClass(classDisabled).attr("disabled", "disabled");
    }
    
    // popup modal dialog

    var $radioButtonEnteredBilling = $("#use-my-billing");
    var $radioButtonEnteredShipping = $("#use-my-shipping");


    var $radioButtonSuggestedBilling = $("#use-suggested-billing");
    var $radioButtonSuggestedShipping = $("#use-suggested-shipping");

    if (params.isError) {

        $newBillingAddress.html(params.gatewayError).addClass(classError);
        $newShippingAddress.html(params.gatewayError).addClass(classError);

        $useSuggestedBilling.parent("li").addClass(classHide);
        $useSuggestedShipping.parent("li").addClass(classHide);

        $radioButtonEnteredBilling.attr("checked", "checked");
        $radioButtonEnteredShipping.attr("checked", "checked");

    } else {

    }

    var copySelectedAddress = function () {

        var useSuggestedBillingAddress = ($("input[name='billing-address']:checked").attr("id") == "use-suggested-billing");
        var useSuggestedShippingAddreess = ($("input[name='shipping-address']:checked").attr("id") == "use-suggested-shipping");


        if (!params.isError && useSuggestedBillingAddress) {


        } else {



        }

        if (!params.isError && useSuggestedShippingAddreess) {


        } else {

        }

       
        params.submit();

    }



}


function GetAddressRequestGatewayError(result) {
    var test = result.d.split("[error]");
    if (test.length > 1) return $.trim(test[1]);
    return "";
}

function IsPostalInputBoxOptional(id) {
    return (id != "BillingAddressControl_txtPostal" || id != "ShippingAddressControl_txtPostal" || id == "AddressControl_txtPostal") && $("#" + id).hasClass("is-postal-optional");
}

function IsWithStates(id) {
    
    var $control = $("#" + id);

    var thisClass = $control.attr("class");
    var status = "";

    if (typeof (thisClass) != "undefined") {

        var classes = thisClass.split(" ");
        var countryStatesFlag = classes[0];
        var withStates = countryStatesFlag.split("-");
        var flag = withStates[$control.prop("selectedIndex")];
        flag = flag.split("::");

        if (flag.length < 1) {
            return false;
        }

        status = flag[0];
    }

    if (typeof (status) == "undefined" || status == "") {
        return true;
    }

    return status.toLowerCase() == "true";
}

function IsSearchable(id) {

    var $control = $("#" + id);

    var thisClass = $control.attr("class");

    if (typeof (thisClass) == "undefined" || thisClass == "") {
        return false;
    }

    var classes = thisClass.split(" ");
    var countryStatesFlag = classes[0];
    var withStates = countryStatesFlag.split("-");
    var flag = withStates[$control.prop("selectedIndex")];
    flag = flag.split("::");

    if (flag.length <= 1) {
        return false;
    }

    var status = flag[1];

    if (typeof (status) == "undefined" || status == "") {
        return false;
    }

    return status.toLowerCase() == "true";
}

function IsResidentialAddress(value){

   if(typeof(value)=="undefined" || value == ""){
    return false;
   }

   return value.toLowerCase() == "residential";

}
