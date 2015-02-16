(function ($) {

    var controlIdOnFocus = "";
    var labelIdOnFocus = "";

    var methods = {
        init: function (options) {

            var settings = $.extend({
                "input-id": "",
                "label-id": "",
                "input-mode": "normal",
                "address-type": "",
                "optional": false
            }, options);

            $(this).ISEBubbleMessage("initEventsListener", settings);

        },
        initEventsListener: function (settings) {

            $this = $("#" + settings["input-id"]);

            // -- IE Fix on label 
            $("#" + settings["label-id"]).unbind("click").click(function () {
                $(this).addClass("display-none");

                $("#" + settings["input-id"]).ISEBubbleMessage("cleanInputControl", { "control-id": settings["input-id"], "input-mode": settings["input-mode"], "label-id": settings["label-id"] });
                $("#" + settings["input-id"]).val("").addClass("ie-placeholder-fix").trigger("focus");
            });
            // <--

            var currentValue = $this.val();
            currentValue = (typeof (currentValue) == "undefined") ? "" : $.trim(currentValue);

            if (currentValue != "") {
                $("#" + settings["label-id"]).addClass("display-none");
            } else {
                $this.val(currentValue);
            }

            if (!settings["optional"]) {
                $this.addClass("requires-validation");
            }

            $this.focus(function () {

                var o = $(this);
                var id = o.attr("id");

                var value = GetInputValue(id);
                value = (typeof (value) == "undefined") ? "" : $.trim(value);

                // Optional Postal Code: if postal is optional and value is empty return false to skip checking of previous control on focus and finding of error tags
                var isPostalInputBox = IsInArray(['BillingAddressControl_txtPostal', 'ShippingAddressControl_txtPostal', 'AddressControl_txtPostal'], id);

                if (o.hasClass("editable-content") || (isPostalInputBox && o.hasClass("is-postal-optional") == true && value == "")) {
                    return false;
                }

                if (!settings["optional"] && o.hasClass("ie-placeholder-fix") == false) {
                    o.ISEBubbleMessage("checkPreviousControlOnFocus", settings);
                    o.ISEBubbleMessage("lookForAnErrorTag", { "input-id": id, "address-type": settings["address-type"] });
                }

                o.removeClass("ie-placeholder-fix");

            });


            $this.blur(function () {

                var o = $(this);
                if (o.hasClass("editable-content")) return false;

                var value = o.val();
                value = (typeof (value) == "undefined") ? "" : $.trim(value);

                if (value == "") {
                    $("#" + settings["label-id"]).removeClass("display-none");
                    $("#" + settings['label-id']).css("color", labelFadeInColor);
                }

                o.val(value);

            });

            $this.keypress(function (e) {
                var o = $(this);
                if (o.hasClass("editable-content")) return false;

                var keycode = e.keyCode ? e.keyCode : e.which;
                if (keycode > 0) {
                    o.ISEBubbleMessage("cleanInputControl", { "control-id": settings["input-id"], "input-mode": settings["input-mode"], "label-id": settings["label-id"] });
                }

            });


            $this.keydown(function (e) {

                var keycode = e.keyCode ? e.keyCode : e.which;
                var o = $(this);


                var value = o.val();
                value = (typeof (value) == "undefined") ? "" : $.trim(value);

                if (value == "" || keycode === 8) {
                    if (o.hasClass("editable-content")) return false;
                    o.ISEBubbleMessage("cleanInputControl", { "control-id": settings["input-id"], "input-mode": settings["input-mode"], "label-id": settings["label-id"] });
                }

            });


            $this.mousedown(function (e) {
                var keycode = e.keyCode ? e.keyCode : e.which;
                var o = $(this);

                var value = o.val();
                value = (typeof (value) == "undefined") ? "" : $.trim(value);

                if (value == "" || keycode === 8) {
                    if (o.hasClass("editable-content")) return false;
                    o.ISEBubbleMessage("cleanInputControl", { "control-id": settings["input-id"], "input-mode": settings["input-mode"], "label-id": settings["label-id"] });
                }

            });

        },
        checkPreviousControlOnFocus: function (settings) {

            $this = $("#" + settings["input-id"]);

            var objectOnfocus = $(".current-object-on-focus").attr("id");
            var objectType = _prevObjectType;
            _prevObjectType = settings["input-mode"];

            $(".requires-validation").each(function () { $(this).removeClass("current-object-on-focus"); });

            if (typeof (objectOnfocus) != "undefined") {

                if ($("#" + objectOnfocus).val() == "") {

                    $("#" + objectOnfocus).addClass("required-input");
                }

                var object = [objectOnfocus, objectType];
                $this.ISEBubbleMessage("validate", object);

            }

            $("#" + settings['label-id']).animate({ color: labelFadeOutColor }, 500);
            $("#" + settings["input-id"]).addClass("current-object-on-focus");

        },
        cleanInputControl: function (settings) {

            $("#" + settings["label-id"]).addClass("display-none");

            var $this = $("#" + settings["control-id"]);

            switch (settings['input-mode']) {

                case "email":

                    $this.removeClass("invalid-email");
                    $this.removeClass("email-duplicates");

                    break;
                case "billing-postal":

                    $this.removeClass("invalid-postal");
                    $this.removeClass("invalid-postal-zero");
                    $this.removeClass("invalid-postal-many");

                    break;
                case "shipping-postal":

                    $this.removeClass("invalid-postal");
                    $this.removeClass("invalid-postal-zero");
                    $this.removeClass("invalid-postal-many");

                    break;
                case "password":

                    $this.removeClass("password-not-match");
                    $this.removeClass("password-not-strong");
                    $this.removeClass("password-length-invalid");

                    break;
                case "password-confirmation":

                    $this.removeClass("password-not-match");
                    $this.removeClass("password-not-strong");

                    break;
                default:
                    break;
            }

            $this.removeClass("required-input");
            $("#ise-message-tips").fadeOut("slow");

        },
        lookForAnErrorTag: function (options) {

            var attributes = $.extend({
                "input-id": "",
                "address-type": ""
            }, options);

            var $this = $("#" + attributes["input-id"]);

            var thisClass = $this.attr("class");
            var classes = thisClass.split(" ");

            var leftPosition = $(this).offset().left;
            var leftDeduction = 17;
            var topPosition = $(this).offset().top;
            var topDeduction = 47;
            var message = "";
            var showTips = false;

            switch (classes[classes.length - 2]) {
                case "required-input":

                    message = ise.StringResource.getString("customersupport.aspx.15");
                    showTips = true;

                    break;
                case "invalid-email":

                    message = ise.StringResource.getString("customersupport.aspx.16");
                    showTips = true;

                    break;
                case "email-duplicates":

                    message = ise.StringResource.getString("createaccount.aspx.94");
                    showTips = true;

                    break;
                case "invalid-postal":

                    leftDeduction = 17;
                    topDeduction = 65;
                    message = ise.StringResource.getString("customersupport.aspx.38") + "<div class='clear-both height-5'></div> Please click <a id='show-postal-listing-dialog-many' onClick='FindPostal(\"" + attributes["address-type"] + "\")' href='javascript:void(1);'>HERE</a> to select your postal code";
                    showTips = true;

                    break;
                case "state-not-found":

                    var addressType = attributes["address-type"];

                    switch (attributes["input-id"]) {
                        case "ShippingAddressControl_txtState":
                            addressType = "Shipping";
                            break;
                        case "BillingAddressControl_txtState":
                            addressType = "Billing";
                            break;
                        default:
                            break;
                    }

                    leftDeduction = 17;
                    topDeduction = 65;

                    message = ise.StringResource.getString("customersupport.aspx.39") + "<div class='clear-both height-5'></div> Please click <a id='show-postal-listing-dialog-many' onClick='FindPostal(\"" + addressType + "\")' href='javascript:void(1);'>HERE</a> to select your postal code";
                    showTips = true;


                    break;
                case "invalid-postal-zero":

                    leftDeduction = 17;
                    topDeduction = 72;
                    message = ise.StringResource.getString("customersupport.aspx.17") + "<div class='clear-both height-12'></div> Please click <a id='show-postal-listing-dialog-many' onClick='FindPostal(\"" + attributes["address-type"] + "\")' href='javascript:void(1);'>HERE</a> to correct your postal code";

                    if (ise.Configuration.getConfigValue("AllowCustomPostal.Enabled") == "true") {
                        topDeduction = 92;
                        message += " <div class='clear-both height-5'></div>  or click <a href='javascript:void(1);' onClick='SkipValidationOnPostal(\"" + attributes["address-type"] + "\", true)'>HERE</a> to continue on using your entered postal code.";
                    }

                    showTips = true;

                    break;
                case "invalid-captcha":

                    message = ise.StringResource.getString("customersupport.aspx.18");
                    showTips = true;

                    break;
                case "password-not-match":

                    message = ise.StringResource.getString("createaccount.aspx.52");
                    showTips = true;

                    break;
                case "password-not-strong":

                    message = ise.StringResource.getString("createaccount.aspx.28");
                    if (message.length > 200) topDeduction = 88;

                    showTips = true;

                    break;
                case "password-length-invalid":

                    message = "Password length is not valid";
                    showTips = true;

                    break;
                case "lead-duplicates":

                    message = ise.StringResource.getString("leadform.aspx.20");
                    showTips = true;

                    break;
                default:
                    break;

            }

            if (showTips) {

                $this.ISEBubbleMessage("showTips",
                {
                    "top-position": topPosition,
                    "top-deduction": topDeduction,
                    "left-position": leftPosition,
                    "left-deduction": leftDeduction,
                    "message": message
                });

            }

        },
        showTips: function (options) {

            var attributes = $.extend({
                "top-position": 0,
                "top-deduction": 0,
                "left-position": 0,
                "left-deduction": 0,
                "message": ""
            }, options);

            $("#ise-message-tips").css("top", attributes["top-position"] - attributes["top-deduction"]);
            $("#ise-message-tips").css("left", attributes["left-position"] - attributes["left-deduction"]);

            $("#ise-message").html(attributes["message"]);
            $("#ise-message-tips").fadeIn("slow");

        },
        validate: function (object) {

            var $this = $("#" + object[0]);
            var skip = $this.hasClass("skip");

            switch (object[1]) {

                case "email":

                    var accountType = $this.parent("span").attr("data-accountType");
                    accountType = (typeof (accountType) == "undefined") ? "" : $.trim(accountType);

                    AjaxSecureEmailAddressVerification(false, $this.val(), accountType, function (response) {

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
                                    bubbleTipsClass = "request-error";
                                    break;
                            }


                            $this.addClass(bubbleTipsClass).attr("response-message", response);
                        }

                    }, function (response) {
                        $this.addClass("request-error").attr("response-message", response);
                    });

                    break;
                case "postal":

                    var country = $("#AddressControl_drpCountry").val();
                    var postal = $("#AddressControl_txtPostal").val();
                    var state = $("#AddressControl_txtState").val();

                    var formatIsInvalid = $this.ISEAddressFinder("isPostalFormatInvalid", { 'country': country, 'postal': postal });

                    break;
                case "shipping-postal":

                    var country = $("#ShippingAddressControl_drpCountry").val();
                    var postal = $("#ShippingAddressControl_txtPostal").val();
                    var state = $("#ShippingAddressControl_txtState").val();

                    var formatIsInvalid = $this.ISEAddressFinder("isPostalFormatInvalid", { 'country': country, 'postal': postal });

                    break;

                case "billing-postal":

                    var country = $("#BillingAddressControl_drpCountry").val();
                    var postal = $("#BillingAddressControl_txtPostal").val();
                    var state = $("#BillingAddressControl_txtState").val();

                    var formatIsInvalid = $this.ISEAddressFinder("isPostalFormatInvalid", { 'country': country, 'postal': postal });

                    break;
                case "state":

                    if (!skip) {

                        var country = $("#AddressControl_drpCountry").val();
                        var postal = $("#AddressControl_txtPostal").val();
                        var state = $("#AddressControl_txtState").val();

                        var formatIsInvalid = $this.ISEAddressFinder("isPostalFormatInvalid", { 'country': country, 'postal': postal });

                        if (!formatIsInvalid) {
                            $this.ISEAddressFinder("verifyStateCode",
                            {
                                "country-id": "#AddressControl_drpCountry",
                                "postal-id": "#AddressControl_txtPostal",
                                "city-id": "#AddressControl_txtCity",
                                "state-id": "#AddressControl_txtState",
                                "city-state-place-holder": ".zip-city-other-place-holder",
                                "enter-postal-label-place-holder": "#enter-postal-label-place-holder",
                                "city-states-id": "city-states"
                            });
                        }
                    }

                    break;
                case "shipping-state":

                    if (!skip) {
                        var country = $("#ShippingAddressControl_drpCountry").val();
                        var postal = $("#ShippingAddressControl_txtPostal").val();
                        var state = $("#ShippingAddressControl_txtState").val();

                        var formatIsInvalid = $this.ISEAddressFinder("isPostalFormatInvalid", { 'country': country, 'postal': postal });

                        if (!formatIsInvalid) {
                            $this.ISEAddressFinder("verifyStateCode",
                            {
                                "country-id": "#ShippingAddressControl_drpCountry",
                                "postal-id": "#ShippingAddressControl_txtPostal",
                                "city-id": "#ShippingAddressControl_txtCity",
                                "state-id": "#ShippingAddressControl_txtState",
                                "city-state-place-holder": ".shipping-zip-city-other-place-holder",
                                "enter-postal-label-place-holder": "#shipping-enter-postal-label-place-holder",
                                "city-states-id": "shipping-city-states"
                            });
                        }
                    }

                    break;
                case "billing-state":

                    if (!skip) {
                        var country = $("#BillingAddressControl_drpCountry").val();
                        var postal = $("#BillingAddressControl_txtPostal").val();
                        var state = $("#BillingAddressControl_txtState").val();

                        var formatIsInvalid = $this.ISEAddressFinder("isPostalFormatInvalid", { 'country': country, 'postal': postal });

                        if (!formatIsInvalid) {
                            $this.ISEAddressFinder("verifyStateCode", {
                                "country-id": "#BillingAddressControl_drpCountry",
                                "postal-id": "#BillingAddressControl_txtPostal",
                                "city-id": "#BillingAddressControl_txtCity",
                                "state-id": "#BillingAddressControl_txtState",
                                "city-state-place-holder": ".billing-zip-city-other-place-holder",
                                "enter-postal-label-place-holder": "#billing-enter-postal-label-place-holder",
                                "city-states-id": "billing-city-states"
                            });
                        }
                    }

                    break;

                case "password":
                    ValidatePassword("ProfileControl_txtPassword");
                    break;
                case "password-confirmation":

                    if ($("#ProfileControl_txtPassword").val() != $("#ProfileControl_txtConfirmPassword").val()) {

                        $("#ProfileControl_txtPassword").addClass("password-not-match");
                        $("#ProfileControl_txtConfirmPassword").addClass("password-not-match");

                    } else {

                        $("#ProfileControl_txtPassword").removeClass("password-not-match");
                        $("#ProfileControl_txtConfirmPassword").removeClass("password-not-match");
                    }

                    break;

                default:

                    break;
            }

        }
    };

    $.fn.ISEBubbleMessage = function (method) {

        if (methods[method]) {

            return methods[method].apply(this, Array.prototype.slice.call(arguments, 1));

        } else if (typeof method === 'object' || !method) {

            return methods.init.apply(this, arguments);

        } else {

            $.error('Method ' + method + ' does not exist on jQuery.tooltip');

        }

    };
})(jQuery);

