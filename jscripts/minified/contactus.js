$(document).ready(function () { $(this).ContactUs.Initialize() });
(function (c) {
    function b(a) { if (a == d.EMPTY_VALUE) return a; -1 == a.indexOf(d.DOT_VALUE) && (a = "#" + a); return a } var h = 1, g = {}, d = { EMPTY_VALUE: "", DOT_VALUE: "." }, m = {
        sendMessageButtonId: "send-message", sendMessageButtonPlaceHolderId: "contact-form-button-place-holder", sendMessageProgressPlaceHolderId: "sending-message-progress-place-holder", sendMessageErrorPlaceHolderId: "sending-message-error-place-holder", securityCodeRefreshButtonId: "captcha-refresh-button", messages: { MESSAGE_SENDING_PROGRESS: "Sending..." }, confactFormControls: {
            CONTACT_NAME_ID: "txtContactName",
            EMAIL_ADDRESS_ID: "txtEmail", PHONE_ID: "txtContactNumber", SUBJECT_ID: "txtSubject", MESSAGE_DETAILS_ID: "txtMessageDetails", CAPTCHA_ID: "txtCaptcha"
        }, bubbleMessagePlaceHolderId: "ise-message-tips", bubbleMessageRequiresValidationClass: ".requires-validation", requiredInputClass: "required-input", objectOnFocusClass: "current-object-on-focus"
    }, n = c.prototype.init; c.prototype.init = function (a, c) {
        var b = n.apply(this, arguments); a && a.selector && (b.context = a.context, b.selector = a.selector); "string" == typeof a && (b.context =
        c || document, b.selector = a); return b
    }; c.prototype.init.prototype = c.prototype; c.fn.ContactUs = {
        Initialize: function (a) { g = c.extend(m, a); this.attachEventsListener(); this.initializedBubbleMessage(); this.setBubbleMessageRequiredStringResources() }, initializedBubbleMessage: function () {
            var a = g, k = a.confactFormControls.CONTACT_NAME_ID, f = a.confactFormControls.EMAIL_ADDRESS_ID, e = a.confactFormControls.PHONE_ID, l = a.confactFormControls.SUBJECT_ID, d = a.confactFormControls.MESSAGE_DETAILS_ID, a = a.confactFormControls.CAPTCHA_ID;
            c(b(k)).ISEBubbleMessage({ "input-id": k, "label-id": "lblContactName" }); c(b(f)).ISEBubbleMessage({ "input-id": f, "label-id": "lblEmail", "input-mode": "email" }); c(b(e)).ISEBubbleMessage({ "input-id": e, "label-id": "lblContactNumber" }); c(b(l)).ISEBubbleMessage({ "input-id": l, "label-id": "lblSubject" }); c(b(d)).ISEBubbleMessage({ "input-id": d, "label-id": "lblMessageDetails" }); c(b(a)).ISEBubbleMessage({ "input-id": a, "label-id": "lblCaptcha" })
        }, attachEventsListener: function () {
            var a = g; c(b(a.securityCodeRefreshButtonId)).unbind("click");
            c(b(a.securityCodeRefreshButtonId)).click(function () { h++; c("#captcha").attr("src", "Captcha.ashx?id=" + h) })
        }, setBubbleMessageRequiredStringResources: function () { var a = []; a.push("customersupport.aspx.15"); a.push("customersupport.aspx.16"); a.push("customersupport.aspx.18"); ise.StringResource.loadResources(a, function () { }) }, validate: function () {
            var a = g, d = !1, f = 0; c(b(a.bubbleMessagePlaceHolderId)).fadeOut("slow"); c(b(a.bubbleMessageRequiresValidationClass)).each(function () {
                var b = c(this); "" == b.val() && (b.removeClass(a.objectOnFocusClass),
                b.addClass(a.requiredInputClass), 0 == f && (b.addClass(a.objectOnFocusClass), b.focus()), d = !0, f++)
            }); var e = c(b(a.confactFormControls.EMAIL_ADDRESS_ID)); return d ? !1 : e.hasClass("invalid-email") || e.hasClass("email-duplicates") ? (e.focus(), !1) : !0
        }
    }
})(jQuery); function formInfoIsGood() { return $(this).ContactUs.validate() };