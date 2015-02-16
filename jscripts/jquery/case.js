$(document).ready(function () {

    InitControls();
    EventsListener();

});

function EventsListener() {

    $("#submit-case").click(function () {

        if ($(this).hasClass("editable-content")) return false;
        SubmitCaseForm();

    });

    $("#captcha-refresh-button").click(function () {

        captchaCounter++;
        $("#captcha").attr("src", "Captcha.ashx?id=" + captchaCounter);

    });

}

function SubmitCaseForm() {

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

            var cssDisplay  = $(".zip-city-other-place-holder").css("display");
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

                var status = IsCountryWithStates("#AddressControl_drpCountry");

                if (cssDisplay == "none" || status == "false") {

                    skip = true;
                    skipStateValidation = true;

                } else {

                    skip = true;
                    if (objectValue == "") skip = false;
  
                }

            }

            // state control <--

            // Optional Postal Code: if postal code is optional skip empty value validation

            var isPostalInputBox = IsInArray(['BillingAddressControl_txtPostal', 'ShippingAddressControl_txtPostal', 'AddressControl_txtPostal'], $(this).attr("id"));
            var isPostalOptional = isPostalInputBox && $(this).hasClass("is-postal-optional");

            if (isPostalOptional) {
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


    if (formHasEmptyFields){
        $(thisObjectId).focus();
        return false;
    }

    var $emailInputBox = $("#txtEmail");

    if ($emailInputBox.hasClass("invalid-email") || $emailInputBox.hasClass("email-duplicates")) {
        $emailInputBox.focus();
        return false;
    }

    // --> verify if CityStates dropdown is initialized

    var cityStates = $("#city-states").val();

    if (typeof (cityStates) == "undefined") {

        var thisLeft = $("#AddressControl_txtPostal").offset().left;
        var thisTop  = $("#AddressControl_txtPostal").offset().top;

        $("#ise-message-tips").css("top", thisTop - 47);
        $("#ise-message-tips").css("left", thisLeft - 17);


        $("#ise-message").html(ise.StringResource.getString("selectaddress.aspx.11"));
        $("#ise-message-tips").fadeIn("slow");

        return false;

    } else {

        $("#AddressControl_txtPostal").removeClass("invalid-postal-zero");
        $("#ise-message-tips").fadeOut("slow");
    }

//<--

    VerifyAddress_v2();
}


function InitControls() {

    $("#AddressControl_txtPostal").ISEAddressFinder({
        'country-id'                     : '#AddressControl_drpCountry',
        'postal-id'                      : '#AddressControl_txtPostal',
        'city-id'                        : '#AddressControl_txtCity',
        'state-id'                       : '#AddressControl_txtState',
        'city-state-place-holder'        : '.zip-city-other-place-holder',
        'enter-postal-label-place-holder': '#enter-postal-label-place-holder',
        'city-states-id'                 : 'city-states'
    });

    $("#AddressControl_txtStreet").ISEBubbleMessage({ "input-id": "AddressControl_txtStreet", "label-id": "AddressControl_lblStreet" });
    $("#AddressControl_txtPostal").ISEBubbleMessage({ "input-id": "AddressControl_txtPostal", "label-id": "AddressControl_lblPostal", "input-mode": "postal"});
    $("#AddressControl_txtCity").ISEBubbleMessage({ "input-id": "AddressControl_txtCity", "label-id": "AddressControl_lblCity" });
    $("#AddressControl_txtState").ISEBubbleMessage({ "input-id": "AddressControl_txtState", "label-id": "AddressControl_lblState", "input-mode": "state" });
    $("#AddressControl_txtCounty").ISEBubbleMessage({ "input-id": "AddressControl_txtCounty", "label-id": "AddressControl_lblCounty", "optional": true });

    $("#txtContactName").ISEBubbleMessage({ "input-id": "txtContactName", "label-id": "lblContactName" });
    $("#txtEmail").ISEBubbleMessage({ "input-id": "txtEmail", "label-id": "lblEmail", "input-mode": "email" });
    $("#txtContactNumber").ISEBubbleMessage({ "input-id": "txtContactNumber", "label-id": "lblContactNumber" });

    $("#txtCaseDetails").ISEBubbleMessage({ "input-id": "txtCaseDetails", "label-id": "lblCaseDetails" });
    $("#txtSubject").ISEBubbleMessage({ "input-id": "txtSubject", "label-id": "lblSubject" });

    $("#txtCaptcha").ISEBubbleMessage({ "input-id": "txtCaptcha", "label-id": "lblCaptcha" });

    GetStringResources("customer-support", true);

    var hasPostal = $.trim($("#AddressControl_txtPostal").val()) != '';

    if (hasPostal){

        $(".zip-city-other-place-holder").fadeIn("Slow");
        $("#enter-postal-label-place-holder").html("<input type='hidden' value='other' id='city-states'>");
        $("#onload-process-place-holder").removeClass("error-message").html("");

        // <--
        HideStateInputBoxForCountryWithState("AddressControl");
    }

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
    INPUT_COUNTY: "AddressControl_txtCounty",
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


function VerifyAddress_v2() {

    var $postalInputBox = GetObjectControl(addressConstantID.INPUT_POSTAL);

    var country = GetInputValue(addressConstantID.DRP_COUNTRY);
    var postalCode = GetInputValue(addressConstantID.INPUT_POSTAL);

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
        return false;
    }

    var $hiddenCityState = $("#txtCityStates");
    var cityStates = GetInputValue(addressConstantID.HIDDEN_CITYSTATE);

    $hiddenCityState.val(cityStates);
    if (cityStates == constants.TYPE_OTHER) {
        var cityStates = GetInputValue(addressConstantID.INPUT_STATE) + ", " + GetInputValue(addressConstantID.INPUT_CITY);
        $("#txtCityStates").val(cityStates);
    }

    ShowProcessMessage(ise.StringResource.getString("customersupport.aspx.25"), "error-summary", "save-case-loader", "save-case-button-place-holder");
    $("#btnSendCaseForm").trigger("click");

}

function GetObjectControl(id) {
    return $(constants.SEP_POUND + id);
}
