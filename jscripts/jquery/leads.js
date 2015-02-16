$(window).load(function () {

    if (ise.Configuration.getConfigValue("IsAdminCurrentlyLoggedIn") == "true") return false;

    var process = getQueryString()["process"];

    if (process != "" && typeof (process) != "undefined") {

        $("#support-form-wrapper").addClass("display-none");
        $("#lead-form-thank-you").fadeIn("slow");

    } else {

        InitControls();

        GetStringResources("lead-form", true);
        LeadFormControlEventsListener();
    }

    AjaxSecureEmailAddressVerification(true, "", "", function (response) { }, function (response) { });

});

function LeadFormControlEventsListener() {

    $("#btnSubmitLF").click(function () { FinalizeLeadForm(); });

    $("#captcha-refresh-button").click(function () {

        captchaCounter++;
        $("#captcha").attr("src", "Captcha.ashx?id=" + captchaCounter);

    });


    $("#user-control-profile-first-name-input").keypress(function () {

        $(this).removeClass("lead-duplicates");
        $("#user-control-profile-last-name-input").removeClass("lead-duplicates");

    });

    $("#user-control-profile-last-name-input").keypress(function () {

        $(this).removeClass("lead-duplicates");
        $("#user-control-profile-first-name-input").removeClass("lead-duplicates");

    });

}

function FinalizeLeadForm() {

    /* 
                
    -> This function revalidates required informations and email address format 
    -> If all information is good then calls function DoSubmissionOfCaseFormAction()

    */

    $("#ise-message-tips").fadeOut("slow");

    var goodForm = true;

    var counter = 0;
    var formHasEmptyFields = false;

    var thisObjectId = "";
    var skip = false;
    var skipStateValidation = false;

    $(".requires-validation").each(function () {   //-> scan all html controls with class .apply-behind-caption-effects

        var inputValue = trimString($(this).val());
        if (inputValue == "") {

            skip = false;

            var object = this;
            var thisObjectId = "#" + $(object).attr("id");

            var cssDisplay = $(".zip-city-other-place-holder").css("display");
            var objectValue = inputValue;

            /* city control -->  
            
            If city control is on display and empty: validate
                
            city control <-- */

            if (thisObjectId == "#AddressControl_txtCity") {

                if (cssDisplay == "none") {

                    skip = true;

                } else {

                    skip = true;
                    if (objectValue == "") skip = false;

                }

            }

            /* state control --> 

            If state control is on display 
                
            -> It must be validated on submit 
            -> Skip EMPTY validation if state control has assigned value

            otherwise:

            skip state validation and skp EMPTY validation of state control

            */

            if (thisObjectId == "#AddressControl_txtState") {

                // see for IsCountryWithStates definition
                var status = IsCountryWithStates("#AddressControl_drpCountry");

                if (cssDisplay == "none" || status == "false") {

                    skip = true;
                    skipStateValidation = true;

                } else {

                    skip = true;
                    if (objectValue == "") skip = false;

                }

            }

            // Optional Postal Code: if postal code is optional skip empty value validation

            var id = $(this).attr("id");
            if (IsPostalInputBoxOptional(id)) {
                skip = true;
            }

            if (skip == false) {

                $(this).removeClass("current-object-on-focus");
                $(this).addClass("required-input");

                /* Points mouse cursor on the first input with no value to render bubble message */

                if (counter == 0) {

                    thisObjectId = "#" + $(this).attr("id");
                    $(this).addClass("current-object-on-focus");
                    $(this).focus();

                }

                formHasEmptyFields = true;
                counter++;
            }
        }
    });

    if (formHasEmptyFields) {
        $(thisObjectId).focus();
        return false;
    }

    var $emailInputBox = $("#ProfileControl_txtEmail");

    if ($emailInputBox.hasClass("invalid-email") || $emailInputBox.hasClass("email-duplicates")) {
        $emailInputBox.focus();
        return false;
    }

    var cityStates = $("#city-states").val();

    if (typeof (cityStates) == "undefined") {

        var thisLeft = $("#AddressControl_txtPostal").offset().left;
        var thisTop = $("#AddressControl_txtPostal").offset().top;

        $("#ise-message-tips").css("top", thisTop - 47);
        $("#ise-message-tips").css("left", thisLeft - 17);

        $("#ise-message").html(ise.StringResource.getString("selectaddress.aspx.11"));
        $("#ise-message-tips").fadeIn("slow");

        return false;

    } else {

        $("#AddressControl_txtPostal").removeClass("undefined-city-states");
        $("#ise-message-tips").fadeOut("slow");
    }

    // validate email format and duplication first before submitting the lead form
    var accountType = $emailInputBox.parent("span").attr("data-accountType");
    accountType = (typeof (accountType) == "undefined") ? "" : $.trim(accountType);

    var message = $.trim(ise.StringResource.getString("createaccount.aspx.158"));
    if (typeof message != constants.TYPE_UNDEFINED && message != constants.EMPTY) {
        ShowProcessMessage(message, constantID.DIV_ERROR_SUMMARY, constantID.DIV_SAVE_LEAD_LOADER, constantID.DIV_LEAD_BUTTON_CONTAINER);
    }

    AjaxSecureEmailAddressVerification(false, $emailInputBox.val(), accountType, function (response) {

        response = trimString(response.d);
        var bubbleTipsClass = "";

        if (response != "") {

            switch (response) {
                case "email-duplicates":
                    bubbleTipsClass = response;
                    break;
                case "invalid-email":
                    bubbleTipsClass = response;
                    break;
                default:
                    bubbleTipsClass = "ajax-request-failed";
                    break;
            }

            $emailInputBox.addClass(bubbleTipsClass).attr("response-message", response);
            $emailInputBox.focus();

            if (bubbleTipsClass == "ajax-request-failed") {
                // if request fails still continue with submission of lead form
                VerifyAddress_v2();
            } else {
                hideProgressIndicator(constantID.DIV_SAVE_LEAD_LOADER, constantID.DIV_LEAD_BUTTON_CONTAINER);
                return false;
            }

        } else {
            VerifyAddress_v2();
        }

    }, function (response) {
        // if request fails still continue with submission of lead form
        VerifyAddress_v2();
    });

}



