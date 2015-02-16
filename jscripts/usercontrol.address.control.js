var _AddressBestMatchFound = "";

$(document).ready(function () {

    RealTimAddressVerificationEventHandler();

    HideStateInputBoxForCountryWithState("AddressControl");
    HideStateInputBoxForCountryWithState("ShippingAddressControl");
    HideStateInputBoxForCountryWithState("BillingAddressControl");

    var param = new Object();
    param.countryCode = "";
    param.postalCode = "";
    param.stateCode = "";

    AjaxCallCommon("ActionService.asmx/GetCity", param, function () { }, function (error) { });

});

function RealTimAddressVerificationEventHandler() {

    $("#select-entered-address").click(function () {

        var values = _AddressBestMatchFound.split("::");

        var task = values[4];
        var onSubmit = values[5];
        var idPrefix = values[6];

        if (onSubmit) ISEAddressVerificationTaskSelector(task);

    });

    $("#select-matching-address").click(function () {

        var values = _AddressBestMatchFound.split("::");

        var nAddress = values[0];
        var nPostal = values[1];
        var nCity = values[2];
        var nState = values[3];
        var task = values[4];
        var onSubmit = values[5];

        var idPrefix = values[6];

        if (nAddress && nPostal && nCity && nState) {
            $("#" + idPrefix + "AddressControl_txtStreet").val(nAddress);
            $("#" + idPrefix + "AddressControl_txtPostal").val(nPostal);
            $("#" + idPrefix + "AddressControl_txtCity").val(nCity);
            $("#" + idPrefix + "AddressControl_txtState").val(nState);
        }

        if (onSubmit) ISEAddressVerificationTaskSelector(task);

    });
}

function RenderCityStates(excludeStateCode, focusOnControl, idPrefix) {

    var addressControlId   = "#" + idPrefix + "AddressControl_drpCountry";
    var postalCodeId       = "#" + idPrefix + "AddressControl_txtPostal";
    var stateCodeId        = "#" + idPrefix + "AddressControl_txtState";

    var cityStatesId       = "#city-states";
    var cityStateClassName = ".zip-city-other-place-holder";
    var enterPlaceHolderId = "#enter-postal-label-place-holder";

    if (idPrefix != "") {

        cityStatesId       = "#" + idPrefix.toLowerCase() + "-city-states";
        cityStateClassName = "." + idPrefix.toLowerCase() + "-zip-city-other-place-holder";
        enterPlaceHolderId = "#" + idPrefix.toLowerCase() + "-enter-postal-label-place-holder";
    }

    var citystateIsVisible = $(cityStateClassName).css("display");
    citystateIsVisible = citystateIsVisible.toLowerCase();

    if ($(postalCodeId).val() == "" && (citystateIsVisible != "none" || citystateIsVisible != "")) {

        $(cityStateClassName).fadeOut("Slow", function () {

            $(enterPlaceHolderId).html(ise.StringResource.getString("customersupport.aspx.40"));
            $("#ise-message-tips").fadeOut("fast");

        });

        return false;
    }

    if (citystateIsVisible != "none" || citystateIsVisible == "") return false;

    var country = $(addressControlId).val()
    var postalCode = $(postalCodeId).val();
    var stateCode = $(stateCodeId).val();

    if (excludeStateCode) stateCode = "";

    if (IsPostalFormatInvalid(country, postalCode)) {

        $(postalCodeId).addClass('support-invalid-postal');
        $(enterPlaceHolderId).html(ise.StringResource.getString("customersupport.aspx.40"));

        if (focusOnControl) $(postalCodeId).focus();

    } else {

        $(postalCodeId).removeClass('support-invalid-postal');
        SearchForCityAndState(country, _PostalCode, stateCode, focusOnControl, idPrefix);

    }

}

