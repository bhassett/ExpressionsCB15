// in preparation to refractoring of js aftert 14. 0

var appConfigs = {
    IS_CAPTCHA_REQUIRED: false,
    IS_SHOW_SHIPPING: false,
    IS_SHOW_TAX: false,
    IS_VAT_ENABLED: false,
    IS_HAS_AGE_REQUIREMENT: false,
    IS_REQUIRE_STRONG_PASSWORD: false,
    REG_EX_STRONG_PASSWORD: "",
    IS_GET_ADDRESS_MATCH: false
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

var constantID = {
    BTN_CAPTCHA_REFRESH: "#captcha-refresh-button",
    BTN_SIGNUP: "#create-customer-account",
    BTN_HIDDEN_SIGNUP: "#btnCreateAccount",
    BTN_UPDATE_ADDRESS: "#btnUpdateAddress",
    BTN_ADD_ADDRESS: "#btnNewAddress",
    CHK_SAME_AS_BILLING: "#copyBillingInfo",
    CHK_IS_WITH_REQUIRED_AGE: "#chkOver13",
    DIV_SHIPPING_SECTION_HEADER: "#shipping-section-head-place-holder",
    DIV_SHIPPING_SECTION_INFO: "#shipping-info-place-holder",
    DIV_MESSAGE_TIPS: "#ise-message-tips",
    DIV_REQUIRED_AGE: "#age-13-place-holder",
    DIV_ENTER_POSTAL: "#enter-postal-label-place-holder",
    DIV_BILLING_ENTER_POSTAL: "#billing-enter-postal-label-place-holder",
    DIV_SHIPPING_ENTER_POSTAL: "#shipping-enter-postal-label-place-holder",
    DIV_SAVE_ACCOUNT_LOADER: "save-account-loader",
    DIV_SAVE_ACCOUNT_BUTTON_CONTAINER: "save-account-button-place-holder",
    DIV_UPDATE_ADDRESS_LOADER: "update-address-loader",
    DIV_UPDATE_ADDRESS_BUTTON_CONTAINER: "update-address-button-place-holder",
    DRP_ADDRESS_TYPE: "#ShippingAddressControl_drpType",
    DRP_BUSINESS_TYPE: "#BillingAddressControl_drpBusinessType",
    INPUT_ACCOUNT_NAME: "#ProfileControl_txtAccountName",
    INPUT_CAPTCHA: "#txtCaptcha",
    INPUT_TAX_NUMBER: "#tax-number-place-holder"

}

var profileConstantID = {
    CHK_IS_OKTOEMAIL: "chkIsOkToEmail",
    CHK_IS_OVER13CHECKED: "chkIsOver13Checked",
    DIV_ERROR_PLACEHOLDER: "profile-error-place-holder",
    DIV_SAVE_PROGRESS_PLACEHOLDER: "save-profile-loader",
    DIV_SAVE_BUTTON_PLACEHOLDER: "save-profile-button-place-holder",
    DRP_SALUTATION: "ProfileControl_drpLstSalutation",
    INPUT_CAPTCHA: "txtCaptcha",
    INPUT_EMAIL: "ProfileControl_txtEmail",
    INPUT_ANONYMOUS_EMAIL: "ProfileControl_txtAnonymousEmail",
    INPUT_FIRSTNAME: "ProfileControl_txtFirstName",
    INPUT_LASTNAME: "ProfileControl_txtLastName",
    INPUT_CONTACT_NUMBER: "ProfileControl_txtContactNumber",
    INPUT_ACCOUNT_NAME: "ProfileControl_txtAccountName",
    INPUT_PASSWORD: "ProfileControl_txtPassword",
    INPUT_CONFIRM_PASSWORD: "ProfileControl_txtConfirmPassword",
    INPUT_MOBILE: "ProfileControl_txtMobile",
    INPUT_OLD_PASSWORD: "old-password-input",
    LABEL_FIRSTNAME: "lblFirstName",
    LABEL_LASTNAME: "lblLastName",
    LABEL_EMAIL: "lblEmail",
    LABEL_ANONYMOUS_EMAIL: "lblAnonymousEmail",
    LABEL_CONTACT_NUMBER: "lblContactNumber",
    LABEL_ACCOUNT_NAME: "lblAccountName",
    LABEL_CONFIRM_PASSWORD: "lblConfirmPassword",
    LABEL_MOBILE: "lblMobile",
    LINK_RESET_SIGN_UP_FORM: "linkResetSignUpForm"
}


var addressConstantID = {
    CHK_SAME_AS_BILLING: "copyBillingInfo",
    DIV_TAX_NUMBER: "tax-number-place-holder",
    DRP_SHIPPING_ADDRESS_TYPE: "ShippingAddressControl_drpType",
    DRP_BILLING_BUSINESS_TYPE: "BillingAddressControl_drpBusinessType",
    DRP_COUNTRY: "AddressControl_drpCountry",
    DRP_BILLING_COUNTRY: "BillingAddressControl_drpCountry",
    DRP_SHIPPING_COUNTRY: "ShippingAddressControl_drpCountry",
    HIDDEN_CITYSTATE: "city-states",
    HIDDEN_BILLING_CITYSTATE: "billing-city-states",
    HIDDEN_SHIPPING_CITYSTATE: "shipping-city-states",
    LABEL_STREET: "AddressControl_lblStreet",
    LABEL_POSTAL: "AddressControl_lblPostal",
    LABEL_CITY: "AddressControl_lblCity",
    LABEL_STATE: "AddressControl_lblState",
    LABEL_COUNTY: "AddressControl_lblCounty",
    LABEL_BILLING_STREET: "BillingAddressControl_lblStreet",
    LABEL_BILLING_POSTAL: "BillingAddressControl_lblPostal",
    LABEL_BILLING_CITY: "BillingAddressControl_lblCity",
    LABEL_BILLING_STATE: "BillingAddressControl_lblState",
    LABEL_BILLING_COUNTY: "BillingAddressControl_lblCounty",
    LABEL_SHIPPING_STREET: "ShippingAddressControl_lblStreet",
    LABEL_SHIPPING_POSTAL: "ShippingAddressControl_lblPostal",
    LABEL_SHIPPING_CITY: "ShippingAddressControl_lblCity",
    LABEL_SHIPPING_STATE: "ShippingAddressControl_lblState",
    LABEL_SHIPPING_COUNTY: "ShippingAddressControl_lblCounty",
    INPUT_STREET: "AddressControl_txtStreet",
    INPUT_POSTAL: "AddressControl_txtPostal",
    INPUT_CITY: "AddressControl_txtCity",
    INPUT_STATE: "AddressControl_txtState",
    INPUT_COUNTY: "AddressControl_txtCounty",
    INPUT_BILLING_STREET: "BillingAddressControl_txtStreet",
    INPUT_BILLING_POSTAL: "BillingAddressControl_txtPostal",
    INPUT_BILLING_CITY: "BillingAddressControl_txtCity",
    INPUT_BILLING_STATE: "BillingAddressControl_txtState",
    INPUT_BILLING_COUNTY: "BillingAddressControl_txtCounty",
    INPUT_SHIPPING_STREET: "ShippingAddressControl_txtStreet",
    INPUT_SHIPPING_POSTAL: "ShippingAddressControl_txtPostal",
    INPUT_SHIPPING_CITY: "ShippingAddressControl_txtCity",
    INPUT_SHIPPING_STATE: "ShippingAddressControl_txtState",
    INPUT_SHIPPING_COUNTY: "ShippingAddressControl_txtCounty"

}

var serviceMethods = {
    UPDATE_ACCOUNTINFO: "UpdateAccountInfo"
};

// <-- in preparation to refractoring of js aftert 14. 0

$(document).ready(function () {

    var loadAtPage = $("#load-at-page").val();

    switch (loadAtPage) {
        case "create-account":


            LoadCustomerRequiredStringResources(constants.TYPE_SIGNUP);

            var callback = function () {

                appConfigs.IS_CAPTCHA_REQUIRED = ise.Configuration.getConfigValue("SecurityCodeRequiredOnCreateAccount") == "true" || ise.Configuration.getConfigValue("SecurityCodeRequiredOnCreateAccountDuringCheckout") == "true";
                appConfigs.IS_SHOW_SHIPPING = ise.Configuration.getConfigValue("AllowShipToDifferentThanBillTo") == "true";
                appConfigs.IS_SHOW_TAX = ise.Configuration.getConfigValue("VAT.ShowTaxFieldOnRegistration") == "true";
                appConfigs.IS_VAT_ENABLED = ise.Configuration.getConfigValue("VAT.Enabled") == "true";
                appConfigs.IS_HAS_AGE_REQUIREMENT = ise.Configuration.getConfigValue("RequireOver13Checked") == "true";
                appConfigs.IS_REQUIRE_STRONG_PASSWORD = ise.Configuration.getConfigValue("UseStrongPwd") == "true";
                appConfigs.REG_EX_STRONG_PASSWORD = ise.Configuration.getConfigValue("CustomerPwdValidator");
                appConfigs.IS_GET_ADDRESS_MATCH = ise.Configuration.getConfigValue("UseShippingAddressVerification") == "true";

                InitSignUpForm();
                AjaxSecureEmailAddressVerification(true, constants.EMPTY, constants.EMPTY, function (a) { }, function (a) { });

            };

            LoadCustmerRequiredAppConfiguration(constants.TYPE_SIGNUP, callback);

            BindBubbleMessagePluginToSignUpForm();
            BindSignUpFormEventsListener();

            break;
        case "edit-profile":


            LoadCustomerRequiredStringResources(constants.TYPE_PROFILE);

            var callback = function () {

                appConfigs.IS_CAPTCHA_REQUIRED = ise.Configuration.getConfigValue("SecurityCodeRequiredOnCreateAccount") == "true";
                appConfigs.IS_REQUIRE_STRONG_PASSWORD = ise.Configuration.getConfigValue("UseStrongPwd") == "true";
                appConfigs.REG_EX_STRONG_PASSWORD = ise.Configuration.getConfigValue("CustomerPwdValidator");

                InitProfileForm();
                AjaxSecureEmailAddressVerification(true, constants.EMPTY, constants.EMPTY, function (a) { }, function (a) { });
            };

            LoadCustmerRequiredAppConfiguration(constants.TYPE_PROFILE, callback);

            BindProfileFormEventsListener();

            break;
        case "select-address":

            LoadCustomerRequiredStringResources(constants.TYPE_ADDRESS);

            var callback = function (configs) {
                appConfigs.IS_GET_ADDRESS_MATCH = ise.Configuration.getConfigValue("UseShippingAddressVerification") == "true";
            };

            LoadCustmerRequiredAppConfiguration(constants.TYPE_ADDRESS, callback);

            var process = getQueryString()["add"];
            if (process == "true") {
                BindAddressControlEventsListener(true);
                BindBubbleMessagePluginToAddressControl();
            }

            break;
        case "edit-address":

            LoadCustomerRequiredStringResources(constants.TYPE_ADDRESS);

            var callback = function (configs) {
                appConfigs.IS_GET_ADDRESS_MATCH = ise.Configuration.getConfigValue("UseShippingAddressVerification") == "true";
            };

            LoadCustmerRequiredAppConfiguration(constants.TYPE_ADDRESS, callback);

            BindAddressControlEventsListener(false);
            BindBubbleMessagePluginToAddressControl();

            break;
        default:
            break;
    }

});


function InitSignUpForm() {

    if (appConfigs.IS_CAPTCHA_REQUIRED) {
        $(".captcha-section").fadeIn("slow");
        $(constantID.INPUT_CAPTCHA).ISEBubbleMessage({ "input-id": "txtCaptcha", "label-id": "lblCaptcha" });
    }

    if (appConfigs.IS_SHOW_SHIPPING == false) {

        $(constantID.DIV_SHIPPING_SECTION_HEADER).html(constants.EMPTY);
        $(constantID.DIV_SHIPPING_SECTION_INFO).html(constants.EMPTY);
        $(".shipping-section-clears").addClass(constants.DISPLAY_NONE);

    }

    $(constantID.CHK_SAME_AS_BILLING).click(function () {
        CopyBillingInformation($(this).is(":checked"));
    });

    $(constantID.CHK_IS_WITH_REQUIRED_AGE).click(function () {

        if ($(this).is(":checked")) {

            $(constantID.DIV_MESSAGE_TIPS).fadeOut("slow");
            $(constantID.DIV_REQUIRED_AGE).removeClass(constantErrorTags.REQUIRED_INPUT);
        }

    });

    var value = GetInputValue(addressConstantID.DRP_BILLING_BUSINESS_TYPE);
    if (ConvertStringToLower(value) == constants.TYPE_WHOLESALE) {
        ShowElement(addressConstantID.DIV_TAX_NUMBER);
    }

}

function BindSignUpFormEventsListener() {

    var $divMessageTips = $(constantID.DIV_MESSAGE_TIPS);

    $(constantID.BTN_SIGNUP).click(function () {
        ValidateSignUpForm();
    });

    $(constantID.BTN_CAPTCHA_REFRESH).click(function () {

        captchaCounter++;
        $("#captcha").attr("src", "Captcha.ashx?id=" + captchaCounter);

    });

    var $passwordInputBox = GetObjectControl(profileConstantID.INPUT_PASSWORD);
    var $confirmPasswordInputBox = GetObjectControl(profileConstantID.INPUT_CONFIRM_PASSWORD);

    $passwordInputBox.keyup(function () {
        var $this = $(this);

        if ($this.val() == constants.EMPTY) {

            ShowElement(profileConstantID.LABEL_CONFIRM_PASSWORD);

            $confirmPasswordInputBox.removeClass(constantErrorTags.PASSWORD_NOT_MATCH).removeClass(constantErrorTags.REQUIRED_INPUT).val(constants.EMPTY);
            $this.removeClass(constantErrorTags.PASSWORD_NOT_STRONG);
        }

    });

    $confirmPasswordInputBox.keyup(function () {

        var password = trimString($(this).val());
        if (password == constants.EMPTY) {
            $passwordInputBox.removeClass(constantErrorTags.PASSWORD_NOT_MATCH).removeClass(constantErrorTags.PASSWORD_NOT_STRONG).removeClass(constantErrorTags.PASSWORD_LENGTH_INVALID).removeClass(constantErrorTags.REQUIRED_INPUT);
        }

    });

    $(constantID.DRP_ADDRESS_TYPE).change(function () {
        $(this).removeClass(constantErrorTags.REQUIRED_INPUT);
        $divMessageTips.fadeOut("slow");

    });

    var $taxNumberInputBoxPlaceHolder = GetObjectControl(addressConstantID.DIV_TAX_NUMBER);

    $(constantID.DRP_BUSINESS_TYPE).change(function () {
        var $this = $(this);

        $this.removeClass(constantErrorTags.REQUIRED_INPUT);
        $divMessageTips.fadeOut("slow");

        var value = ConvertStringToLower($this.val());
        var wholesaleType = ConvertStringToLower(ise.StringResource.getString("createaccount.aspx.79"));

        if (value == wholesaleType) {

            ShowElement(addressConstantID.DIV_TAX_NUMBER);
            $taxNumberInputBoxPlaceHolder.fadeIn("slow");

        } else {
            $taxNumberInputBoxPlaceHolder.fadeOut("slow");
        }

    });

    $firstNameInputBox = GetObjectControl(profileConstantID.INPUT_FIRSTNAME);
    $lastNameInputBox = GetObjectControl(profileConstantID.INPUT_LASTNAME);

    $accountNameInputBox = GetObjectControl(profileConstantID.INPUT_ACCOUNT_NAME);
    $accountNameLabel = GetObjectControl(profileConstantID.LABEL_ACCOUNT_NAME);

    $firstNameInputBox.blur(function () {

        var firstName = trimString($(this).val());
        var lastName = trimString($lastNameInputBox.val());

        if (firstName != constants.EMPTY && lastName != constants.EMPTY) {
            HideElement(profileConstantID.LABEL_ACCOUNT_NAME);
            $accountNameInputBox.removeClass(constantErrorTags.REQUIRED_INPUT).val(firstName + constants.SEP_SPACE + lastName);
        }

    });

    $lastNameInputBox.blur(function () {

        var fistName = trimString($firstNameInputBox.val());
        var lastName = trimString($(this).val());

        if (fistName != constants.EMPTY && lastName != constants.EMPTY) {
            HideElement(profileConstantID.LABEL_ACCOUNT_NAME);
            $accountNameInputBox.removeClass(constantErrorTags.REQUIRED_INPUT).val(fistName + constants.SEP_SPACE + lastName);
        }

    });

    $linkFormReset = GetObjectControl(profileConstantID.LINK_RESET_SIGN_UP_FORM);
    $linkFormReset.click(function () {
        ResetSignUpForm();
    })

    ReturnFalseOnEnter();
}

function ResetSignUpForm() {

    // Reset profile control 

    GetObjectControl(profileConstantID.INPUT_FIRSTNAME).val(constants.EMPTY).removeClass(constantErrorTags.REQUIRED_INPUT);
    ShowElement(profileConstantID.LABEL_FIRSTNAME);

    GetObjectControl(profileConstantID.INPUT_LASTNAME).val(constants.EMPTY).removeClass(constantErrorTags.REQUIRED_INPUT);
    ShowElement(profileConstantID.LABEL_LASTNAME);

    var $inputEmail = GetObjectControl(profileConstantID.INPUT_EMAIL);
    if (typeof ($inputEmail.val()) != constants.TYPE_UNDEFINED) {
        GetObjectControl(profileConstantID.INPUT_EMAIL).val(constants.EMPTY).removeClass(constantErrorTags.REQUIRED_INPUT).removeClass(constantErrorTags.INVALID_EMAIL);
        ShowElement(profileConstantID.LABEL_EMAIL);
    }

    var $inputAnonymousEmail = GetObjectControl(profileConstantID.INPUT_ANONYMOUS_EMAIL);
    if (typeof ($inputAnonymousEmail.val()) != constants.TYPE_UNDEFINED) {
        GetObjectControl(profileConstantID.INPUT_ANONYMOUS_EMAIL).val(constants.EMPTY).removeClass(constantErrorTags.REQUIRED_INPUT).removeClass(constantErrorTags.INVALID_EMAIL);
        ShowElement(profileConstantID.LABEL_ANONYMOUS_EMAIL);
    }

    GetObjectControl(profileConstantID.INPUT_CONTACT_NUMBER).val(constants.EMPTY).removeClass(constantErrorTags.REQUIRED_INPUT);
    ShowElement(profileConstantID.LABEL_CONTACT_NUMBER);

    // Reset billing control

    GetObjectControl(addressConstantID.INPUT_BILLING_STREET).val(constants.EMPTY).removeClass(constantErrorTags.REQUIRED_INPUT);
    ShowElement(addressConstantID.LABEL_BILLING_STREET);

    GetObjectControl(addressConstantID.INPUT_BILLING_CITY).val(constants.EMPTY).removeClass(constantErrorTags.REQUIRED_INPUT);
    ShowElement(addressConstantID.LABEL_BILLING_CITY);

    GetObjectControl(addressConstantID.INPUT_BILLING_STATE).val(constants.EMPTY).removeClass(constantErrorTags.REQUIRED_INPUT);
    ShowElement(addressConstantID.LABEL_BILLING_STATE);

    GetObjectControl(addressConstantID.INPUT_BILLING_POSTAL).val(constants.EMPTY).trigger("blur").removeClass(constantErrorTags.REQUIRED_INPUT);
    ShowElement(addressConstantID.LABEL_BILLING_POSTAL);

    // Reset shipping control

    GetObjectControl(addressConstantID.INPUT_SHIPPING_STREET).val(constants.EMPTY).removeClass(constantErrorTags.REQUIRED_INPUT);
    ShowElement(addressConstantID.LABEL_SHIPPING_STREET);

    GetObjectControl(addressConstantID.INPUT_SHIPPING_CITY).val(constants.EMPTY).removeClass(constantErrorTags.REQUIRED_INPUT);
    ShowElement(addressConstantID.LABEL_SHIPPING_CITY);

    GetObjectControl(addressConstantID.INPUT_SHIPPING_STATE).val(constants.EMPTY).removeClass(constantErrorTags.REQUIRED_INPUT);
    ShowElement(addressConstantID.LABEL_SHIPPING_STATE);

    GetObjectControl(addressConstantID.INPUT_SHIPPING_POSTAL).val(constants.EMPTY).trigger("blur").removeClass(constantErrorTags.REQUIRED_INPUT);
    ShowElement(addressConstantID.LABEL_SHIPPING_POSTAL);

    GetObjectControl(addressConstantID.CHK_SAME_AS_BILLING).removeAttr("checked");

    // Reset dropdown list to first option

    GetObjectControl(addressConstantID.DRP_BILLING_BUSINESS_TYPE).val(GetDropdownListFirstOption(addressConstantID.DRP_BILLING_BUSINESS_TYPE));
    GetObjectControl(addressConstantID.DRP_SHIPPING_ADDRESS_TYPE).val(GetDropdownListFirstOption(addressConstantID.DRP_SHIPPING_ADDRESS_TYPE));
    GetObjectControl(addressConstantID.DRP_BILLING_COUNTRY).val(GetDropdownListFirstOption(addressConstantID.DRP_BILLING_COUNTRY));
    GetObjectControl(addressConstantID.DRP_SHIPPING_COUNTRY).val(GetDropdownListFirstOption(addressConstantID.DRP_SHIPPING_COUNTRY));

    // Enabled shipping control (if on disabled state)

    CopyBillingInformation(false);
    GetObjectControl(profileConstantID.INPUT_CAPTCHA).val(constants.EMPTY).removeClass(constantErrorTags.REQUIRED_INPUT);
}

function GetDropdownListFirstOption(id) {
    return $.trim($(constants.SEP_POUND + id + " option:first").val());

}

function ReturnFalseOnEnter() {

    GetObjectControl(profileConstantID.INPUT_EMAIL).keypress(function (event) { return enterEvent(event); });
    GetObjectControl(profileConstantID.INPUT_ANONYMOUS_EMAIL).keypress(function (event) { return enterEvent(event); });
    GetObjectControl(profileConstantID.INPUT_FIRSTNAME).keypress(function (event) { return enterEvent(event); });
    GetObjectControl(profileConstantID.INPUT_LASTNAME).keypress(function (event) { return enterEvent(event); });
    GetObjectControl(profileConstantID.INPUT_CONTACT_NUMBER).keypress(function (event) { return enterEvent(event); });
    GetObjectControl(profileConstantID.INPUT_ACCOUNT_NAME).keypress(function (event) { return enterEvent(event); });
    GetObjectControl(profileConstantID.INPUT_PASSWORD).keypress(function (event) { return enterEvent(event); });
    GetObjectControl(profileConstantID.INPUT_CONFIRM_PASSWORD).keypress(function (event) { return enterEvent(event); });

}

function enterEvent(event) { if (event.which == 13) return false; }

function IsPasswordStrong(id) {

    if (appConfigs.IS_REQUIRE_STRONG_PASSWORD == false) return true;

    var expression = appConfigs.REG_EX_STRONG_PASSWORD;

    if (expression == constants.EMPTY) {
        return false;
    }

    var password = $("#" + id).val();
    password = $.trim(password);

    return (password.match(expression));
}

function ValidatePassword(id) {

    var $passwordInputBox = GetObjectControl(id);
    var $confirmPasswordInputBox = GetObjectControl(profileConstantID.INPUT_CONFIRM_PASSWORD);

    var value = GetInputValue(id);

    if (typeof (value) == constants.TYPE_UNDEFINED) return true;

    var strongPassword = true;
    strongPassword = IsPasswordStrong(id);

    if (value.length < parseInt(ise.Configuration.getConfigValue("PasswordMinLength"))) {

        $passwordInputBox.addClass(constantErrorTags.PASSWORD_LENGTH_INVALID);
        return false;

    }

    if (!strongPassword) {

        $passwordInputBox.addClass(constantErrorTags.PASSWORD_NOT_STRONG);
        return false;

    } else {

        $passwordInputBox.removeClass(constantErrorTags.PASSWORD_NOT_STRONG);

        var myPassword = value;
        var confirmPassword = GetInputValue(profileConstantID.INPUT_CONFIRM_PASSWORD);

        if (myPassword != constants.EMPTY && confirmPassword != constants.EMPTY) {

            if (myPassword != confirmPassword) {

                $passwordInputBox.addClass(constantErrorTags.PASSWORD_NOT_MATCH);
                $confirmPasswordInputBox.addClass(constantErrorTags.PASSWORD_NOT_MATCH);

                return false;

            } else {

                $passwordInputBox.removeClass(constantErrorTags.PASSWORD_NOT_MATCH);
                $confirmPasswordInputBox.removeClass(constantErrorTags.PASSWORD_NOT_MATCH);

                return true;
            }

        }

    }

    return true;
}

function CopyBillingInformation(copy) {

    var $countrySelector = GetObjectControl(addressConstantID.DRP_SHIPPING_COUNTRY);
    var $streetInputBox = GetObjectControl(addressConstantID.INPUT_SHIPPING_STREET);
    var $postalInputBox = GetObjectControl(addressConstantID.INPUT_SHIPPING_POSTAL);
    var $stateInputBox = GetObjectControl(addressConstantID.INPUT_SHIPPING_STATE);
    var $cityInputBox = GetObjectControl(addressConstantID.INPUT_SHIPPING_CITY);
    var $countyInputBox = GetObjectControl(addressConstantID.INPUT_SHIPPING_COUNTY);

    $streetInputBox.removeClass(constantErrorTags.REQUIRED_INPUT);
    $postalInputBox.removeClass(constantErrorTags.REQUIRED_INPUT).removeClass(constantErrorTags.INVALID_POSTAL);
    $stateInputBox.removeClass(constantErrorTags.REQUIRED_INPUT).removeClass(constantErrorTags.STATE_NOT_FOUND);
    $cityInputBox.removeClass(constantErrorTags.REQUIRED_INPUT);
    $countyInputBox.removeClass(constantErrorTags.REQUIRED_INPUT);

    if (copy) {

        var country = GetInputValue(addressConstantID.DRP_BILLING_COUNTRY);
        var billingCityState = GetInputValue(addressConstantID.HIDDEN_BILLING_CITYSTATE);
        var city = constants.EMPTY;
        var state = constants.EMPTY;

        if (typeof (billingCityState) == constants.TYPE_UNDEFINED || billingCityState == constants.TYPE_OTHER) {

            city = GetInputValue(addressConstantID.INPUT_BILLING_CITY);
            state = GetInputValue(addressConstantID.INPUT_BILLING_STATE);

        } else {

            var bc = billingCityState.split(", ");

            if (bc.length > 1) {

                city = $.trim(bc[1]);
                state = $.trim(bc[0]);

            } else {

                city = $.trim(bc[0]);
                state = constants.EMPTY;
            }

        }

        if (typeof (city) == constants.TYPE_UNDEFINED) {
            city = constants.EMPTY;
        }

        if (typeof (state) == constants.TYPE_UNDEFINED) {
            state = constants.EMPTY;
        }

        $streetInputBox.val(GetInputValue(addressConstantID.INPUT_BILLING_STREET));
        $postalInputBox.val(GetInputValue(addressConstantID.INPUT_BILLING_POSTAL));
        $cityInputBox.val(city);
        $stateInputBox.val(state);
        $countrySelector.val(country);

        var county = GetInputValue(addressConstantID.INPUT_BILLING_COUNTY);

        if (typeof (county) != constants.TYPE_UNDEFINED && county != constants.EMPTY) {
            $countyInputBox.val(county);
        }

        $(constantID.DIV_MESSAGE_TIPS).fadeOut("slow");
        DisableShippingControl(true);

    } else {

        DisableShippingControl(false);

    }

    if (IsWithStates(addressConstantID.DRP_BILLING_COUNTRY)) {

        $cityInputBox.removeClass(constants.WIDTH_NO_STATE);
        $stateInputBox.fadeIn("slow");

    } else {

        $stateInputBox.fadeOut("slow", function () {

            HideElement(addressConstantID.LABEL_SHIPPING_STATE);
            $cityInputBox.addClass(constants.WIDTH_NO_STATE);

        });

    }
}

function DisableShippingControl(disable) {

    var $countrySelector = GetObjectControl(addressConstantID.DRP_SHIPPING_COUNTRY);
    var $streetInputBox = GetObjectControl(addressConstantID.INPUT_SHIPPING_STREET);
    var $postalInputBox = GetObjectControl(addressConstantID.INPUT_SHIPPING_POSTAL);
    var $stateInputBox = GetObjectControl(addressConstantID.INPUT_SHIPPING_STATE);
    var $cityInputBox = GetObjectControl(addressConstantID.INPUT_SHIPPING_CITY);
    var $countyInputBox = GetObjectControl(addressConstantID.INPUT_SHIPPING_COUNTY);

    if (disable) {

        HideElement(addressConstantID.LABEL_SHIPPING_STREET);
        HideElement(addressConstantID.LABEL_SHIPPING_STATE);
        HideElement(addressConstantID.LABEL_SHIPPING_CITY);
        HideElement(addressConstantID.LABEL_SHIPPING_POSTAL);
        HideElement(addressConstantID.LABEL_BILLING_STATE);
        HideElement(addressConstantID.LABEL_COUNTY);

        $streetInputBox.addClass(constants.CONTROL_DISABLED).attr("disabled", "disabled");
        $postalInputBox.addClass(constants.CONTROL_DISABLED).attr("disabled", "disabled")
        $cityInputBox.addClass(constants.CONTROL_DISABLED).attr("disabled", "disabled");
        $stateInputBox.addClass(constants.CONTROL_DISABLED).attr("disabled", "disabled");
        $countyInputBox.addClass(constants.CONTROL_DISABLED).attr("disabled", "disabled");

        $countrySelector.css("background", "#ccc").attr("disabled", "disabled");

        $(constantID.DIV_SHIPPING_ENTER_POSTAL).removeClass(constants.WIDTH_ENTER_POSTAL).html("<input type='hidden' id='shipping-city-states' value='other'/>");
        GetObjectControl(addressConstantID.HIDDEN_SHIPPING_CITYSTATE).fadeOut("Slow", function () { $(".shipping-zip-city-other-place-holder").fadeIn("Slow"); });

    } else {

        $streetInputBox.removeClass(constants.CONTROL_DISABLED).removeAttr("disabled");
        $postalInputBox.removeClass(constants.CONTROL_DISABLED).removeAttr("disabled");
        $cityInputBox.removeClass(constants.CONTROL_DISABLED).removeAttr("disabled", "disabled");
        $stateInputBox.removeClass(constants.CONTROL_DISABLED).removeAttr("disabled");
        $countyInputBox.removeClass(constants.CONTROL_DISABLED).removeAttr("disabled");

        $countrySelector.css("background", "#fff").removeAttr("disabled");
    }
}

function ValidateSignUpForm() {

    /*           
        -> This function revalidates required informations and email address format 
        -> If all information is good then calls function DoSubmissionOfCaseFormAction()
    */

    $divMessageTips = $(constantID.DIV_MESSAGE_TIPS);

    $businessTypeSelector = GetObjectControl(addressConstantID.DRP_BILLING_BUSINESS_TYPE);
    $addressTypeSelector = GetObjectControl(addressConstantID.DRP_SHIPPING_ADDRESS_TYPE)

    $billingPostalInputBox = GetObjectControl(addressConstantID.INPUT_BILLING_POSTAL);
    $shippingPostalInputBox = GetObjectControl(addressConstantID.INPUT_SHIPPING_POSTAL);

    $divMessageTips.fadeOut("slow");

    var counter = 0;
    var isRequiredFieldEmpty = false;

    var skip = false;
    var skipStateValidation = false;

    // scan each form controls with a class name of requires-validation

    var isSameWithBillingAddress = IsElementChecked("copyBillingInfo");

    $(".requires-validation").each(function () {

        var $this = $(this);
        var value = $.trim($this.val());
        var id = $this.attr("id");
        id = $.trim(id);

        if (value == "") {

            skip = false;

            var cssDisplay = "";

            /*  billing city and state : skip if input box is hidden */

            if (id == addressConstantID.INPUT_BILLING_CITY || id == addressConstantID.INPUT_BILLING_STATE) {
                skip = IsElementHidden(".billing-zip-city-other-place-holder");
            }

            /* skip state validation if country don't have states */

            if (id == addressConstantID.INPUT_BILLING_STATE && skip == false) {
                skip = IsWithStates(addressConstantID.DRP_BILLING_COUNTRY) == false;
            }

            if (appConfigs.IS_SHOW_SHIPPING) {

                /*  skip shipping validation if it is the same as billing info */

                if (id == addressConstantID.INPUT_SHIPPING_CITY || id == addressConstantID.INPUT_SHIPPING_STATE) {
                    skip = IsElementHidden(".shipping-zip-city-other-place-holder");
                }

                if (id == addressConstantID.INPUT_SHIPPING_STATE && skip == false) {
                    skip = IsWithStates(addressConstantID.DRP_SHIPPING_COUNTRY) == false || isSameWithBillingAddress;
                }

            }

            // state control <--

            // Optional Postal Code: if postal code is optional skip empty value validation   
            if (IsPostalInputBoxOptional(id)) {
                skip = true;
            }

            // skip validation if shipping is the as with billing information

            if (IsInArray([addressConstantID.INPUT_SHIPPING_STREET,
                             addressConstantID.INPUT_SHIPPING_CITY,
                             addressConstantID.INPUT_SHIPPING_STATE,
                             addressConstantID.INPUT_SHIPPING_POSTAL], id) && isSameWithBillingAddress) {
                skip = true;
            }

            // tag input box error 

            if (skip == false) {

                $this.removeClass(constants.ON_FOCUS).addClass(constantErrorTags.REQUIRED_INPUT);

                /* Points mouse cursor on the first input with no value to render bubble message */

                if (counter == 0) {

                    $this.addClass(constants.ON_FOCUS);
                    $this.focus();

                }

                isRequiredFieldEmpty = true;
                counter++;
            }
        }

    });


    if (isRequiredFieldEmpty) {
        return false;
    }

    var emailInputValue = GetObjectControl(profileConstantID.INPUT_EMAIL).val();
    var $emailInputBox = (typeof (emailInputValue) == constants.TYPE_UNDEFINED) ? GetObjectControl(profileConstantID.INPUT_ANONYMOUS_EMAIL) : GetObjectControl(profileConstantID.INPUT_EMAIL);

    if ($emailInputBox.hasClass("invalid-email") || $emailInputBox.hasClass("email-duplicates")) {
        $emailInputBox.focus();
        return false;
    }

    var pInputId = profileConstantID.INPUT_PASSWORD;

    if (!ValidatePassword(pInputId) && GetInputValue(pInputId) != constants.EMPTY) {
        GetObjectControl(pInputId).focus();
        return false;
    }

    var offset = null;

    if (appConfigs.IS_SHOW_TAX && appConfigs.IS_VAT_ENABLED) {

        if (ConvertStringToLower($businessTypeSelector.val()) == ConvertStringToLower(ise.StringResource.getString("createaccount.aspx.82"))) {

            SetObjectFieldAsRequired($businessTypeSelector);
            offset = getObectPositionOffset($businessTypeSelector);
            showTips(offset, ise.StringResource.getString("selectaddress.aspx.13"));

            return false;

        }
    }

    if (appConfigs.IS_SHOW_SHIPPING) {

        if (ConvertStringToLower($addressTypeSelector.val()) == ConvertStringToLower(ise.StringResource.getString("selectaddress.aspx.6"))) {

            SetObjectFieldAsRequired($addressTypeSelector);

            offset = getObectPositionOffset($addressTypeSelector);
            showTips(offset, ise.StringResource.getString("selectaddress.aspx.12"));

            return false;
        }

    }

    if (GetInputValue(addressConstantID.HIDDEN_BILLING_CITYSTATE) == constants.EMPTY) {

        $billingPostalInputBox.addClass(constantErrorTags.POSTAL_NOT_FOUND);

        offset = getObectPositionOffset($billingPostalInputBox);
        showTips(offset, ise.StringResource.getString("selectaddress.aspx.11"));

        return false;

    } else {

        $billingPostalInputBox.removeClass(constantErrorTags.POSTAL_NOT_FOUND);
        $divMessageTips.fadeOut("slow");
    }

    if (isSameWithBillingAddress) {
        CopyBillingInformation(true);
    }

    if (appConfigs.IS_SHOW_SHIPPING && isSameWithBillingAddress == false) {

        if (GetInputValue(addressConstantID.HIDDEN_SHIPPING_CITYSTATE) == constants.EMPTY) {

            $shippingPostalInputBox.addClass(constantErrorTags.POSTAL_NOT_FOUND);

            offset = getObectPositionOffset($shippingPostalInputBox);
            showTips(offset, ise.StringResource.getString("selectaddress.aspx.11"));

            return false;

        } else {

            $shippingPostalInputBox.removeClass(constantErrorTags.POSTAL_NOT_FOUND);
            $divMessageTips.fadeOut("slow");
        }
    }


    var $messageTips = $(constantID.DIV_MESSAGE_TIPS);

    if (appConfigs.IS_HAS_AGE_REQUIREMENT && $("#chkOver13").is(':checked') == false) {

        $(constantID.DIV_REQUIRED_AGE).addClass(constantErrorTags.REQUIRED_INPUT);

        var thisLeft = $(constantID.DIV_REQUIRED_AGE).offset().left;
        var thisTop = $(constantID.DIV_REQUIRED_AGE).offset().top;

        $messageTips.css("top", thisTop - 47);
        $messageTips.css("left", thisLeft - 17);

        $("#ise-message").html(ise.StringResource.getString("createaccount.aspx.123"));
        $messageTips.fadeIn("slow");

        return false;

    } else {

        $(constantID.DIV_REQUIRED_AGE).removeClass(constantErrorTags.REQUIRED_INPUT);

    }

    // validate email format and duplication first before submitting the sign up form
    var accountType = $emailInputBox.parent("span").attr("data-accountType");
    accountType = (typeof (accountType) == "undefined") ? "" : $.trim(accountType);

    var message = $.trim(ise.StringResource.getString("createaccount.aspx.158"));

    if (typeof message != constants.TYPE_UNDEFINED && message != constants.EMPTY) {
        ShowProcessMessage(message, "error-summary", constantID.DIV_SAVE_ACCOUNT_LOADER, constantID.DIV_SAVE_ACCOUNT_BUTTON_CONTAINER);
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

                // if request fails still continue with submission of sign up form
                VerifySignUpFormAddress_v2();

            } else {

                hideProgressIndicator(constantID.DIV_SAVE_ACCOUNT_LOADER, constantID.DIV_SAVE_ACCOUNT_BUTTON_CONTAINER);
                return false;
            }

        } else {

            VerifySignUpFormAddress_v2();
        }

    }, function (response) {


        // if request fails still continue with submission of sign up form
        VerifySignUpFormAddress_v2();

    });



}