function VerifyCaptcha(args) {

    /* -> This function looks into ActionService -> CreateLeadTaskController with task validate-captca 
    -> If mismatch add class to captcha control to indicates its status
    */

    var list = [args[0]];
    var jsonText = JSON.stringify({ list: list, task: "validate-captcha" });

    $.ajax({
        type: "POST",
        url: "ActionService.asmx/CreateLeadTaskController",
        data: jsonText,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (result) {

            if (result.d == "match") {

                $("#support-security-code").removeClass("support-invalid-captcha");

            } else {


                $("#support-security-code").addClass("support-invalid-captcha");

            }

        },
        fail: function (result) { }
    });

}

function ShowProcessMessage(Message, errorPlaceHolderID, messageLoaderId, buttonPlaceHolderId) {

    var thisProcessStringResource = Message;
    var lMessage = "<div style='float:left;width:12px;'><i class='icon icon-spin icon-spinner progress-indicator'></i></div> <div id='loader-container'>" + thisProcessStringResource + "</div>";

    $("#" + buttonPlaceHolderId).css("display", "none");

    $("#" + messageLoaderId).html("<div style='float:right' class='OPCLoadStep'>" + lMessage + "</div>");
    $("#" + messageLoaderId).fadeIn("slow");

    $("#" + errorPlaceHolderID).html("");
    $("#" + errorPlaceHolderID).fadeOut("slow");

}