function SearchForCityAndState(country, postalCode, stateCode, focusOnControl, idPrefix) {

    var successFunction = function (result) {

        var addressControlId = "#" + idPrefix + "AddressControl_drpCountry";
        var postalCodeId = "#" + idPrefix + "AddressControl_txtPostal";
        var stateCodeId = "#" + idPrefix + "AddressControl_txtState";

        var cityStatesId = "city-states";
        var cityStateClassName = ".zip-city-other-place-holder";
        var enterPlaceHolderId = "#enter-postal-label-place-holder";

        if (idPrefix != "") {
            cityStatesId = idPrefix.toLowerCase() + "-city-states";
            cityStateClassName = "." + idPrefix.toLowerCase() + "-zip-city-other-place-holder";
            enterPlaceHolderId = "#" + idPrefix.toLowerCase() + "-enter-postal-label-place-holder";
        }

        if (result.d != "") {

            var renderHTML = "<select id='" + cityStatesId + "' class='light-style-input'>";
            renderHTML += result.d;
            renderHTML += "<option value='other'>" + ise.StringResource.getString("customersupport.aspx.41") + "</option>";
            renderHTML += "</select>";

            if (result.d.length > 0) {

                $(cityStateClassName).fadeOut("Slow", function () {

                    $(enterPlaceHolderId).html(renderHTML);

                });

            } else {

                $(enterPlaceHolderId).html(ise.StringResource.getString("customersupport.aspx.40"));
                if (focusOnControl) $(postalCodeId).focus();
            }

            $("#" + cityStatesId + "").change(function () {

                if ($(this).val() == "other") {

                    $(this).fadeOut("Slow", function () {

                        $(cityStateClassName).fadeIn("Slow")

                    });

                    HideStateInputBoxForCountryWithState(idPrefix + "AddressControl");

                }
            });

            $(postalCodeId).removeClass("invalid-postal-zero");

        } else {

            $(enterPlaceHolderId).html(ise.StringResource.getString("customersupport.aspx.40"));
            $(postalCodeId).addClass("invalid-postal-zero");

            if (focusOnControl) $(postalCodeId).focus();
        }
    };

    var errorFunction = function () { };

    var data = new Object();
    data.countryCode = country;
    data.postalCode = postalCode;
    data.stateCode = stateCode;

    AjaxCallCommon("ActionService.asmx/GetCity", data, successFunction, errorFunction);
}

function IsCountryWithStates(addressControlId) {

    var thisClass = $(addressControlId).attr("class");
    var status = "";

    if (typeof (thisClass) != "undefined") {

        var classes = thisClass.split(" ");
        var countryStatesFlag = classes[0];
        var withStates = countryStatesFlag.split("-");
        var flag = withStates[$(addressControlId).prop("selectedIndex")];
        flag = flag.split("::");

        status = flag[0];
    }

    return status.toLowerCase();

}

function IsCountrySearchable(addressControlId) {

    var thisClass = $(addressControlId).attr("class");

    var classes = thisClass.split(" ");
    var countryStatesFlag = classes[0];
    var withStates = countryStatesFlag.split("-");
    var flag = withStates[$(addressControlId).prop("selectedIndex")];
    flag = flag.split("::");

    var status = flag[1];

    return status.toLowerCase();

}

function VerifyPostalCode(country, postalCode, focusOnControl, skipStateValidation, doSumbissionIfGood, postalCodeId, page) {

    if ($(postalCodeId).hasClass("skip")) {
        $(postalCodeId).removeClass("invalid-postal-zero");
        PageSubmitProcess(page);
        return true;
    }

    var successFunction = function (result) {
        if (result.d != false) {

            $(postalCodeId).removeClass("invalid-postal-zero");
            if (doSumbissionIfGood == true && IsAddressConrolBad() == 0) {
                PageSubmitProcess(page);
            }

        } else {

            $(postalCodeId).addClass("invalid-postal-zero");
            if (focusOnControl) $(postalCodeId).focus();

        }
    };

    var errorFunction = function () { };

    var data = new Object();
    data.countryCode = country;
    data.postalCode = postalCode;
    data.stateCode = "";

    AjaxCallCommon("ActionService.asmx/IsPostalCodeValid", data, successFunction, errorFunction);

}