function IsElementChecked(id) {
    return $("#" + id).is(':checked');
}

function IsElementHidden(selector) {
    return $(selector).css("display") == "none";
}

function IsUndefined(value) {
    return (typeof (value) == constants.TYPE_UNDEFINED);
}

function SetObjectFieldAsRequired(o) {
    o.addClass(constantErrorTags.REQUIRED_INPUT);
}

function getObectPositionOffset(o) {
    return { left: o.offset().left, top: o.offset().top }
}

function showTips(offset, text) {

    $divMessageTips = $("#ise-message-tips");
    $divMessageText = $("#ise-message");

    $divMessageTips.css("top", offset.top - 47).css("left", offset.left - 17);
    $divMessageText.html(text);
    $divMessageTips.fadeIn("slow");

}


function BindBubbleMessagePluginToSignUpForm() {

    $("#BillingAddressControl_txtPostal").ISEAddressFinder();

    $("#BillingAddressControl_txtStreet").ISEBubbleMessage({ "input-id": "BillingAddressControl_txtStreet", "label-id": "BillingAddressControl_lblStreet" });
    $("#BillingAddressControl_txtPostal").ISEBubbleMessage({ "input-id": "BillingAddressControl_txtPostal", "label-id": "BillingAddressControl_lblPostal", "input-mode": "billing-postal", "address-type": 'Billing' });
    $("#BillingAddressControl_txtCity").ISEBubbleMessage({ "input-id": "BillingAddressControl_txtCity", "label-id": "BillingAddressControl_lblCity" });
    $("#BillingAddressControl_txtState").ISEBubbleMessage({ "input-id": "BillingAddressControl_txtState", "label-id": "BillingAddressControl_lblState", "input-mode": "billing-state" });
    $("#BillingAddressControl_txtTaxNumber").ISEBubbleMessage({ "input-id": "BillingAddressControl_txtTaxNumber", "label-id": "BillingAddressControl_lblTaxNumber", "optional": true });

    if (typeof ($("#BillingAddressControl_txtCounty").val()) != "undefined") {

        $("#BillingAddressControl_txtCounty").ISEBubbleMessage({ "input-id": "BillingAddressControl_txtCounty", "label-id": "BillingAddressControl_lblCounty", "optional": true });

    }

    $("#ProfileControl_txtFirstName").ISEBubbleMessage({ "input-id": "ProfileControl_txtFirstName", "label-id": "lblFirstName" });
    $("#ProfileControl_txtLastName").ISEBubbleMessage({ "input-id": "ProfileControl_txtLastName", "label-id": "lblLastName" });

    var $inputEmail = GetObjectControl(profileConstantID.INPUT_EMAIL);
    if (typeof ($inputEmail.val()) != constants.TYPE_UNDEFINED) {
        $inputEmail.ISEBubbleMessage({ "input-id": profileConstantID.INPUT_EMAIL, "label-id": profileConstantID.LABEL_EMAIL, "input-mode": "email" });
    }

    var $inputAnonymousEmail = GetObjectControl(profileConstantID.INPUT_ANONYMOUS_EMAIL);
    if (typeof ($inputAnonymousEmail.val()) != constants.TYPE_UNDEFINED) {
        $inputAnonymousEmail.ISEBubbleMessage({ "input-id": profileConstantID.INPUT_ANONYMOUS_EMAIL, "label-id": profileConstantID.LABEL_ANONYMOUS_EMAIL, "input-mode": "email" });
    }

    $("#ProfileControl_txtContactNumber").ISEBubbleMessage({ "input-id": "ProfileControl_txtContactNumber", "label-id": "lblContactNumber" });
    $("#ProfileControl_txtConfirmPassword").ISEBubbleMessage({ "input-id": "ProfileControl_txtConfirmPassword", "label-id": "lblConfirmPassword", "input-mode": "password-confirmation" });

    $("#ProfileControl_txtAccountName").ISEBubbleMessage({ "input-id": "ProfileControl_txtAccountName", "label-id": "lblAccountName" });
    $("#ProfileControl_txtPassword").ISEBubbleMessage({ "input-id": "ProfileControl_txtPassword", "label-id": "lblPassword", "input-mode": "password" });
    $("#ProfileControl_txtMobile").ISEBubbleMessage({ "input-id": "ProfileControl_txtMobile", "label-id": "lblMobile", "optional": true });

    $("#ShippingAddressControl_txtPostal").ISEAddressFinder({
        'country-id': '#ShippingAddressControl_drpCountry',
        'postal-id': '#ShippingAddressControl_txtPostal',
        'city-id': '#ShippingAddressControl_txtCity',
        'state-id': '#ShippingAddressControl_txtState',
        'city-state-place-holder': '.shipping-zip-city-other-place-holder',
        'enter-postal-label-place-holder': '#shipping-enter-postal-label-place-holder',
        'city-states-id': 'shipping-city-states'
    });

    $("#ShippingAddressControl_txtStreet").ISEBubbleMessage({ "input-id": "ShippingAddressControl_txtStreet", "label-id": "ShippingAddressControl_lblStreet" });
    $("#ShippingAddressControl_txtPostal").ISEBubbleMessage({ "input-id": "ShippingAddressControl_txtPostal", "label-id": "ShippingAddressControl_lblPostal", "input-mode": "shipping-postal", "address-type": 'Shipping' });
    $("#ShippingAddressControl_txtCity").ISEBubbleMessage({ "input-id": "ShippingAddressControl_txtCity", "label-id": "ShippingAddressControl_lblCity" });
    $("#ShippingAddressControl_txtState").ISEBubbleMessage({ "input-id": "ShippingAddressControl_txtState", "label-id": "ShippingAddressControl_lblState", "input-mode": "shipping-state" });

    if (typeof ($("#ShippingAddressControl_txtCounty").val()) != "undefined") {

        $("#ShippingAddressControl_txtCounty").ISEBubbleMessage({ "input-id": "ShippingAddressControl_txtCounty", "label-id": "ShippingAddressControl_lblCounty", "optional": true });

    }

    if (IsElementChecked("copyBillingInfo")) {
        CopyBillingInformation(true);
    }

    GetSelectedCityState();
}