function ShowFailedMessage(message, errorPlaceHolderID, messageLoaderId, buttonPlaceHolderId) {

    if (message == "") {

        $("#" + errorPlaceHolderID).removeClass("error-place-holder");

    } else {

        $("#" + errorPlaceHolderID).addClass("error-place-holder");

    }

    $("#" + errorPlaceHolderID).html(message);
    $("#" + errorPlaceHolderID).fadeIn("slow", function () {

        $("#" + messageLoaderId).fadeOut("fast", function () {

            $("#" + buttonPlaceHolderId).fadeIn("fast");

        });

    });


}


function PopulateResources(config, resources) {

    var lst = $.parseJSON(resources);

    for (var i = 0; i < lst.length; i++) {

        if (config) {

            ise.Configuration.registerConfig(lst[i].Key, lst[i].Value);

        } else {

            ise.StringResource.registerString(lst[i].Key, lst[i].Value);

        }

    }
}

function ShowBubbleTips(id, message) {

    var thisLeft = $(id).offset().left;
    var thisTop = $(id).offset().top;

    $("#ise-message-tips").css("top", thisTop - 47);
    $("#ise-message-tips").css("left", thisLeft - 17);

    $("#ise-message").html(message);
    $("#ise-message-tips").fadeIn("slow");

}

