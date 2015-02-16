var _imRegistered = false;
var _finalButtonText = "";

$(document).ready(function () {

    /* .ready() 

    -> execute the following functions when the DOM is fully loaded
        
    1. InitilizeOnePageInfo()
    2. EventsListener()
    3. CreditCardPanelWorkAround()
    4. ClearHiddentFieldsValue()
    5. DisplayErrorMessagePlaceHolder()
        
    note: each of the functions has attached definition, read it.

    */

    InitPageContent();
    InitCreditCardInfo();
    InitPlugIn();

    EventsListener();

    LoadCheckoutStringResources();

    var callback = function () { };
    LoadCheckoutRequiredAppConfiguration(callback);

});

function LoadCheckoutRequiredAppConfiguration(callback) {

    var keys = new Array();

    keys.push("VAT.ShowTaxFieldOnRegistration");
    keys.push("RequireOver13Checked");
    keys.push("AllowShipToDifferentThanBillTo");
    keys.push("AllowCustomerDuplicateEMailAddresses");
    keys.push("VAT.Enabled");
    keys.push("UseShippingAddressVerification");
    keys.push("RequireTermsAndConditionsAtCheckout");
    keys.push("SagePay.PaymentTerm");

    ise.Configuration.loadResources(keys, callback);

}

function LoadCheckoutStringResources() {

    var keys = new Array();

    keys.push("createaccount.aspx.94");
    keys.push("createaccount.aspx.123");

    keys.push("checkout1.aspx.9");
    keys.push("checkout1.aspx.45");
    keys.push("checkout1.aspx.46");
    keys.push("checkout1.aspx.47");
    keys.push("checkout1.aspx.48");

    keys.push("selectaddress.aspx.6");
    keys.push("checkoutpayment.aspx.5");

    keys.push("checkoutshipping.aspx.9");
    keys.push("checkoutshipping.aspx.10");
    keys.push("checkoutshipping.aspx.11");
    keys.push("checkoutshipping.aspx.12");

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

    keys.push("checkoutpayment.aspx.25");
    keys.push("checkoutpayment.aspx.65");

    var callback = function () { }
    ise.StringResource.loadResources(keys, callback);
}


function EventsListener() {

    $("#im-over-13-years-of-age").click(function () {

        $("#ise-message-tips").fadeOut("slow");
        $(this).removeClass("required-input");

    });

    $("#ise-message-tips").click(function () {
        $("#ise-message-tips").fadeOut("slow");
    });

    $("#checkoutpayment-submit-button").click(function () {
        var requiresTermsAndConditions = ise.Configuration.getConfigValue("RequireTermsAndConditionsAtCheckout");
        if (requiresTermsAndConditions == "true") {
            var isNoPaymentRequired = Boolean($("#ctrlPaymentTerm_hidNoPaymentRequired").val());
            var isTermsAndConditionsChecked = $("#ctrlPaymentTerm_termsAndConditionsChecked").is(':checked');

            // if nopayment required (zero total), we'll continue checkout whether terms and conditions is checked or not
            if (isTermsAndConditionsChecked || isNoPaymentRequired) {
                CompletePurchase();
            }
            else {
                ShowTermsAndConditionsMessage();
            }
        }
        else {
            CompletePurchase();
        }
    });

}

function InitPageContent() {

    /* update billing form value - get billing address */

    var primaryAddressCode = $(".is-primary-address").val();

    if (typeof ($("input[name='credit-card-code']").val()) != "undefined") {

        GetCreditCardInfo(primaryAddressCode, true, "");

    } else {

        if(typeof(primaryAddressCode) != "undefined") GetCreditCardInfo(primaryAddressCode, false, "");
    }

    /* <-- */

    if (IsAnonymousUser()) {
        $("#billing-address-grid").addClass("display-none");
        $("#billing-details-place-holder").addClass("display-none");
    } else {
        $("#billing-address-grid").removeClass("display-none");
        $("#billing-details-place-holder").removeClass("display-none");
    }

    /* <-- */

}