function GetSelectedCityState() {

    if ($("#billingTxtCityStates").val() != "") {

        $(".billing-zip-city-other-place-holder").fadeIn("Slow");
        $("#billing-enter-postal-label-place-holder").html("<input type='hidden' id='billing-city-states' value='other'/>");

        var _bCityState = $("#billingTxtCityStates").val().split(",");

        if (_bCityState.length > 1) {

            if (_bCityState[0] != "") $("#BillingAddressControl_lblState").addClass("display-none");
            if (_bCityState[1] != "") $("#BillingAddressControl_lblCity").addClass("display-none");

            $("#BillingAddressControl_txtState").val(_bCityState[0]);
            $("#BillingAddressControl_txtCity").val(_bCityState[1]);


        } else {

            if (_bCityState[0] != "") $("#BillingAddressControl_lblCity").addClass("display-none");
            $("#BillingAddressControl_txtCity").val(_bCityState[0]);
        }

    }

    if ($("#shippingTxtCityStates").val() != "") {

        $(".shipping-zip-city-other-place-holder").fadeIn("Slow");
        $("#shipping-enter-postal-label-place-holder").html("<input type='hidden' id='shipping-city-states' value='other'/>");

        var _sCityState = $("#shippingTxtCityStates").val().split(",");

        if (_sCityState.length > 1) {

            if (_sCityState[0] != "") $("#ShippingAddressControl_lblState").addClass("display-none");
            if (_sCityState[1] != "") $("#ShippingAddressControl_lblCity").addClass("display-none");

            $("#ShippingAddressControl_txtState").val($.trim(_sCityState[0]));
            $("#ShippingAddressControl_txtCity").val($.trim(_sCityState[1]));

        } else {

            if (_sCityState[0] != "") $("#ShippingAddressControl_lblCity").addClass("display-none");
            $("#ShippingAddressControl_txtCity").val(_sCityState[0]);

        }

    }

    if (IsWithStates("BillingAddressControl_drpCountry") == false) {

        $("#BillingAddressControl_lblState").removeClass("display-none")
        $("#BillingAddressControl_txtState").val("");
        $("#BillingAddressControl_txtState").addClass("control-disabled");

        $("#BillingAddressControl_txtState").fadeOut("slow", function () {

            $("#BillingAddressControl_lblState").addClass("display-none");
            $("#BillingAddressControl_txtCity").addClass("city-width-if-no-state");

        });

    }

    if (IsWithStates("ShippingAddressControl_drpCountry") == false) {

        $("#ShippingAddressControl_lblState").removeClass("display-none")
        $("#ShippingAddressControl_txtState").val("");
        $("#ShippingAddressControl_txtState").addClass("control-disabled");

        $("#ShippingAddressControl_txtState").fadeOut("slow", function () {

            $("#ShippingAddressControl_lblState").addClass("display-none");
            $("#ShippingAddressControl_txtCity").addClass("city-width-if-no-state");

        });

    }
}