function GetStringResources(type, includeBubbleTipsMessages) {

    var keys = new Array();

    switch (type) {
        case "lead-form":

            keys.push("leadform.aspx.24");
            keys.push("createaccount.aspx.94");
            keys.push("leadform.aspx.20");
            keys.push("createaccount.aspx.81");
            keys.push("customersupport.aspx.40");
            keys.push("createaccount.aspx.158");
            keys.push("createaccount.aspx.159");

            break;
        case "customer-support":

            keys.push("customersupport.aspx.25");
            keys.push("createaccount.aspx.81");

            break;

        case "create-account":

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


            break;
        case "edit-profile":

            keys.push("createaccount.aspx.81");
            keys.push("createaccount.aspx.94");

            keys.push("createaccount.aspx.52");
            keys.push("createaccount.aspx.120");

            keys.push("createaccount.aspx.121");
            keys.push("createaccount.aspx.122");

            keys.push("selectaddress.aspx.6");
            keys.push("selectaddress.aspx.12");
            keys.push("selectaddress.aspx.13");

            break;
        case "address":

            keys.push("selectaddress.aspx.8");
            keys.push("selectaddress.aspx.9");

            break;
        case "one-page-checkout":

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

            break;
        case "case-history":

            keys.push("customersupport.aspx.44");
            keys.push("customersupport.aspx.45");
            keys.push("customersupport.aspx.46");
            keys.push("customersupport.aspx.47");
            keys.push("customersupport.aspx.48");

            break;
        default: break;
    }

    if (includeBubbleTipsMessages) {

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

    }

    var jsonText = jsonText = JSON.stringify({ keys: keys });

    $.ajax({
        type: "POST",
        url: "ActionService.asmx/GetStringResources",
        dataType: "json",
        data: jsonText,
        contentType: "application/json; charset=utf-8",
        success: function (result) {
            PopulateResources(false, result.d);
        },
        error: function (result) { console.log(result.d); }
    });

}

function IsEmailGood(email) {
    var emailPattern = /(^[a-zA-Z0-9._-]+@[a-zA-Z0-9]+([.-]?[a-zA-Z0-9]+)?([\.]{1}[a-zA-Z]{2,4}){1,50}$)/;
    return emailPattern.test(email);
}

function GetInputValue(id) {
    return $.trim($("#" + id).val());
}