function CompletePurchase() {

    if (IsAnonymousUser()) {
        $("#btnDoProcessPayment").trigger("click");
        return true;
    }

    var callback = function () {

        /* if user is anonymous and payment term is not credit card skip address validation */

        if ($("input[name='ctrlPaymentTerm$']:checked").attr("pm") == "Credit Card") {
            ValidateBillingAddress();
        } else {
            $("#btnDoProcessPayment").trigger("click");
        }
    };

    if ($("input[name='ctrlPaymentTerm$']:checked").attr("pm") != "Credit Card") {

        var cardCode = $("input[name='credit-card-code']").val();

        if (typeof (cardCode) != "undefined")  {
            GetCreditCardInfo(cardCode, true, callback);
        } else {
            GetCreditCardInfo($("input[name = 'multiple-billing-address']").val(), false, callback);
        }

    } else {
        callback();
    }
}

function IsAnonymousUser(){
    
    if($.trim($("#isRegistered").html()).toLowerCase() == "false") return true;
    return false;
}

function ClearCreditCardInfo(cardCode, counter) {

    /* definition:
        
    1. This function is action on opc-clearcard (anchor html control / link) click event
    2. Calls the action service ClearCreditCardInfo
    3. After process is successfully execute clear selected save cc info row

    */

    var jsonText = JSON.stringify({ cardCode: cardCode });

    $.ajax({
        type: "POST",
        url: "ActionService.asmx/ClearCreditCardInfo",
        data: jsonText,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (result) {

            $("#" + counter + "-credit-card-type").html("---");
            $("#" + counter + "-credit-card-expiry").html("---");
            $("#" + counter + "-credit-card-clear").html("---");
            $("#" + counter + "-credit-card-description").html("");

            DisableControlPayment(false);

        },
        fail: function (result) { }
    });

}

function InitCreditCardInfo() {

    /*
    This is a workaround i did to manipulate the style of PaymentTermControl

    Handles the following event:

    1. ctrlPayment radio button click Event
    2. saved cc info radio button click Event

    On input[name='ctrlPaymentTerm$'] click event:

    1. Checks if id = ctrlPaymentTerm_ctrlPaymentTerm_1
    2. If #1 condition is TRUE: append card-description html input (text) control on save-as-credit-place-holder (div)
    3. Slide down credit card options
    4. if #1 condition is FALSE: slide up credit-card-options place holder (div)
    5. Store the attribute (pm) of the selected payment control on the variable: selectedPayment

    6. if variable selectedPayment value is Credit Card and Credit Card description place holder is not empty
    -> fade in save-as-credit-place-holder (div) and override CreditCardPaymentMethodPanel border style
    -> slide down credit card options

    On input[name='credit-card-code'] click event:

    1. Assign the value of selected option on variable: cardCode
    2. Calls web service GetCreditCardInfo (required cardCode parameter)

    On .opc-clearcard click event:

    1. Get the attribute (id) of the selected option
    2. Splits the id using "::" as separator
    3. Assign element with an index of 1 to variable cardCode
    4. Assign element wit and index of 2 to variable counter
    5. Calls ClearCreditCardInfo function (requires params :cardCode and counter)
       
    */

    CreditInfoEventHandlers();

    if (!IsAnonymousUser()) {
        ShowBillingAddress("");
    }

    CreditCardPanelStyles_Workaround();
}

function CreditInfoEventHandlers(){

    $("input[name='ctrlPaymentTerm$']").click(function () {
        
        if (!IsAnonymousUser()) {
            ShowBillingAddress($(this).attr("pm"));
        }

    });

    $("input[name='credit-card-code']").click(function () {
        GetCreditCardInfo($(this).val(), true, "");
    });

    $("input[name = 'multiple-billing-address']").click(function () {
        GetCreditCardInfo($(this).val(), false, "");
    });

    $(".opc-clearcard").click(function () {

        var thisId = $(this).attr("id");
        var str = thisId.split("::");
        var cardCode = str[1];
        var counter = str[2];

        ClearCreditCardInfo(cardCode, counter);

    });

}