// Create Account ends here <--


/* Edit Profile starts here --> 
  
List of active function used on Edit Account:

1. InitProfileForm
2. Profile_EventHandlers
3. UpdateCustomerProfile
4. Profile_EditPassword

*/

function InitProfileForm() { // <-- this function is active on jquery/bubble.message.js: see InitCreateAccountAppConfigs function

    if (appConfigs.IS_CAPTCHA_REQUIRED) {

        $(constantID.INPUT_CAPTCHA).val(constants.EMPTY);
        $(constantID.INPUT_CAPTCHA).ISEBubbleMessage({ "input-id": "txtCaptcha", "label-id": "lblCaptcha" });
        $(".captcha-section").fadeIn("slow");
    }

    $("#ProfileControl_txtFirstName").ISEBubbleMessage({ "input-id": "ProfileControl_txtFirstName", "label-id": "lblFirstName" });
    $("#ProfileControl_txtLastName").ISEBubbleMessage({ "input-id": "ProfileControl_txtLastName", "label-id": "lblLastName" });
    $("#ProfileControl_txtEmail").ISEBubbleMessage({ "input-id": "ProfileControl_txtEmail", "label-id": "lblEmail", "input-mode": "email" });
    $("#ProfileControl_txtContactNumber").ISEBubbleMessage({ "input-id": "ProfileControl_txtContactNumber", "label-id": "lblContactNumber" });

    $("#ProfileControl_txtPassword").ISEBubbleMessage({ "input-id": "ProfileControl_txtPassword", "label-id": "lblPassword", "input-mode": "password" });
    $("#ProfileControl_txtConfirmPassword").ISEBubbleMessage({ "input-id": "ProfileControl_txtConfirmPassword", "label-id": "lblConfirmPassword", "input-mode": "password-confirmation" });
    $("#old-password-input").ISEBubbleMessage({ "input-id": "old-password-input", "label-id": "old-password-input-label" });
    $("#ProfileControl_txtMobile").ISEBubbleMessage({ "input-id": "ProfileControl_txtMobile", "label-id": "lblMobile", "optional": true });


}