function InitControls() {

    $("#AddressControl_txtPostal").ISEAddressFinder({
        'country-id': '#AddressControl_drpCountry',
        'postal-id': '#AddressControl_txtPostal',
        'city-id': '#AddressControl_txtCity',
        'state-id': '#AddressControl_txtState',
        'city-state-place-holder': '.zip-city-other-place-holder',
        'enter-postal-label-place-holder': '#enter-postal-label-place-holder',
        'city-states-id': 'city-states'
    });

    $("#AddressControl_txtStreet").ISEBubbleMessage({ "input-id": "AddressControl_txtStreet", "label-id": "AddressControl_lblStreet" });
    $("#AddressControl_txtPostal").ISEBubbleMessage({ "input-id": "AddressControl_txtPostal", "label-id": "AddressControl_lblPostal", "input-mode": "postal" });
    $("#AddressControl_txtCity").ISEBubbleMessage({ "input-id": "AddressControl_txtCity", "label-id": "AddressControl_lblCity" });
    $("#AddressControl_txtState").ISEBubbleMessage({ "input-id": "AddressControl_txtState", "label-id": "AddressControl_lblState", "input-mode": "state" });

    if (typeof ($("#AddressControl_txtCounty").val()) != "undefined") {

        $("#AddressControl_txtCounty").ISEBubbleMessage({ "input-id": "AddressControl_txtCounty", "label-id": "AddressControl_lblCounty", "optional": true });

    }

    $("#ProfileControl_txtFirstName").ISEBubbleMessage({ "input-id": "ProfileControl_txtFirstName", "label-id": "lblFirstName" });
    $("#ProfileControl_txtLastName").ISEBubbleMessage({ "input-id": "ProfileControl_txtLastName", "label-id": "lblLastName" });

    $("#ProfileControl_txtEmail").ISEBubbleMessage({ "input-id": "ProfileControl_txtEmail", "label-id": "lblEmail", "input-mode": "email" });
    $("#ProfileControl_txtContactNumber").ISEBubbleMessage({ "input-id": "ProfileControl_txtContactNumber", "label-id": "lblContactNumber" });
    $("#ProfileControl_txtMobile").ISEBubbleMessage({ "input-id": "ProfileControl_txtMobile", "label-id": "lblMobile", "optional": true });

    $("#txtMessage").ISEBubbleMessage({ "input-id": "txtMessage", "label-id": "lblMessage" });
    $("#txtCaptcha").ISEBubbleMessage({ "input-id": "txtCaptcha", "label-id": "lblCaptcha" });

    $("#account-name-wrapper").html("");
    $("#passwords-wrapper").html("");
}