function ShowBillingAddress( selectedPayment) {

    $("#ise-message-tips").fadeOut("fast");

    if (selectedPayment == "") selectedPayment = $("input[name='ctrlPaymentTerm$']:checked").attr("pm");

    var selectedPaymentTerm = $("input[name='ctrlPaymentTerm$']:checked").attr("pr");

    if (selectedPayment == "Credit Card" && selectedPaymentTerm != ise.Configuration.getConfigValue("SagePay.PaymentTerm")) {

        if ($.trim($("#isTokenization").html()).toLowerCase() == "true") {

            $("#credit-card-options").slideDown("slow", function () {
                $("#billing-details-place-holder").slideDown("slow");
                $("#divSaveCardInfo").removeClass("display-none");
            });

        } else {

            $("#pnlBillingAddressGrid").slideDown("slow", function () {
                $("#billing-details-place-holder").slideDown("slow");
            });

        }

    } else {

        if ($.trim($("#isTokenization").html()).toLowerCase() == "true") {

            $("#credit-card-options").slideUp("slow", function () {
                $("#billing-details-place-holder").slideUp("slow");
                $("#divSaveCardInfo").addClass("display-none");
            });

        } else {
  
           $("#pnlBillingAddressGrid").slideUp("slow", function () {
                $("#billing-details-place-holder").slideUp("slow");
            });
        
        }

    }
    
}

function CreditCardPanelStyles_Workaround() {

    $(".RedirectPaymentMethodPanel > tbody > tr > td").css("border", "0");
    $(".RedirectPaymentMethodPanel > tbody > tr > td > span").addClass("strong-font");

    $("#ctrlPaymentTerm_nameOnCard").addClass("light-style-input");
    $("#ctrlPaymentTerm_cardNumber").addClass("light-style-input");
    $("#ctrlPaymentTerm_cvv").addClass("light-style-input");
    $("#ctrlPaymentTerm_cardType").addClass("light-style-input");
    $("#ctrlPaymentTerm_expirationMonth").addClass("light-style-input");
    $("#ctrlPaymentTerm_expirationYear").addClass("light-style-input");

    if (typeof ($("#ctrlPaymentTerm_startMonth")) != "undefined") {

        $("#ctrlPaymentTerm_startMonth").addClass("light-style-input");
    }

    if (typeof ($("#ctrlPaymentTerm_startYear")) != "undefined") {

        $("#ctrlPaymentTerm_startYear").addClass("light-style-input");

    }
}

function ValidateBillingAddress() {

    $("#ise-message-tips").fadeOut("fast");

    if (!IsBillingInfoGood()) {
        return false;
    }

    if (!IsBillingAddressGood()) {
        return false;
    }

    var billingState = "";
    var billingCountry = $("#BillingAddressControl_drpCountry").val();
    var billingPostal = $("#BillingAddressControl_txtPostal").val();
    var billingCityState = $("#billing-city-states").val();

    if (typeof (billingCityState) == "undefined" || billingCityState == "other") {

        billingCity = $("#BillingAddressControl_txtCity").val();
        billingState = $("#BillingAddressControl_txtState").val();

    } else {

        var bc = billingCityState.split(", ");

        if (bc.length > 1) {


            billingState = bc[0];
            billingCity = bc[1];


        } else {

            billingState = "";
            billingCity = bc[0];
          
        }

    }

    var isWithRequiredAge = ($("#im-over-13-years-of-age").attr('checked') == "checked"); 
    IsBillingInfoCorrect(billingCountry, billingPostal, billingState, isWithRequiredAge);
}