function BindProfileFormEventsListener() {

    Profile_EditPassword(false);

    $("#edit-password").removeAttr("checked");

    $("#edit-password").click(function () {

        var checked = $(this).attr("checked");

        if (checked == "checked") {

            Profile_EditPassword(true);

        } else {

            Profile_EditPassword(false);

        }

    });

    $("#captcha-refresh-button").click(function () {

        _CaptchaCounter += 1;
        _CaptchaCounter++;

        $("#captcha").attr("src", "Captcha.ashx?id=" + _CaptchaCounter);

    });

    $("#support-security-code").css("width", "193px");
    $("#captcha-label").css("padding-right", "31px");

    $("#update-profile").click(function () {

        var $this = $(this);

        if ($this.hasClass("editable-content")) { return false; }
        if (!ValidateProfileRequiredFields()) { return false; }

        var $emailInputBox = GetObjectControl(profileConstantID.INPUT_EMAIL);

        var accountType = $emailInputBox.parent("span").attr("data-accountType");
        accountType = (typeof (accountType) == "undefined") ? "" : $.trim(accountType);

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
                    // if request fails still continue with submission of account form
                    SubmitUpdateAccountButton();
                } else {
                    return false;
                }

            } else {
                SubmitUpdateAccountButton();
            }

        }, function (response) {
            $this.addClass("request-error").attr("response-message", response);
        });


    });

}