function VerifyStateCode(country, postalCode, stateCode, doSumbissionIfGood, page, stateCodeId) {

    var successFunction = function (result) {

        if (result.d == false) {

            $(stateCodeId).addClass("state-not-found");
            if (doSumbissionIfGood == true) {
                $(stateCodeId).addClass("current-object-on-focus");
                $(stateCodeId).focus();
            }

        } else {

            $(stateCodeId).removeClass("state-not-found");
            if (doSumbissionIfGood == true) PageSubmitProcess(page);
        }

        return result.d;
    };

    var errorFunction = function () { return 0 };

    var data = new Object();
    data.countryCode = country;
    data.postalCode = postalCode;
    data.stateCode = stateCode;

    AjaxCallCommon("ActionService.asmx/IsStateCodeValid", data, successFunction, errorFunction);

}

function IsShippingStateCodeIsGood(country, postalCode, stateCode) {

    var successFunction = function (result) {
        if (result.d == "0") {
            $("#shipping-states-input").addClass("support-state-not-found");
            return false;
        } else {
            $("#shipping-states-input").removeClass("support-state-not-found");
            return true;
        }
    };

    var errorFunction = function () { return false; };

    var data = new Object();
    data.countryCode = country;
    data.postalCode = postalCode;
    data.stateCode = stateCode;

    AjaxCallCommon("ActionService.asmx/GetCity", data, successFunction, errorFunction)

}

function GetCountryPostalFormats(country, postalCode) {

    var names = new Array();
    names.push("united states of america");

    var digits = new Array();
    digits.push("12345-6789");

    var index = 0;

    if (country.toLowerCase() != "united states of america") {
        return "free-form";
    }

    return digits[index];

}