function IsBillingInfoCorrect(countryCode, postalCode, stateCode, isWithRequiredAge) {

    var successFunction = function (result) {
        
        if (result.d == "valid" || $("#BillingAddressControl_txtPostal").hasClass("skip")) {

            ShowProcessMessage(ise.StringResource.getString("checkout1.aspx.47"), "payment-form-error-container", "save-billing-method-loader", "billing-method-button");

            var callback = function () {

                var cityStates = $("#billing-city-states").val();

                if (cityStates != "other") {
                    $("#txtCityStates").val(cityStates);
                }
              
                $("#btnDoProcessPayment").trigger("click");

                if ($("#errorSummary_Board_Errors").children("li").length > 0) {
                  ShowFailedMessage("", "payment-form-error-container", "save-billing-method-loader", "billing-method-button");
                }
            }

            if (typeof ($.fn.RealTimeAddressVerification) != "undefined") {
                $.fn.RealTimeAddressVerification.requestAddressMatch(callback);
            } else {
                callback();
            }

            return true;
        }

        if (result.d != "valid") {

            switch (result.d) {

                case 'invalid-postal':

                    $("#BillingAddressControl_txtPostal").addClass("invalid-postal-zero");
                    $("#BillingAddressControl_txtPostal").focus();

                    ShowFailedMessage("", "payment-form-error-container", "save-billing-method-loader", "billing-method-button");

                    break;
                case 'invalid-state':

                    $("#BillingAddressControl_txtState").addClass("state-not-found");
                    $("#BillingAddressControl_txtState").focus();

                    ShowFailedMessage("", "payment-form-error-container", "save-billing-method-loader", "billing-method-button");

                    break;
                case 'required-over-13':

                    var hasRequired13CheckBox = $("#im-over-13-years-of-age").attr("name");

                    if (typeof (hasRequired13CheckBox) != "undefined") {

                        $("#im-over-13-years-of-age").addClass("required-input");

                        var thisLeft = $("#im-over-13-years-of-age").offset().left;
                        var thisTop = $("#im-over-13-years-of-age").offset().top;

                        $("#ise-message-tips").css("top", thisTop - 47);
                        $("#ise-message-tips").css("left", thisLeft - 17);

                        $("#ise-message").html(ise.StringResource.getString("createaccount.aspx.123"));
                        $("#ise-message-tips").fadeIn("slow");

                        ShowFailedMessage("", "payment-form-error-container", "save-billing-method-loader", "billing-method-button");

                    } else {

                        $("#btnDoProcessPayment").trigger("click");

                    }

                    break;
                default:

                    ShowFailedMessage(result.d, "payment-form-error-container", "save-billing-method-loader", "billing-method-button");

                    break;
            }
        }

    }

    var errorFunction = function (result) {
        ShowFailedMessage(result.d, "payment-form-error-container", "save-billing-method-loader", "billing-method-button");
    }

    var data = new Object();
    data.countryCode = countryCode;
    data.postalCode = postalCode;
    data.stateCode = stateCode;
    data.isWithRequiredAge = isWithRequiredAge;

    AjaxCallCommon("ActionService.asmx/IsBillingInfoCorrect", data, successFunction, errorFunction);

}

function RegisterThisCustomerShippingMethod(resources) {

    var lst = $.parseJSON(resources);

    for (var i = 0; i < lst.length; i++) {

        ise.StringResource.registerString(lst[i].Key, lst[i].Value);

    }
}