function SubmitUpdateAccountButton() {

    if ($("#edit-password").attr("checked") == "checked") {
        if (!ValidatePassword("ProfileControl_txtPassword")) {
            $("#ProfileControl_txtPassword").focus();
            return false;
        }

        if (!ValidatePassword("ProfileControl_txtConfirmPassword")) {
            $("#ProfileControl_txtConfirmPassword").focus();
            return false;
        }
        UpdateCustomerProfile(true);
    } else {
        UpdateCustomerProfile(false);
    }
}

function UpdateCustomerProfile(updatePassword) {

    var salutation = GetInputValue(profileConstantID.DRP_SALUTATION);

    var password = constants.EMPTY;
    var oldPassword = constants.EMPTY;

    if (updatePassword) {
        password = GetInputValue(profileConstantID.INPUT_PASSWORD);
        oldPassword = GetInputValue(profileConstantID.INPUT_OLD_PASSWORD);
    }

    ShowProcessMessage(ise.StringResource.getString("createaccount.aspx.121"), profileConstantID.DIV_ERROR_PLACEHOLDER, profileConstantID.DIV_SAVE_PROGRESS_PLACEHOLDER, profileConstantID.DIV_SAVE_BUTTON_PLACEHOLDER);

    var accountInfo = new Object();
    accountInfo.Salutation = salutation;
    accountInfo.FirstName = GetInputValue(profileConstantID.INPUT_FIRSTNAME);
    accountInfo.LastName = GetInputValue(profileConstantID.INPUT_LASTNAME);
    accountInfo.ContactNumber = GetInputValue(profileConstantID.INPUT_CONTACT_NUMBER);
    accountInfo.Email = GetInputValue(profileConstantID.INPUT_EMAIL);
    accountInfo.IsOkToEmail = IsElementChecked(profileConstantID.CHK_IS_OKTOEMAIL);
    accountInfo.IsOver13Checked = IsElementChecked(profileConstantID.CHK_IS_OVER13CHECKED);
    accountInfo.Mobile = GetInputValue(profileConstantID.INPUT_MOBILE);

    var data = new Object();
    data.account = JSON.stringify(accountInfo);
    data.newPassword = password;
    data.oldPassword = oldPassword;
    data.captcha = GetInputValue(profileConstantID.INPUT_CAPTCHA);

    $divErrorPlachHolder = GetObjectControl(profileConstantID.DIV_ERROR_PLACEHOLDER);

    var successCallback = function (result) {
        message = result.d;


        if (message == constants.EMPTY) {
            ShowProcessMessage(ise.StringResource.getString("createaccount.aspx.122"), profileConstantID.DIV_ERROR_PLACEHOLDER, profileConstantID.DIV_SAVE_PROGRESS_PLACEHOLDER, profileConstantID.DIV_SAVE_BUTTON_PLACEHOLDER);
            parent.location = "account.aspx";
        } else {
            $divErrorPlachHolder.removeClass(constants.DISPLAY_NONE);
            ShowFailedMessage(message, profileConstantID.DIV_ERROR_PLACEHOLDER, profileConstantID.DIV_SAVE_PROGRESS_PLACEHOLDER, profileConstantID.DIV_SAVE_BUTTON_PLACEHOLDER);

        }

    }

    var failureCallback = function (result) {
        ShowFailedMessage(result.d, profileConstantID.DIV_ERROR_PLACEHOLDER, profileConstantID.DIV_SAVE_PROGRESS_PLACEHOLDER, profileConstantID.DIV_SAVE_BUTTON_PLACEHOLDER);
        $divErrorPlachHolder.removeClass(constants.DISPLAY_NONE);
    }

    AjaxCallWithSecuritySimplified(serviceMethods.UPDATE_ACCOUNTINFO, data, successCallback, failureCallback);

}

function Profile_EditPassword(edit) {

    $("#old-password-input").removeClass("required-input");
    $("#old-password-input").removeClass("current-object-on-focus");

    $("#ProfileControl_txtPassword").removeClass("required-input");
    $("#ProfileControl_txtPassword").removeClass("current-object-on-focus");

    $("#ProfileControl_txtConfirmPassword").removeClass("required-input");
    $("#ProfileControl_txtConfirmPassword").removeClass("current-object-on-focus");

    $("#ProfileControl_txtPassword").removeClass("password-not-match");
    $("#ProfileControl_txtPassword").removeClass("password-not-strong");

    $("#ProfileControl_txtConfirmPassword").removeClass("password-not-match");

    $("#old-password-input-label").css("color", "#8E8E8E");
    $("#lblPassword").css("color", "#8E8E8E");
    $("#lblConfirmPassword").css("color", "#8E8E8E");

    $("#ise-message-tips").fadeOut("slow");

    if (edit) {

        $("#old-password-input").val("");
        $("#ProfileControl_txtPassword").val("");
        $("#ProfileControl_txtConfirmPassword").val("");

        $("#old-password-input").removeClass("control-disabled");

        $("#old-password-input-label").removeClass("display-none");
        $("#old-password-input").removeAttr("disabled");

        $("#ProfileControl_txtPassword").removeClass("control-disabled");
        $("#ProfileControl_txtPassword").removeAttr("disabled");

        $("#ProfileControl_lblPassword").removeClass("display-none");

        $("#ProfileControl_txtConfirmPassword").removeClass("control-disabled");
        $("#ProfileControl_lblConfirmPassword").removeClass("display-none");

        $("#ProfileControl_txtConfirmPassword").removeAttr("disabled");

        $("#password-caption").removeClass("control-caption-disabled");
        $("#old-password-label-place-holder").removeClass("control-caption-disabled");

    } else {

        $("#old-password-label-place-holder").addClass("control-caption-disabled");

        $("#old-password-input").addClass("control-disabled");
        $("#old-password-input").attr("disabled", "disabled");

        $("#ProfileControl_txtPassword").addClass("control-disabled");
        $("#ProfileControl_txtPassword").attr("disabled", "disabled");

        $("#ProfileControl_txtConfirmPassword").addClass("control-disabled");
        $("#ProfileControl_txtConfirmPassword").attr("disabled", "disabled");

        $("#password-caption").addClass("control-caption-disabled");

    }

}