// note : all codes below this line... need to transfer to plugin (addrress verification)

var constantErrorTags = {
    POSTAL_NOT_FOUND: "invalid-postal-zero",
    INVALID_EMAIL: "invalid-email",
    INVALID_POSTAL: "invalid-postal",
    STATE_NOT_FOUND: "state-not-found",
    REQUIRED_INPUT: "required-input",
    PASSWORD_NOT_MATCH: " password-not-match",
    PASSWORD_NOT_STRONG: "password-not-strong",
    PASSWORD_LENGTH_INVALID: "password-length-invalid"
}

var addressConstantID = {
    DRP_COUNTRY: "AddressControl_drpCountry",
    HIDDEN_CITYSTATE: "city-states",
    LABEL_STREET: "AddressControl_lblStreet",
    LABEL_POSTAL: "AddressControl_lblPostal",
    LABEL_CITY: "AddressControl_lblCity",
    LABEL_STATE: "AddressControl_lblState",
    LABEL_COUNTY: "AddressControl_lblCounty",
    INPUT_STREET: "AddressControl_txtStreet",
    INPUT_POSTAL: "AddressControl_txtPostal",
    INPUT_CITY: "AddressControl_txtCity",
    INPUT_STATE: "AddressControl_txtState",
    INPUT_COUNTY: "AddressControl_txtCounty"
}

var constants = {
    CONTROL_DISABLED: "control-disabled",
    DISPLAY_NONE: "display-none",
    WIDTH_NO_STATE: "city-width-if-no-state",
    WIDTH_ENTER_POSTAL: "enter-postal-message-width",
    EMPTY: "",
    SEP_SPACE: " ",
    SEP_POUND: "#",
    SET_DOT: ".",
    TYPE_WHOLESALE: "wholesale",
    TYPE_SIGNUP: "signup",
    TYPE_PROFILE: "profile",
    TYPE_ADDRESS: "address",
    TYPE_UNDEFINED: "undefined",
    TYPE_OTHER: "other",
    ON_FOCUS: "current-object-on-focus"
}

var constantID = {
    DIV_ERROR_SUMMARY: "error-summary",
    DIV_SAVE_LEAD_LOADER: "save-lead-loader",
    DIV_LEAD_BUTTON_CONTAINER: "save-lead-button-place-holder"
}