function IsBillingInfoGood() {

    // added if in case validation of billing will be triggered and billing address control is hidden - show it.


    $("#ise-message-tips").fadeOut("slow");

    var counter = 0;
    var goodForm = true;
    var formHasEmptyFields = false;
    var skip = false;
    var skipStateValidation = false;
    var thisObjectId = "";
    var inputBoxToSetFocus = "";
    var ids = new Array();

    ids.push("#spare");
    ids.push("#txtBillingContactName");
    ids.push("#txtBillingContactNumber");
    ids.push("#BillingAddressControl_txtStreet");
    ids.push("#BillingAddressControl_txtCity");
    ids.push("#BillingAddressControl_txtState");
    ids.push("#BillingAddressControl_txtPostal");

    $(".requires-validation").each(function () {   //-> scan all html controls with class .apply-behind-caption-effects

        var object = this;
        var thisObjectId = "#" + $(object).attr("id");
        var validateMe = jQuery.inArray(thisObjectId, ids);


        var inputValue = trimString($(this).val());
        if (inputValue == "" && validateMe > 0) {

            skip = false;

            var cssDisplay = $(".billing-zip-city-other-place-holder").css("display");
            var objectValue = inputValue;

            /* city control -->  
            
            If city control is on display and empty: validate
                
            city control <-- */

            if (thisObjectId == "#BillingAddressControl_txtCity") {

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

            // -> validation (IsEmpty) for billing states input:

            if (thisObjectId == "#BillingAddressControl_txtState") {

                cssDisplay = $(".billing-zip-city-other-place-holder").css("display");
                var status = IsCountryWithStates("#BillingAddressControl_drpCountry");

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

            var id = $(this).attr("id");
            if (IsPostalInputBoxOptional(id)) {
                skip = true;
            }

            if (skip == false && validateMe > 0) {

                $(this).removeClass("current-object-on-focus");
                $(this).addClass("required-input");

                /* Points mouse cursor on the first input with no value to render bubble message */

                if (counter == 0) {

                    thisObjectId = "#" + $(this).attr("id");
                    inputBoxToSetFocus = thisObjectId;

                    $(this).addClass("current-object-on-focus");
                    //   $(this).focus();

                }

                formHasEmptyFields = true;
                counter++;
            }
        }

    });


    if (formHasEmptyFields) {

        if (!IsBillingAddressControlHidden()) {
            $(inputBoxToSetFocus).focus();
        }

        return false;
    }


    if (goodForm) {

        var b_cityStates = $("#billing-city-states").val();

        if (typeof (b_cityStates) == "undefined") {

            if (!IsBillingAddressControlHidden()) {
                $("#BillingAddressControl_txtPostal").focus();
            }
            return false;
        }
    }

    return true;
}

function IsBillingAddressControlHidden() {

    // This function is only use on address validation of billing
    // If billing address validation is triggered and billing address control is hidden - display it

    var isBillingAddressOnDisplay = $("#billing-details-place-holder").css("display");
    if (isBillingAddressOnDisplay == "none") {
        $("#billing-details-place-holder").css("display", "");
        return true;
    }

    return false;
}


function UpdateBillingInfoFormValues(cc, credit, adjustCityWidth, copyShippingInfo) {

    var contact = "";
    var email = "";
    var phone = "";
    var address = "";
    var city = "";
    var state = "";
    var postal = "";
    var country = "";
    var county = "";

    if (cc) {

        contact = credit.NameOnCard;
        phone = credit.PhoneNumber;
        address = credit.Address;
        city = credit.City;
        state = credit.State;
        postal = credit.Postal;
        country = credit.Country;
        county = credit.County;
    }

    $("#BillingAddressControl_drpCountry").val(country);

    if (contact != "") {

        $("#txtBillingContactName").val(contact);
        $("#lblBillingContactName").addClass("display-none");
    }

    if (phone != "") {
        $("#txtBillingContactNumber").val(phone);
        $("#lblBillingContactNumber").addClass("display-none");
    }

    if (address != "") {

        $("#BillingAddressControl_txtStreet").val(address);
        $("#BillingAddressControl_lblStreet").addClass("display-none");

    }

    // handle optional postal code :

    var $postalInputBox = $("#BillingAddressControl_txtPostal");
    var $postalLabel = $("#BillingAddressControl_lblPostal");

    var postalIsOptionalIn = $postalInputBox.attr("data-postalIsOptionalIn");

    var isPostalOptional = ($(this).ISEAddressFinder("isPostalCodeOptional", { 'postalIsOptionalIn': postalIsOptionalIn, 'country': country }));
    $postalInputBox.attr("data-isOptional", isPostalOptional);

    var postalLabelText = $postalInputBox.attr("data-labeltext");
    postalLabelText = $.trim(postalLabelText);

    if (isPostalOptional) {
        $postalInputBox.addClass("is-postal-optional");
        $postalLabel.html(postalLabelText + " (optional)");
    } else {
        $postalInputBox.removeClass("is-postal-optional");
        $postalLabel.html(postalLabelText);
    }

    if (postal != "") {

        $postalInputBox.val(postal);
        $postalLabel.addClass("display-none");

    } else {

        if (isPostalOptional) {
            $postalInputBox.val("");
            $postalLabel.removeClass("display-none");  
        }

    }

    if ($("#shipping-city-states").val() == "other") { } else { }

    if (city != "") {

        $("#BillingAddressControl_txtCity").val(city);
        $("#BillingAddressControl_lblCity").addClass("display-none");

    } else {

        $("#BillingAddressControl_lblCity").removeClass("display-none");

    }

    if (state != "") {

        $("#BillingAddressControl_txtState").val(state);
        $("#BillingAddressControl_txtState").removeClass("display-none");
        $("#BillingAddressControl_lblState").addClass("display-none");

        $("#BillingAddressControl_txtCity").removeClass("city-width-if-no-state");


    } else {

        $("#BillingAddressControl_txtState").val("");
        $("#BillingAddressControl_lblState").removeClass("display-none")

        if (adjustCityWidth) {

            $("#BillingAddressControl_lblState").addClass("display-none")
            $("#BillingAddressControl_txtState").addClass("display-none");
            $("#BillingAddressControl_txtCity").addClass("city-width-if-no-state");

        }

    }

    if (typeof ($("#BillingAddressControl_txtCounty").val()) != "undefined") {

        if (county != "") {

            $("#BillingAddressControl_lblCounty").addClass("display-none");
            $("#BillingAddressControl_txtCounty").val(county);

        } else {

            $("#BillingAddressControl_txtCounty").val(county);
            $("#BillingAddressControl_lblCounty").removeClass("display-none");

        }

    }

    $(".billing-zip-city-other-place-holder").fadeIn("Slow");
    $("#billing-enter-postal-label-place-holder").html("<input type='hidden' value='other' id='billing-city-states'>");
}

function IsBillingAddressGood() {

    /* Definition:
        
    1. Check if selected billing country is searchable
    2. if #1 is true, it validates the postal code format (see function IsPostalFormatInvalid)
    3. returns false if format is not valid else returns true

    */

    var $postalCodeInputBox = $("#BillingAddressControl_txtPostal");

    var b_citystateIsVisible = $(".billing-zip-city-other-place-holder").css("display");
    var b_citystateIsVisible = b_citystateIsVisible.toLowerCase();
    var b_country = $("#BillingAddressControl_drpCountry").val();
    var b_postalCode = $.trim($postalCodeInputBox.val());
    var b_searchable = IsCountrySearchable("#BillingAddressControl_drpCountry");
    var postalIsGood = true;
    var b_postalHasInvalidFormat = false;

    // optional postal code
    var b_isOptionalPostalCode =$postalCodeInputBox.hasClass("is-postal-optional") && b_postalCode == "";

    if (b_searchable == "true" && b_isOptionalPostalCode == false) {

        b_postalHasInvalidFormat = IsPostalFormatInvalid(b_country, b_postalCode);

        if (b_postalHasInvalidFormat) {

            $("#BillingAddressControl_txtPostal").addClass("invalid-postal");

            if (b_citystateIsVisible == "none") $("#billing-enter-postal-label-place-holder").html(ise.StringResource.getString("customersupport.aspx.40"));

            if (!IsBillingAddressControlHidden()) {
                $("#BillingAddressControl_txtPostal").focus();
            }

            postalIsGood = false;
        }

        if (!postalIsGood) return false;
    }

    

    return true;
}

function GetCreditCardInfo(cardCode, updateCreditCardInfo, callback) {
    var jsonText = JSON.stringify({ cardCode: cardCode });

    $.ajax({
        type: "POST",
        url: "ActionService.asmx/GetCreditCardInfo",
        data: jsonText,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (result) {

            if (result.d != "") {

                $("#txtCode").val(result.d.CreditCardCode);
                $("#ctrlPaymentTerm_nameOnCard").val(result.d.NameOnCard);

                UpdateBillingInfoFormValues(true, result.d, true, false);
                
                if (updateCreditCardInfo) {

                    AssignValueToControlPayment(result.d);

                    if (result.d.CardNumber != "" && result.d.RefNo != 0) {
                        DisableControlPayment(true);
                    } else {
                        DisableControlPayment(false);
                    }
                }
            }

            if (typeof (callback) != "undefined" && callback != "") {

                callback();

            }

        },
        fail: function (result) { }
    });
}

function AssignValueToControlPayment(creditCard) {

    /* This function is active on GetCreditCardInfo and .ready() functions */

    $("#ctrlPaymentTerm_cardNumber").val(creditCard.CardNumber);
    $("#ctrlPaymentTerm_cardType").val(creditCard.CardType);
    $("#ctrlPaymentTerm_expirationMonth").val(creditCard.ExpMonth);
    $("#ctrlPaymentTerm_expirationYear").val(creditCard.ExpYear);
    $("#ctrlPaymentTerm_cardDescription").val(creditCard.Description);

    $("#credit-card-masked").html("cardNumber");

    if (typeof ($("#ctrlPaymentTerm_startMonth")) != "undefined") {

        $("#ctrlPaymentTerm_startMonth").val(creditCard.StartMonth);
    }

    if (typeof ($("#ctrlPaymentTerm_startYear")) != "undefined") {

        $("#ctrlPaymentTerm_startYear").val(creditCard.StartYear);

    }

    $("#hidMaskCardNumber").val(creditCard.CardNumber);
    $("#hidCreditCardCode").val(creditCard.CreditCardCode);

}

function DisableControlPayment(disable) {

    /* This function is active on GetCreditCardInfo function
        
    1. Accepts a boolean param: disable
    2. If param: disable is true add disable attribute and class control-disabled to the following controls:
    ELSE: remove disabled attribute and class control-disabled / and clear the values of the controls

    -> ctrlPaymentTerm_NameOnCard
    -> ctrlPaymentTerm_CardNumber
    -> ctrlPaymentTerm_CardType
    -> ctrlPaymentTerm_ExpirationMonth
    -> ctrlPaymentTerm_ExpirationYear
    -> ctrlPaymentTerm_CVV
   
    */

    if (disable) {

        $("#ctrlPaymentTerm_nameOnCard").attr("readonly", "true");
        $("#ctrlPaymentTerm_nameOnCard").addClass("control-disabled");

        $("#ctrlPaymentTerm_cardNumber").attr("readonly", "true");
        $("#ctrlPaymentTerm_cardNumber").addClass("control-disabled");

        $("#ctrlPaymentTerm_cardType").attr("disabled", "disabled");
        $("#ctrlPaymentTerm_cardType").addClass("control-disabled");

        $("#ctrlPaymentTerm_expirationMonth").attr("disabled", "disabled");
        $("#ctrlPaymentTerm_expirationMonth").addClass("control-disabled");

        $("#ctrlPaymentTerm_expirationYear").attr("disabled", "disabled");
        $("#ctrlPaymentTerm_expirationYear").addClass("control-disabled");

        $("#ctrlPaymentTerm_cvv").attr("readonly", "true");
        $("#ctrlPaymentTerm_cvv").addClass("control-disabled");

        if (typeof ($("#ctrlPaymentTerm_startMonth")) != "undefined") {

            $("#ctrlPaymentTerm_startMonth").attr("disabled", "disabled");
            $("#ctrlPaymentTerm_startMonth").addClass("control-disabled");
        }

        if (typeof ($("#ctrlPaymentTerm_startYear")) != "undefined") {

            $("#ctrlPaymentTerm_startYear").attr("disabled", "disabled");
            $("#ctrlPaymentTerm_startYear").addClass("control-disabled");

        }

        $("#ctrlPaymentTerm_cvv").parent("td").parent("tr").addClass("display-none");

    } else {

        $("#ctrlPaymentTerm_cardNumber").val("");
        $("#ctrlPaymentTerm_cardType").val("");
        $("#ctrlPaymentTerm_expirationMonth").val("");
        $("#ctrlPaymentTerm_expirationYear").val("");
        $("#ctrlPaymentTerm_cardDescription").val("");

        $("#ctrlPaymentTerm_nameOnCard").removeAttr("readonly");
        $("#ctrlPaymentTerm_nameOnCard").removeClass("control-disabled");

        $("#ctrlPaymentTerm_cardNumber").removeAttr("readonly");
        $("#ctrlPaymentTerm_cardNumber").removeClass("control-disabled");

        $("#ctrlPaymentTerm_cardType").removeAttr("disabled");
        $("#ctrlPaymentTerm_cardType").removeClass("control-disabled");

        $("#ctrlPaymentTerm_expirationMonth").removeAttr("disabled");
        $("#ctrlPaymentTerm_expirationMonth").removeClass("control-disabled");

        $("#ctrlPaymentTerm_expirationYear").removeAttr("disabled");
        $("#ctrlPaymentTerm_expirationYear").removeClass("control-disabled");

        $("#ctrlPaymentTerm_cvv").removeAttr("readonly");
        $("#ctrlPaymentTerm_cvv").removeClass("control-disabled");

        $("#credit-card-masked").html("");


        if (typeof ($("#ctrlPaymentTerm_startMonth")) != "undefined") {

            $("#ctrlPaymentTerm_startMonth").removeAttr("disabled");
            $("#ctrlPaymentTerm_startMonth").removeClass("control-disabled");
        }

        if (typeof ($("#ctrlPaymentTerm_sStartYear")) != "undefined") {

            $("#ctrlPaymentTerm_startYear").removeAttr("disabled");
            $("#ctrlPaymentTerm_startYear").removeClass("control-disabled");

        }

        $("#ctrlPaymentTerm_cvv").parent("td").parent("tr").removeClass("display-none");
    }
}

function InitPlugIn() {

    /* 
    This function append ISEAddressFinder (see jquery/address.verification.js) plugin to the followings on (checkout1.aspx):

    1. input text: BillingAddressControl_txtPostal


    This function appends ISEBubbleMessage (see jquery/bubble.message.js) plugin to the following controls (checkout1.aspx):

    1. BillingAddressControl_txtStreet
    2. BillingAddressControl_txtPostal
    3. BillingAddressControl_txtCity
    4. BillingAddressControl_txtState
    5. txtBillingContactName
    6. txtBillingContactNumber

    */

    $("#BillingAddressControl_txtPostal").ISEAddressFinder();


    $("#BillingAddressControl_txtStreet").ISEBubbleMessage({ "input-id": "BillingAddressControl_txtStreet", "label-id": "BillingAddressControl_lblStreet" });
    $("#BillingAddressControl_txtPostal").ISEBubbleMessage({ "input-id": "BillingAddressControl_txtPostal", "label-id": "BillingAddressControl_lblPostal", "input-mode": "billing-postal", "address-type": "Billing" });
    $("#BillingAddressControl_txtCity").ISEBubbleMessage({ "input-id": "BillingAddressControl_txtCity", "label-id": "BillingAddressControl_lblCity" });
    $("#BillingAddressControl_txtState").ISEBubbleMessage({ "input-id": "BillingAddressControl_txtState", "label-id": "BillingAddressControl_lblState", "input-mode": "billing-state" });

    if (typeof ($("#BillingAddressControl_txtCounty").val()) != "undefined") {

        $("#BillingAddressControl_txtCounty").ISEBubbleMessage({ "input-id": "BillingAddressControl_txtCounty", "label-id": "BillingAddressControl_lblCounty", "optional": true });

    }

    $("#txtBillingContactName").ISEBubbleMessage({ "input-id": "txtBillingContactName", "label-id": "lblBillingContactName" });
    $("#txtBillingContactNumber").ISEBubbleMessage({ "input-id": "txtBillingContactNumber", "label-id": "lblBillingContactNumber" });
    
}

function ShowTermsAndConditionsMessage() {

    $("#ctrlPaymentTerm_termsAndConditionsChecked").addClass("required-input");

    var thisLeft = $("#ctrlPaymentTerm_termsAndConditionsChecked").offset().left;
    var thisTop = $("#ctrlPaymentTerm_termsAndConditionsChecked").offset().top;

    $("#ise-message-tips").css("top", thisTop - 47);
    $("#ise-message-tips").css("left", thisLeft -19);

    $("#ise-message").html(ise.StringResource.getString("checkoutpayment.aspx.5"));
    $("#ise-message-tips").fadeIn("slow");

}