function BindBubbleMessagePluginToAddressControl() {

    $("#AddressControl_txtPostal").ISEAddressFinder({
        'country-id': '#AddressControl_drpCountry',
        'postal-id': '#AddressControl_txtPostal',
        'city-id': '#AddressControl_txtCity',
        'state-id': '#AddressControl_txtState',
        'city-state-place-holder': '.zip-city-other-place-holder',
        'enter-postal-label-place-holder': '#enter-postal-label-place-holder',
        'city-states-id': 'city-states'
    });

    $("#txtContactName").ISEBubbleMessage({ "input-id": "txtContactName", "label-id": "lblContactName" });
    $("#txtContactNumber").ISEBubbleMessage({ "input-id": "txtContactNumber", "label-id": "lblContactNumber" });

    $("#AddressControl_txtStreet").ISEBubbleMessage({ "input-id": "AddressControl_txtStreet", "label-id": "AddressControl_lblStreet" });
    $("#AddressControl_txtPostal").ISEBubbleMessage({ "input-id": "AddressControl_txtPostal", "label-id": "AddressControl_lblPostal", "input-mode": "postal" });
    $("#AddressControl_txtCity").ISEBubbleMessage({ "input-id": "AddressControl_txtCity", "label-id": "AddressControl_lblCity" });
    $("#AddressControl_txtState").ISEBubbleMessage({ "input-id": "AddressControl_txtState", "label-id": "AddressControl_lblState", "input-mode": "state" });

    if (typeof ($("#AddressControl_txtCounty").val()) != "undefined") {

        $("#AddressControl_txtCounty").ISEBubbleMessage({ "input-id": "AddressControl_txtCounty", "label-id": "AddressControl_lblCounty", "optional": true });

    }

    $("#txtContactName").addClass("edit-address-contact-name");

}

function BindAddressControlEventsListener(addnew) {

    if (!addnew) {
        $(".zip-city-other-place-holder").fadeIn("Slow");
        $("#enter-postal-label-place-holder").html("<input type='hidden' id='city-states' value='other'/>");
    }

    $("#save-address").click(function () {
        if ($(this).hasClass("editable-content")) return true;
        SaveAddress(addnew);
    });

}

function SaveAddress(addnew) {

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

                var status = IsWithStates("AddressControl_drpCountry");

                if (cssDisplay == "none" || status == false) {

                    skip = true;
                    skipStateValidation = true;

                } else {

                    skip = true;
                    if (objectValue == "") skip = false;

                }

            }

            // state control <--

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

    if (goodForm) {

        // --> verify if CityStates dropdown is initialized

        var cityStates = $("#city-states").val();
        var $thisPostalInputBox = $("#AddressControl_txtPostal");

        if (typeof cityStates == constants.TYPE_UNDEFINED || cityStates == constants.EMPTY) {

            $thisPostalInputBox.addClass("invalid-postal-zero").focus();
            return false;

        } else {

            $thisPostalInputBox.removeClass("invalid-postal-zero").focus();
        }

        VerifyAddress_v2(addnew);
    }

}

function ValidateProfileRequiredFields() {

    var counter = 0;
    var IsGood = true;
    var skip = false;

    $(".requires-validation").each(function () {   //-> scan all html controls with class .apply-behind-caption-effects
        var $this = $(this);

        skip = false;

        // Optional Postal Code: skip empty validation for optional postal code
        if ($this.attr("disabled") == "disabled") skip = true;

        if ($this.val() == "" && skip == false) {

            $this.removeClass("current-object-on-focus").addClass("required-input");
            if (counter == 0) {
                $this.addClass("current-object-on-focus").focus();
            }

            counter++;
            IsGood = false;
        }
    });

    return IsGood;
}

function LoadCustmerRequiredAppConfiguration(type, callback) {

    var keys = new Array();

    if (type == constants.TYPE_SIGNUP || type == constants.TYPE_PROFILE) {

        keys.push("UseStrongPwd");
        keys.push("RequireOver13Checked");
        keys.push("AllowShipToDifferentThanBillTo");
        keys.push("AllowCustomerDuplicateEMailAddresses");
        keys.push("VAT.ShowTaxFieldOnRegistration");
        keys.push("VAT.Enabled");
        keys.push("CustomerPwdValidator");
        keys.push("SecurityCodeRequiredOnCreateAccount");
        keys.push("PasswordMinLength");
        keys.push("UseShippingAddressVerification");

    } else {
        keys.push("UseShippingAddressVerification");
    }

    ise.Configuration.loadResources(keys, callback);

}

function LoadCustomerRequiredStringResources(type) {

    var keys = new Array();

    switch (type) {

        case constants.TYPE_SIGNUP:

            keys.push("createaccount.aspx.28");
            keys.push("createaccount.aspx.52");
            keys.push("createaccount.aspx.79");
            keys.push("createaccount.aspx.80");
            keys.push("createaccount.aspx.81");
            keys.push("createaccount.aspx.82");
            keys.push("createaccount.aspx.94");
            keys.push("createaccount.aspx.120");
            keys.push("createaccount.aspx.123");
            keys.push("createaccount.aspx.125");
            keys.push("selectaddress.aspx.6");
            keys.push("selectaddress.aspx.12");
            keys.push("selectaddress.aspx.13");
            keys.push("createaccount.aspx.158")
            keys.push("createaccount.aspx.159")

            break;
        case constants.TYPE_PROFILE:

            keys.push("createaccount.aspx.81");
            keys.push("createaccount.aspx.94");

            keys.push("createaccount.aspx.28");
            keys.push("createaccount.aspx.52");
            keys.push("createaccount.aspx.120");

            keys.push("createaccount.aspx.121");
            keys.push("createaccount.aspx.122");

            keys.push("selectaddress.aspx.6");
            keys.push("selectaddress.aspx.12");
            keys.push("selectaddress.aspx.13");

            break;
        case constants.TYPE_ADDRESS:

            keys.push("selectaddress.aspx.8");
            keys.push("selectaddress.aspx.9");
            keys.push("createaccount.aspx.159")
            break;

        default: break;
    }

    // bubble message tips required string resources

    keys.push("customersupport.aspx.15");
    keys.push("customersupport.aspx.16");
    keys.push("customersupport.aspx.17");
    keys.push("customersupport.aspx.18");
    keys.push("customersupport.aspx.23");
    keys.push("customersupport.aspx.24");
    keys.push("customersupport.aspx.38");
    keys.push("customersupport.aspx.39");
    keys.push("customersupport.aspx.40");
    keys.push("customersupport.aspx.41");


    keys.push("selectaddress.aspx.7");
    keys.push("selectaddress.aspx.10");
    keys.push("selectaddress.aspx.11");

    var callback = function () { }
    ise.StringResource.loadResources(keys, callback);
}

function GetObjectControl(id) {
    return $(constants.SEP_POUND + id);
}

function HideElement(id) {
    $(constants.SEP_POUND + id).addClass(constants.DISPLAY_NONE);
}

function ShowElement(id) {
    $(constants.SEP_POUND + id).removeClass(constants.DISPLAY_NONE);
}

function ConvertStringToLower(string) {

    if (typeof (string) == "undefined") {
        return "";
    }

    return string.toLowerCase();
}

// note : all codes below this line... need to transfer to plugin (addrress verification)

function VerifyAddress_v2(addnew) {

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
        hideProgressIndicator(constantID.DIV_UPDATE_ADDRESS_LOADER, constantID.DIV_UPDATE_ADDRESS_BUTTON_CONTAINER);
        return false;
    }

    var data = new Object();

    data.country = country;
    data.postal = postalCode;
    data.stateCode = stateCode;

    var callback = (addnew) ? submitAddNewAddresForm : submitEditAddresForm;
    validatePostalCode(data, callback);
}

function VerifySignUpFormAddress_v2() {

    // -> Billing Address

    var $bPostalInputBox = GetObjectControl(addressConstantID.INPUT_BILLING_POSTAL);

    var bCountry = GetInputValue(addressConstantID.DRP_BILLING_COUNTRY);
    var bPostalCode = GetInputValue(addressConstantID.INPUT_BILLING_POSTAL);
    var bStateCode = GetInputValue(addressConstantID.INPUT_BILLING_STATE);

    var postalIsGood = true;

    // check format

    var b_postalHasInvalidFormat = false;
    var s_postalHasInvalidFormat = false;

    // optional postal code 
    var isPostalOptional = ($bPostalInputBox.hasClass("is-postal-optional") == true && bPostalCode == "");

    if (IsSearchable(addressConstantID.DRP_BILLING_COUNTRY) && isPostalOptional == false) {

        if (IsPostalFormatInvalid(bCountry, bPostalCode)) {

            $bPostalInputBox.addClass(constantErrorTags.INVALID_POSTAL);

            if (ConvertStringToLower($(".billing-zip-city-other-place-holder").css("display")) == "none") {
                $(constantID.DIV_BILLING_ENTER_POSTAL).html(ise.StringResource.getString("customersupport.aspx.40"));
            }

            postalIsGood = false;
        }

    }

    // -> Shipping Address
    var sCountry = constants.EMPTY;
    var sPostalCode = constants.EMPTY;
    var sStateCode = constants.EMPTY;

    if (appConfigs.IS_SHOW_SHIPPING && IsElementChecked("copyBillingInfo") == false) {

        var $sPostalInputBox = GetObjectControl(addressConstantID.INPUT_SHIPPING_POSTAL);

        sCountry = GetInputValue(addressConstantID.DRP_SHIPPING_COUNTRY);
        sPostalCode = GetInputValue(addressConstantID.INPUT_SHIPPING_POSTAL);
        sStateCode = GetInputValue(addressConstantID.INPUT_SHIPPING_STATE);

        // optional postal code 
        var isPostalOptional = ($sPostalInputBox.hasClass("is-postal-optional") == true && sPostalCode == "");

        if (IsSearchable(addressConstantID.DRP_SHIPPING_COUNTRY) && isPostalOptional == false) {

            if (IsPostalFormatInvalid(sCountry, sPostalCode)) {

                $sPostalInputBox.addClass(constantErrorTags.INVALID_POSTAL);

                if (ConvertStringToLower($(".shipping-zip-city-other-place-holder").css("display")) == "none") {
                    $(constantID.DIV_SHIPPING_ENTER_POSTAL).html(ise.StringResource.getString("customersupport.aspx.40"));
                }

                postalIsGood = false;
            }
        }

    }


    /*
    
    All is Good:
    
    -> Profile Info
    -> Billing Info
    -> Shipping Info [if app config AllowShipToDifferentThanBillTo is set to TRUE]
    -> Email Format, 
    -> Postal Pormat, 
    -> Password Matched( and strong [if app config UseStrongPwd is set to TRUE] 
    -> Checked Over13 [if app config RequireOver13Checked is set to TRUE] 
    -> Business Type [if app config VAT.Enabled && VAT.ShowTaxFieldOnRegistration is set to TRUE]

    Do revalidation of POSTAL (billing and shipping[if app config AllowShipToDifferentThanBillTo is set to TRUE])
    Do create account if ALL is ok

    */

    if (postalIsGood == false) {
        hideProgressIndicator(constantID.DIV_SAVE_ACCOUNT_LOADER, constantID.DIV_SAVE_ACCOUNT_BUTTON_CONTAINER);
        return false;
    }


    if (appConfigs.IS_SHOW_SHIPPING && IsElementChecked("copyBillingInfo")) {
        sPostalCode = bPostalCode;
        sCountry = bCountry;
        sStateCode = bStateCode;
    }

    var data = new Object();
    data.country = bCountry;
    data.postal = bPostalCode;
    data.stateCode = bStateCode;
    data.shipToCountry = sCountry;
    data.shipToPostal = sPostalCode;
    data.shipToStateCode = sStateCode;

    validateSignUpPostalCodes(data, submitSignUpForm);
}