function IsPostalFormatInvalid(country, postalCode) {

    var postalFormat = GetCountryPostalFormats(country, postalCode);
    var formats = postalFormat;

    formats = formats.split("-")

    if (formats.length > 0 && postalFormat != "free-form") {

        var postal = postalCode.split("-");
        _PostalCode = postal[0];

        if (postal.length > 1) {

            /*
                           
            Check if the user postal input number of elements separated by hypen(-) is the 
            same as your defined number of digits
                           
            */

            if (postal.length != formats.length) {

                return true;

            } else {

                /* 
                loops through your postal elements separated  by hypen(-)

                -> verify if each element in user postal has the same length with each element in your postal format
                              
                */

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

            _PostalCode = postalCode;
            if (postalCode.length == 0 || postalCode.length != formats[0].length) return true;

        }


    } else {

        _PostalCode = postalCode;

    }

    return false;
}


function ValidateAddressDetails(focusOnControl, formSubmission, skipStateValidation, idPrefix, page) {

    /*
    Function definition:

    1. Validates address (billing or shipping <if IsUsedInShippingControl is set to TRUE>)
    2. If formSubmission is SET TO true does PageSubmitProcess after validating the Address
    3. Checks if Country is SEARCHABLE (if true do address validation else: jump to PageSubmitProcess())

    4. If postal is good and county is SEARCHABLE function does the following

    -> checks if cityState is equals to "other" and if skipStateValidation is set to TRUE: call VerifyPostalCode()
    -> if skipStateValidation is set to FALSE do VerifyStateCode
    -> if citystate not equals to "other" and formSubmission is set false calls RenderCityStates()
       
    */

    var addressControlId   = "#" + idPrefix + "AddressControl_drpCountry";
    var postalCodeId       = "#" + idPrefix + "AddressControl_txtPostal";
    var statesId           = "#" + idPrefix + "AddressControl_txtState";
    var stateCodeId        = "#" + idPrefix + "AddressControl_txtState";

    var county             = "#" + idPrefix + "AddressControl_txtCounty";

    var cityStatesId       = "#city-states";
    var cityStateClassName = ".zip-city-other-place-holder";
    var enterPlaceHolderId = "#enter-postal-label-place-holder";

    if (idPrefix != "") {

        cityStatesId       = "#" + idPrefix.toLowerCase() + "-city-states";
        cityStateClassName = "." + idPrefix.toLowerCase() + "-zip-city-other-place-holder";
        enterPlaceHolderId = "#" + idPrefix.toLowerCase() + "-enter-postal-label-place-holder";
    }

    var citystateIsVisible = $(cityStateClassName).css("display");
    citystateIsVisible = citystateIsVisible.toLowerCase();

    var country = $(addressControlId).val();
    var postalCode = $(postalCodeId).val();

    var postalIsGood = true;

    // check format
    var isOptionalPostalCode = $(postalCodeId).hasClass("is-postal-optional") && postalCode == "";

    if (IsCountrySearchable(addressControlId) == "true" && isOptionalPostalCode == false) {

        if (IsPostalFormatInvalid(country, postalCode)) {

            $(postalCodeId).addClass('invalid-postal'); 

            // hide citystates dropdown only if city and state inputs is hidden

            if (citystateIsVisible == "none") {
                $(enterPlaceHolderId).html(ise.StringResource.getString("customersupport.aspx.40"));
            }

            if (focusOnControl) $(postalCodeId).focus();
            return false;
        }

    } else {

        PageSubmitProcess(page);
        return true;
    }

   
 
    if (postalIsGood) {

        /* 

        -> if CityState dropdown box is not on display and City and State inputs are hidden 
        -> select city state based on user postal inputs
                            
        otherwise:

        */

        _prevPostal = _currentPostal;
        _currentPostal = postalCode;

        var cityState = $(cityStatesId).val();

        if (cityState == "other") {

            if (skipStateValidation) {
               
                VerifyPostalCode(country, _PostalCode, focusOnControl, skipStateValidation, formSubmission, postalCodeId, page);

            } else {

                var stateCode = $(statesId).val();
                VerifyStateCode(country, _PostalCode, stateCode, true, page, stateCodeId);
            }


        } else {

            if ((citystateIsVisible == "none" && formSubmission == false) && (typeof (cityState) == "undefined" || (_prevPostal != _currentPostal))) {

                RenderCityStates(true, focusOnControl, idPrefix);

            }

            if (formSubmission == true) {

                VerifyPostalCode(country, _PostalCode, focusOnControl, skipStateValidation, formSubmission, postalCodeId, page);

            }

        }

    }

}

function IsAddressConrolBad() {

    var postalZero    = $(".invalid-postal-zero").length;
    var postalInvalid = $(".invalid-postal").length;
    var stateInvalid  = $(".state-not-found").length;
    var emptyFields   = $(".required-input").length;

    return postalZero + postalInvalid + stateInvalid + emptyFields;
}

function PageSubmitProcess(page) {

    switch (page) {
        case "case-form":

            var progress = ["errorSummary", "save-case-loader", "save-case-button-place-holder"];
            ISEAddressVerification(true, "case-form", "", false, progress);

            break;
        case "lead-form":

            var progress = ["errorSummary", "save-lead-loader", "save-lead-button-place-holder"];
            ISEAddressVerification(true, "lead-form", "", false, progress);

            break;
        case "add-address":

            var progress = ["errorSummary", "save-address-loader", "save-address-button-place-holder"];
            ISEAddressVerification(true, "add-address", "", false, progress);

            break;
        case "update-address":

            var progress = ["errorSummary", "update-address-loader", "update-address-button-place-holder"];
            ISEAddressVerification(true, "update-address", "", false, progress);

            break;
        case "shipping-calculator":
            
            DoSubmissionOfShippingCalculatorAction();

            break;
        default:
            break;
    }

}

function GetAddressVerificationGatewayError(result) {
    var test = result.d.split("[error]");
    if (test.length > 1) return $.trim(test[1]);
    return "";
}

function ClearAddressControl() {

    $("#shipping-address-input").val("");
    $("#shipping-postal-code-input").val("");
    $("#shipping-city-input").val("");
    $("#shipping-states-input").val("");

    $("#shipping-address-input-label").removeClass("display-none");
    $("#shipping-postal-code-input-label").removeClass("display-none");
    $("#shipping-city-input-label").removeClass("display-none");
    $("#shipping-states-input-label").removeClass("display-none");

    $("#shipping-address-input-label").css("color", labelFadeInColor);
    $("#shipping-postal-code-input-label").css("color", labelFadeInColor);
    $("#shipping-city-input-label").css("color", labelFadeInColor);
    $("#shipping-states-input-label").css("color", labelFadeInColor);

    $("#shipping-address-input").removeClass("support-required-input");
    $("#shipping-city-input").removeClass("support-required-input");
    $("#shipping-states-input").removeClass("support-required-input");
    $("#shipping-postal-code-input").removeClass("support-required-input");

    $("#shipping-postal-code-input").removeClass("support-invalid-postal");
    $("#shipping-postal-code-input").removeClass("support-current-input-on-focus");

}

function UpdateAddressInfo(idPrefix, address, city, state, postal) {

    $("#ise-message-tips").fadeOut("slow");

    var addressId = "#" + idPrefix + "AddressControl_txtStreet";
    var cityId    = "#" + idPrefix + "AddressControl_txtCity";
    var stateId   = "#" + idPrefix + "AddressControl_txtState";
    var postalId  = "#" + idPrefix + "AddressControl_txtPostal";

    $(addressId).removeClass("required-input");
    $(cityId).removeClass("required-input");
    $(stateId).removeClass("required-input");

    $(postalId).removeClass("required-input");
    $(postalId).removeClass("invalid-postal");
    $(postalId).removeClass("current-input-on-focus");

    if (address != "") {

        $("#" + idPrefix + "AddressControl_lblStreet").addClass("display-none");
        $(addressId).val(address);
    }

    if (city != "") {

        $("#" + idPrefix + "AddressControl_lblCity").addClass("display-none");
        $(cityId).val(city);

    }

    if (state != "") {

        $("#" + idPrefix + "AddressControl_lblState").addClass("display-none");
        $(stateId).val(state);

    }

    if (postal != "") {

        $("#" + idPrefix + "AddressControl_lblPostal").addClass("display-none");
        $(postalId).val(postal);

    }

}

function _SplitAreaCodeAndPhone(areaCodeId, primaryPhoneId, phone) {

    var details = phone.split(")");
    var areaCode = "";
    var primary = "";

    if (details.length > 1) {

        areaCode = details[0];

        areaCode = areaCode.replace(")", "");
        areaCode = areaCode.replace("(", "");
        areaCode = areaCode.replace("+", "");

        primary = details[1];

    } else {

        var i = 1;

        details = phone.replace("+", "");

        if (phone.length >= 10) {

            primary = details.substring(3, phone.length);
            areaCode = details.substring(0, 3);

        } else {

            primary = details;
            $("#" + primaryPhoneId + "-label").removeClass("display-none");
        }
    }

    if (areaCode != "") {

        $("#" + areaCodeId[0]).val($.trim(areaCode));
        $("#" + areaCodeId[1]).addClass("display-none");

    }

    if (primary != "") {

        $("#" + primaryPhoneId[0]).val($.trim(primary));
        $("#" + primaryPhoneId[1]).addClass("display-none");

    }
}

function HideStateInputBoxForCountryWithState(controlId) {
    
    var withState = IsCountryWithStates("#" + controlId  + "_drpCountry");

    if (typeof (withState) != "undefined" && withState == "false") {

        $("#" + controlId + "_txtState").addClass("display-none");
        $("#" + controlId + "_lblState").addClass("display-none");

        $("#" + controlId + "_txtCity").addClass("city-width-if-no-state");

    } else {

        $("#" + controlId + "_txtState").removeClass("display-none");
        $("#" + controlId + "_txtCity").removeClass("city-width-if-no-state");
    }

}