function VerifyAddress_v2() {

    var $postalInputBox = GetObjectControl(addressConstantID.INPUT_POSTAL);

    var country = GetInputValue(addressConstantID.DRP_COUNTRY);
    var postalCode = GetInputValue(addressConstantID.INPUT_POSTAL);
    var stateCode = GetInputValue(addressConstantID.INPUT_STATE);

    var postalIsGood = true;

    // optional postal code 
    var isPostalOptional = ($postalInputBox.hasClass("is-postal-optional") == true && postalCode == "");

    if (IsSearchable(addressConstantID.DRP_COUNTRY) && isPostalOptional == false) {

        if (IsPostalFormatInvalid(country, postalCode)) {

            $postalInputBox.addClass(constantErrorTags.INVALID_POSTAL);

            if (ConvertStringToLower($(".zip-city-other-place-holder").css("display")) == "none") {
                $(constantID.DIV_ENTER_POSTAL).html(ise.StringResource.getString("customersupport.aspx.40"));
            }

            postalIsGood = false;
        }

    }

    if (postalIsGood == false) {
        hideProgressIndicator(constantID.DIV_SAVE_LEAD_LOADER, constantID.DIV_LEAD_BUTTON_CONTAINER);
        return false;
    }

    var data = new Object();

    data.country = country;
    data.postal = postalCode;
    data.stateCode = stateCode;

    validatePostalCode(data, submitLeadForm);
}


function validatePostalCode(o, callback) {

    var data = { country: o.country, postal: o.postal, stateCode: o.stateCode, shipToCountry: constants.EMPTY, shipToPostal: constants.EMPTY, shipToStateCode: constants.EMPTY }

    var successCallback = function (result) {

        $postalInputBox = GetObjectControl(addressConstantID.INPUT_POSTAL);

        GetObjectControl(addressConstantID.INPUT_STATE).removeClass(constantErrorTags.STATE_NOT_FOUND);
        $postalInputBox.removeClass(constantErrorTags.POSTAL_NOT_FOUND);

        var response = parseInt(result.d);

        if (response == 0) {
            callback(data);
            return false;
        }

        if (response == 1) {
            GetObjectControl(addressConstantID.INPUT_STATE).addClass(constantErrorTags.STATE_NOT_FOUND);
            $postalInputBox.addClass(constantErrorTags.POSTAL_NOT_FOUND);
            $postalInputBox.focus();
        }

        hideProgressIndicator(constantID.DIV_SAVE_LEAD_LOADER, constantID.DIV_LEAD_BUTTON_CONTAINER);
    }

    var failedCallback = function (result) {
        callback(data);
    }

    var message = $.trim(ise.StringResource.getString("createaccount.aspx.159"));
    if (typeof message != constants.TYPE_UNDEFINED && message != constants.EMPTY) {
        ShowProcessMessage(message, constantID.DIV_ERROR_SUMMARY, constantID.DIV_SAVE_LEAD_LOADER, constantID.DIV_LEAD_BUTTON_CONTAINER);
    }

    AjaxCallCommon("ActionService.asmx/ValidatePostalCode", data, successCallback, failedCallback);
}

function submitLeadForm(data) {
    var callback = function () {

        var $hiddenCityState = $("#txtCityStates");
        var cityStates = GetInputValue(addressConstantID.HIDDEN_CITYSTATE);

        $hiddenCityState.val(cityStates);
        if (cityStates == constants.TYPE_OTHER) {
            var cityStates = GetInputValue(addressConstantID.INPUT_STATE) + ", " + GetInputValue(addressConstantID.INPUT_CITY);
            $("#txtCityStates").val(cityStates);
        }

        ShowProcessMessage(ise.StringResource.getString("leadform.aspx.24"), constantID.DIV_ERROR_SUMMARY, constantID.DIV_SAVE_LEAD_LOADER, constantID.DIV_LEAD_BUTTON_CONTAINER);
        $("#btnSaveLead").trigger("click");
    }

    if (ise.Configuration.getConfigValue("UseShippingAddressVerification") == "false" || data.postalCode == "") {
        callback();
        return false;
    }

    if (typeof ($.fn.RealTimeAddressVerification) != constants.TYPE_UNDEFINED) {
        $.fn.RealTimeAddressVerification.requestAddressMatch(callback);
    } else {
        callback();

    }
}

function GetObjectControl(id) {
    return $(constants.SEP_POUND + id);
}

function hideProgressIndicator(id, container) {
    $("#" + id).fadeOut("slow", function () {
        $("#" + container).fadeIn("slow");
    });
}