function validateSignUpPostalCodes(o, callback) {

    var data = {
        country: o.country,
        postal: o.postal,
        stateCode: o.stateCode,
        shipToCountry: o.shipToCountry,
        shipToPostal: o.shipToPostal,
        shipToStateCode: o.shipToStateCode
    }

    var successCallback = function (result) {

        var response = parseInt(result.d);

        $billingPostalInputBox = GetObjectControl(addressConstantID.INPUT_BILLING_POSTAL);
        $billingStateInputBox = GetObjectControl(addressConstantID.INPUT_BILLING_STATE);

        $shippingPostalInputBox = GetObjectControl(addressConstantID.INPUT_SHIPPING_POSTAL);
        $shippingStateInputBox = GetObjectControl(addressConstantID.INPUT_SHIPPING_STATE);

        $billingPostalInputBox.removeClass(constantErrorTags.POSTAL_NOT_FOUND);
        $billingStateInputBox.removeClass(constantErrorTags.STATE_NOT_FOUND);

        $shippingPostalInputBox.removeClass(constantErrorTags.POSTAL_NOT_FOUND);
        $shippingStateInputBox.removeClass(constantErrorTags.STATE_NOT_FOUND);

        if (response == 0) {
            callback(data);
            return false;
        }

        hideProgressIndicator(constantID.DIV_SAVE_ACCOUNT_LOADER, constantID.DIV_SAVE_ACCOUNT_BUTTON_CONTAINER);

        if (response == 1) {
            $billingPostalInputBox.addClass(constantErrorTags.POSTAL_NOT_FOUND);
            $billingStateInputBox.addClass(constantErrorTags.STATE_NOT_FOUND);
            $billingPostalInputBox.focus();
            return false;
        }

        if (response == 2) {
            $shippingPostalInputBox.addClass(constantErrorTags.POSTAL_NOT_FOUND);
            $shippingStateInputBox.addClass(constantErrorTags.STATE_NOT_FOUND);
            $shippingPostalInputBox.focus();
            return false;
        }

        if (response == 3) {

            $billingPostalInputBox.addClass(constantErrorTags.POSTAL_NOT_FOUND);
            $billingStateInputBox.addClass(constantErrorTags.STATE_NOT_FOUND);

            $shippingPostalInputBox.addClass(constantErrorTags.POSTAL_NOT_FOUND);
            $shippingStateInputBox.addClass(constantErrorTags.STATE_NOT_FOUND);

            $billingPostalInputBox.focus();
            return false;
        }



    }

    var failedCallback = function (result) {
        callback(data);
    }

    var message = $.trim(ise.StringResource.getString("createaccount.aspx.159"));

    if (typeof message != constants.TYPE_UNDEFINED && message != constants.EMPTY) {
        ShowProcessMessage(message, "error-summary", constantID.DIV_SAVE_ACCOUNT_LOADER, constantID.DIV_SAVE_ACCOUNT_BUTTON_CONTAINER);
    }

    AjaxCallCommon("ActionService.asmx/ValidatePostalCode", data, successCallback, failedCallback);
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

        hideProgressIndicator(constantID.DIV_UPDATE_ADDRESS_LOADER, constantID.DIV_UPDATE_ADDRESS_BUTTON_CONTAINER);
    }

    var failedCallback = function (result) {
        callback(data);
    }

    var message = $.trim(ise.StringResource.getString("createaccount.aspx.159"));
    if (typeof message != constants.TYPE_UNDEFINED && message != constants.EMPTY) {
        ShowProcessMessage(message, "error-summary", constantID.DIV_UPDATE_ADDRESS_LOADER, constantID.DIV_UPDATE_ADDRESS_BUTTON_CONTAINER);
    }

    AjaxCallCommon("ActionService.asmx/ValidatePostalCode", data, successCallback, failedCallback);
}

function submitSignUpForm(data) {

    var callback = function () {

        var $buttonCreateAccount = $(constantID.BTN_HIDDEN_SIGNUP);
        var $billingHiddenCityState = $("#billingTxtCityStates");

        var bCityStates = GetInputValue(addressConstantID.HIDDEN_BILLING_CITYSTATE)

        $billingHiddenCityState.val(bCityStates);

        if (bCityStates == constants.TYPE_OTHER) {
            $billingHiddenCityState.val(GetInputValue(addressConstantID.INPUT_BILLING_STATE) + ", " + GetInputValue(addressConstantID.INPUT_BILLING_CITY));
        }

        if (appConfigs.IS_SHOW_SHIPPING) {

            var $ShippingHiddenCityState = $("#shippingTxtCityStates");
            var sCityStates = GetInputValue(addressConstantID.HIDDEN_SHIPPING_CITYSTATE)

            $ShippingHiddenCityState.val(sCityStates);

            if (sCityStates == constants.TYPE_OTHER) {
                $ShippingHiddenCityState.val(GetInputValue(addressConstantID.INPUT_SHIPPING_STATE) + "," + GetInputValue(addressConstantID.INPUT_SHIPPING_CITY));
            }

        }

        ShowProcessMessage(ise.StringResource.getString("createaccount.aspx.125"), "error-summary", constantID.DIV_SAVE_ACCOUNT_LOADER, constantID.DIV_SAVE_ACCOUNT_BUTTON_CONTAINER);
        $buttonCreateAccount.trigger("click");
    }

    if (appConfigs.IS_GET_ADDRESS_MATCH == false || data.postal == "" || (data.shipToPostal == "" && appConfigs.IS_SHOW_SHIPPING)) {
        callback();
        return false;
    }

    if (typeof ($.fn.RealTimeAddressVerification) != constants.TYPE_UNDEFINED) {
        $.fn.RealTimeAddressVerification.requestAddressMatch(callback);
    } else {
        callback();
    }

}

function submitAddNewAddresForm(data) {

    var callback = function () {

        var $hiddenCityState = $("#txtCityStates");
        var cityStates = GetInputValue(addressConstantID.HIDDEN_CITYSTATE);

        $hiddenCityState.val(cityStates);
        if (cityStates == constants.TYPE_OTHER) {
            var cityStates = GetInputValue(addressConstantID.INPUT_STATE) + ", " + GetInputValue(addressConstantID.INPUT_CITY);
            $("#txtCityStates").val(cityStates);
        }

        ShowProcessMessage(ise.StringResource.getString("selectaddress.aspx.9"), "error-summary", constantID.DIV_UPDATE_ADDRESS_LOADER, constantID.DIV_UPDATE_ADDRESS_BUTTON_CONTAINER);
        $(constantID.BTN_ADD_ADDRESS).trigger("click");
    }

    if (appConfigs.IS_GET_ADDRESS_MATCH == false || data.postal == "") {

        callback();
        return false;
    }

    if (typeof ($.fn.RealTimeAddressVerification) != constants.TYPE_UNDEFINED) {
        $.fn.RealTimeAddressVerification.requestAddressMatch(callback);
    } else {
        callback();
    }
}

function submitEditAddresForm(data) {

    var callback = function () {

        var $hiddenCityState = $("#txtCityStates");
        var cityStates = GetInputValue(addressConstantID.HIDDEN_CITYSTATE);

        $hiddenCityState.val(cityStates);
        if (cityStates == constants.TYPE_OTHER) {
            var cityStates = GetInputValue(addressConstantID.INPUT_STATE) + ", " + GetInputValue(addressConstantID.INPUT_CITY);
            $("#txtCityStates").val(cityStates);
        }

        ShowProcessMessage(ise.StringResource.getString("selectaddress.aspx.8"), "error-summary", constantID.DIV_UPDATE_ADDRESS_LOADER, constantID.DIV_UPDATE_ADDRESS_BUTTON_CONTAINER);
        $(constantID.BTN_UPDATE_ADDRESS).trigger("click");
    }

    if (appConfigs.IS_GET_ADDRESS_MATCH == false || data.postal == "") {
        callback();
        return false;
    }

    if (typeof ($.fn.RealTimeAddressVerification) != constants.TYPE_UNDEFINED) {
        $.fn.RealTimeAddressVerification.requestAddressMatch(callback);
    } else {
        callback();
    }
}


function hideProgressIndicator(id, container) {
    $("#" + id).fadeOut("slow", function () {
        $("#" + container).fadeIn("slow");
    });
}